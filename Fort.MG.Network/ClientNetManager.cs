using Fort.MG.Systems;
using Fort.Network;
using LiteNetLib;
using ConnectionState = Fort.Network.ConnectionState;

namespace Fort.MG.Network;

public class ClientNetManager : EngineSystem
{
    private readonly NetClient _client = new();

    public MessageListener Listener => _client.MessageListener;
    public ConnectionState State => _client.State;

    public event NetEvents.OnPeerConnected OnConnected;
    public event NetEvents.OnPeerDisconnected OnDisconnected;

    public void Connect()
    {
        _client.ConnectedEvent += ClientOnConnectedEvent;
        _client.DisconnectedEvent += ClientOnDisconnectedEvent;
        _client.Connect("localhost", 27150, "gammerslame");
    }

    public void Disconnect()
    {
        _client.ConnectedEvent -= ClientOnConnectedEvent;
        _client.DisconnectedEvent -= ClientOnDisconnectedEvent;
        _client.Disconnect();
    }

    private void ClientOnConnectedEvent(NetPeer peer)
    {
        OnConnected?.Invoke(peer);
    }

    private void ClientOnDisconnectedEvent(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        OnDisconnected?.Invoke(peer, disconnectInfo);
    }

    public void SendMessage<T>(T message) where T : IMessage
    {
        _client.Send(message);
    }

}
