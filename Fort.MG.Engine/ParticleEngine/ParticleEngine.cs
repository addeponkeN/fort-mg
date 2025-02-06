using Fort.MG.EntitySystem;
using Fort.MG.Systems;
using Fort.MG.Utils;

namespace Fort.MG.ParticleEngine;

public class ParticleEngine : EngineSystem, IRenderable
{
    List<ParticleEmitter> _emitters;

    public ParticleEngine()
    {
        _emitters = new List<ParticleEmitter>();
    }

    public void AddEmitter(string name)
    {
        
    }
    
    public void AddEmitter(int type)
    {
        
    }
    
    public void AddEmitter(ParticleEmitter em)
    {
        _emitters.Add(em);
    }

    public void Clear()
    {
        _emitters.Clear();
    }

    public override void Update(IGameTime t)
    {
        base.Update(t);

        for(int i = _emitters.Count - 1; i >= 0; i--)
        {
            var e = _emitters[i];
            e.Update(t);
        }
        
    }

    public override void Draw()
    {
        base.Draw();

        for(int i = 0; i < _emitters.Count; i++)
            _emitters[i].Draw();
        
    }
}