using Fort.MG.Core;
using Fort.MG.Gui;
using Fort.MG.Gui.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Fort.MG.Example;

public class Game1 : FortCoreGame
{
	private Canvas _canvas;

	protected override void Initialize()
	{
		base.Initialize();
	}

	protected override void LoadContent()
	{
		GuiContent.Load();

		_canvas = new Canvas();

		var panel = new StackPanel
		{
			Position = new Vector2(100),
		};

		panel.Add(new Label
		{
			Text = "long cooooooool text ababab aabba a\nNJEW LIONOE !",
			Foreground = Color.White,
		});

		_canvas.Add(panel);
	}

	protected override void Update(GameTime gt)
	{
		base.Update(gt);
		if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
			Exit();
	}

	protected override void UpdateGame(GameTime gt)
	{
		base.UpdateGame(gt);
		_canvas.Update(gt);
	}

	protected override void Render(GameTime gt)
	{
		base.Render(gt);
		_canvas.Render();
	}

	protected override void Draw(GameTime gt)
	{
		base.Draw(gt);

		GraphicsDevice.SetRenderTarget(null);
		GraphicsDevice.Clear(Color.CornflowerBlue);

		_canvas.Draw();
	}
}
