using System.Reflection;
using Fort.MG.EntitySystem.Parsing;

namespace Fort.MG.Editor.Fields;

/// <summary>
/// A single editable member of a component, projected from the exact discovery/naming rules
/// <see cref="ComponentSerializer"/> uses when it reads and writes YAML templates. Editing a field
/// therefore round-trips through serialization by construction: the key shown is the key written.
/// </summary>
public sealed class ComponentField
{
	private readonly MemberInfo _member;

	internal ComponentField(MemberInfo member, string name, Type fieldType)
	{
		_member = member;
		Name = name;
		FieldType = fieldType;
	}

	/// <summary>Serialization key (YAML key) this field is written under.</summary>
	public string Name { get; }

	/// <summary>Declared CLR type of the member.</summary>
	public Type FieldType { get; }

	public bool IsEnum => FieldType.IsEnum;

	/// <summary>Display label for the inspector (CLR member name, e.g. "Mass").</summary>
	public string MemberName => _member.Name;

	public object? GetValue(object component) => ComponentSerializer.GetMemberValue(_member, component);

	public void SetValue(object component, object? value) => ComponentSerializer.SetMemberValue(_member, component, value);

	public override string ToString() => $"{Name} ({FieldType.Name})";
}
