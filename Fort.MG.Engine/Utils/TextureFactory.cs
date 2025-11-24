using Fort.MG.Assets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Fort.MG.Utils;

public class TextureFactory
{
    public Texture2D CreateCircle(int radius, Color color)
    {
        int diameter = radius * 2;
        Texture2D texture = new Texture2D(Graphics.GraphicsDevice, diameter, diameter);

        Color[] data = new Color[diameter * diameter];

        float rs = radius * radius;

        int index = 0;
        for (int y = 0; y < diameter; y++)
        {
            float dy = y - radius;

            for (int x = 0; x < diameter; x++)
            {
                float dx = x - radius;

                if (dx * dx + dy * dy <= rs)
                    data[index] = color;
                else
                    data[index] = Color.Transparent;

                index++;
            }
        }

        texture.SetData(data);
        return texture;
    }
}
