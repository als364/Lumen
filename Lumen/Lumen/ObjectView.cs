using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using Box2DX.Dynamics;
using Box2DX.Common;
using Box2DX.Collision;

namespace Lumen{
    public class ObjectView{
        protected GameObject viewedObject;
        public GameObject ViewObject { get { return viewedObject; } }

        public ObjectView(GameObject obj)
        {
            viewedObject = obj;
        }

        public static int ComparePositions(ObjectView dis, ObjectView dat)
        {
            float THRESHOLD = .05f;
            if (dis.viewedObject.Position.Y - dat.viewedObject.Position.Y < -THRESHOLD)
                return -1;
            else if (dis.viewedObject.Position.Y - dat.viewedObject.Position.Y > +THRESHOLD)
                return +1;
            else if (dis.viewedObject.Position.X - dat.viewedObject.Position.X < 0)
                return -1;
            else
                return +1;
        }

        //draw texture of viewed object to the screen
        public virtual void Draw(SpriteBatch spriteBatch, LightMap map)
        {
            Texture2D image = viewedObject.SpriteSheet;
            int spriteStartX = 0;
            if (viewedObject.IsOutlet)
            {
                if (viewedObject.IsOn)
                {
                    spriteStartX = (int) viewedObject.Framesize.X;
                }
                else
                {
                    spriteStartX = 0;
                }
            } 
            else 
            {
                spriteStartX = (int) (viewedObject.Orientation * viewedObject.Framecount / MathHelper.TwoPi);
                while (spriteStartX < 0)
                {
                    spriteStartX += viewedObject.Framecount;
                }
                while (spriteStartX >= viewedObject.Framecount)
                {
                    spriteStartX -= viewedObject.Framecount;
                }
                spriteStartX *= (int) viewedObject.Framesize.X;
            }


            //Get average tint over all points within the light convex hull 
            //every 1 meter radiating outwards in a grid from the "center" of the object.
            float minX = viewedObject.GetScreenPosition().X; float maxX = minX;
            float minY = viewedObject.GetScreenPosition().Y; float maxY = minY;
            foreach(Vector2 v in viewedObject.LightConvexHull) {
                if(v.X > maxX) maxX = v.X;
                if(v.X < minX) minX = v.X;
                if(v.Y > maxY) maxY = v.Y;
                if(v.Y < minY) minY = v.Y;
            }
            int tintSum = 0; int count = 0;
            for (float n = viewedObject.GetScreenPosition().X; n <= maxX; n += GameObject.worldScale)
            {
                for (float m = viewedObject.GetScreenPosition().Y; m <= maxY; m += GameObject.worldScale)
                {
                    tintSum += map.GetLightValue((int)n, (int)m);
                    count++;
                }
            }
            for (float n = viewedObject.GetScreenPosition().X - GameObject.worldScale; n >= minX; n -= GameObject.worldScale)
            {
                for (float m = viewedObject.GetScreenPosition().Y; m <= maxY; m += GameObject.worldScale)
                {
                    tintSum += map.GetLightValue((int)n, (int)m);
                    count++;
                }
            }
            for (float n = viewedObject.GetScreenPosition().X; n <= maxX; n += GameObject.worldScale)
            {
                for (float m = viewedObject.GetScreenPosition().Y - GameObject.worldScale; m >= minY; m -= GameObject.worldScale)
                {
                    tintSum += map.GetLightValue((int)n,(int)m);
                    count++;
                }
            }
            for (float n = viewedObject.GetScreenPosition().X - GameObject.worldScale; n >= minX; n -= GameObject.worldScale)
            {
                for (float m = viewedObject.GetScreenPosition().Y - GameObject.worldScale; m >= minY; m -= GameObject.worldScale)
                {
                    tintSum += map.GetLightValue((int)n,(int)m);
                    count++;
                }
            }


            int tint = tintSum / count;

            spriteBatch.Draw(image, viewedObject.GetScreenPosition(), new Rectangle(spriteStartX, 0, (int) viewedObject.Framesize.X, (int) viewedObject.Framesize.Y), 
                             new Microsoft.Xna.Framework.Color(tint, tint, tint), 0f, viewedObject.GetSpriteCenter(), 
                             GameObject.worldScale/viewedObject.ImgScale, SpriteEffects.None, 0);
                
        }
        public void DrawWires(SpriteBatch spriteBatch)
        {
            foreach (Wire wire in viewedObject.Wires)
            {
                Texture2D wiretexture = viewedObject.cordPic;
                wire.Draw(wiretexture, spriteBatch);

            }
        }
    }
}
