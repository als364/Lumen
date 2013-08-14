using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Lumen
{
    public class LightMap
    {
        private Color[] alphaMap;
        private Texture2D lightMask;
        private int width, height;

        public LightMap(int width, int height)
        {
            alphaMap = new Color[width * height];
            for (int n = 0; n < alphaMap.Length; n++)
            {
                alphaMap[n] = Color.White;
            }
            this.width = width;
            this.height = height;
        }


        

        /// <summary>
        /// Gets light value at input point (x,y)
        /// </summary>
        /// <returns>Returns an int from 0 to 255</returns>
        public int GetLightValue(int x, int y) {
            int pos = x + width * y;
            if (pos < 0 || pos >= alphaMap.Length)
                return 0;
            
            Color c = alphaMap[pos];
            return c.R;
        }

        /*public Texture2D getTexture()
        {
            return lightMask;
        }*/

        public void SetMap(Texture2D mask)
        {
            lightMask = mask;
            mask.GetData<Color>(alphaMap);
        }

        /*/// <summary>
        /// Sets light value at that point (x,y) to lightval
        /// </summary>
        /// <param name="lightval">Must be value from 0 to 255</param>
        void SetLightValue(int lightval, int x, int y)
        {
            int val = lightval;
            if (lightval > 255) val = 255;
            else if (lightval < 0) val = 0;

            alphaMap[x, y] = val;
        }*/

        public int Width
        {
            get
            {
                return width;
            }
        }

        public int Height
        {
            get
            {
                return height;
            }
        }

        public Texture2D Texture
        {
            get
            {
                return lightMask;
            }
        }
    }
}
