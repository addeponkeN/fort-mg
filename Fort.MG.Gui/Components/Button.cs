using Fort.MG.Extensions;
using Fort.MG.Gui.Controls;
using Microsoft.Xna.Framework;

namespace Fort.MG.Gui.Components;

public class Button : GuiComponent
{
	private readonly Label _title;

	public string Text
	{
		get => _title.Text;
		set
		{
			_title.Text = value;
			IsPositionDirty = true;
		}
	}

	public override Vector2 Position
	{
		get => base.Position;
		set
		{
			base.Position = value;
			_title.Position = GHelper.Center(value, _title.Size, Size);
		}
	}

	public override Vector2 Size
	{
		get => base.Size == Vector2.Zero ? _title.Size + new Vector2(8) : base.Size;
		set => base.Size = Vector2.Clamp(value, MinSize, value);
	}

	public Vector2 MinSize { get; set; } = new Vector2(96, 24);

	public Button()
	{
		_title = new();

		Size = MinSize;
	}

	public override void Start()
	{
		base.Start();
		AddComponent(new InteractiveComponent());
		if (Skins.Count == 0)
			AddSkin(new Skin());
		_title.Canvas = Canvas;
	}

	public override void Update(GameTime gt)
	{
		base.Update(gt);
		_title.Update(gt);
	}

	public override void Draw()
	{
		base.Draw();
	}

	public override void DrawText()
	{
		base.DrawText();
		_title.DrawText();
	}
}
