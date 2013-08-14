using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Box2DX.Common;
using Box2DX.Collision;
using Box2DX.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MyDataTypes;

namespace Lumen{
    public class GirlController : ObjectController{
        private Girl girl;
        private Level level;
        private List<Joint> girljoints;
        private const float CARRY_OFFSET = 1;
        private Vector2 north = new Vector2(0, 1);
        private Vector2 south = new Vector2(0, -1);
        private Vector2 east = new Vector2(1, 0);
        private Vector2 west = new Vector2(-1, 0);
        private bool playingWalking;
        private SoundEffectInstance walkingInstance;
        private SoundEffectInstance chumInstance;
        private SoundEffectInstance offInstance;
        private SoundEffectInstance onInstance;
        private SoundEffectInstance rotateInstance;
        private SoundEffectInstance slideInstance;
        private SoundEffectInstance wheelsInstance;
        private bool playingChum;
        private bool playingOn;
        private bool playingOff;
        private bool playingRotate;
        private bool playingSlide;
        private bool playingWheels;
        private bool playedCry;
        private bool playedMeep;

        private int startingSwitching; //0 none, 1 resetting animation, 2 waiting for next frame, 3 waiting for animation to finish
        private int startingPlugging;
        private int startingPicking;

        private GameObject outlet;

        private LevelController levelCtrl;

        /*or... I was thinking...
         * Sowwy Gabriel =3
         */
        /*public GirlController(Level currentLevel)
        {
            Vector2 startPos = new Vector2(0);
            Vector2 startVel = new Vector2(0);
            Girl controlledObject = new Girl(currentLevel);
            GirlView girlDrawer = new GirlView(controlledObject);
            girljoints = new List<Joint>();
            level = currentLevel;
        }*/

        public GirlController(Girl thisGirl, Level thisLevel, LevelController lvlController) : base(thisGirl)
        {
            girljoints = new List<Joint>();
            girl = thisGirl;
            level = thisLevel;

            levelCtrl = lvlController;

            startingSwitching = 0;
            startingPlugging = 0;
            startingPicking = 0;

            playedCry = false;
            playedMeep = false;
        }

        /*public Girl getControlledObject()
        {
            return girl;
        }

        public void setControlledObject(Girl obj)
        {
            girl = obj;
        }*/

