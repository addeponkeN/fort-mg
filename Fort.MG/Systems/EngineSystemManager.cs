using Fort.MG.EntitySystem;
using Fort.MG.Scenes;
using Fort.MG.Utils;

namespace Fort.MG.Systems;

public sealed class EngineSystemManager
{
    public readonly List<EngineSystem> Systems = new();
    private readonly List<IRenderable> _renderables = new();
    private readonly List<IDrawableGui> _guiDrawables = new();
    private readonly List<IDrawableControl> _drawableControls = new();

    public T Get<T>() where T : EngineSystem
    {
        for(int i = 0; i < Systems.Count; i++)
            if(Systems[i] is T system)
                return system;
        return null;
    }

    public void Clear()
    {
        Systems.Clear();
    }

    public T Register<T>() where T : EngineSystem, new()
    {
        var system = new T();
		Add(system);
		return system;
    }

    public void Add(EngineSystem system)
    {
        system.SystemManager = this;
        if(!system._inited)
        {
            system.Init();
            system._inited = true;
        }
        Systems.Add(system);

        if(system is IRenderable entRenderable)
            _renderables.Add(entRenderable);
        if(system is IDrawableControl entDrawableControl)
            _drawableControls.Add(entDrawableControl);
        if(system is IDrawableGui entGui)
            _guiDrawables.Add(entGui);
    }

    public void OnSceneChanged(Scene newScene)
    {
        for(int i = 0; i < Systems.Count; i++)
            Systems[i].OnSceneChanged(newScene);
    }

    public void Update(IGameTime t)
    {
        for(int i = 0; i < Systems.Count; i++)
            Systems[i].Update(t);
    }
    
    public void PostUpdate(IGameTime t)
    {
        for(int i = 0; i < Systems.Count; i++)
            Systems[i].PostUpdate(t);
    }

    public void Draw()
    {
        for(int i = 0; i < _renderables.Count; i++)
            _renderables[i].Draw();
    }

    public void DrawGui()
    {
        for(int i = 0; i < _guiDrawables.Count; i++)
            _guiDrawables[i].DrawGui();
    }

    public void Render()
    {
        for(int i = 0; i < _renderables.Count; i++)
            _renderables[i].Render();
    }

    public void OnDrawBegin()
    {
        for(int i = 0; i < _drawableControls.Count; i++)
            _drawableControls[i].OnDrawBegin();
    }

    public void OnDrawEnd()
    {
        for(int i = 0; i < _drawableControls.Count; i++)
            _drawableControls[i].OnDrawEnd();
    }
}