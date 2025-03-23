using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Fort.MG.Gui.Components;

public class ThreeSlice : Skin
{
	public enum Orientation { Horizontal, Vertical }

	public int BorderSize { get; set; } = 5;
	public Orientation SliceOrientation { get; set; } = Orientation.Horizontal;
	public bool StretchCenter { get; set; } = true;

	private enum SliceRegionTypes { LeftOrTop, Center, RightOrBottom }

	private readonly SliceRegion[] _regions = new SliceRegion[3];
	private bool _regionsDirty = true;

	public Point SliceCornerSize = new Point(32, 32);
	public Point SliceMidSize = new Point(32, 32);

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

		int sliceCornerWidth = SliceCornerSize.X;
		int sliceCornerHeight = SliceCornerSize.Y;
		int sliceMidWidth = SliceMidSize.X;
		int sliceMidHeight = SliceMidSize.Y;

		if (SliceOrientation == Orientation.Horizontal)
		{
			// Left
			SetSlice(SliceRegionTypes.LeftOrTop, new SliceRegion(
				new Rectangle(src.X, src.Y, sliceCornerWidth, src.Height),
				new Rectangle(pos.X, pos.Y, sliceCornerWidth, size.Y)));

			// Center (Stretchable)
			SetSlice(SliceRegionTypes.Center, new SliceRegion(
				new Rectangle(src.X + sliceCornerWidth, src.Y, src.Width - 2 * sliceCornerWidth, src.Height),
				new Rectangle(pos.X + sliceCornerWidth, pos.Y, size.X - 2 * sliceCornerWidth, size.Y)));

			// Right
			SetSlice(SliceRegionTypes.RightOrBottom, new SliceRegion(
				new Rectangle(src.Right - sliceCornerWidth, src.Y, sliceCornerWidth, src.Height),
				new Rectangle(pos.X + size.X - sliceCornerWidth, pos.Y, sliceCornerWidth, size.Y)));
		}
		else // Vertical
		{
			// Top
			SetSlice(SliceRegionTypes.LeftOrTop, new SliceRegion(
				new Rectangle(src.X, src.Y, src.Width, sliceCornerHeight),
				new Rectangle(pos.X, pos.Y, size.X, sliceCornerHeight)));

			// Center (Stretchable)
			SetSlice(SliceRegionTypes.Center, new SliceRegion(
				new Rectangle(src.X, src.Y + sliceCornerHeight, src.Width, src.Height - 2 * sliceCornerHeight),
				new Rectangle(pos.X, pos.Y + sliceCornerHeight, size.X, size.Y - 2 * sliceCornerHeight)));

			// Bottom
			SetSlice(SliceRegionTypes.RightOrBottom, new SliceRegion(
				new Rectangle(src.X, src.Bottom - sliceCornerHeight, src.Width, sliceCornerHeight),
				new Rectangle(pos.X, pos.Y + size.Y - sliceCornerHeight, size.X, sliceCornerHeight)));
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
		Draw(Texture, GuiParent.Style.Background);
	}

	internal override void Draw(Texture2D tx, Color clr)
	{
		if (Texture == null) return;

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
