using Fort.MG.Assets.Data;
using Fort.MG.Core;
using Fort.MG.EntitySystem;
using Fort.MG.Extensions;
using Fort.MG.ParticleEngine.EmitterStates;
using Fort.MG.Utils;
using Fort.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Fort.MG.ParticleEngine;

public class ParticleEmitter : Entity
{
	private static readonly Color ShadowColor = new Color(1, 1, 1, 50);

	public int Type;

	private readonly List<Particle> _particles;
	private readonly List<EmitterState> _states;

	private SpriteRegion _spriteRegion;

    internal ParticleData D;
    internal EmitterData E;

    private float _rateTimer;

    public ParticleEmitter()
    {
        _particles = new List<Particle>();
        _states = new List<EmitterState>();
    }

    public void AddState(EmitterState state)
    {
        state.Init(this);
        _states.Add(state);
    }

    public override void Init()
    {
        base.Init();
    }

    public void Init(EmitterData data)
    {
        this.E = data;
        D = data.particle;
        _spriteRegion = Engine.AssetManager.Pixel;
    }

    private void SpawnParticle()
    {
        var p = PoolManager<Particle>.Spawn();
        p.position = RngMG.PositionInRadius(E.size, Transform.LocalPosition);
        p.Init(D);
        _particles.Add(p);
    }

    public override void Update(IGameTime t)
    {
        base.Update(t);
        var dt = t.Delta;
        _rateTimer += dt;
        if(_rateTimer >= E.spawnRate)
        {
            _rateTimer -= E.spawnRate;
            SpawnParticle();
        }

        for(int i = 0; i < _particles.Count; i++)
        {
            var p = _particles[i];
            p.Update(D, dt);
            for(int j = 0; j < _states.Count; j++)
                _states[i].Update(p);
            if(p.life < 0)
            {
                _particles.RemoveAt(i--);
                PoolManager<Particle>.Free(p);
            }
        }
    }

    public override void Draw()
    {
        base.Draw();
        for(int i = 0; i < _particles.Count; i++)
        {
            var p = _particles[i];
            var scale = new Vector2(p.size.X / _spriteRegion.Frame.Width, p.size.Y / _spriteRegion.Frame.Height);
            var orig = new Vector2(_spriteRegion.Frame.Width / 2f, _spriteRegion.Frame.Height / 2f);
            var pos = new Vector2(p.position.X, p.position.Y - p.z);
            if(D.shadow)
                Graphics.SpriteBatch.Draw(_spriteRegion, p.position, _spriteRegion, ShadowColor, p.rotation, orig, scale,
                    SpriteEffects.None, 0.5f);
            Graphics.SpriteBatch.Draw(_spriteRegion, pos, _spriteRegion, p.color, p.rotation, orig, scale,
                SpriteEffects.None, 0.5f);
        }
    }
}