using Fort.MG.Assets.Data;
using Fort.MG.Assets.Storage;
using Fort.MG.EntitySystem;
using Fort.MG.Extensions;
using Fort.MG.Gui.Components;
using Fort.MG.Rendering;
using Fort.MG.Scenes;
using Fort.MG.TilemapEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Fort.MG.Renderers;

public class TileRenderData
{

    private readonly TileDataManager _data;

    public Dictionary<int, Rectangle> Frames;

    public TilesetAtlas TilesetAtlas { get; set; }

    public TileRenderData(TileDataManager data)
    {
        _data = data;
    }

    public void Load(string tilesetName = "tiles")
    {
        Frames = new();
        TilesetAtlas = FortEngine.Assets.GetAsset<TilesetAtlas>(tilesetName);

        var sets = _data.GetTilesets();

        int ts = 32;
        int frameSize = 32;

        int orientationCount = (int)TileOrientation.All + 1;

        foreach (var set in sets)
        {
            var setName = set.Tileset;

            if (set.IsSolo)
            {
                for (byte i = 0; i < set.Variances.Count; i++)
                {
                    var variance = set.Variances[i];
                    for (int j = 0; j < variance.Chances.Length; j++)
                    {
                        AddFrame(set.Id, set.Tileset, TileOrientation.All, (byte)j);
                    }
                }
            }
            else
            {
                for (byte i = 0; i < orientationCount; i++)
                {
                    var orientation = (TileOrientation)i;
                    AddFrame(set.Id, set.Tileset, orientation, 0);
                }

                for (byte j = 0; j < set.Variances.Count; j++)
                {
                    var variance = set.Variances[j];
                    if (!Enum.TryParse<TileOrientation>(variance.Orientation, true, out var ori))
                    {
                        Logger.Error($"Failed to parse tileset '{setName}' variance orientation '{variance.Orientation}' ");
                        continue;
                    }

                    for (byte i = 0; i < variance.Chances?.Length; i++)
                    {
                        AddFrame(set.Id, set.Tileset, ori, (byte)(i));
                    }
                }
            }
        }
    }

    private void AddFrame(byte type, string tilesetName, TileOrientation orientation, byte variant)
    {
        var tileRegion = FortEngine.Assets.GetTile(tilesetName, (int)orientation, variant);
        var key = TileDataKeyPacker.Pack(type, (int)orientation, variant);
        Frames[key] = tileRegion.Frame;
    }

    public Rectangle GetFrame(byte type, TileOrientation orientation, byte variant)
    {
        int key = TileDataKeyPacker.Pack(type, (int)orientation, variant);
        return Frames.TryGetValue(key, out var frame) ? frame : Rectangle.Empty;
    }
}

public struct TileRenderCache
{
    public int X, Y;
    public ushort FrameX, FrameY;
}

public sealed class TileRenderLayerCache
{
    private readonly int _layerCount;
    private readonly int _bufferSize;
    private readonly int[] _cacheLength;
    private TileRenderCache[,] _tileCache;
    private readonly Vector2[] _positions;
    private readonly Rectangle[] _sourceRects;

    public TileRenderLayerCache(int layerCount, int bufferSize)
    {
        _layerCount = layerCount;
        _bufferSize = bufferSize;

        _tileCache = new TileRenderCache[layerCount, bufferSize];
        _cacheLength = new int[layerCount];

        _positions = new Vector2[bufferSize];
        _sourceRects = new Rectangle[bufferSize];
    }

    public TileRenderCache[,] TileCache => _tileCache;

    public int GetCacheLength(int layer) => _cacheLength[layer];

    public void AddCache(int layer, in TileRenderCache cache)
    {
        if (_cacheLength[layer] < _bufferSize)
        {
            _tileCache[layer, _cacheLength[layer]] = cache;
            _cacheLength[layer]++;
        }
    }

    public void Reset()
    {
        Array.Clear(_cacheLength, 0, _layerCount);
    }

    public void DrawLayer(int layer, SpriteBatch sb, Texture2D atlas)
    {
        int count = _cacheLength[layer];
        if (count == 0) return;

        for (int i = 0; i < count; i++)
        {
            ref var cache = ref _tileCache[layer, i];
            _positions[i] = new Vector2(cache.X, cache.Y);
            _sourceRects[i] = new Rectangle(cache.FrameX, cache.FrameY, 32, 32);
        }

        const int batchSize = 512;
        for (int start = 0; start < count; start += batchSize)
        {
            int end = Math.Min(start + batchSize, count);
            for (int i = start; i < end; i++)
            {
                sb.Draw(atlas, _positions[i], _sourceRects[i], Color.White);
            }
        }
    }
}

