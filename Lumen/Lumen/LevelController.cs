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
using Box2DX.Common;
using Box2DX.Collision;
using MyDataTypes;
using System.Diagnostics;
using System.Xml;
using System.IO;

namespace Lumen{
    /// <summary>
    /// This is the main controller for the game
    /// </summary>
    public class LevelController : Microsoft.Xna.Framework.Game{
        public const int SCREEN_WIDTH = 800;
        public const int SCREEN_HEIGHT = 600;

        ContentManager content;
        GirlContactListener listener;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        List<ObjectController> objectControllerList;
        private List<ObjectController> removeQueue;
        private List<ObjectController> addQueue;
        List<ObjectView> objectViewList;
        LightMap lightmap;
        LightMapController lightmapcontroller;
        LightMapView lightmapview;
        private Level level;
        private LevelView levelView;
        public AudioManager audioManager;

        private bool playingMenuMusic;
        private bool playingBackgroundMusic;

        private string levelname;

        private SpriteFont font;

        private Girl girl;
        private GirlController girlController;

        private bool waitingForSpaceRelease;

        private int checkPointID;

        private enum MenuChoice
        {
            
            Continue,
            Exit,
            New,
            NoSelect,
        }
        private MenuChoice menuchoice;
        private Texture2D[] menuImages;

        private enum GameState
        {
            Playing,
            MainMenu,
            Fading,
            Unfading
        }
        private GameState gamestate;

        private Texture2D fearMeter;
        private Texture2D fearBar;

        private const float CHECKPOINT_RADIUS = 5f;

        public const int DARKNESS = 100;
        public const int LIGHT = 200;
        public const float MAX_COURAGE_DECREASE = .01f * Girl.MAX_COURAGE;
        public const float COURAGE_GAIN = .1f * Girl.MAX_COURAGE;

        private int fade;
        private const int faderate = 20;
        private Texture2D whitepixel;

