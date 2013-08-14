using System;
using Microsoft.Xna.Framework;
using MyDataTypes;
using System.Diagnostics;

namespace Lumen{

    /// <summary>
    /// Exit is a class that keeps track of where the entrances/exits to a level are, 
    /// and what level that entrance/exit takes you to. 
    /// It is called Exit, despite it representing both entrances (to tents) and exits (from tents), 
    /// because in any case being within an exit causes you to exit the level.
    /// </summary>
    public class Exit {

        //location of the exit
        private Vector2 tlCorner, brCorner;
        //level/where the exit takes you
        private string exitDestination;
        public string ExitDestination { get { return exitDestination; } }

        private int id;
        public int ID { get { return id; } }
        private int destID;
        public int DestinationID { get { return destID; } }

        private int direction;

        private const float NUDGE_DIST = 1f;

        public Exit(Vector2 tl, Vector2 br, string dest, int src, int sink, int dir)
        {
            tlCorner = tl;
            brCorner = br;
            exitDestination = dest;
            id = src;
            destID = sink;
            direction = dir;
        }

        public bool Contains(Vector2 pos)
        {
            return pos.X >= tlCorner.X && pos.X <= brCorner.X && pos.Y >= tlCorner.Y && pos.Y <= brCorner.Y;
        }

        /// <summary>
        /// Turns this Exit into an ExitInstance
        /// </summary>
        public ExitInstance Compress()
        {
            ExitInstance anInstance = new ExitInstance();
            anInstance.exitLocation = this.ExitDestination;
            anInstance.tlCorner = this.tlCorner;
            anInstance.brCorner = this.brCorner;
            anInstance.id = this.id;
            anInstance.destination = this.destID;
            anInstance.direction = this.direction;
            return anInstance;
        }


        /// <summary>
        /// Returns the world position at which the girl should begin this level.
        /// </summary>
        public Vector2 SpawnPosition()
        {
            switch (direction)
            {
                case 0:
                    return new Vector2((tlCorner.X / 2 + brCorner.X / 2), brCorner.Y + NUDGE_DIST);
                case 1:
                    return new Vector2((tlCorner.X / 2 + brCorner.X / 2), tlCorner.Y - NUDGE_DIST);
                case 2:
                    return new Vector2((tlCorner.X - NUDGE_DIST), (tlCorner.Y / 2 + brCorner.Y / 2));
                case 3:
                    return new Vector2((brCorner.X + NUDGE_DIST), (tlCorner.Y / 2 + brCorner.Y / 2));
                default:
                    Debug.Assert(false, "Exit has an invalid direction");
                    return new Vector2();
            }
        }
        
    }
}
