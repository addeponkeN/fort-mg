namespace Fort.MG.ParticleEngine.EmitterStates;

public abstract class EmitterState
{
    protected ParticleEmitter emitter;

    public virtual void Init(ParticleEmitter particleEmitter)
    {
        emitter = particleEmitter;
    }

    public virtual void Update(Particle particle)
    {
    }
}