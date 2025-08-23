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
			var template = new EntityTemplate
			{
				Name = entity.Name ?? "Unnamed", // You'll need to add a Name property to Entity or provide it somehow
				Transform = new TransformData
				{
					Position = entity.Transform.Position3,
					LocalPosition = entity.Transform.LocalPosition3,
					Size = entity.Transform.Size
				}
			};

			foreach (var component in entity.Components)
			{
				if (component is Transform) continue;

				var componentDict = ComponentSerializer.SerializeComponentToDict(component);
				template.Components.Add(componentDict);
			}

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
		var templates = YamlDeserializer.Deserialize<List<EntityTemplate>>(yaml);

		return templates.Select(CreateEntityFromTemplate).ToList();
	}

	// Load single entity by name from a file containing multiple entities
	public static Entity LoadEntityTemplate(string filePath, string entityName)
	{
		var entities = LoadEntityTemplates(filePath);
		return entities.FirstOrDefault(e => e.Name == entityName);
	}

	// Load first entity from file (for backward compatibility)
	public static Entity LoadEntityTemplate(string filePath)
	{
		var entities = LoadEntityTemplates(filePath);
		return entities.FirstOrDefault();
	}

	public static Entity CreateEntityFromTemplate(EntityTemplate template)
	{
		var entity = Entity.Create();
		entity.Name = template.Name; // Assuming Entity has a Name property

		if (template.Transform != null)
		{
			entity.Transform.Position3 = template.Transform.Position;
			entity.Transform.LocalPosition3 = template.Transform.LocalPosition;
			entity.Transform.Size = template.Transform.Size;
		}

		foreach (var componentDict in template.Components)
		{
			var component = ComponentSerializer.DeserializeComponentFromDict(componentDict);
			if (component != null)
				entity.AddComponent(component);
		}

		return entity;
	}
}