namespace Fort.MG.Gui.Components;

public enum MouseButton
{
	None,
	Left, Right, Middle,
}

public enum MouseState
{
	None,
	Click,
	Release,
	Hold,
}

public struct MouseClickEvent
{
	public MouseButton Button;
	public MouseState State;
	public bool IsClick => State == MouseState.Click;
}

public partial class GuiComponent
{
	public event Action<MouseClickEvent> OnMouseEvent;
	public event Action OnMouseEnter;
	public event Action OnMouseLeave;

	public event Action OnTriggerEvent;

	public bool IsHovered;
	public bool IsPressed;

	private void UpdateEventLogic()
	{
		var m = Canvas == null ? Input.MousePos : Input.MouseTransformedPos(Canvas.TransformMatrix);

		if (IsHovered)
		{
			if (!Bounds.Contains(m))
			{
				IsHovered = false;
				OnMouseLeave?.Invoke();
			}

			if (Input.LeftClick)
			{
				IsPressed = true;
				OnMouse(new MouseClickEvent { Button = MouseButton.Left, State = MouseState.Click });
			}
		}
		else
		{
			if (Bounds.Contains(m))
			{
				IsHovered = true;
				OnMouseEnter?.Invoke();
			}
		}

		if (IsPressed)
		{
			if (Input.LeftRelease)
			{
				IsPressed = false;
				if (IsHovered)
				{
					OnMouse(new MouseClickEvent { Button = MouseButton.Left, State = MouseState.Release });
				}
			}
			else
			{
				OnMouse(new MouseClickEvent { Button = MouseButton.Left, State = MouseState.Hold });
			}
		}
	}

	public virtual void OnMouse(MouseClickEvent arg)
	{
		OnMouseEvent?.Invoke(arg);
	}

}
