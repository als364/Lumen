using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using MyDataTypes;

namespace Lumen
{
    public class LightMapController
    {
        //private LightMap map; //um, this is kind of a silly class.

        private int width, height;

        private RenderTarget2D map;
        private GraphicsDevice device;

        private const float PI = 3.1415926535f;

        private LightMap mapModel;

        BasicEffect effect;

        private const float PENUMBRA_DIST = 1000f;

        private int mapBuffer = 100;
        private int bufferedWidth, bufferedHeight;
        

        private static Texture2D penumbraTex;

        private GraphicsDeviceManager graphics;

        public LightMapController(GraphicsDevice inDevice, LightMap inMapModel, Level lvl, GraphicsDeviceManager manager) {
            device = inDevice;
            mapModel = inMapModel;

            width = mapModel.Width;
            height = mapModel.Height;
            map = new RenderTarget2D(device, width, height);

            bufferedWidth = width + 2 * mapBuffer;
            bufferedHeight = height + 2 * mapBuffer;

            foreach (GameObject obj in lvl.GetGameObjectList())
            {
                if (obj is Light)
                {
                    Light light = (Light)obj;
                    light.RenderTarget = new RenderTarget2D(device, bufferedWidth, bufferedHeight);
                }
            }

            effect = new BasicEffect(device);

            graphics = manager;
        }

        public static void LoadContent(GraphicsDevice device)
        {
            penumbraTex = Reader.LoadImage("Content/images/penumbra.png", device);
            
        }


        /// <summary>
        /// Recalculates the light value at each point and updates LightMap to reflect this
        /// <param name="____">GameObject, LightMap, Level Controller, Light</param>
        /// </summary>
        public void UpdateLightMap(SpriteBatch spriteBatch, Level lvl)
        {
            List<GameObject> objects = lvl.GetGameObjectList();
            List<RenderTarget2D> renderTargets = new List<RenderTarget2D>();

            /*//clear mirrors
            foreach (GameObject o in objects)
            {
                if (o is Light)
                {
                    Light l = (Light)o;
                    if (l.isMirror())
                    {
                        l.ClearMirrorLists();
                    }
                }
            }*/

            //create a render target for each light
            foreach(GameObject obj in objects) 
            {
                if(obj is Light) 
                {
                    Light light = (Light) obj;
                    if (light.IsOn)
                    {

                        RenderTarget2D lightTarget = DrawLight(light, objects, spriteBatch);
                        renderTargets.Add(lightTarget);

                        Color[] data = new Color[bufferedHeight * bufferedWidth];
                        lightTarget.GetData<Color>(data);
                            
                        /*//update each mirror
                        foreach (GameObject o in objects)
                        {
                            if (o is Light && o != light)
                            {
                                Light l = (Light)o;
                                if (l.isMirror())
                                {
                                    Vector2 point = l.GetReflectionPoint();
                                    int raypos = bufferedWidth * mapBuffer + bufferedWidth * (int)point.Y + (mapBuffer + (int)point.X);
                                    if (data[raypos].R > 0)
                                    {
                                        l.AddReflectedLightSource(light.GetScreenPosition());
                                        l.AddReflectedLightIntensity(data[raypos].R);
                                    }
                                }
                            }
                        }*/
                    }
                }
            }

           

            //set correct Render Target
            device.SetRenderTarget(map);
            device.Clear(Color.Black);

            //reset "map" to completely blank pallet
            /*spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.Opaque);
            spriteBatch.Draw(pixel, new Rectangle(0, 0, width, height), Color.White);
            spriteBatch.End();*/


            //additively draw the render target for each light to the LightMap buffer "map"
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.Additive);
            foreach (RenderTarget2D image in renderTargets)
            {
                spriteBatch.Draw(image, new Vector2(0,0), 
                                  new Rectangle(mapBuffer, mapBuffer, mapBuffer+width, mapBuffer+height), Color.White);
            }
            spriteBatch.End();

            device.SetRenderTarget(null);

            //update the LightMap
            mapModel.SetMap(map);
        }