        public override void Update(GameTime gameTime)
        {
            KeyboardState keyboard = Keyboard.GetState();
            //Console.WriteLine("Begin attached to :" + this.girljoints.Count);
            //act appropriately
            switch (girl.Action)
            {
                case Girl.GIRL_STATE.STANDING:
                    //Console.WriteLine("Standing");
                    /*Console.WriteLine("Orientation: " + girl.Orientation.X + ", " + girl.Orientation.Y);
                    Console.WriteLine(girl.Body.IsStatic());*/
                    if (playingOn)
                    {
                        onInstance.Stop();
                        playingOn = false;
                    }
                    if (playingRotate)
                    {
                        rotateInstance.Stop();
                        playingRotate = false;
                    }
                    if (playingSlide)
                    {
                        slideInstance.Stop();
                        playingSlide = false;
                    }
                    if (playingWheels)
                    {
                        wheelsInstance.Stop();
                        playingWheels = false;
                    }
                    if (playingWalking)
                    {
                        walkingInstance.Stop();
                        playingWalking = false;
                    }
                    if (playingOff)
                    {
                        offInstance.Stop();
                        playingOff = false;
                    }
                    stand();
                    release();
                    break;
                case Girl.GIRL_STATE.WALKING:
                    //Console.WriteLine("Walking");
                    /*Console.WriteLine("Orientation: " + girl.Orientation.X + ", " + girl.Orientation.Y);
                    Console.WriteLine(girl.Body.IsStatic());*/
                    if (playingChum)
                    {
                        chumInstance.Stop();
                        playingChum = false;
                    }
                    if (playingOff)
                    {
                        offInstance.Stop();
                        playingOff = false;
                    }
                    if (playingOn)
                    {
                        onInstance.Stop();
                        playingOn = false;
                    }
                    if (playingRotate)
                    {
                        rotateInstance.Stop();
                        playingRotate = false;
                    }
                    if (playingSlide)
                    {
                        slideInstance.Stop();
                        playingSlide = false;
                    }
                    if (playingWheels)
                    {
                        wheelsInstance.Stop();
                        playingWheels = false;
                    }
                    if (!playingWalking)
                    {
                        walkingInstance = levelCtrl.audioManager.Play(AudioManager.SFXSelection.GirlRunning);
                        //walkingInstance.IsLooped = true;
                        playingWalking = true;
                    }
                    release();
                    walk(new Box2DX.Common.Vec2(Girl.SPEED * girl.Facing.X, Girl.SPEED * girl.Facing.Y));
                    break;
                case Girl.GIRL_STATE.HOLDING:
                    //Console.WriteLine("Holding");
                    //Console.WriteLine(girl.Adjacent);
                    if (playingOn)
                    {
                        onInstance.Stop();
                        playingOn = false;
                    }
                    if (playingRotate)
                    {
                        rotateInstance.Stop();
                        playingRotate = false;
                    }
                    if (playingSlide)
                    {
                        slideInstance.Stop();
                        playingSlide = false;
                    }
                    if (playingWheels)
                    {
                        wheelsInstance.Stop();
                        playingWheels = false;
                    }
                    if (playingWalking)
                    {
                        walkingInstance.Stop();
                        playingWalking = false;
                    }
                    if (playingOff)
                    {
                        offInstance.Stop();
                        playingOff = false;
                    }
                    grab();
                    hold();
                    break;
                case Girl.GIRL_STATE.PUSHING:
                case Girl.GIRL_STATE.STRAFE_RIGHT:
                case Girl.GIRL_STATE.STRAFE_LEFT:
                case Girl.GIRL_STATE.PULLING:
                    //Console.WriteLine("PUSHING");
                    //Console.WriteLine(girl.Adjacent);
                    grab();
                    if (playingChum)
                    {
                        chumInstance.Stop();
                        playingChum = false;
                    }
                    if (playingOff)
                    {
                        offInstance.Stop();
                        playingOff = false;
                    }
                    if (playingOn)
                    {
                        onInstance.Stop();
                        playingOn = false;
                    }
                    if (playingRotate)
                    {
                        rotateInstance.Stop();
                        playingRotate = false;
                    }
                    if (!playingWalking)
                    {
                        walkingInstance = levelCtrl.audioManager.Play(AudioManager.SFXSelection.GirlRunning);
                        //walkingInstance.IsLooped = true;
                        playingWalking = true;
                    }
                    if (!playingWheels)
                    {
                        wheelsInstance = levelCtrl.audioManager.Play(AudioManager.SFXSelection.RollingWheels);
                        //wheelsInstance.IsLooped = true;
                        playingWheels = true;
                    }
                    if (girl.Possession!=null && !girl.Possession.CanRotate)
                    {
                        if (!playingSlide)
                        {
                            slideInstance = levelCtrl.audioManager.Play(AudioManager.SFXSelection.SlideCrate);
                            //slideInstance.IsLooped = true;
                            playingSlide = true;
                        }
                    }
                    else
                    {
                        if (playingSlide)
                        {
                            slideInstance.Stop();
                            playingSlide = false;
                        }
                    }
                    if (keyboard.IsKeyDown(Keys.Up))
                    {
                        pull(new Box2DX.Common.Vec2(0, -(CARRY_OFFSET * Girl.SPEED)));
                    }
                    else if (keyboard.IsKeyDown(Keys.Down))
                    {
                        pull(new Box2DX.Common.Vec2(0, (CARRY_OFFSET * Girl.SPEED)));
                    }
                    else if (keyboard.IsKeyDown(Keys.Left))
                    {
                        pull(new Box2DX.Common.Vec2(-(CARRY_OFFSET * Girl.SPEED), 0));
                    }
                    else if (keyboard.IsKeyDown(Keys.Right))
                    {
                        pull(new Box2DX.Common.Vec2((CARRY_OFFSET * Girl.SPEED), 0));
                    }
                    break;

                //Console.WriteLine("Pushing");
                //Console.WriteLine(girl.Adjacent);
                /*grab();
                if (keyboard.IsKeyDown(Keys.Up))
                {
                    push(new Box2DX.Common.Vec2(0, -Girl.SPEED));
                }
                else if (keyboard.IsKeyDown(Keys.Down))
                {
                    push(new Box2DX.Common.Vec2(0, Girl.SPEED));
                }
                else if (keyboard.IsKeyDown(Keys.Left))
                {
                    push(new Box2DX.Common.Vec2(-Girl.SPEED, 0));
                }
                else if (keyboard.IsKeyDown(Keys.Right))
                {
                    push(new Box2DX.Common.Vec2(Girl.SPEED, 0));
                }
                break;*/
                case Girl.GIRL_STATE.ROTATING_CW:
                    grab();
                    if (playingChum)
                    {
                        chumInstance.Stop();
                        playingChum = false;
                    }
                    if (playingOff)
                    {
                        offInstance.Stop();
                        playingOff = false;
                    }
                    if (playingOn)
                    {
                        onInstance.Stop();
                        playingOn = false;
                    }
                    if (playingWalking)
                    {
                        walkingInstance.Stop();
                        playingWalking = false;
                    }
                    if (!playingRotate)
                    {
                        rotateInstance = levelCtrl.audioManager.Play(AudioManager.SFXSelection.RotateLight);
                        //rotateInstance.IsLooped = true;
                        playingRotate = true;
                    }
                    rotateCW();
                    break;
                case Girl.GIRL_STATE.ROTATING_CCW:
                    grab();
                    if (playingChum)
                    {
                        chumInstance.Stop();
                        playingChum = false;
                    }
                    if (playingOff)
                    {
                        offInstance.Stop();
                        playingOff = false;
                    }
                    if (playingOn)
                    {
                        onInstance.Stop();
                        playingOn = false;
                    }
                    if (playingWalking)
                    {
                        walkingInstance.Stop();
                        playingWalking = false;
                    }
                    if (!playingRotate)
                    {
                        rotateInstance = levelCtrl.audioManager.Play(AudioManager.SFXSelection.RotateLight);
                        //rotateInstance.IsLooped = true;
                        playingRotate = true;
                    }
                    rotateCCW();
                    break;
                case Girl.GIRL_STATE.SWITCHING_UP:
                    if (playingOff)
                    {
                        offInstance.Stop();
                        playingOff = false;
                    }
                    if (playingRotate)
                    {
                        rotateInstance.Stop();
                        playingOn = false;
                    }
                    if (playingSlide)
                    {
                        slideInstance.Stop();
                        playingSlide = false;
                    }
                    if (playingWheels)
                    {
                        wheelsInstance.Stop();
                        playingWheels = false;
                    }
                    if (playingWalking)
                    {
                        walkingInstance.Stop();
                        playingWalking = false;
                    }
                    if (!playingOn)
                    {
                        onInstance = levelCtrl.audioManager.Play(AudioManager.SFXSelection.SwitchOn);
                        //onInstance.IsLooped = true;
                        playingOn = true;
                    }
                    if (!playingChum)
                    {
                        chumInstance = levelCtrl.audioManager.Play(AudioManager.SFXSelection.ElectricChum);
                        //chumInstance.IsLooped = true;
                        playingChum = true;
                    }
                    throwSwitch(+1, gameTime);
                    break;
                case Girl.GIRL_STATE.SWITCHING_DOWN:
                    if (playingOn)
                    {
                        onInstance.Stop();
                        playingOn = false;
                    }
                    if (playingRotate)
                    {
                        rotateInstance.Stop();
                        playingRotate = false;
                    }
                    if (playingSlide)
                    {
                        slideInstance.Stop();
                        playingSlide = false;
                    }
                    if (playingWheels)
                    {
                        wheelsInstance.Stop();
                        playingWheels = false;
                    }
                    if (playingWalking)
                    {
                        walkingInstance.Stop();
                        playingWalking = false;
                    }
                    if (!playingOff)
                    {
                        offInstance = levelCtrl.audioManager.Play(AudioManager.SFXSelection.SwitchOff);
                        //offInstance.IsLooped = true;
                        playingOff = true;
                    }
                    if (!playingChum)
                    {
                        chumInstance = levelCtrl.audioManager.Play(AudioManager.SFXSelection.ElectricChum);
                        //chumInstance.IsLooped = true;
                        playingChum = true;
                    }
                    throwSwitch(-1, gameTime);
                    break;
                case Girl.GIRL_STATE.PLUGGING:
                    plug(+1, gameTime);
                    if (playingOn)
                    {
                        onInstance.Stop();
                        playingOn = false;
                    }
                    if (playingRotate)
                    {
                        rotateInstance.Stop();
                        playingRotate = false;
                    }
                    if (playingSlide)
                    {
                        slideInstance.Stop();
                        playingSlide = false;
                    }
                    if (playingWheels)
                    {
                        wheelsInstance.Stop();
                        playingWheels = false;
                    }
                    if (playingWalking)
                    {
                        walkingInstance.Stop();
                        playingWalking = false;
                    }
                    if (playingOff)
                    {
                        offInstance.Stop();
                        playingOff = false;
                    }
                    if (!playingChum)
                    {
                        chumInstance = levelCtrl.audioManager.Play(AudioManager.SFXSelection.ElectricChum);
                        //chumInstance.IsLooped = true;
                        playingChum = true;
                    }
                    break;
                case Girl.GIRL_STATE.UNPLUGGING:
                    plug(-1, gameTime);
                    if (playingOn)
                    {
                        onInstance.Stop();
                        playingOn = false;
                    }
                    if (playingRotate)
                    {
                        rotateInstance.Stop();
                        playingRotate = false;
                    }
                    if (playingSlide)
                    {
                        slideInstance.Stop();
                        playingSlide = false;
                    }
                    if (playingWheels)
                    {
                        wheelsInstance.Stop();
                        playingWheels = false;
                    }
                    if (playingWalking)
                    {
                        walkingInstance.Stop();
                        playingWalking = false;
                    }
                    if (playingOff)
                    {
                        offInstance.Stop();
                        playingOff = false;
                    }
                    if (!playingChum)
                    {
                        chumInstance = levelCtrl.audioManager.Play(AudioManager.SFXSelection.ElectricChum);
                        //chumInstance.IsLooped = true;
                        playingChum = true;
                    }
                    break;
                case Girl.GIRL_STATE.PICKING_UP:
                    if (playingOn)
                    {
                        onInstance.Stop();
                        playingOn = false;
                    }
                    if (playingRotate)
                    {
                        rotateInstance.Stop();
                        playingRotate = false;
                    }
                    if (playingSlide)
                    {
                        slideInstance.Stop();
                        playingSlide = false;
                    }
                    if (playingWheels)
                    {
                        wheelsInstance.Stop();
                        playingWheels = false;
                    }
                    if (playingWalking)
                    {
                        walkingInstance.Stop();
                        playingWalking = false;
                    }
                    if (playingOff)
                    {
                        offInstance.Stop();
                        playingOff = false;
                    }
                    pickup(+1, gameTime);
                    break;
                case Girl.GIRL_STATE.PUTTING_DOWN:
                    if (playingOn)
                    {
                        onInstance.Stop();
                        playingOn = false;
                    }
                    if (playingRotate)
                    {
                        rotateInstance.Stop();
                        playingRotate = false;
                    }
                    if (playingSlide)
                    {
                        slideInstance.Stop();
                        playingSlide = false;
                    }
                    if (playingWheels)
                    {
                        wheelsInstance.Stop();
                        playingWheels = false;
                    }
                    if (playingWalking)
                    {
                        walkingInstance.Stop();
                        playingWalking = false;
                    }
                    if (playingOff)
                    {
                        offInstance.Stop();
                        playingOff = false;
                    }
                    pickup(-1, gameTime);
                    break;
            }
            /*Console.WriteLine("Courage: " + girl.Courage);
            Console.WriteLine("Meep: " + playedMeep);
            Console.WriteLine("Cry: " + playedCry);*/
            if (girl.Courage <= .5f && !playedMeep)
            {
                levelCtrl.audioManager.PlayCry(AudioManager.SFXSelection.Meep);
                playedMeep = true;
            }
            if (girl.Courage == 0f && !playedCry)
            {
                levelCtrl.audioManager.PlayCry(AudioManager.SFXSelection.CryingGirl);
                playedCry = true;
            }
            if (girl.Courage > 0f)
            {
                playedCry = false;
            }
            if (girl.Courage > .25f)
            {
                playedMeep = false;
            }


            //every "framesPerAnimation", we call animate and update teh nextAnimationFrame
            if ((gameTime.TotalGameTime.TotalMilliseconds * 60 / 1000) >= girl.NextAnimationFrame)
            {
                animate(girl.Action);
                girl.NextAnimationFrame = ((int)gameTime.TotalGameTime.TotalMilliseconds * 60 / 1000) + girl.FramesPerAnimation;
            }
        }

