using System.Globalization;
using Fort.MG.Gui;
using Fort.MG.Gui.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Fort.MG.Editor.Fields;

/// <summary>
/// Shared widget builders used by both the reflection-driven inspector rows and the hand-built
/// transform rows, so every numeric/colour field behaves identically.
///
/// Numeric and colour values are parsed/format with <see cref="CultureInfo.InvariantCulture"/> to
/// match the YAML converters; a culture-specific parse is accepted as a fallback so a comma-decimal
/// keyboard still works. Values commit live, but only once the text parses - so intermediate typing
/// like "-" or "" never writes a bogus value into the template.
/// </summary>
public static class FieldWidgets
{
	private static Texture2D? _swatchTexture;

	/// <summary>
	/// 1x1 white texture the colour swatch is tinted with. The editor owns its own copy rather than
	/// reaching into Fort.MG.Gui's internal <c>GuiContent</c>.
	/// </summary>
	private static Texture2D SwatchTexture
	{
		get
		{
			if (_swatchTexture == null || _swatchTexture.IsDisposed)
			{
				_swatchTexture = new Texture2D(Graphics.GraphicsDevice, 1, 1);
				_swatchTexture.SetData([Color.White]);
			}

			return _swatchTexture;
		}
	}

	public static TextBox MakeTextBox(string text, Action<string> onChanged)
	{
		var box = new TextBox
		{
			Size = new Vector2(120f, 20f),
			Foreground = StyleManager.Foreground1Color,
		};

		// TextBox builds its EditLabel in Start() and only surfaces edits through it, so start the
		// widget now and seed the initial text before the framework's first update.
		box.Start();
		box.EditLabel.Text = text;
		box.EditLabel.OnTextChanged += args => onChanged(args.Text);

		return box;
	}

	public static StackPanel MakeNumericVectorRow(float[] initial, Func<float[], object> build, Action<object> commit)
	{
		var row = new StackPanel
		{
			ItemOrientation = Orientation.Horizontal,
			Spacing = 2,
			AutoSize = true,
		};

		var parts = (float[])initial.Clone();

		for (var i = 0; i < parts.Length; i++)
		{
			var index = i;
			row.AddItem(MakeTextBox(Format(parts[index]), text =>
			{
				if (!TryParseFloat(text, out var parsed))
					return;

				parts[index] = parsed;
				commit(build(parts));
			}));
		}

		return row;
	}

	public static StackPanel MakeColorRow(Color initial, Action<Color> commit)
	{
		var row = new StackPanel
		{
			ItemOrientation = Orientation.Horizontal,
			Spacing = 2,
			AutoSize = true,
		};

		var swatch = new Image
		{
			Texture = SwatchTexture,
			Source = new Rectangle(0, 0, 1, 1),
			Size = new Vector2(18f, 18f),
			Foreground = initial,
		};
		row.AddItem(swatch);

		var channels = new[] { initial.R, initial.G, initial.B, initial.A };

		for (var i = 0; i < channels.Length; i++)
		{
			var index = i;
			row.AddItem(MakeTextBox(channels[index].ToString(CultureInfo.InvariantCulture), text =>
			{
				if (!byte.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var channel))
					return;

				channels[index] = channel;
				var colour = new Color(channels[0], channels[1], channels[2], channels[3]);
				swatch.Foreground = colour;
				commit(colour);
			}));
		}

		return row;
	}

	public static string Format(object? value) => value switch
	{
		null => string.Empty,
		float f => f.ToString("0.###", CultureInfo.InvariantCulture),
		double d => d.ToString("0.###", CultureInfo.InvariantCulture),
		IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
		_ => value.ToString() ?? string.Empty,
	};

	public static bool TryParseFloat(string text, out float value) =>
		float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value) ||
		float.TryParse(text, NumberStyles.Float, CultureInfo.CurrentCulture, out value);

	public static bool TryParseNumber(string text, Type type, out object? value)
	{
		value = null;

		if (!TryParseFloat(text, out var parsed))
			return false;

		value = type == typeof(float)
			? parsed
			: Convert.ChangeType(parsed, type, CultureInfo.InvariantCulture);

		return true;
	}

	public static bool IsNumeric(Type type) =>
		type == typeof(int) || type == typeof(long) || type == typeof(float) || type == typeof(double) ||
		type == typeof(uint) || type == typeof(short) || type == typeof(byte);
}
