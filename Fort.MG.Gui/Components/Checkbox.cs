﻿using Fort.MG.Extensions;
using Microsoft.Xna.Framework;

namespace Fort.MG.Gui.Components;

public class Checkbox : GuiComponent
{
	private Skin _background;
	private Image _tickMark;
	private GuiComponent? _content;

	private bool _isChecked;

	public Skin Background
	{
		get => _background;
		set => SetComponent(ref _background, value);
	}

	public Image TickMark
	{
		get => _tickMark;
		set => SetComponent(ref _tickMark, value);
	}

	public GuiComponent? Content
	{
		get => _content;
		set => SetComponent(ref _content, value);
	}

	public override Vector2 Size
	{
		get => base.Size;
		set
		{
			Background.Size = value;
			base.Size = value;
		}
	}

	public bool IsChecked
	{
		get => _isChecked;
		set
		{
			if (_isChecked == value)
				return;

			_isChecked = value;
			OnCheckedChanged?.Invoke(_isChecked);
			TickMark.IsVisible = IsChecked;
		}
	}

	public event Action<bool> OnCheckedChanged;

	public Checkbox()
	{
		base.Size = new Vector2(24); // Default size for the checkbox
	}

	protected override void UpdateTransforms()
	{
		base.UpdateTransforms();

		if (Background == null || TickMark == null) return;

		Background.Size = Size;
		Background.Position = Position;

		if (Content != null)
		{
			Content.Position = new Vector2(Position.X + Size.X + 4, Position.Y + Size.Y * 0.5f - Content.Size.Y * 0.5f);
		}

		TickMark.Size = new Vector2(Size.X - 4, Size.Y - 4);

		TickMark.Position = GHelper.Center(Position, TickMark.Size, Size);
	}

	public override void Start()
	{
		base.Start();

		Background ??= new Skin();
		TickMark ??= new Image
		{
			Foreground = StyleManager.ForegroundColor,
			IsVisible = IsChecked,
		};

		Content ??= new Label
		{
			Text = "Checkbox",
			Foreground = StyleManager.ForegroundColor,
		};

		UpdateTransforms();
		if (Skins.Count == 0)
			AddSkin(new Skin());
	}

	public override void Update(GameTime gt)
	{
		base.Update(gt);
	}

	public override void UpdateInput()
	{
		base.UpdateInput();

		if (Bounds.Contains(Canvas.MousePosition) && Input.LeftClick)
		{
			IsChecked = !IsChecked;
		}
	}

	public override void Draw()
	{
		base.Draw();
	}

	public override void DrawDebug()
	{
		base.DrawDebug();
	}

	public override void DrawText()
	{
		base.DrawText();
	}
}