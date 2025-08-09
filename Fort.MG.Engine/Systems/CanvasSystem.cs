using Fort.MG.EntitySystem;
using Fort.MG.Gui;

namespace Fort.MG.Systems;

public class CanvasSystem : EngineSystem, IRenderable, IDrawableGui
{
	public Canvas Canvas { get; set; }

	public override void Init()
	{
		base.Init();
		Canvas = new();
	}

	public override void Update(IGameTime t)
	{
		base.Update(t);
		//Canvas.UpdateInput();
		Canvas.Update(FortEngine.Time);
	}

	public override void Render()
	{
		base.Render();
		Canvas.Render();
	}

	public void DrawGui()
	{
		Canvas.Draw();
	}
}
