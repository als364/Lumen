using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace LumenLevelEditor
{
    public class StringField : Field
    {
        protected string sValue;

        public StringField(FIELD_TYPE type, Rectangle r, String s, string val) : base(type, r, s, 0f)
        {
            sValue = val;
            visibleValue = sValue;
        }

        /// <summary>
        /// input keyboard data
        /// </summary>
        /// <param name="ks"></param>
        public override void Typed(KeyboardState ks, Engine e)
        {
            if (waitingForKeyRelease && ks.GetPressedKeys().Count() == 0)
            {
                waitingForKeyRelease = false;
            }
            else if (!waitingForKeyRelease && ks.GetPressedKeys().Count() > 0)
            {
                waitingForKeyRelease = true;
                Keys[] pressed = ks.GetPressedKeys();
                
                if (ks.IsKeyDown(Keys.Back) && visibleValue.Length >= 1)
                {
                    visibleValue = visibleValue.Substring(0, visibleValue.Length - 1);
                }
                else if (ks.IsKeyDown(Keys.Enter))
                {
                    sValue = visibleValue;
                    e.SetActiveField(null);
                    switch (fType)
                    {
                        case FIELD_TYPE.LVL_DEST:
                            e.LevelDest = sValue;
                            break;
                    }
                }
                else if (ks.IsKeyDown(Keys.Tab))
                {
                    Undo();
                    e.SetActiveField(null);
                }
                else if (ks.IsKeyDown(Keys.Space))
                {
                    visibleValue += ' ';
                }
                else if (pressed.Length > 0)
                {
                    for(int n = 0; n < pressed.Length; n++) 
                    {
                        string k = pressed[n].ToString();
                        if (k.Length == 1)
                        {
                            char c = k.ToCharArray()[0];
                            if (Char.IsLetter(c))
                            {
                                /*if (ks.IsKeyDown(Keys.LeftShift) || ks.IsKeyDown(Keys.RightShift))
                                {
                                    visibleValue += c;
                                }
                                else
                                {*/
                                    visibleValue += Char.ToLower(c);
                                //}
                            }
                            break;
                        }
                        else if (k.Length == 2)
                        {
                            char c = k.ToCharArray()[1];
                            if (Char.IsDigit(c))
                            {
                                visibleValue += c;
                            }
                            break;
                        }
                    }
                } 
            }


        }

        public override void Undo()
        {
            visibleValue = sValue.ToString();
        }
    }
}
