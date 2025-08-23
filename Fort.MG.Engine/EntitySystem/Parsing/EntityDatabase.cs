using Fort.MG.Assets;

namespace Fort.MG.EntitySystem.Parsing;

public static class EntityDatabase
{
	public static string TemplatesFolderPath { get; set; } = "templates";

	public static string GetFilePath(string name) => Path.Combine(AssetManager.ContentRoot, TemplatesFolderPath, $"{name}.yaml");

	public static Entity LoadEntity(string name)
	{
		var path = GetFilePath(name);
		return EntitySerializer.LoadEntityTemplate(path);
	}
}