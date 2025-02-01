using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Fort.MG.Core;

public static class Input
{
	private static Game Game => FortCore.Game;

	public static KeyboardState KeyState;
	public static MouseState MouseState;
	public static KeyboardState KeyStateOld;
	public static MouseState MouseStateOld;
	public static int ScrollValue => MouseState.ScrollWheelValue;
	public static int ScrollValueOld;

	public static Vector2 MousePos => new Vector2(MouseState.X, MouseState.Y);
	public static Vector2 OldMousePos => new Vector2(MouseStateOld.X, MouseStateOld.Y);

	public static Vector2 MouseTransformedPos(Matrix matrix) => Vector2.Transform(MousePos, Matrix.Invert(matrix));
	public static Vector2 OldMouseTransformedPos(Matrix matrix) => Vector2.Transform(OldMousePos, Matrix.Invert(matrix));

	public static void Update()
	{
		if (!Game.IsActive)
			return;

		KeyStateOld = KeyState;
		KeyState = Keyboard.GetState();

		MouseStateOld = MouseState;
		MouseState = Mouse.GetState();
	}

	public static void PostUpdate()
	{
		ScrollValueOld = ScrollValue;
	}

	public static bool AnyKeyDown => KeyState.GetPressedKeys().Length != 0;
	public static bool AnyMouseButtonDown => LeftHold || RightHold || MiddleHold;

	public static bool KeyClick(Keys key) => KeyState.IsKeyDown(key) && KeyStateOld.IsKeyUp(key);
	public static bool KeyClick(params Keys[] keys) => keys.Any(x => KeyState.IsKeyDown(x) && KeyStateOld.IsKeyUp(x));

	public static bool KeyRelease(Keys key) => KeyState.IsKeyUp(key) && KeyStateOld.IsKeyDown(key);
	public static bool KeyRelease(params Keys[] keys) => keys.Any(x => KeyState.IsKeyUp(x) && KeyStateOld.IsKeyDown(x));

	public static bool KeyHold(Keys key) => KeyState.IsKeyDown(key);
	public static bool KeyHold(params Keys[] keys) => keys.Any(x => KeyState.IsKeyDown(x));

	public static bool LeftClickRec(Rectangle rec) => rec.Contains(MousePos) && LeftClick;
	public static bool LeftClick => MouseState.LeftButton == ButtonState.Pressed && MouseStateOld.LeftButton == ButtonState.Released;
	public static bool LeftRelease => MouseState.LeftButton == ButtonState.Released && MouseStateOld.LeftButton == ButtonState.Pressed;
	public static bool LeftHold => MouseState.LeftButton == ButtonState.Pressed;

	public static bool RightClickRec(Rectangle rec) => rec.Contains(MousePos) && RightClick;
	public static bool RightClick => MouseState.RightButton == ButtonState.Pressed && MouseStateOld.RightButton == ButtonState.Released;
	public static bool RightRelease => MouseState.RightButton == ButtonState.Released && MouseStateOld.RightButton == ButtonState.Pressed;
	public static bool RightHold => MouseState.RightButton == ButtonState.Pressed;

	public static bool MiddleClick => MouseState.MiddleButton == ButtonState.Pressed && MouseStateOld.MiddleButton == ButtonState.Released;
	public static bool MiddleRelease => MouseState.MiddleButton == ButtonState.Released && MouseStateOld.MiddleButton == ButtonState.Pressed;
	public static bool MiddleHold => MouseState.MiddleButton == ButtonState.Pressed;

	public static bool Mouse4Click => MouseState.XButton1 == ButtonState.Pressed && MouseStateOld.XButton1 == ButtonState.Released;
	public static bool Mouse4Release => MouseState.XButton1 == ButtonState.Released && MouseStateOld.XButton1 == ButtonState.Pressed;
	public static bool Mouse4Hold => MouseState.XButton1 == ButtonState.Pressed;

	public static bool Mouse5Click => MouseState.XButton2 == ButtonState.Pressed && MouseStateOld.XButton2 == ButtonState.Released;
	public static bool Mouse5Release => MouseState.XButton2 == ButtonState.Released && MouseStateOld.XButton2 == ButtonState.Pressed;
	public static bool Mouse5Hold => MouseState.XButton2 == ButtonState.Pressed;

	public static bool ShiftLeftDown => KeyHold(Keys.LeftShift);
	public static bool ShiftRightDown => KeyHold(Keys.RightShift);
	public static bool ControlLeftDown => KeyHold(Keys.LeftControl);
	public static bool ControlRightDown => KeyHold(Keys.RightControl);

	/// <summary>
	/// after use it in IF -- write under the IF: Input.ScrollValueOld = Input.ScrollValue;
	/// </summary>
	public static bool WheelUp => ScrollValue > ScrollValueOld;
	/// <summary>
	/// after use it in IF -- write under the IF: Input.ScrollValueOld = Input.ScrollValue;
	/// </summary>
	public static bool WheelDown => ScrollValue < ScrollValueOld;
}
