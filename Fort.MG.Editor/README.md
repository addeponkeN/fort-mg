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

## Use in a fresh project

### 1. Layout

There are no NuGet packages — Fort.MG is consumed as source, as `ProjectReference`s. `Fort.MG` and
`Fort.MG.Extensions` both reference `..\..\Fort\Fort.Utility\Fort.Utility.csproj`, so your game
project has to sit next to the `Fort` and `Fort.MG` checkouts:

```
Projects\
  Fort\      <- Fort.Utility, Fort.TexturePacker, ...
  Fort.MG\   <- this repo
  MyGame\    <- your game
```

### 2. MyGame.csproj

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="MonoGame.Framework.DesktopGL" Version="3.8.4" />
  </ItemGroup>

  <ItemGroup>
    <!-- One reference is enough: Fort.MG.Editor pulls in Fort.MG.Engine and Fort.MG.Gui. -->
    <ProjectReference Include="..\Fort.MG\Fort.MG.Editor\Fort.MG.Editor.csproj" />
  </ItemGroup>

  <ItemGroup>
    <!-- The GUI reads this straight off disk, it is NOT built by MGCB (see the appendix). -->
    <None Update="content\fonts\defaultfont.ttf">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </None>
  </ItemGroup>
</Project>
```

The editor loads no content of its own, so this is all it takes to run it — no atlas, no pipeline
extension, no `Content.mgcb`.

### 3. Wire it up

`Program.cs`:

```csharp
using var game = new MyGame();
game.Run();
```

`MyGame.cs`:

```csharp
using Fort.MG;
using Fort.MG.Editor;
using Microsoft.Xna.Framework;

public class MyGame : FortGame
{
    private EntityEditorSystem? _editor;

    protected override void LoadContent()
    {
        base.LoadContent();                 // FortEngine.Load() must have run first

        FortEngine.RegisterSystem<EntityEditorSystem>();
        _editor = FortEngine.GetSystem<EntityEditorSystem>();
    }

    protected override void Render(GameTime gt) { base.Render(gt); _editor?.RenderGui(); }

    protected override void Draw(GameTime gt)   { base.Draw(gt);   _editor?.DrawGui(); }
}
```

`RenderGui()` renders the editor canvas into its own target and `DrawGui()` blits it, so both must
be called where **no `SpriteBatch` is active** — after your own `base.Render`/`base.Draw`, next to
where a scene draws its canvas.

### 4. content/

```
MyGame\
  content\
    fonts\
      defaultfont.ttf   <- required, copy it from Fort.MG.Example/content/fonts/
    templates\
      mything.yaml      <- what the editor browses and edits
```

- `defaultfont.ttf` is mandatory: `GuiContent` looks for `<cwd>/content/fonts/defaultfont.ttf` as a
  raw file and FontStashSharp needs it.
- `templates/` is optional. If it is missing the Templates panel is simply empty (the engine logs
  `Missing templates folder`), and the editor creates the folder on the first **Save**.

### 5. Run it

```
dotnet run
```

Press **F1**. See the panel table below for what the panels do.

### Gotchas

- Call `RenderGui()`/`DrawGui()` outside any batch — that is why `EntityEditorSystem` is not an
  `IFortDrawableGui` (that pass runs inside an ambient batch).
- Fort.MG.Gui has no cross-canvas input routing yet, so while the editor is open your own canvas
  still receives the same mouse state.

### Appendix: the content build

Only needed once your game ships real content (textures, atlases, fonts). The example project is
the reference implementation; the pieces are:

- **MGCB as a local tool.** `MonoGame.Content.Builder.Task` alone is not enough on Windows — MGCB
  is invoked as `dotnet mgcb` and resolved from `.config/dotnet-tools.json`, so that file must list
  `dotnet-mgcb` (3.8.4.1) and be restored before the content build:

  ```xml
  <Target Name="RestoreContentBuilderTool" BeforeTargets="PrepareContentBuilder">
    <Exec Command="&quot;$(DotnetCommand)&quot; tool restore" WorkingDirectory="$(MSBuildProjectDirectory)" />
  </Target>
  ```

- **The pipeline extension.** Copy the built `Fort.MG.PipelineExtension.dll` (and `RectpackSharp.dll`)
  next to `Content.mgcb` and reference it there with `/reference:Fort.MG.PipelineExtension.dll`, so
  the `SpriteAtlasImporter`/`SpriteAtlasProcessor` are available. `Fort.MG.Example` does this with a
  `SyncContentPipelineExtension` target rather than a checked-in binary.
- **Recipes.** `Nopipeline.Task` + `content/Content.npl` drive the atlas (`textures/*.atlas` →
  `SpriteAtlas*`) and texture builds; equivalent `Content.mgcb` entries are generated from it.

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