        private RenderTarget2D DrawLight(Light light, List<GameObject> objects, SpriteBatch spriteBatch)
        {
            //large use of http://archive.gamedev.net/reference/articles/article2032.asp

            //Debug.assert(intensity > 0f && intensity <= 1f);

            RenderTarget2D lightTarget = light.RenderTarget;
            device.SetRenderTarget(lightTarget);
            device.Clear(Color.Black);

            //reset "renderTarget" to completely blank pallet
            //spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.Opaque);
            //spriteBatch.Draw(pixel, new Rectangle(0, 0, width, height), Color.White);
            //spriteBatch.End();

            //do
            //{

                float intensity = light.Intensity;
                int numSubdivisions = 32;

                Color brightest = new Color(intensity, intensity, intensity, 1f);
                Color dimmest = new Color(0f, 0f, 0f, 1f);

                VertexPositionColor[] points = new VertexPositionColor[3 * numSubdivisions];

                double angleAdjustment = Math.PI / numSubdivisions;
                float lightRadius = light.LightRadius;

                Vector2 cent = light.GetScreenPosition();// +new Vector2(mapBuffer, mapBuffer);
                Vector2 center = new Vector2((cent.X - (width / 2)) / (bufferedWidth / 2),
                                             ((height / 2) - cent.Y) / (bufferedHeight / 2));



                for (int n = 0; n < numSubdivisions; n++)
                {
                    double angle = -1 * n * (2 * Math.PI / numSubdivisions);

                    points[3 * n] = new VertexPositionColor(new Vector3(center.X, center.Y, 0f), brightest);

                    Vector3 pos1 = new Vector3(center.X + lightRadius * (float)Math.Cos(angle + angleAdjustment),
                                               center.Y + lightRadius * (float)Math.Sin(angle + angleAdjustment),
                                               0);
                    Vector3 pos2 = new Vector3(center.X + lightRadius * (float)Math.Cos(angle - angleAdjustment),
                                               center.Y + lightRadius * (float)Math.Sin(angle - angleAdjustment),
                                               0);

                    points[3 * n + 1] = new VertexPositionColor(pos1, dimmest);
                    points[3 * n + 2] = new VertexPositionColor(pos2, dimmest);
                }

                //change to vertex color effect
                effect.TextureEnabled = false;
                effect.VertexColorEnabled = true;
                //setup shading and whatnot so we can DrawUserPrimitive
                effect.CurrentTechnique.Passes[0].Apply();

                //draw array of points
                device.DrawUserPrimitives(PrimitiveType.TriangleList, points, 0, numSubdivisions,
                                            VertexPositionColor.VertexDeclaration);


                //subtract shadows from Render Target
                foreach (GameObject obj in objects)
                {
                    if (light != obj && obj.CastsShadows)
                    {
                        DrawShadow(light, obj.LightConvexHull, lightTarget);
                    }
                }

                //sculpt directed light
                foreach (Vector2[] invisibleHull in light.getInvisibleHulls())
                {
                    DrawShadow(light, invisibleHull, lightTarget);
                }

            //} while (light.isMirror() && !light.LightSourcesEmpty());

            //makes sure when you exit function, no longer drawing to this target
            //might be removed?
            device.SetRenderTarget(null);

            return lightTarget;
        }


        private void DrawShadow(Light lightSource, Vector2[] hull, RenderTarget2D lightTarget)
        {
            //will be CCW
            Vector2[] vertices = hull;


            //Copied from Orangy Tang

            bool[] backFacing = new bool[vertices.Length];

            //compute facing of each edge, using N*L
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector2 firstVertex = vertices[i];
                Vector2 secondVertex = vertices[(i + 1) % vertices.Length];
                Vector2 middle = (firstVertex + secondVertex) / 2;

                Vector2 L = lightSource.GetScreenPosition() - middle;

                Vector2 N = new Vector2(-(secondVertex.Y - firstVertex.Y), secondVertex.X - firstVertex.X);

                backFacing[i] = Vector2.Dot(N, L) < 0;
            }

            //find beginning and ending vertices which
            //belong to the shadow
            int startingIndex = 0;
            int endingIndex = 0;
            for (int i = 0; i < vertices.Length; i++)
            {
                int currentEdge = i;
                int nextEdge = (i + 1) % vertices.Length;

                if (backFacing[currentEdge] && !backFacing[nextEdge])
                    startingIndex = nextEdge;//endingIndex = nextEdge;

                if (!backFacing[currentEdge] && backFacing[nextEdge])
                    endingIndex = nextEdge;//startingIndex = nextEdge;
            }



