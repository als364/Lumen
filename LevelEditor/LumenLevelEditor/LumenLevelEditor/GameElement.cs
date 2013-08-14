using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;

namespace LumenLevelEditor
{

    /// <summary>
    /// A class which represents some GameObject (or Light or Girl) that will exist in the Level
    /// </summary>
    public class GameElement
    {
        private Vector2 position; //position either in game world or in menu, as denoted by boolean below
        private bool menu; //true if in right hand menu, false if in world
        private int spriteWidth, spriteHeight;
        public int SpriteWidth { get { return spriteWidth; } }
        public int SpriteHeight { get { return spriteHeight; } }
        private int menuWidth, menuHeight;
        private Vector2 spriteCenter;
        public Vector2 SpriteCenter { get { return spriteCenter; } }

        private float imgScale;
        public float ImgScale { get { return imgScale; } }
        private string xmlFileLocation;

        private float orientation;
        private bool on;
        private int Type; // Type of object, needs to be saved later when we export for/to the game.
        private List<int> powerSource;
        private List<Vector2[]> wires;
        public List<Vector2[]> Wires { get { return wires; } }

        //private bool rotatable;
        //private bool slidable;

        protected Texture2D sprite;

        public const float worldScale = 40f;

        private static int maxPowerSource;

        private bool autoBorder;
        public bool IsAutoBorder { get { return autoBorder; } set { autoBorder = value; } }

        public GameElement(float x, float y, float scale, int sw, int sh, Vector2 spriteCent, Texture2D pic, 
            bool isMenuItem, int mw, int mh, string xmlFile, int theType, int[] pwrsrc)
        {
            position = new Vector2(x, y);

            imgScale = scale;
            spriteWidth = sw;
            spriteHeight = sh;
            spriteCenter = spriteCent;
            sprite = pic;

            menu = isMenuItem;
            menuWidth = mw;
            menuHeight = mh;

            xmlFileLocation = xmlFile;

            powerSource = new List<int>();
            powerSource.AddRange(pwrsrc);
            foreach (int n in powerSource)
            {
                if (n > maxPowerSource)
                {
                    maxPowerSource = n;
                }
            }

            if (!menu && powerSource.Count == 0)
            {
                powerSource.Add(maxPowerSource);
                maxPowerSource++;
            }


            on = false;
            orientation = 0f;
            Type = theType;
            autoBorder = false;
            wires = new List<Vector2[]>();
        }

        /// <summary>
        /// checks if given x and y are within the gameElement's screenPosition
        /// </summary>
        public bool isPressed(float x, float y, Vector2 worldPos)
        {
            //is x,y within this element's rectangle on the screen
            Vector2 pos = GetScreenPosition(worldPos);

            if (menu)
            {
                return x >= pos.X && x <= pos.X + menuWidth && y >= pos.Y && y <= pos.Y + menuHeight;
            }
            else
            {
                return onScreen(worldPos) &&
                    x >= (pos.X - .5) * worldScale && x <= (pos.X + .5) * worldScale &&
                    y >= (pos.Y - .25) * worldScale && y <= (pos.Y + .25) * worldScale;
                    /*x >= pos.X * worldScale && x <= (pos.X + spriteWidth / imgScale) * worldScale && 
                    y >= pos.Y * worldScale && y <= (pos.Y + spriteHeight / imgScale) * worldScale ;*/
            }
        }

        private bool onScreen(float x, float y, float worldX, float worldY) {
            return x >= worldX && y >= worldY && x <= worldX+Engine.WORLD_WIDTH && y <= worldY+Engine.WORLD_HEIGHT;
        }

        public bool onScreen(Vector2 worldPosition)
        {
            return onScreen(GetScreenPosition(worldPosition).X * worldScale, GetScreenPosition(worldPosition).Y * worldScale, 0, 0);
        }

