using Microsoft.Xna.Framework;

namespace Fort.MG.TilemapEngine;

[Flags]
public enum TileOrientation : byte
{
    None = 0,
    Top,
    Right,
    TopRight,

    Bot,
    TopBot,
    BotRight,
    TopRightBot,

    Left,
    TopLeft,
    RightLeft,
    TopRightLeft,

    BotLeft,
    TopBotLeft,
    RightBotLeft,
    All = 15,
}

public class Tile
{
    public byte Type { get; set; }              // Tileset type (grass, stone, etc.)
    public TileOrientation Orientation { get; set; }  // Auto-tile orientation
    public byte Variant { get; set; }           // Variant within the orientation

    public Tile()
    {
        Type = 0;
        Orientation = TileOrientation.None;
        Variant = 0;
    }

    public Tile(byte type, TileOrientation orientation = TileOrientation.None, byte variant = 0)
    {
        Type = type;
        Orientation = orientation;
        Variant = variant;
    }
}

public class TileMap
{
    public int ChunkSize = 10;
    public int ChunkLength => ChunkSize * ChunkSize;
    public int TileSize = 32;
    public int Layers = 3;

    public TileDataManager Content { get; private set; }
    public TileObjectStore ObjectStore = new();
    public Dictionary<Point, Chunk> Chunks = new();

    public TileMap(TileDataManager tileData)
    {
        Content = tileData;
    }

    public void SetTile(int tileX, int tileY, int layer, Tile tile)
    {
        var c = GetChunk(tileX, tileY);
        int i = TileHelper.TileToTileIndex(tileX, tileY, c.X, c.Y, ChunkSize);
        c.SetTile(i, layer, tile);
    }

    public Chunk GetChunk(int tileX, int tileY)
    {
        var chunkCoords = TileHelper.TileToChunk(tileX, tileY, ChunkSize);
        return GetChunkByCC(chunkCoords.X, chunkCoords.Y);
    }

    public Chunk GetChunkByCC(int chunkX, int chunkY)
    {
        var chunkCoords = new Point(chunkX, chunkY);
        if (!Chunks.TryGetValue(chunkCoords, out var c))
        {
            c = CreateChunk(chunkCoords.X, chunkCoords.Y);
            Chunks.Add(chunkCoords, c);
        }
        return c;
    }

    public Chunk CreateChunk(int chunkX, int chunkY)
    {
        var chunk = new Chunk { X = chunkX, Y = chunkY };
        chunk.Setup(this);
        return chunk;
    }

    public void Clear()
    {
        foreach (var chunk in Chunks)
        {
            chunk.Value.Clear();
        }
    }
}

public class TileLayer
{
    public Tile[] Tiles;
    public void Setup(TileMap tm)
    {
        var content = tm.Content;
        Tiles = new Tile[tm.ChunkLength];
        for (int i = 0; i < Tiles.Length; i++)
        {
            Tiles[i] = null;//content.GetTile(content.DefaultTileType);
        }
    }

    public void Clear()
    {
        Array.Clear(Tiles, 0, Tiles.Length);
    }
}

public class TileObject
{
    public int TileType;
    public int DataIndex;
}

public struct TileObjectWorldData
{
    public byte State;
}

public class TileObjectStore
{
    private readonly List<TileObjectWorldData> _data = new();
    private readonly Queue<int> _unused = new();

    public int RequestStorage()
    {
        if (_unused.TryDequeue(out int id))
        {
            return id;
        }
        else
        {
            var data = new TileObjectWorldData();
            _data.Add(data);
            id = _data.Count;
        }

        return id;
    }

    public void ReturnStorage(int id)
    {
        _unused.Enqueue(id);
    }

}

public class TileObjectData
{
    public int Width;
    public int Height;
    public byte[] Roots;
}

public class Chunk
{
    public TileLayer[] Layers;
    public TileObject[] Objects;

    public int X;
    public int Y;
    public byte ChunkSize;

    public void Setup(TileMap tm)
    {
        ChunkSize = (byte)tm.ChunkSize;
        Objects = new TileObject[tm.ChunkLength];
        Layers = new TileLayer[tm.Layers];
        for (int i = 0; i < tm.Layers; i++)
        {
            var layer = new TileLayer();
            layer.Setup(tm);
            Layers[i] = layer;
        }
    }

    public Tile GetTile(int i, int layer)
    {
        return Layers[layer].Tiles[i];
    }

    public void SetTile(int i, int layer, Tile tile)
    {
        Layers[layer].Tiles[i] = tile;
    }

    public void SetObject(int x, int y, TileObject to)
    {
        int i = TileHelper.TileToTileIndex(x, y, X, Y, ChunkSize);
        Objects[i] = to;
    }

    public void Clear()
    {
        for (int i = 0; i < Layers.Length; i++)
        {
            Layers[i].Clear();
        }

        Array.Clear(Objects);

    }
}