
namespace Fort.MG.EntitySystem.Parsing;

public class EntityTemplate
{
    public string Name { get; set; }
    public TransformData Transform { get; set; }
    public List<Dictionary<string, object>> Components { get; set; } = new();

    public List<EntityTemplate> Children { get; set; } = new();
}
