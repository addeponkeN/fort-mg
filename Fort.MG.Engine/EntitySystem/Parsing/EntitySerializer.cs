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
        var templates = new List<EntityTemplate>();

        foreach (var entity in entities)
        {
            var template = CreateTemplateFromEntity(entity);
            templates.Add(template);
        }

        var yaml = YamlSerializer.Serialize(templates);
        File.WriteAllText(filePath, yaml);
    }

    // Save single entity (convenience method)
    public static void SaveEntityTemplate(Entity entity, string filePath)
    {
        SaveEntityTemplates(new[] { entity }, filePath);
    }

    // Load multiple entities from a single file
    public static List<Entity> LoadEntityTemplates(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Entity templates not found: {filePath}");

        var yaml = File.ReadAllText(filePath);

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

    private static EntityTemplate CreateTemplateFromEntity(Entity entity)
    {
        // entity
        var template = new EntityTemplate
        {
            Name = entity.Name ?? "Unnamed",
            Transform = new TransformData
            {
                Position = entity.Transform.Position3,
                LocalPosition = entity.Transform.LocalPosition3,
                Size = entity.Transform.Size
            }
        };

        // components
        foreach (var component in entity.Components)
        {
            if (component is Transform) continue;

            var dict = ComponentSerializer.SerializeComponentToDict(component);

            var typeName = (string)dict["type"];
            dict.Remove("type");

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