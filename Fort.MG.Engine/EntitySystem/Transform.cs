using Microsoft.Xna.Framework;

namespace Fort.MG.EntitySystem;

public class Transform : Component
{
    private Vector3 _localPosition;
    private Vector3 _position;
    private bool _dirty = true;

    public Vector2 Position
    {
        get => new Vector2(_position.X, _position.Y);
        set
        {
            _position = new Vector3(value, _position.Z);
            _dirty = true;
        }
    }

    public Vector3 Position3
    {
        get => CalculateWorldPosition();
        set
        {
            if (Entity.Parent != null)
            {
                var parentTransform = Entity.Parent.Transform;
                if (parentTransform != null)
                {
                    _localPosition = value - parentTransform.Position3;
                }
                else
                {
                    _localPosition = value;
                }
            }
            else
            {
                _position = value;
            }
            _dirty = true;
        }
    }

    public Vector2 LocalPosition
    {
        get => new Vector2(_localPosition.X, _localPosition.Y);
        set
        {
            _localPosition = new Vector3(value, _localPosition.Z);
            _dirty = true;
        }
    }

    public Vector3 LocalPosition3
    {
        get => _localPosition;
        set
        {
            _localPosition = value;
            _dirty = true;
        }
    }

    public Vector2 Size { get; set; }
    //public Vector2 Scale { get; set; } = Vector2.One;

    internal Vector3 CalculateWorldPosition()
    {
        if (!_dirty)
            return _position;

        if (Entity?.Parent != null)
        {
            var parentTransform = Entity.Parent.Transform;
            if (parentTransform != null)
            {
                _position = parentTransform.Position3 + _localPosition;
            }
            else
            {
                _position = _localPosition;
            }
        }
        else
        {
            //_position = _localPosition;
        }
        _dirty = false;
        return _position;
    }
}
