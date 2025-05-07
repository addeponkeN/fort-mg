using FontStashSharp;
using Fort.MG.Extensions;
using Fort.MG.Gui.Controls;
using Microsoft.Xna.Framework;

namespace Fort.MG.Gui.Components;

public class Button : GuiComponent
{
    public Label Title { get; }

    public DynamicSpriteFont Font
    {
        get => Title.Font;
        set
        {
            Title.Font = value;
            IsDirty = true;
            UpdateTextPosition();
        }
    }

    public string Text
    {
        get => Title.Text;
        set
        {
            Title.Text = value;
            IsDirty = true;
            UpdateTextPosition();
        }
    }

    public override Vector2 Position
    {
        get => base.Position;
        set
        {
            base.Position = value;
            UpdateTextPosition();
        }
    }

    public override Vector2 Size
    {
        get => base.Size == Vector2.Zero ? Title.Size + new Vector2(8) : base.Size;
        set => base.Size = Vector2.Clamp(value, MinSize, value);
    }

    public Vector2 MinSize { get; set; } = new Vector2(96, 24);

    private void UpdateTextPosition()
    {
        Title.Position = GHelper.Center(Bounds, Title.Size);
    }

    public Button()
    {
        IsFocusable = true;
        Title = new();
        base.Size = MinSize;
    }

    public override void Start()
    {
        base.Start();
        AddComponent(new InteractiveComponent());
        if (Skins.Count == 0)
            AddSkin(Skin.DefaultSkinBackground2);
        Title.Canvas = Canvas;
    }

    public override void Update(GameTime gt)
    {
        base.Update(gt);
        Title.Update(gt);
    }

    protected override void OnMouse(MouseClickEvent arg)
    {
        base.OnMouse(arg);
        if (arg.Button == MouseButton.Left && arg.IsClick)
        {
            OnTrigger();
        }
    }

    public override void OnTrigger()
    {
        base.OnTrigger();
    }

    public override void Draw()
    {
        base.Draw();
    }

    public override void DrawText()
    {
        base.DrawText();
        Title.DrawText();
    }
}