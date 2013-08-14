using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LumenLevelEditor
{
    /// <summary>
    /// Field extends Button
    /// A text box, when pressed, allows user to type.
    /// </summary>
    public class Field : Button
    {
        protected FIELD_TYPE fType;

        protected float value;
        protected string visibleValue;

        protected bool waitingForKeyRelease;

        public Field(FIELD_TYPE type, Rectangle r, String s, float val) : base(BUTTON_TYPE.FIELD, r, s)
        {
            fType = type;
            value = val;
            visibleValue = value.ToString();

            waitingForKeyRelease = false;
        }

        /*
        -Field to specify ambient light
        -button or field to change overall level size
        -courage field represents starting courage?
         */
        public enum FIELD_TYPE
        {
            AMBIENT,
            LEVEL_WIDTH,
            LEVEL_HEIGHT,
            INIT_COURAGE,

            EXIT_SRC,
            EXIT_SINK,

            LVL_DEST
        }

        public void ClearField()
        {
            visibleValue = "";
        }

        /// <summary>
        /// input keyboard data
        /// </summary>
        /// <param name="ks"></param>
        public virtual void Typed(KeyboardState ks, Engine e) 
        {
            if(waitingForKeyRelease && ks.GetPressedKeys().Count()==0) 
            {
                waitingForKeyRelease = false;
            } 
            else if(!waitingForKeyRelease && ks.GetPressedKeys().Count() > 0)
            {
                waitingForKeyRelease = true;
                if (ks.IsKeyDown(Keys.D1))
                {
                    visibleValue += '1';
                }
                else if (ks.IsKeyDown(Keys.D2))
                {
                    visibleValue += '2';
                }
                else if (ks.IsKeyDown(Keys.D3))
                {
                    visibleValue += '3';
                }
                else if (ks.IsKeyDown(Keys.D4))
                {
                    visibleValue += '4';
                }
                else if (ks.IsKeyDown(Keys.D5))
                {
                    visibleValue += '5';
                }
                else if (ks.IsKeyDown(Keys.D6))
                {
                    visibleValue += '6';
                }
                else if (ks.IsKeyDown(Keys.D7))
                {
                    visibleValue += '7';
                }
                else if (ks.IsKeyDown(Keys.D8))
                {
                    visibleValue += '8';
                }
                else if (ks.IsKeyDown(Keys.D9))
                {
                    visibleValue += '9';
                }
                else if (ks.IsKeyDown(Keys.D0))
                {
                    visibleValue += '0';
                }
                else if (ks.IsKeyDown(Keys.OemPeriod))
                {
                    visibleValue += '.';
                }
                else if (ks.IsKeyDown(Keys.OemMinus)) 
                {
                    visibleValue += '-';
                }
                else if (ks.IsKeyDown(Keys.Back))
                {
                    visibleValue = visibleValue.Substring(0, visibleValue.Length - 1);
                }
                else if (ks.IsKeyDown(Keys.Enter))
                {
                    value = float.Parse(visibleValue);
                    e.SetActiveField(null);
                    switch (fType)
                    {
                        case FIELD_TYPE.AMBIENT:
                            e.AmbientLight = value;
                            break;
                        case FIELD_TYPE.INIT_COURAGE:
                            e.InitialCourage = value;
                            break;
                        case FIELD_TYPE.LEVEL_HEIGHT:
                            e.LevelHeight = value;
                            break;
                        case FIELD_TYPE.LEVEL_WIDTH:
                            e.LevelWidth = value;
                            break;
                        case FIELD_TYPE.EXIT_SRC:
                            e.ExitOrigin = (int) value;
                            break;
                        case FIELD_TYPE.EXIT_SINK:
                            e.ExitDestination = (int)value;
                            break;
                    }
                }
                else if (ks.IsKeyDown(Keys.Tab))
                {
                    Undo();
                    e.SetActiveField(null);
                }
            }
            
            
        }

        public virtual void Undo()
        {
            visibleValue = value.ToString();
        }

        public override void Draw(SpriteBatch spriteBatch, Texture2D tex, SpriteFont font)
        {
            spriteBatch.Draw(tex, position, Color.White);
            spriteBatch.DrawString(font, text+": "+visibleValue, 
                                    new Vector2(position.Location.X, position.Location.Y), Color.Black);
        }
    }
}
