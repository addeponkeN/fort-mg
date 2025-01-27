using Fort.MG.Utils;
using Microsoft.Xna.Framework;
using OboEngine.EntitySystem;

namespace Fort.MG.EntitySystem;

public class Transform : Component
{
    public Vector2 PositionZ => new Vector2(Position.X, Position.Y - Z);
    public Vector2 LocalPositionZ => new Vector2(LocalPosition.X, LocalPosition.Y - LocalZ);

    public Vector2 Size;
    public Vector2 Position;
    public float Z;
    public Vector2 LocalPosition;
    public float LocalZ;

    public override void Update(IGameTime t)
    {
        // base.Update();

        if(Parent.Parent != null)
        {
            Position.X = LocalPosition.X + Parent.Parent.Transform.Position.X;
            Position.Y = LocalPosition.Y + Parent.Parent.Transform.Position.Y;
            Z = LocalZ + Parent.Parent.Transform.Z;
        }
    }
}