        /// <summary>
        /// returns upper left hand corner of gameElement's position on the screen. 
        /// Varies based on whether it is on the right hand menu or actually in the world.
        /// </summary>
        public Vector2 GetScreenPosition(Vector2 worldPos) {
            if (menu)
            {
                return position;
            }
            else
            {
                return new Vector2(position.X - worldPos.X, position.Y - worldPos.Y);
            }
        }

        public GameElement Copy(MouseState ms, Engine e)
        {
            return Copy(ms.X / worldScale + e.WorldPosition.X, ms.Y / worldScale + e.WorldPosition.Y);
        }

        public GameElement Copy(float x, float y)
        {
            return new GameElement(x, y, imgScale, spriteWidth, spriteHeight, spriteCenter, sprite,
                                   false, menuWidth, menuHeight,
                                   xmlFileLocation, Type, powerSource.ToArray());
        }

        /// <summary>
        /// react to release of mouse. This game Element was previously selected
        /// </summary>
        /// <param name="ms"></param>
        public void Released(MouseState ms, Engine e) {
            //if started in menu:
            if (menu)
            {
                //if moved into game world, add that Game Element to the world at that position (snap to grid?)
                throw new Exception("has selected a menu element");
            }
            //if started in game world:
            else
            {
                float x = ms.X; float y = ms.Y; 
                Vector2 worldPos = e.WorldPosition;
                //float worldX = 0; float worldY = 0;

                //if not moved, change displayed menu to be this GameElement’s menu
                /*if (isPressed(x, y, worldPos))
                {
                    //e.setGameElementMenu(this);
                }
                //if moved within game world, remove from original position and move to the new position. (snap to grid?)                
                else*/
                if (onScreen(x, y, 0, 0))
                {
                    if (!e.SnapToGrid)
                    {
                        position = new Vector2(worldPos.X + x / worldScale, worldPos.Y + y / worldScale);
                    }
                    else
                    {
                        float xPos = (float) Math.Round((double) (worldPos.X + x / worldScale));
                        float yPosX2 = (float)Math.Round(2.0 * (double)(worldPos.Y + y / worldScale));
                        position = new Vector2(xPos, yPosX2 / 2f);
                    }
                }
                //if moved out of game world, then remove from the level
                else
                {
                    e.RemoveElement(this);
                }
            }
        }

        /// <summary>
        /// Draw sprite to its position in the world (or on the menu), 
        /// assuming it is within the viewing portal of the World.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch, Vector2 worldPosition)
        {
            if (menu)
            {
                spriteBatch.Draw(sprite, new Rectangle((int)position.X, (int)position.Y, menuWidth, menuHeight),
                    new Rectangle(0, 0, spriteWidth, spriteHeight), Color.White, Orientation, new Vector2(0, 0), 
                    SpriteEffects.None, 1f);
            }
            else
            {
                spriteBatch.Draw(sprite, GetScreenPosition(worldPosition) * worldScale, 
                    new Rectangle(0, 0, spriteWidth, spriteHeight), Color.White, Orientation, spriteCenter, 
                    worldScale/imgScale, SpriteEffects.None, 1f);
            }
        }

        public bool isMenu()
        {
            return menu;
        }

        public void TurnOnSwitch()
        {
            on = !on;
        }

        public float Orientation
        {
            get
            {
                return orientation;
            }
            set
            {
                orientation = value;
            }
        }

        public int getType()
        {
            return Type;
        }

        public List<int> PowerSource { get { return powerSource; } }

        public string getXMLDataFileLocation()
        {
            return this.xmlFileLocation;
        }

        public Vector2 getPosition()
        {
            return position;
        }
        public void SetPosition(MouseState ms, Vector2 worldPos)
        {
            position = new Vector2(ms.X / worldScale + worldPos.X, ms.Y / worldScale + worldPos.Y);
        }

        public float getOrientation()
        {
            return orientation;
        }

        public bool On {
            get { return on; }
            set { on = value; }
        }

    }
}
