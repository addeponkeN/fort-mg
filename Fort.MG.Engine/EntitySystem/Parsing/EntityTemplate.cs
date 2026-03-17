
namespace Fort.MG.EntitySystem.Parsing;

public class EntityTemplate
{
    public int Version { get; set; } = 1;

    public string Name { get; set; }
    public string Extends { get; set; }

    public TransformData Transform { get; set; }

    public Dictionary<string, object> Components { get; set; } = new();
    public Dictionary<string, EntityTemplate> Children { get; set; } = new();
}