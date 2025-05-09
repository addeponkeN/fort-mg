using Fort.MG.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Fort.MG.Gui.Components;

public class ListBox : Container, IDisposable
{
    private readonly RasterizerState _rasterizer = new() { ScissorTestEnable = true };
    private readonly List<GuiComponent> _visibleItems = new();

    private readonly byte _scrollRateLength = 100;
    private float _scrollOffset;
    private int _visibleItemStart;
    private int _visibleItemEnd;
    private float _totalHeight;
    private float _totalWidth;
    private int _selectedIndex;

    public bool IsHighlightSelection { get; set; } = true;

    private Rectangle _transformedBounds;

    public Orientation ItemOrientation { get; set; } = Orientation.Vertical;

    public bool CanScroll => ItemOrientation == Orientation.Vertical
        ? Items.Count > 0 && _totalHeight > Size.Y
        : Items.Count > 0 && _totalWidth > Size.X;

    public float ScrollRate { get; set; } = 100f;

    public int SelectedIndex
    {
        get => _selectedIndex;
        set
        {
            if (_selectedIndex == value)
                return;
            _selectedIndex = value;
            SelectedItemChangedEvent?.Invoke(SelectedItem);
        }
    }

    public GuiComponent? SelectedItem =>
        SelectedIndex >= 0 && SelectedIndex < Items.Count ? Items[SelectedIndex] : null;

    public override Vector2 Size
    {
        get => base.Size;
        set
        {
            base.Size = value;
            UpdateVisibleItems();
        }
    }

    public event Action<GuiComponent?> SelectedItemChangedEvent;

    public ListBox()
    {
        base.Size = new Vector2(160, 250);
        AutoSize = false;
        base.AddSkin(new Skin());

        FortCore.WindowSizeChanged += FortCoreOnWindowSizeChanged;
    }

    private void FortCoreOnWindowSizeChanged(object? sender, EventArgs e)
    {
        UpdateTransforms();
    }

    public override void Start()
    {
        base.Start();

        CalculateTotalDimensions();
        UpdateVisibleItems();
    }

