namespace Fort.MG.TilemapEngine;

public class AutoTileConfig
{
    public bool PreserveExistingVariants { get; set; } = false;
    public bool UseWeightedSelection { get; set; } = true;

    private readonly WeightedVariantSelector _variantSelector;

    public AutoTileConfig(WeightedVariantSelector variantSelector)
    {
        _variantSelector = variantSelector;
    }

    public byte SelectVariant(byte tileType, TileOrientation orientation)
    {
        if (UseWeightedSelection)
        {
            return _variantSelector.SelectVariant(tileType, orientation);
        }
        else
        {
            // Fallback to uniform random selection
            int variantCount = _variantSelector.GetVariantCount(tileType, orientation);
            return (byte)new Random().Next(0, variantCount);
        }
    }
}

public class TileMapAutoTiler
{
    private readonly TileMap _map;
    private readonly AutoTileConfig _config;
    private readonly Tile?[] _tiles;

    public TileMapAutoTiler(TileMap map, AutoTileConfig config = null)
    {
        _map = map ?? throw new ArgumentNullException(nameof(map));
        var selector = new WeightedVariantSelector(map.Content);
        _config = config ?? new AutoTileConfig(selector);

        _tiles = new Tile[5];
    }

    public void AutoTile(int startX, int startY, int width, int height, int layer = 0)
    {
        int checkStartX = startX - 1;
        int checkStartY = startY - 1;
        int endX = checkStartX + width + 2;
        int endY = checkStartY + height + 2;

        for (int y = checkStartY; y < endY; y++)
        {
            for (int x = checkStartX; x < endX; x++)
            {
                ProcessTile(x, y, layer);
            }
        }
    }

    private void ProcessTile(int x, int y, int layer)
    {
        var center = GetTile(x, y, layer);
        if (center == null || center.Type == 0)
            return;

        var tileData = _map.Content.GetTilesetData(center.Type);
        //_config.PreserveExistingVariants = !tileData.IsSolo;
        if (tileData.IsSolo)
        {
            ApplyTileOrientation(x, y, layer, center, TileOrientation.All);
            return;
        }

        _tiles[0] = center;                     // center
        _tiles[1] = GetTile(x, y - 1, layer);   // up
        _tiles[2] = GetTile(x, y + 1, layer);   // down
        _tiles[3] = GetTile(x - 1, y, layer);   // left
        _tiles[4] = GetTile(x + 1, y, layer);   // right

        var ori = CalculateTileOrientation();

        ApplyTileOrientation(x, y, layer, center, ori);
    }

    private void ApplyTileOrientation(int x, int y, int layer, Tile currentTile, TileOrientation newOrientation)
    {
        try
        {
            byte variant = 0;

            if (!_config.PreserveExistingVariants || currentTile.Orientation != newOrientation)
            {
                variant = _config.SelectVariant(currentTile.Type, newOrientation);
            }

            var tile = _map.Content.GetTile(currentTile.Type, newOrientation, variant);
            _map.SetTile(x, y, layer, tile);
        }
        catch
        {
            // Ignore errors
        }
    }

    private TileOrientation CalculateTileOrientation()
    {
        var centerTile = _tiles[0];
        if (centerTile == null || centerTile.Type == 0) return TileOrientation.None;

        TileOrientation orientation = TileOrientation.None;

        // up
        var topTile = _tiles[1];
        if (centerTile.CanConnectTo(topTile))
            orientation |= TileOrientation.Top;

        // down
        var bottomTile = _tiles[2];
        if (centerTile.CanConnectTo(bottomTile))
            orientation |= TileOrientation.Bot;

        // left
        var leftTile = _tiles[3];
        if (centerTile.CanConnectTo(leftTile))
            orientation |= TileOrientation.Left;

        // right
        var rightTile = _tiles[4];
        if (centerTile.CanConnectTo(rightTile))
            orientation |= TileOrientation.Right;

        return orientation;
    }

    private Tile? GetTile(int x, int y, int layer)
    {
        try
        {
            var chunk = _map.GetChunk(x, y);
            if (chunk == null) return null;

            int tileIndex = TileHelper.TileToTileIndex(x, y, chunk.X, chunk.Y, _map.ChunkSize);
            return chunk.GetTile(tileIndex, layer);
        }
        catch
        {
            return null;
        }
    }
}