        public void stand()
        {
            girl.Body.SetLinearVelocity(new Vec2(0, 0));
            if (girl.Possession != null && girl.Possession.Body != null)
            {
                girl.Possession.Body.SetLinearVelocity(new Vec2(0, 0));
                MassData massdata = new MassData();
                massdata.Mass = 0;
                massdata.Center = girl.Possession.Body.GetLocalCenter();
                massdata.I = girl.Possession.Body.GetInertia();
                girl.Possession.Body.SetMass(massdata);
                girl.Possession = null;
            }
        }

        /// <summary>
        /// Changes the object's image to the correct action.
        /// Updates the animationCycleState to increment appropriately.
        /// </summary>
        private void animate(Lumen.Girl.GIRL_STATE spriteNum)
        {
            

            /*//set total number of frames in the animation

            if (dir <= 1) //up or down
                girl.AnimationCycleState[1] = 8;
            else //left or right
                girl.AnimationCycleState[1] = 6;

            //rotating only has 4 frames
            if (sprite == Girl.GIRL_STATE.ROTATING_CW || sprite == Girl.GIRL_STATE.ROTATING_CCW)
                girl.AnimationCycleState[1] = 4;
            //not moving
            else if (sprite == Girl.GIRL_STATE.STANDING || sprite == Girl.GIRL_STATE.HOLDING)
                girl.AnimationCycleState[1] = 1;
            */

            //increment frame of animation
            girl.AnimationCycleState = (girl.AnimationCycleState + 1) % Girl.AnimationStates;

            //Console.WriteLine("dir is " + dir + " and sprite is " + (int)sprite);
            //choose correct image from matrix
            girl.SetSpriteSheet(girl.Animations[(int)spriteNum]);
        }


