using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MyDataTypes
{
    public class ObjectInstance
    {
        public string XMLDataFileLocation; //Type File Def.
        public Vector2 InitialPosition;
        public float Orientation;
        public bool On;

        //the first number is an ID, used for the outlets and such
        //any number after than refer to another object's ID, 
        //which we assume is a power source like an outlet or plug.
        public int[] PowerSourceID;
        public List<Vector2[]> Wires;
    }
}
