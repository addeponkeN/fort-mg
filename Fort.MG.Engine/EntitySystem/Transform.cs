using Microsoft.Xna.Framework;

namespace Fort.MG.EntitySystem;

public class Transform : Component
{
    private Vector3 _localPosition = Vector3.Zero;  // local space
    private Vector3 _worldPosition = Vector3.Zero;  // cached
    private bool _dirty = true;

    public Transform? Parent => Entity.Parent?.Transform;

    // ------------------------
    // Public API
    // ------------------------

    public Vector2 Position
    {
        get
        {
            CalculateWorldPosition();
            return new Vector2(_worldPosition.X, _worldPosition.Y);
        }
        set => Position3 = new Vector3(value, _worldPosition.Z);
    }

    public Vector3 Position3
    {
        get => CalculateWorldPosition();
        set
        {
            if (Parent != null)
            {
                // Convert world → local
                _localPosition = value - Parent.Position3;
            }
            else
            {
                // Root entity → world = local
                _localPosition = value;
            }

            MarkDirty();
        }
    }

    public Vector2 LocalPosition
    {
        get => new(_localPosition.X, _localPosition.Y);
        set => LocalPosition3 = new Vector3(value, _localPosition.Z);
    }

    public Vector3 LocalPosition3
    {
        get => _localPosition;
        set
        {
            _localPosition = value;
            MarkDirty();
        }
    }

    public override void Start()
    {
        base.Start();
        MarkDirty();
    }

    public Vector2 Size { get; set; }

    // ------------------------
    // Dirty propagation
    // ------------------------

    private void MarkDirty()
    {
        if (_dirty) return;

        _dirty = true;

        // Notify children that world position must be recalculated
        if (Entity._children != null)
            foreach (var child in Entity._children)
            {
                child.Transform.MarkDirty();
            }
    }

    // ------------------------
    // Calculate world position
    // ------------------------

    internal Vector3 CalculateWorldPosition()
    {
        if (!_dirty)
            return _worldPosition;

        if (Parent != null)
        {
            _worldPosition = Parent.CalculateWorldPosition() + _localPosition;
        }
        else
        {
            _worldPosition = _localPosition; // Root transform
        }

        _dirty = false;
        return _worldPosition;
    }
}