        /// <summary>
        /// assigns possession to girl if appropriate to do so
        /// </summary>
        public void grab()
        {
            //Console.WriteLine("Count: " + girl.Adjacent.Count);
            float dist = 1.0f;
            GameObject closest = null; //new GameObject();
            if (girl.Adjacent.Count > 0) //otherwise stick with what you got
            {
                //Console.WriteLine("Adjacent to things");
                if (girl.Possession == null)
                {
                    //Console.WriteLine("No Possession");
                    foreach (GameObject obj in girl.Adjacent) //check all objects adjacent to girl
                    {
                        if (obj != girl)
                        {
                            if (girl.Facing == new Vector2(1, 0))
                            {
                                if (obj.Position.X > girl.Position.X && System.Math.Abs(obj.Position.Y - girl.Position.Y) < dist)
                                {
                                    dist = System.Math.Abs(obj.Position.Y - girl.Position.Y);
                                    closest = obj;
                                }
                            }
                            else if (girl.Facing == new Vector2(0, 1))
                            {
                                if (obj.Position.Y > girl.Position.Y && System.Math.Abs(obj.Position.X - girl.Position.X) < dist)
                                {
                                    dist = System.Math.Abs(obj.Position.X - girl.Position.X);
                                    closest = obj;
                                }
                            }
                            else if (girl.Facing == new Vector2(-1, 0))
                            {
                                if (obj.Position.X < girl.Position.X && System.Math.Abs(obj.Position.Y - girl.Position.Y) < dist)
                                {
                                    dist = System.Math.Abs(obj.Position.Y - girl.Position.Y);
                                    closest = obj;
                                }
                            }
                            else if (girl.Facing == new Vector2(0, -1))
                            {
                                if (obj.Position.Y < girl.Position.Y && System.Math.Abs(obj.Position.X - girl.Position.X) < dist)
                                {
                                    dist = System.Math.Abs(obj.Position.X - girl.Position.X);
                                    closest = obj;
                                }
                            }
                        }
                        //Console.WriteLine("Closest Selected");
                    }
                    if (closest != null)
                    {
                        //Console.WriteLine("Switch: " + closest.IsSwitch);
                        if (closest.IsSwitch)
                        {
                            if (closest.Facing == new Vector2(-girl.Facing.X, -girl.Facing.Y))
                            {
                                girl.Possession = closest;

                                //closest.FlipSwitch();
                                if (closest.IsOn) girl.Action = Girl.GIRL_STATE.SWITCHING_DOWN;
                                else girl.Action = Girl.GIRL_STATE.SWITCHING_UP;

                                startingSwitching = 1;
                            }
                        }
                        else if (closest.IsOutlet)
                        {
                            if (closest.Facing == new Vector2(-girl.Facing.X, -girl.Facing.Y))
                            {
                                outlet = closest;
                                if (girl.CarriedPlug != null && !closest.IsOn) 
                                {
                                    girl.Action = Girl.GIRL_STATE.PLUGGING;   
                                }
                                else if (girl.CarriedPlug == null && closest.IsOn)
                                {
                                    girl.Action = Girl.GIRL_STATE.UNPLUGGING;
                                } 

                                startingPlugging = 1;
                            }
                        }
                        else if (closest.CanRotate || closest.CanSlide)
                        {
                            girl.Possession = closest;
                            //Console.WriteLine("Closest Possessed: " + girl.Possession);
                            if (girljoints.Count > 0)
                            {
                                foreach (Joint joint in girljoints)
                                {
                                    level.World.DestroyJoint(joint);
                                }
                                girljoints.Clear();
                                //Console.WriteLine("Destroyed a bunch of joints");
                            }
                            RevoluteJointDef jointdef = new RevoluteJointDef();
                            jointdef.Initialize(girl.Body, girl.Possession.Body, girl.Body.GetPosition() - girl.Possession.Body.GetPosition());
                            jointdef.CollideConnected = true;
                            girljoints.Add(level.World.CreateJoint(jointdef));
                            MassData massdata = new MassData();
                            massdata.Mass = girl.Body.GetMass();
                            massdata.Center = girl.Possession.Body.GetLocalCenter();
                            massdata.I = girl.Possession.Body.GetInertia();
                            girl.Possession.Body.SetMass(massdata);
                            /*girl.Body.SetMass(massdata);*/
                            //Console.WriteLine("Attached to: " + girljoints.Count);
                        }
                    }
                }
            }
            //there is a plug cord coordinate nearby
            else if (girl.Possession == null)//no adjacent object, try to place / pickup plug
            {
                if (girl.CarriedPlug != null)
                {
                    girl.Action = Girl.GIRL_STATE.PUTTING_DOWN;
                    startingPicking = 1;
                }
                else
                {
                    foreach (GameObject obj in level.GetGameObjectList())
                    {
                        if (obj.IsPlug && (obj.Position - girl.Position).Length() < .5f)
                        {
                            girl.Action = Girl.GIRL_STATE.PICKING_UP;
                            girl.CarriedPlug = obj;
                            startingPicking = 1;
                            break;
                        }
                    }
                }
            }
            else
            {
                release();
            }
        }

