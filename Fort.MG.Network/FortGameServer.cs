using System.Collections.Concurrent;
using Fort.Network;
using LiteNetLib;

namespace Fort.MG.Network;

public class GameServerConfig
{
    public int Version { get; set; } = 1;
    public string Name { get; set; } = "Fort";
    public int Port { get; set; } = 27150;
    public int TickRate { get; set; } = 60;
}

public class FortGameServer
{
    protected readonly GameServerConfig Config;
    private readonly ConcurrentDictionary<Guid, Session> _sessions = new();
    private readonly CancellationTokenSource _cts = new();

    private bool _isRunning;

    public NetServer Net;

    public MessageListener Listener => Net.MessageListener;

    public FortGameServer(GameServerConfig? config = null)
    {
        Config = config ?? new();
    }

    public void Run()
    {
        _isRunning = true;

        Logger.LogFn = (level, message) =>
        {
            var prefix = $"[{DateTime.Now:HH:mm:ss.fff}][{Config.Name}][{level}]:";
            Console.WriteLine($"{prefix} {message}");
        };

        Logger.Info("Configuring...");
        Configure(Config);

        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true;
            Logger.Info("Shut down requested...");
            Shutdown();
        };
        AppDomain.CurrentDomain.ProcessExit += (_, _) => Shutdown();

        Net = new NetServer();
        Net.Start(Config.Port);

        Logger.Info("Initializing...");
        Initialize();

        CreateSession();

        Logger.Info("Running. Press Ctrl+C to stop.");
        _cts.Token.WaitHandle.WaitOne();
        Logger.Info("Shutting down...");

        Task.WhenAll(_sessions.Values.Select(s => s.CompletionTask))
            .Wait(TimeSpan.FromSeconds(5));

        Logger.Info("Stopped.");
    }

    public virtual void Configure(GameServerConfig config)
    {
    }

    public virtual void Initialize()
    {
    }

    public virtual void OnSessionAdded(Session session)
    {
    }

    public void AddSession(Session session)
    {
        session.Server = this;
        _sessions[session.Id] = session;
        session.OnEnded += () => _sessions.TryRemove(session.Id, out _);
        Logger.Info($"Session created: {session.Id}");
        OnSessionAdded(session);
        session.Start(_cts.Token);
    }

    public Session CreateSession()
    {
        var session = new Session(Config);
        AddSession(session);
        return session;
    }

    public void SendMessage<T>(T message, NetPeer peer) where T : IMessage
    {
        Net.SendMessage(message, peer);
    }

    public void SendMessage<T>(T message, IEnumerable<NetPeer> peers) where T : IMessage
    {
        Net.SendMessage(message, peers);
    }

    private void Shutdown()
    {
        if (!_isRunning) return;
        _isRunning = false;

        foreach (var session in _sessions.Values)
            session.Stop();

        _cts.Cancel();
    }
}

public class Session
{
    private readonly double _fixedDeltaTime;
    private const float MaxDeltaTime = 0.0166f;
    private const int MaxTicksPerFrame = 5;

    public Guid Id { get; } = Guid.NewGuid();
    public event Action? OnEnded;

    private Thread? _thread;
    private volatile bool _running;
    private TaskCompletionSource _tcs = new();

    internal FortGameServer Server;

    public MessageListener Listener { get; private set; }

    protected ServerScene Scene { get; private set; }

    public Session(GameServerConfig config)
    {
        _fixedDeltaTime = 1.0 / config.TickRate;
    }

    public Task CompletionTask => _tcs.Task;

    public void Start(CancellationToken ct = default)
    {
        if (_running) return;
        _running = true;

        Listener = Server.Listener;

        _thread = new Thread(() => RunLoop(ct))
        {
            Name = $"Session-{Id}",
            IsBackground = true
        };

        Logger.Info($"[Session {Id}] Starting.");
        _thread.Start();
    }

    public void Stop()
    {
        _running = false;
        Logger.Info($"[Session {Id}] Stop requested.");
    }

    public void SetScene(ServerScene scene)
    {
        Scene?.Exit();
        Scene = scene;
        Scene.Attach(this);
        Scene.Init();
        Scene.LoadContent();
    }

    private void RunLoop(CancellationToken ct)
    {
        try
        {
            var timer = System.Diagnostics.Stopwatch.StartNew();
            long previousTicks = timer.ElapsedTicks;
            double accumulator = 0.0;

            var gameTime = new ServerGameContext();

            long ticksPerSecond = System.Diagnostics.Stopwatch.Frequency;

            while (_running && !ct.IsCancellationRequested)
            {
                long currentTicks = timer.ElapsedTicks;
                float elapsed = (currentTicks - previousTicks) / (float)ticksPerSecond;
                previousTicks = currentTicks;

                elapsed = Math.Min(elapsed, MaxDeltaTime);
                accumulator += elapsed;

                int ticks = 0;
                while (accumulator >= _fixedDeltaTime && ticks < MaxTicksPerFrame)
                {
                    var totalTime = timer.Elapsed.TotalSeconds;
                    gameTime.Delta = (float)_fixedDeltaTime;
                    gameTime.TotalGameTime = (float)totalTime;
                    gameTime.Delta64 = _fixedDeltaTime;
                    gameTime.TotalGameTime64 = totalTime;
                    UpdateInternal(gameTime);
                    accumulator -= _fixedDeltaTime;
                    ticks++;
                }

                double remaining = _fixedDeltaTime - accumulator;
                if (remaining > 0.001f)
                    Thread.Sleep((int)(remaining * 1000));
            }
        }
        catch (Exception ex)
        {
            Logger.Error($"[Session {Id}] Unhandled exception in game loop. Ex:\n{ex}");
        }
        finally
        {
            Logger.Info($"[Session {Id}] Ended.");
            OnEnded?.Invoke();
            _tcs.TrySetResult();
        }
    }

    private void ProcessMessages()
    {
        Server.Net.Update();
    }

    private void UpdateInternal(IGameContext gt)
    {
        ProcessMessages();
        Scene.Update(gt);
    }
}
