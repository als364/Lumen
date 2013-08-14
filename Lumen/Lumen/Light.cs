using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Box2DX.Collision;
using MyDataTypes;
using Box2DX.Dynamics;
using System.Diagnostics;

namespace Lumen{
    public class Light :GameObject{
/*Light is an abstract subclass of GameObject that represents all objects that emit light. This includes lampposts, floodlights, spotlights, strings of lights, and so on.
Class: Light	
Superclass: GameObject
Responsibilities
Collaborators
knows own position/velocity	
knows own shape/body	Box2D
knows own orientation	
knows own type	
knows own sprite/texture	
knows the shape of own light emission	

 */
        protected List<Vector2[]> invisibleHulls;

        private RenderTarget2D renderTarget;

        private POWER_SOURCE powerSource;

        private GameObject swich;

        protected float intensity;
        protected float lightRadius;

        protected bool checkpoint;
        public bool IsCheckpoint { get { return checkpoint; } }

        //mirror specific fields
        List<Vector2> reflectedLightSources;
        List<int> reflectedLightIntensity;
        int reflectedIntensityIncrement;

        public enum POWER_SOURCE{
            ALWAYS_ON,
            SWITCH,
            PLUG,
            MIRROR
        }

        public Light(ObjectInstance oi, ObjectType objdef, GraphicsDevice device, Level lvl) : base (oi, objdef, device, lvl)
        {
            intensity = objdef.intensity;
            lightRadius = objdef.lightRadius;
            checkpoint = objdef.checkpoint;

            //read in invisible hulls from data file
            invisibleHulls = objdef.invisibleHulls;

            foreach (Vector2[] hull in invisibleHulls)
            {
                for (int n = 0; n < hull.Length; n++)
                {
                    hull[n] = new Vector2((hull[n].X - spriteCenter.X) / imgScale, (hull[n].Y - spriteCenter.Y) / imgScale);
                }
            }
            //currentHull.Add(new Vector2((ray[n] - spriteCenter.X) / imgScale, (ray[n + 1] - spriteCenter.Y) / imgScale));

            /*
            int m = 0;
            invisibleHulls = new List<Vector2[]>();
            List<Vector2> currentHull = new List<Vector2>();

            for (int n = 0; n < ray.Length; n++)
            {
                if (m == 0)
                {
                    if(n!=0)
                        invisibleHulls.Add(currentHull.ToArray());

                    m = (int) ray[n];
                    currentHull = new List<Vector2>();
                }
                else 
                {
                    //transform invisible hulls to have Box2D coordinates,
                    //relative to center of the image
                    currentHull.Add(new Vector2((ray[n] - spriteCenter.X) / imgScale, (ray[n + 1] - spriteCenter.Y) / imgScale));
                    m--; n++;
                }
            }
            invisibleHulls.Add(currentHull.ToArray());

            */

        }

        //new XML stuff
        /*public Light(Vector2 pos, Level lvl, ObjectType lightTypeDef)
        {
            //level = lvl;
            intensity = lightTypeDef.intensity;
            lightRadius = lightTypeDef.lightRadius;
            checkpoint = lightTypeDef.checkpoint;
            
            ImgScale = lightTypeDef.ImgScale;
            SpriteFilePath = lightTypeDef.SpriteFilePath;
            spriteWidth = lightTypeDef.spriteWidth;
            spriteHeight = lightTypeDef.spriteHeight;
            Framecount = lightTypeDef.Framecount;
            spriteCenter = lightTypeDef.spriteCenter;
            CanSlide = lightTypeDef.CanSlide;
            CanRotate = lightTypeDef.CanRotate;
            IsSwitch = lightTypeDef.IsSwitch;
            IsPlug = lightTypeDef.IsPlug;
            isOutlet = lightTypeDef.isOutlet;
            CastsShadows = lightTypeDef.CastsShadows;
            Density = lightTypeDef.Density;
            Friction = lightTypeDef.Friction;
            Restitution = lightTypeDef.Restitution;


            //Console.WriteLine(this.spriteFilePath);


            //make it a body

            BodyDef def = new BodyDef();
            def.Position.Set(pos.X, pos.Y);
            def.AllowSleep = true;
            def.FixedRotation = true;

            body = level.World.CreateBody(def);
        }*/

        
        /*public Light(Vector2 pos, string xmlLocation, Level lvl) 
            : base(pos, xmlLocation, lvl) 
        {
            //renderTarget = new RenderTarget2D(device, width, height);
            invisibleHulls = new List<Vector2[]>();
        }*/

