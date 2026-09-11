namespace Fort.MG.Threading;

public class TaskNotifierLocal : TaskNotifier
{
    public void Call<TVal>(Task<TVal> task, Action<TVal> callback, Action<Exception> errorCallback = null, CancellationToken? token = null)
    {
        base.Call(task, callback, errorCallback, token);
    }
}

public class TaskNotifierManager
{
    /// <summary>
    /// The default (and, today, only) task notifier manager, owned by <see cref="GameInstance"/>.
    /// Prefer injecting/using an explicit instance where possible.
    /// Only valid once <see cref="FortEngine.Default"/> has finished initializing - never touch
    /// it from a constructor that can run while that initialization is still in progress.
    /// </summary>
    public static TaskNotifierManager Get => FortEngine.Default.TaskNotifiers;

    private readonly List<TaskNotifier> _notifiers = new();

    private readonly TaskNotifierLocal _local;

    public TaskNotifierManager()
    {
        _local = new();
        // Register explicitly: TaskNotifier no longer self-registers from its constructor,
        // because that dereferenced TaskNotifierManager.Get during FortEngine's static
        // initialization (cycle -> NullReferenceException, engine never started).
        Add(_local);
    }

    public void Add(TaskNotifier notifier) => _notifiers.Add(notifier);

    public void Update()
    {
        for (var i = 0; i < _notifiers.Count; i++)
        {
            var n = _notifiers[i];
            if (!n.IsActive)
            {
                _notifiers.Remove(n);
                i--;
                continue;
            }
            n.Update();
        }
    }


    public void Call<T>(Task<T> t, Action<T> callback, Action<Exception> errorCallback = null, CancellationToken? cancelToken = null)
    {
        _local.Call(t, callback, errorCallback, cancelToken);
    }
}