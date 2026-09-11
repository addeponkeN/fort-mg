# Fort.MG.Editor

An in-game entity editor for Fort.MG. It browses, edits, and saves the engine's data-driven
**entity templates** (`content/templates/*.yaml`) — the same files `EntityDatabase` /
`EntitySerializer` load at runtime.

## Use

In a `FortGame` subclass:

```csharp
protected override void LoadContent()
{
    base.LoadContent();                 // FortEngine.Load() must have run first

    FortEngine.RegisterSystem<EntityEditorSystem>();
    _editor = FortEngine.GetSystem<EntityEditorSystem>();
}

protected override void Render(GameTime gt) { base.Render(gt); _editor?.RenderGui(); }

protected override void Draw(GameTime gt)   { base.Draw(gt);   _editor?.DrawGui(); }
```

- **F1** toggles the editor.
- `RenderGui()` and `DrawGui()` must be called where **no `SpriteBatch` is active** (the editor's
  `Canvas` begins its own batch and renders to its own target). `ExampleGame` calls them next to its
  own `_canvas.Render()` / `_canvas.Draw()`. The system is deliberately *not* an `IFortDrawableGui`,
  because the global `EngineSystemManager.DrawGui` pass runs inside an ambient batch.

## Panels

| Panel | What it does |
|---|---|
| Editor (toolbar) | Save, Reload, Undo, Redo, New, Add Child, Delete |
| Templates | Lists `content/templates/*.yaml`; click to load |
| Hierarchy | Depth-first tree of the entity and its children; add a child |
| Inspector | Entity name, transform (position/size), and one row per serializable component field; add/remove components |
| Viewport | Gizmo view of the working tree; click to select, drag to move |

## How editing works

- Property rows are generated from the component's own `[Serialize]` members via
  `ComponentFieldModel`, so the key shown is exactly the YAML key written. Unknown types
  (sprite/data-object references, or anything the editor cannot safely author) render read-only
  rather than writing a bad value.
- Every mutation goes through `EntityEditorSession`, which marks the document dirty and pushes a
  YAML snapshot, so **undo/redo restores byte-identical templates**.
- The working tree is **detached** from any scene (`Entity.Create` + `Parent`, never
  `Entity.Instantiate`), so editing never leaks entities into the running game.
- Saving writes through the template overload so a template's `extends:` key is preserved.

## Limitations

- The viewport is a schematic gizmo/bounds view — it does not draw real sprites yet.
- Fort.MG.Gui has no cross-canvas input routing, so while the editor is open the host game's own
  canvas still sees the same mouse state. Closed, the editor consumes nothing.
- Numeric/string fields commit on each parseable keystroke, so undo granularity for typing is
  per-valid-value rather than per-edit-session.
- Removing a parent entity does not reparent its children.