        /// <summary>
        /// get rid of possession if you have one
        /// </summary>
        public void release()
        {
            if (girl.Possession != null && girl.Possession.Body != null)
            {
                girl.Possession.Body.SetLinearVelocity(new Vec2(0, 0));
                girl.Possession.Body.SetAngularVelocity(0);
                if (girljoints.Count > 0)
                {
                    foreach (Joint joint in girljoints)
                    {
                        level.World.DestroyJoint(joint);
                    }
                    girljoints.Clear();
                    //Console.WriteLine("Object Released");
                }
                MassData massdata = new MassData();
                massdata.Mass = 0;
                massdata.Center = girl.Possession.Body.GetLocalCenter();
                massdata.I = girl.Possession.Body.GetInertia();
                girl.Possession.Body.SetMass(massdata);
            }
            girl.Possession = null;
        }

        /// <summary>
        /// Makes sure the object stops moving when you do
        /// </summary>
        public void hold()
        {
            if (girl.Possession != null && girl.Possession.Body != null)
            {
                girl.Possession.Body.SetLinearVelocity(new Vec2(0, 0));
                girl.Possession.Body.SetAngularVelocity(0);
            }
                
        }

        /// <summary>
        /// If no collisions, move according to velocity
        /// </summary>
        public void walk(Vec2 vel)
        {
            if (girljoints.Count > 0)
            {
                foreach (Joint joint in girljoints)
                {
                    level.World.DestroyJoint(joint);
                }
                //Console.WriteLine("Companion Cube Euthanized");
                girljoints.Clear();
            }
            //girl.Possession.Body.SetMassFromShapes();
            //Debug.Assert(false, "force: "+force.X+", "+force.Y);
            girl.Body.SetLinearVelocity(vel);
            //girl.Body.SetXForm(girl.Body.GetPosition() + girl.Body.GetLinearVelocity(), 0);
        }

