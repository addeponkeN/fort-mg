using Fort.MG.Assets.Data;
using Fort.MG.Core;
using Fort.MG.Gui;
using Fort.MG.Gui.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Fort.MG.Example;

public class ExampleGame : FortGame
{
	private Canvas _canvas;

	//private GuyDataTypeReader reader;

	SpriteAtlas atlas;

	protected override void Initialize()
	{
		base.Initialize();
	}

	protected override void LoadContent()
	{
		base.LoadContent();

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

		//reader = new GuyDataTypeReader();

		//var guyData = Content.Load<GuyData>("data/guydata");

		atlas = Content.Load<SpriteAtlas>("textures/world");
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

		var sb = Graphics.SpriteBatch;

		sb.Begin();
		sb.Draw(Engine.AssetManager.Pixel, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, new Vector2(atlas.Texture.Width, atlas.Texture.Height), SpriteEffects.None, 0f);
		sb.Draw(atlas.Texture, Vector2.Zero, Color.White);

		var rocksRegion = atlas["tree1"];
		sb.Draw(rocksRegion, new Vector2(0, 300), rocksRegion, Color.White);


		sb.End();

		_canvas.Draw();
	}
}
