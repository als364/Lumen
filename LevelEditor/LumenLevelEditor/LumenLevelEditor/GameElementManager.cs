using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using MyDataTypes;
using System.Diagnostics;
using System.IO;

namespace LumenLevelEditor
{

    /// <summary>
    /// Contains a list of GameElements
    /// </summary>
    public class GameElementManager
    {
        //the size of everything on the menu
        public const int menuSizeWidth = 50;
        public const int menuSizeHeight = 50;

        public const int margins = 10;
        public const int picsPerRow = 4;

        private List<GameElement> elements;
        private List<Exit> exits;

        BasicEffect effect;

        private GraphicsDevice device;

        public GameElementManager(GraphicsDevice dev)
        {
            effect = new BasicEffect(dev);
            elements = new List<GameElement>();
            exits = new List<Exit>();

            device = dev;
        }

        /// <summary>
        /// React to moust click
        /// </summary>
        /// <param name="ms"></param>
        public GameElement OnClick(MouseState ms, Engine e) {
            //check if any GameElement is being selected
            foreach (GameElement ge in elements)
            {
                if (ge.isPressed(ms.X, ms.Y, e.WorldPosition))
                {
                    //if it is (and is in the world), return that element (which will be set to the selected element)
                    if (!ge.isMenu())
                    {
                        return ge;
                    }
                    else
                    {
                        GameElement elem = ge.Copy(ms, e);
                        Add(elem);
                        return elem;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// populate GameElements (Done by reading in all available GameObject XML files)
        /// </summary>
        /// <param name="content"></param>
        public void PopulateMenu(ContentManager content, Point p)
        {
            //TODO

            String[] xmlFiles = Reader.ParseAvailableGameElements("Content/xml/AvailableGameElements.xml");

            //for each xml file in subfolder

            //Read in XML file
            //create new GameElement
            //make position into a grid pattern along right side of level editor    
            //also, load sprite, put into GameElement's sprite field
            for (int n = 0; n < xmlFiles.Length; n++)
            {
                ObjectType typ = Reader.ParseObjectType("Content/" + xmlFiles[n] + ".xml"); //content.Load<ObjectType>(xmlFiles[n]);

                Texture2D pic = Reader.LoadImage("Content/" + typ.SpriteFilePath + ".png", device);

                float imgScale = typ.ImgScale;
                int spriteWidth = typ.spriteWidth;
                int spriteHeight = typ.spriteHeight;
                int type = typ.Type;
                Vector2 spriteCenter = typ.spriteCenter;

                int[] pwrsrc = new int[0];

                Add(new GameElement(p.X + (n % picsPerRow) * (menuSizeWidth + margins),
                    p.Y + (n / picsPerRow) * (menuSizeHeight + margins), 
                    imgScale, spriteWidth, spriteHeight, spriteCenter, pic,
                    true, menuSizeWidth, menuSizeHeight, xmlFiles[n], type, pwrsrc));
            }

            
        }

        /// <summary>
        /// Creates a border of walls in the world. Removes old border automatically.
        /// </summary>
        public void CreateBorder(float width, float height)
        {
            //clear old borders
            for (int n = 0; n < elements.Count; n++)
            {
                if (elements[n].IsAutoBorder)
                {
                    elements.RemoveAt(n);
                    n--;
                }
            }

            //create new borders
            int currentListSize;

            //load in data for side wall
            GameElement sidewall = null;
            foreach(GameElement elem in elements) {
                if(elem.getXMLDataFileLocation() == "xml/WallSide") {
                    sidewall = elem;
                    break;
                }
            }

            //left side
            currentListSize = elements.Count;
            for (int y = 2; y < 2*height - 1; y++)
            {
                int x = 1;
                elements.Add(sidewall.Copy(x, y/2f));
                elements[currentListSize + y - 2].IsAutoBorder = true;
            }

            //right side
            currentListSize = elements.Count;
            for (int y = 2; y < 2*height - 1; y++)
            {
                int x = (int)width - 2;
                elements.Add(sidewall.Copy(x, y/2f));
                elements[currentListSize + y - 2].IsAutoBorder = true;
            }

            //load in data for top wall
            GameElement topwall = null;
            foreach (GameElement elem in elements)
            {
                if (elem.getXMLDataFileLocation() == "xml/WallTop")
                {
                    topwall = elem;
                    break;
                }
            }

            //top
            currentListSize = elements.Count;
            for (int x = 2; x < width - 2; x++)
            {
                int y = 1;
                elements.Add(topwall.Copy(x, y));
                elements[currentListSize + x - 2].IsAutoBorder = true;
            }

            //bottom
            currentListSize = elements.Count;
            for (int x = 2; x < width - 2; x++)
            {
                int y = (int)height - 1;
                elements.Add(topwall.Copy(x, y));
                elements[currentListSize + x - 2].IsAutoBorder = true;
            }
        }

        //Scrubs the element list of any gameElements that are in the world. 
        //Menu game Elements will be left alone.
        public void clearWorld()
        {

            List<GameElement> tempList = new List<GameElement>();
            foreach (GameElement ge in this.elements)
            {
                if (ge.isMenu())
                {
                    tempList.Add(ge);
                }
            }

            elements = tempList;

            exits.Clear();
        }

        public void Remove(GameElement ge)
        {
            elements.Remove(ge);
        }

        public void Add(GameElement ge)
        {
            elements.Add(ge);
        }

        public void Add(Exit ex)
        {
            exits.Add(ex);
        }

        public bool RemoveExit(Vector2 point)
        {
            foreach (Exit ex in exits)
            {
                if (ex.TLCorner.X <= point.X && ex.BRCorner.X >= point.X &&
                   ex.TLCorner.Y <= point.Y && ex.BRCorner.Y >= point.Y)
                {
                    exits.Remove(ex);
                    return true;
                }
            }
            return false;
        }

        private int ComparePositions(GameElement dis, GameElement dat)
        {
            float THRESHOLD = .4f;
            if (dis.getPosition().Y - dat.getPosition().Y < -THRESHOLD)
                return -1;
            else if (dis.getPosition().Y - dat.getPosition().Y > +THRESHOLD)
                return +1;
            else if (dis.getPosition().X - dat.getPosition().X < 0)
                return -1;
            else if (dis.getPosition().X - dat.getPosition().X > 0)
                return +1;
            else return +1; 
        }

        /// <summary>
        /// draws GameElements that are in the world, along with lines connecting power sources
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="device"></param>
        /// <param name="worldPosition"></param>
        public void DrawWorld(SpriteBatch spriteBatch, GraphicsDevice device, Vector2 worldPosition, SpriteFont font)
        {
            elements.Sort(ComparePositions);

            //set up shading for line drawing
            effect.VertexColorEnabled = true;
            effect.CurrentTechnique.Passes[0].Apply();

            foreach (GameElement ge in elements)
            {
                if (!ge.isMenu() && ge.onScreen(worldPosition))
                {
                    ge.Draw(spriteBatch, worldPosition);

                    //draw a line from each object to its power source
                    List<int> sources = ge.PowerSource;
                    for (int n = 1; n < sources.Count; n++)
                    {
                        int i = sources[n];
                        foreach (GameElement src in elements)
                        {
                            if (src.PowerSource[0] == i)
                            {
                                Vector2 dis = new Vector2(ge.GetScreenPosition(worldPosition).X * 2 * GameElement.worldScale / Engine.WIDTH - 1f,
                                                          1f - ge.GetScreenPosition(worldPosition).Y * 2 * GameElement.worldScale / Engine.HEIGHT);
                                Vector2 dat = new Vector2(src.GetScreenPosition(worldPosition).X * 2 * GameElement.worldScale / Engine.WIDTH - 1f,
                                                          1f - src.GetScreenPosition(worldPosition).Y * 2 * GameElement.worldScale / Engine.HEIGHT);

                                VertexPositionColor[] linePoints = { 
                                   new VertexPositionColor(new Vector3(dis, 0), Color.Black), 
                                   new VertexPositionColor(new Vector3(dat, 0), Color.Black) };
                                device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, linePoints, 0, 1);
                                break;
                            }
                        }
                    }
                }
            }

            foreach (Exit ex in exits)
            {
                ex.Draw(spriteBatch, worldPosition, font);
            }
        }

        public List<GameElement> Elements
        {
            get 
            {
                return elements;
            }
        }

        public List<Exit> Exits
        {
            get
            {
                return exits;
            }
        }

        public void DrawMenuSprites(SpriteBatch spriteBatch) 
        {
            foreach (GameElement ge in elements)
            {
                if (ge.isMenu())
                {
                    ge.Draw(spriteBatch, new Vector2(0,0));
                }
            }
        }

        public bool ReplaceWallImage(GameElement replace, MouseState ms, Engine e)
        {
            Vector2 worldPos = e.WorldPosition;
            //Vector2 pos = new Vector2(worldPos.X + ms.X / GameElement.worldScale, worldPos.Y + ms.Y / GameElement.worldScale);
            foreach (GameElement ge in elements)
            {
                if (!ge.isMenu() && ge.getXMLDataFileLocation().Contains("xml/walls/") && ge != replace) 
                {
                    Vector2 tlCorner = ge.GetScreenPosition(worldPos) * GameElement.worldScale - 
                                       new Vector2(ge.SpriteCenter.X * GameElement.worldScale / ge.ImgScale, 
                                                   ge.SpriteCenter.Y * GameElement.worldScale / ge.ImgScale);
                    Vector2 brCorner = new Vector2(tlCorner.X + ge.SpriteWidth * GameElement.worldScale / ge.ImgScale,
                                                   tlCorner.Y + ge.SpriteHeight * GameElement.worldScale / ge.ImgScale);

                    if (tlCorner.X < ms.X && ms.X < brCorner.X && tlCorner.Y < ms.Y && ms.Y < brCorner.Y)
                    {
                        elements.Add(e.WriteWallXML(tlCorner / GameElement.worldScale, brCorner / GameElement.worldScale, replace.getXMLDataFileLocation()));
                        elements.Remove(ge);
                        elements.Remove(replace);
                        return true;
                    }             
                }
            }
            return false;
        }
    }
}
