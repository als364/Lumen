using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MyDataTypes
{
    public class ExitInstance
    {
        public string exitLocation;
        public Vector2 tlCorner;
        public Vector2 brCorner;
        public int id;
        public int destination;
        public int direction; //down = 0, up = 1, left = 2, right = 3
    }
}
