using Fort.MG.Core;
using Fort.MG.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Fort.MG.EntitySystem.Components;

public class CameraControls : Component
{
    Camera cam;

    public float StepSize;

    public bool EnableScrolling = true;
    public bool EnableDragging = true;

    public override void Init()
    {
        base.Init();
        StepSize = 1.3333333f;
    }

    public override void OnAdded()
    {
        base.OnAdded();
        cam = Parent as Camera;
    }

    public override void Update(IGameTime t)
    {
        base.Update(t);

        if(EnableDragging && Input.RightHold)
        {
            var mousePos = Input.MouseTransformedPos(cam.UpdateMatrix);
            var oldMousePos = Input.OldMouseTransformedPos(cam.UpdateMatrix);
            Vector2 mouseMovement = mousePos - oldMousePos;
            cam.Transform.Position -= mouseMovement;
        }

        if(EnableScrolling)
        {
            if(Input.WheelDown)
            {
                cam.Zoom /= StepSize;
            }

            if(Input.WheelUp)
            {
                cam.Zoom *= StepSize;
            }

            if(cam.Zoom < 0.01)
                cam.Zoom = 0.01f;
        }

        if(Input.KeyClick(Keys.F5))
            cam.Reset();
    }
}