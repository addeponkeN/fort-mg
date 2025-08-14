using Microsoft.Xna.Framework;

namespace Fort.MG.EntitySystem.Parsing;

public class TransformData
{
	public Vector3 Position { get; set; } = Vector3.Zero;
	public Vector3 LocalPosition { get; set; } = Vector3.Zero;
	public Vector2 Size { get; set; } = Vector2.Zero;
	public float Rotation { get; set; } = 0f;
	public Vector2 Scale { get; set; } = Vector2.One;
}