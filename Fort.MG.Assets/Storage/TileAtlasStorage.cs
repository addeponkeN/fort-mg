using Fort.MG.Assets.Data;

namespace Fort.MG.Assets.Storage;

public static class TileDataKeyPacker
{
	public static int Pack(byte type, int orientation, byte variant)
	{
		return (type << 12) | (((int)orientation & 0xF) << 8) | variant;
	}

	public static void Unpack(int key, out byte type, out int orientation, out byte variant)
	{
		type = (byte)(key >> 12);
		orientation = (key >> 8) & 0xF;
		variant = (byte)(key & 0xFF);
	}
}

public readonly struct TileRegionKey : IEquatable<TileRegionKey>
{
	private readonly string _tileset;
	private readonly int _orientation;
	private readonly int _variant;
	public TileRegionKey(string tileset, int orientation, int variant)
	{
		_tileset = tileset;
		_orientation = orientation;
		_variant = variant;
	}
	public bool Equals(TileRegionKey other) =>
		_tileset == other._tileset && _orientation == other._orientation && _variant == other._variant;
	public override bool Equals(object obj) => obj is TileRegionKey other && Equals(other);
	public override int GetHashCode() =>
		HashCode.Combine(_tileset, _orientation, _variant);
	public override string ToString() => $"{_tileset},{_orientation},{_variant}";
}

internal class TileAtlasStorage() : BaseStorage<TilesetAtlas>("textures")
{
	public override TilesetAtlas Load(string name)
	{
		var path = GetFilePath(name);
		var atlas = AssetManager.ContentManager.Load<TilesetAtlas>(path);
		foreach (var region in atlas.Regions)
		{
			AssetManager.RegisterTileRegion(region);
		}
		Add(name, atlas);
		return atlas;
	}

	public override TilesetAtlas Get(string name)
	{
		if (!Storage.TryGetValue(name, out var asset))
		{
			asset = Load(name);
		}

		return asset;
	}
}
