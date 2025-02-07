using Fort.MG.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Fort.MG.Gui.Components;

public class Skin
{
	public Texture2D Tex { get; set; }
	public Rectangle Source { get; set; }
}

public class Style
{
	public Color Background { get; set; } = new Color(200, 200, 200, 255);
	public Color Foreground { get; set; } = Color.White;
	public int BorderThickness { get; set; } = 1;
}

public class GuiComponent
{
	private Vector2 _totalPosition;
	private Vector2 _localPosition;
	private Vector2 _position;
	private Vector2 _size;
	private bool _started;

	internal Canvas _canvas;

	public virtual Canvas Canvas
	{
		get => _canvas;
		internal set => _canvas = value;
	}

	public GuiComponent Parent { get; set; }
	public Skin Skin { get; set; }
	public Style Style { get; set; } = new();

	public Rectangle Bounds { get; private set; }
	public int Id { get; set; }
	public string Name { get; set; }

	internal bool IsPositionDirty;

	public virtual Color Foreground
	{
		get => Style.Foreground;
		set => Style.Foreground = value;
	}

	public Vector2 LocalPosition
	{
		get => _localPosition;
		set
		{
			_localPosition = value;
			UpdateTransforms();
		}
	}

	public virtual Vector2 Position
	{
		get => _totalPosition;
		set
		{
			_position = value;
			UpdateTransforms();
			IsPositionDirty = true;
		}
	}

	public virtual Vector2 Size
	{
		get => _size;
		set
		{
			_size = value;
			UpdateTransforms();
		}
	}

	private void UpdateTransforms()
	{
		_totalPosition = _position + _localPosition;
		var pos = Position + LocalPosition;
		Bounds = new Rectangle((int)pos.X, (int)pos.Y, (int)Size.X, (int)Size.Y);
		IsPositionDirty = true;
	}

	public virtual void Start()
	{
		_started = true;
	}

	public virtual void UpdateInput()
	{
	}

	public virtual void Update(GameTime gt)
	{
		if (!_started)
		{
			Start();
			_started = true;
		}
	}

	public virtual void Draw()
	{
		Bounds.DrawLined(Color.MonoGameOrange);
	}
}

public class Container : GuiComponent
{
	public List<GuiComponent> Items { get; set; } = new();

	public override Canvas Canvas
	{
		get => _canvas;
		internal set
		{
			_canvas = value;
			foreach (var item in Items)
				item._canvas = _canvas;
		}
	}

	public float Spacing { get; set; }
	public Vector2 Padding { get; set; }
	public bool AutoSize { get; set; } = true;

	public override Vector2 Position
	{
		get => base.Position;
		set
		{
			if (base.Position == value)
				return;
			base.Position = value;
			UpdateItemTransforms();
		}
	}

	public Container()
	{
		Spacing = 4;
		Padding = new Vector2(4);
	}

	protected float GetTotalHeight(int i)
	{
		if (Items == null) return 0;

		float spacing = (i - 1) * Spacing;
		float totalItemHeight = 0f;
		for (int j = 0; j < i; j++)
		{
			totalItemHeight += Items[i].Size.Y;
		}

		return totalItemHeight + spacing;
	}

	protected virtual Vector2 GetItemTransform(GuiComponent item, int i)
	{
		return new Vector2(Position.X, Position.Y);
	}

	protected virtual void UpdateItemTransforms()
	{
		float sizeY = 0;
		for (int i = 0; i < Items.Count; i++)
		{
			var item = Items[i];
			item.Position = new Vector2(Position.X, Position.Y + sizeY);
			sizeY += item.Size.Y + Spacing;
		}
	}

	public virtual void Add(GuiComponent item)
	{
		item._canvas = Canvas;
		item.Parent = this;
		Items.Add(item);
		UpdateItemTransforms();
		UpdateSize();
	}

	public virtual void Remove(GuiComponent item)
	{
		Items.Remove(item);
		item.Parent = null;
		UpdateItemTransforms();
		UpdateSize();
	}

	protected void UpdateSize()
	{
		if (!AutoSize) return;
		if (Items.Count == 0) return;

		float maxWidth = 0;
		float totalHeight = Padding.Y * 2 + (Items.Count - 1) * Spacing;

		foreach (var item in Items)
		{
			maxWidth = Math.Max(maxWidth, item.Size.X);
			totalHeight += item.Size.Y;
		}

		Size = new Vector2(maxWidth + Padding.X * 2, totalHeight);
	}

