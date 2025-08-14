namespace Fort.MG.Assets.Storage;

public interface IAssetStorage<T>
{
    T Load(string name);
    T Get(string name);
}