            //draw penumbra fins
            Vector2 firstUmbraPos;
            {
                Vector2 lightCent = lightSource.GetScreenPosition();

                VertexPositionTexture[] penumbra1 = new VertexPositionTexture[3];

                Vector2 startPos = vertices[startingIndex];

                Vector2 lightRay = lightCent - startPos;

                Vector2 normalInner, normalOuter;
                bool quad1or3 = lightRay.X * lightRay.Y < 0;
                if (quad1or3)
                {
                    normalInner = new Vector2(-lightRay.X, lightRay.Y);
                    normalOuter = new Vector2(lightRay.X, -lightRay.Y);
                }
                else
                {
                    normalInner = new Vector2(lightRay.X, -lightRay.Y);
                    normalOuter = new Vector2(-lightRay.X, lightRay.Y);
                } 
                normalInner.Normalize();
                normalInner = lightSource.LightRadius * normalInner;
                Vector2 innerPos = lightCent + normalInner;
                Vector2 penumbra = startPos - innerPos;
                penumbra.Normalize();
                Vector2 penumbraPos = startPos + penumbra * PENUMBRA_DIST;
                
                normalOuter.Normalize();
                normalOuter = lightSource.LightRadius * normalOuter;
                Vector2 outerPos = lightCent + normalOuter;
                Vector2 umbra = startPos - outerPos;
                umbra.Normalize();
                Vector2 umbraPos = startPos + umbra * PENUMBRA_DIST;

                firstUmbraPos = umbraPos;

                penumbra1[0] = new VertexPositionTexture(new Vector3(startPos, 0), new Vector2(0, 1));
                penumbra1[1] = new VertexPositionTexture(new Vector3(penumbraPos, 0), new Vector2(0, 0));
                penumbra1[2] = new VertexPositionTexture(new Vector3(umbraPos, 0), new Vector2(1, 0));

                //convert each vertex to draw user primitives axis system
                for (int n = 0; n < penumbra1.Length; n++)
                {
                    Vector3 oldPos = penumbra1[n].Position;
                    penumbra1[n].Position =
                        new Vector3(((oldPos.X) - (width / 2)) / (bufferedWidth / 2),
                                ((height / 2) - (oldPos.Y)) / (bufferedHeight / 2), 0); 
                        //new Vector3((oldPos.X - (width / 2)) / (width / 2), ((height / 2) - oldPos.Y) / (height / 2), 0);
                }

                //set effect to texture
                effect.VertexColorEnabled = false;
                effect.TextureEnabled = true;
                effect.Texture = penumbraTex;


                //setup shading and whatnot so we can DrawUserPrimitive
                effect.CurrentTechnique.Passes[0].Apply();

                device.DrawUserPrimitives(PrimitiveType.TriangleList, penumbra1, 0, 1, VertexPositionTexture.VertexDeclaration);
            }


