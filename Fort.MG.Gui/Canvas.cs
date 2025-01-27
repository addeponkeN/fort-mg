using Fort.MG.Gui.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Fort.MG.Gui;

public class Canvas : Container
{
	private RenderTarget2D _target;

	internal SpriteBatch Sb;
	public ContentManager Content;
	public Matrix Matrix;

	public int Width, Height;

	public Canvas(SpriteBatch sb, ContentManager content)
	{
		Sb = sb;
		Content = content;
	}

	public void SetCanvasSize(int w, int h)
	{
		_target?.Dispose();

		Size = new Vector2(w, h);

		Width = w;
		Height = h;

		Matrix = Matrix.CreateScale(Width);

		_target = new(Sb.GraphicsDevice, w, h);
	}

	public override void Update(GameTime gt)
	{
		base.Update(gt);
	}

	public override void Add(Component item)
	{
		item.Canvas = this;
		item.Parent = this;
		Items.Add(item);
	}

	public override void Remove(Component item)
	{
		item.Canvas = null;
		Items.Remove(item);
	}

	private void Render()
	{
		var gd = Sb.GraphicsDevice;
		gd.SetRenderTarget(_target);
		gd.Clear(Color.Transparent);

		Sb.Begin(SpriteSortMode.BackToFront, transformMatrix: Matrix);
		base.Draw();
		Sb.End();
	}

	public override void Draw()
	{
		Sb.Begin();
		var scale = Vector2.One;
		Sb.Draw(_target, Position, _target.Bounds, Style.Foreground, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
		Sb.End();
	}
}
