using Microsoft.Xna.Framework;

namespace Fort.MG.Extensions;

public static class GameTimeExtensions
{
    public static float Delta(this GameTime gt) => (float)gt.ElapsedGameTime.TotalSeconds;
    public static float DeltaMs(this GameTime gt) => (float)gt.ElapsedGameTime.TotalMilliseconds;
    public static float TotalSeconds(this GameTime gt) => (float)gt.TotalGameTime.TotalSeconds;
}