        public LevelController()
        {
            graphics = new GraphicsDeviceManager(this);
            content = new ContentManager(Services);
            content.RootDirectory = "Content";
            objectControllerList = new List<ObjectController>();
            removeQueue = new List<ObjectController>();
            addQueue = new List<ObjectController>();
            objectViewList = new List<ObjectView>();
            listener = new GirlContactListener();
            audioManager = new AudioManager();
            playingBackgroundMusic = false;
            playingMenuMusic = false;
            fade = 255;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //set up screen
            graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
            graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
            graphics.IsFullScreen = true;
            graphics.ApplyChanges();

            //level = new Level();
            //levelView = new LevelView(level);

            waitingForSpaceRelease = false;

            gamestate = GameState.MainMenu; IsMouseVisible = true;

            audioManager.Initialize();
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent(){
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            font = content.Load<SpriteFont>("SpriteFont1");

            audioManager.LoadContent(content);

            //level.LoadContent(content);
            LightMapController.LoadContent(GraphicsDevice);
            /*foreach (GameObject obj in level.GetGameObjectList())
            {
                if (obj is Girl)
                {
                    Girl girl1 = (Girl) obj;
                    girl.LoadContent(content);
                }
                obj.LoadContent(content);
            }*/

            menuImages = new Texture2D[(int)MenuChoice.NoSelect + 1];
            menuImages[(int)MenuChoice.New] = Reader.LoadImage("Content/images/mainmenu_selectnew.png", GraphicsDevice);
            menuImages[(int)MenuChoice.Continue] = Reader.LoadImage("Content/images/mainmenu_selectcontinue.png", GraphicsDevice);
            menuImages[(int)MenuChoice.Exit] = Reader.LoadImage("Content/images/mainmenu_selectexit.png", GraphicsDevice);
            menuImages[(int)MenuChoice.NoSelect] = Reader.LoadImage("Content/images/mainmenu_noselect.png", GraphicsDevice);

            fearMeter = Reader.LoadImage("Content/images/fearmeter.png", GraphicsDevice);
            fearBar = Reader.LoadImage("Content/images/fearbar.png", GraphicsDevice);

            whitepixel = new Texture2D(GraphicsDevice, 1, 1);
            Color[] data = { Color.White };
            whitepixel.SetData(data);

            //levelname = "seanslevel";
            //Restart();
        }

        private void Restart()
        {
            gamestate = GameState.Fading;
        }
        private void ContinueRestart() {
            
            SaveData dat = Reader.ParseSaveData("Content/savedata.xml");
            levelname = dat.Level;
            if (dat.Checkpoint)
            {
                girl = LoadLevel();
                checkPointID = dat.ExitID;
            }
            else
            {
                girl = LoadLevel(dat.ExitID);
            }
            gamestate = GameState.Unfading;
        }

        

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent(){
            content.Unload();
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime){
            // Allows the game to exit
            KeyboardState keyboard = Keyboard.GetState();
            /*Girl girl = null;
            foreach(GameObject obj in level.GetGameObjectList()) {
                if(obj is Girl) {
                    girl = (Girl) obj;
                    break;
                }
            }*/
            //Console.WriteLine(girl.Possession);

            if (keyboard.IsKeyDown(Keys.Escape))
                this.Exit();

            switch(gamestate) {

                case GameState.Fading:
                    if (fade <= 0)
                    {
                        ContinueRestart();
                    }
                    else
                    {
                        fade -= faderate;
                        if (fade < 0) fade = 0;
                    }
                    break;
                case GameState.Unfading:
                    if (fade >= 255)
                    {
                        gamestate = GameState.Playing;
                    }
                    else
                    {
                        fade += faderate;
                        if (fade > 255) fade = 255;
                    }
                    break;
                case GameState.MainMenu:
                    if (!playingMenuMusic)
                    {
                        audioManager.Play(AudioManager.MusicSelection.MenuScreen);
                        playingMenuMusic = true;
                    }

                    MouseState ms = Mouse.GetState();

                    float widthRescale = 800 / SCREEN_WIDTH;
                    float heightRescale = 600 / SCREEN_HEIGHT;

                    Vector2 tlNew = new Vector2(216, 48);
                    Vector2 brNew = new Vector2(407, 133);

                    Vector2 tlContinue = new Vector2(134, 246);
                    Vector2 brContinue = new Vector2(329, 329);

                    Vector2 tlExit = new Vector2(119, 445);
                    Vector2 brExit = new Vector2(312, 526);

                    float x = ms.X * widthRescale;
                    float y = ms.Y * heightRescale;

                    if(tlNew.X < x && x < brNew.X && tlNew.Y < y && y < brNew.Y) {
                        menuchoice = MenuChoice.New;
                    }
                    else if(tlContinue.X < x && x < brContinue.X && tlContinue.Y < y && y < brContinue.Y) {
                        menuchoice = MenuChoice.Continue;
                    }
                    else if(tlExit.X < x && x < brExit.X && tlExit.Y < y && y < brExit.Y) {
                        menuchoice = MenuChoice.Exit;
                    }
                    else {
                        menuchoice = MenuChoice.NoSelect;
                    }

                    if (ms.LeftButton == ButtonState.Pressed)
                    {
                        switch (menuchoice)
                        {
                            case MenuChoice.New:
                                gamestate = GameState.Playing; IsMouseVisible = false;
                                ClearSaveData();
                                fade = 0;
                                ContinueRestart();
                                break;
                            case MenuChoice.Continue:
                                gamestate = GameState.Playing; IsMouseVisible = false;
                                fade = 0;
                                ContinueRestart();
                                break;
                            case MenuChoice.Exit:
                                this.Exit();
                                break;
                            case MenuChoice.NoSelect:
                                break;
                        }
                    }
                    
                    break;

                case GameState.Playing:
                    if (playingMenuMusic)
                    {
                        audioManager.Stop();
                        playingMenuMusic = false;
                    }
                    if (!playingBackgroundMusic)
                    {
                        audioManager.Play(AudioManager.MusicSelection.CreepyOverworld);
                        playingBackgroundMusic = true;
                    }
                    if (girl.Action != Girl.GIRL_STATE.SWITCHING_DOWN && girl.Action != Girl.GIRL_STATE.SWITCHING_UP &&
                        girl.Action != Girl.GIRL_STATE.PICKING_UP && girl.Action != Girl.GIRL_STATE.PUTTING_DOWN &&
                        girl.Action != Girl.GIRL_STATE.PLUGGING && girl.Action != Girl.GIRL_STATE.UNPLUGGING)
                    {
                        if (waitingForSpaceRelease && keyboard.IsKeyUp(Keys.Space))
                        {
                            waitingForSpaceRelease = false;
                        }

                        if (girl.Possession == null && !waitingForSpaceRelease && keyboard.IsKeyDown(Keys.Space))
                        {
                            girl.Action = Girl.GIRL_STATE.HOLDING;
                            waitingForSpaceRelease = true;
                        }
                        else if (girl.Possession != null && !waitingForSpaceRelease && keyboard.IsKeyDown(Keys.Space))
                        {
                            girl.Action = Girl.GIRL_STATE.STANDING;
                            waitingForSpaceRelease = true;
                        }
                        else if (girl.Possession == null)
                        {
                            girl.Action = Girl.GIRL_STATE.STANDING;
                            if (keyboard.IsKeyDown(Keys.Up) || keyboard.IsKeyDown(Keys.Down) || keyboard.IsKeyDown(Keys.Left) || keyboard.IsKeyDown(Keys.Right))
                            {
                                girl.Facing = new Vector2(0, 0);
                            }
                            if (keyboard.IsKeyDown(Keys.Up))
                            {
                                //girl.Facing = new Vector2(0, 0);
                                girl.Facing += new Vector2(0, -1);
                                girl.Action = Girl.GIRL_STATE.WALKING;
                            }
                            if (keyboard.IsKeyDown(Keys.Down))
                            {
                                //girl.Facing = new Vector2(0, 0);
                                girl.Facing += new Vector2(0, +1);
                                girl.Action = Girl.GIRL_STATE.WALKING;
                            }
                            if (keyboard.IsKeyDown(Keys.Right))
                            {
                                //girl.Facing = new Vector2(0, 0);
                                girl.Facing += new Vector2(+1, 0);
                                girl.Action = Girl.GIRL_STATE.WALKING;
                            }
                            if (keyboard.IsKeyDown(Keys.Left))
                            {
                                //girl.Facing = new Vector2(0, 0);
                                girl.Facing += new Vector2(-1, 0);
                                girl.Action = Girl.GIRL_STATE.WALKING;
                            }
                        }
                        else
                        {
                            if ((keyboard.IsKeyDown(Keys.LeftControl) || keyboard.IsKeyDown(Keys.RightControl)) && girl.Possession.CanRotate)
                            {
                                if (keyboard.IsKeyDown(Keys.Left))
                                {
                                    girl.Action = Girl.GIRL_STATE.ROTATING_CW;
                                }
                                else if (keyboard.IsKeyDown(Keys.Right))
                                {
                                    girl.Action = Girl.GIRL_STATE.ROTATING_CCW;
                                }
                                else
                                {
                                    girl.Action = Girl.GIRL_STATE.HOLDING;
                                }
                            }
                            else if (keyboard.IsKeyDown(Keys.Up) && girl.Possession.CanSlide)
                            {
                                if (girl.Facing.Y == -1) girl.Action = Girl.GIRL_STATE.PUSHING;
                                else if (girl.Facing.Y == +1) girl.Action = Girl.GIRL_STATE.PULLING;
                                else if (girl.Facing.X == -1) girl.Action = Girl.GIRL_STATE.STRAFE_RIGHT;
                                else if (girl.Facing.X == +1) girl.Action = Girl.GIRL_STATE.STRAFE_LEFT;
                            }
                            else if (keyboard.IsKeyDown(Keys.Down) && girl.Possession.CanSlide)
                            {
                                if (girl.Facing.Y == -1) girl.Action = Girl.GIRL_STATE.PULLING;
                                else if (girl.Facing.Y == +1) girl.Action = Girl.GIRL_STATE.PUSHING;
                                else if (girl.Facing.X == -1) girl.Action = Girl.GIRL_STATE.STRAFE_LEFT;
                                else if (girl.Facing.X == +1) girl.Action = Girl.GIRL_STATE.STRAFE_RIGHT;
                            }
                            else if (keyboard.IsKeyDown(Keys.Right) && girl.Possession.CanSlide)
                            {
                                if (girl.Facing.X == -1) girl.Action = Girl.GIRL_STATE.PULLING;
                                else if (girl.Facing.X == +1) girl.Action = Girl.GIRL_STATE.PUSHING;
                                else if (girl.Facing.Y == -1) girl.Action = Girl.GIRL_STATE.STRAFE_RIGHT;
                                else if (girl.Facing.Y == +1) girl.Action = Girl.GIRL_STATE.STRAFE_LEFT;
                            }
                            else if (keyboard.IsKeyDown(Keys.Left) && girl.Possession.CanSlide)
                            {
                                if (girl.Facing.X == -1) girl.Action = Girl.GIRL_STATE.PUSHING;
                                else if (girl.Facing.X == +1) girl.Action = Girl.GIRL_STATE.PULLING;
                                else if (girl.Facing.Y == -1) girl.Action = Girl.GIRL_STATE.STRAFE_LEFT;
                                else if (girl.Facing.Y == +1) girl.Action = Girl.GIRL_STATE.STRAFE_RIGHT;
                            }
                            else
                            {
                                girl.Action = Girl.GIRL_STATE.HOLDING;
                            }
                        }
                    }
                    foreach (ObjectController controller in objectControllerList)
                    {
                        controller.Update(gameTime);
                    }
                    foreach (ObjectController controller in removeQueue)
                    {
                        objectControllerList.Remove(controller);
                    }
                    removeQueue.Clear();
                    foreach (ObjectController controller in addQueue)
                    {
                        objectControllerList.Add(controller);
                    }
                    addQueue.Clear();

                    level.ScreenPosition = new Vector2(girl.Position.X - SCREEN_WIDTH / 2 / GameObject.worldScale,
                                                       girl.Position.Y - SCREEN_HEIGHT / 2 / GameObject.worldScale);




                    float dt = 1f / 60f;
                    int iterations = 10;
                    level.World.Step(dt, iterations, iterations);

                    UpdateCourage(girl);

                    foreach (GameObject obj in level.GetGameObjectList())
                    {
                        if (obj.PowerSourceID != checkPointID && obj is Light)
                        {
                            Light l = (Light)obj;
                            if (l.IsCheckpoint && (l.Position - girl.Position).Length() < CHECKPOINT_RADIUS)
                            {//l.LightRadius) {
                                checkPointID = l.PowerSourceID;

                                SaveData dat = new SaveData();
                                dat.Level = levelname;
                                dat.Checkpoint = true;
                                dat.ExitID = checkPointID;
                                Writer.WriteSaveData(dat, "Content/savedata.xml");
                                if (File.Exists("../../../../Lumen/LevelController.cs"))
                                    Writer.WriteSaveData(dat, "../../../../LumenContent/savedata.xml");

                                SaveLevel();
                            }
                        }
                    }

                    foreach (Exit ex in level.GetExitList())
                    {
                        if (ex.Contains(girl.Position))
                        {
                            if (girl.CarriedPlug != null)
                            {
                                girlController.putDownPlug();
                            }
                            else
                            {
                                level.DeleteFromObjectList(girl);
                                SaveLevel();

                                SaveData dat = new SaveData();
                                dat.Level = ex.ExitDestination;
                                dat.Checkpoint = false;
                                dat.ExitID = ex.DestinationID;
                                Writer.WriteSaveData(dat, "Content/savedata.xml");
                                if (File.Exists("../../../../Lumen/LevelController.cs"))
                                    Writer.WriteSaveData(dat, "../../../../LumenContent/savedata.xml");

                                levelname = ex.ExitDestination;
                                //girl = LoadLevel(ex.DestinationID);
                                Restart();
                            }
                        }
                    }

                    if (girl.Courage == 0)
                    {
                        Restart();
                    }

                    break;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Draws the objects in the game.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.Clear(Color.CornflowerBlue);

            switch(gamestate) {
                case GameState.MainMenu:
                    spriteBatch.Begin();
                    spriteBatch.Draw(menuImages[(int)menuchoice], new Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT), Color.White);
                    spriteBatch.End();
                    break;
                case GameState.Fading:
                case GameState.Unfading:
                case GameState.Playing:
                    lightmapcontroller.UpdateLightMap(spriteBatch, level);

                    spriteBatch.Begin();
                    levelView.Draw(spriteBatch);

            
                    spriteBatch.End();

                    lightmapview.Draw(spriteBatch);

                    spriteBatch.Begin();
                    foreach (ObjectView view in objectViewList)
                    {
                        view.DrawWires(spriteBatch);
                    }
                    objectViewList.Sort(ObjectView.ComparePositions);
                    foreach (ObjectView view in objectViewList)
                    {
                        view.Draw(spriteBatch, lightmap);
                    }
            
                    /*foreach (ObjectView view in objectViewList)
                    {
                        if (view is LightView)
                            ((LightView)view).DrawIfOn(spriteBatch, gameTime);
                        else if (view is GirlView)
                            view.Draw(spriteBatch, gameTime);
                    }*/
                    spriteBatch.End();

                    /*Girl girl = null;

                    foreach (GameObject obj in level.GetGameObjectList())
                    {
                        if (obj is Girl)
                        {
                            girl = (Girl)obj;
                            break;
                        }
                    }*/
                    spriteBatch.Begin();
                    int meterWidth = 1637; int meterHeight = 373;
                    float scale = (float) SCREEN_WIDTH / meterWidth;
                    int x1 = 257; int y1 = 223;
                    int x2 = 1317; int y2 = 295;
                    int yTop = SCREEN_HEIGHT - (int)(meterHeight * scale);
                    
                    spriteBatch.Draw(fearMeter, new Vector2(0, SCREEN_HEIGHT), null, Color.White, 0f, new Vector2(0, meterHeight), scale, SpriteEffects.None, 0f);
                    spriteBatch.Draw(fearBar, new Rectangle((int)(x1 * scale), (int)(y1 * scale + yTop), (int)((x2 - x1) * scale * girl.Courage), (int)((y2 - y1) * scale)), Color.White);
                    //spriteBatch.DrawString(font, "Time: "+gameTime.TotalGameTime.TotalMilliseconds, new Vector2(0, 0), Color.White);
                    //spriteBatch.DrawString(font, "Courage: " + (int)(girl.Courage), new Vector2(400, 0), Color.White);
                    spriteBatch.End();

                    if (gamestate == GameState.Fading || gamestate == GameState.Unfading)
                    {
                        BlendState mult = new BlendState();
                        mult.ColorSourceBlend = Blend.Zero; // Blend.DestinationColor;//
                        mult.AlphaSourceBlend = Blend.Zero; // Blend.DestinationAlpha;//
                        mult.ColorDestinationBlend = Blend.SourceColor;
                        mult.AlphaDestinationBlend = Blend.SourceAlpha;
                        spriteBatch.Begin(SpriteSortMode.BackToFront, mult);
                        Color f = new Color(fade, fade, fade);
                        spriteBatch.Draw(whitepixel, new Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT), f);
                        spriteBatch.End();
                    }
                    break;
            }

 	        base.Draw(gameTime);
        }

