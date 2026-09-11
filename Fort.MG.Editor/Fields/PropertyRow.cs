using Fort.MG.EntitySystem;
using Fort.MG.Gui;
using Fort.MG.Gui.Components;
using Microsoft.Xna.Framework;

namespace Fort.MG.Editor.Fields;

/// <summary>
/// One editable row in the inspector: the field's serialization key on the left, and a widget chosen
/// from the field's CLR type on the right. Edits go through the session, so they are undoable and
/// land on the working entity immediately.
///
/// Widget mapping: bool→Checkbox, enum→Dropdown, string→TextBox, numeric→TextBox, Vector2/Vector3
/// and Color→a row of numeric TextBoxes (plus a swatch for Color). Types the editor cannot safely
/// author (sprite/data-object references, or anything unknown) render read-only rather than silently
/// writing garbage into the template.
/// </summary>
public sealed class PropertyRow : StackPanel
{
	private readonly EntityEditorSession _session;
	private readonly Component _component;
	private readonly ComponentField _field;

	public PropertyRow(EntityEditorSession session, Component component, ComponentField field)
	{
		_session = session;
		_component = component;
		_field = field;

		ItemOrientation = Orientation.Horizontal;
		AutoSize = true;
		Spacing = 4;

		var label = new Label
		{
			Text = field.Name,
			Foreground = StyleManager.Foreground2Color,
		};
		label.Size = new Vector2(130f, label.Size.Y);
		AddItem(label);

		var editor = CreateEditor(field.GetValue(component));
		if (editor != null)
			AddItem(editor);
	}

	private GuiComponent? CreateEditor(object? value)
	{
		var type = _field.FieldType;

		if (type == typeof(bool))
		{
			var checkbox = new Checkbox
			{
				IsChecked = value is true,
				Size = new Vector2(20f, 20f),
			};
			checkbox.OnCheckedChanged += v => Commit(v);
			return checkbox;
		}

		if (type.IsEnum)
		{
			var dropdown = new Dropdown();
			dropdown.Items = new List<string>(Enum.GetNames(type));
			dropdown.SelectedItem = value?.ToString() ?? dropdown.Items[0];
			dropdown.OnSelectionChanged += name =>
			{
				if (Enum.TryParse(type, name, out var parsed))
					Commit(parsed);
			};
			return dropdown;
		}

		if (type == typeof(string))
			return FieldWidgets.MakeTextBox(value as string ?? string.Empty, Commit);

		if (FieldWidgets.IsNumeric(type))
		{
			return FieldWidgets.MakeTextBox(FieldWidgets.Format(value), text =>
			{
				if (FieldWidgets.TryParseNumber(text, type, out var number))
					Commit(number);
			});
		}

		if (type == typeof(Vector2))
		{
			var v = value is Vector2 vec ? vec : Vector2.Zero;
			return FieldWidgets.MakeNumericVectorRow([v.X, v.Y], parts => new Vector2(parts[0], parts[1]), Commit);
		}

		if (type == typeof(Vector3))
		{
			var v = value is Vector3 vec ? vec : Vector3.Zero;
			return FieldWidgets.MakeNumericVectorRow([v.X, v.Y, v.Z],
				parts => new Vector3(parts[0], parts[1], parts[2]), Commit);
		}

		if (type == typeof(Color))
			return FieldWidgets.MakeColorRow(value is Color c ? c : Color.White, colour => Commit(colour));

		// SpriteRegion / DataObject references and anything else: show the current value, don't edit it.
		return new Label
		{
			Text = $"{Describe(value)} (read-only)",
			Foreground = Color.Gray,
		};
	}

	private void Commit(object? value) => _session.SetFieldValue(_component, _field, value);

	private static string Describe(object? value) => value switch
	{
		null => "null",
		Fort.MG.Assets.Data.SpriteRegion region => region.Name,
		Fort.MG.EntitySystem.Parsing.DataObject data => data.Name,
		_ => value.ToString() ?? "?",
	};
}
