using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace LumenLevelEditor
{
    /// <summary>
    /// Contains a list of Buttons (and their position on the screen)
    /// </summary>
    public class ButtonManager
    {
        private List<Button> mainButtons;
        private List<Button> elementButtons;

        Texture2D pixel;

        SpriteFont font;

        protected bool waitingOnMouseClick;

        int buttonWidth = 150;
        int buttonHeight = 30;
        int fieldWidth = 150;
        int fieldHeight = 30;
        int margins = 10;


        public ButtonManager(GraphicsDevice dev, Engine e)
        {
            mainButtons = new List<Button>();
            elementButtons = new List<Button>();

            pixel = new Texture2D(dev, 1, 1);
            Color[] data = {Color.White};
            pixel.SetData<Color>(data);

            PopulateButtons(e);

            waitingOnMouseClick = true;
        }

        private void PopulateButtons(Engine e)
        {
            //add main menu buttons to top right; 
            //we will put game elements under this?
            /*
             Overall menu:
                -save the damn level button
                -load a level button (needs GUI for browsing?)
                -Field to specify ambient light
                -contains a number of GameElements (not buttons) to drag into the world
                -shadows on/off button
                -button or field to change overall level size
                -courage field represents starting courage?


             */

            addField(Field.FIELD_TYPE.LVL_DEST, new Rectangle(Engine.WORLD_WIDTH + margins, margins, 2 * fieldWidth, fieldHeight), "Lvl Dest", e.LevelDest, true);

            addButton(Button.BUTTON_TYPE.SAVE, new Rectangle(Engine.WORLD_WIDTH + margins, 2 * margins + fieldHeight, buttonWidth, buttonHeight), "SAVE", true);
            addButton(Button.BUTTON_TYPE.LOAD, new Rectangle(Engine.WORLD_WIDTH + 2 * margins + buttonWidth, 2 * margins + fieldHeight, buttonWidth, buttonHeight), "LOAD", true);

            //addField(Field.FIELD_TYPE.AMBIENT, new Rectangle(Engine.WORLD_WIDTH + margins, 3 * margins + buttonHeight + fieldHeight, fieldWidth, fieldHeight), "Ambient", e.AmbientLight, true);
            addButton(Button.BUTTON_TYPE.ADD_WALL, new Rectangle(Engine.WORLD_WIDTH + margins, 3 * margins + buttonHeight + fieldHeight, buttonWidth, buttonHeight), "ADD WALL", true);
            addButton(Button.BUTTON_TYPE.SNAP_TO_GRID, new Rectangle(Engine.WORLD_WIDTH + 2 * margins + fieldWidth, 3 * margins + buttonHeight + fieldHeight, buttonWidth, buttonHeight), "SNAP TO GRID", true);

            addField(Field.FIELD_TYPE.LEVEL_WIDTH, new Rectangle(Engine.WORLD_WIDTH + margins, 4 * margins + buttonHeight + 2 * fieldHeight, fieldWidth, fieldHeight), "Lvl W", e.LevelWidth, true);
            addField(Field.FIELD_TYPE.LEVEL_HEIGHT, new Rectangle(Engine.WORLD_WIDTH + 2 * margins + fieldWidth, 4 * margins + buttonHeight + 2 * fieldHeight, fieldWidth, fieldHeight), "Lvl H", e.LevelHeight, true);

            //addButton(Button.BUTTON_TYPE.SHADOWS, new Rectangle(Engine.WORLD_WIDTH + margins, 5 * margins + buttonHeight + 3 * fieldHeight, buttonWidth, buttonHeight), "SHADOWS ON/OFF", true);
            addButton(Button.BUTTON_TYPE.ADD_EXIT, new Rectangle(Engine.WORLD_WIDTH + margins, 5 * margins + buttonHeight + 3 * fieldHeight, buttonWidth, buttonHeight), "ADD/REM EXIT", true);
            addField(Field.FIELD_TYPE.INIT_COURAGE, new Rectangle(Engine.WORLD_WIDTH + 2 * margins + buttonWidth, 5 * margins + buttonHeight + 3 * fieldHeight, fieldWidth, fieldHeight), "Cour", e.InitialCourage, true);

            addField(Field.FIELD_TYPE.EXIT_SRC, new Rectangle(Engine.WORLD_WIDTH + margins, 6 * margins + buttonHeight + 4 * fieldHeight, fieldWidth, fieldHeight), "Exit Orig", e.ExitOrigin, true);
            addField(Field.FIELD_TYPE.EXIT_SINK, new Rectangle(Engine.WORLD_WIDTH + 2 * margins + fieldWidth, 6 * margins + buttonHeight + 4 * fieldHeight, fieldWidth, fieldHeight), "Exit Dest", e.ExitDestination, true);




            //add initially hidden buttons and fields
            //for controlling the selected game element
            /*
             * For a particular Game Element:C:\Users\Sean\Documents\cis3000\technical prototype\trunk\LevelEditor\LumenLevelEditor\LumenLevelEditor\GameElementManager.cs
                -a button that opens up the GameElement XML file editor thingy... that will need to wait
                -slidable? -rotatable?
                -switch / plug button that allows user to specify power source with next click
                -rotation button specifies initial orientation (possible field as well?)
                -on or off (for switches)
                -light color?
                -button brings you back to main menu
             */
            addButton(Button.BUTTON_TYPE.MAIN_MENU, new Rectangle(Engine.WORLD_WIDTH + margins, margins, buttonWidth, buttonHeight), "MAIN MENU", false);

            addButton(Button.BUTTON_TYPE.ROTATE_CCW, new Rectangle(Engine.WORLD_WIDTH + margins, 2 * margins + buttonHeight, buttonWidth, buttonHeight), "ROTATE CCW", false);
            addButton(Button.BUTTON_TYPE.ROTATE_CW, new Rectangle(Engine.WORLD_WIDTH + 2 * margins + buttonWidth, 2 * margins + buttonHeight, buttonWidth, buttonHeight), "ROTATE CW", false);

            addButton(Button.BUTTON_TYPE.SELECT_SOURCE, new Rectangle(Engine.WORLD_WIDTH + margins, 3 * margins + 2 * buttonHeight, buttonWidth, buttonHeight), "SELECT POWER SOURCE", false);
            addButton(Button.BUTTON_TYPE.CLEAR_SOURCES, new Rectangle(Engine.WORLD_WIDTH + 2 * margins + buttonWidth, 3 * margins + 2 * buttonHeight, buttonWidth, buttonHeight), "CLEAR SOURCES", false);

            addButton(Button.BUTTON_TYPE.TURN_ON, new Rectangle(Engine.WORLD_WIDTH + margins, 4 * margins + 3 * buttonHeight, buttonWidth, buttonHeight), "TURN ON", false);
            addButton(Button.BUTTON_TYPE.TURN_OFF, new Rectangle(Engine.WORLD_WIDTH + 2 * margins + buttonWidth, 4 * margins + 3 * buttonHeight, buttonWidth, buttonHeight), "TURN OFF", false);


        }

        public Point GetElementMenuStartPos()
        {
            return new Point(Engine.WORLD_WIDTH + margins, 7 * margins + buttonHeight + 5 * fieldHeight);
        }

        public void addButton(Button.BUTTON_TYPE t, Rectangle r, String s, bool mainMenu)
        {
            if(mainMenu)
                mainButtons.Add(new Button(t, r, s));
            else
                elementButtons.Add(new Button(t, r, s));
        }

        public void addField(Field.FIELD_TYPE t, Rectangle r, String s, float initVal, bool mainMenu)
        {
            if (mainMenu)
                mainButtons.Add(new Field(t, r, s, initVal));
            else
                elementButtons.Add(new Field(t, r, s, initVal));
        }

        public void addField(Field.FIELD_TYPE t, Rectangle r, String s, string initVal, bool mainMenu)
        {
            if (mainMenu)
                mainButtons.Add(new StringField(t, r, s, initVal));
            else
                elementButtons.Add(new StringField(t, r, s, initVal));
        }

        public void setFont(SpriteFont f)
        {
            font = f;
        }

        /// <summary>
        /// react to a mouse click
        /// </summary>
        /// <param name="ms"></param>
        /// <returns>true if a button is at the location, false otherwise</returns>
        public bool OnClick(MouseState ms, Engine e, bool mainMenu) {
            if (waitingOnMouseClick)
            {
                waitingOnMouseClick = false;
                if (mainMenu)
                {
                    // check if a button is being pressed
                    foreach (Button b in mainButtons)
                    {
                        if (b.isPressed(ms.X, ms.Y))
                        {
                            // if so, call appropriate button.Pressed
                            b.Pressed(e);
                            return true;
                        }
                    }
                }
                else
                {
                    // check if a button is being pressed
                    foreach (Button b in elementButtons)
                    {
                        if (b.isPressed(ms.X, ms.Y))
                        {
                            // if so, call appropriate button.Pressed
                            b.Pressed(e);
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public void OnRelease()
        {
            waitingOnMouseClick = true;
        }

        public void DrawMainMenu(SpriteBatch spriteBatch)
        {
            foreach (Button b in mainButtons)
            {
                b.Draw(spriteBatch, pixel, font);
            }
        }

        public void DrawElementMenu(SpriteBatch spriteBatch)
        {
            foreach (Button b in elementButtons)
            {
                b.Draw(spriteBatch, pixel, font);
            }
        }
    }
}
