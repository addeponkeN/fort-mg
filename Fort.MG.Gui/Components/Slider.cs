using Microsoft.Xna.Framework;

namespace Fort.MG.Gui.Components;

public class Slider : GuiComponent
{
	private bool _isDragging = false;
	private Skin _background;
	private Image _handle;
	private float _value = 0f;
	private float _prevTriggerValue;
	public float MinValue { get; set; } = 0f;
	public float MaxValue { get; set; } = 1f;

	public float Value
	{
		get => _value;
		set
		{
			if (_value == value) return;
			_value = value;
			UpdateHandlePosition();
			OnValueChanged?.Invoke(Value);
		}
	}

	public float HandleOffset { get; set; } = 0f;

	public Skin Background
	{
		get => _background;
		set => SetComponent(ref _background, value);
	}

	public Image Handle
	{
		get => _handle;
		set => SetComponent(ref _handle, value);
	}

	public override Vector2 Size
	{
		get => base.Size;
		set
		{
			base.Size = value;
			UpdateTransforms();
		}
	}

	public override Vector2 Position
	{
		get => base.Position;
		set
		{
			base.Position = value;
			UpdateTransforms();
		}
	}

	public event Action<float> OnValueChanged;

	public Slider()
	{
		IsFocusable = true;
		base.Size = new Vector2(200, 20);
		IsDirty = true;
	}

	protected override void UpdateTransforms()
	{
		base.UpdateTransforms();
		if (Background == null || Handle == null) return;

		Handle.Size = new Vector2(Size.Y);
		Background.Size = Size;
		Background.Position = Position;
		UpdateHandlePosition();
	}

	private void UpdateHandlePosition()
	{
		if (Handle != null)
			Handle.Position = Background.Position + new Vector2(GetHandleOffset() - HandleOffset, 0);
	}

	private float GetHandleOffset()
	{
		float p = (Value - MinValue) / (MaxValue - MinValue);
		return p * (Size.X - Handle.Size.X + HandleOffset * 2f);
	}

	public override void Start()
	{
		base.Start();

		Background ??= new Skin();

		Handle ??= new Image { Foreground = new Color(200, 200, 200, 255) };

		UpdateTransforms();
		if (Skins.Count == 0)
			AddSkin(new Skin());
	}

	public override void OnTrigger()
	{
		base.OnTrigger();
		if (Value > 0f)
		{
			_prevTriggerValue = Value;
			Value = 0f;
		}
		else
		{
			if (_prevTriggerValue == 0f)
				_prevTriggerValue = 1f;
			Value = _prevTriggerValue;
		}
	}

	public override void UpdateInput(InputHandlerArgs args)
	{
		base.UpdateInput(args);

		var mpos = Canvas.MousePosition;
		if (((Handle?.Bounds.Contains(mpos) ?? false) || Bounds.Contains(mpos)) && Input.LeftClick)
		{
			_isDragging = true;
		}

		if (_isDragging)
		{
			Value = MathHelper.Clamp(
				(mpos.X - Position.X) / Size.X,
				0f, 1f) * (MaxValue - MinValue) + MinValue;

			if (Input.LeftRelease)
				_isDragging = false;
		}
	}
}
