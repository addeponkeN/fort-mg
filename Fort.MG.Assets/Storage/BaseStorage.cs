namespace Fort.MG.Assets.Storage;

public abstract class BaseStorage<T>(string folder) : IAssetStorage<T>, IDisposable
{
	protected readonly Dictionary<string, T> Storage = new();

	public string Folder { get; set; } = folder;
	public string ContentPath => Path.Combine(AssetManager.ContentRoot, Folder);

	protected string GetFilePath(string filename) => Path.Combine(Folder, filename);

	public virtual Type GetAssetType() => typeof(T);

	public abstract T Load(string name);

	public virtual T Get(string name)
	{
		if (!Storage.TryGetValue(name, out var asset))
			asset = Load(name);
		return asset;
	}

	public void Add(string name, T data) => Storage[name] = data;

	public virtual void Dispose()
	{
		foreach (var asset in Storage.Values)
			if (asset is IDisposable disposable)
				disposable.Dispose();
		Storage.Clear();
	}
}
