using Fort.MG.Threading;

namespace Fort.MG.Systems;

public class TaskNotifierSystem : EngineSystem
{
	private TaskNotifierManager _manager;

	public override void Init()
	{
		base.Init();
		_manager = new();
	}

	public override void Update(IGameTime t)
	{
		base.Update(t);
		_manager.Update();
	}
}