	public override void Update(GameTime gt)
	{
		base.Update(gt);
		for (int i = 0; i < Items.Count; i++)
		{
			var item = Items[i];
			item.Update(gt);
		}
	}

	protected void UpdateDirtyItems()
	{
		for (int i = 0; i < Items.Count; i++)
		{
			var item = Items[i];
			if (item.IsPositionDirty)
			{
				UpdateSize();
				UpdateItemTransforms();
				UpdateSize();
				break;
			}
		}
	}

	public override void Draw()
	{
		base.Draw();
		foreach (var item in Items)
		{
			item.Draw();
		}
	}
}

public class StackPanel : Container
{
	public Orientation ItemOrientation { get; set; } = Orientation.Vertical;
	public Alignment HorizontalAlignment { get; set; } = Alignment.Left;
	public Alignment VerticalAlignment { get; set; } = Alignment.Center;

	public override void Update(GameTime gt)
	{
		base.Update(gt);
		UpdateDirtyItems();
	}

	protected override void UpdateItemTransforms()
	{
		if (Items == null || Items.Count == 0) return;

		float offset = 0;

		foreach (var item in Items)
		{
			Vector2 alignedPosition = GetAlignedItemPosition(item, offset);

			item.Position = alignedPosition;

			offset += ItemOrientation == Orientation.Vertical
				? item.Size.Y + Spacing
				: item.Size.X + Spacing;

			item.IsPositionDirty = false;
		}
	}

	private Vector2 GetAlignedItemPosition(GuiComponent item, float offset)
	{
		float x = Position.X + Padding.X;
		float y = Position.Y + Padding.Y;

		if (ItemOrientation == Orientation.Vertical)
		{
			// Handle horizontal alignment
			switch (HorizontalAlignment)
			{
				case Alignment.Left:
					x = Position.X + Padding.X;
					break;
				case Alignment.Center:
					x = Position.X + (Size.X - item.Size.X) / 2;
					break;
				case Alignment.Right:
					x = Position.X + Size.X - item.Size.X - Padding.X;
					break;
			}

			// Handle vertical alignment for centering items within vertical orientation
			switch (VerticalAlignment)
			{
				case Alignment.Top:
					y = Position.Y + Padding.Y;
					break;
				case Alignment.Center:
					y = Position.Y + (Size.Y - GetTotalItemsHeight()) / 2;
					break;
				case Alignment.Bot:
					y = Position.Y + Size.Y - GetTotalItemsHeight() - Padding.Y;
					break;
			}

			return new Vector2(x, y + offset);
		}
		else // Horizontal orientation
		{
			// Handle vertical alignment
			switch (VerticalAlignment)
			{
				case Alignment.Top:
					y = Position.Y + Padding.Y;
					break;
				case Alignment.Center:
					y = Position.Y + (Size.Y - item.Size.Y) / 2;
					break;
				case Alignment.Bot:
					y = Position.Y + Size.Y - item.Size.Y - Padding.Y;
					break;
			}

			// Handle horizontal alignment
			switch (HorizontalAlignment)
			{
				case Alignment.Left:
					x = Position.X + Padding.X;
					break;
				case Alignment.Center:
					x = Position.X + (Size.X - GetTotalItemsWidth()) / 2;
					break;
				case Alignment.Right:
					x = Position.X + Size.X - GetTotalItemsWidth() - Padding.X;
					break;
			}

			return new Vector2(x + offset, y);
		}
	}

	private float GetTotalItemsHeight()
	{
		float totalHeight = Padding.Y * 2 + (Items.Count - 1) * Spacing;

		foreach (var item in Items)
		{
			totalHeight += item.Size.Y;
		}

		return totalHeight;
	}

	private float GetTotalItemsWidth()
	{
		float totalWidth = Padding.X * 2 + (Items.Count - 1) * Spacing;

		foreach (var item in Items)
		{
			totalWidth += item.Size.X;
		}

		return totalWidth;
	}

}


public enum Alignment
{
	Center,
	Left,
	Right,
	Top,
	Bot,
}

public enum Orientation
{
	Horizontal,
	Vertical,
}