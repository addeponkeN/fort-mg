using Fort.MG.ParticleEngine.EmitterStates;
using Microsoft.Xna.Framework;

namespace Fort.MG.ParticleEngine;

public class ParticleData
{
    public string Texture;
    
    public Vector2 Size;
    public Vector2 SizeRange;
    public Vector2 PositionRange;
    public Vector2 Direction;
    public Vector2 DirectionRange;
    public float Z;
    public float ZRange;

    public float Speed;
    public float SpeedRange;
    
    public float Acceleration;
    public float ZAcceleration;
    public float RotationAcceleration;
    
    public float RotationAccelerationRange;
    public float AccelerationRange;
    public float ZAccelerationRange;

    public float LifeTime;
    public float LifeTimeRange;
    public float StartTime;
    public float EndTime;

    public Color ColorRange;
    public Color Color;
    public Color StartColor;
    public Color EndColor;

    public float Gravity;
    public float Bounce;

    public bool Shadow;

    public bool RotateToDirection;

    public EmitterState[] States;

    public ParticleData()
    {
        Color = Color.White;
        StartColor = Color.Transparent;
        EndColor = Color.Transparent;
        LifeTime = 1;
    }
}