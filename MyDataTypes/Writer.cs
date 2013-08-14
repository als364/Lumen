using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework;

namespace MyDataTypes
{
    public class Writer
    {
        private static string StringOfVector2 (Vector2 v) {
            return v.X.ToString()+" "+v.Y.ToString();
        }

        private static string StringOfVector2Array (Vector2[] v) {
            string s = "";
            for(int n=0; n<v.Length; n++) {
                s += v[n].X + " " + v[n].Y + "\n\r";
            }
            return s;
        }

        public static void WriteLevelDef(LevelDef input, string filename)
        {
            XmlTextWriter writer = new XmlTextWriter(filename, null);
            writer.Formatting = Formatting.Indented;
            writer.WriteStartDocument();
            writer.WriteStartElement("LumenContent");


            writer.WriteElementString("floorTextureLocation", input.floorTextureLocation);
            writer.WriteElementString("ambientLight", input.ambientLight.ToString());
            writer.WriteElementString("initialCourage", input.initialCourage.ToString());
            writer.WriteElementString("levelWidth", input.levelWidth.ToString());
            writer.WriteElementString("levelHeight", input.levelHeight.ToString());
            writer.WriteElementString("levelObjectsLocation", input.levelObjectsLocation);
            writer.WriteElementString("levelExitsLocation", input.levelExitsLocation);

            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();
        }
        public static void WriteObjectType(ObjectType input, string filename)
        {
            XmlTextWriter writer = new XmlTextWriter(filename, null);
            writer.Formatting = Formatting.Indented;
            writer.WriteStartDocument();
            writer.WriteStartElement("LumenContent");

            writer.WriteElementString("Type", input.Type.ToString());
            writer.WriteElementString("ImgScale", input.ImgScale.ToString());
            writer.WriteElementString("SpriteFilePath", input.SpriteFilePath);
            writer.WriteElementString("spriteWidth", input.spriteWidth.ToString());
            writer.WriteElementString("spriteHeight", input.spriteHeight.ToString());
            writer.WriteElementString("Framecount", input.Framecount.ToString());
            writer.WriteElementString("spriteCenter", StringOfVector2(input.spriteCenter));
            if (input.CanSlide) writer.WriteElementString("CanSlide", "true");
            else writer.WriteElementString("CanSlide", "false");
            if (input.CanRotate) writer.WriteElementString("CanRotate", "true");
            else writer.WriteElementString("CanRotate", "false");
            if (input.IsSwitch) writer.WriteElementString("IsSwitch", "true");
            else writer.WriteElementString("IsSwitch", "false");
            if (input.IsPlug) writer.WriteElementString("IsPlug", "true");
            else writer.WriteElementString("IsPlug", "false");
            if (input.isOutlet) writer.WriteElementString("isOutlet", "true");
            else writer.WriteElementString("isOutlet", "false");
            if (input.CastsShadows) writer.WriteElementString("CastsShadows", "true");
            else writer.WriteElementString("CastsShadows", "false");
            writer.WriteElementString("Density", input.Density.ToString());
            writer.WriteElementString("Friction", input.Friction.ToString());
            writer.WriteElementString("Restitution", input.Restitution.ToString());
            writer.WriteElementString("Facing", StringOfVector2(input.Facing));

            writer.WriteElementString("lightConvexHull", StringOfVector2Array(input.lightConvexHull));
            writer.WriteElementString("physicsConvexHull", StringOfVector2Array(input.physicsConvexHull));

            writer.WriteElementString("intensity", input.intensity.ToString());
            writer.WriteElementString("lightRadius", input.lightRadius.ToString());
            if (input.checkpoint) writer.WriteElementString("checkpoint", "true");
            else writer.WriteElementString("checkpoint", "false");

            writer.WriteStartElement("invisibleHulls");
            foreach(Vector2[] ray in input.invisibleHulls) {
                writer.WriteElementString("Item", StringOfVector2Array(ray));
            }
            writer.WriteEndElement();

            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();
        }
        public static void WriteObjectInstances(ObjectInstance[] inputs, string filename)
        {
            XmlTextWriter writer = new XmlTextWriter(filename, null);
            writer.Formatting = Formatting.Indented;
            writer.WriteStartDocument();
            writer.WriteStartElement("LumenContent");

            foreach (ObjectInstance input in inputs)
            {
                writer.WriteStartElement("Item");

                writer.WriteElementString("XMLDataFileLocation", input.XMLDataFileLocation);
                writer.WriteElementString("InitialPosition", StringOfVector2(input.InitialPosition));
                writer.WriteElementString("Orientation", input.Orientation.ToString());
                if(input.On) writer.WriteElementString("On", "true");
                else writer.WriteElementString("On", "false");

                string pwrsrc = "";
                foreach (int id in input.PowerSourceID)
                {
                    pwrsrc += id.ToString() + " ";
                }
                writer.WriteElementString("PowerSourceID", pwrsrc);

                writer.WriteStartElement("Wires");
                foreach (Vector2[] ray in input.Wires)
                {
                    writer.WriteElementString("Item", StringOfVector2Array(ray));
                }
                writer.WriteEndElement();

                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();
        }
        public static void WriteExitInstances(ExitInstance[] inputs, string filename)
        {
            XmlTextWriter writer = new XmlTextWriter(filename, null);
            writer.Formatting = Formatting.Indented;
            writer.WriteStartDocument();
            writer.WriteStartElement("LumenContent");

            foreach (ExitInstance input in inputs)
            {
                writer.WriteStartElement("Item");

                writer.WriteElementString("exitLocation", input.exitLocation);
                writer.WriteElementString("tlCorner", StringOfVector2(input.tlCorner));
                writer.WriteElementString("brCorner", StringOfVector2(input.brCorner));
                writer.WriteElementString("id", input.id.ToString());
                writer.WriteElementString("destination", input.destination.ToString());
                writer.WriteElementString("direction", input.direction.ToString());

                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();
        }

        public static void WriteSaveData(SaveData input, string filename)
        {
            XmlTextWriter writer = new XmlTextWriter(filename, null);
            writer.Formatting = Formatting.Indented;
            writer.WriteStartDocument();
            writer.WriteStartElement("LumenContent");

            writer.WriteElementString("Level", input.Level);
            if (input.Checkpoint) writer.WriteElementString("Checkpoint", "true");
            else writer.WriteElementString("Checkpoint", "false");
            writer.WriteElementString("ExitID", input.ExitID.ToString());

            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();
        }
    }
}
