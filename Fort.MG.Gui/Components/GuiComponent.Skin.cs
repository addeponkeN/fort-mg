﻿using Fort.MG.Extensions;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Fort.MG.Gui.Components;

public class Skin : Image
{
	private Vector2 _localSize;

	internal static Skin DefaultSkinForeground => new Skin { Foreground = StyleManager.Foreground1Color };
	internal static Skin DefaultSkinBackground1 => new Skin { Foreground = StyleManager.Background1Color };
	internal static Skin DefaultSkinBackground2 => new Skin { Foreground = StyleManager.Background2Color };

	protected GuiComponent GuiParent;

	public override Texture2D Texture
	{
		get => base.Texture;
		set => base.Texture = value;
	}

	public bool FitParentSize { get; set; } = true;

	public override Vector2 Size
	{
		get => FitParentSize ? GuiParent.Size : _localSize;
		set => _localSize = value;
	}

	public override void Start()
	{
		base.Start();
		GuiParent = (GuiComponent)Parent;
	}

	public override void Update(GameTime gt)
	{
		if (!Started)
		{
			Start();
			Started = true;
		}

		if (FitParentSize)
		{
			base.Size = GuiParent.Size;
			base.Position = GuiParent.Position;
		}
		else
		{
			base.Position = GHelper.Center(GuiParent.Bounds, Size);
		}
	}
}

public partial class GuiComponent
{
	private bool _isDefaultSkin = true;

	public List<Skin> Skins { get; } = new();

	public virtual void AddSkin(Skin skin)
	{
		if (_isDefaultSkin)
		{
			ClearSkins();
			_isDefaultSkin = false;
		}

		skin.Parent = this;
		Skins.Add(skin);
	}

	public virtual void ClearSkins()
	{
		foreach (var skin in Skins)
			skin.Parent = null;
		Skins.Clear();
	}

	public virtual void RemoveSkin(Skin skin)
	{
		Skins.Remove(skin);
	}
}
