using Microsoft.Xna.Framework;

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

public struct InputHandlerArgs(Vector2 mousePos)
{
	public bool MouseHandled { get; set; }
	public bool KeyboardHandled { get; set; }
	public bool ScrollHandled { get; set; }
	public bool HoverHandled { get; set; }

	public Vector2 MousePosition { get; private set; } = mousePos;
}

public struct TriggerArgs
{
	public GuiComponent Component { get; set; }
}

public partial class GuiComponent
{
	public event Action<MouseClickEvent> OnMouseEvent;
	public event Action OnMouseEnter;
	public event Action OnMouseLeave;

	public event Action<TriggerArgs> OnTriggerEvent;

	public bool IsHovered;
	public bool IsPressed;
	public bool IsReleased;

	private void HandleHover(InputHandlerArgs args)
	{
		var m = args.MousePosition;
		if (IsHovered)
		{
			//	Exit hovered
			if (!Bounds.Contains(m))
			{
				IsHovered = false;
				OnMouseLeave?.Invoke();
				OnHover(IsHovered);
			}
			else
			{
				args.HoverHandled = true;
			}
		}
		else
		{
			// Enter hovered
			if (Bounds.Contains(m))
			{
				IsHovered = true;
				OnMouseEnter?.Invoke();
				OnHover(IsHovered);
				args.HoverHandled = true;
			}

		}
	}

	private void HandleLeftClick(InputHandlerArgs args)
	{
		IsReleased = false;

		if (IsHovered)
		{
			if (Input.LeftClick)
			{
				IsPressed = true;
				OnMouse(new MouseClickEvent { Button = MouseButton.Left, State = MouseState.Click });
				args.MouseHandled = true;
			}
		}

		if (IsPressed)
		{
			if (Input.LeftRelease)
			{
				IsReleased = true;
				IsPressed = false;
				if (IsHovered)
				{
					OnMouse(new MouseClickEvent { Button = MouseButton.Left, State = MouseState.Release });
					args.MouseHandled = true;
				}
			}
			else
			{
				OnMouse(new MouseClickEvent { Button = MouseButton.Left, State = MouseState.Hold });
				args.MouseHandled = true;
			}
		}
	}

	private void UpdateEventLogic(InputHandlerArgs args)
	{
		if (!args.HoverHandled)
			HandleHover(args);
		if (!args.MouseHandled)
			HandleLeftClick(args);
	}

	protected virtual void OnMouse(MouseClickEvent arg)
	{
		OnMouseEvent?.Invoke(arg);
	}

	protected virtual void OnHover(bool isHovered)
	{
	}
}
