using System.Text.Json.Serialization;

namespace Fort.MG.ParticleEngine;

public class EmitterData
{
    public float LifeTime;
    public float Size;

    public float SpawnRate;

    [JsonIgnore] public ParticleData Particle;
}