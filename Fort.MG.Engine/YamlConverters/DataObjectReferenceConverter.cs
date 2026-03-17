using Fort.MG.EntitySystem.Parsing;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Fort.MG.YamlConverters;

public class DataObjectReferenceConverter : IYamlTypeConverter
{
    public static bool IsLoadingDataObject { get; set; }

    public bool Accepts(Type type)
    {
        return typeof(DataObject).IsAssignableFrom(type);
    }

    public object ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
    {
        // Only accept SCALARS for DataObject reference fields
        if (parser.TryConsume<Scalar>(out var scalar))
        {
            string name = scalar.Value ?? "";
            return DataRegistry.Get(type, name);
        }

        if (IsLoadingDataObject)
            return rootDeserializer(type);

        Logger.Error($"Error loading: {type}");
        return null;
    }

    public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
    {
        if (value is not DataObject data)
            emitter.Emit(new Scalar("null"));
        else
            emitter.Emit(new Scalar(data.Name));
    }
}

