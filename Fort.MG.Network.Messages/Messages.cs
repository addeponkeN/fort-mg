using Fort.Network;

namespace Fort.MG.Network.Messages;

[NetData]
public partial struct EntitySpawnMessage
{
    public EntityId Id;
    public EntityType Type;
    public NetPosition Position;
}

[NetData]
public partial struct EntityRemoveMessage
{
    public EntityId Id;
}

[NetData]
public partial struct EntityPositionMessage
{
    public EntityId Id;
    public NetPosition Position;
}

[NetData]
public partial struct EntityPositionSmoothMessage
{
    public EntityId Id;
    public NetPosition Position;
}

[NetData]
public partial struct EntityStateMessage
{
    public EntityId Id;
    public byte State;
}