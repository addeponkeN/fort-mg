using Fort.MG.ParticleEngine.EmitterStates;
using Microsoft.Xna.Framework;

namespace Fort.MG.ParticleEngine;

public class ParticleData
{
    public string texture;
    
    public Vector2 size;
    public Vector2 sizeRange;
    public Vector2 positionRange;
    public Vector2 direction;
    public Vector2 directionRange;
    public float z;
    public float zRange;

    public float speed;
    public float speedRange;
    
    public float acceleration;
    public float zAcceleration;
    public float rotationAcceleration;
    
    public float rotationAccelerationRange;
    public float accelerationRange;
    public float zAccelerationRange;

    public float lifeTime;
    public float lifeTimeRange;
    public float startTime;
    public float endTime;

    public Color colorRange;
    public Color color;
    public Color startColor;
    public Color endColor;

    public float gravity;
    public float bounce;

    public bool shadow;

    public bool rotateToDirection;

    public EmitterState[] States;

    public ParticleData()
    {
        color = Color.White;
        startColor = Color.Transparent;
        endColor = Color.Transparent;
        lifeTime = 1;
    }
}