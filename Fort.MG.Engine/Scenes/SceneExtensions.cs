using Fort.MG.EntitySystem;

namespace Fort.MG.Scenes;

public static class SceneExtensions
{
    public static T? GetComponent<T>(this Scene scene) where T : Component
    {
        return scene.EntityManagerSystem.GetComponent<T>();
    }
}
