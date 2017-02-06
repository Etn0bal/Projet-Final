using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace AtelierXNA
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class JoinMenu : Microsoft.Xna.Framework.DrawableGameComponent
    {
        Point positionSouris { get; set; }
        InputManager GestionnaireInputs { get; set; }
        public JoinMenu(Game game)
            : base(game)
        { }

        public override void Initialize()
        {
            GestionnaireInputs = Game.Services.GetService(typeof(InputManager)) as InputManager;
            positionSouris = new Point(0, 0);

            //Arriere plan
            Rectangle arri�rePlan = new Rectangle(0, 0, Game.Window.ClientBounds.Width, Game.Window.ClientBounds.Height);
            SpriteMainMenu fondd�cran = new SpriteMainMenu(Game, arri�rePlan, "dragon");
            Game.Components.Add(fondd�cran);

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            base.Update(gameTime);
        }
    }
}