        /*/// <summary>
        /// Adds a controller to the list of controllers run in this level.
        /// </summary>
        /// <param name="controller">The controller to be added.</param>
        public void AddToObjectList(ObjectController controller){
            objectControllerList.Add(controller);
        }*/

        /*/// <summary>
        /// Deletes a controller from the list of controllers run in this level.
        /// </summary>
        /// <param name="controller">The controller to be deleted.</param>
        public void RemoveFromObjectList(ObjectController controller){
            objectControllerList.Remove(controller);
        }*/

        /*/// <summary>
        /// Returns the list of controllers runs in this level.
        /// </summary>
        /// <returns>Returns the list of ObjectControllers run in this level</returns>
        public List<ObjectController> GetObjectList(){
            return objectControllerList;
        }*/

        /// <summary>
        /// Adds a controller to the list of controllers run in this level.
        /// </summary>
        /// <param name="controller">The controller to be added.</param>
        public void AddToViewList(GameObject obj){
            objectViewList.Add(new ObjectView(obj));
        }

        /// <summary>
        /// Deletes a controller from the list of controllers run in this level.
        /// </summary>
        /// <param name="controller">The controller to be deleted.</param>
        public void RemoveFromViewList(GameObject obj){
            for (int n=0; n < objectViewList.Count; n++)
            {
                if (objectViewList[n].ViewObject == obj)
                {
                    objectViewList.RemoveAt(n);
                    return;
                }
            }
        }