        /// <summary>
        /// Load in sprite sheet
        /// </summary>
        /*public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);

            //everything here needs to be replaced with code that reads in from an XML file

            intensity = 1f;
            lightRadius = 20f;

            if (xmlFilePath.Equals("xml/floodlight"))
            {
                powerSource = POWER_SOURCE.SWITCH;
                List<GameObject> objects = level.GetGameObjectList();
                foreach (GameObject obj in objects)
                {
                    if (obj.IsSwitch)
                    {
                        swich = obj;
                        break;
                    }
                }
            }
            else if (xmlFilePath.Equals("xml/lamppost"))
            {
                powerSource = POWER_SOURCE.ALWAYS_ON;
            }
            else if (xmlFilePath.Equals("xml/mirror"))
            {
                powerSource = POWER_SOURCE.MIRROR;
                reflectedLightSources = new List<Vector2>();
                reflectedLightIntensity = new List<int>();
                reflectedIntensityIncrement = 20;
            }
            else
            {
                powerSource = POWER_SOURCE.ALWAYS_ON;
            }

            
            if (xmlFilePath.Equals("xml/floodlight") || xmlFilePath.Equals("xml/mirror"))
            {
                //adds invisible hulls to contain light. 
                //Should eventually be replaced with data driven design
                float halfwidth = 80/imgScale;
                float halfheight = 80/imgScale;
                Vector2[] rightBox = { new Vector2(halfwidth, -halfheight-.1f),
                                   new Vector2(halfwidth-.01f, -halfheight-.1f),
                                   new Vector2(halfwidth-.01f, halfheight+.1f),
                                   new Vector2(halfwidth, halfheight+.1f)     };
                Vector2[] topBox = { new Vector2(-halfwidth-.1f, -halfheight),
                                   new Vector2(-halfwidth-.1f, -halfheight+.01f),
                                   new Vector2(halfwidth+.1f, -halfheight+.01f),
                                   new Vector2(halfwidth+.1f, -halfheight)     };
                Vector2[] leftBox = { new Vector2(-halfwidth+.01f, -halfheight-.1f),
                                   new Vector2(-halfwidth, -halfheight-.1f),
                                   new Vector2(-halfwidth, halfheight+.1f),
                                   new Vector2(-halfwidth+.01f, halfheight+.1f)     };
                invisibleHulls.Add(rightBox);
                invisibleHulls.Add(topBox);
                invisibleHulls.Add(leftBox);
            }

            width = spriteSheet.Width / imgScale;
            height = spriteSheet.Height / imgScale;

            PolygonDef polydef = new PolygonDef();
            polydef.SetAsBox(width /2, height /2);
            polydef.Density = 10f; //density;
            polydef.Friction = 0.0f; //friction;
            polydef.Restitution = 0f; //restitution;

            shape = body.CreateShape(polydef);
            body.SetMassFromShapes();
            //body.SetUserData(this);


            spriteCenter = new Vector2(width/2, height/2);
            convexHull = new Vector2[4];
            convexHull[0] = new Vector2(-width / 2, -height / 2);
            convexHull[1] = new Vector2(-width / 2, height / 2);
            convexHull[2] = new Vector2(width / 2, height / 2);
            convexHull[3] = new Vector2(width / 2, -height / 2); 

        }*/


        public float LightRadius
        {
            get
            {
                //if mirror, dynamically calculate radius from intensity? probably not
                return lightRadius;
            }
        }
        public float Intensity
        {
            get
            {
                /*if (isMirror())
                {
                    //pop off an intensity and return it plus some increment
                    float turn = reflectedLightIntensity[reflectedLightIntensity.Count - 1] + (reflectedIntensityIncrement/255f);
                    reflectedLightIntensity.RemoveAt(reflectedLightIntensity.Count - 1);
                    return turn;
                }*/
                return intensity;
            }
        }

