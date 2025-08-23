using System.Linq.Expressions;
using System.Reflection;

namespace Fort.MG.EntitySystem.Parsing;

// Optional attribute for custom component names
[AttributeUsage(AttributeTargets.Class)]
public class ComponentNameAttribute : Attribute
{
	public string Name { get; }

	public ComponentNameAttribute(string name)
	{
		Name = name;
	}
}

public static class ComponentRegistry
{
	private static readonly Dictionary<string, Type> ComponentTypes = new();
	private static readonly Dictionary<Type, string> TypeNames = new();
	private static readonly Dictionary<Type, Func<Component>> ComponentFactories = new();

	static ComponentRegistry()
	{
		AutoRegisterComponents();
	}

	private static void AutoRegisterComponents()
	{
		// Get all assemblies that might contain components
		var assemblies = new[]
		{
			Assembly.GetExecutingAssembly(), // Current assembly
			Assembly.GetCallingAssembly(),   // Assembly that called this
			Assembly.GetEntryAssembly()      // Main application assembly
		}.Where(a => a != null).Distinct();

		// Also search any loaded assemblies that might contain game code
		var allAssemblies = AppDomain.CurrentDomain.GetAssemblies()
			.Where(a => !a.IsDynamic &&
			            !a.FullName.StartsWith("System") &&
			            !a.FullName.StartsWith("Microsoft") &&
			            !a.FullName.StartsWith("mscorlib"))
			.Concat(assemblies)
			.Distinct();

		foreach (var assembly in allAssemblies)
		{
			try
			{
				var componentTypes = assembly.GetTypes()
					.Where(type => type.IsClass &&
					               !type.IsAbstract &&
					               typeof(Component).IsAssignableFrom(type) &&
					               type.GetConstructor(Type.EmptyTypes) != null) // Must have parameterless constructor
					.ToList();

				foreach (var type in componentTypes)
				{
					RegisterComponent(type);
				}
			}
			catch (ReflectionTypeLoadException ex)
			{
				var loadedTypes = ex.Types.Where(t => t != null);
				foreach (var type in loadedTypes)
				{
					if (type.IsClass && !type.IsAbstract && typeof(Component).IsAssignableFrom(type) &&
					    type.GetConstructor(Type.EmptyTypes) != null)
					{
						RegisterComponent(type);
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Failed to scan assembly {assembly.FullName}: {ex.Message}");
			}
		}
	}

	public static void RegisterComponent<T>() where T : Component, new()
	{
		RegisterComponent(typeof(T));
	}

	private static void RegisterComponent(Type type)
	{
		var name = GetComponentName(type);

		if (ComponentTypes.ContainsKey(name))
		{
			name = type.FullName;
		}

		ComponentTypes[name] = type;
		TypeNames[type] = name;
		ComponentFactories[type] = CreateComponentFactory(type);

		Console.WriteLine($"Registered component: {name} -> {type.FullName}");
	}

	private static string GetComponentName(Type type)
	{
		// Check for custom attribute first
		var customNameAttr = type.GetCustomAttribute<ComponentNameAttribute>();
		if (customNameAttr != null)
			return customNameAttr.Name;

		// Default to class name
		return type.Name;
	}

	private static Func<Component> CreateComponentFactory(Type type)
	{
		// Create a compiled expression for fast instantiation
		var constructor = type.GetConstructor(Type.EmptyTypes);
		if (constructor == null)
			throw new InvalidOperationException($"Component {type.Name} must have a parameterless constructor");

		var newExpression = Expression.New(constructor);
		var lambdaExpression = Expression.Lambda<Func<Component>>(newExpression);
		return lambdaExpression.Compile();
	}

	public static Type GetComponentType(string typeName)
	{
		ComponentTypes.TryGetValue(typeName, out var type);
		return type;
	}

	public static string GetTypeName(Type type)
	{
		TypeNames.TryGetValue(type, out var name);
		return name ?? type.Name;
	}

	public static Component CreateComponent(string typeName)
	{
		var type = GetComponentType(typeName);
		if (type == null)
		{
			Console.WriteLine($"Component type '{typeName}' not found. Available types: {string.Join(", ", ComponentTypes.Keys)}");
			return null;
		}

		if (ComponentFactories.TryGetValue(type, out var factory))
		{
			var component = factory();
			component.Init(); // Make sure to initialize
			return component;
		}

		Console.WriteLine($"No factory found for component type '{typeName}'");
		return null;
	}

	public static T CreateComponent<T>() where T : Component
	{
		return (T)CreateComponent(typeof(T).Name);
	}

	public static Component CreateComponent(Type type)
	{
		if (ComponentFactories.TryGetValue(type, out var factory))
		{
			var component = factory();
			component.Init();
			return component;
		}

		return null;
	}

	public static IEnumerable<string> GetAllComponentNames() => ComponentTypes.Keys;

	public static IEnumerable<Type> GetAllComponentTypes() => ComponentTypes.Values;

	public static void RegisterComponent(string name, Type type)
	{
		if (!typeof(Component).IsAssignableFrom(type))
			throw new ArgumentException($"Type {type.Name} is not a Component");

		ComponentTypes[name] = type;
		TypeNames[type] = name;
		ComponentFactories[type] = CreateComponentFactory(type);
	}

	// Check if a component is registered
	public static bool IsRegistered(string typeName) => ComponentTypes.ContainsKey(typeName);
	public static bool IsRegistered<T>() where T : Component => TypeNames.ContainsKey(typeof(T));
}