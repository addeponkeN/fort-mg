using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System.Text;
using Fort.MG.Extensions;

namespace Fort.MG.Gui.Components;

public class EditLabel : GuiComponent
{
	private readonly char[] _splitters = [' ', '.', ';', ':', '!', '?', '(', ')', '[', ']', '{', '}', '-', '/', '\\', '*', '&', '^', '%', '$', '#', '@', '"', '\''];

	private readonly Dictionary<char, float> _characterWidths = new();
	private StringBuilder _text = new();
	private int _cursorPosition = 0;
	private int _prevCursorPosition = 0;
	private float _cursorPositionX = 0;
	private int _selectionStart = -1;
	private double _lastBlinkTime;
	private bool _cursorVisible = true;
	private const double CursorBlinkRate = 0.5;
	private Label _label;

	public Label Label
	{
		get => _label;
		set => SetComponent(ref _label, value);
	}

	public string Text
	{
		get => _text.ToString();
		set
		{
			if (_text.ToString() != value)
			{
				_text.Clear();
				_text.Append(value);
				_cursorPosition = _text.Length;
				_selectionStart = -1;
				UpdateText(value);
			}
		}
	}

	public override Vector2 Position
	{
		get => base.Position;
		set
		{
			base.Position = value;
			if (Label != null)
				Label.Position = value;
		}
	}

	public Color MarkerColor { get; set; } = new Color(220, 220, 220, 255);

	public event Action<string> OnTextChanged;

	public EditLabel()
	{
		base.Size = new Vector2(200, 20);
		base.Foreground = new Color(200, 200, 200, 255);
	}

	public override void Start()
	{
		base.Start();
		Label = new() { Text = Text };
		Input.OnTextInput += Input_OnTextInput;
	}

	private void Input_OnTextInput(TextInputEventArgs e)
	{
		if (!IsFocused) return;

		switch (e.Key)
		{
			case Keys.Back:
				DeleteCharacter(-1);
				break;
			case Keys.Delete:
				DeleteCharacter(1);
				break;
			default:
				if (!Input.CtrlDown)
					InsertText(e.Character.ToString());
				break;
		}
	}

	public override void Update(GameTime gt)
	{
		base.Update(gt);

		//if (Input.LeftClick)
		//{
		//	_isFocused = false;
		//}

		if (Bounds.Contains(Canvas.MousePosition))
		{
			if (Input.LeftClick)
			{
				//_isFocused = true;
				_cursorPosition = GetCursorPositionFromMouse();
				_selectionStart = _cursorPosition;
			}
			else if (Input.LeftHold)
			{
				//_isFocused = true;
				_cursorPosition = GetCursorPositionFromMouse();
			}
		}


		if (!IsFocused) return;

		_lastBlinkTime += gt.Delta();
		if (_lastBlinkTime >= CursorBlinkRate)
		{
			_cursorVisible = !_cursorVisible;
			_lastBlinkTime = 0;
		}

		HandleKeyboardInput();
	}

	private void HandleKeyboardInput()
	{
		if (Input.CtrlDown && Input.KeyClick(Keys.A)) SelectAll();

		if (Input.KeyClick(Keys.Left)) MoveCursor(-1);
		if (Input.KeyClick(Keys.Right)) MoveCursor(1);

		if (Input.KeyClick(Keys.Home)) MoveCursorToStart();
		if (Input.KeyClick(Keys.End)) MoveCursorToEnd();

		if (Input.CtrlDown && Input.KeyClick(Keys.C)) CopySelection();
		if (Input.CtrlDown && Input.KeyClick(Keys.X)) CutSelection();
		if (Input.CtrlDown && Input.KeyClick(Keys.V)) PasteClipboard();
	}

	private void SelectAll()
	{
		_selectionStart = 0;
		SetCursorPosition(_text.Length);
	}

	private void MoveCursor(int direction)
	{
		if (_selectionStart == -1 && Input.ShiftDown)
			_selectionStart = _cursorPosition;

		int position = _cursorPosition;

		if (Input.CtrlDown)
			position = GetNextWordBoundary(position, direction);
		else
			position = Math.Clamp(position + direction, 0, _text.Length);

		SetCursorPosition(position);

		if (!Input.ShiftDown)
			_selectionStart = -1;
	}

	private int GetNextWordBoundary(int position, int direction)
	{
		if (_text.Length == 0) return 0;

		int newPos = position + direction;

		if (direction > 0)
		{
			while (newPos < _text.Length && !_splitters.Contains(_text[newPos]))
				newPos++;

			while (newPos < _text.Length && (_splitters.Contains(_text[newPos])))
				newPos++;
		}
		else
		{
			while (newPos > 0 && !_splitters.Contains(_text[newPos - 1]))
				newPos--;

			while (newPos > 0 && (_splitters.Contains(_text[newPos - 1])))
				newPos--;
		}

		return Math.Clamp(newPos, 0, _text.Length);
	}

