namespace Fort.MG.Scenes;

public class LoadingScene : Scene
{
	private readonly Scene _sceneToLoad;

	protected List<ILoadTask> LoadTasks = new();

	protected bool IsDone { get; private set; }

	public LoadingScene(Scene sceneToLoad)
	{
		_sceneToLoad = sceneToLoad;
		EnterTime = 0.2f;
		ExitTime = 0.2f;

		//AddLoadTask(() =>
		//{
		//	sceneToLoad.SceneManager = SceneManager;
		//	sceneToLoad.Init();
		//	sceneToLoad.LoadContent();
		//});
	}

	public void AddLoadTask(Action loadAction)
	{
		LoadTasks.Add(new LoadItem(loadAction));
	}

	public void AddAsyncLoadTask(Func<Task> asyncLoadAction)
	{
		LoadTasks.Add(new LoadItemAsync(asyncLoadAction));
	}

	public override void Init()
	{
		base.Init();
	}

	public override void Start()
	{
		base.Start();
		foreach (var loadTask in LoadTasks)
		{
			loadTask.Load();
		}
	}

	private bool AllTasksDone()
	{
		if (LoadTasks.Count == 0)
			return true;
		for (int i = 0; i < LoadTasks.Count; i++)
			if (!LoadTasks[i].IsCompleted)
				return false;
		return true;
	}

	public override void Update(IGameTime t)
	{
		base.Update(t);

		if (!IsDone && AllTasksDone())
		{
			IsDone = true;
			SetScene(_sceneToLoad);
		}
	}
}


public interface ILoadTask
{
	bool IsCompleted { get; }
	void Load();
}

public class LoadItemAsync : ILoadTask
{
	private readonly Func<Task> _loadFn;
	private Task _loadTask;

	public bool IsCompleted => _loadTask.IsCompleted;

	public LoadItemAsync(Func<Task> loadFn)
	{
		_loadFn = loadFn;
	}

	public void Load()
	{
		_loadTask = _loadFn.Invoke();
	}
}


public class LoadItem : ILoadTask
{
	private readonly Action _loadFn;

	public bool IsCompleted { get; private set; }

	public LoadItem(Action loadFn)
	{
		_loadFn = loadFn;
	}

	public void Load()
	{
		_loadFn.Invoke();
		IsCompleted = true;
	}
}
