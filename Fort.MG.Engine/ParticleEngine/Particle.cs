using Fort.Utility;
using Microsoft.Xna.Framework;

namespace Fort.MG.ParticleEngine;

public class Particle
{
	public Vector2 Position;
	public float Z;
	public Vector2 Size;
	public float Rotation;
	public float Life;
	public Vector2 Direction;
	public float Speed;
	public Color Color;

	private float _zacceleration;
	private float _rotacceleration;
	private float _acceleration;

	private float M => Rng.NextFloat(-1f, 1f);

	public void Init(ParticleData d)
	{
		Life = d.LifeTime + d.LifeTimeRange * M;
		Rotation = 0f;
		Z = 1;
		Size = new Vector2(MathF.Max(1, d.Size.X + d.SizeRange.X * M),
			MathF.Max(1, d.Size.X + d.SizeRange.X * M));
		Direction = d.Direction + new Vector2(d.DirectionRange.X * M, d.DirectionRange.Y * M);
		if (Direction != Vector2.Zero)
			Direction.Normalize();
		Speed = d.Speed + d.SpeedRange * M;
		Color = new Color(
			(int)(d.ColorRange.R * M),
			(int)(d.ColorRange.G * M),
			(int)(d.ColorRange.B * M),
			(int)(d.ColorRange.A * M));

		_acceleration = d.Acceleration + d.AccelerationRange * M;
		_zacceleration = d.ZAcceleration + d.ZAccelerationRange * M;
		_rotacceleration = d.RotationAcceleration + d.RotationAccelerationRange * M;

	}


	public void Update(ParticleData data, float dt)
	{
		Life -= dt;
		Speed += _acceleration * dt;
		Position += Direction * Speed * dt;
		Rotation += _rotacceleration * dt;

		Z += _zacceleration * dt;
		if (data.Gravity > 0)
		{
			_zacceleration -= data.Gravity * dt;
			if (Z <= 0.001f)
			{
				Z = 0;
				_zacceleration *= -1f * data.Bounce;
				Speed *= Speed > 10 ? 0.5f : 0f;
				_rotacceleration *= _rotacceleration > 0.3f ? 0.5f : 0f;
			}
		}
	}
}