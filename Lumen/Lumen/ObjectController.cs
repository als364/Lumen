using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Box2DX.Collision;
using Box2DX.Common;
using Box2DX.Dynamics.Controllers;

namespace Lumen
{

    /// <summary>
    /// This is the controller for any one individual GameObject.
    /// </summary>
    public class ObjectController
    {
        protected GameObject controlledObject;
        public GameObject ControlledObject { get { return controlledObject; } }

        public ObjectController(GameObject obj)
        {
            controlledObject = obj;
        }

        /*public ObjectController()
        {
        }*/

        /// <summary>
        /// Gets the object that this controller controls
        /// </summary>
        /// <returns>The controlled object</returns>
        public GameObject getControlledObject()
        {
            return controlledObject;
        }

        /// <summary>
        /// Sets the object that this controlled controlls
        /// </summary>
        /// <param name="obj">The object to be controlled</param>
        public void setControlledObject(GameObject obj)
        {
            controlledObject = obj;
        }

        /// <summary>
        /// Updates the controlled object
        /// </summary>
        /// <param name="gameTime">Provides snapshot of timinmg values.</param>
        public virtual void Update(GameTime gameTime)
        {
        }


    }
}
