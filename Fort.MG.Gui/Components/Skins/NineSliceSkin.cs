using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Fort.MG.Gui.Components;

public class NineSlice : Skin
{
	private enum SliceRegionType
	{
		TopLeft, TopCenter, TopRight,
		MiddleLeft, Center, MiddleRight,
		BottomLeft, BottomCenter, BottomRight
	}

	private readonly SliceRegion[] _regions = new SliceRegion[9];
	private bool _regionsDirty = true;
	private float _borderExtend = 1f;

	/// <summary>
	/// Size of the slice border (not source)
	/// </summary>
	public int SliceSize { get; set; }

	public bool StretchCenter { get; set; } = true;

	public bool DrawCenter { get; set; } = true;

	public float BorderExtend
	{
		get => _borderExtend;
		set
		{
			_borderExtend = value;
			if (SliceSize == 0 && _borderExtend > 0)
			{
				SliceSize = Source.Width / 3;
			}
			_regionsDirty = true;
		}
	}

	public override Vector2 Size
	{
		get => base.Size;
		set
		{
			if (base.Size == value)
				return;
			base.Size = value;
			_regionsDirty = true;
		}
	}

	public override Vector2 Position
	{
		get => base.Position;
		set
		{
			if (base.Position == value)
				return;
			base.Position = value;
			UpdateTransforms();
			_regionsDirty = true;
		}
	}

	public NineSlice()
	{
		Style.Foreground = Color.White;
	}

	protected override void UpdateTransforms()
	{
		base.UpdateTransforms();
		CalculateRegions();
	}

	private void SetSlice(SliceRegionType regionType, SliceRegion slice)
	{
		_regions[(int)regionType] = slice;
	}

	private void CalculateRegions()
	{
		if (Texture == null) return;

		var src = Source;

		if (src.Width < 3 || src.Height < 3) return;

		int w3 = src.Width / 3;
		int h3 = src.Height / 3;

		int sliceWidth = SliceSize > 0 ? SliceSize : w3;
		int sliceHeight = SliceSize > 0 ? SliceSize : h3;

		var pos = Position.ToPoint();
		var size = Size.ToPoint();

		// Apply the border offset
		int offset = SliceSize;
		pos.X -= (int)(offset * BorderExtend);
		pos.Y -= (int)(offset * BorderExtend);
		size.X += (int)(offset * 2 * BorderExtend);
		size.Y += (int)(offset * 2 * BorderExtend);

		// Ensure minimum size constraints
		int minWidth = sliceWidth;
		int minHeight = sliceHeight;

		if (size.X < minWidth) size.X = minWidth;
		if (size.Y < minHeight) size.Y = minHeight;

		// Top Row
		SetSlice(SliceRegionType.TopLeft, new SliceRegion(
			new Rectangle(src.X, src.Y, w3, h3),
			new Rectangle(pos.X, pos.Y, sliceWidth, sliceHeight)));

		SetSlice(SliceRegionType.TopCenter, new SliceRegion(
			new Rectangle(src.X + w3, src.Y, w3, h3),
			new Rectangle(pos.X + sliceWidth, pos.Y, size.X - sliceWidth * 2, sliceHeight)));

		SetSlice(SliceRegionType.TopRight, new SliceRegion(
			new Rectangle(src.X + 2 * w3, src.Y, w3, h3),
			new Rectangle(pos.X + size.X - sliceWidth, pos.Y, sliceWidth, sliceHeight)));

		// Middle Row
		SetSlice(SliceRegionType.MiddleLeft, new SliceRegion(
			new Rectangle(src.X, src.Y + h3, w3, h3),
			new Rectangle(pos.X, pos.Y + sliceHeight, sliceWidth, size.Y - sliceHeight * 2)));

		SetSlice(SliceRegionType.Center, new SliceRegion(
			new Rectangle(src.X + w3, src.Y + h3, w3, h3),
			new Rectangle(pos.X + sliceWidth, pos.Y + sliceHeight, size.X - sliceWidth * 2, size.Y - sliceHeight * 2)));

		SetSlice(SliceRegionType.MiddleRight, new SliceRegion(
			new Rectangle(src.X + 2 * w3, src.Y + h3, w3, h3),
			new Rectangle(pos.X + size.X - sliceWidth, pos.Y + sliceHeight, sliceWidth, size.Y - sliceHeight * 2)));

		// Bottom Row
		SetSlice(SliceRegionType.BottomLeft, new SliceRegion(
			new Rectangle(src.X, src.Y + 2 * h3, w3, h3),
			new Rectangle(pos.X, pos.Y + size.Y - sliceHeight, sliceWidth, sliceHeight)));

		SetSlice(SliceRegionType.BottomCenter, new SliceRegion(
			new Rectangle(src.X + w3, src.Y + 2 * h3, w3, h3),
			new Rectangle(pos.X + sliceWidth, pos.Y + size.Y - sliceHeight, size.X - sliceWidth * 2, sliceHeight)));

		SetSlice(SliceRegionType.BottomRight, new SliceRegion(
			new Rectangle(src.X + 2 * w3, src.Y + 2 * h3, w3, h3),
			new Rectangle(pos.X + size.X - sliceWidth, pos.Y + size.Y - sliceHeight, sliceWidth, sliceHeight)));

		_regionsDirty = false;
	}


	public override void Update(GameTime gt)
	{
		base.Update(gt);

		if (_regionsDirty)
			CalculateRegions();
	}

	private void DrawRegion(Texture2D tx, Color clr, int regionInt)
	{
		var sb = Graphics.SpriteBatch;
		var region = _regions[regionInt];
		sb.Draw(Texture, region.Destination, region.Source, clr);
	}

	public override void Draw()
	{
		Draw(Texture, Style.Foreground);
	}

	internal override void Draw(Texture2D tx, Color clr)
	{
		//base.Draw(tx, clr);
		if (tx == null) return;

		// Top row
		DrawRegion(tx, clr, 0);
		DrawRegion(tx, clr, 1);
		DrawRegion(tx, clr, 2);

		//	Middle row
		DrawRegion(tx, clr, 3);
		DrawRegion(tx, clr, 5);

		// Bot row
		DrawRegion(tx, clr, 6);
		DrawRegion(tx, clr, 7);
		DrawRegion(tx, clr, 8);

		// Center region
		if (DrawCenter)
		{
			var centerRegion = _regions[4];

			if (StretchCenter)
			{
				var sb = Graphics.SpriteBatch;
				sb.Draw(tx, centerRegion.Destination, centerRegion.Source, clr);
			}
			else
			{
				TileRegion(tx, clr, centerRegion);
			}
		}
	}

	private void TileRegion(Texture2D tx, Color clr, SliceRegion region)
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