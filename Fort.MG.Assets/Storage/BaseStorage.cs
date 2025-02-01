using System.Collections.Generic;
using System.IO;

namespace Fort.MG.Assets.Storage;

public abstract class BaseStorage<T> : IAssetStorage
{
    protected Dictionary<string, T> Storage = new();
    public string ContentPath => $"{AssetManager.FullContentPath}\\{Folder}";
    public string Folder { get; set; }
    protected string GetFilePath(string filename) => Path.Combine(ContentPath, filename);
    public abstract object Load(string name);
    public virtual object Get(string name)
    {
        return (!Storage.TryGetValue(name, out var a) ? Load(name) : a)!;
    }

    protected void Add(string name, T data) => Storage.Add(name, data);
}