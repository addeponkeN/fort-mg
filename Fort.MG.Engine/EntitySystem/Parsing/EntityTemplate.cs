using System.Text.Json.Nodes;

namespace Fort.MG.EntitySystem.Parsing;

public class EntityTemplate
{
	public string Name { get; set; }
	public string Description { get; set; }
	public TransformData Transform { get; set; }
	public List<JsonNode> Components { get; set; } = new();
}