using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Fort.MG.Gui.Components;

public class Skin : Image
{
	protected GuiComponent GuiParent;

	public Skin()
	{
		Style.Foreground = StyleManager.BackgroundColor;
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
		Position = GuiParent.Position;
		Size = GuiParent.Size;
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