        public List<Vector2[]> getInvisibleHulls()
        {
            /*if (isMirror())
            {
                //pop a light source point off of reflected light sources
                Vector2 source = reflectedLightSources[reflectedLightSources.Count-1];
                reflectedLightSources.RemoveAt(reflectedLightSources.Count - 1);
                
                //calculuate angle of reflection

                return getInvisibleHulls(0);
            }*/
            return getInvisibleHulls(Orientation);
        }

        /// <summary>
        /// Gets a List of Vector2 arrays, each of which defines a convex hull
        /// that is solely used to block light.
        /// Is in the scale of pixels on the screen.
        /// </summary>
        /// <param name="orient"></param>
        /// <returns></returns>
        private List<Vector2[]> getInvisibleHulls(float orient)
        {
            List<Vector2[]> turn = new List<Vector2[]>();
            foreach (Vector2[] hull in invisibleHulls)
            {
                Vector2[] temp = new Vector2[hull.Length];
                for (int n = 0; n < temp.Length; n++)
                {
                    Matrix m = Matrix.CreateRotationZ(orient);
                    Vector2 rotated = Vector2.Transform(hull[n], m);

                    temp[n] = new Vector2(rotated.X * worldScale + GetScreenPosition().X,
                                          rotated.Y * worldScale + GetScreenPosition().Y);
                }
                turn.Add(temp);
            }

            return turn;
        }

        public RenderTarget2D RenderTarget
        {
            get
            {
                return renderTarget;
            }
            set
            {
                renderTarget = value;
            }
        }

        /* //This method is being replaced with IsOn in Game Object
         * //This is to allow on to represent both the switch state and the light state
         * //and have the switch update the light directly instead of a check every frame
         * public bool isOn()
        {
            switch (powerSource) {
                case POWER_SOURCE.ALWAYS_ON:
                    return true;
                case POWER_SOURCE.SWITCH:
                    if (swich == null)
                        return true;
                    else
                        return swich.SwitchedOn;
                case POWER_SOURCE.PLUG:
                    return true; //TODO
                case POWER_SOURCE.MIRROR:
                    return reflectedLightSources.Count > 0;
            }
            return false;
        }*/


        //Mirror functions

        /*public bool isMirror()
        {
            return powerSource == POWER_SOURCE.MIRROR;
        }

        public bool LightSourcesEmpty()
        {
            Debug.Assert(isMirror(), "Checking if any more light sources on something that isn't a mirror");
            return reflectedLightSources.Count == 0;
        }

        /// <summary>
        /// clears all mirror data from last fram
        /// </summary>
        public void ClearMirrorLists()
        {
            reflectedLightIntensity.Clear();
            reflectedLightSources.Clear();
        }
        /// <summary>
        /// returns point that measures light intensity to determine whether or not to reflect
        /// </summary>
        /// <returns></returns>
        public Vector2 GetReflectionPoint()
        {
            Vector2 pos = GetScreenPosition();
            double orient = Body.GetAngle();
            float constant = 10f;
            return new Vector2(pos.X + constant * (float) Math.Acos(orient),
                               pos.Y + constant * (float) Math.Asin(orient));
            //Assumes the "angle" is 0 at pointing right? might need to change this...
        }
        /// <summary>
        /// Add a source of reflected light to this mirror
        /// </summary>
        /// <param name="source"></param>
        public void AddReflectedLightSource(Vector2 source) 
        {
            reflectedLightSources.Add(source);
        }
        /// <summary>
        /// Add the intensity of the reflected light
        /// </summary>
        /// <param name="intense"></param>
        public void AddReflectedLightIntensity(int intense)
        {
            reflectedLightIntensity.Add(intense);
        }*/

        /*
        enum PossibleLightEmissionShapes{Burst, Cone, Line, DiscreteLine};
    
        private PossibleLightEmissionShapes lightEmissionShape;
    

        //gets the light emission shape
        public PossibleLightEmissionShapes GetLightEmissionShape(){
            return lightEmissionShape;
        }
	        
        //sets the light emission shape
        void SetLightEmissionShape(PossibleLightEmissionShapes shape){
            lightEmissionShape = shape;
        }
         * */
    }
}
