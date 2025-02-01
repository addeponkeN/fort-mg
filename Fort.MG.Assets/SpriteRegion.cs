//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;

//namespace Fort.MG.Assets;

//public class SpriteRegion
//{
//	public string Name { get; set; }
//    public Rectangle Frame { get; private set; }
//    public Texture2D Texture { get; private set; }

//    public SpriteRegion(string name, Texture2D texture, Rectangle frame)
//    {
//        Texture = texture;
//        Frame = frame;
//    }

//    public SpriteRegion(string name, Texture2D texture, int x, int y, int w, int h) 
//        : this(name, texture, new Rectangle(x, y, w, h))
//    {
//    }

//    public static implicit operator Texture2D(SpriteRegion tf) => tf.Texture;
//    public static implicit operator Rectangle(SpriteRegion tf) => tf.Frame;
//}