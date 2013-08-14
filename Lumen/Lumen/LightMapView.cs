using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lumen
{
    class LightMapView
    {
        private LightMap map;

        public LightMapView(LightMap inMap)
        {
            map = inMap;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //UpdateLightMap(spriteBatch, lvl);



            //use multiplicative blending
            BlendState mult = new BlendState();
            mult.ColorSourceBlend = Blend.Zero; // Blend.DestinationColor;//
            mult.AlphaSourceBlend = Blend.Zero; // Blend.DestinationAlpha;//
            mult.ColorDestinationBlend = Blend.SourceColor;
            mult.AlphaDestinationBlend = Blend.SourceAlpha;
            spriteBatch.Begin(SpriteSortMode.BackToFront, mult);

            spriteBatch.Draw(map.Texture, new Vector2(0, 0), Color.White);
            spriteBatch.End();
        }
        
    }
}
