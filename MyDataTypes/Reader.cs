using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace MyDataTypes
{
    /// <summary>
    /// Reads in XML data files at the specified location 
    /// and returns a class (really more of a struct)
    /// that has all of the internal information.
    /// Can be done at Runtime!!!
    /// </summary>
    public class Reader
    {
        private static Vector2[] VectorArrayOfString(string s)
        {
            List<Vector2> vecs = new List<Vector2>();
            char[] delims = {' ', '\n', '\t', '\r'};
            List<string> points = new List<string>();
            points.AddRange(s.Split(delims));
            for (int n = 0; n < points.Count; n++)
            {
                if (points[n] == "")
                {
                    points.RemoveAt(n);
                    n--;
                }
            }
            for(int n = 0; n < points.Count; n += 2) {
                vecs.Add(new Vector2(float.Parse(points[n]), 
                                     float.Parse(points[n+1])));
            }
            return vecs.ToArray();
        }

        /// <summary>
        /// turns 2 numbers separated by a space into a Vector2
        /// </summary>
        private static Vector2 VectorOfString(string s)
        {
            int index = s.IndexOf(' ');
            float x = float.Parse(s.Substring(0, index));
            float y = float.Parse(s.Substring(index + 1, s.Length - index - 1));
            return new Vector2(x, y);
        }

        public static LevelDef ParseLevelDef(string filename)
        {
            LevelDef turn = new LevelDef();
            XmlTextReader reader = new XmlTextReader(filename);
            while (reader.Read())
            {
                if (reader.Name == "floorTextureLocation")
                    turn.floorTextureLocation = reader.ReadElementContentAsString();
                else if (reader.Name == "ambientLight")
                    turn.ambientLight = reader.ReadElementContentAsFloat();
                else if (reader.Name == "initialCourage")
                    turn.initialCourage = reader.ReadElementContentAsFloat();
                else if (reader.Name == "levelWidth")
                    turn.levelWidth = reader.ReadElementContentAsFloat();
                else if (reader.Name == "levelHeight")
                    turn.levelHeight = reader.ReadElementContentAsFloat();
                else if (reader.Name == "levelObjectsLocation")
                    turn.levelObjectsLocation = reader.ReadElementContentAsString();
                else if (reader.Name == "levelExitsLocation")
                    turn.levelExitsLocation = reader.ReadElementContentAsString();
            }
            reader.Close();
            return turn;
        }

        public static ObjectType ParseObjectType(string filename) 
        {
            ObjectType turn = new ObjectType();
            XmlTextReader reader = new XmlTextReader(filename);
            Console.WriteLine(filename);
            while (reader.Read())
            {
                if (reader.Name == "Type")
                    turn.Type = reader.ReadElementContentAsInt();
                else if (reader.Name == "ImgScale")
                    turn.ImgScale = reader.ReadElementContentAsInt();
                else if (reader.Name == "SpriteFilePath")
                    turn.SpriteFilePath = reader.ReadElementContentAsString();
                else if (reader.Name == "spriteWidth")
                    turn.spriteWidth = reader.ReadElementContentAsInt();
                else if (reader.Name == "spriteHeight")
                    turn.spriteHeight = reader.ReadElementContentAsInt();
                else if (reader.Name == "Framecount")
                    turn.Framecount = reader.ReadElementContentAsInt();
                else if (reader.Name == "spriteCenter")
                    turn.spriteCenter = VectorOfString(reader.ReadElementContentAsString().Trim());
                else if (reader.Name == "CanSlide")
                    turn.CanSlide = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "CanRotate")
                    turn.CanRotate = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "IsSwitch")
                    turn.IsSwitch = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "IsPlug")
                    turn.IsPlug = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "isOutlet")
                    turn.isOutlet = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "CastsShadows")
                    turn.CastsShadows = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "Density")
                    turn.Density = reader.ReadElementContentAsFloat();
                else if (reader.Name == "Friction")
                    turn.Friction = reader.ReadElementContentAsFloat();
                else if (reader.Name == "Restitution")
                    turn.Restitution = reader.ReadElementContentAsFloat();
                else if (reader.Name == "Facing")
                    turn.Facing = VectorOfString(reader.ReadElementContentAsString());

                else if (reader.Name == "lightConvexHull")
                    turn.lightConvexHull = VectorArrayOfString(reader.ReadElementContentAsString());
                else if (reader.Name == "physicsConvexHull")
                    turn.physicsConvexHull = VectorArrayOfString(reader.ReadElementContentAsString());

                else if (reader.Name == "intensity")
                    turn.intensity = reader.ReadElementContentAsFloat();
                else if (reader.Name == "lightRadius")
                    turn.lightRadius = reader.ReadElementContentAsFloat();
                else if (reader.Name == "checkpoint")
                    turn.checkpoint = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "invisibleHulls")
                {
                    List<Vector2[]> hulls = new List<Vector2[]>();
                    if (!reader.IsEmptyElement)
                    {
                        while (reader.Read())
                        {
                            if (reader.Name == "Item")
                                hulls.Add(VectorArrayOfString(reader.ReadElementContentAsString()));
                            else if (reader.Name == "invisibleHulls" & reader.NodeType == XmlNodeType.EndElement)
                                break;
                        }
                    }
                    turn.invisibleHulls = hulls;
                }
            }
            reader.Close();
            return turn;
        }

        public static ObjectInstance[] ParseObjectInstances(string filename)
        {
            List<ObjectInstance> ls = new List<ObjectInstance>();
            XmlTextReader reader = new XmlTextReader(filename);
            while (reader.Read())
            {
                if (reader.Name == "Item")
                {
                    ObjectInstance turn = new ObjectInstance();
                    turn.Wires = new List<Vector2[]>();
                    while (reader.Read())
                    {
                        if (reader.Name == "XMLDataFileLocation")
                            turn.XMLDataFileLocation = reader.ReadElementContentAsString();
                        else if (reader.Name == "InitialPosition")
                            turn.InitialPosition = VectorOfString(reader.ReadElementContentAsString());
                        else if (reader.Name == "Orientation")
                            turn.Orientation = reader.ReadElementContentAsFloat();
                        else if (reader.Name == "On")
                            turn.On = reader.ReadElementContentAsBoolean();
                        else if (reader.Name == "PowerSourceID")
                        {
                            List<int> ids = new List<int>();
                            char[] delims = { ' ', '\t', '\n' };
                            foreach (string s in reader.ReadElementContentAsString().Trim().Split(delims))
                                ids.Add(int.Parse(s));
                            turn.PowerSourceID = ids.ToArray();
                        }
                        else if (reader.Name == "Wires")
                        {
                            if (!reader.IsEmptyElement)
                            {
                                while (reader.Read())
                                {
                                    if (reader.Name == "Item")
                                        turn.Wires.Add(VectorArrayOfString(reader.ReadElementContentAsString()));
                                    else if (reader.Name == "Wires" & reader.NodeType == XmlNodeType.EndElement)
                                        break;
                                }
                            }
                        }
                        else if (reader.Name == "Item" & reader.NodeType == XmlNodeType.EndElement)
                        {
                            ls.Add(turn);
                            break;
                        }
                    }
                }
            }
            reader.Close();    
            return ls.ToArray();
        }
        
        
        public static ExitInstance[] ParseExitInstances (string filename) {
            List<ExitInstance> ls = new List<ExitInstance>();
            XmlTextReader reader = new XmlTextReader(filename);
            while (reader.Read())
            {
                if (reader.Name == "Item")
                {
                    ExitInstance turn = new ExitInstance();

                    while (reader.Read())
                    {
                        if (reader.Name == "exitLocation")
                            turn.exitLocation = reader.ReadElementContentAsString();
                        else if (reader.Name == "tlCorner")
                            turn.tlCorner = VectorOfString(reader.ReadElementContentAsString());
                        else if (reader.Name == "brCorner")
                            turn.brCorner = VectorOfString(reader.ReadElementContentAsString());
                        else if (reader.Name == "id")
                            turn.id = reader.ReadElementContentAsInt();
                        else if (reader.Name == "destination")
                            turn.destination = reader.ReadElementContentAsInt();
                        else if (reader.Name == "direction")
                            turn.direction = reader.ReadElementContentAsInt();
                        else if (reader.Name == "Item" & reader.NodeType == XmlNodeType.EndElement)
                        {
                            ls.Add(turn);
                            break;
                        }
                    }
                }
            }
            reader.Close();
            return ls.ToArray();
        }

        public static SaveData ParseSaveData(string filename)
        {
            SaveData turn = new SaveData();
            XmlTextReader reader = new XmlTextReader(filename);
            while (reader.Read())
            {
                if (reader.Name == "Level")
                    turn.Level = reader.ReadElementContentAsString();
                else if (reader.Name == "Checkpoint")
                    turn.Checkpoint = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "ExitID")
                    turn.ExitID = reader.ReadElementContentAsInt();
            }
            reader.Close();
            return turn;
        }

        public static string[] ParseAvailableGameElements(string filename)
        {
            List<string> turn = new List<string>();
            XmlTextReader reader = new XmlTextReader(filename);
            while (reader.Read())
            {
                if (reader.Name == "Item")
                    turn.Add(reader.ReadElementContentAsString());
            }
            reader.Close();
            return turn.ToArray();
        }

        private static Dictionary<string, Texture2D> memo = new Dictionary<string, Texture2D>();

        public static Texture2D LoadImage(string filename, GraphicsDevice device)
        {
            Texture2D img;
            if (memo.TryGetValue(filename, out img))
            {
                return img;
            }
            else
            {
                FileStream stream = new FileStream(filename, FileMode.Open);
                Texture2D pic = Texture2D.FromStream(device, stream); 
                stream.Close();
                memo.Add(filename, pic);
                return pic;
            }
            
        }
    }
}
