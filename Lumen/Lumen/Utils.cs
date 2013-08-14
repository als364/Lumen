using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using MyDataTypes;

namespace Lumen
{
    class Utils
    {
        public static Vector2[] LightHullToPhysics(Vector2[] lighthull)
        {
            Vector2[] physicshull = new Vector2[lighthull.Length];
            for (int i = 0; i < lighthull.Length; i++)
            {
                physicshull[i] = new Vector2(lighthull[i].X - .5f * lighthull[i].Y, 2f * lighthull[i].Y);
            }
            return physicshull;
        }
    }
}
