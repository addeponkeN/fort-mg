using Fort.MG.Assets;
using Fort.MG.Editor.Fields;
using Fort.MG.EntitySystem;
using Fort.MG.EntitySystem.Parsing;
using Microsoft.Xna.Framework;

namespace Fort.MG.Editor;

/// <summary>
/// Editing state for one entity template: the working entity tree, the current selection, a dirty
/// flag, and a YAML-snapshot undo/redo history.
///
/// The working tree is deliberately <b>detached</b> from any scene — it is built with
/// <see cref="Entity.Create"/> and linked via <see cref="Entity.Parent"/>, never through
/// <c>Entity.Instantiate</c>, so editing cannot leak objects into the running game. Undo/redo
/// snapshots reuse the template serializer, so what the editor restores is exactly what it saves.
/// </summary>
public sealed class EntityEditorSession
{
	private const int MaxHistory = 64;

	private readonly List<string> _history = new();
	private int _historyIndex = -1;

	private string? _sourceExtends;
	private int _structureAtLoad;

	private static readonly List<Entity> FlattenStack = new();

	public Entity? Root { get; private set; }

	/// <summary>Registry name of the loaded template (lower-case file name), when any.</summary>
	public string? TemplateName { get; private set; }

	/// <summary>File the template was loaded from / will be saved to.</summary>
	public string? TemplatePath { get; private set; }

	public bool IsDirty { get; private set; }

	public bool HasDocument => Root != null;

	/// <summary>
	/// Bumped whenever the document *structure* changes (load, new, undo/redo, add/remove of
	/// entities or components) but NOT when a field value is edited. Panels use it to decide when
	/// they must be rebuilt, so typing into a field never destroys the widget being typed into.
	/// </summary>
	public int StructureVersion { get; private set; }

	public Entity? SelectedEntity { get; private set; }
	public Component? SelectedComponent { get; private set; }

	public bool CanUndo => _historyIndex > 0;
	public bool CanRedo => _historyIndex >= 0 && _historyIndex < _history.Count - 1;

	/// <summary>Raised whenever the working tree, selection, or dirty state changes.</summary>
	public event Action? Changed;

	// ---------------------------------------------------------------------
	// Document lifecycle
	// ---------------------------------------------------------------------

	public IReadOnlyList<string> GetTemplateNames()
	{
		var names = new List<string>(EntityDatabase.GetAllTemplateNames());
		names.Sort(StringComparer.OrdinalIgnoreCase);
		return names;
	}

	public bool LoadTemplate(string name)
	{
		var path = EntityDatabase.GetTemplatePath(name);
		if (path == null || !File.Exists(path))
			return false;

		var yaml = File.ReadAllText(path);
		var entities = EntitySerializer.DeserializeEntityTemplates(yaml);
		if (entities.Count == 0)
			return false;

		// Remember the template-level "extends" key: DeserializeEntityTemplates resolves inheritance,
		// so the resolved entity tree no longer carries it and it would be lost on the next save.
		var templates = YamlSerializationFactory.Deserializer.Deserialize<List<EntityTemplate>>(yaml);

		Reset(entities[0], name, path, templates.Count > 0 ? templates[0].Extends : null);
		return true;
	}

	public bool Reload()
	{
		return TemplateName != null && LoadTemplate(TemplateName);
	}

	/// <summary>Creates an empty template. <paramref name="path"/> may be null until first save.</summary>
	public void NewTemplate(string name, string? path = null)
	{
		var root = Entity.Create();
		root.Name = name;
		root.Transform.Size = new Vector2(32f, 32f);
		Reset(root, name, path, null);
	}

	private void Reset(Entity root, string name, string? path, string? sourceExtends)
	{
		Root = root;
		TemplateName = name;
		TemplatePath = path;
		_sourceExtends = sourceExtends;
		SelectedEntity = root;
		SelectedComponent = null;
		IsDirty = false;
		StructureVersion++;
		_structureAtLoad = StructureVersion;

		_history.Clear();
		_history.Add(EntitySerializer.SerializeEntityTemplates(new[] { root }));
		_historyIndex = 0;

		Changed?.Invoke();
	}

	public bool Save()
	{
		if (Root == null || TemplateName == null)
			return false;

		var path = TemplatePath
		           ?? EntityDatabase.GetTemplatePath(TemplateName)
		           ?? Path.Combine(AssetManager.ContentRoot, EntityDatabase.TemplatesFolderName, TemplateName + ".yaml");

		var directory = Path.GetDirectoryName(path);
		if (!string.IsNullOrEmpty(directory))
			Directory.CreateDirectory(directory);

		// Write through the template overload so template-level metadata ("extends") survives; the
		// entity-based overload cannot carry it because inheritance is resolved at load time.
		var template = EntitySerializer.CreateTemplateFromEntity(Root);

		if (!string.IsNullOrEmpty(_sourceExtends))
		{
			template.Extends = _sourceExtends;

			if (StructureVersion != _structureAtLoad)
				Logger.Warn($"Template '{TemplateName}' extends '{_sourceExtends}'; saving after structural " +
				            "edits writes the resolved component set alongside the 'extends' key.");
		}

		File.WriteAllText(path, EntitySerializer.SerializeTemplates(new[] { template }));

		TemplatePath = path;
		IsDirty = false;
		Changed?.Invoke();
		return true;
	}

