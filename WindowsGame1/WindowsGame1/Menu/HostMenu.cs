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
    public class HostMenu : Microsoft.Xna.Framework.DrawableGameComponent
    {
        Rectangle positionBackButton { get; set; }
        Rectangle positionStartGameButton { get; set; }
        Rectangle positionSettingsButton { get; set; }

        Point positionSouris { get; set; }
        InputManager GestionnaireInputs { get; set; }
        public HostMenu(Game game)
            : base(game)
        { }

        public override void Initialize()
        {
            GestionnaireInputs = Game.Services.GetService(typeof(InputManager)) as InputManager;
            positionSouris = new Point(0, 0);

            //Arriere plan
            Rectangle arri�rePlan = new Rectangle(0, 0, Game.Window.ClientBounds.Width, Game.Window.ClientBounds.Height);
            SpriteHostMenu fondd�cran = new SpriteHostMenu(Game, arri�rePlan, "dragon");
            Game.Components.Add(fondd�cran);

            //Boutons
            positionStartGameButton = new Rectangle(7 * (Game.Window.ClientBounds.Width / 10), 3 * (Game.Window.ClientBounds.Height / 10), 2 * (Game.Window.ClientBounds.Width / 10), (Game.Window.ClientBounds.Height / 10));
            SpriteHostMenu StartGameButton = new SpriteHostMenu(Game, positionStartGameButton, "StartGame");
            Game.Components.Add(StartGameButton);

            positionSettingsButton = new Rectangle(7 * (Game.Window.ClientBounds.Width / 10), 5 * (Game.Window.ClientBounds.Height / 10), 2 * (Game.Window.ClientBounds.Width / 10), (Game.Window.ClientBounds.Height / 10));
            SpriteHostMenu SettingsGameButton = new SpriteHostMenu(Game, positionSettingsButton, "Settings");
            Game.Components.Add(SettingsGameButton);
            

            positionBackButton = new Rectangle(7 * (Game.Window.ClientBounds.Width / 10), 7 * (Game.Window.ClientBounds.Height / 10), 2 * (Game.Window.ClientBounds.Width / 10), (Game.Window.ClientBounds.Height / 10));
            SpriteHostMenu BackButton = new SpriteHostMenu(Game,positionBackButton, "BackButton");
            Game.Components.Add(BackButton);

            //titre
            Rectangle titre = new Rectangle((Game.Window.ClientBounds.Width / 10), Game.Window.ClientBounds.Height / 10, 8 * (Game.Window.ClientBounds.Width / 10), (Game.Window.ClientBounds.Height / 10));
            SpriteHostMenu titreHostGame = new SpriteHostMenu(Game, titre, "HostGame");
            Game.Components.Add(titreHostGame);

            base.Initialize();
        }
        public override void Update(GameTime gameTime)
        {
            G�rerSouris();
        }
        void G�rerSouris()
        {
            positionSouris = GestionnaireInputs.GetPositionSouris();

            if (positionBackButton.Contains(positionSouris))
            {
                if (GestionnaireInputs.EstNouveauClicGauche())
                {
                    ((Game1)Game).ChangerD�tat(0);
                }
            }
          

        }
    }
}
