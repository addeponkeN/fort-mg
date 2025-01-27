using Fort.MG.EntitySystem;
using Fort.MG.Systems;
using OboEngine.ParticleEngine;

namespace Fort.MG.ParticleEngine;

public class ParticleEngine : EngineSystem, IRenderable
{
    List<ParticleEmitter> emitters;

    public ParticleEngine()
    {
        emitters = new List<ParticleEmitter>();
    }

    public void AddEmitter(string name)
    {
        
    }
    
    public void AddEmitter(int type)
    {
        
    }
    
    public void AddEmitter(ParticleEmitter em)
    {
        emitters.Add(em);
    }

    public void Clear()
    {
        emitters.Clear();
    }

    public override void Update(IOboTime t)
    {
        base.Update(t);

        for(int i = emitters.Count - 1; i >= 0; i--)
        {
            var e = emitters[i];
            e.Update(t);
        }
        
    }

    public override void Draw()
    {
        base.Draw();

        for(int i = 0; i < emitters.Count; i++)
            emitters[i].Draw();
        
    }
}