using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LumenLevelEditor
{
    public class Exit 
    {
        private string nextLevel;
        private Vector2 tlCorner;
        public Vector2 TLCorner { get { return tlCorner; } }
        private Vector2 brCorner;
        public Vector2 BRCorner { get { return brCorner; } }
        private Texture2D sprite;

        private int src, sink;
        public int Origin { get { return src; } }
        public int Destination { get { return sink; } }

        private int direction;
        public int Direction { get { return direction; } set { direction = value; } }



        private const float SPRITE_SCALE = 50;
        private const int SPRITE_WIDTH = 50;
        private const int SPRITE_HEIGHT = 50;
        private static Vector2 SPRITE_CENTER = new Vector2(25, 25);

        public string NextLevel { get { return nextLevel; } }
        public float[] Points { get { float[] turn = { tlCorner.X, tlCorner.Y, brCorner.X, brCorner.Y }; return turn; } }

        /*public Exit(float x, float y, Texture2D pic) : base(x, y, SPRITE_SCALE, SPRITE_WIDTH, SPRITE_HEIGHT, SPRITE_CENTER, pic, 
            false, 0, 0, "", -1, new int[0]) {} //string xmlFile, int theType, int[] pwrsrc)*/
        public Exit(Vector2 pt1, Vector2 pt2, string s, Texture2D pic, int orig, int dest)
        {
            tlCorner = pt1;
            brCorner = pt2;
            nextLevel = s;
            sprite = pic;
            src = orig;
            sink = dest;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 worldPos, SpriteFont font)
        {
            spriteBatch.Draw(sprite, new Rectangle((int)((tlCorner.X - worldPos.X) * GameElement.worldScale),
                                                   (int)((tlCorner.Y - worldPos.Y) * GameElement.worldScale),
                                                   (int)((brCorner.X - tlCorner.X) * GameElement.worldScale),
                                                   (int)((brCorner.Y - tlCorner.Y) * GameElement.worldScale)), Color.White);
            spriteBatch.DrawString(font, (" "+src + "," + sink), new Vector2((tlCorner.X - worldPos.X) * GameElement.worldScale,
                                                   (tlCorner.Y - worldPos.Y) * GameElement.worldScale), Color.Red);
        }
    }
}
