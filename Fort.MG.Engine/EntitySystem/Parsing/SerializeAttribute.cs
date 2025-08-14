// ReSharper disable once CheckNamespace
namespace Fort.MG;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class SerializeAttribute : Attribute
{
	public string Name { get; set; }
	public SerializeAttribute(string name = null)
	{
		Name = name;
	}
}