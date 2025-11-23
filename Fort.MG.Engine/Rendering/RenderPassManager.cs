using Fort.MG.EntitySystem;
using Microsoft.Xna.Framework.Graphics;

namespace Fort.MG.Rendering;

internal static class RenderableDepthComparers
{
    public static readonly Comparison<IFortRenderable> Ascending =
        (a, b) => a.DrawLayer.CompareTo(b.DrawLayer);

    public static readonly Comparison<IFortRenderable> Descending =
        (a, b) => b.DrawLayer.CompareTo(a.DrawLayer);
}

public sealed class PooledRenderPassBucket
{
    public RenderPass Pass;
    public readonly List<IFortRenderable> Renderables;

    public PooledRenderPassBucket(RenderPass pass)
    {
        Pass = pass;
        Renderables = new List<IFortRenderable>(64);
    }

    public void Clear()
    {
        Renderables.Clear();
    }
}

public sealed class RenderPassManager
{
    // Pool of buckets for reuse
    readonly Stack<PooledRenderPassBucket> _bucketPool = new();
    // Active buckets for this frame
    readonly List<PooledRenderPassBucket> _activeBuckets = new();

    // Map from RenderPass (as key) -> bucket for quick lookup (avoid allocations each frame by reusing dictionary)
    Dictionary<RenderPass, PooledRenderPassBucket> _map = new();

    // Dirty flag — external systems set this when renderables collection changes
    public bool IsDirty { get; private set; } = true;

    // Keep a stable comparer delegates to avoid allocations when passing to Sort
    readonly Comparison<IFortRenderable> _depthAsc = RenderableDepthComparers.Ascending;
    readonly Comparison<IFortRenderable> _depthDesc = RenderableDepthComparers.Descending;

    public RenderPassManager(int initialBucketCapacity = 16)
    {
        _map = new Dictionary<RenderPass, PooledRenderPassBucket>(initialBucketCapacity);
    }

    // Mark dirty when entity collection changes (Add/Remove), or when a large number of draw-layer changes happened
    public void MarkDirty() => IsDirty = true;

    public void CollectIntoBuckets(IEnumerable<IFortRenderable> renderables, List<PooledRenderPassBucket> resultBuckets)
    {
        // If we are dirty we rebuild. If not dirty, we reuse previously produced _activeBuckets.
        if (!IsDirty)
        {
            // copy active buckets references into resultBuckets, but do not clone underlying lists
            resultBuckets.Clear();
            resultBuckets.AddRange(_activeBuckets);
            return;
        }

        // reset & return
        for (int i = 0; i < _activeBuckets.Count; i++)
        {
            var b = _activeBuckets[i];
            b.Clear();
            _bucketPool.Push(b);
        }
        _activeBuckets.Clear();
        _map.Clear();

        foreach (var r in renderables)
        {
            if (!r.Enabled || r.IsDestroyed) continue;

            var pass = r.RenderPass ?? RenderPasses.Default;

            if (!_map.TryGetValue(pass, out var bucket))
            {
                if (_bucketPool.Count > 0)
                {
                    bucket = _bucketPool.Pop();
                    bucket.Pass = pass;
                }
                else
                {
                    bucket = new PooledRenderPassBucket(pass);
                }

                _map.Add(pass, bucket);
                _activeBuckets.Add(bucket);
            }

            bucket.Renderables.Add(r);
        }

        _activeBuckets.Sort((a, b) =>
        {
            // sort by Priority (lower = back, higher = front)
            int prio = a.Pass.Priority.CompareTo(b.Pass.Priority);
            if (prio != 0) 
                return prio;

            // 2. Pass sorting heuristic (depth-sorted passes first)
            int score(RenderPass p) =>
                (p.SortMode == SpriteSortMode.BackToFront ||
                 p.SortMode == SpriteSortMode.FrontToBack) ? 0 : 1;

            int sa = score(a.Pass);
            int sb = score(b.Pass);
            if (sa != sb) 
                return sa - sb;

            unchecked
            {
                return a.Pass.GetHashCode().CompareTo(b.Pass.GetHashCode());
            }
        });

        for (int i = 0; i < _activeBuckets.Count; i++)
        {
            var b = _activeBuckets[i];
            if (b.Renderables.Count == 0) continue;
            var mode = b.Pass.SortMode;
            if (mode == SpriteSortMode.BackToFront)
                b.Renderables.Sort(_depthDesc);
            else if (mode == SpriteSortMode.FrontToBack)
                b.Renderables.Sort(_depthAsc);

            // Deferred/Immediate/Texture: keep insertion order
        }

        resultBuckets.Clear();
        resultBuckets.AddRange(_activeBuckets);

        IsDirty = false;
    }

    // #todo: avoid full rebuild when track which bucket changed.
    public void MarkRenderableSortChanged(IFortRenderable r)
    {
        // for simplicity mark whole manager dirty, implement per-bucket partial sort todo
        IsDirty = true;
    }
}