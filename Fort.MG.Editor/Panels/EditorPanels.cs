using Fort.MG.Editor.Fields;
using Fort.MG.EntitySystem;
using Fort.MG.Gui;
using Fort.MG.Gui.Components;
using Microsoft.Xna.Framework;

namespace Fort.MG.Editor.Panels;

/// <summary>
/// Base for the editor's floating panels. Each panel owns a <see cref="Window"/> that the
/// <see cref="EntityEditorSystem"/> adds to its canvas, and refreshes itself from the session.
/// </summary>
public abstract class EditorPanel
{
	protected readonly EntityEditorSession Session;

	public Window Window { get; }

	protected EditorPanel(EntityEditorSession session, string title, Vector2 position, Vector2 size)
	{
		Session = session;
		Window = new Window
		{
			Name = title,
			Position = position,
			Size = size,
			AutoSize = false,
		};
		Window.AddItem(new Label { Text = title, Foreground = Color.White });
	}

	/// <summary>Called once per frame while the editor is open.</summary>
	public virtual void Update()
	{
	}

	protected static Button MakeButton(string text, Action onClick, float width = 76f)
	{
		var button = new Button { Text = text, Size = new Vector2(width, 24f) };
		button.OnClick += onClick;
		return button;
	}

	protected static Label RowLabel(string text)
	{
		var label = new Label { Text = text, Foreground = StyleManager.Foreground2Color };
		label.Size = new Vector2(130f, label.Size.Y);
		return label;
	}
}

/// <summary>Save / reload / undo / redo / new, plus add-child and delete for the selected entity.</summary>
public sealed class ToolbarPanel : EditorPanel
{
	private readonly Button _undo;
	private readonly Button _redo;

	public ToolbarPanel(EntityEditorSession session, Vector2 position)
		: base(session, "Editor", position, new Vector2(660f, 70f))
	{
		var row = new StackPanel
		{
			ItemOrientation = Orientation.Horizontal,
			Spacing = 4,
			AutoSize = true,
		};

		row.AddItem(MakeButton("Save", () => Session.Save()));
		row.AddItem(MakeButton("Reload", () => Session.Reload()));

		_undo = MakeButton("Undo", () => Session.Undo());
		_redo = MakeButton("Redo", () => Session.Redo());
		row.AddItem(_undo);
		row.AddItem(_redo);

		row.AddItem(MakeButton("New", () => Session.NewTemplate("untitled")));
		row.AddItem(MakeButton("Add Child", OnAddChild, 90f));
		row.AddItem(MakeButton("Delete", OnDelete));

		Window.AddItem(row);
	}

	private void OnAddChild()
	{
		if (Session.SelectedEntity != null)
			Session.AddChild(Session.SelectedEntity);
	}

	private void OnDelete()
	{
		if (Session.SelectedEntity != null)
			Session.RemoveEntity(Session.SelectedEntity);
	}

	public override void Update()
	{
		_undo.IsEnabled = Session.CanUndo;
		_redo.IsEnabled = Session.CanRedo;
	}
}

/// <summary>Lists the mapped template files and loads the clicked one.</summary>
public sealed class TemplateBrowserPanel : EditorPanel
{
	private readonly ListBox _list = new() { Spacing = 2, Size = new Vector2(200f, 200f) };

	private List<string> _names = new();
	private bool _suppressSelection;

	public TemplateBrowserPanel(EntityEditorSession session, Vector2 position)
		: base(session, "Templates", position, new Vector2(220f, 290f))
	{
		_list.SelectedItemChangedEvent += _ => OnSelected();
		Window.AddItem(_list);
		Window.AddItem(MakeButton("Refresh", Refresh, 100f));
		Refresh();
	}

	public void Refresh()
	{
		_names = new List<string>(Session.GetTemplateNames());

		_suppressSelection = true;
		_list.Items.Clear();
		_list.SelectedIndex = -1;

		foreach (var name in _names)
			_list.AddItem(new Label { Text = name, Foreground = StyleManager.Foreground1Color });

		_list.UpdateItemTransforms();
		_suppressSelection = false;
	}

	private void OnSelected()
	{
		if (_suppressSelection)
			return;

		var index = _list.SelectedIndex;
		if (index < 0 || index >= _names.Count)
			return;

		Session.LoadTemplate(_names[index]);
	}
}

/// <summary>Depth-first tree of the working entity and its children; click selects an entity.</summary>
public sealed class HierarchyPanel : EditorPanel
{
	private readonly ListBox _list = new() { Spacing = 2, Size = new Vector2(220f, 200f) };
	private readonly List<Entity> _entities = new();

	private int _lastStructure = -1;
	private bool _suppressSelection;

	public HierarchyPanel(EntityEditorSession session, Vector2 position)
		: base(session, "Hierarchy", position, new Vector2(240f, 300f))
	{
		_list.SelectedItemChangedEvent += _ => OnSelected();
		Window.AddItem(_list);
		Window.AddItem(MakeButton("Add Child", () =>
		{
			if (Session.SelectedEntity != null)
				Session.AddChild(Session.SelectedEntity);
		}, 100f));
	}

	public override void Update()
	{
		if (Session.StructureVersion != _lastStructure)
			Rebuild();
	}

