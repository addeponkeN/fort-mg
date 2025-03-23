using Fort.MG.Extensions;
using Fort.MG.Gui.Components;
using Microsoft.Xna.Framework;

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

internal class InteractiveComponent : ComponentBase
{
	private GuiComponent _guiParent;

	private readonly Color _clrHover = Color.White * 0.05f;
	private readonly Color _clrClick = Color.Black * 0.05f;

	private Color? _activeColor;

	public Outline Outline;

	public override void Start()
	{
		base.Start();
		_guiParent = (GuiComponent)Parent;

		Outline = new Outline(_guiParent, 1f, Color.Black);
	}

	public override void Update(GameTime gt)
	{
		base.Update(gt);

		if (_guiParent.IsPressed)
		{
			_activeColor = _clrClick;
		}
		else if (_guiParent.IsHovered)
		{
			_activeColor = _clrHover;
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

		Outline.Draw();
	}
}
