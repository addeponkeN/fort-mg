using Fort.MG.EntitySystem;
using Fort.MG.Scenes;
using Fort.MG.VirtualViewports;
using Microsoft.Xna.Framework;

namespace Fort.MG.Components;

public class Camera : Component
{
    public static Camera Main => Scene.Current.Cam;

    public VirtualViewport Viewport;

    public float Zoom = 1f;
    public float Rotation;

    /// <summary>
    /// used in spritebatch matrix parameter
    /// </summary>
    public Matrix DrawMatrix;

    /// <summary>
    /// used in update loop & draw loop
    /// use DrawMatrix in spritebatch parameter
    /// </summary>
    public Matrix UpdateMatrix;

    public Rectangle Bounds => new(
            (int)(Transform.Position.X - Viewport.Width / Zoom / 2f),
            (int)(Transform.Position.Y - Viewport.Height / Zoom / 2f),
            (int)(Viewport.Width / Zoom),
            (int)(Viewport.Height / Zoom));

    public Rectangle BoundsExcludeZoom => new(
        (int)(Transform.Position.X - Viewport.Width / 2f),
        (int)(Transform.Position.Y - Viewport.Height / 2f),
        (int)Viewport.Width,
        (int)Viewport.Height);

    private Vector2 _zoomOffset;

    public Camera()
    {
        Viewport = new VirtualViewportScaling(1280, 720);
    }

    public override void Update(IGameTime t)
    {
        base.Update(t);

        _zoomOffset = new Vector2(
            (Viewport.Width - Viewport.Width / Zoom) / 2f,
            (Viewport.Height - Viewport.Height / Zoom) / 2f);

        var tfPos = Transform.Position;
        var pos = new Vector3((-tfPos.X - _zoomOffset.X + Viewport.Width * 0.5f),
            (-tfPos.Y - _zoomOffset.Y + Viewport.Height * 0.5f), 0f);

        DrawMatrix = Matrix.CreateTranslation(pos) *
                     Matrix.CreateRotationZ(Rotation) *
                     Matrix.CreateScale(Zoom, Zoom, 1f)
                     * Viewport.Matrix;

        UpdateMatrix = Matrix.CreateTranslation(pos) *
                       Matrix.CreateRotationZ(Rotation) *
                       Matrix.CreateScale(Zoom, Zoom, 1f)
                       * Viewport.Matrix;
    }

    public void Reset()
    {
        Zoom = 1f;
        Rotation = 0f;
    }
}