	private void Rebuild()
	{
		_lastStructure = Session.StructureVersion;

		_entities.Clear();
		_suppressSelection = true;
		_list.Items.Clear();
		_list.SelectedIndex = -1;

		if (Session.Root != null)
		{
			foreach (var entity in EntityEditorSession.Flatten(Session.Root))
			{
				_entities.Add(entity);
				_list.AddItem(new Label
				{
					Text = new string(' ', Depth(entity) * 2) + (entity.Name ?? "<unnamed>"),
					Foreground = ReferenceEquals(entity, Session.SelectedEntity)
						? Color.Orange
						: StyleManager.Foreground1Color,
				});
			}
		}

		_list.UpdateItemTransforms();
		_suppressSelection = false;
	}

	private void OnSelected()
	{
		if (_suppressSelection)
			return;

		var index = _list.SelectedIndex;
		if (index < 0 || index >= _entities.Count)
			return;

		Session.Select(_entities[index]);
	}

	private static int Depth(Entity entity)
	{
		var depth = 0;
		var parent = entity.Parent;
		while (parent != null)
		{
			depth++;
			parent = parent.Parent;
		}

		return depth;
	}
}

/// <summary>
/// Shows the selected entity: its name, its transform (serialized via TransformData, so it is not a
/// component field), and one <see cref="PropertyRow"/> per serializable component field.
/// </summary>
public sealed class InspectorPanel : EditorPanel
{
	private readonly StackPanel _content = new() { AutoSize = true, Spacing = 3 };
	private readonly Dropdown _addComponent = new();

	private Entity? _lastEntity;
	private Component? _lastComponent;
	private int _lastComponentCount = -1;

	public InspectorPanel(EntityEditorSession session, Vector2 position)
		: base(session, "Inspector", position, new Vector2(380f, 470f))
	{
		_addComponent.Items = new List<string>(ComponentFieldModel.GetComponentNames());
		_addComponent.OnSelectionChanged += name => Session.AddComponent(name);

		Window.AddItem(_content);
		Window.AddItem(new Label { Text = "add component", Foreground = StyleManager.Foreground2Color });
		Window.AddItem(_addComponent);

		Rebuild();
	}

	public override void Update()
	{
		var entity = Session.SelectedEntity;
		var component = Session.SelectedComponent;
		var count = entity?.Components.Count ?? 0;

		// Deliberately keyed on identity + component count rather than a version counter: a rename or
		// a field edit must NOT rebuild the panel, or the TextBox being typed into would be destroyed.
		if (ReferenceEquals(entity, _lastEntity) && ReferenceEquals(component, _lastComponent) &&
		    count == _lastComponentCount)
			return;

		Rebuild();
	}

	private void Rebuild()
	{
		var entity = Session.SelectedEntity;
		_lastEntity = entity;
		_lastComponent = Session.SelectedComponent;
		_lastComponentCount = entity?.Components.Count ?? 0;

		_content.Items.Clear();

		if (entity == null)
		{
			_content.AddItem(new Label { Text = "no entity selected", Foreground = Color.Gray });
			return;
		}

		var nameRow = new StackPanel { ItemOrientation = Orientation.Horizontal, Spacing = 4, AutoSize = true };
		nameRow.AddItem(RowLabel("name"));
		nameRow.AddItem(FieldWidgets.MakeTextBox(entity.Name ?? string.Empty, text => Session.SetEntityName(entity, text)));
		_content.AddItem(nameRow);

		_content.AddItem(new Label { Text = "transform", Foreground = Color.DarkKhaki });
		_content.AddItem(VectorRow("position",
			[entity.Transform.LocalPosition3.X, entity.Transform.LocalPosition3.Y, entity.Transform.LocalPosition3.Z],
			parts => new Vector3(parts[0], parts[1], parts[2]),
			value =>
			{
				entity.Transform.LocalPosition3 = (Vector3)value;
				Session.Commit();
			}));
		_content.AddItem(VectorRow("size",
			[entity.Transform.Size.X, entity.Transform.Size.Y],
			parts => new Vector2(parts[0], parts[1]),
			value =>
			{
				entity.Transform.Size = (Vector2)value;
				Session.Commit();
			}));

		foreach (var component in entity.Components)
		{
			if (component is Transform)
				continue;

			_content.AddItem(ComponentHeader(component));

			var fields = ComponentFieldModel.GetFields(component.GetType());
			if (fields.Count == 0)
			{
				_content.AddItem(new Label { Text = "  (no serializable fields)", Foreground = Color.Gray });
				continue;
			}

			foreach (var field in fields)
				_content.AddItem(new PropertyRow(Session, component, field));
		}
	}

	private StackPanel ComponentHeader(Component component)
	{
		var row = new StackPanel { ItemOrientation = Orientation.Horizontal, Spacing = 4, AutoSize = true };
		row.AddItem(new Label
		{
			Text = ComponentFieldModel.GetComponentTypeName(component.GetType()) + ":",
			Foreground = Color.DarkKhaki,
		});
		row.AddItem(MakeButton("remove", () => Session.RemoveComponent(component), 70f));
		return row;
	}

	private static StackPanel VectorRow(string label, float[] initial, Func<float[], object> build, Action<object> commit)
	{
		var row = new StackPanel { ItemOrientation = Orientation.Horizontal, Spacing = 4, AutoSize = true };
		row.AddItem(RowLabel(label));
		row.AddItem(FieldWidgets.MakeNumericVectorRow(initial, build, commit));
		return row;
	}
}