        /// <summary>
        /// Adds a controller to the list of controllers run in this level.
        /// </summary>
        /// <param name="controller">The controller to be added.</param>
        public void AddToControlList(GameObject obj)
        {
            addQueue.Add(new ObjectController(obj));
        }

        /// <summary>
        /// Deletes a controller from the list of controllers run in this level.
        /// </summary>
        /// <param name="controller">The controller to be deleted.</param>
        public void RemoveFromControlList(GameObject obj)
        {
            for (int n = 0; n < objectControllerList.Count; n++)
            {
                if (objectControllerList[n].ControlledObject == obj)
                {
                    removeQueue.Add(objectControllerList[n]);
                    return;
                }
            }
        }

        private void UpdateCourage(Girl girl)
        {
            float courage = girl.Courage;
            Vector2 girlPosition = girl.GetScreenPosition();
            int lightAtPos = lightmap.GetLightValue((int)girlPosition.X, (int)girlPosition.Y);
            //Console.WriteLine("LightAtPos: " + (int)girlPosition.X + ", " + (int)girlPosition.Y + ", " + lightAtPos);
            if (lightAtPos < DARKNESS)
            {
                bool nextToLight = false;
                foreach (GameObject obj in girl.Adjacent)
                {
                    if (obj is Light)
                    {
                        Light l = (Light)obj;
                        if (l.IsOn)
                        {
                            nextToLight = true;
                            break;
                        }
                    }
                }
                if(!nextToLight)
                    girl.Courage -= MAX_COURAGE_DECREASE * (1f - (float) lightAtPos / DARKNESS);
            }
            else if (lightAtPos < LIGHT)
            {
                float localMaxCourage = Girl.MAX_COURAGE * ((float) lightAtPos / LIGHT);
                if (girl.Courage < localMaxCourage)
                {
                    girl.Courage += COURAGE_GAIN;
                    if (girl.Courage > localMaxCourage)
                        girl.Courage = localMaxCourage;
                }
            }
            else
            {
                girl.Courage += COURAGE_GAIN;
            }
            //Console.WriteLine("Courage: " + (int)girl.Courage);
        }

