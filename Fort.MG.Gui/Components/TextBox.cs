using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Fort.MG.Gui.Components;

public class TextBox : GuiComponent
{
	public EditLabel EditLabel
	{
		get => _editLabel;
		set => SetComponent(ref _editLabel, value);
	}

	private float _scrollOffset = 0; // How much the text has been scrolled
	private float _visibleWidth; // Width of the text area
	private float _cursorXPosition; // Cursor position relative to text
	private float _padding = 4; // Space between text and box edges
	private EditLabel _editLabel;

	public override bool IsFocused
	{
		get => _editLabel.IsFocused;
		set => _editLabel.IsFocused = value;
	}

	public TextBox()
	{
		IsFocusable = true;
		base.Size = new Vector2(200, 20);
		base.Foreground = new Color(200, 200, 200, 255);
	}

	public override void Start()
	{
		base.Start();
		EditLabel = new EditLabel
		{
			Position = Position + new Vector2(_padding, 0),
			Size = new Vector2(Size.X - 2 * _padding, Size.Y),
			Foreground = Foreground
		};

		EditLabel.OnTextChanged += EditLabelOnOnTextChanged;

		_visibleWidth = Size.X - 2 * _padding;
	}

	private void EditLabelOnOnTextChanged(string text)
	{
		UpdateScroll();
	}

	public override void Update(GameTime gt)
	{
		base.Update(gt);
		UpdateScroll();
	}

	private void UpdateScroll()
	{
		_cursorXPosition = EditLabel.GetCursorXPosition();

		if (_cursorXPosition - _scrollOffset > _visibleWidth)
		{
			_scrollOffset = _cursorXPosition - _visibleWidth;
		}
		else if (_cursorXPosition - _scrollOffset < 0)
		{
			_scrollOffset = _cursorXPosition;
		}

		EditLabel.Position = Position + new Vector2(_padding - _scrollOffset, 0);
	}

	private RasterizerState _rasterizer = new RasterizerState { ScissorTestEnable = true };

	public override void DrawContent()
	{
		base.DrawContent();

		var sb = Graphics.SpriteBatch;
		var gd = Graphics.GraphicsDevice;

		Rectangle previousScissorRect = gd.ScissorRectangle;
		RasterizerState originalRasterizerState = gd.RasterizerState;

		gd.ScissorRectangle = Bounds;
		gd.RasterizerState = _rasterizer;

		sb.Begin(SpriteSortMode.Deferred, Canvas.BlendState, Canvas.SamplerState, null, gd.RasterizerState);
		sb.Draw(GuiContent.Pixel, Bounds, Style.Background);
		base.Draw();
		sb.End();

		sb.Begin(samplerState: SamplerState.AnisotropicClamp, rasterizerState: gd.RasterizerState);
		base.DrawText();
		sb.End();

		gd.ScissorRectangle = previousScissorRect;
		gd.RasterizerState = originalRasterizerState;
	}

	public override void Draw()
	{

	}

	public override void DrawText()
	{

	}
}