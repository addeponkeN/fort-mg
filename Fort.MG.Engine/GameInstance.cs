using Fort.MG.Assets;
using Fort.MG.Scenes;
using Fort.MG.Systems;
using Fort.MG.Threading;
using Microsoft.Xna.Framework;

namespace Fort.MG;

/// <summary>
/// Holds the mutable simulation-level state that previously lived directly as static fields on
/// <see cref="FortEngine"/> (scene manager, global systems, assets, current frame time).
/// <see cref="FortEngine"/> forwards to a single default instance (<see cref="FortEngine.Default"/>)
/// today, preserving existing single-instance behavior; this type exists so that state can move
/// toward being owned by an explicit, instantiable context instead of being accessed ambiently
/// (see AI_CONTEXT.md §12 - "Static facades" guidance).
/// </summary>
public class GameInstance
{
	public FortGame Game { get; internal set; }
	public GameTime Time { get; internal set; }
	public SceneManager SceneManager { get; internal set; }
	public EngineSystemManager SystemManager { get; internal set; }
	public AssetManager Assets { get; internal set; }
	public TaskNotifierManager TaskNotifiers { get; } = new();
	public double TimeScale { get; set; } = 1.0;

	internal bool StartedExiting;
}