	private void MoveCursorToStart()
	{
		if (_selectionStart == -1)
			_selectionStart = _cursorPosition;

		SetCursorPosition(0);
	}

	private void MoveCursorToEnd()
	{
		if (_selectionStart == -1)
			_selectionStart = _cursorPosition;

		SetCursorPosition(_text.Length);
	}

	private void SetCursorPosition(int i)
	{
		_cursorPosition = i;
		_cursorVisible = true;
		_lastBlinkTime = 0;
	}

	private void DeleteCharacter(int direction)
	{
		if (_selectionStart != -1)
		{
			DeleteSelection();
			return;
		}

		if (direction == -1 && _cursorPosition > 0)
		{
			int length = Input.CtrlDown ? _cursorPosition - GetNextWordBoundary(_cursorPosition, direction) : 1;
			_text.Remove(_cursorPosition - length, length);
			_cursorPosition -= length;
			UpdateText(_text.ToString());
		}

		else if (direction == 1 && _cursorPosition < _text.Length)
		{
			int length = Math.Abs(Input.CtrlDown ? _cursorPosition - GetNextWordBoundary(_cursorPosition, direction) : 1);
			_text.Remove(_cursorPosition, length);
			UpdateText(_text.ToString());
		}
	}

	private void InsertText(string inputText)
	{
		DeleteSelection();
		_text.Insert(_cursorPosition, inputText);
		SetCursorPosition(_cursorPosition + inputText.Length);
		UpdateText(_text.ToString());
	}

	private void UpdateText(string text)
	{
		if (Label == null) return;
		Label.Text = text;
		OnTextChanged?.Invoke(_text.ToString());
	}

	private void DeleteSelection()
	{
		if (_selectionStart == -1) return;

		int start = Math.Min(_cursorPosition, _selectionStart);
		int length = Math.Abs(_cursorPosition - _selectionStart);

		_text.Remove(start, length);
		_cursorPosition = start;
		_selectionStart = -1;

		UpdateText(_text.ToString());
	}

	private void CopySelection()
	{
		if (_selectionStart == -1) return;
		int start = Math.Min(_cursorPosition, _selectionStart);
		int length = Math.Abs(_cursorPosition - _selectionStart);
		//Input.SetClipboard(_text.ToString(start, length));
	}

	private void CutSelection()
	{
		CopySelection();
		DeleteSelection();
	}

	private void PasteClipboard()
	{
		//InsertText(Input.GetClipboard());
	}

	private int GetCursorPositionFromMouse()
	{
		float x = Canvas.MousePosition.X - Position.X;
		float totalWidth = 0;

		for (int i = 0; i < _text.Length; i++)
		{
			float charWidth = GetCharacterWidth(_text[i]);
			if (x < totalWidth + charWidth / 2)
				return i;
			totalWidth += charWidth;
		}

		return _text.Length;
	}

	private float GetCharacterWidth(char c)
	{
		if (_characterWidths.TryGetValue(c, out float w))
			return w;

		w = Label.Font.MeasureString(c.ToString()).X;
		_characterWidths.Add(c, w);

		return w;
	}

	internal float GetCursorXPosition()
	{
		if (_cursorPosition == _prevCursorPosition)
			return _cursorPositionX;

		_prevCursorPosition = _cursorPosition;
		_cursorPositionX = 0f;

		for (int i = 0; i < _cursorPosition; i++)
		{
			_cursorPositionX += GetCharacterWidth(_text[i]);
		}

		return _cursorPositionX;
	}

	public override void Draw()
	{
		Label.Foreground = Foreground;
		Label.Position = Position;

		base.Draw();

		var sb = Graphics.SpriteBatch;

		if (_selectionStart != -1 && _selectionStart != _cursorPosition)
		{
			int start = Math.Min(_cursorPosition, _selectionStart);
			int end = Math.Max(_cursorPosition, _selectionStart);

			float startX = Position.X;
			float endX = Position.X;

			for (int i = 0; i < start; i++) startX += GetCharacterWidth(_text[i]);
			for (int i = 0; i < end; i++) endX += GetCharacterWidth(_text[i]);

			sb.Draw(GuiContent.Pixel,
				new Rectangle((int)startX, (int)Position.Y, (int)(endX - startX), (int)Size.Y),
				new Color(100, 100, 255, 100));
		}

		if (IsFocused && _cursorVisible)
		{
			float cursorX = Position.X + GetCursorXPosition();
			sb.Draw(GuiContent.Pixel, new Rectangle((int)cursorX, (int)Position.Y, 1, (int)Size.Y), MarkerColor);
		}
	}

	public override void DrawText()
	{
		base.DrawText();

	}
}