        /// <summary>
        /// girl walks backwards
        /// possession gains appropriate velocity and is told to move
        /// </summary>
        private void pull(Box2DX.Common.Vec2 vel)
        {
            //Console.WriteLine("Pulling");
            /*if ((girl.Possession != null && girl.Possession.Body != null) && girl.Possession.CanSlide)
            {
                girl.Possession.Body.SetLinearVelocity(vel);
                //girl.Possession.Body.SetXForm((girl.Possession.Body.GetPosition() + vel), girl.Body.GetAngle());
                girl.Possession.State = GameObject.OBJECT_STATE.MOVING;
            }*/
            girl.Body.SetLinearVelocity(vel);
        }

        /// <summary>
        /// girl walks forwards
        /// possession gains appropriate velocity and is told to move
        /// </summary>
        private void push(Box2DX.Common.Vec2 vel)
        {
            Console.WriteLine("Pushing");
            /*if ((girl.Possession != null && girl.Possession.Body != null) && girl.Possession.CanSlide)
            {
                girl.Possession.Body.SetLinearVelocity(vel);
                //girl.Possession.Body.SetXForm((girl.Possession.Body.GetPosition() + vel), girl.Body.GetAngle());
                girl.Possession.State = GameObject.OBJECT_STATE.MOVING;
            }*/
            girl.Body.SetLinearVelocity(vel);
        }

