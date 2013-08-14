using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LumenLevelEditor
{
    
    /// <summary>
    /// A button that reacts on a single click. 
    /// Perhaps used to change the working mode Game Engine is in, 
    /// or to change flags internal to a particular XML file for a particular Game Element
    /// </summary>
    public class Button
    {
        protected Rectangle position;
        protected String text;
        protected BUTTON_TYPE bType;



        /*
        Misc:
        -Buttons to scroll the game world

        Overall menu:
        -save the damn level button
        -load a level button (needs GUI for browsing?)
        -Field to specify ambient light
        -contains a number of GameElements (not buttons) to drag into the world
        -shadows on/off button
        -button or field to change overall level size
        -courage field represents starting courage?

        For a particular Game Element:
        -a button that opens up the GameElement XML file editor thingy... that will need to wait
        -slidable? -rotatable?
        -switch / plug button that allows user to specify power source with next click
        -rotation button specifies initial orientation (possible field as well?)
        -on or off (for switches)
        -light color?
        -button brings you back to main menu
         * */
        public enum BUTTON_TYPE
        {
            SCROLL_DOWN,
            SCROLL_UP,
            SCROLL_RIGHT,
            SCROLL_LEFT,
            SAVE,
            LOAD,
            SNAP_TO_GRID,
            SHADOWS,
            ADD_EXIT,
            ADD_WALL,

            //SLIDABLE,
            //ROTATABLE,
            TURN_ON,
            TURN_OFF,
            ROTATE_CW,
            ROTATE_CCW,
            SELECT_SOURCE,
            CLEAR_SOURCES,

            MAIN_MENU,
            FIELD
        }

        public Button(BUTTON_TYPE type, Rectangle pos, string s) 
        {
            bType = type;
            position = pos;
            text = s;

            
        }

        public bool isPressed(float x, float y)
        {
            return position.Contains((int) x, (int) y);
        }

        /// <summary>
        /// do something, probably sends some sort of flag back up the chain to Engine
        /// </summary>
        public void Pressed(Engine e) 
        {
            Debug.Write(bType);
            switch (bType)
            {
                case BUTTON_TYPE.SCROLL_DOWN:
                    e.ChangeWorldPosition(new Vector2(+1, 0));
                    break;
                case BUTTON_TYPE.SCROLL_UP:
                    e.ChangeWorldPosition(new Vector2(-1, 0));
                    break;
                case BUTTON_TYPE.SCROLL_RIGHT:
                    e.ChangeWorldPosition(new Vector2(0, +1));
                    break;
                case BUTTON_TYPE.SCROLL_LEFT:
                    e.ChangeWorldPosition(new Vector2(0, -1));
                    break;
                case BUTTON_TYPE.SAVE:
                    e.Save();
                    break;
                case BUTTON_TYPE.LOAD:
                    e.Load();
                    break;
                case BUTTON_TYPE.SNAP_TO_GRID:
                    e.SnapToGrid = !e.SnapToGrid;
                    break;
                case BUTTON_TYPE.SHADOWS:
                    e.switchShadows();
                    break;
                case BUTTON_TYPE.ADD_EXIT:
                    e.AddExit();
                    break;
                case BUTTON_TYPE.ADD_WALL:
                    e.AddWall();
                    break;
                /*case BUTTON_TYPE.SLIDABLE:
                    break;*/
                /*case BUTTON_TYPE.ROTATABLE:
                    break;*/
                case BUTTON_TYPE.TURN_ON:
                    e.TurnOnOff(true);
                    break;
                case BUTTON_TYPE.TURN_OFF:
                    e.TurnOnOff(false);
                    break;
                case BUTTON_TYPE.ROTATE_CW:
                    e.RotateObject(+1f);
                    break;
                case BUTTON_TYPE.ROTATE_CCW:
                    e.RotateObject(-1f);
                    break;
                case BUTTON_TYPE.SELECT_SOURCE:
                    e.SelectSource();
                    break;
                case BUTTON_TYPE.CLEAR_SOURCES:
                    e.ClearSources();
                    break;
                case BUTTON_TYPE.MAIN_MENU:
                    e.setGameElementMenu(null);
                    break;
                case BUTTON_TYPE.FIELD:
                    Field f = (Field) this;
                    e.SetActiveField(f);
                    f.ClearField();
                    break;
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch, Texture2D tex, SpriteFont font)
        {
            spriteBatch.Draw(tex, position, Color.Gray);
            spriteBatch.DrawString(font, text, new Vector2(position.Location.X, position.Location.Y), Color.Black);
        }
    }
}
