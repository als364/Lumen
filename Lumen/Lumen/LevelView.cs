using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Box2DX.Collision;
using System.Diagnostics;

namespace Lumen{
    public class LevelView
    {
        Level level;

        public LevelView(Level inLevel)
        {
            level = inLevel;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(level.Floor, new Vector2(0, 0), Color.White);

            AABB worldbox = level.AABB;
            int width = (int) (GameObject.worldScale * (worldbox.UpperBound.X - worldbox.LowerBound.X));
            int height = (int) (GameObject.worldScale * (worldbox.UpperBound.Y - worldbox.LowerBound.Y));
            //Console.WriteLine(worldbox.UpperBound.X);
            Vector2 screenPos = level.ScreenPosition * GameObject.worldScale;
            spriteBatch.Draw(level.Floor, screenPos, Color.White);
            for(int n = -level.Floor.Width; n < width + level.Floor.Width; n+=level.Floor.Width) {
                for(int m = -level.Floor.Height; m < height + level.Floor.Height; m+=level.Floor.Height) {
                    //Console.WriteLine(n + ", " + m);
                    spriteBatch.Draw(level.Floor, new Rectangle(n - (int)screenPos.X, m - (int)screenPos.Y, level.Floor.Width, level.Floor.Height), Color.White);
                }
            }
            /*while (widthtracker < width)
            {
                while (heighttracker < height)
                {
                    spriteBatch.Draw(level.Floor, new Vector2(widthtracker, heighttracker) - , Color.White);
                    heighttracker += level.Floor.Height;
                }
                widthtracker += level.Floor.Width;
            }*/
        }
    }
}
