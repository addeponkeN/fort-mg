using Fort.MG.EntitySystem;
using Fort.MG.Gui;
using Fort.MG.Utils;

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
		Canvas.UpdateInput();
		Canvas.Update(Engine.gt);
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
