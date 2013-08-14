using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box2DX;
using Box2DX.Collision;
using Box2DX.Common;
using Box2DX.Dynamics;
using Box2DX.Dynamics.Controllers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;
using MyDataTypes;

namespace Lumen{
	public class GameObject{ 
       
        //position, velocity, shape, orientation of the game object. orientation is double in radians
        protected Shape shape;
        protected Body body;
        //protected double orientation;

        //deprecated
        protected float width, height;

        public int spriteWidth;
        public int spriteHeight;

        //used to block light, contains points relative to center of object
        protected Vector2[] lightConvexHull;
        protected Vector2[] physicsConvexHull;
        public Vector2 spriteCenter;

        //textureFilePath, spriteFilePath of the game object.
        //private string textureFilePath;
        protected string spriteFilePath;

        /*public string SpriteFilePath
        {
            get
            {
                return spriteFilePath;
            }
            set
            {
                spriteFilePath = value;
            }
        }*/

        //used for saving and loading
        protected string xmlFilePath;
        protected int[] pwrsrc;

        protected Texture2D spriteSheet;

        //a whole bunch of data-driven design bools (mostly), that define how the object works
        protected bool canSlide, canRotate;
        protected bool isSwitch;
        protected int powerSourceID;
        protected List<GameObject> connectedLights;
        protected bool castsShadows;

        protected bool isPlug;
        protected List<Vector2> cordPoints;
        public const float CORD_LENGTH = .1f;
        public int wireLeft;
        List<Body> cordBodies;
        public Texture2D cordPic;

        protected bool on;
        public bool IsOn { get { return on; } set { on = value; } }

        //scales object from original sprite size
        //public float SCALE;
        public const float worldScale = 40f;

        protected Level level;

        protected Vector2 facing;

        public Vector2 Facing
        {
            get
            {
                return facing;
            }
            set
            {
                facing = value;
            }
        }

        protected GameObject carriedPlug;

        public GameObject CarriedPlug
        {
            get
            {
                return carriedPlug;
            }
            set
            {
                Debug.Assert(value == null || value.IsPlug, "trying to carry a gameobject that is not a plug");
                carriedPlug = value;
            }
        }

        // What object is doing at the time
        public enum OBJECT_STATE{
            IDLE = 0,
            MOVING = 1,
            HOLDING = 2,
            ROTATING_CW = 3,
            ROTATING_CCW = 4,
            PULLING = 5,
            PUSHING = 6,
        };

        protected OBJECT_STATE state;

        //Getters and setters for position
        public Vector2 Position
        {
            get
            {
                Vec2 pos = body.GetWorldCenter();
                return new Vector2(pos.X, pos.Y);
            }
        }

        protected bool waitingOnRelease;

        public bool WaitingOnRelease
        {
            get
            {
                return waitingOnRelease;
            }
            set
            {
                waitingOnRelease = value;
            }
        }

        protected Vector2 framesize;

        public Vector2 Framesize
        {
            get
            {
                return framesize;
            }
            /*set
            {
                framesize = value;
            }*/
        }

        protected int framecount;

        public int Framecount
        {
            get
            {
                return framecount;
            }
        }

        //Getters and setters for velocity in terms of pixels/frame
        public Vector2 Velocity
        {
            get
            {
                Vec2 vel = body.GetLinearVelocity();
                return new Vector2(vel.X, vel.Y);
            }
        }

        //gets the shape of the GameObject
        /*public Shape Shape
        {
            get
            {
                return shape;
            }
            set
            {
                shape = value;
            }
        }*/

        //gets the body of the GameObject
        public Body Body
        {
            get
            {
                return body;
            }
            set
            {
                body = value;
            }
        }

        public bool CanSlide
        {
            get
            {
                return canSlide;
            }
            /*set
            {
                canSlide = value;
            }*/
        }

        //how many pixels in an img is 1 meter.
        protected float imgScale;

        //gets the body of the GameObject
        public float ImgScale
        {
            get
            {
                return imgScale;
            }
            /*set
            {
                imgScale = value;
            }*/
        }

        public bool CanRotate
        {
            get
            {
                return canRotate;
            }
            /*set
            {
                canRotate = value;
            }*/
        }