        /// <summary>
        /// change possession's currect action to rotating
        /// </summary>
        public void rotateCW()
        {
            if ((girl.Possession != null && girl.Possession.Body != null) && girl.Possession.CanRotate)
            {
                girl.Possession.Orientation += .10f;
                girl.Possession.Body.SetLinearVelocity(new Vec2(0, 0));
                girl.Possession.State = GameObject.OBJECT_STATE.ROTATING_CW;
            }
        }

        /// <summary>
        /// change possession's currect action to rotating
        /// </summary>
        public void rotateCCW()
        {
            if ((girl.Possession != null && girl.Possession.Body != null) && girl.Possession.CanRotate)
            {
                girl.Possession.Orientation -= .10f;
                girl.Possession.Body.SetLinearVelocity(new Vec2(0, 0));
                girl.Possession.State = GameObject.OBJECT_STATE.ROTATING_CCW;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void throwSwitch(int dir, GameTime gameTime)
        {
            if(startingSwitching == 1) {
                girl.NextAnimationFrame = ((int)gameTime.TotalGameTime.TotalMilliseconds * 60 / 1000);
                girl.AnimationCycleState = girl.FramesPerAnimation-1;
                startingSwitching++;
            }
            else if (startingSwitching == 2 && girl.AnimationCycleState > 0)
            {
                startingSwitching++;
            }
            else if (startingSwitching == 3 && girl.AnimationCycleState == 0)
            {
                girl.Possession.FlipSwitch();
                girl.Action = Girl.GIRL_STATE.STANDING;
                girl.Possession = null;
                startingSwitching = 0;
            }

            if (startingSwitching > 1)
            {
                float ratio = 0f;
                if (dir > 0)
                {
                    ratio = ((float)girl.AnimationCycleState / girl.FramesPerAnimation);
                }
                else
                {
                    ratio = 1f - ((float) (girl.AnimationCycleState+1) / girl.FramesPerAnimation);
                }
                girl.Possession.Orientation = ratio * MathHelper.TwoPi;
            }
        }

        public void plug(int dir, GameTime gameTime)
        {
            if (startingPlugging == 1)
            {
                girl.NextAnimationFrame = ((int)gameTime.TotalGameTime.TotalMilliseconds * 60 / 1000);
                girl.AnimationCycleState = girl.FramesPerAnimation - 1;
                startingPlugging++;
            }
            else if (startingPlugging == 2 && girl.AnimationCycleState > 0)
            {
                startingPlugging++;
            }
            else if (startingPlugging == 3 && girl.AnimationCycleState == 0)
            {
                if (dir > 0)
                {
                    outlet.CarriedPlug = girl.CarriedPlug;
                    girl.CarriedPlug = null;
                    outlet.AddConnectedLight(girl.PopConnectedLight());
                    if (girl.Wires.Count > 0)
                    {
                        girl.Wires[0].ChangeSource(outlet);
                        outlet.Wires.Add(girl.Wires[0]);
                        girl.Wires.RemoveAt(0);
                    }
                    outlet.IsOn = true;
                    outlet.TurnLightOn();
                }
                else
                {
                    outlet.TurnLightOff();
                    girl.CarriedPlug = outlet.CarriedPlug;
                    outlet.CarriedPlug = null;
                    girl.AddConnectedLight(outlet.PopConnectedLight());
                    if (outlet.Wires.Count > 0)
                    {
                        outlet.Wires[0].ChangeSource(girl);
                        girl.Wires.Add(outlet.Wires[0]);
                        outlet.Wires.RemoveAt(0);
                    }
                    outlet.IsOn = false;
                }
                girl.Action = Girl.GIRL_STATE.STANDING;
                startingPlugging = 0;
            }
        }

        private PolygonDef polydef;

        public void pickup(int dir, GameTime gameTime)
        {
            if (startingPicking == 1)
            {
                girl.NextAnimationFrame = ((int)gameTime.TotalGameTime.TotalMilliseconds * 60 / 1000);
                girl.AnimationCycleState = girl.FramesPerAnimation - 1;
                startingPicking++;
            }
            else if (startingPicking == 2 && girl.AnimationCycleState > 0)
            {
                startingPicking++;
            }
            else if (startingPicking == 3 && girl.AnimationCycleState == 0)
            {
                if (dir > 0)
                {
                    girl.AddConnectedLight(girl.CarriedPlug.PopConnectedLight());

                    level.GetGameObjectList().Remove(girl.CarriedPlug);
                    levelCtrl.RemoveFromViewList(girl.CarriedPlug);
                    levelCtrl.RemoveFromControlList(girl.CarriedPlug);

                    if(girl.CarriedPlug.Wires.Count > 0) {
                        girl.CarriedPlug.Wires[0].ChangeSource(girl);
                        girl.Wires.Add(girl.CarriedPlug.Wires[0]);
                        girl.CarriedPlug.Wires.RemoveAt(0);
                    }

                    PolygonShape shape = (PolygonShape)girl.CarriedPlug.Body.GetShapeList();
                    polydef = new PolygonDef();
                    polydef.VertexCount = shape.VertexCount;
                    polydef.Vertices = new Vec2[polydef.VertexCount];
                    for (int n = 0; n < polydef.Vertices.Length; n++)
                    {
                        polydef.Vertices[n] = shape.GetVertices()[n];
                    }
                    polydef.Filter.GroupIndex = -1;
                    polydef.Density = shape.Density;
                    polydef.Friction = shape.Friction;
                    polydef.Restitution = shape.Restitution;

                    level.World.DestroyBody(girl.CarriedPlug.Body);
                }
                else
                {
                    girl.CarriedPlug.AddConnectedLight(girl.PopConnectedLight());

                    //recreate the carried plug's body
                    BodyDef def = new BodyDef();
                    //List<Vector2> pnts = girl.Wires[0].Points;
                    def.Position.Set(girl.Position.X, girl.Position.Y); //pnts[pnts.Count-1].X, pnts[pnts.Count-1].Y);
                    def.AllowSleep = true;
                    def.FixedRotation = false;
                    def.Angle = 0;
                    def.LinearDamping = 10.0f;

                    girl.CarriedPlug.Body = level.World.CreateBody(def);

                    girl.CarriedPlug.Body.CreateShape(polydef);
                    girl.CarriedPlug.Body.SetMassFromShapes();
                    girl.CarriedPlug.Body.SetUserData(girl.CarriedPlug);


                    //add to world
                    level.GetGameObjectList().Add(girl.CarriedPlug);
                    levelCtrl.AddToViewList(girl.CarriedPlug);
                    levelCtrl.AddToControlList(girl.CarriedPlug);

                    if (girl.Wires.Count > 0)
                    {
                        girl.Wires[0].ChangeSource(girl.CarriedPlug);
                        girl.CarriedPlug.Wires.Add(girl.Wires[0]);
                        girl.Wires.RemoveAt(0);
                    }
                    girl.CarriedPlug = null;
                }
                girl.Action = Girl.GIRL_STATE.STANDING;
                startingPicking = 0;
            }
        }

        public void putDownPlug()
        {
            girl.Action = Girl.GIRL_STATE.PUTTING_DOWN;
            if (startingPicking == 0)
            {
                startingPicking = 1;
            }
        }
    }
}
