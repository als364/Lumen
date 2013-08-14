using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using MyDataTypes;
using System.Diagnostics;

namespace Lumen{
    //enum Object_State { Walking, Idle, Grabbing};

    public class Girl : GameObject{

        //private Object_State objectState = Object_State.Idle;

        //list of objects the girl is adjacent to, updated by a GirlContactListener
        private List<GameObject> adjacent;

        //whatever you are grabbing, just the floodlight for now
        protected GameObject possession;

        

        //size of rectangle of girl on sprite seet
        private const int sprite_WIDTH = 280;

        //where you are
        protected Vector2 position;

        // Current image for this object
        protected Texture2D objectImage;

        //which direction you are facing, should be unit vector in one of 4 cardinal directions
        protected Vector2 orientation;

        //pixels per frame
        public const float SPEED = 10f;

        //parameter is part of logic to determine if girl can reach whatever she is grabbing
        public const float GRAB_DIST = 15f;

        //keep track of courage
        private float courage;
        public const float MAX_COURAGE = 1f;

        //keep track of where we are in animatoin
        private const int framesPerAnimation = 4;
        protected int nextAnimationFrame;

        //Stores what step out of how many steps the animation is currently in.
        protected int animationCycleState;
        public const int AnimationStates = 8;

        //a matrix containing all animations
        //first # is direction / orientation
        //second # refers to current action
        private Texture2D[] animations;

        // What object is doing at the time
        // Lights and Girls move by "Walking", for example
        public enum GIRL_STATE
        {
            STANDING,
            WALKING,
            HOLDING,
            ROTATING_CW,
            ROTATING_CCW,
            PULLING,
            PUSHING,
            STRAFE_RIGHT,
            STRAFE_LEFT,
            SWITCHING_DOWN,
            SWITCHING_UP,
            PICKING_UP,
            PUTTING_DOWN,
            PLUGGING,
            UNPLUGGING,
            UNUSED
        };

        protected GIRL_STATE action;

        public GIRL_STATE Action
        {
            get{
                return action;
            }
            set{
                action = value;
            }
        }

        public GameObject Possession
        {
            get
            {
                return possession;
            }
            set
            {
                possession = value;
            }
        }

        

        //Constructors of various capabilities.
        public Girl(ObjectInstance oi, ObjectType objdef, GraphicsDevice device, Level lvl) : base(oi, objdef, device, lvl) { Init(); LoadContent(device); }

        /*public Girl(Level lvl) : base(new Vector2(0), "xml/girl", lvl) { Init(); }

        public Girl(Vector2 pos, Level lvl) : base(pos, "xml/girl", lvl) { Init(); }

        public Girl(Vector2 pos, String textureFile, Level lvl) :base(pos, textureFile, lvl){ Init(); }*/

        /// <summary>
        /// Initializes most of logic.
        /// Separated from constructor so it can be called when game is restarted
        /// </summary>
        public void Init()
        {
            adjacent = new List<GameObject>();
            possession = null;
            animationCycleState = 0;
            //animationCycleState[1] = 1;
            courage = MAX_COURAGE;
            nextAnimationFrame = 0;
            state = OBJECT_STATE.IDLE;
            carriedPlug = null;
        }



        //more getters and setters
        /*public Vector2 Orientation{
            get
            {
                return orientation;
            }
            set
            {
                orientation = value;
            }
        }*/

        

        /*public Vector2 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }*/

        public Texture2D[] Animations
        {
            get
            {
                return animations;
            }
        }

        /*public int SPRITE_WIDTH
        {
            get
            {
                return sprite_WIDTH;
            }
        }*/

        /*public Texture2D ObjectImage
        {
            get
            {
                return objectImage;
            }
            set
            {
                objectImage = value;
            }
        }*/

        public int FramesPerAnimation
        {
            get
            {
                return framesPerAnimation;
            }
        }

        public int NextAnimationFrame
        {
            get
            {
                return nextAnimationFrame;
            }
            set
            {
                nextAnimationFrame = value;
            }
        }

        public float Courage{
            get{
                return courage;
            }
            set{
                //don't increase courage more than maximum or less than zero
                if (value > MAX_COURAGE)
                    courage = MAX_COURAGE;
                else if (value < 0)
                    courage = 0;
                else
                    courage = value;
            }
        }

        public int AnimationCycleState
        {
            get
            {
                return animationCycleState;
            }
            set
            {
                animationCycleState = value;
            }
        }

        /*public GameObject.OBJECT_STATE getObjectState(){
            return state;
        }

        public void setObjectState(GameObject.OBJECT_STATE newState){
            state = newState;
        }*/

        public List<GameObject> Adjacent
        {
            get
            {
                return adjacent;
            }
            set
            {
                adjacent = value;
            }
        }

        public void SetSpriteSheet(Texture2D value) 
        {
            spriteSheet = value;
        }

