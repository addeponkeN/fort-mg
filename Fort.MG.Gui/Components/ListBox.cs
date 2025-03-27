using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Fort.MG.Gui.Components;

public class ListBox : Container
{
	private readonly TextRenderer _t = new();
	private int _tIndex = 0;

	private readonly RasterizerState _rasterizer = new RasterizerState { ScissorTestEnable = true };
	private readonly List<GuiComponent> _visibleItems = new();

	private readonly byte _scrollRateLength = 100;
	private float _scrollOffset;
	private int _visibleItemStart;
	private int _visibleItemEnd;
	private float _totalHeight;

	public bool CanScroll => Items.Count > 0 && _totalHeight > Size.Y;

	public float ScrollRate { get; set; } = 5f;

	public override Vector2 Size
	{
		get => base.Size;
		set
		{
			base.Size = value;
			UpdateVisibleItems();
		}
	}

	public ListBox()
	{
		Size = new Vector2(160, 250);
		AutoSize = false;
	}

	public override void Start()
	{
		base.Start();

		if (Skins.Count == 0)
			AddSkin(new Skin());

		CalculateTotalHeight();
		UpdateVisibleItems();
	}

	protected override void UpdateItemTransforms()
	{
		CalculateTotalHeight();
		UpdateVisibleItems();
	}

	public override void Update(GameTime gt)
	{
		base.Update(gt);

		if (Input.WheelDown)
			Scroll(_scrollRateLength * ScrollRate / Size.Y);
		else if (Input.WheelUp)
			Scroll(-(_scrollRateLength * ScrollRate / Size.Y));

		foreach (var item in _visibleItems)
		{
			item.Update(gt);
		}
	}

	private void CalculateTotalHeight()
	{
		_totalHeight = 0;
		foreach (var item in Items)
		{
			_totalHeight += item.Size.Y + Spacing;
		}
		if (Items.Count > 0)
			_totalHeight -= Spacing;
	}

	private void UpdateVisibleItems()
	{
		foreach (var item in _visibleItems)
		{
			item.IsEnabled = false;
		}

		_visibleItems.Clear();
		float currentHeight = 0;
		_visibleItemStart = 0;
		_visibleItemEnd = Items.Count;

		float totalVisibleItemSize = 0f;

		for (int i = 0; i < Items.Count; i++)
		{
			var item = Items[i];
			float size = currentHeight + item.Size.Y + Spacing;
			currentHeight += item.Size.Y + Spacing;
			if (size >= _scrollOffset)
			{
				_visibleItemStart = i;
				break;
			}
			item.IsEnabled = false;
			totalVisibleItemSize += item.Size.Y + Spacing;
		}

		for (int i = _visibleItemStart; i < Items.Count; i++)
		{
			var item = Items[i];
			float size = currentHeight + item.Size.Y + Spacing;
			currentHeight += item.Size.Y + Spacing;
			if (size > _scrollOffset + Size.Y + Spacing + 10)
			{
				_visibleItemEnd = i + 1;
				break;
			}
		}

		float offset = -(_scrollOffset - totalVisibleItemSize);
		for (int i = _visibleItemStart; i < _visibleItemEnd; i++)
		{
			var item = Items[i];
			_visibleItems.Add(item);
			item.Position = new Vector2(Padding.X + Position.X, Padding.Y + Position.Y + offset);
			item.IsEnabled = IsEnabled;
			offset += item.Size.Y + Spacing;
		}
	}

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

		base.DrawSkins();
		base.DrawComponents();

		for (int i = 0; i < _visibleItems.Count; i++)
		{
			var item = _visibleItems[i];
			item.Draw();
		}

		sb.End();

		sb.Begin(samplerState: SamplerState.AnisotropicClamp, rasterizerState: gd.RasterizerState);
		foreach (var item in _visibleItems)
		{
			item.DrawText();
		}

		sb.End();
		//sb.Begin();
		//_tIndex = 0;
		//draw($"start: {_visibleItemStart}");
		//draw($"end: {_visibleItemEnd}");
		//draw($"scroll X: {_scrollOffset}");
		//sb.End();

		gd.ScissorRectangle = previousScissorRect;
		gd.RasterizerState = originalRasterizerState;
	}

	private void draw(string t)
	{
		var pos = Position + new Vector2(Size.X, _tIndex++ * 16);
		_t.Position = pos;
		_t.Text = t;
		_t.DrawText();
	}

	public override void Draw() { }
	public override void DrawText() { }

	public void Scroll(float direction)
	{
		if (!CanScroll) return;

		_scrollOffset = Math.Clamp(_scrollOffset + direction, 0, _totalHeight - Size.Y);
		UpdateVisibleItems();
	}

	public override void AddItem(GuiComponent item)
	{
		item._canvas = Canvas;
		item.Parent = this;
		Items.Add(item);
		CalculateTotalHeight();
		UpdateVisibleItems();
		item.IsEnabled = false;
	}

	public override void RemoveItem(GuiComponent item)
	{
		if (Items.Remove(item))
		{
			item.Parent = null;
			CalculateTotalHeight();
			UpdateVisibleItems();
		}
	}
}