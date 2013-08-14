using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MyDataTypes;
using LumenLevelEditor;
using System.Xml;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using System.Diagnostics;
using System.Xml.Serialization;
using System.IO;

namespace LumenLevelEditor
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Engine : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        BasicEffect effect;

        SpriteFont font;

        private ButtonManager buttons;
        private GameElementManager gameElementManager;
        //private List<GameElement> gameElements;
        
        //the last clicked on GameElement
        //is null if no GameElement is selected
        private GameElement selected;

        private GameElement visibleMenu;

        private Field activeField;

        //position of "world", i.e. what portion of the world you are viewing on screen
        Vector2 worldPos;

        //size of world when drawn to screen
        public const int WORLD_WIDTH = 800;
        public const int WORLD_HEIGHT = 780;

        //size of level editor
        public const int WIDTH = 1200;
        public const int HEIGHT = 810;

        //button information
        private bool shadowsDisplayed;
        private bool snapToGrid;

        private int addingExit; //0 no, 1 mouse unclicking, 2 point 1, 3 point 2
        private Vector2 exitPoint1;
        private Exit addedExit;

        private int addingWall;//0 no, 1 mouse unclicking, 2 holding and dragging
        private Vector2 wallPoint1;
        private const int WALL_SPRITE_WIDTH = 900;
        private const int WALL_SPRITE_HEIGHT = 900;

        //field information
        private float levelHeight;
        private float levelWidth;
        private float initialCourage;
        private float ambientLight;
        private int exitOrigin;
        private int exitDestination;

        private string levelDest;

        private Vector2 selectionPos;

        private int selectingSource; //0 is no, 1 is waiting for mouse release, 2 waiting for mouse click
        private List<Vector2> wirePoints;

        private Texture2D exitPic;
        
        public Engine()
        {
            graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";

            

            graphics.PreferredBackBufferWidth = WIDTH;
            graphics.PreferredBackBufferHeight = HEIGHT;
            graphics.ApplyChanges();

            IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            effect = new BasicEffect(GraphicsDevice);

            //create window of constant size, split into world viewing portal and menu (on right, i imagine)

            worldPos = new Vector2(0, 0);
            shadowsDisplayed = false;
            snapToGrid = true;

            //field information
            levelHeight = 30;
            levelWidth = 30;
            initialCourage = 10f;
            ambientLight = 1f;

            addingExit = 0;
            addingWall = 0;

            levelDest = "samplelevel";

            activeField = null;

            buttons = new ButtonManager(GraphicsDevice, this);
            //TODO populate buttonManager

            gameElementManager = new GameElementManager(GraphicsDevice);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Debug.WriteLine(Content.RootDirectory);

            font = Content.Load<SpriteFont>("Font");
            SpriteFont font14 = Content.Load<SpriteFont>("Font14");
            buttons.setFont(font14);

            exitPic = Reader.LoadImage("Content/images/exit.png", GraphicsDevice);

            //populate GamegameElementManager (Done by reading in all available GameObject XML files)
            gameElementManager.PopulateMenu(Content, buttons.GetElementMenuStartPos());

            //gameElementManager.CreateBorder(levelWidth, levelHeight);


            
            //load in each xml file. For each GameObject xml file, we create a new GameElement with the given sprite image
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            MouseState ms = Mouse.GetState();
            KeyboardState ks = Keyboard.GetState();

            if (ks.IsKeyDown(Keys.Escape))
                this.Exit();
            else if (ks.IsKeyDown(Keys.Left))
                worldPos += new Vector2(-1, 0);
            else if (ks.IsKeyDown(Keys.Right))
                worldPos += new Vector2(+1, 0);
            else if (ks.IsKeyDown(Keys.Up))
                worldPos += new Vector2(0, -1);
            else if (ks.IsKeyDown(Keys.Down))
                worldPos += new Vector2(0, +1);


            if (addingExit != 0)
            {
                if (addingExit == 1 && ms.LeftButton == ButtonState.Released)
                {
                    addingExit++;
                }
                else if (addingExit == 2 && ms.LeftButton == ButtonState.Pressed)
                {
                    exitPoint1 = new Vector2((ms.X / GameElement.worldScale) + worldPos.X, 
                                             (ms.Y / GameElement.worldScale) + worldPos.Y);
                    if (gameElementManager.RemoveExit(exitPoint1))
                    {
                        addingExit = 0;
                    }
                    else
                    {
                        addingExit++;
                    }
                }
                else if (addingExit == 3 && ms.LeftButton == ButtonState.Released)
                {
                    Vector2 exitPoint2 = new Vector2((ms.X / GameElement.worldScale) + worldPos.X,
                                    (ms.Y / GameElement.worldScale) + worldPos.Y);
                    if(exitPoint2.X > exitPoint1.X && exitPoint2.Y > exitPoint1.Y) {
                        addedExit = new Exit(exitPoint1, exitPoint2,
                            levelDest, exitPic, exitOrigin, exitDestination);
                        gameElementManager.Add(addedExit);
                        addingExit++;
                    } else {
                        addingExit = 0;
                    }
                }
                else if(addingExit == 4 && ms.LeftButton == ButtonState.Pressed) {
                    int dir = -1;
                    if (ms.X < (addedExit.TLCorner.X - worldPos.X) * GameElement.worldScale)
                        dir = 2;
                    else if (ms.X > (addedExit.BRCorner.X - worldPos.X) * GameElement.worldScale)
                        dir = 3;
                    else if (ms.Y < (addedExit.TLCorner.Y - worldPos.Y) * GameElement.worldScale)
                        dir = 1;
                    else
                        dir = 0;
                    addedExit.Direction = dir;
                    addingExit = 0;
                }
            }
            else if (addingWall > 0)
            {
                if (addingWall == 1 && ms.LeftButton == ButtonState.Released)
                {
                    addingWall++;
                }
                else if (addingWall == 2 && ms.LeftButton == ButtonState.Pressed)
                {
                    if (snapToGrid)
                    {
                        float xPos = worldPos.X + ms.X / GameElement.worldScale;
                        int n;
                        for (n=0; xPos > 1; n++)
                        {
                            xPos--;
                        }
                        xPos = n + .5f;

                        float yPosX2 = 2f * (worldPos.Y + ms.Y / GameElement.worldScale);
                        for (n=0; yPosX2 > 1; n++) 
                        {
                            yPosX2--;
                        }
                        yPosX2 = n + .5f;

                        wallPoint1 = new Vector2(xPos, yPosX2 / 2f);
                    }
                    else
                    {
                        wallPoint1 = new Vector2((ms.X / GameElement.worldScale) + worldPos.X,
                                                 (ms.Y / GameElement.worldScale) + worldPos.Y);
                    }
                    addingWall++;
                }
                else if (addingWall == 3 && ms.LeftButton == ButtonState.Released)
                {
                    Vector2 wallPoint2;
                    if (snapToGrid)
                    {
                        float xPos = worldPos.X + ms.X / GameElement.worldScale;
                        int n;
                        for (n = 0; xPos > 1; n++)
                        {
                            xPos--;
                        }
                        xPos = n + .5f;

                        float yPosX2 = 2f * (worldPos.Y + ms.Y / GameElement.worldScale);
                        for (n = 0; yPosX2 > 1; n++)
                        {
                            yPosX2--;
                        }
                        yPosX2 = n + .5f;

                        wallPoint2 = new Vector2(xPos, yPosX2 / 2f);
                    }
                    else
                    {
                        wallPoint2 = new Vector2((ms.X / GameElement.worldScale) + worldPos.X,
                                                 (ms.Y / GameElement.worldScale) + worldPos.Y);
                    }
                    if (wallPoint2.X > wallPoint1.X && wallPoint2.Y > wallPoint1.Y)
                    {
                        GameElement wall = WriteWallXML(wallPoint1, wallPoint2, "xml/WallTop");
                        gameElementManager.Add(wall);
                    }
                    addingWall = 0; 
                }
            }
            else if (selectingSource > 0)
            {
                if(selectingSource == 1) {
                    if (ms.LeftButton == ButtonState.Released)
                    {
                        selectingSource++;
                    }
                } else {
                    if (ms.LeftButton == ButtonState.Pressed)
                    {
                        GameElement elem = gameElementManager.OnClick(ms, this);
                        if (elem != null && !elem.isMenu())
                        {
                            wirePoints.Add(elem.getPosition());
                            visibleMenu.Wires.Add(wirePoints.ToArray());
                            visibleMenu.PowerSource.Add(elem.PowerSource[0]);
                            selectingSource = 0;
                        }
                        else
                        {
                            wirePoints.Add(new Vector2((ms.X / GameElement.worldScale) + worldPos.X,
                                                       (ms.Y / GameElement.worldScale) + worldPos.Y));
                            selectingSource = 1;
                        }
                    }
                    else if (ms.RightButton == ButtonState.Pressed)
                    {
                        selectingSource = 0;
                    }
                }
            }
            //if mouse not previously selected a game element
            else if (selected == null)
            {
                //check if mouse pressed
                if (ms.LeftButton == ButtonState.Pressed)
                {
                    if (!buttons.OnClick(ms, this, (visibleMenu == null)))
                    {
                        selected = gameElementManager.OnClick(ms, this);
                        if (selected != null)
                            selectionPos = new Vector2(ms.X, ms.Y);
                    }
                    //ButtonManager.OnClick
                    //could change position of game world
                    //could change data for a given game element (later stored in xml)
                    //could try to save level
                    //could try to load level
                    //might turn shadows off?
                    //change data for the level (later stored in xml)
                    //might change menu
                    //GameElementManger.OnClick
                    //set selectedGameElement to appropriate GameElement
                }
                else
                {
                    //release mouse for button manager
                    buttons.OnRelease();
                }
            }
            //if there is some Game Element that was previously clicked on
            else
            {
                Vector2 currentMousePos = new Vector2(ms.X, ms.Y);

                //check if mouse released
                if (ms.LeftButton == ButtonState.Released)
                {
                    //release mouse for button manager
                    buttons.OnRelease();

                    bool onWall = gameElementManager.ReplaceWallImage(selected, ms, this);

                    if (!onWall)
                    {
                        if ((currentMousePos - selectionPos).Length() > 5f)
                            //GameElement.Released
                            //might add new Game Element to world
                            //might move GameElement in the world
                            //might remove GameElement from the world
                            selected.Released(ms, this);
                        else
                            // change menu
                            visibleMenu = selected;
                    }

                    selected = null;
                }
                else
                {
                    if ((currentMousePos - selectionPos).Length() > 5f)
                        //drag picture of game element along with the mouse
                        selected.SetPosition(ms, WorldPosition);
                }

            }

            if (activeField != null)
            {
                activeField.Typed(ks, this);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            if (snapToGrid)
            {
                DrawGridLines(spriteBatch);
            }

            //Read in gameElementManager from GameElementManager. 
            gameElementManager.DrawWorld(spriteBatch, GraphicsDevice, worldPos, font);

            if (visibleMenu == null)
            {
                gameElementManager.DrawMenuSprites(spriteBatch);
                buttons.DrawMainMenu(spriteBatch);
            }
            else
            {
                buttons.DrawElementMenu(spriteBatch);
            }

            spriteBatch.End();

            //draw line from center of exit to mouse when specifying direction
            if (addingExit == 4)
            {
                effect.VertexColorEnabled = true;
                effect.CurrentTechnique.Passes[0].Apply();

                Vector2 dis = new Vector2((float) Mouse.GetState().X * 2f / Engine.WIDTH - 1f, 
                                          1f - (float) Mouse.GetState().Y * 2f / Engine.HEIGHT);
                float screenX = ((addedExit.BRCorner.X / 2 + addedExit.TLCorner.X / 2) - worldPos.X) * GameElement.worldScale;
                float screenY = ((addedExit.BRCorner.Y / 2 + addedExit.TLCorner.Y / 2) - worldPos.Y) * GameElement.worldScale;
                Vector2 dat = new Vector2(screenX * 2 / Engine.WIDTH - 1f,
                                          1f - screenY * 2 / Engine.HEIGHT);

                Console.WriteLine(dis + ", " + dat);
                VertexPositionColor[] linePoints = { 
                                   new VertexPositionColor(new Vector3(dis, 0), Color.Black), 
                                   new VertexPositionColor(new Vector3(dat, 0), Color.Black) };
                GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, linePoints, 0, 1);
            }

            //draw buttons to screen

            base.Draw(gameTime);
        }


        /// <summary>
        /// Draws horizontal lines and vertical lines representing the grid 
        /// onto the visible portions of the game world.
        /// Vertical line every 1/2 meter, horizontal one every meter.
        /// </summary>
        private void DrawGridLines(SpriteBatch spriteBatch) {
            //get a pixel for drawing
            Texture2D whitePixel = new Texture2D(GraphicsDevice, 1, 1);
            Color[] data = {Color.White};
            whitePixel.SetData<Color>(data);

            //draw vertical lines
            for (int n = 1; n < levelWidth; n++)
            {
                int xPos = (int) (( n -.5f - worldPos.X ) * GameElement.worldScale);
                if (xPos >= 0 && xPos <= WORLD_WIDTH)
                {
                    spriteBatch.Draw(whitePixel, new Rectangle(xPos, 0, 1, WORLD_HEIGHT), Color.Black);
                }
            }

            //draw horizontal lines
            for (int n = 1; n < 2 * levelHeight; n++)
            {
                int yPos = (int)((n - .5f - 2*worldPos.Y) * GameElement.worldScale/2);
                if (yPos >= 0 && yPos <= WORLD_HEIGHT)
                {
                    spriteBatch.Draw(whitePixel, new Rectangle(0, yPos, WORLD_WIDTH, 1), Color.Black);
                }
            }
        }

        /// <summary>
        /// Write data to an XML file
        /// </summary>
        public void Save()
        {
            //Read gameElementManager from GameElementManager. 
            //Stores position, orientation, etc data in xml file. 
            //Also store global attributes set by button on the overarching menu.

            List<GameElement> gameElements = gameElementManager.Elements;
            List<Exit> levelExits = gameElementManager.Exits;


            LevelDef lvlDef = new LevelDef();
            lvlDef.floorTextureLocation = "images/floor";
            lvlDef.ambientLight = this.ambientLight;
            lvlDef.initialCourage = this.initialCourage;
            lvlDef.levelHeight = this.levelHeight;
            lvlDef.levelWidth = this.levelWidth;
            lvlDef.levelObjectsLocation = "xml/"+levelDest+"Objects";
            lvlDef.levelExitsLocation = "xml/" + levelDest + "Exits";

            List<string> exitLocs = new List<string>();
            List<float> exitPoints = new List<float>();

            List<ObjectInstance> objectsToSerialize = new List<ObjectInstance>();
            List<ExitInstance> exitsToSerialize = new List<ExitInstance>();

            foreach (GameElement element in gameElements)
            {
                if (!element.isMenu()) 
                {
                    ObjectInstance oi = CompressGameElement(element);
                    objectsToSerialize.Add(oi);
                    //IntermediateSerializer.Serialize<
                }
            }
            objectsToSerialize.Sort(CompareBySourceListSize);

            foreach (Exit ex in levelExits)
            {
                ExitInstance ei = CompressExit(ex);
                exitsToSerialize.Add(ei);
            }

            ObjectInstance[] objects = objectsToSerialize.ToArray();
            ExitInstance[] exits = exitsToSerialize.ToArray();

            Writer.WriteLevelDef(lvlDef, "Content/xml/" + levelDest + ".xml");
            Writer.WriteLevelDef(lvlDef, "../../../../../../Lumen/LumenContent/xml/" + levelDest + ".xml");

            Writer.WriteObjectInstances(objects, "Content/xml/" + levelDest + "Objects.xml");
            Writer.WriteObjectInstances(objects, "../../../../../../Lumen/LumenContent/xml/" + levelDest + "Objects.xml");

            Writer.WriteExitInstances(exits, "Content/xml/" + levelDest + "Exits.xml");
            Writer.WriteExitInstances(exits, "../../../../../../Lumen/LumenContent/xml/" + levelDest + "Exits.xml");

            /*
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            XmlWriter writer = XmlWriter.Create("Content/xml/" + levelDest + ".xml", settings);
            IntermediateSerializer.Serialize(writer, lvlDef, null); 
            writer.Close();

            XmlWriter writerBuild = XmlWriter.Create("../../../../LumenLevelEditorContent/xml/" + levelDest + ".xml", settings);
            IntermediateSerializer.Serialize(writerBuild, lvlDef, null);
            writerBuild.Close();

            XmlWriter writer2 = XmlWriter.Create("Content/xml/" + levelDest + "Objects.xml", settings);
            IntermediateSerializer.Serialize(writer2, objects, null);
            writer2.Close();

            XmlWriter writer2Build = XmlWriter.Create("../../../../LumenLevelEditorContent/xml/" + levelDest + "Objects.xml", settings);
            IntermediateSerializer.Serialize(writer2Build, objects, null);
            writer2Build.Close();

            
            XmlWriter writer3 = XmlWriter.Create("Content/xml/" + levelDest + "Exits.xml", settings);
            IntermediateSerializer.Serialize(writer3, exits, null);
            writer3.Close();

            XmlWriter writer3Build = XmlWriter.Create("../../../../LumenLevelEditorContent/xml/" + levelDest + "Exits.xml", settings);
            IntermediateSerializer.Serialize(writer3Build, exits, null);
            writer3Build.Close();
             */
        }

        /// <summary>
        /// used to sort object instances so power sources occur earlier in the xml file.
        /// </summary>
        public int CompareBySourceListSize(ObjectInstance dis, ObjectInstance dat)
        {
            return dis.PowerSourceID.Length - dat.PowerSourceID.Length;
        }

        //Turns a GameElement into a ObjectInstance
        public ObjectInstance CompressGameElement(GameElement element){
            ObjectInstance anInstance = new ObjectInstance();

            anInstance.XMLDataFileLocation = element.getXMLDataFileLocation();
            anInstance.InitialPosition = element.getPosition();
            anInstance.Orientation = element.getOrientation();
            anInstance.On = element.On;
            anInstance.PowerSourceID = element.PowerSource.ToArray();
            anInstance.Wires = element.Wires;
            return anInstance;
        }

        //Turns an Exit into an ExitInstance
        public ExitInstance CompressExit(Exit ex)
        {
            ExitInstance anInstance = new ExitInstance();

            anInstance.exitLocation = ex.NextLevel;
            anInstance.tlCorner = ex.TLCorner;
            anInstance.brCorner = ex.BRCorner;
            anInstance.id = ex.Origin;
            anInstance.destination = ex.Destination;
            anInstance.direction = ex.Direction;
            return anInstance;
        }

        /// <summary>
        /// Read data from an XML file
        /// </summary>
        /// xmlFile should be a location of an xml file for a levelInstance.
        public void Load() {
            gameElementManager.clearWorld();
            //Read in an XML file of the appropriate format. 
            //Will tell where different game objects exist in the world.

            //get level def
            LevelDef lvl = Reader.ParseLevelDef("Content/xml/" + levelDest + ".xml"); //Content.Load<LevelDef>("xml/"+levelDest);

            this.ambientLight = lvl.ambientLight;
            this.initialCourage = lvl.initialCourage;
            this.levelHeight = lvl.levelHeight;
            this.levelWidth = lvl.levelWidth;

            string xmlLevelObjects = lvl.levelObjectsLocation;

            //get object instance list
            ObjectInstance[] levelGameObjects = Reader.ParseObjectInstances("Content/" + xmlLevelObjects + ".xml"); //Content.Load<ObjectInstance[]>(xmlLevelObjects);

            //add each one to a list of gamegameElementManager we'll want.
            foreach(ObjectInstance gameObject in levelGameObjects){
                gameElementManager.Add(extractGameElement(gameObject));
            }


            string xmlLevelExits = lvl.levelExitsLocation;

            //get exit instance list
            ExitInstance[] levelGameExits = Reader.ParseExitInstances("Content/" + xmlLevelExits + ".xml"); //Content.Load<ExitInstance[]>(xmlLevelExits);

            //add each one to a list of gamegameElementManager we'll want.
            foreach (ExitInstance exit in levelGameExits)
            {
                gameElementManager.Add(extractExit(exit));
            }

        }

        //give us back an GameElement
        public GameElement extractGameElement(ObjectInstance theInstance){

            ObjectType theType = Reader.ParseObjectType("Content/" + theInstance.XMLDataFileLocation + ".xml"); //Content.Load<ObjectType>(theInstance.XMLDataFileLocation);

            //bool isMenuItem, float x, float y, int w, int h, int mw, int mh, Texture2D tex
            float xPos = theInstance.InitialPosition.X;
            float yPos = theInstance.InitialPosition.Y;
            int spriteWidth = theType.spriteWidth;
            int spriteHeight = theType.spriteHeight;
            int menuWidth = (int) GameElementManager.menuSizeWidth;
            int menuHeight = (int) GameElementManager.menuSizeHeight;
            string objectTypeFile = theInstance.XMLDataFileLocation;
            Vector2 spriteCenter = theType.spriteCenter;

            Texture2D texture = Reader.LoadImage("Content/" + theType.SpriteFilePath + ".png", GraphicsDevice);

            int[] powerSource = theInstance.PowerSourceID;
            float scale = theType.ImgScale;
            GameElement newElement = new GameElement(xPos, yPos, scale, spriteWidth, spriteHeight, spriteCenter, texture, 
                false, menuWidth, menuHeight, objectTypeFile, 0, powerSource);

            newElement.On = theInstance.On;
            newElement.Orientation = theInstance.Orientation;
            return newElement;
        }

        //give us back an Exit
        public Exit extractExit(ExitInstance inst) {
            Exit e = new Exit(inst.tlCorner, inst.brCorner, inst.exitLocation, exitPic, inst.id, inst.destination);
            e.Direction = inst.direction;
            return e;
        }

        //Methods called by a Button:

        public bool SnapToGrid { get { return snapToGrid; } set { snapToGrid = value; } }

        public void ChangeWorldPosition(Vector2 v)
        {
            worldPos += v;
        }

        public void switchShadows()
        {
            shadowsDisplayed = !shadowsDisplayed;
        }

        

        public void setGameElementMenu(GameElement m)
        {
            visibleMenu = m;
        }

        public void TurnOnOff(bool b)
        {
            visibleMenu.On = b;
        }

        public void RotateObject(float f)
        {
            visibleMenu.Orientation += f / 36 * MathHelper.TwoPi;
        }

        public void SelectSource()
        {
            wirePoints = new List<Vector2>();
            wirePoints.Add(visibleMenu.getPosition());
            selectingSource = 1;
        }

        public void ClearSources()
        {
            if (visibleMenu.PowerSource.Count > 0)
                visibleMenu.PowerSource.RemoveRange(1, visibleMenu.PowerSource.Count - 1);
        }


        public Vector2 WorldPosition
        {
            get
            {
                return worldPos;
            }
        }

        public void RemoveElement(GameElement ge) {
            gameElementManager.Remove(ge);
        }
        public void AddElement(GameElement ge)
        {
            gameElementManager.Add(ge);
        }


        //for field
        public void SetActiveField(Field f)
        {
            if (activeField != null)
            {
                activeField.Undo();
            }
            activeField = f;
        }

        public float AmbientLight
        { get { return ambientLight; }
          set {ambientLight = value; } }
        public float InitialCourage
        { get { return initialCourage; }
          set {initialCourage = value; } }
        public float LevelHeight
        { get { return levelHeight; }
          set { levelHeight = value; } }//gameElementManager.CreateBorder(levelWidth, levelHeight); } }
        public float LevelWidth
        { get { return levelWidth; }
            set { levelWidth = value; } }//gameElementManager.CreateBorder(levelWidth, levelHeight); }
        
        public int ExitOrigin
        { get { return exitOrigin; } 
          set { exitOrigin = value; } }
        public int ExitDestination
        { get { return exitDestination; } 
          set { exitDestination = value; } }

        public string LevelDest
        {
            get { return levelDest; }
            set { levelDest = value; }
        }

        public void AddExit()
        {
            addingExit = 1;
        }

        public void AddWall()
        {
            addingWall = 1;
        }

        public GameElement WriteWallXML(Vector2 p1, Vector2 p2, string typeXmlLoc)
        {
            ObjectType typ = Reader.ParseObjectType("Content/"+typeXmlLoc+".xml");

            string wallType = typeXmlLoc;
            while(wallType.IndexOf('/') != -1) {
                wallType = wallType.Substring(wallType.IndexOf('/')+1);
            }

            Texture2D tex = Reader.LoadImage("Content/" + typ.SpriteFilePath + ".png", GraphicsDevice);

            int wldScale = 100;

            float width = p2.X - p1.X; 
            float height = p2.Y - p1.Y; 

            int widthMax = (int) Math.Ceiling((float) width);
            int heightMax = (int) Math.Ceiling((float) height);

            int pixelWidth = (int) (width * wldScale) ;
            int pixelHeight = (int) (height * wldScale);
            int[] pwrsrc = { 0 };

            RenderTarget2D tar = new RenderTarget2D(GraphicsDevice,
                    widthMax * wldScale, heightMax * wldScale);

            GraphicsDevice.SetRenderTarget(tar);
            spriteBatch.Begin();
            for (int w = 0; w < pixelWidth; w += wldScale)
            {
                for (int h = 0; h < pixelHeight; h += wldScale)
                {
                    spriteBatch.Draw(tex, new Rectangle(w, h, wldScale, wldScale), 
                                     new Rectangle(0, 0, typ.spriteWidth, typ.spriteHeight), Color.White);
                }
            }
            spriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);

            string name = "Wall"+pixelWidth+"by"+pixelHeight+wallType;

            FileStream writeStream = new FileStream("Content/images/walls/"+name+".png", FileMode.Create);
            tar.SaveAsPng(writeStream, widthMax * wldScale, heightMax * wldScale);
            writeStream.Close();

            if (File.Exists("../../../../../../Lumen/LumenContent/images/exit.png"))
            {
                FileStream stream2 = new FileStream("../../../../../../Lumen/LumenContent/images/walls/" + name + ".png", FileMode.Create);
                tar.SaveAsPng(stream2, widthMax * wldScale, heightMax * wldScale);
                stream2.Close();
            }

            ObjectType newType = new ObjectType();

            newType.Type = 0;
            newType.ImgScale = wldScale;
            newType.SpriteFilePath = "images/walls/" + name;
            newType.spriteWidth = pixelWidth;
            newType.spriteHeight = pixelHeight;
            newType.Framecount = 1;
            newType.spriteCenter = new Vector2(newType.ImgScale / 2f, newType.ImgScale / 4f);
            newType.CanSlide = false;
            newType.CanRotate = false;
            newType.IsSwitch = false;
            newType.IsPlug = false;
            newType.isOutlet = false;
            newType.CastsShadows = true;
            newType.Density = 0;
            newType.Friction = 1;
            newType.Restitution = 0;

            Vector2[] hull = {new Vector2(0,0), new Vector2(0, pixelHeight), new Vector2(pixelWidth, pixelHeight), new Vector2(pixelWidth, 0)};
            newType.lightConvexHull = hull;
            newType.physicsConvexHull = hull;

            newType.intensity = 0;
            newType.lightRadius = 0;
            newType.checkpoint = false;

            newType.invisibleHulls = new List<Vector2[]>();

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            Writer.WriteObjectType(newType, "Content/xml/walls/" + name + ".xml");
            if (File.Exists("../../../../../../Lumen/LumenContent/images/exit.png"))
            {
                Writer.WriteObjectType(newType, "../../../../../../Lumen/LumenContent/xml/walls/" + name + ".xml");
            }

            GameElement wall = new GameElement(p1.X + .5f, p1.Y + .25f,
                newType.ImgScale, pixelWidth, pixelHeight,
                newType.spriteCenter, tar, false, 0, 0,
                "xml/walls/" + name, 0, pwrsrc);

            return wall;
        }

        //Load Game Objects for a level
        //public void

    }
}
