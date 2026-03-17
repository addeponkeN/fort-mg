using Fort.Network;

namespace Fort.MG.Network.Messages;

[NetData]
public partial struct EntityId
{
    public static implicit operator ushort(EntityId id) => id.Id;
    public static implicit operator EntityId(ushort id) => new() { Id = id };
    public ushort Id;
}

[NetData]
public partial struct EntityType
{
    public static implicit operator byte(EntityType type) => type.Type;
    public static implicit operator EntityType(byte type) => new() { Type = type };
    public byte Type;
}

[NetData]
public partial struct NetPosition
{
    public float X;
    public float Y;
}

[NetData]
public partial struct NetDirection
{
    public byte Direction;
}