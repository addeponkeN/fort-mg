using Fort.MG.Assets;
using Fort.MG.EntitySystem;
using Fort.MG.ParticleEngine.EmitterStates;
using Fort.MG.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OboEngine.ParticleEngine;

namespace Fort.MG.ParticleEngine;

public class ParticleEmitter : Entity
{
    public int Type;

    List<Particle> particles;
    AtlasRegion _atlasRegion;

    List<EmitterState> states;

    internal ParticleData d;
    internal EmitterData e;

    float rateTimer;

    public ParticleEmitter()
    {
        particles = new List<Particle>();
        states = new List<EmitterState>();
    }

    public void AddState(EmitterState state)
    {
        state.Init(this);
        states.Add(state);
    }

    public override void Init()
    {
        base.Init();
    }

    public void Init(EmitterData data)
    {
        this.e = data;
        d = data.particle;
        _atlasRegion = Content.Get.Pixel;
    }

    void SpawnParticle()
    {
        var p = PoolManager<Particle>.Spawn();
        p.position = GRng.PositionInRadius(e.size, Transform.LocalPosition);
        p.Init(d);
        particles.Add(p);
    }

    public override void Update(IOboTime t)
    {
        base.Update(t);
        var dt = t.Delta;
        rateTimer += dt;
        if(rateTimer >= e.spawnRate)
        {
            rateTimer -= e.spawnRate;
            SpawnParticle();
        }

        for(int i = 0; i < particles.Count; i++)
        {
            var p = particles[i];
            p.Update(d, dt);
            for(int j = 0; j < states.Count; j++)
                states[i].Update(p);
            if(p.life < 0)
            {
                particles.RemoveAt(i--);
                PoolManager<Particle>.Free(p);
            }
        }
    }

    static Color shadowColor = new Color(1, 1, 1, 50);

    public override void Draw()
    {
        base.Draw();
        for(int i = 0; i < particles.Count; i++)
        {
            var p = particles[i];
            var scale = new Vector2(p.size.X / _atlasRegion.Frame.Width, p.size.Y / _atlasRegion.Frame.Height);
            var orig = new Vector2(_atlasRegion.Frame.Width / 2f, _atlasRegion.Frame.Height / 2f);
            var pos = new Vector2(p.position.X, p.position.Y - p.z);
            if(d.shadow)
                Graphics.SpriteBatch.Draw(_atlasRegion, p.position, _atlasRegion, shadowColor, p.rotation, orig, scale,
                    SpriteEffects.None, 0.5f);
            Graphics.SpriteBatch.Draw(_atlasRegion, pos, _atlasRegion, p.color, p.rotation, orig, scale,
                SpriteEffects.None, 0.5f);
        }
    }
}