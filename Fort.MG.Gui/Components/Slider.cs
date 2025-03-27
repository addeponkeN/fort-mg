using Microsoft.Xna.Framework;

namespace Fort.MG.Gui.Components;

public class Slider : GuiComponent
{
	private bool _isDragging = false;
	private float _prevValue;
	private Skin _background;
	private Image _handle;
	private float _value = 0f;
	private bool _triggerValueChanged = true;

	public float MinValue { get; set; } = 0f;
	public float MaxValue { get; set; } = 1f;

	public float Value
	{
		get => _value;
		set
		{
			_value = value;
			if (_triggerValueChanged)
				OnValueChanged?.Invoke(_value);
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
		IsPositionDirty = true;
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

	public override void UpdateInput()
	{
		base.UpdateInput();

		var mpos = Canvas.MousePosition;
		if (((Handle?.Bounds.Contains(mpos) ?? false) || Bounds.Contains(mpos)) && Input.LeftClick)
		{
			_isDragging = true;
		}

		if (_isDragging)
		{
			_value = MathHelper.Clamp(
				(mpos.X - Position.X) / Size.X,
				0f, 1f) * (MaxValue - MinValue) + MinValue;
			UpdateHandlePosition();

			if (_prevValue != _value)
			{
				_triggerValueChanged = false;
				OnValueChanged?.Invoke(Value);
				_triggerValueChanged = true;
			}

			_prevValue = _value;
			if (Input.LeftRelease)
				_isDragging = false;
		}
	}

	public override void Draw()
	{
		base.Draw();
		//Background.Draw();
		//Handle.Draw();
	}
}
