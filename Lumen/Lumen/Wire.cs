using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box2DX;
using Box2DX.Collision;
using Box2DX.Common;
using Box2DX.Dynamics;
using Box2DX.Dynamics.Controllers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;
using MyDataTypes;

namespace Lumen
{
    public class Wire
    {
        public List<Body> segments;
        public Level level;
        private List<Joint> joints;

        public Wire(Level lvl, Vector2[] points, GameObject light, GameObject powerSource)
        {
            level = lvl;
            segments = new List<Body>();
            /*Vector2 lowestLightPoint = new Vector2(light.Position.X, 0);
            Vector2 lowestPlugPoint = new Vector2(powerSource.Position.X, 0);
            for (int i = 0; i < light.PhysicsConvexHull.Length; i++)
            {
                if (light.PhysicsConvexHull[i].Y >= lowestLightPoint.Y)
                {
                    lowestLightPoint.Y = light.PhysicsConvexHull[i].Y;
                }
            }
            for (int i = 0; i < powerSource.PhysicsConvexHull.Length; i++)
            {
                if (powerSource.PhysicsConvexHull[i].Y >= lowestPlugPoint.Y)
                {
                    lowestPlugPoint.Y = powerSource.PhysicsConvexHull[i].Y;
                }
            }
            points[0] = lowestLightPoint;
            points[points.Length - 1] = lowestPlugPoint;*/
            points[0] = light.Position;
            points[points.Length - 1] = powerSource.Position;
            joints = new List<Joint>();
            for (int i = 0; i < points.Length - 1; i++)
            {
                BodyDef bodydef = new BodyDef();
                bodydef.Position.Set(points[i].X / 2f + points[i+1].X / 2f, points[i].Y / 2f + points[i+1].Y / 2f);
                bodydef.AllowSleep = true;
                bodydef.FixedRotation = false;
                bodydef.LinearDamping = 10.0f;

                float vert = points[i + 1].Y - points[i].Y;
                float horiz = points[i + 1].X - points[i].X;

                bodydef.Angle = (float)System.Math.Atan2(vert, horiz);

                float width = (points[i+1] - points[i]).Length() / 2f;
                float height = .05f;

                PolygonDef polydef = new PolygonDef();
                polydef.SetAsBox(width, height);
                polydef.Filter.GroupIndex = -1;
                polydef.Density = 1.0f;
                polydef.Friction = 0.0f;
                polydef.Restitution = 0.0f;
                Body body = lvl.World.CreateBody(bodydef);
                body.CreateShape(polydef);
                body.SetMassFromShapes();
                segments.Add(body);
            }
            for (int i = 0; i < points.Length; i++)
            {
                if (i == 0)
                {
                    RevoluteJointDef jointdef = new RevoluteJointDef();
                    jointdef.Initialize(light.Body, segments[i], new Vec2(points[i].X, points[i].Y));
                    joints.Add(lvl.World.CreateJoint(jointdef));
                }
                else if (i == points.Length - 1)
                {
                    RevoluteJointDef jointdef = new RevoluteJointDef();
                    jointdef.Initialize(segments[i-1], powerSource.Body, new Vec2(points[i].X, points[i].Y));
                    joints.Add(lvl.World.CreateJoint(jointdef));
                }
                else
                {
                    RevoluteJointDef jointdef = new RevoluteJointDef();
                    jointdef.Initialize(segments[i-1], segments[i], new Vec2(points[i].X, points[i].Y));
                    joints.Add(lvl.World.CreateJoint(jointdef));
                }
            }
        }

        public Vector2 GetScreenPosition(Body body)
        {
            return (new Vector2(body.GetPosition().X, body.GetPosition().Y) - level.ScreenPosition) * GameObject.worldScale;
        }

        public void ChangeSource(GameObject obj) {
            Joint j = joints[joints.Count - 1];
            joints.RemoveAt(joints.Count - 1);
            level.World.DestroyJoint(j);
            RevoluteJointDef jointdef = new RevoluteJointDef();
            jointdef.Initialize(segments[segments.Count-1], obj.Body, new Vec2(obj.Position.X, obj.Position.Y));
            joints.Add(level.World.CreateJoint(jointdef));
        }

        public List<Vector2> Points
        {
            get
            {
                List<Vector2> points = new List<Vector2>();
                //points.Add(new Vector2(joints[0].Anchor1.X, joints[0].Anchor1.Y));
                foreach (Joint j in joints)
                {
                    points.Add(new Vector2(j.Anchor2.X, j.Anchor2.Y));
                }
                return points;
            }
        }

        public void Draw(Texture2D wiretexture, SpriteBatch spriteBatch)
        {
            foreach (Body segment in this.segments)
            {
                Vector2 center = new Vector2(this.GetScreenPosition(segment).X, this.GetScreenPosition(segment).Y);
                float width = ((PolygonShape)segment.GetShapeList()).GetVertices()[2].X * 2f * GameObject.worldScale + 5f;
                float height = ((PolygonShape)segment.GetShapeList()).GetVertices()[2].Y * 2f * GameObject.worldScale;
                Rectangle dest = new Rectangle((int)(center.X - width / 2f), (int)(center.Y - height / 2f),
                                               (int)width, (int)height);
                spriteBatch.Draw(wiretexture, dest, null, Microsoft.Xna.Framework.Color.White, segment.GetAngle(),
                    new Vector2(wiretexture.Width / 2, wiretexture.Height / 2), SpriteEffects.None, 0f);
            }
        }

    }
}
