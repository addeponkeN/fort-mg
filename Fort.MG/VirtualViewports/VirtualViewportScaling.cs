﻿using Microsoft.Xna.Framework;

namespace Fort.MG.VirtualViewports;

public class VirtualViewportScaling : VirtualViewport
{
    public VirtualViewportScaling(int virtualWidth, int virtualHeight) : base(virtualWidth, virtualHeight)
    {
    }

    public override Matrix Matrix
    {
        get
        {
            var scaleX = (float)Utils.Screen.Width / Width;
            var scaleY = (float)Utils.Screen.Height / Height;
            return Matrix.CreateScale(scaleX, scaleY, 1.0f);
        }
    }
}