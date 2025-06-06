﻿using System.Diagnostics;
using System.Linq;
using Fort.MG.Assets.Data;
using Fort.MG.Gui;
using Fort.MG.Gui.Components;
using Fort.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Fort.MG.Example;

public class ExampleGame : FortGame
{
	private Canvas _canvas;
	private SpriteAtlas _atlas;
	private Texture2D _pixel;
	private SpriteAtlas _guiAtlas;
	private SpriteRegion _nineSlice;
	private SpriteRegion _threeSlice;

	private Window _debugWindow;

	protected override void Initialize()
	{
		base.Initialize();
		Screen.VSync = false;
		LimitFPS(300);
	}

	protected override void LoadContent()
	{
		base.LoadContent();

		_atlas = Content.Load<SpriteAtlas>("textures/world");
		_guiAtlas = Content.Load<SpriteAtlas>("textures/gui");
		_nineSlice = _guiAtlas["9slice_3"];
		_threeSlice = _guiAtlas["3slice"];

		_pixel = new Texture2D(GraphicsDevice, 1, 1);
		_pixel.SetData([Color.White]);

		LoadTest();

		_textRen = new TextRenderer();
	}

	private void LoadTest()
	{
		_canvas = new Canvas();

		_debugWindow = new Window
		{
			Name = "debug",
			Position = new Vector2(100, 100),
		};

		var lbAddWindow = new Label { Name = "AddWindow", Text = "AddItem window" };
		lbAddWindow.OnMouseEvent += (_) => AddWindow();
		_debugWindow.AddItem(lbAddWindow);

		var lbRemoveWindow = new Label { Text = "RemoveItem window" };
		lbRemoveWindow.OnMouseEvent += (_) => RemoveWindow();
		_debugWindow.AddItem(lbRemoveWindow);

		int btCounter = 0;
		var btCount = new Button { Text = "Button1", Size = new Vector2(200, 48) };
		btCount.Style.Background = Color.White * 1f;
		btCount.AddSkin(new ThreeSlice { Texture = _threeSlice, Source = _threeSlice, BorderSize = 32 });
		btCount.OnTriggerEvent += () => { btCount.Text = $"Button ({++btCounter})"; };
		_debugWindow.AddItem(btCount);

		_canvas.AddItem(_debugWindow);

		var win1 = new Window
		{
			Name = "win1",
			Position = new Vector2(350),
			Size = new Vector2(500),
			AutoSize = false,
		};
		var win1Skin = new NineSlice
		{
			Texture = _nineSlice,
			Source = _nineSlice,
			SliceSize = 64,
		};
		win1.AddSkin(win1Skin);


		win1.AddItem(new Label
		{
			Text = "WINDOW #1",
			Foreground = Color.White,
		});

		var bt1 = new Button
		{
			Text = "BAUTTON",
			Size = new Vector2(256, 64),
		};
		bt1.AddSkin(new ThreeSlice { Texture = _threeSlice, Source = _threeSlice, BorderSize = 32 });
		win1.AddItem(bt1);

		var spSlider = new StackPanel { ItemOrientation = Orientation.Horizontal };

		var slider = new Slider { Value = 1f };
		var sliderValue = new Label { Text = $"{slider.Value:N1}" };

		slider.OnValueChanged += (v) =>
		{
			win1Skin.BorderExtend = v;
			sliderValue.Text = $"{v:N1}";
		};
		spSlider.AddItem(slider);
		spSlider.AddItem(sliderValue);
		win1.AddItem(spSlider);

		var checkbox = new Checkbox();
		win1.AddItem(checkbox);

		var edlb = new EditLabel { Text = "Editable text" };
		win1.AddItem(edlb);

		var tb = new TextBox();
		win1.AddItem(tb);

		var lb = new ListBox
		{
			Spacing = 4,
			ScrollRate = 25f,
		};
		for (int j = 0; j < 56; j++)
		{
			//lb.AddItem(new Image { Texture = _atlas.Regions[1], Source = _atlas.Regions[1] });
			//lb.AddItem(new Label { Text = $"labbel {j}" });
			lb.AddItem(new Button { Text = $"butti {j}" });
		}
		win1.AddItem(lb);

		_canvas.AddItem(win1);
	}

	private void RemoveWindow()
	{
		var lastItem = _canvas.Items.LastOrDefault();
		if (lastItem != null)
			_canvas.RemoveItem(lastItem);
	}

	private void AddWindow()
	{
		var win = new Window
		{
			Name = "rnd_win" + _canvas.Items.Count,
			Position = new Vector2(Rng.Next(_canvas.VirtualViewport.Width), Rng.Next(_canvas.VirtualViewport.Height)),
		};
		win.AddItem(new Label
		{
			Text = $"i am window number #{_canvas.Items.Count} thanks lots poggy",
		});
		_canvas.AddItem(win);
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
		if (Input.KeyClick(Keys.F5))
			LoadTest();
		_canvas.Update(gt);
	}

	protected override void Render(GameTime gt)
	{
		base.Render(gt);
		_canvas.Render();
	}

	protected override void Draw(GameTime gt)
	{
		swDrawMs = Stopwatch.StartNew();
		base.Draw(gt);

		GraphicsDevice.SetRenderTarget(null);
		GraphicsDevice.Clear(Color.Black);

		var win1 = _canvas.GetItem<Window>("win1");
		if (win1 != null)
		{
			//win1.Position = Input.MouseTransformedPos(_canvas.TransformMatrix);
		}

		_canvas.Draw();
		swDrawMs.Stop();

		DrawDebug(gt);
	}

	public void DrawDebug(GameTime gt)
	{
		var sb = Graphics.SpriteBatch;
		var met = GraphicsDevice.Metrics;
		i = 0;


		elapsedTime += (float)gt.ElapsedGameTime.TotalSeconds;
		frameCount++;

		if (elapsedTime >= 1.0f) // Every second, update FPS
		{
			fps = frameCount / elapsedTime;
			frameCount = 0;
			elapsedTime = 0;
		}

		sb.Begin(samplerState: SamplerState.AnisotropicClamp);


		DrawText($"FPS: {fps:N2}");
		DrawText($"Draw MS: {swDrawMs.Elapsed.TotalMilliseconds}ms");
		DrawText($"Draw Calls: {met.DrawCount}");
		DrawText($"Texture Swaps: {met.TextureCount}");
		DrawText($"Sprite & Text Calls: {met.SpriteCount}");
		sb.End();
	}

	private float fps;
	private float elapsedTime;
	private int frameCount;
	private int i = 0;
	private Stopwatch swDrawMs;
	private TextRenderer _textRen;

	void DrawText(string text)
	{
		_textRen.Text = text;
		_textRen.Position = new Vector2(4, 300 + i++ * 16);
		_textRen.DrawText();
	}
}