        /*protected bool isLight;

        public bool IsLight
        {
            get
            {
                return isLight;
            }
            set
            {
                isLight = value;
            }
        }*/

        public bool IsSwitch
        {
            get
            {
                return isSwitch;
            }
            /*set
            {
                isSwitch = value;
            }*/
        }

        

        public bool IsPlug
        {
            get
            {
                return isPlug;
            }
            /*set
            {
                isPlug = value;
            }*/
        }

        protected bool isOutlet;

        protected List<Wire> wires;

        public List<Wire> Wires
        {
            get
            {
                return wires;
            }
            set
            {
                wires = value;
            }
        }

        public bool IsOutlet
        {
            get
            {
                return isOutlet;
            }
        }

        public bool CastsShadows
        {
            get
            {
                return castsShadows;
            }
        }

        /*protected float density;

        public float Density
        {
            get
            {
                return density;
            }
        }*/

        /*protected float friction;

        public float Friction
        {
            get
            {
                return friction;
            }
            set
            {
                friction = value;
            }
        }*/

        protected List<GameObject> switchedLights;

        /*public List<GameObject> SwitchedLights
        {
            get
            {
                return switchedLights;
            }
        }*/

        /*protected float restitution;

        public float Restitution
        {
            get
            {
                return restitution;
            }
            set
            {
                restitution = value;
            }
        }*/

        protected bool isWire;

        public bool IsWire
        {
            get
            {
                return isWire;
            }
            set
            {
                isWire = value;
            }
        }

        public OBJECT_STATE State
        {
            get
            {
                return state;
            }
            set
            {
                state = value;
            }
        }

        public string XMLFilePath
        {
            get
            {
                return xmlFilePath;
            }
        }
        public int[] Pwrsrc { get { return pwrsrc; } }

        /*public GameObject()
        {
        }*/

        /*
        /// <summary>
        /// pretty straightforward constructor
        /// </summary>
        /// <param name="parentIn">game engine containing this object</param>
        /// <param name="pos">initial position</param>
        /// <param name="vel">initial velocity</param>
        /// <param name="xmllocation">string of data file's location</param>
        /// <param name="scale">scale relative to sprite size</param>
        public GameObject(Vector2 pos, string xmlLocation, Level lvl)
        {
            level = lvl;

            BodyDef def = new BodyDef();
            def.Position.Set(pos.X, pos.Y);
            def.AllowSleep = true;
            def.FixedRotation = true;

            body = level.World.CreateBody(def);

            //velocity = vel;
            xmlFilePath = xmlLocation;
            
        }*/

