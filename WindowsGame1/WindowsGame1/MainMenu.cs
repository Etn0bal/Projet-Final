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
    public class MainMenu : Microsoft.Xna.Framework.DrawableGameComponent
    {
        Point positionSouris { get; set; }

        Rectangle positionCreateGameButton { get; set; }
        Rectangle positionHostGameButton { get; set; }
        Rectangle positionQuitGameButton { get; set; }
            
        InputManager GestionnaireInputs { get; set; }


            public MainMenu(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            GestionnaireInputs = Game.Services.GetService(typeof(InputManager)) as InputManager;

            //Arriere plan
            Sprite fondd…cran = new Sprite(Game, 0, 0, Game.Window.ClientBounds.Width, Game.Window.ClientBounds.Height, "dragon");
            Game.Components.Add(fondd…cran);
            //VidÈo

            //Button
            Sprite CreateGameButton = new Sprite(Game, Game.Window.ClientBounds.Width / 10, 5 * Game.Window.ClientBounds.Height / 10, 3 * (Game.Window.ClientBounds.Width / 10), Game.Window.ClientBounds.Height / 10, "JoinGame");
            positionCreateGameButton = new Rectangle(Game.Window.ClientBounds.Width / 10, 5 * Game.Window.ClientBounds.Height / 10, 3 * (Game.Window.ClientBounds.Width / 10), Game.Window.ClientBounds.Height / 10);
            Game.Components.Add(CreateGameButton);

            Sprite HostGameButton = new Sprite(Game, Game.Window.ClientBounds.Width/10, 3*Game.Window.ClientBounds.Height/10, 3 * (Game.Window.ClientBounds.Width / 10), Game.Window.ClientBounds.Height/10, "HostGame");
            positionHostGameButton = new Rectangle(Game.Window.ClientBounds.Width / 10, 3 * Game.Window.ClientBounds.Height / 10, 3 * (Game.Window.ClientBounds.Width / 10), Game.Window.ClientBounds.Height / 10);
            Game.Components.Add(HostGameButton);

            Sprite quitButton = new Sprite(Game, Game.Window.ClientBounds.Width / 10, 7 * Game.Window.ClientBounds.Height / 10, 3 * (Game.Window.ClientBounds.Width / 10), Game.Window.ClientBounds.Height / 10, "Quit");
            positionQuitGameButton = new Rectangle(Game.Window.ClientBounds.Width / 10, 7 * Game.Window.ClientBounds.Height / 10, 3 * (Game.Window.ClientBounds.Width / 10), Game.Window.ClientBounds.Height / 10);
            Game.Components.Add(quitButton);

            //Titre

            Sprite TitreMainMenu = new Sprite(Game, Game.Window.ClientBounds.Width/10, Game.Window.ClientBounds.Height / 10, 8 *( Game.Window.ClientBounds.Width / 10),(Game.Window.ClientBounds.Height/ 10), "MLANBA");
            Game.Components.Add(TitreMainMenu);


            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            GÈrerSouris();
            base.Update(gameTime);
        }
        void GÈrerSouris()
        {
            positionSouris = GestionnaireInputs.GetPositionSouris();

            if (positionQuitGameButton.Contains(positionSouris))
            {
                if (GestionnaireInputs.EstNouveauClicGauche())
                {
                    Game.Exit();
                }
            }
        }
    }
}
