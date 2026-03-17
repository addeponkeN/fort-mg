using Fort.MG.EntitySystem;
using Fort.MG.Network.Messages;
using Fort.Network;
using LiteNetLib;

namespace Fort.MG.Network;

public interface IEntityFactory
{
    Entity Create(EntityId id, EntityType type);
}

public delegate void OnEntitySpawn(Entity entity);
public delegate void OnEntityRemove(Entity entity);

/// <summary>
/// Network entitymanager
/// </summary>
public class NetEntityManager : IDisposable
{
    private readonly IMessageListener _listener;
    private readonly IEntityFactory _factory;

    private HashSet<EntityType> _registeredEntities;

    private List<Entity> _entities = new();
    private Dictionary<EntityId, Entity> _entitiesById = new();

    public event OnEntitySpawn EntitySpawned;
    public event OnEntityRemove EntityRemoved;

    public NetEntityManager(IMessageListener listener, IEntityFactory factory)
    {
        _listener = listener;
        _factory = factory;

        _listener.Sub<EntitySpawnMessage>(OnEntitySpawnMessage);
        _listener.Sub<EntityRemoveMessage>(OnEntityRemoveMessage);
    }

    private void AddEntity(EntityId id, Entity entity)
    {
        _entities.Add(entity);
        _entitiesById[id] = entity;
    }

    private void RemoveEntity(EntityId id)
    {
        var entity = _entitiesById[id];
        _entities.Remove(entity);
        _entitiesById.Remove(id);
    }

    private void OnEntityRemoveMessage(EntityRemoveMessage message, byte type, NetPeer sender)
    {
        var entity = _entitiesById[message.Id];
        RemoveEntity(message.Id);
        EntityRemoved(entity);
    }

    private void OnEntitySpawnMessage(EntitySpawnMessage message, byte type, NetPeer sender)
    {
        var entity = _factory.Create(message.Id, message.Type);
        AddEntity(message.Id, entity);
        EntitySpawned(entity);
    }

    public T? GetEntity<T>(EntityId id) where T : Entity
    {
        return _entitiesById[id] as T;
    }

    public void Dispose()
    {
        _listener.UnSub<EntitySpawnMessage>(OnEntitySpawnMessage);
        _listener.UnSub<EntityRemoveMessage>(OnEntityRemoveMessage);
    }
}
