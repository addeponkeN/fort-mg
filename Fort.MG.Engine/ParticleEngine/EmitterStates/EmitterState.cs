namespace Fort.MG.ParticleEngine.EmitterStates;

public abstract class EmitterState
{
    protected ParticleEmitter Emitter;

    public virtual void Init(ParticleEmitter particleEmitter)
    {
        Emitter = particleEmitter;
    }

    public virtual void Update(Particle particle)
    {
    }
}