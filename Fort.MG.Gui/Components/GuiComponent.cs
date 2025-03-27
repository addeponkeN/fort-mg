using Fort.MG.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Fort.MG.Gui.Components;

public class ComponentBase
{
	private ComponentBase _parent;

	protected bool Started;

	internal Canvas _canvas;

	public virtual Canvas Canvas
	{
		get => _canvas;
		internal set => _canvas = value;
	}

	public ComponentBase Parent
	{
		get => _parent;
		internal set => _parent = value;
	}

	public bool IsVisible { get; set; } = true;
	public bool IsEnabled { get; set; } = true;

	public virtual void Start()
	{
		Started = true;
	}

	public virtual void UpdateInput()
	{
	}

	public virtual void Update(GameTime gt)
	{
		if (!Started)
		{
			Start();
			Started = true;
		}

	}

	public virtual void DrawContent()
	{
	}

	public virtual void Draw()
	{
	}

	public virtual void DrawText()
	{
	}

	public virtual void DrawDebug()
	{
	}
}

public partial class GuiComponent : ComponentBase
{
	private Vector2 _totalPosition;
	private Vector2 _localPosition;
	private Vector2 _position;
	private Vector2 _size;

	public Style Style { get; set; } = new();

	public Rectangle Bounds { get; private set; }
	public int Id { get; set; }
	public string Name { get; set; }

	internal bool IsPositionDirty = true;

	public virtual bool IsFocused { get; set; }
	public bool IsFocusable { get; set; }

	public virtual Color Foreground
	{
		get => Style.Foreground;
		set => Style.Foreground = value;
	}

	public virtual Vector2 LocalPosition
	{
		get => _localPosition;
		set
		{
			_localPosition = value;
			UpdateTransforms();
			IsPositionDirty = true;
		}
	}

	public virtual Vector2 Position
	{
		get => _totalPosition;
		set
		{
			_position = value;
			UpdateTransforms();
			IsPositionDirty = true;
		}
	}

	public virtual Vector2 Size
	{
		get => _size;
		set
		{
			_size = value;
			UpdateTransforms();
			IsPositionDirty = true;
		}
	}

	protected virtual void UpdateTransforms()
	{
		_totalPosition = _position + _localPosition;
		Bounds = new Rectangle((int)_totalPosition.X, (int)_totalPosition.Y, (int)Size.X, (int)Size.Y);
		IsPositionDirty = false;
	}

	public override void Update(GameTime gt)
	{
		base.Update(gt);

		if (IsPositionDirty)
		{
			UpdateTransforms();
		}

		UpdateEventLogic();

		foreach (var skin in Skins)
		{
			skin.Update(gt);
		}

		foreach (var child in Components)
		{
			child.Update(gt);
		}
	}

	public override void Draw()
	{
		DrawSelf();

		DrawSkins();

		DrawComponents();
	}

	internal void DrawSkins()
	{
		foreach (var skin in Skins)
		{
			skin.Draw();
		}
	}

	internal void DrawSelf()
	{
		base.Draw();
	}

	internal void DrawComponents()
	{
		foreach (var comp in Components)
		{
			comp.Draw();
		}
	}

	public override void DrawText()
	{
		base.DrawText();
		foreach (var comp in Components)
		{
			comp.DrawText();
		}
	}

	internal virtual void Draw(Texture2D tx, Color clr)
	{
	}

	public override void DrawDebug()
	{
		if (!IsVisible) return;

		base.DrawDebug();
		Bounds.DrawLined(Color.MonoGameOrange);
	}

	public virtual void OnFocus(bool isFocus)
	{
		IsFocused = isFocus;
	}

	public virtual void OnTrigger()
	{
		OnTriggerEvent?.Invoke();
	}

}