	// ---------------------------------------------------------------------
	// Undo / redo (whole-document YAML snapshots)
	// ---------------------------------------------------------------------

	/// <summary>Records the current tree as the latest history state (call after a mutation).</summary>
	public void Commit()
	{
		if (Root == null)
			return;

		if (_historyIndex < _history.Count - 1)
			_history.RemoveRange(_historyIndex + 1, _history.Count - _historyIndex - 1);

		_history.Add(EntitySerializer.SerializeEntityTemplates(new[] { Root }));

		if (_history.Count > MaxHistory)
			_history.RemoveAt(0);

		_historyIndex = _history.Count - 1;
		IsDirty = true;
		Changed?.Invoke();
	}

	public bool Undo()
	{
		if (!CanUndo)
			return false;

		_historyIndex--;
		RestoreSnapshot();
		return true;
	}

	public bool Redo()
	{
		if (!CanRedo)
			return false;

		_historyIndex++;
		RestoreSnapshot();
		return true;
	}

	private void RestoreSnapshot()
	{
		var entities = EntitySerializer.DeserializeEntityTemplates(_history[_historyIndex]);
		if (entities.Count == 0)
			return;

		Root = entities[0];
		SelectedEntity = Root;
		SelectedComponent = null;
		IsDirty = true;
		StructureVersion++;
		Changed?.Invoke();
	}

	// ---------------------------------------------------------------------
	// Selection
	// ---------------------------------------------------------------------

	public void Select(Entity? entity, Component? component = null)
	{
		SelectedEntity = entity;
		SelectedComponent = component;
		Changed?.Invoke();
	}

	// ---------------------------------------------------------------------
	// Mutations (each ends in Commit() so it is undoable)
	// ---------------------------------------------------------------------

	public bool SetFieldValue(Component component, ComponentField field, object? value)
	{
		if (component == null || field == null)
			return false;

		field.SetValue(component, value);
		Commit();
		return true;
	}

	/// <summary>
	/// Renames an entity. Bumps <see cref="StructureVersion"/> so the hierarchy list re-renders; the
	/// inspector deliberately does not rebuild on that counter, so typing a name keeps focus.
	/// </summary>
	public bool SetEntityName(Entity entity, string name)
	{
		if (entity == null)
			return false;

		entity.Name = name;
		StructureVersion++;
		Commit();
		return true;
	}

	public Component? AddComponent(string componentName)
	{
		if (SelectedEntity == null)
			return null;

		var component = ComponentRegistry.CreateComponent(componentName);
		if (component == null)
			return null;

		SelectedEntity.AddComponent(component);
		SelectedComponent = component;
		StructureVersion++;
		Commit();
		return component;
	}

	public bool RemoveComponent(Component component)
	{
		if (component == null || component.Entity == null)
			return false;

		component.Entity.RemoveComponent(component);
		if (SelectedComponent == component)
			SelectedComponent = null;

		StructureVersion++;
		Commit();
		return true;
	}

	/// <summary>Adds a child entity to <paramref name="parent"/> without registering it in a scene.</summary>
	public Entity AddChild(Entity parent)
	{
		var child = Entity.Create();
		child.Name = "child";
		child.Transform.Size = parent.Transform.Size;
		child.Parent = parent;
		SelectedEntity = child;
		SelectedComponent = null;
		StructureVersion++;
		Commit();
		return child;
	}

	/// <summary>Detaches and destroys a child entity subtree (editor working copy only).</summary>
	public bool RemoveEntity(Entity entity)
	{
		if (entity == null || entity == Root)
			return false;

		entity.Parent = null;
		entity.Destroy();
		SelectedEntity = Root;
		SelectedComponent = null;
		StructureVersion++;
		Commit();
		return true;
	}

	// ---------------------------------------------------------------------
	// Helpers for panels
	// ---------------------------------------------------------------------

	/// <summary>Depth-first list of <paramref name="root"/> and all descendants.</summary>
	public static IEnumerable<Entity> Flatten(Entity root)
	{
		yield return root;

		var children = root.GetChildren();
		if (children == null)
			yield break;

		for (var i = 0; i < children.Count; i++)
		{
			foreach (var descendant in Flatten(children[i]))
				yield return descendant;
		}
	}

	/// <summary>
	/// Depth-first traversal into <paramref name="buffer"/>. Reuses the caller's list (and an internal
	/// stack) so update/draw paths allocate nothing; main-thread only, like the rest of the editor.
	/// </summary>
	public static void FlattenInto(Entity root, List<Entity> buffer)
	{
		buffer.Clear();
		var stack = FlattenStack;
		stack.Clear();
		stack.Add(root);

		while (stack.Count > 0)
		{
			var entity = stack[^1];
			stack.RemoveAt(stack.Count - 1);
			buffer.Add(entity);

			var children = entity.GetChildren();
			for (var i = children.Count - 1; i >= 0; i--)
				stack.Add(children[i]);
		}
	}
}
