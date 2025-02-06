namespace Fort.MG.Utils;

public struct Life
{
    float _lifeTime;

    public float LifeTime
    {
        get => _lifeTime;
        set => _lifeTime = Timer = value;
    }

    public float IntroTime;
    public float OutTime;

    public float InLerp => (LifeTime - Timer) / IntroTime;
    public float OutLerp => Timer / OutTime;
    public float Lerp => Timer / LifeTime;

    public float Timer;

    public float GetCurrentLerp()
    {
        if(Timer > LifeTime - IntroTime)
        {
            return InLerp;
        }

        if(Timer < OutTime)
        {
            return OutLerp;
        }

        return 1f;
    }

    public float GetCurrentLerpReverted()
    {
        return 1f - GetCurrentLerp();
    }

    public Life(float lifeTime, float inTime, float outTime)
    {
        _lifeTime = lifeTime;
        Timer = lifeTime;
        IntroTime = inTime;
        OutTime = outTime;
    }

    public void Update(IGameTime t)
    {
        if(Timer >= 0)
        {
            Timer -= t.Delta;
            if(Timer < 0f)
                Timer = 0f;
        }
    }

    public static implicit operator Life(float lifeTime) => new(lifeTime, .01f, .01f);
    public static implicit operator float(Life life) => life.Timer;
    public static implicit operator bool(Life life) => life.LifeTime <= 0 || life.Timer > 0;
}