using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Lumen{

	public class GirlView : ObjectView{

        Girl objectToDraw;

        public GirlView(Girl girl) : base(girl)
        {
            objectToDraw = girl;
        }

        // Draw this object
        public override void Draw(SpriteBatch spriteBatch, LightMap map)
        {
            int dir = 0;
            //going down
            if (objectToDraw.Facing.Y > 0) 
            {
                dir = 0;
            }
            //going up
            if (objectToDraw.Facing.Y < 0)
            {
                dir = 1;
            }
            //going left
            if (objectToDraw.Facing.X < 0) 
            {
                dir = 2;
            }
            //Going right
            if (objectToDraw.Facing.X > 0)
            {
                dir = 3;
            }

            int tint = map.GetLightValue((int)viewedObject.GetScreenPosition().X, (int)viewedObject.GetScreenPosition().Y);
            if (tint < LevelController.DARKNESS)
                tint = LevelController.DARKNESS;

            //Position - parent.getScreenPosition
            spriteBatch.Draw(objectToDraw.SpriteSheet, objectToDraw.GetScreenPosition(),
                new Rectangle((int)objectToDraw.Framesize.X * objectToDraw.AnimationCycleState, (int)objectToDraw.Framesize.Y * dir, 
                    (int)objectToDraw.Framesize.X, (int)objectToDraw.Framesize.Y), new Color(tint, tint, tint), 0f,
                new Vector2(objectToDraw.spriteCenter.X, objectToDraw.spriteCenter.Y), GameObject.worldScale/objectToDraw.ImgScale, SpriteEffects.None, 0f);
            
        }



        ///////////////////// old stuff starts here
        /*
        /// <summary>
        /// Draw the center of this object at its position relative to the SCREEN.
        /// Also scale appropriately.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime){
            spriteLoc = objectToDraw.GetSpriteFilePath();
            GraphicsDevice gdevice = new GraphicsDevice();
            SpriteBatch spriteBatch = new SpriteBatch();
            spriteBatch.Begin();
            Vector2 temp = new Vector2(objectImage.Width/2, objectImage.Height/2);
            spriteBatch.Draw(objectImage, Position-parent.getScreenPosition(), null, Color.White, 0f, 
                             , SCALE, SpriteEffects.None, 0f);
            spriteBatch.End();
        }


        /// <summary>
        /// Changes the object's image to the correct action.
        /// Updates the animationCycleState to increment appropriately.
        /// </summary>
        private void animate(Object_State sprite)
        {
            int dir = 0;
            //Walking up
            if (objectToDraw.Orientation == new Vector2(0, -1))
            {
                dir = 0;
            }
            //going down
            if (objectToDraw.Orientation == new Vector2(0, +1))
            {
                dir = 1;
            }
            //going right
            if (objectToDraw.Orientation == new Vector2(1, 0))
            {
                dir = 2;
            }
            //Going left
            if (objectToDraw.Orientation == new Vector2(-1, 0))
            {
                dir = 3;
            }

            //set total number of frames in the animation

            if (dir <= 1) //up or down
                animationCycleState[1] = 8;
            else //left or right
                animationCycleState[1] = 6;

            //rotating only has 4 frames
            if (sprite == OBJECT_ACTION.ROTATING_CW || sprite == OBJECT_ACTION.ROTATING_CCW)
                animationCycleState[1] = 4;
            //not moving
            else if (sprite == OBJECT_ACTION.STANDING || sprite == OBJECT_ACTION.HOLDING)
                animationCycleState[1] = 1;

            //increment frame of animation
            animationCycleState[0] = (animationCycleState[0] + 1) % animationCycleState[1];

            //choose correct image from matrix
            objectImage = animations[dir, (int)sprite];
        }

        

        // Draw this object
        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(objectImage, Position - parent.getScreenPosition(),
                new Rectangle(SPRITE_WIDTH * animationCycleState[0], 0, SPRITE_WIDTH, SPRITE_WIDTH), Color.White, 0f,
                new Vector2(SPRITE_WIDTH / 2, SPRITE_WIDTH / 2), SCALE, SpriteEffects.None, 0f);
            spriteBatch.End();
        }
        */
    }
}