        public GameObject(ObjectInstance inst, ObjectType objdef, GraphicsDevice device, Level lvl)
        {
            level = lvl;
            xmlFilePath = inst.XMLDataFileLocation;


            //ObjectType objdef = content.Load<ObjectType>(inst.XMLDataFileLocation);
            BodyDef def = new BodyDef();
            def.Position.Set(inst.InitialPosition.X, inst.InitialPosition.Y);
            def.AllowSleep = true;
            if (!isWire)
            {
                def.FixedRotation = true;
            }
            else
            {
                def.FixedRotation = false;
                def.Angle = inst.Orientation;
            }
            def.LinearDamping = 10.0f;
            //def.Angle = inst.Orientation;
            Orientation = inst.Orientation;

            body = level.World.CreateBody(def);

            imgScale = objdef.ImgScale;

            spriteSheet = Reader.LoadImage("Content/"+objdef.SpriteFilePath+".png", device);
            cordPic = Reader.LoadImage("Content/images/cord.png", device);
            framesize = new Vector2(objdef.spriteWidth, objdef.spriteHeight);
            framecount = objdef.Framecount;

            spriteCenter = objdef.spriteCenter;

            lightConvexHull = new Vector2[objdef.lightConvexHull.Length];
            for (int n = 0; n < lightConvexHull.Length; n++)
            {
                Vector2 temp = objdef.lightConvexHull[n];
                lightConvexHull[n] = new Vector2(temp.X / imgScale - spriteCenter.X / imgScale,
                                                 temp.Y / imgScale - spriteCenter.Y / imgScale);
            }

            //physicsConvexHull = Utils.LightHullToPhysics(lightConvexHull);

            Vec2[] physicsBox2DHull = new Vec2[objdef.physicsConvexHull.Length];
            int len = physicsBox2DHull.Length;
            for (int n = 0; n < physicsBox2DHull.Length; n++)
            {
                Vector2 temp = objdef.physicsConvexHull[len - n - 1];
                physicsBox2DHull[n] = new Vec2(temp.X / imgScale - spriteCenter.X / imgScale,
                                               temp.Y / imgScale - spriteCenter.Y / imgScale);
            }
            
            PolygonDef polydef = new PolygonDef();
            polydef.VertexCount = physicsBox2DHull.Length;
            polydef.Vertices = physicsBox2DHull;
            if (this is Girl || this.IsWire || objdef.IsPlug)
            {
                polydef.Filter.GroupIndex = -1;
            }
            else
            {
                polydef.Filter.GroupIndex = 1;
            }
            polydef.Density = objdef.Density;
            polydef.Friction = objdef.Friction;
            polydef.Restitution = objdef.Restitution;

            //Console.WriteLine(inst.XMLDataFileLocation);

            shape = body.CreateShape(polydef);
            body.SetMassFromShapes();
            body.SetUserData(this);

            pwrsrc = inst.PowerSourceID;

            powerSourceID = inst.PowerSourceID[0];
            wires = new List<Wire>();
            for (int n = 1; n < inst.PowerSourceID.Length; n++)
            {
                int id = inst.PowerSourceID[n];
                foreach (GameObject obj in lvl.GetGameObjectList())
                {
                    if (obj.PowerSourceID == id)
                    {
                        obj.AddConnectedLight(this);
                        if (inst.Wires.Count >= n)
                        {//TODO delete this if statement 
                            Wire w = new Wire(level, inst.Wires[n - 1], this, obj);
                            wires.Add(w);
                            obj.wires.Add(w);
                        }
                    }
                }
            }


            connectedLights = new List<GameObject>();
            
            canSlide = objdef.CanSlide;
            canRotate = objdef.CanRotate;
            isSwitch = objdef.IsSwitch;
            isPlug = objdef.IsPlug;
            isOutlet = objdef.isOutlet;
            isWire = objdef.isWire;
            castsShadows = objdef.CastsShadows;

            on = inst.On;

            if (isSwitch || isPlug)
            {
                connectedLights = new List<GameObject>();
            }

            facing = objdef.Facing;

            
        }

        public void Initialize(Level lvl)
        {
            level = lvl;
        }

        /// <summary>
        /// Load in sprite sheet
        /// </summary>
        /*public virtual void LoadContent(ContentManager content){

            //gets correct image. needs to be changed! (read in from xml)
            imgScale = 100;

            if (xmlFilePath.Equals("xml/girl"))
            {
                spriteFilePath = "images\\standback";
            }
            else if (xmlFilePath.Equals("xml/floodlight"))
            {
                spriteFilePath = "images/floodlight";
            }
            else if (xmlFilePath.Equals("xml/lamppost"))
            {
                spriteFilePath = "images/lamppost";
            }
            else if (xmlFilePath.Equals("xml/mirror"))
            {
                spriteFilePath = "images/mirror";
            }
            else if (xmlFilePath.Equals("xml/crate")) 
            {
                spriteFilePath = "images/crate";
            }
            else if (xmlFilePath.Equals("xml/switch"))
            {
                spriteFilePath = "images/grableft";
                isSwitch = true;
                switchedOn = true;
            }
            else
            {
                spriteFilePath = "images/grableft";
            }

            spriteSheet = content.Load<Texture2D>(spriteFilePath);

            //Everything below this point should be replaced with data from XML files
            
            width = spriteSheet.Width;
            height = spriteSheet.Height;

            float halfwidth = width /2f / imgScale;
            float halfheight = height /2f / imgScale;

            PolygonDef polydef = new PolygonDef();
            polydef.SetAsBox(halfwidth, halfheight);
            polydef.Density = .0003f; //density;
            polydef.Friction = 0.0f; //friction;
            polydef.Restitution = 0f; //restitution;

            shape = body.CreateShape(polydef);
            body.SetMassFromShapes();
            body.SetUserData(this);


            spriteCenter = new Vector2(halfwidth, halfheight);
            lightConvexHull = new Vector2[4];
            lightConvexHull[0] = new Vector2(-halfwidth, -halfheight);
            lightConvexHull[1] = new Vector2(-halfwidth, halfheight);
            lightConvexHull[2] = new Vector2(halfwidth, halfheight);
            lightConvexHull[3] = new Vector2(halfwidth, -halfheight);
        }*/

