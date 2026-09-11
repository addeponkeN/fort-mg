using Fort.MG.Editor.Panels;
using Fort.MG.Gui;
using Fort.MG.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Fort.MG.Editor;

/// <summary>
/// Hosts the entity editor UI on its own <see cref="Canvas"/> and drives it from the engine's system
/// pipeline. Registered once via <see cref="FortEngine.RegisterSystem{T}"/> and toggled with
/// <see cref="ToggleKey"/>; while closed it updates and draws nothing, so it never consumes gameplay
/// input or frame time.
///
/// Draw contract: <see cref="Canvas"/> owns a render target and issues its own
/// <c>SpriteBatch.Begin/End</c>, so <see cref="RenderGui"/> and <see cref="DrawGui"/> must be called
/// from a point where no batch is open (the same place a scene draws its canvas). The system is
/// intentionally NOT wired through <see cref="IFortDrawableGui"/>, because the global
/// <c>EngineSystemManager.DrawGui</c> pass runs inside an ambient batch.
/// </summary>
public class EntityEditorSystem : EngineSystem
{
	public const Keys ToggleKey = Keys.F1;

	private readonly List<EditorPanel> _panels = new();

	public Canvas Canvas { get; private set; } = null!;

	public EntityEditorSession Session { get; } = new();

	/// <summary>Gizmo viewport for the working tree (selection + drag).</summary>
	public EditorPreview Preview { get; private set; } = null!;

	public bool IsOpen { get; private set; }

	public override void Init()
	{
		base.Init();

		Canvas = new Canvas();

		AddPanel(new ToolbarPanel(Session, new Vector2(16f, 16f)));
		AddPanel(new TemplateBrowserPanel(Session, new Vector2(16f, 100f)));
		AddPanel(new HierarchyPanel(Session, new Vector2(250f, 100f)));
		AddPanel(new InspectorPanel(Session, new Vector2(505f, 100f)));

		// Sits to the right of the panels so it never draws over them: the canvas draws its
		// non-window items after the windows.
		Preview = new EditorPreview(Session, new Vector2(900f, 100f), new Vector2(360f, 460f));
		Canvas.AddItem(Preview);

		Session.NewTemplate("untitled");
		SetOpen(false);
	}

	private void AddPanel(EditorPanel panel)
	{
		_panels.Add(panel);
		Canvas.AddItem(panel.Window);
	}

	public override void Update(IGameContext t)
	{
		base.Update(t);

		if (Input.KeyClick(ToggleKey))
			SetOpen(!IsOpen);

		if (!IsOpen)
			return;

		// Panel refresh logic; the panel widgets and the preview are updated by Canvas.Update.
		for (var i = 0; i < _panels.Count; i++)
			_panels[i].Update();

		Canvas.Update(FortEngine.Time);
	}

	/// <summary>Renders the editor canvas into its own target. Call while no SpriteBatch is active.</summary>
	public void RenderGui()
	{
		if (IsOpen)
			Canvas.Render();
	}

	/// <summary>Blits the editor canvas to the back buffer. Call while no SpriteBatch is active.</summary>
	public void DrawGui()
	{
		if (IsOpen)
			Canvas.Draw();
	}

	public void Toggle() => SetOpen(!IsOpen);

	private void SetOpen(bool open)
	{
		IsOpen = open;
		Canvas.IsVisible = open;
	}
}
