using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Lumen
{
    public class LightView : ObjectView
    {
        public LightView(Light light) : base(light) { }

        /*public void DrawIfOn(SpriteBatch spriteBatch, GameTime gameTime) {
            if (((Light)viewedObject).IsOn)
            {
                base.Draw(spriteBatch, gameTime);
            }
        }*/
    }
}
