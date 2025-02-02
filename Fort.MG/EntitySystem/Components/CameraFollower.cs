using Fort.MG.Utils;
using Microsoft.Xna.Framework;

namespace Fort.MG.EntitySystem.Components;

public class CameraFollower : Component
{
	private Camera _cam;

	public Transform? Target;
	public float Speed = 10f;
	public float MinDistance = 1f;

	public override void OnAdded()
	{
		base.OnAdded();
		_cam = GetComponent<Camera>();
	}

	public void SetTarget(Transform target)
	{
		Target = target;
		_cam.Transform.Position = Target.LocalPosition;
	}

	public override void Update(IGameTime t)
	{
		base.Update(t);

		if (Target == null) return;

		var center = new Vector2(Target.Position.X + Target.Size.X * .5f, Target.Position.Y + Target.Size.Y * .5f);
		var distance = Vector2.Distance(_cam.Transform.Position, center);
		if (distance > MinDistance)
		{
			var dir = Vector2.Normalize(center - _cam.Transform.Position);
			_cam.Transform.Position += dir * Speed * distance * t.Delta;
		}
	}
}