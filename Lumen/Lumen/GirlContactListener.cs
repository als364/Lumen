using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box2DX.Collision;
using Box2DX.Common;
using Box2DX.Dynamics;
using Box2DX.Dynamics.Controllers;

namespace Lumen
{
    class GirlContactListener : ContactListener
    {
        /// <summary>
        /// Called when a contact point is added. This includes the geometry
        /// and the forces.
        /// </summary>
        public override void Add(ContactPoint point)
        {
            //Console.WriteLine("Contact");
            if ((point.Shape1.GetBody().GetUserData() is Girl && point.Shape2.GetBody().GetUserData() is GameObject))
            {
                Girl girl = (Girl)point.Shape1.GetBody().GetUserData();
                GameObject adj = (GameObject)point.Shape2.GetBody().GetUserData();
                /*List<GameObject> adjlist = girl.Adjacent;
                if (adj.CanSlide || adj.CanRotate)
                {
                    adjlist.Add(adj);
                }
                girl.Adjacent = adjlist;*/
                girl.Adjacent.Add(adj);
                //Console.WriteLine("Size: " + girl.Adjacent.Count);
            }
            else if (point.Shape2.GetBody().GetUserData() is Girl && point.Shape1.GetBody().GetUserData() is GameObject)
            {
                Girl girl = (Girl)point.Shape2.GetBody().GetUserData();
                GameObject adj = (GameObject)point.Shape1.GetBody().GetUserData();
                /*List<GameObject> adjlist = girl.Adjacent;
               if (adj.CanSlide || adj.CanRotate)
               {
                   adjlist.Add(adj);
               }
               girl.Adjacent = adjlist;*/
                girl.Adjacent.Add(adj);
                //Console.WriteLine("Size: " + girl.Adjacent.Count);
            }
        }

        public override void Remove(ContactPoint point)
        {
            //Console.WriteLine("End Contact");
            if (point.Shape1.GetBody().GetUserData() is Girl && point.Shape2.GetBody().GetUserData() is GameObject)
            {
                Girl girl = (Girl)point.Shape1.GetBody().GetUserData();
                GameObject adj = (GameObject)point.Shape2.GetBody().GetUserData();
                /*List<GameObject> adjlist = girl.Adjacent;
                if (adj.CanSlide || adj.CanRotate)
                {
                    adjlist.Remove(adj);
                }
                girl.Adjacent = adjlist;*/
                girl.Adjacent.Remove(adj);
                //Console.WriteLine("Size: " + girl.Adjacent.Count);
            }
            else if (point.Shape2.GetBody().GetUserData() is Girl && point.Shape1.GetBody().GetUserData() is GameObject)
            {
                Girl girl = (Girl)point.Shape2.GetBody().GetUserData();
                GameObject adj = (GameObject)point.Shape1.GetBody().GetUserData();
                /*List<GameObject> adjlist = girl.Adjacent;
                if (adj.CanSlide || adj.CanRotate)
                {
                    adjlist.Remove(adj);
                }
                girl.Adjacent = adjlist;*/
                girl.Adjacent.Remove(adj);
                //Console.WriteLine("Size: " + girl.Adjacent.Count);
            }
        }
    }
}
