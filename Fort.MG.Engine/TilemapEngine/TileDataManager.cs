using System.Text.Json.Serialization;
using Fort.Utility;

namespace Fort.MG.TilemapEngine;

public class TilesetVariance
{
    [JsonPropertyName("orientation")]
    public string Orientation { get; set; }

    [JsonPropertyName("chances")]
    public int[]? Chances { get; set; }
}

public class TilesetDataFile
{
    [JsonIgnore]
    public byte Id { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("tileset")]
    public string Tileset { get; set; }

    [JsonPropertyName("solo")]
    public bool IsSolo { get; set; }

    [JsonPropertyName("autoTiles")]
    public List<string> AutoTile { get; set; }

    [JsonPropertyName("variances")]
    public List<TilesetVariance> Variances { get; set; } = new();
}

public readonly struct TileDataKey : IEquatable<TileDataKey>
{
    private readonly byte _type;
    private readonly TileOrientation _orientation;
    private readonly byte _variant;

    public TileDataKey(byte type, TileOrientation orientation, byte variant)
    {
        _type = type;
        _orientation = orientation;
        _variant = variant;
    }

    public bool Equals(TileDataKey other) =>
        _type == other._type && _orientation == other._orientation && _variant == other._variant;

    public override bool Equals(object obj) => obj is TileDataKey other && Equals(other);

    public override int GetHashCode() =>
        HashCode.Combine(_type, (int)_orientation, _variant);

    public override string ToString()
    {
        return $"{_type},{_orientation},{_variant}";
    }
}

public class TileDataManager
{
    private readonly string _tilesetDataName;
    private bool _loaded;
    private readonly Dictionary<byte, TilesetDataFile> _tilesets = new();
    private readonly Dictionary<TileDataKey, Tile> _tiles = new();

    public TileDataManager(string tilesetDataName)
    {
        _tilesetDataName = tilesetDataName;
        if (!Path.HasExtension(_tilesetDataName) && !_tilesetDataName.EndsWith(".json"))
        {
            _tilesetDataName += ".json";
        }
    }

    public byte DefaultTileType { get; set; } = 1;

    public void Load()
    {
        if (_loaded)
            return;

        var data = JsonHelper.Load<List<TilesetDataFile>>(_tilesetDataName);

        byte typeId = 0;
        foreach (var tileset in data)
        {
            tileset.Id = ++typeId;
            _tilesets[tileset.Id] = tileset;

            LoadTile(tileset);
        }
        _loaded = true;
    }

    public List<TilesetDataFile> GetTilesets() => _tilesets.Values.ToList();

    public TilesetDataFile GetTilesetData(byte tileType)
    {
        Load();
        return _tilesets[tileType];
    }

    private void AddTile(byte type, TileOrientation ori = TileOrientation.All, byte variant = 0)
    {
        var key = new TileDataKey(type, ori, variant);
        _tiles[key] = new Tile(type, ori, variant);
    }

    private void LoadTile(TilesetDataFile tilesetData)
    {
        if (tilesetData.IsSolo)
        {
            var orientation = TileOrientation.All;
            for (byte i = 0; i < tilesetData.Variances.Count; i++)
            {
                var variant = tilesetData.Variances[i];
                for (byte j = 0; j < variant.Chances?.Length; j++)
                {
                    AddTile(tilesetData.Id, orientation, j);
                }
            }
        }
        else
        {
            var oriCount = (int)TileOrientation.All + 1;
            for (int i = 0; i < oriCount; i++)
            {
                var orientation = (TileOrientation)i;
                AddTile(tilesetData.Id, orientation);
            }

            foreach (var variance in tilesetData.Variances)
            {
                if (!Enum.TryParse<TileOrientation>(variance.Orientation, true, out var orientation))
                {
                    //Logger.Error($"Failed to parse Tileset '{tilesetData.Type}' orientation data: {variance.Orientation}");
                    continue;
                }

                int variantCount = variance.Chances?.Length ?? 0;
                for (byte variant = 0; variant < variantCount; variant++)
                {
                    AddTile(tilesetData.Id, orientation, variant);
                }
            }
        }
    }

    public Tile GetTile(byte type, TileOrientation ori = TileOrientation.All, byte variant = 0)
    {
        Load();
        var key = new TileDataKey(type, ori, variant);
        return _tiles[key];//.TryGetValue(key, out var tile) ? tile : null;
    }
}