        /// <summary>
        /// Get position of object on the screen, in scale of pixels on the screen
        /// </summary>
        /// <returns></returns>
        public Vector2 GetScreenPosition(){
            return (Position - level.ScreenPosition) * worldScale;
        }

        public Vector2[] PhysicsConvexHull
        {
            get
            {
                Vec2[] vertices = ((PolygonShape)body.GetShapeList()).GetVertices();
                int vertCount = ((PolygonShape)body.GetShapeList()).VertexCount;
                Vector2[] turn = new Vector2[vertCount];
                for (int n = 0; n < vertCount; n++)
                {
                    turn[n] = new Vector2(vertices[n].X + Position.X, vertices[n].Y + Position.Y);
                }

                return turn;
            }
            set
            {
                physicsConvexHull = value;
            }
        }

        public Vector2[] LightConvexHull{
            get{
                Vector2[] turn = new Vector2[lightConvexHull.Length];
                for (int n = 0; n < turn.Length; n++)
                {
                    turn[n] = new Vector2(lightConvexHull[n].X * worldScale + GetScreenPosition().X,
                                          lightConvexHull[n].Y * worldScale + GetScreenPosition().Y);
                }

                return turn;
            }
            /*set{
                lightConvexHull = value;
            }*/
        }

	    //gets the orientation of the GameObject in radians ccw
        private float orientation;
        public float Orientation
        {
            get { return orientation; }
            set
            {
                orientation = value;
            }
        }

        /*private Vector2 facing;
        public Vector2 Facing
        {
            get
            {
                return facing;
            }
            set
            {
                facing = value;
            }
        }*/

        //gets the sprite file path
        /*public string GetSpriteFilePath(){
            return spriteFilePath;
        }*/

	    //sets the sprite file path
        /*void SetSpriteFilePath(string path){
            spriteFilePath = path;
        }*/

        public virtual Texture2D SpriteSheet{
            get{
                return spriteSheet;
            }
        }

        public Vector2 GetSpriteCenter()
        {
            return spriteCenter;
        }


        //designed to be overriden by subclasses
        //public virtual void Update(GameTime gameTime) { }

        /// <summary>
        /// changes state of switch, and all things attached to it.
        /// </summary>
        public void FlipSwitch() {
            Debug.Assert(isSwitch, "trying to flip a switch and this is not a switch");
            on = !on;
            foreach (GameObject obj in connectedLights)
            {
                obj.on = !obj.IsOn;
            }
        }

        /// <summary>
        /// adds game object to things connected to this switch
        /// </summary>
        public void AddConnectedLight(GameObject obj) {
            connectedLights.Add(obj);
        }

        public GameObject PopConnectedLight()
        {
            if(connectedLights.Count == 0) return null;

            GameObject obj = connectedLights[0];
            connectedLights.RemoveAt(0);
            return obj;
        }

        public void TurnLightOn() { connectedLights[0].IsOn = true; }
        public void TurnLightOff() { connectedLights[0].IsOn = false; }

        public int PowerSourceID
        {
            get { return powerSourceID; }
        }

        /// <summary>
        /// Turns a GameElement into an ObjectInstance
        /// </summary>
        public ObjectInstance Compress()
        {
            ObjectInstance anInstance = new ObjectInstance();

            anInstance.XMLDataFileLocation = this.XMLFilePath;
            anInstance.InitialPosition = this.Position;
            anInstance.Orientation = this.Orientation;
            anInstance.On = this.IsOn;
            anInstance.PowerSourceID = this.Pwrsrc;

            List<Vector2[]> wirePoints = new List<Vector2[]>();
            if (this is Light)
            {
                foreach (Wire w in wires)
                {
                    wirePoints.Add(w.Points.ToArray());
                }
            }
            anInstance.Wires = wirePoints;

            return anInstance;
        }
	}
}
