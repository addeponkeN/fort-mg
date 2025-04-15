using Fort.MG.Extensions;
using Fort.MG.Gui.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Fort.MG.Gui.Controls;

public struct Outline
{
	public GuiComponent Target;
	public float Size;
	public Color Color;
	public bool IsEnabled;

	public Outline(GuiComponent target, float size = 1f, Color? color = null)
	{
		Target = target;
		Size = size;
		Color = color ?? new Color(200, 200, 200, 255);
		IsEnabled = true;
	}
	public void Draw()
	{
		if (!IsEnabled) return;
		Target.Bounds.DrawLined(Color, Size);
	}
}

public class InteractiveComponent : ComponentBase
{
	public Color HoverColor { get; set; } = Color.White * 0.05f;
	public Color ClickColor { get; set; } = Color.Black * 0.05f;

	private GuiComponent _guiParent;
	private Color? _activeColor;

	public Outline Outline;

	public override ComponentBase Parent
	{
		get => base.Parent;
		set
		{
			base.Parent = value;
			_guiParent = (GuiComponent)value;
			Outline = new Outline(_guiParent, 1f, Color.White * 0.1f);
		}
	}

	public Texture2D? Texture { get; set; }
	public Rectangle Source { get; set; }

	public override void Update(GameTime gt)
	{
		base.Update(gt);

		if (_guiParent.IsPressed)
		{
			_activeColor = ClickColor;
		}
		else if (_guiParent.IsHovered)
		{
			_activeColor = HoverColor;
		}
		else
		{
			_activeColor = null;
		}
	}

	public override void Draw()
	{
		base.Draw();

		if (_activeColor.HasValue)
		{
			foreach (var skin in _guiParent.Skins)
			{
				var txWhite = GuiContent.GetWhiteTexture(skin.Texture);
				skin.Draw(txWhite, _activeColor.Value);
			}
		}

		if (_guiParent.IsHovered)
			Outline.Draw();
	}
}
