using System.Text.Json.Serialization;

namespace Fort.MG.ParticleEngine;

public class EmitterData
{
    public float lifeTime;
    public float size;

    public float spawnRate;

    [JsonIgnore] public ParticleData particle;
}