using Fort.MG.Scenes;
using Fort.Network;
using LiteNetLib;

namespace Fort.MG.Network;

public class ServerScene : BaseScene
{
    protected Session Session { get; private set; }
    protected NetEntityManager NetEntities { get; private set; }

    internal void Attach(Session session)
    {
        Session = session;
    }

    public override void Init()
    {
        base.Init();
    }

    public override void LoadContent()
    {
        base.LoadContent();
    }

    public override void Start()
    {
        base.Start();

    }

    public override void Update(IGameContext t)
    {
        base.Update(t);
    }

    public override void ActiveUpdate()
    {
    }

    protected void InitNetEntities(IEntityFactory factory)
    {
        NetEntities?.Dispose();
        NetEntities = new NetEntityManager(Session.Listener, factory);
    }

    protected void Send<T>(T message, NetPeer peer) where T : IMessage
        => Session.Server.SendMessage(message, peer);

    protected void Broadcast<T>(T message, IEnumerable<NetPeer> peers) where T : IMessage
        => Session.Server.SendMessage(message, peers);
}