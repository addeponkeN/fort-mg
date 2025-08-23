using Fort.MG.EntitySystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Fort.MG.Components;

public class DefaultCameraControls : Component
{
    private Camera _cam;

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
        _cam = Entity.GetComponent<Camera>();
    }

    public override void Update(IGameTime t)
    {
        base.Update(t);

        if(EnableDragging && Input.RightHold)
        {
            var mousePos = Input.MouseTransformedPos(_cam.UpdateMatrix);
            var oldMousePos = Input.OldMouseTransformedPos(_cam.UpdateMatrix);
            Vector2 mouseMovement = mousePos - oldMousePos;
            _cam.Transform.Position -= mouseMovement;
        }

        if(EnableScrolling)
        {
            if(Input.WheelDown)
            {
                _cam.Zoom /= StepSize;
            }

            if(Input.WheelUp)
            {
                _cam.Zoom *= StepSize;
            }

            if(_cam.Zoom < 0.01)
                _cam.Zoom = 0.01f;
        }

        if(Input.KeyClick(Keys.F5))
            _cam.Reset();
    }
}