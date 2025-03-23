namespace Fort.MG.Gui.Components;

public partial class GuiComponent
{
	public event Action OnClick;
	public event Action OnRelease;
	public event Action OnMouseEnter;
	public event Action OnMouseLeave;

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
				OnClick?.Invoke();
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
					OnRelease?.Invoke();
				}
			}
		}
	}
}