        private void LoadContent(GraphicsDevice device){
            animations = new Texture2D[(int)GIRL_STATE.UNUSED];
            //string dir;

            //Console.WriteLine("got here");

            //back
            //dir = "back";
            animations[(int)GIRL_STATE.HOLDING] = Reader.LoadImage("Content/images/girlholding.png", device);
            animations[(int)GIRL_STATE.PUSHING] = Reader.LoadImage("Content/images/girlpushing.png", device);
            animations[(int)GIRL_STATE.PULLING] = Reader.LoadImage("Content/images/girlpulling.png", device);
            animations[(int)GIRL_STATE.ROTATING_CCW] = Reader.LoadImage("Content/images/girlrotating.png", device);
            animations[(int)GIRL_STATE.ROTATING_CW] = Reader.LoadImage("Content/images/girlrotating.png", device);
            animations[(int)GIRL_STATE.STANDING] = Reader.LoadImage("Content/images/girlstanding.png", device);
            animations[(int)GIRL_STATE.WALKING] = Reader.LoadImage("Content/images/girlwalking.png", device);
            animations[(int)GIRL_STATE.STRAFE_RIGHT] = Reader.LoadImage("Content/images/girlpushing.png", device);
            animations[(int)GIRL_STATE.STRAFE_LEFT] = Reader.LoadImage("Content/images/girlpushing.png", device);
            animations[(int)GIRL_STATE.SWITCHING_DOWN] = Reader.LoadImage("Content/images/girloffgrid.png", device);
            animations[(int)GIRL_STATE.SWITCHING_UP] = Reader.LoadImage("Content/images/girlongrid.png", device);
            animations[(int)GIRL_STATE.PLUGGING] = Reader.LoadImage("Content/images/girlplugunplug.png", device);
            animations[(int)GIRL_STATE.UNPLUGGING] = Reader.LoadImage("Content/images/girlplugunplug.png", device);
            animations[(int)GIRL_STATE.PICKING_UP] = Reader.LoadImage("Content/images/girl pickup.png", device);
            animations[(int)GIRL_STATE.PUTTING_DOWN] = Reader.LoadImage("Content/images/girl pickup.png", device);


            //animations[0, (int)GIRL_STATE.PULLING] = content.Load<Texture2D>("images\\push" + dir);

            /*//fwd
            dir = "forward";
            animations[1, (int)GIRL_STATE.HOLDING] = content.Load<Texture2D>("images\\grab" + dir);
            animations[1, (int)GIRL_STATE.PUSHING] = content.Load<Texture2D>("images\\push" + dir);
            animations[1, (int)GIRL_STATE.PULLING] = content.Load<Texture2D>("images\\pull" + dir);
            animations[1, (int)GIRL_STATE.ROTATING_CCW] = content.Load<Texture2D>("images\\rotate" + dir);
            animations[1, (int)GIRL_STATE.ROTATING_CW] = content.Load<Texture2D>("images\\rotate" + dir);
            animations[1, (int)GIRL_STATE.STANDING] = content.Load<Texture2D>("images\\stand" + dir);
            animations[1, (int)GIRL_STATE.WALKING] = content.Load<Texture2D>("images\\walk" + dir);
            //animations[1, (int)GIRL_STATE.PULLING] = content.Load<Texture2D>("images\\push" + dir);

            //right
            dir = "right";
            animations[2, (int)GIRL_STATE.HOLDING] = content.Load<Texture2D>("images\\grab" + dir);
            animations[2, (int)GIRL_STATE.PUSHING] = content.Load<Texture2D>("images\\push" + dir);
            animations[2, (int)GIRL_STATE.PULLING] = content.Load<Texture2D>("images\\pull" + dir);
            animations[2, (int)GIRL_STATE.ROTATING_CCW] = content.Load<Texture2D>("images\\rotate" + dir);
            animations[2, (int)GIRL_STATE.ROTATING_CW] = content.Load<Texture2D>("images\\rotate" + dir);
            animations[2, (int)GIRL_STATE.STANDING] = content.Load<Texture2D>("images\\stand" + dir);
            animations[2, (int)GIRL_STATE.WALKING] = content.Load<Texture2D>("images\\walk" + dir);
            //animations[2, (int)GIRL_STATE.PULLING] = content.Load<Texture2D>("images\\push" + dir);

            //left
            dir = "left";
            animations[3, (int)GIRL_STATE.HOLDING] = content.Load<Texture2D>("images\\grab" + dir);
            animations[3, (int)GIRL_STATE.PUSHING] = content.Load<Texture2D>("images\\push" + dir);
            animations[3, (int)GIRL_STATE.PULLING] = content.Load<Texture2D>("images\\pull" + dir);
            animations[3, (int)GIRL_STATE.ROTATING_CCW] = content.Load<Texture2D>("images\\rotate" + dir);
            animations[3, (int)GIRL_STATE.ROTATING_CW] = content.Load<Texture2D>("images\\rotate" + dir);
            animations[3, (int)GIRL_STATE.STANDING] = content.Load<Texture2D>("images\\stand" + dir);
            animations[3, (int)GIRL_STATE.WALKING] = content.Load<Texture2D>("images\\walk" + dir);
            //animations[3, (int)GIRL_STATE.PULLING] = content.Load<Texture2D>("images\\push" + dir);
            */
            //base.LoadContent(content);

            /*Vector2[] convexHullTemp = new Vector2[11];
            convexHullTemp[0] = new Vector2(954, 187);
            convexHullTemp[1] = new Vector2(945, 191);
            convexHullTemp[2] = new Vector2(937, 198);
            convexHullTemp[3] = new Vector2(947, 208);
            convexHullTemp[4] = new Vector2(969, 215);
            convexHullTemp[5] = new Vector2(998, 217);
            convexHullTemp[6] = new Vector2(1017, 208);
            convexHullTemp[7] = new Vector2(1024, 195);
            convexHullTemp[8] = new Vector2(1007, 192);
            convexHullTemp[9] = new Vector2(981, 188);
            convexHullTemp[10] = new Vector2(954, 187);

            this.lightConvexHull = convexHullTemp;*/
        }
    }
}