    protected override void UpdateTransforms()
    {
        base.UpdateTransforms();
        _transformedBounds = Bounds;
        if (Canvas != null)
        {
            var transformMatrix = Canvas.TransformMatrix;
            var pos = Vector2.Transform(new Vector2(_transformedBounds.X, _transformedBounds.Y), transformMatrix);
            var size = Vector2.Transform(new Vector2(_transformedBounds.Width, _transformedBounds.Height),
                transformMatrix);
            _transformedBounds = new Rectangle((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y);
        }
    }

    public override void UpdateItemTransforms()
    {
        CalculateTotalDimensions();
        UpdateVisibleItems();
    }

    public override void Update(GameTime gt)
    {
        base.Update(gt);

        UpdateDirtyItems();

        if (IsHovered)
        {
            if (Input.WheelDown)
                Scroll(_scrollRateLength * ScrollRate / (ItemOrientation == Orientation.Vertical ? Size.Y : Size.X));
            else if (Input.WheelUp)
                Scroll(-(_scrollRateLength * ScrollRate / (ItemOrientation == Orientation.Vertical ? Size.Y : Size.X)));
        }

        if (base.IsPressed)
        {
            for (int i = 0; i < _visibleItems.Count; i++)
            {
                var item = _visibleItems[i];
                if (item.Bounds.Contains(Canvas.MousePosition))
                {
                    int actualIndex = _visibleItemStart + i;
                    SelectedIndex = actualIndex;
                    break;
                }
            }
        }

        foreach (var item in _visibleItems)
        {
            item.Update(gt);
        }
    }

    private void CalculateTotalDimensions()
    {
        _totalHeight = 0;
        _totalWidth = 0;

        foreach (var item in Items)
        {
            if (ItemOrientation == Orientation.Vertical)
            {
                _totalHeight += item.Size.Y + Spacing;
            }
            else
            {
                _totalWidth += item.Size.X + Spacing;
            }
        }

        if (Items.Count > 0)
        {
            if (ItemOrientation == Orientation.Vertical)
                _totalHeight -= Spacing;
            else
                _totalWidth -= Spacing;
        }
    }

    private void UpdateVisibleItems()
    {
        UpdateTransforms();

        foreach (var item in _visibleItems)
        {
            item.IsEnabled = false;
        }

        _visibleItems.Clear();

        foreach (var item in Items)
        {
            if (item is Container container)
            {
                container.UpdateItemTransforms();
            }
        }

        if (ItemOrientation == Orientation.Vertical)
        {
            UpdateVisibleItemsVertical();
        }
        else
        {
            UpdateVisibleItemsHorizontal();
        }
    }

    private void UpdateVisibleItemsVertical()
    {
        float currentHeight = 0;
        _visibleItemStart = 0;
        _visibleItemEnd = Items.Count;

        float totalVisibleItemSize = 0f;

        for (int i = 0; i < Items.Count; i++)
        {
            var item = Items[i];
            float size = currentHeight + item.Size.Y;
            if (size > _scrollOffset)
            {
                _visibleItemStart = i;
                break;
            }

            currentHeight += item.Size.Y + Spacing;
            item.IsEnabled = false;
            totalVisibleItemSize += item.Size.Y + Spacing;
        }

        currentHeight = totalVisibleItemSize;

        for (int i = _visibleItemStart; i < Items.Count; i++)
        {
            var item = Items[i];
            if (currentHeight > _scrollOffset + Size.Y)
            {
                _visibleItemEnd = i;
                break;
            }

            currentHeight += item.Size.Y + Spacing;
        }

        _visibleItemEnd = Math.Min(_visibleItemEnd, Items.Count);

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

    private void UpdateVisibleItemsHorizontal()
    {
        float currentWidth = 0;
        _visibleItemStart = 0;
        _visibleItemEnd = Items.Count;

        float totalVisibleItemSize = 0f;

        for (int i = 0; i < Items.Count; i++)
        {
            var item = Items[i];
            float size = currentWidth + item.Size.X;
            if (size > _scrollOffset)
            {
                _visibleItemStart = i;
                break;
            }

            currentWidth += item.Size.X + Spacing;
            item.IsEnabled = false;
            totalVisibleItemSize += item.Size.X + Spacing;
        }

        currentWidth = totalVisibleItemSize;

        for (int i = _visibleItemStart; i < Items.Count; i++)
        {
            var item = Items[i];
            if (currentWidth > _scrollOffset + Size.X)
            {
                _visibleItemEnd = i;
                break;
            }

            currentWidth += item.Size.X + Spacing;
        }

        _visibleItemEnd = Math.Min(_visibleItemEnd, Items.Count);

        float offset = -(_scrollOffset - totalVisibleItemSize);
        for (int i = _visibleItemStart; i < _visibleItemEnd; i++)
        {
            var item = Items[i];
            _visibleItems.Add(item);
            item.Position = new Vector2(Padding.X + Position.X + offset, Padding.Y + Position.Y);
            item.IsEnabled = IsEnabled;
            offset += item.Size.X + Spacing;
        }
    }

    private void DrawSelectedItem()
    {
        if (!IsHighlightSelection) return;
        if (SelectedIndex < 0 || SelectedIndex >= Items.Count)
            return;

        var bounds = SelectedItem.Bounds;
        var outerBounds = bounds;
        outerBounds.Inflate(2, 2);

        bounds.DrawRec(Color.SteelBlue * 0.025f);
        outerBounds.DrawLined(Color.SteelBlue * 0.4f, 1f);
    }

    public override void DrawContent()
    {
        base.DrawContent();

        var sb = Graphics.SpriteBatch;
        var gd = Graphics.GraphicsDevice;

        Rectangle previousScissorRect = gd.ScissorRectangle;
        RasterizerState originalRasterizerState = gd.RasterizerState;

        var tf = Canvas.TransformMatrix;

        gd.ScissorRectangle = _transformedBounds;
        gd.RasterizerState = _rasterizer;

        sb.Begin(SpriteSortMode.Deferred, Canvas.BlendState, Canvas.SamplerState, null, gd.RasterizerState,
            transformMatrix: tf);

        base.DrawSkins();
        base.DrawComponents();

        DrawSelectedItem();

        for (int i = 0; i < _visibleItems.Count; i++)
        {
            _visibleItems[i].Draw();
        }

        sb.End();

        Graphics.BeginText();

        for (var i = 0; i < _visibleItems.Count; i++)
        {
            _visibleItems[i].DrawText();
        }

        sb.End();

        gd.ScissorRectangle = previousScissorRect;
        gd.RasterizerState = originalRasterizerState;
    }

    public override void Draw()
    {
    }

    public override void DrawText()
    {
    }

    public void Scroll(float direction)
    {
        if (!CanScroll) return;

        _scrollOffset = ItemOrientation == Orientation.Vertical
            ? Math.Clamp(_scrollOffset + direction, 0, _totalHeight - Size.Y)
            : Math.Clamp(_scrollOffset + direction, 0, _totalWidth - Size.X);

        UpdateVisibleItems();
    }

    public void SetItemOrientation(Orientation mode)
    {
        if (ItemOrientation == mode)
            return;

        ItemOrientation = mode;
        _scrollOffset = 0;
        CalculateTotalDimensions();
        UpdateVisibleItems();
    }

    public override void AddItem(GuiComponent item)
    {
        item.Canvas = Canvas;
        item.Parent = this;
        Items.Add(item);
        CalculateTotalDimensions();
        UpdateVisibleItems();
        item.IsEnabled = false;
    }

    public override void RemoveItem(GuiComponent item)
    {
        if (!Items.Remove(item))
            return;

        item.Parent = null;
        CalculateTotalDimensions();
        UpdateVisibleItems();

        if (SelectedItem == item)
        {
            SelectedIndex--;
        }
    }

    public void Dispose()
    {
        _rasterizer.Dispose();
        FortCore.WindowSizeChanged -= FortCoreOnWindowSizeChanged;
    }
}