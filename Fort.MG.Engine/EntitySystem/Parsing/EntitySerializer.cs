using Microsoft.Xna.Framework;
using YamlDotNet.Serialization;

namespace Fort.MG.EntitySystem.Parsing;

public static class EntitySerializer
{
    private static readonly ISerializer YamlSerializer = YamlSerializationFactory.Serializer;
    private static readonly IDeserializer YamlDeserializer = YamlSerializationFactory.Deserializer;

    // Save multiple entities to a single file
    public static void SaveEntityTemplates(IEnumerable<Entity> entities, string filePath)
    {
        File.WriteAllText(filePath, SerializeEntityTemplates(entities));
    }

    /// <summary>
    /// Serializes a set of entities to the YAML template format without touching the file system.
    /// Additive: used by the editor for in-memory snapshots (undo/redo) and diffing.
    /// </summary>
    public static string SerializeEntityTemplates(IEnumerable<Entity> entities)
    {
        var templates = new List<EntityTemplate>();

        foreach (var entity in entities)
        {
            var template = CreateTemplateFromEntity(entity);
            templates.Add(template);
        }

        return YamlSerializer.Serialize(templates);
    }

    // Save single entity (convenience method)
    public static void SaveEntityTemplate(Entity entity, string filePath)
    {
        SaveEntityTemplates(new[] { entity }, filePath);
    }

    /// <summary>
    /// Serializes already-built templates. Additive: the editor uses this to write a template it has
    /// edited while preserving template-level metadata such as
    /// <see cref="EntityTemplate.Extends"/>, which the entity-based overload cannot carry.
    /// </summary>
    public static string SerializeTemplates(IEnumerable<EntityTemplate> templates)
    {
        return YamlSerializer.Serialize(new List<EntityTemplate>(templates));
    }

    // Load multiple entities from a single file
    public static List<Entity> LoadEntityTemplates(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Entity templates not found: {filePath}");

        return DeserializeEntityTemplates(File.ReadAllText(filePath));
    }

    /// <summary>
    /// Parses YAML template text into entities (the inverse of <see cref="SerializeEntityTemplates"/>).
    /// Additive: used by the editor for in-memory undo/redo snapshots.
    /// </summary>
    public static List<Entity> DeserializeEntityTemplates(string yaml)
    {
        // An entity template file can contain multiple templates, therefore deserialize a List<>
        var templates = YamlDeserializer.Deserialize<List<EntityTemplate>>(yaml);

        return templates.Select(CreateEntityFromTemplate).ToList();
    }

    // Load single entity by name from a file containing multiple entities
    public static Entity LoadEntityTemplate(string filePath, string entityName)
    {
        var entities = LoadEntityTemplates(filePath);
        return entities.FirstOrDefault(e => e.Name == entityName);
    }

    // Load first entity from file
    public static Entity LoadEntityTemplate(string filePath)
    {
        var entities = LoadEntityTemplates(filePath);
        return entities.FirstOrDefault();
    }

    public static Entity CreateEntityFromTemplate(EntityTemplate template)
    {
        template = ResolveInheritance(template);

        var entity = Entity.Create();
        entity.Name = template.Name;

        // transform
        if (template.Transform != null)
        {
            entity.Transform.Position3 = template.Transform.Position;
            entity.Transform.LocalPosition3 = template.Transform.LocalPosition;
            entity.Transform.Size = template.Transform.Size;
        }

        // components
        foreach (var (typeName, rawData) in template.Components)
        {
            Dictionary<string, object> data;

            if (rawData == null)
            {
                data = new Dictionary<string, object>();
            }
            else if (rawData is Dictionary<string, object> strDict)
            {
                data = strDict;
            }
            else if (rawData is Dictionary<object, object> objDict)
            {
                data = objDict.ToDictionary(x => x.Key.ToString(), x => x.Value);
            }
            else
            {
                throw new Exception($"Invalid component data for {typeName}");
            }

            var component = ComponentSerializer.DeserializeComponentFromDict(typeName, data, entity);
            if (component != null)
            {
                entity.AddComponentViaSerializer(component);
            }
            else
            {
                // Unregistered component type (e.g. a template referring to a component that no longer
                // exists): surface it rather than silently dropping the component.
                Logger.Warn($"Entity template '{entity.Name}': unknown component type '{typeName}' - skipped.");
            }
        }

        for (var i = 0; i < entity.Components.Count; i++)
            entity.Components[i].Init();
        for (var i = 0; i < entity.Components.Count; i++)
            entity.Components[i].OnAdded();

        // children
        foreach (var (childName, childTemplate) in template.Children)
        {
            var child = CreateEntityFromTemplate(childTemplate);
            child.Name = childName;
            child.Parent = entity;

            if (child.Transform.Size == Vector2.Zero)
            {
                child.Transform.Size = entity.Transform.Size;
            }
        }

        return entity;
    }

    private static EntityTemplate ResolveInheritance(EntityTemplate template, HashSet<string> visited = null)
    {
        if (template.Extends == null)
            return template;

        visited ??= [];

        if (!visited.Add(template.Name))
            throw new Exception($"Cyclic inheritance detected for '{template.Name}'");

        var parent = EntityDatabase.LoadEntityTemplate(template.Extends);

        parent = ResolveInheritance(parent, visited);

        return MergeTemplates(parent, template);
    }

    private static EntityTemplate MergeTemplates(EntityTemplate parent, EntityTemplate child)
    {
        var result = new EntityTemplate
        {
            Name = child.Name ?? parent.Name,
            Transform = child.Transform ?? parent.Transform
        };

        foreach (var kv in parent.Components)
            result.Components[kv.Key] = kv.Value;

        foreach (var kv in child.Components)
            result.Components[kv.Key] = kv.Value;

        foreach (var kv in parent.Children)
            result.Children[kv.Key] = kv.Value;

        foreach (var kv in child.Children)
            result.Children[kv.Key] = kv.Value;

        return result;
    }

    /// <summary>
    /// Builds an <see cref="EntityTemplate"/> (including child templates) from a live entity tree.
    /// Additive: exposed for the editor; previously private.
    /// </summary>
    public static EntityTemplate CreateTemplateFromEntity(Entity entity)
    {
        // entity
        var template = new EntityTemplate
        {
            Name = entity.Name ?? "Unnamed",
            Transform = new TransformData
            {
                // Templates store the transform relative to the parent, which is what
                // CreateEntityFromTemplate applies (it sets Position3 first, then LocalPosition3, so
                // LocalPosition is the effective value). Writing the world position into Position as
                // well was inert on load but self-inconsistent - a child's "position" disagreed with
                // its "localPosition" - so both keys now carry the same parent-relative value.
                Position = entity.Transform.LocalPosition3,
                LocalPosition = entity.Transform.LocalPosition3,
                Size = entity.Transform.Size
            }
        };

        // components
        foreach (var component in entity.Components)
        {
            if (component is Transform) continue;

            var dict = ComponentSerializer.SerializeComponentToDict(component);

            // The component type name is the YAML key. SerializeComponentToDict only emits member
            // entries (plus "enabled"), so resolve the name from the registry instead of reading a
            // "type" key that is never written (which previously threw KeyNotFoundException on save).
            var typeName = ComponentRegistry.GetTypeName(component.GetType());

            template.Components[typeName] = dict;
        }

        // children
        foreach (var child in entity.GetChildren())
        {
            var childTemplate = CreateTemplateFromEntity(child);
            var key = child.Name ?? $"child_{template.Children.Count}";
            template.Children[key] = childTemplate;
        }

        return template;
    }

}