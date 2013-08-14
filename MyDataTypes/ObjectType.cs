using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MyDataTypes
{
    public class ObjectType
    {
        public int Type;//0 is object, 1 is light, 2 is girl
        
        public int ImgScale;
        public string SpriteFilePath;
        public int spriteWidth;
        public int spriteHeight;
        public int Framecount;
        public Vector2 spriteCenter;
        public bool CanSlide;
        public bool CanRotate;
        public bool IsSwitch;
        public bool IsPlug;
        public bool isOutlet;
        public bool isWire;
        public bool CastsShadows;
        public float Density;
        public float Friction;
        public float Restitution;
        public Vector2 Facing;

        public Vector2[] lightConvexHull;
        public Vector2[] physicsConvexHull;

        public float intensity;
        public float lightRadius;
        public bool checkpoint;

        public List<Vector2[]> invisibleHulls;
    }
}
