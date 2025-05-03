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
		set => _canvas = value;
	}

	public virtual ComponentBase Parent
	{
		get => _parent;
		set => _parent = value;
	}

	public virtual bool IsVisible { get; set; } = true;
	public bool IsEnabled { get; set; } = true;

	public bool IsInputEnabled { get; set; } = true;

	public virtual void Start()
	{
		Started = true;
	}

	public virtual void UpdateInput(InputHandlerArgs args)
	{
		OnFirstFrame();
	}

	public virtual void Update(GameTime gt)
	{
		OnFirstFrame();
	}

	internal void OnFirstFrame()
	{
		if (!Started)
		{
			Start();
			Started = true;
		}
	}

	/// <summary>	
	/// Draw component to a custom target
	/// </summary>
	public virtual void DrawTarget()
	{
	}

	/// <summary>
	/// Draw component to a custom batch
	/// </summary>
	public virtual void DrawContent()
	{
	}

	/// <summary>
	/// Draw component to the main batch
	/// </summary>
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

	public bool IsDirty = true;

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
			IsDirty = true;
		}
	}

	public virtual Vector2 Position
	{
		get => _totalPosition;
		set
		{
			_position = value;
			UpdateTransforms();
			IsDirty = true;
		}
	}

	public virtual Vector2 Size
	{
		get => _size;
		set
		{
			_size = value;
			UpdateTransforms();
			IsDirty = true;
		}
	}

	protected virtual void UpdateTransforms()
	{
		_totalPosition = _position + _localPosition;
		Bounds = new Rectangle((int)_totalPosition.X, (int)_totalPosition.Y, (int)Size.X, (int)Size.Y);
		IsDirty = false;
	}

	public override void Update(GameTime gt)
	{
		base.Update(gt);

		if (IsDirty)
		{
			UpdateTransforms();
		}

		foreach (var skin in Skins)
		{
			if (skin.IsEnabled)
				skin.Update(gt);
		}

		UpdateComponents(gt);
	}

	internal void UpdateComponents(GameTime gt)
	{
		foreach (var component in Components)
		{
			if (component.IsEnabled)
				component.Update(gt);
		}
	}

	public override void UpdateInput(InputHandlerArgs args)
	{
		base.UpdateInput(args);

		UpdateEventLogic(args);

		foreach (var component in Components)
		{
			component.UpdateInput(args);
		}
	}


	public override void Draw()
	{
		DrawSelf();

		DrawSkins();

		DrawComponents();

		DrawFocused();
	}

	internal void DrawFocused()
	{
		if (IsFocused)
		{
			var b = Bounds;
			b.Inflate(2, 2);
			b.DrawLined(Color.White * 0.3f);
		}
	}

	public override void DrawTarget()
	{
		base.DrawTarget();
	}

	internal void DrawSkins()
	{
		foreach (var skin in Skins)
		{
			if (skin.IsVisible)
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
			if (comp.IsVisible)
				comp.Draw();
		}
	}

	public override void DrawText()
	{
		base.DrawText();
		foreach (var comp in Components)
		{
			if (comp.IsVisible)
				comp.DrawText();
		}
	}

	internal virtual void Draw(Texture2D tx, Color clr)
	{
	}

	public override void DrawDebug()
	{
		if (!GuiSettings.Debug) return;
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
		OnTriggerEvent?.Invoke(new TriggerArgs { Component = this });
	}
}