public class TileMapRenderer : Component, IFortRenderable
{
    public float DrawLayer { get; set; } = 0.25f;
    public RenderPass RenderPass { get; set; } = RenderPasses.DefaultTileMapRenderer;

    private TileMapAutoTiler _autoTiler;
    private TileDataManager _tileData;
    private TileRenderData _tileRenderData;

    private Rectangle _prevRefreshChunkBounds;
    private Rectangle _chunkBounds;

    private TileMap _map;
    public List<Chunk> CurrentChunks = new();

    private TileRenderLayerCache _cache;

    private int _tileSize;
    private int _chunkSize;

    private Rectangle _screenBounds;

    public void RefreshCurrentChunks()
    {
        _tileSize = _map.TileSize;
        _chunkSize = _map.ChunkSize;

        for (int i = CurrentChunks.Count - 1; i >= 0; i--)
        {
            var c = CurrentChunks[i];
            if (!_chunkBounds.Contains(c.X, c.Y))
            {
                CurrentChunks.RemoveAt(i);
            }
        }

        for (int x = _chunkBounds.X; x <= _chunkBounds.Right; x++)
        {
            for (int y = _chunkBounds.Y; y <= _chunkBounds.Bottom; y++)
            {
                if (!_prevRefreshChunkBounds.Contains(x, y))
                {
                    var c = _map.GetChunkByCC(x, y);
                    CurrentChunks.Add(c);

                    var tx = TileHelper.ChunkToTile(c.X, c.Y, c.ChunkSize);
                    _autoTiler.AutoTile(tx.X, tx.Y, c.ChunkSize, c.ChunkSize);
                }
            }
        }

        _cache.Reset();
        RebuildCache();

        _prevRefreshChunkBounds = _chunkBounds;
    }

    private void RebuildCache()
    {
        for (int layerIndex = 0; layerIndex < _map.Layers; layerIndex++)
        {
            foreach (var chunk in CurrentChunks)
            {
                if (layerIndex >= chunk.Layers.Length) continue;

                var chunkWorldX = chunk.X * _chunkSize * _tileSize;
                var chunkWorldY = chunk.Y * _chunkSize * _tileSize;

                var layer = chunk.Layers[layerIndex];
                var tiles = layer.Tiles;

                for (int tileIndex = 0; tileIndex < tiles.Length; tileIndex++)
                {
                    var tile = tiles[tileIndex];
                    if (tile == null) continue;

                    int localY = tileIndex / _chunkSize;
                    int localX = tileIndex % _chunkSize;
                    int worldX = chunkWorldX + localX * _tileSize;
                    int worldY = chunkWorldY + localY * _tileSize;

                    if (!_screenBounds.Intersects(new Rectangle(worldX, worldY, _tileSize, _tileSize)))
                        continue;

                    var frameData = _tileRenderData.GetFrame(tile.Type, tile.Orientation, tile.Variant);
                    if (frameData == Rectangle.Empty) continue;

                    var tileCache = new TileRenderCache
                    {
                        X = worldX,
                        Y = worldY,
                        FrameX = (ushort)frameData.X,
                        FrameY = (ushort)frameData.Y,
                    };

                    _cache.AddCache(layerIndex, in tileCache);
                }
            }
        }
    }

    public override void Init()
    {
        base.Init();

    }

    public void SetTileMap(TileMap tilemap)
    {
        _map = tilemap;

        _cache = new(_map.Layers, 2048 * 1024); // 2M tiles max

        _autoTiler = new TileMapAutoTiler(_map);
        _tileRenderData = new TileRenderData(_map.Content);
        _tileRenderData.Load();
        _tileData = _map.Content;

        _tileSize = _map.TileSize;
        _chunkSize = _map.ChunkSize;
    }

    public override void Start()
    {
        base.Start();

        //var canvas = SceneManager.CurrentScene.Canvas;
        //canvas.AddItem(_coordsText);
    }

