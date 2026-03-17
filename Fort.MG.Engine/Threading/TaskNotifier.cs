using System.Collections.Concurrent;

namespace Fort.MG.Threading;

public abstract class TaskNotifier : IDisposable
{
    private class TaskCallback
    {
        public bool IsError => Exception != null;
        public Exception Exception;
        public object? Value;
        public Action Callback;
        public object OriginalCallback;
        public Action<Exception> ErrorCallback;
    }

    public bool IsActive { get; private set; } = true;

    private readonly ConcurrentQueue<TaskCallback> _values = new();
    private readonly List<object> _queuedCalls = new();

    private readonly object _lock = new object();

    protected TaskNotifier()
    {
        TaskNotifierManager.Get.Add(this);
    }

    public bool IsCalling<T>(T callbackFunc)
    {
        lock (_lock)
        {
            return _queuedCalls.Contains(callbackFunc);
        }
    }

    protected virtual void Call<TVal>(Task<TVal> task, Action<TVal> callback, Action<Exception> errCallback = null, CancellationToken? token = null)
    {
        lock (_lock)
        {
            _queuedCalls.Add(callback);
        }

        _ = Task.Run(async () =>
        {
            try
            {
                var result = await task;

                if (token?.IsCancellationRequested ?? false)
                    return;

                _values.Enqueue(new TaskCallback
                {
                    Value = result,
                    Callback = () => callback(result),
                    OriginalCallback = callback,
                    ErrorCallback = errCallback
                });
            }
            catch (Exception ex)
            {
                _values.Enqueue(new TaskCallback
                {
                    ErrorCallback = errCallback,
                    OriginalCallback = callback,
                    Exception = ex
                });
            }
        });
    }

    public virtual void Update()
    {
        if (_values.TryDequeue(out var taskCallback))
        {
            if (taskCallback.IsError)
                taskCallback.ErrorCallback?.Invoke(taskCallback.Exception);
            else
                taskCallback.Callback.Invoke();
            lock (_lock)
            {
                _queuedCalls.Remove(taskCallback.OriginalCallback);
            }
        }
    }

    public void Dispose()
    {
        IsActive = false;
    }
}