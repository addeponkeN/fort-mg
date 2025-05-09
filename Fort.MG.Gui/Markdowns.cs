using Microsoft.Xna.Framework;

namespace Fort.MG.Gui;

public static class Markdowns
{
    public static Color DefaultColor = Color.White;

    private static readonly Dictionary<string, Color> MarkdownColors = new()
    {
        { "red", Color.IndianRed },
        { "green", Color.ForestGreen },
        { "blue", Color.CornflowerBlue },
        { "yellow", Color.Goldenrod },
        { "purple", Color.Purple },
        { "white", Color.White },
        { "black", Color.Black },
    };

    public static IEnumerable<(string, Color)> GetMarkdownColors()
    {
        return MarkdownColors.Select(x => (x.Key, x.Value));
    }

    public static bool TryGetColor(string key, out Color color)
    {
        return MarkdownColors.TryGetValue(key, out color);
    }
    
    public static Color GetColor(string key)
    {
        return MarkdownColors.GetValueOrDefault(key, DefaultColor);
    }

    public static void SetColor(string key, Color clr)
    {
        MarkdownColors[key] = clr;
    }

    public static void RemoveColor(string key)
    {
        MarkdownColors.Remove(key);
    }
}