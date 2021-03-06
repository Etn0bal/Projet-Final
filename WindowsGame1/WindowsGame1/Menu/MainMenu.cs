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
            
        InputManager GestionInputs { get; set; }


            public MainMenu(Microsoft.Xna.Framework.Game game)
            : base(game)
        { }

        public override void Initialize()
        {
            GestionInputs = Game.Services.GetService(typeof(InputManager)) as InputManager;
            positionSouris = new Point(0,0);

            //Arriere plan
            Rectangle arrièrePlan = new Rectangle(0, 0, Game.Window.ClientBounds.Width, Game.Window.ClientBounds.Height);
            SpriteMainMenu fonddÉcran = new SpriteMainMenu(Game, arrièrePlan, "imagedefond");
            Game.Components.Add(fonddÉcran);           

            //Button
            positionCreateGameButton = new Rectangle(Game.Window.ClientBounds.Width / 10, 5 * Game.Window.ClientBounds.Height / 10, 3 * (Game.Window.ClientBounds.Width / 10), Game.Window.ClientBounds.Height / 10);
            SpriteMainMenu CreateGameButton = new SpriteMainMenu(Game, positionCreateGameButton, "JoinGame");
            Game.Components.Add(CreateGameButton);
            

            positionHostGameButton = new Rectangle(Game.Window.ClientBounds.Width / 10, 3 * Game.Window.ClientBounds.Height / 10, 3 * (Game.Window.ClientBounds.Width / 10), Game.Window.ClientBounds.Height / 10);
            SpriteMainMenu HostGameButton = new SpriteMainMenu(Game,positionHostGameButton, "HostGame");
            Game.Components.Add(HostGameButton);

            positionQuitGameButton = new Rectangle(Game.Window.ClientBounds.Width / 10, 7 * Game.Window.ClientBounds.Height / 10, 3 * (Game.Window.ClientBounds.Width / 10), Game.Window.ClientBounds.Height / 10);
            SpriteMainMenu quitButton = new SpriteMainMenu(Game,positionQuitGameButton, "Quit");
            Game.Components.Add(quitButton);

            //Titre

            Rectangle titre = new Rectangle(Game.Window.ClientBounds.Width / 10, Game.Window.ClientBounds.Height / 10, 6 * (Game.Window.ClientBounds.Width / 10), (Game.Window.ClientBounds.Height / 10));
            SpriteMainMenu TitreMainMenu = new SpriteMainMenu(Game,titre, "MLANBA");
            Game.Components.Add(TitreMainMenu);


            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            if(GestionInputs.EstNouvelleTouche(Keys.NumPad0))
            {
                ((Game)Game).ChangerDÉtat(3);
            }
            GérerSouris();
        }
        void GérerSouris()
        {
            positionSouris = GestionInputs.GetPositionSouris();

            if (positionQuitGameButton.Contains(positionSouris))
            {
                if (GestionInputs.EstNouveauClicGauche())
                {
                    Game.Exit();
                }
            }
            if(positionHostGameButton.Contains(positionSouris))
            {
                if (GestionInputs.EstNouveauClicGauche())
                {
                    ((Game)Game).ChangerDÉtat(2);
                }
            }
            if (positionCreateGameButton.Contains(positionSouris))
            {
                if (GestionInputs.EstNouveauClicGauche())
                {
                    ((Game)Game).ChangerDÉtat(1);

                }
            }


        }
    }
}
