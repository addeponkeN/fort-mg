using Fort.MG.Core;
using Fort.MG.Scenes;
using Fort.MG.Utils;
using Microsoft.Xna.Framework;

namespace Fort.MG;

public class FortGame : FortCoreGame
{
    public SceneManager SceneManager;

    public FortGame()
    {
        SceneManager = new SceneManager();
        Engine.Start(this);
    }

    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void LoadContent()
    {
        base.LoadContent();
        Engine.Load();
	}

    protected override void OnFirstFrame(GameTime gt)
    {
	    base.OnFirstFrame(gt);
        Engine.FirstFrameInit(gt);
    }

    protected override void Update(GameTime gt)
    {
        base.Update(gt);
        Engine.gt = gt;
    }

    protected override void UpdateGame(GameTime gt)
    {
	    base.UpdateGame(gt);

        Engine.Update();

        SceneManager.Update(Time.GetTimeManager());

        Engine.PostUpdate();
    }

    protected override void Render(GameTime gt)
    {
	    base.Render(gt);
        SceneManager.Render();
    }

    protected override void Draw(GameTime gt)
    {
        base.Draw(gt);
        Engine.DrawBegin();
        SceneManager.Draw();
        Engine.Draw();
        Engine.DrawEnd();
    }
}