            //draw second fin
            Vector2 secondUmbraPos;
            {
                Vector2 lightCent = lightSource.GetScreenPosition();

                VertexPositionTexture[] penumbra2 = new VertexPositionTexture[3];

                Vector2 startPos = vertices[endingIndex];

                Vector2 lightRay = lightCent - startPos;

                Vector2 normalInner, normalOuter;
                bool quad1or3 = lightRay.X * lightRay.Y < 0;
                if (quad1or3)
                {
                    normalInner = new Vector2(lightRay.X, -lightRay.Y);
                    normalOuter = new Vector2(-lightRay.X, lightRay.Y);
                }
                else
                {
                    normalInner = new Vector2(-lightRay.X, lightRay.Y);
                    normalOuter = new Vector2(lightRay.X, -lightRay.Y);
                } 
                normalInner.Normalize();
                normalInner = lightSource.LightRadius * normalInner;
                Vector2 innerPos = lightCent + normalInner;
                Vector2 penumbra = startPos - innerPos;
                penumbra.Normalize();
                Vector2 penumbraPos = startPos + penumbra * PENUMBRA_DIST;

                normalOuter.Normalize();
                normalOuter = lightSource.LightRadius * normalOuter;
                Vector2 outerPos = lightCent + normalOuter;
                Vector2 umbra = startPos - outerPos;
                umbra.Normalize();
                Vector2 umbraPos = startPos + umbra * PENUMBRA_DIST;

                secondUmbraPos = umbraPos;

                penumbra2[0] = new VertexPositionTexture(new Vector3(startPos, 0), new Vector2(0, 1));
                penumbra2[1] = new VertexPositionTexture(new Vector3(umbraPos, 0), new Vector2(1, 0));
                penumbra2[2] = new VertexPositionTexture(new Vector3(penumbraPos, 0), new Vector2(0, 0));

                //convert each vertex to draw user primitives axis system
                for (int n = 0; n < penumbra2.Length; n++)
                {
                    Vector3 oldPos = penumbra2[n].Position;
                    penumbra2[n].Position =
                        new Vector3(((oldPos.X) - (width / 2)) / (bufferedWidth / 2),
                                ((height / 2) - (oldPos.Y)) / (bufferedHeight / 2), 0); 
                }

                //set effect to texture
                effect.VertexColorEnabled = false;
                effect.TextureEnabled = true;
                effect.Texture = penumbraTex;


                //setup shading and whatnot so we can DrawUserPrimitive
                effect.CurrentTechnique.Passes[0].Apply();

                device.DrawUserPrimitives(PrimitiveType.TriangleList, penumbra2, 0, 1, VertexPositionTexture.VertexDeclaration);
            }



            //number of vertices that are in the shadow
            int shadowVertexCount;

            if (endingIndex < startingIndex)
                shadowVertexCount = startingIndex - endingIndex + 1;
            else
                shadowVertexCount = vertices.Length + 1 - startingIndex + endingIndex;

            VertexPositionColor[] shadowVertices = new VertexPositionColor[shadowVertexCount * 2];

            //create a triangle strip that has the shape of the shadow ("on screen")
            int currentIndex = startingIndex;
            for (int n=0; n < shadowVertices.Length; n += 2)
            {
                //one vertex on the hull
                Vector3 vertexPos = new Vector3(vertices[currentIndex], 0);
                shadowVertices[n] = new VertexPositionColor(vertexPos, Color.Black);//Transparent);

                //one extruded by the light direction
                shadowVertices[n + 1] = new VertexPositionColor();
                shadowVertices[n + 1].Color = Color.Black;//Transparent;
                Vector3 L2P;
                if(currentIndex == startingIndex) {
                    L2P = new Vector3(firstUmbraPos, 0) - vertexPos;
                }
                else if (currentIndex == endingIndex)
                {
                    L2P = new Vector3(secondUmbraPos, 0) - vertexPos;
                }
                else
                {
                    L2P = vertexPos - new Vector3(lightSource.GetScreenPosition(), 0);
                }
                L2P.Normalize();
                shadowVertices[n + 1].Position = new Vector3(lightSource.GetScreenPosition(), 0) + L2P * 50000;

                //currentIndex = (currentIndex + 1) % vertices.Length;
                currentIndex--;
                if (currentIndex < 0) currentIndex += vertices.Length;
            }

            //convert each vertex to draw user primitives axis system
            for (int n = 0; n < shadowVertices.Length; n++)
            {
                Vector3 oldPos = shadowVertices[n].Position;
                shadowVertices[n].Position = 
                    new Vector3(((oldPos.X) - (width / 2)) / (bufferedWidth / 2),
                                ((height / 2) - (oldPos.Y)) / (bufferedHeight / 2), 0); 
                //new Vector3((oldPos.X - (width / 2)) / (width / 2), ((height / 2) - oldPos.Y) / (height / 2), 0);
            }

            //change to vertex color effect
            effect.TextureEnabled = false;
            effect.VertexColorEnabled = true;
            //setup shading and whatnot so we can DrawUserPrimitive
            effect.CurrentTechnique.Passes[0].Apply();

            //draw array of points
            device.DrawUserPrimitives(PrimitiveType.TriangleStrip, shadowVertices, 0, shadowVertices.Length - 2,
                                        VertexPositionColor.VertexDeclaration);




        }

        /*public int MapBuffer
        {
            get
            {
                return mapBuffer;
            }
        }*/
    }
}
