using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Fort.MG.Gui.Components;

public class ThreeSlice : Skin
{
	private enum SliceRegionTypes { LeftOrTop, Center, RightOrBottom }
	public enum Orientation { Horizontal, Vertical }

	private readonly SliceRegion[] _regions = new SliceRegion[3];
	private bool _regionsDirty = true;

	public Orientation SliceOrientation { get; set; } = Orientation.Horizontal;
	public bool StretchCenter { get; set; } = true;
	public int SliceCornerSize { get; set; }

	public override Vector2 Size
	{
		get => base.Size;
		set
		{
			base.Size = value;
			_regionsDirty = true;
		}
	}

	public override Vector2 Position
	{
		get => base.Position;
		set
		{
			base.Position = value;
			_regionsDirty = true;
		}
	}

	private void SetSlice(SliceRegionTypes region, SliceRegion slice)
	{
		_regions[(int)region] = slice;
	}

	private void CalculateRegions()
	{
		if (Texture == null || !_regionsDirty) return;
		_regionsDirty = false;

		var src = Source;
		var pos = Position.ToPoint();
		var size = Size.ToPoint();

		if (SliceOrientation == Orientation.Horizontal)
		{
			int sizeWidth = size.X / 3;
			int sourceWidth = src.Width / 3;
			int sourceHeight = src.Height;

			int sliceCornerWidth = SliceCornerSize == 0 ? sizeWidth : SliceCornerSize;

			// Calculate adjusted corner sizes based on available space
			int totalWidth = size.X;
			float adjustedCornerSize = 0;
			bool showCenter = true;

			if (totalWidth <= sliceCornerWidth * 2)
			{
				// If total size is too small, scale corner pieces proportionally
				adjustedCornerSize = (totalWidth) / 2f;
				showCenter = false;
			}
			else
			{
				adjustedCornerSize = sliceCornerWidth;
			}

			// Left
			SetSlice(SliceRegionTypes.LeftOrTop, new SliceRegion(
				new Rectangle(src.X, src.Y, sourceWidth, sourceHeight),
				new Rectangle(pos.X, pos.Y, (int)adjustedCornerSize, size.Y)));

			// Center (Stretchable)
			if (showCenter)
			{
				int centerWidth = (int)(size.X - 2 * adjustedCornerSize);
				SetSlice(SliceRegionTypes.Center, new SliceRegion(
					new Rectangle(src.X + sourceWidth, src.Y, sourceWidth, sourceHeight),
					new Rectangle((int)(pos.X + adjustedCornerSize), pos.Y, centerWidth, size.Y)));
			}
			else
			{
				// Hide center by setting zero width
				SetSlice(SliceRegionTypes.Center, new SliceRegion(
					new Rectangle(src.X + sourceWidth, src.Y, sourceWidth, sourceHeight),
					new Rectangle((int)(pos.X + adjustedCornerSize), pos.Y, 0, size.Y)));
			}

			// Right
			SetSlice(SliceRegionTypes.RightOrBottom, new SliceRegion(
				new Rectangle(src.Right - sourceWidth, src.Y, sourceWidth, sourceHeight),
				new Rectangle((int)(pos.X + size.X - adjustedCornerSize), pos.Y, (int)adjustedCornerSize, size.Y)));
		}
		else // Vertical
		{
			int sourceWidth = src.Width;
			int sourceHeight = src.Height / 3;

			int sliceCornerHeight = SliceCornerSize == 0 ? sourceHeight : SliceCornerSize;

			// Calculate adjusted corner sizes based on available space
			int totalHeight = size.Y;
			int adjustedCornerSize = 0;
			bool showCenter = true;

			if (totalHeight <= sliceCornerHeight * 2)
			{
				// If total size is too small, scale corner pieces proportionally
				adjustedCornerSize = totalHeight / 2;
				showCenter = false;
			}
			else
			{
				adjustedCornerSize = sliceCornerHeight;
			}

			// Top
			SetSlice(SliceRegionTypes.LeftOrTop, new SliceRegion(
				new Rectangle(src.X, src.Y, sourceWidth, sourceHeight),
				new Rectangle(pos.X, pos.Y, size.X, adjustedCornerSize)));

			// Center (Stretchable)
			if (showCenter)
			{
				int centerHeight = size.Y - 2 * adjustedCornerSize;
				SetSlice(SliceRegionTypes.Center, new SliceRegion(
					new Rectangle(src.X, src.Y + sourceHeight, sourceWidth, sourceHeight),
					new Rectangle(pos.X, pos.Y + adjustedCornerSize, size.X, centerHeight)));
			}
			else
			{
				// Hide center by setting zero height
				SetSlice(SliceRegionTypes.Center, new SliceRegion(
					new Rectangle(src.X, src.Y + sourceHeight, sourceWidth, sourceHeight),
					new Rectangle(pos.X, pos.Y + adjustedCornerSize, size.X, 0)));
			}

			// Bottom
			SetSlice(SliceRegionTypes.RightOrBottom, new SliceRegion(
				new Rectangle(src.X, src.Bottom - sourceHeight, sourceWidth, sourceHeight),
				new Rectangle(pos.X, pos.Y + size.Y - adjustedCornerSize, size.X, adjustedCornerSize)));
		}
	}

	public override void Update(GameTime gt)
	{
		base.Update(gt);
		if (_regionsDirty)
			CalculateRegions();
	}

	public override void Draw()
	{
		Draw(Texture, Style.Foreground);
	}

	internal override void Draw(Texture2D tx, Color clr)
	{
		if (Texture == null) return;

		// Skip drawing completely if size is zero
		if (Size.X <= 0 || Size.Y <= 0)
			return;

		// Draw Left/Top
		DrawRegion(0, tx, clr);

		// Draw Center
		if (StretchCenter)
		{
			DrawRegion(1, tx, clr);
		}
		else
		{
			TileRegion(_regions[1], tx, clr);
		}

		// Draw Right/Bottom
		DrawRegion(2, tx, clr);
	}

	private void DrawRegion(int regionType, Texture2D tx, Color clr)
	{
		var regionData = _regions[regionType];

		// Skip drawing regions with zero width or height
		if (regionData.Destination.Width <= 0 || regionData.Destination.Height <= 0)
			return;

		Graphics.SpriteBatch.Draw(tx, regionData.Destination, regionData.Source, clr);
	}

	private void TileRegion(SliceRegion region, Texture2D tx, Color clr)
	{
		var sb = Graphics.SpriteBatch;
		Rectangle source = region.Source;
		Rectangle dest = region.Destination;

		if (source.Width <= 0 || source.Height <= 0 ||
			dest.Width <= 0 || dest.Height <= 0)
			return;

		int tilesX = (int)MathF.Ceiling((float)dest.Width / source.Width);
		int tilesY = (int)MathF.Ceiling((float)dest.Height / source.Height);

		for (int y = 0; y < tilesY; y++)
		{
			for (int x = 0; x < tilesX; x++)
			{
				int tileWidth = Math.Min(source.Width, dest.Width - x * source.Width);
				int tileHeight = Math.Min(source.Height, dest.Height - y * source.Height);

				if (tileWidth <= 0 || tileHeight <= 0) continue;

				Rectangle tileSource = new Rectangle(
					source.X, source.Y,
					tileWidth, tileHeight);

				Rectangle tileDest = new Rectangle(
					dest.X + x * source.Width,
					dest.Y + y * source.Height,
					tileWidth, tileHeight);

				sb.Draw(tx, tileDest, tileSource, clr);
			}
		}
	}
}