    public void SetBounds(Rectangle bounds)
    {
        _chunkBounds = _map.WorldBoundsToChunk(bounds);

        // Add some padding for smoother scrolling
        var padding = 2; // chunks
        _screenBounds = new Rectangle(
            bounds.X - padding * _chunkSize * _tileSize,
            bounds.Y - padding * _chunkSize * _tileSize,
            bounds.Width + 2 * padding * _chunkSize * _tileSize,
            bounds.Height + 2 * padding * _chunkSize * _tileSize
        );

        if (_prevRefreshChunkBounds != _chunkBounds)
        {
            RefreshCurrentChunks();
        }
    }

    public override void Update(IGameTime t)
    {
        base.Update(t);

        //if (Input.KeyHold(Keys.D1))
        //{
        //	SetTile(1, 0, 2);
        //}
        //else if (Input.KeyHold(Keys.D2))
        //{
        //	SetTile(2, 1, 1);
        //}
        //else if (Input.KeyHold(Keys.D3))
        //{
        //	SetTile(3, 2, 0);
        //}
        //else if (Input.KeyHold(Keys.D4))
        //{
        //	SetTile(4, 2, 0);
        //}
        //else if (Input.KeyHold(Keys.D5))
        //{
        //	SetTile(5, 2, 0);
        //}
        //else if (Input.KeyHold(Keys.D6))
        //{
        //	SetTile(6, 2, 0);
        //}
        //else if (Input.KeyHold(Keys.D7))
        //{
        //	SetTile(7, 2, 0);
        //}
        //else if (Input.KeyHold(Keys.D8))
        //{
        //	SetTile(8, 2, 0);
        //}
        //else if (Input.KeyHold(Keys.D9))
        //{
        //	SetTile(9, 2, 0);
        //}
        //else if (Input.KeyHold(Keys.D0))
        //{
        //	SetTile(10, 2, 0);
        //}
        //else if (Input.KeyClick(Keys.Space))
        //{
        //	_map.Clear();
        //}

    }

    private void SetTile(byte type, byte layer, int radius = 0)
    {
        var pos = SceneManager.CurrentScene.MousePositionWorld;
        var tileCoords = TileHelper.WorldToTile((int)pos.X, (int)pos.Y, _tileSize);

        var tile = _tileData.GetTile(type);

        for (int dx = -radius; dx <= radius; dx++)
        {
            for (int dy = -radius; dy <= radius; dy++)
            {
                int tx = tileCoords.X + dx;
                int ty = tileCoords.Y + dy;

                _map.SetTile(tx, ty, layer, tile);
            }
        }

        _autoTiler.AutoTile(tileCoords.X - radius, tileCoords.Y - radius, radius * 2 + 1, radius * 2 + 1, layer);

        RefreshCurrentChunks();
    }

    public void Render()
    {
    }

    public void Draw()
    {

        DrawTilemap();
        //DrawTilemapUnsafe();

        //var atlas = _tileRenderData.TilesetAtlas;
        //var sb = Graphics.SpriteBatch;
        //sb.Draw(atlas, Vector2.Zero, Color.White);
        //for (int i = 0; i < 25; i++)
        //{
        //	var g = _tileRenderData.GetFrame(1, TileOrientation.All, (byte)i);
        //	g.DrawLined(Color.Red, 1);
        //}

        //var grass0 = _tileRenderData.GetFrame(1, TileOrientation.All, 0);
        //var grass1 = _tileRenderData.GetFrame(1, TileOrientation.All, 1);
        //var grass2 = _tileRenderData.GetFrame(1, TileOrientation.All, 2);
        //grass0.DrawLined(Color.Red, 1);
        //grass1.DrawLined(Color.Red, 1);
        //grass2.DrawLined(Color.Red, 1);

        //var pos = SceneManager.CurrentScene.MousePositionWorld;
        //var tileCoords = TileHelper.WorldToTile((int)pos.X, (int)pos.Y, _tileSize);
        //_coordsText.Text = $"{tileCoords}";
        //_coordsText.Position = pos;
        //_coordsText.DrawText();
    }

    private void DrawTilemap()
    {
        var atlas = _tileRenderData.TilesetAtlas;
        var sb = Graphics.SpriteBatch;

        // Draw each layer using optimized batch method
        for (int layer = 0; layer < _map.Layers; layer++)
        {
            _cache.DrawLayer(layer, sb, atlas);
        }
    }

    private void DrawDebug()
    {
        Rectangle rec;
        for (int i = 0; i < CurrentChunks.Count; i++)
        {
            var c = CurrentChunks[i];
            rec = c.WorldBounds(_chunkSize, _tileSize);
            rec.DrawLined(Color.MonoGameOrange);
        }
    }
}
