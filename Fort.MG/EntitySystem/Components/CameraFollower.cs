using Fort.MG.Utils;
using Microsoft.Xna.Framework;

namespace Fort.MG.EntitySystem.Components;

public class CameraFollower : Component
{
    public Transform Target;
    public float Speed;

    Camera cam;

    public CameraFollower(Transform target)
    {
        this.Target = target;
        Speed = 10f;
    }
    
    public CameraFollower(Entity target) : this(target.Transform)
    {
    }

    public override void Init()
    {
        base.Init();
        cam = Engine.Cam;
        cam.Transform.Position = Target.LocalPosition;
    }

    public override void Update(IGameTime t)
    {
        base.Update(t);
        var center = new Vector2(Target.Position.X + Target.Size.X * .5f, Target.Position.Y + Target.Size.Y * .5f);
        var distance = Vector2.Distance(cam.Transform.Position, center);
        if(distance > 1)
        {
            var dir = Vector2.Normalize(center - cam.Transform.Position);
            cam.Transform.Position += dir * Speed * distance * t.Delta;
        }
    }
}