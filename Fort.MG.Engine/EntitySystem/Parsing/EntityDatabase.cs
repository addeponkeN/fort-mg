using Fort.MG.JsonConverters;
using System.Text.Json;
using Fort.MG.Assets;

namespace Fort.MG.EntitySystem.Parsing;

public static class EntityDatabase
{
	public static string TemplatesFolderPath { get; set; } = "templates";

	public static string GetFilePath(string name) => Path.Combine(AssetManager.ContentRoot, TemplatesFolderPath, $"{name}.json");

	public static Entity LoadEntity(string name)
	{
		var path = GetFilePath(name);
		return EntitySerializer.LoadEntityTemplate(path);
	}
}

public static class EntitySerializer
{
	private static readonly JsonSerializerOptions Options = new()
	{
		WriteIndented = true,
		PropertyNamingPolicy = null,
		Converters =
		{
			new Vector2Converter(),
			new Vector3Converter(),
			new ColorConverter(),
			new RectangleConverter()
		}
	};

	public static void SaveEntityTemplate(Entity entity, string filePath)
	{
		var template = new EntityTemplate
		{
			Name = Path.GetFileNameWithoutExtension(filePath),
			Transform = new TransformData
			{
				Position = entity.Transform.Position3,
				LocalPosition = entity.Transform.LocalPosition3,
				Size = entity.Transform.Size
			}
		};

		// Serialize components (skip Transform since it's handled separately)
		foreach (var component in entity.Components)
		{
			if (component is Transform) continue;

			var componentJson = ComponentSerializer.SerializeComponent(component);
			template.Components.Add(componentJson);
		}

		var json = JsonSerializer.Serialize(template, Options);
		File.WriteAllText(filePath, json);
	}

	public static Entity LoadEntityTemplate(string filePath)
	{
		if (!File.Exists(filePath))
		{
			throw new FileNotFoundException($"Entity template not found: {filePath}");
		}

		var json = File.ReadAllText(filePath);
		var template = JsonSerializer.Deserialize<EntityTemplate>(json, Options);

		return CreateEntityFromTemplate(template);
	}

	public static Entity CreateEntityFromTemplate(EntityTemplate template)
	{
		var entity = Entity.Create();

		// Apply transform data
		if (template.Transform != null)
		{
			entity.Transform.Position3 = template.Transform.Position;
			entity.Transform.LocalPosition3 = template.Transform.LocalPosition;
			entity.Transform.Size = template.Transform.Size;
		}

		// Add components
		foreach (var componentJson in template.Components)
		{
			var component = ComponentSerializer.DeserializeComponent(componentJson);
			if (component != null)
			{
				entity.AddComponent(component);
			}
		}

		return entity;
	}
}