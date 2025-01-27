using Fort.MG.Gui;
using Fort.MG.Gui.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Fort.MG.Example;

public class Game1 : Game
{
	private GraphicsDeviceManager _graphics;
	private SpriteBatch _spriteBatch;

	private FortCore _fort;
	private Canvas _canvas;

	public Game1()
	{
		_graphics = new GraphicsDeviceManager(this);
		Content.RootDirectory = "Content";
		IsMouseVisible = true;

		_graphics.PreferredBackBufferWidth = 1920;
		_graphics.PreferredBackBufferHeight = 1080;
	}

	protected override void Initialize()
	{
		base.Initialize();
	}

	protected override void LoadContent()
	{
		_fort = new(this);

		_spriteBatch = new SpriteBatch(GraphicsDevice);
		_canvas = new Canvas(_spriteBatch);

		var panel = new StackPanel
		{
			Position = new Vector2(100),
		};

		panel.Add(new Label { Text = "hej hej hehe" });

		_canvas.Add(panel);
	}

	protected override void Update(GameTime gt)
	{
		if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
			Exit();

		_fort.Update(gt);

		_canvas.Update(gt);

		base.Update(gt);
	}

	protected override void Draw(GameTime gameTime)
	{
		GraphicsDevice.Clear(Color.CornflowerBlue);

		_canvas.Draw();

		base.Draw(gameTime);
	}
}