        public Girl LoadLevel(int exitID)
        {
            checkPointID = -1;

            LoadLevel();
            Exit ex = null;
            foreach (Exit e in level.GetExitList()) {
                if(e.ID == exitID) {
                    ex = e;
                    break;
                }
            }
            Debug.Assert(ex != null, "Exit ID "+exitID+" not present in loading level");

            ObjectInstance oi = new ObjectInstance();
            oi.XMLDataFileLocation = "xml/Girl"; 
            oi.InitialPosition = ex.SpawnPosition();
            oi.Orientation = 0;
            oi.On = false;
            int[] psrc = {-1};
            oi.PowerSourceID = psrc;

            Girl g = new Girl(oi, Reader.ParseObjectType("Content/xml/Girl.xml"), GraphicsDevice, level);
            girlController = new GirlController(g, level, this);

            level.AddToObjectList(g);
            objectControllerList.Add(girlController);
            objectViewList.Add(new GirlView(g));

            return g;
        }

        public Girl LoadLevel()
        {
            level = new Level(levelname, GraphicsDevice);
            levelView = new LevelView(level);

            objectControllerList.Clear();
            objectViewList.Clear();

            Girl g = null;

            foreach (GameObject obj in level.GetGameObjectList())
            {
                if (obj is Girl)
                {
                    girlController = new GirlController((Girl)obj, level, this);
                    objectControllerList.Add(girlController);
                    objectViewList.Add(new GirlView((Girl)obj));
                    g = (Girl)obj;
                }
                else if (obj is Light)
                {
                    objectControllerList.Add(new LightController((Light)obj));
                    objectViewList.Add(new LightView((Light)obj));
                }
                else //object
                {
                    objectControllerList.Add(new ObjectController(obj));
                    objectViewList.Add(new ObjectView(obj));
                }
            }

            level.World.SetContactListener(listener);

            //create light map for shadows
            lightmap = new LightMap(SCREEN_WIDTH, SCREEN_HEIGHT);
            lightmapcontroller = new LightMapController(GraphicsDevice, lightmap, level, graphics);
            lightmapview = new LightMapView(lightmap);

            return g;
        }

