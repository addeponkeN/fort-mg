using Microsoft.Xna.Framework;

namespace Fort.MG.EntitySystem.Components;

public class Body : Component
{
	public Vector2 Velocity;
	public float Mass = 1f;

	public void ApplyMovement(Vector2 movement)
	{
		Transform.Position3 += new Vector3(movement, 0f);
	}

	public override void Update(IGameTime t)
	{
		if (Velocity != Vector2.Zero)
		{
			ApplyMovement(Velocity * t.Delta);
		}
	}
}
