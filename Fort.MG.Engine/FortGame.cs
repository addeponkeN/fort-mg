using Fort.MG.Scenes;
using Fort.MG.Utils;
using Microsoft.Xna.Framework;

namespace Fort.MG;

public class FortGame : FortCoreGame
{
    public SceneManager SceneManager { get; private set; }

    public FortGame()
    {
        SceneManager = new SceneManager();
        FortEngine.Start(this);
    }

    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void LoadContent()
    {
        base.LoadContent();
        FortEngine.Load();
	}

    protected override void OnFirstFrame(GameTime gt)
    {
	    base.OnFirstFrame(gt);
        FortEngine.FirstFrameInit(gt);
    }

    protected override void Update(GameTime gt)
    {
        FortEngine.Gt = gt;
        base.Update(gt);
    }

    protected override void UpdateGame(GameTime gt)
    {
	    base.UpdateGame(gt);

        FortEngine.Update();

        SceneManager.Update(Time.GetTimeManager());

        FortEngine.PostUpdate();
    }

    protected override void Render(GameTime gt)
    {
	    base.Render(gt);
        SceneManager.Render();
    }

    protected override void Draw(GameTime gt)
    {
        base.Draw(gt);
        FortEngine.DrawBegin();
        SceneManager.Draw();
        FortEngine.Draw();
        FortEngine.DrawEnd();
    }
}