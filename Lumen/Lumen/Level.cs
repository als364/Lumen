using System;
using System.Collections.Generic;
using Box2DX.Collision;
using Box2DX.Common;
using Box2DX.Dynamics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MyDataTypes;
using System.IO;

namespace Lumen{

    public class Level{

        /*Level
Level is a class that owns the set of GameObjects contained in the level, stores initial conditions for the level, and keeps track of entrances and exits to the level.
Class: Level	
Superclass: Object
Responsibilities
Collaborators
knows GameObjects contained in level	GameObject
knows locations of entrances and exits	Exit
knows file path of XML level file	*/

        //list of GameObjects in level... GameObject extending objects.
        private List<GameObject> gameObjects;
        //list of exits in level... Exit objects.
        private List<Exit> exits;
        //filePath to level data (an xml file). 
        private string filePath;
        public string FilePath { get { return filePath; } }

        private String backgroundImagePath = "images/background";
        private Texture2D backgroundImage;

        //Create a world in which bodies can sleep if they want to save perf... Am assuming they'll wake up for collisions.
        private World physicsWorld;

        private AABB aabb;

        private Vector2 screenPosition;

        private float ambientLight;
        public float AmbientLight { get { return ambientLight; } }
        private float initCourage;
        public float InititalCourage { get { return initCourage; } }

        private Texture2D floor;
        public Texture2D Floor { get { return floor; } }

        //save data fields
        private string floorImageLocation;
        private string xmlLocation;

        public World World{
            get{
                return physicsWorld;
            }
        }

        //
        public Level(string xmlLevelDefLocation, GraphicsDevice device) {
            string suffix = ".xml";
            if (File.Exists("Content/xml/" + xmlLevelDefLocation + "-sav.xml"))
            {
                suffix = "-sav.xml";
            }
            LevelDef lvldef = Reader.ParseLevelDef("Content/xml/" + xmlLevelDefLocation + suffix);//content.Load<LevelDef>("xml/"+xmlLevelDefLocation);

            aabb = new AABB();
            aabb.LowerBound.Set(0, 0);
            aabb.UpperBound.Set(lvldef.levelWidth, lvldef.levelHeight);

            ambientLight = lvldef.ambientLight;
            initCourage = lvldef.initialCourage;
            floor = Reader.LoadImage("Content/" + lvldef.floorTextureLocation + ".png", device);//content.Load<Texture2D>(lvldef.floorTextureLocation);


            
            //make the world with the specfied edges... have 0 gravity (it's top down.. gravity doesn't exist.. :P) and allow 
            //bodies to sleep themselves for performance. We're assuming (as Box2D claims) that they'll wake up for collisions.
            physicsWorld = new World(aabb, new Vec2(0f, 0f), false);



            //add objects to world
            ObjectInstance[] objs = Reader.ParseObjectInstances(
                                        "Content/" + lvldef.levelObjectsLocation + suffix);//content.Load<ObjectInstance[]>(lvldef.levelObjectsLocation);


            gameObjects = new List<GameObject>();
            foreach (ObjectInstance oi in objs)
            {
                ObjectType objdef = Reader.ParseObjectType("Content/" + oi.XMLDataFileLocation + ".xml");//content.Load<ObjectType>(oi.XMLDataFileLocation);
                if (objdef.Type == 0)
                {
                    gameObjects.Add(new GameObject(oi, objdef, device, this));
                }
                else if (objdef.Type == 1)
                {
                    gameObjects.Add(new Light(oi, objdef, device, this));
                }
                else if (objdef.Type == 2)
                {
                    gameObjects.Add(new Girl(oi, objdef, device, this));
                }
            }

            //add exits to world
            ExitInstance[] exs = Reader.ParseExitInstances("Content/" + lvldef.levelExitsLocation + suffix);//content.Load<ExitInstance[]>(lvldef.levelExitsLocation);

            exits = new List<Exit>();
            foreach (ExitInstance ei in exs)
            {
                exits.Add(new Exit(ei.tlCorner, ei.brCorner, ei.exitLocation, ei.id, ei.destination, ei.direction));
            }
            
            //store information for saving later
            xmlLocation = xmlLevelDefLocation;
            floorImageLocation = lvldef.floorTextureLocation;
        }

        /*public void LoadContent(ContentManager content){
            backgroundImage = content.Load<Texture2D>(backgroundImagePath);
            foreach (GameObject obj in gameObjects)
            {
                obj.LoadContent(content);
            }
        }*/

        //gets the list of GameObjects in the level
        public List<GameObject> GetGameObjectList(){
            return gameObjects;
        }
	
	    //Adds obj to ObjectList
        public void AddToObjectList(GameObject obj){
            gameObjects.Add(obj);
        }

        //deletes obj from ObjectList
        public void DeleteFromObjectList(GameObject obj){
            gameObjects.Remove(obj);
        }
	        
        //gets the list of exits
        public List<Exit> GetExitList(){
            return exits;
        }
	        
        //Adds exit to the list of exits
        public void AddToExitList(Exit exit){
            exits.Add(exit);
        }
	        
        //gets level file path
        public string GetLevelFilePath(){
            return filePath;
        }
	        
        //sets level file path
        public void SetLevelFilePath(String TheFilePath){
            filePath = TheFilePath;
        }

        public AABB AABB
        {
            get
            {
                return aabb;
            }
            set
            {
                aabb = value;
            }
        }

        //Expressed in meters
        public Vector2 ScreenPosition {
            get{
                return screenPosition;
            }
            set{
                screenPosition = value;
            }       
        }

        public Texture2D BackgroundImage
        {
            get
            {
                return backgroundImage;
            }
        }

        public LevelDef Save(float courage)
        {
            LevelDef lvlDef = new LevelDef();
            lvlDef.floorTextureLocation = floorImageLocation;
            lvlDef.ambientLight = this.ambientLight;
            lvlDef.initialCourage = courage;
            lvlDef.levelHeight = aabb.UpperBound.Y;
            lvlDef.levelWidth = aabb.UpperBound.X;
            lvlDef.levelObjectsLocation = "xml/" + xmlLocation + "Objects";
            lvlDef.levelExitsLocation = "xml/" + xmlLocation + "Exits";
            return lvlDef;
        }
    }
}
