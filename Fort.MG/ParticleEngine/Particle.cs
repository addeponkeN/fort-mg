using Fort.Utility;
using Microsoft.Xna.Framework;

namespace Fort.MG.ParticleEngine;

public class Particle
{
	public Vector2 position;
	public float z;
	public Vector2 size;
	public float rotation;
	public float life;
	public Vector2 direction;
	public float speed;
	public Color color;

	private float zacceleration;
	private float rotacceleration;
	private float acceleration;

	private float m => Rng.NextFloat(-1f, 1f);

	public void Init(ParticleData d)
	{
		life = d.lifeTime + d.lifeTimeRange * m;
		rotation = 0f;
		z = 1;
		size = new Vector2(MathF.Max(1, d.size.X + d.sizeRange.X * m),
			MathF.Max(1, d.size.X + d.sizeRange.X * m));
		direction = d.direction + new Vector2(d.directionRange.X * m, d.directionRange.Y * m);
		if (direction != Vector2.Zero)
			direction.Normalize();
		speed = d.speed + d.speedRange * m;
		color = new Color(
			(int)(d.colorRange.R * m),
			(int)(d.colorRange.G * m),
			(int)(d.colorRange.B * m),
			(int)(d.colorRange.A * m));

		acceleration = d.acceleration + d.accelerationRange * m;
		zacceleration = d.zAcceleration + d.zAccelerationRange * m;
		rotacceleration = d.rotationAcceleration + d.rotationAccelerationRange * m;

	}


	public void Update(ParticleData data, float dt)
	{
		life -= dt;
		speed += acceleration * dt;
		position += direction * speed * dt;
		rotation += rotacceleration * dt;

		z += zacceleration * dt;
		if (data.gravity > 0)
		{
			zacceleration -= data.gravity * dt;
			if (z <= 0.001f)
			{
				z = 0;
				zacceleration *= -1f * data.bounce;
				speed *= speed > 10 ? 0.5f : 0f;
				rotacceleration *= rotacceleration > 0.3f ? 0.5f : 0f;
			}
		}
	}
}