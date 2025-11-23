using Microsoft.Xna.Framework.Graphics;
using Fort.MG.EntitySystem;

namespace Fort.MG.Rendering;

public sealed class RenderPass
{
    public SpriteSortMode SortMode { get; init; } = SpriteSortMode.BackToFront;
    public BlendState BlendState { get; init; } = BlendState.AlphaBlend;
    public SamplerState SamplerState { get; init; } = SamplerState.LinearClamp;
    public DepthStencilState DepthStencilState { get; init; } = null;
    public RasterizerState RasterizerState { get; init; } = null;
    public Effect Effect { get; init; } = null;
    public string Name { get; init; } = "Unnamed";
    public int Priority { get; set; } = 0;

    // Equality based on references and values so identical-config passes are deduped
    public override bool Equals(object? obj)
    {
        if (obj is not RenderPass other) return false;
        return SortMode == other.SortMode
               && BlendState == other.BlendState
               && SamplerState == other.SamplerState
               && DepthStencilState == other.DepthStencilState
               && RasterizerState == other.RasterizerState
               && Effect == other.Effect;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = (int)SortMode;
            hash = (hash * 397) ^ (BlendState?.GetHashCode() ?? 0);
            hash = (hash * 397) ^ (SamplerState?.GetHashCode() ?? 0);
            hash = (hash * 397) ^ (DepthStencilState?.GetHashCode() ?? 0);
            hash = (hash * 397) ^ (RasterizerState?.GetHashCode() ?? 0);
            hash = (hash * 397) ^ (Effect?.GetHashCode() ?? 0);
            return hash;
        }
    }
}

public sealed class RenderPassBucket
{
    public RenderPass Pass { get; }
    public List<IFortRenderable> Renderables { get; } = new();

    public RenderPassBucket(RenderPass pass) => Pass = pass;
}