        public void SaveLevel()
        {
            LevelDef lvlDef = level.Save(girl.Courage);

            List<string> exitLocs = new List<string>();
            List<float> exitPoints = new List<float>();

            List<ObjectInstance> objectsToSerialize = new List<ObjectInstance>();
            List<ExitInstance> exitsToSerialize = new List<ExitInstance>();

            foreach (GameObject obj in level.GetGameObjectList())
            {
                ObjectInstance oi = obj.Compress();
                objectsToSerialize.Add(oi);
            }
            objectsToSerialize.Sort(CompareBySourceListSize);

            foreach (Exit ex in level.GetExitList())
            {
                ExitInstance ei = ex.Compress();
                exitsToSerialize.Add(ei);
            }

            ObjectInstance[] objects = objectsToSerialize.ToArray();
            ExitInstance[] exits = exitsToSerialize.ToArray();

            Writer.WriteLevelDef(lvlDef, "Content/xml/" + levelname + "-sav.xml");
            Writer.WriteObjectInstances(objects, "Content/xml/" + levelname + "Objects-sav.xml");
            Writer.WriteExitInstances(exits, "Content/xml/" + levelname + "Exits-sav.xml");

            /*XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            XmlWriter writer = XmlWriter.Create("Content/xml/" + level.FilePath + "-sav.xml", settings);
            IntermediateSerializer.Serialize(writer, lvlDef, null);
            writer.Close();

            XmlWriter writer2 = XmlWriter.Create("Content/xml/" + level.FilePath + "Objects-sav.xml", settings);
            IntermediateSerializer.Serialize(writer2, objects, null);
            writer2.Close();

            XmlWriter writer3 = XmlWriter.Create("Content/xml/" + level.FilePath + "Exits-sav.xml", settings);
            IntermediateSerializer.Serialize(writer3, exits, null);
            writer3.Close();*/
        }

        private void ClearSaveData()
        {
            string[] xmlFiles = Directory.GetFiles("Content/xml");
            foreach (string s in xmlFiles)
            {
                if (s.Contains("-sav"))
                    File.Delete(s);
            }

            SaveData dat = new SaveData();
            dat.Level = "tutorial";
            dat.Checkpoint = true;
            dat.ExitID = 0;
            Writer.WriteSaveData(dat, "Content/savedata.xml");
            if (File.Exists("../../../../Lumen/LevelController.cs"))
                Writer.WriteSaveData(dat, "../../../../LumenContent/savedata.xml");
        }

        /// <summary>
        /// used to sort object instances so power sources occur earlier in the xml file.
        /// </summary>
        public int CompareBySourceListSize(ObjectInstance dis, ObjectInstance dat)
        {
            return dis.PowerSourceID.Length - dat.PowerSourceID.Length;
        }
    }
}
