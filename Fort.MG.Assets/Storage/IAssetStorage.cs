namespace Fort.MG.Assets.Storage;

public interface IAssetStorage
{
    object Load(string name);
    object Get(string name);
}