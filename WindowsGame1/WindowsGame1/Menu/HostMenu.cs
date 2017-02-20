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
        Rectangle positionCreateServerButton { get; set; }
        Rectangle positionStartGameButton { get; set; }

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
            Rectangle arrièrePlan = new Rectangle(0, 0, Game.Window.ClientBounds.Width, Game.Window.ClientBounds.Height);
            SpriteHostMenu fonddÉcran = new SpriteHostMenu(Game, arrièrePlan, "imagedefond");
            Game.Components.Add(fonddÉcran);

            //Boutons
            positionCreateServerButton = new Rectangle(7 * (Game.Window.ClientBounds.Width / 10), 3 * (Game.Window.ClientBounds.Height / 10), 2 * (Game.Window.ClientBounds.Width / 10), (Game.Window.ClientBounds.Height / 10));
            SpriteHostMenu CreateServerButton = new SpriteHostMenu(Game, positionCreateServerButton, "CreateServer");
            Game.Components.Add(CreateServerButton);

            positionStartGameButton = new Rectangle(7 * (Game.Window.ClientBounds.Width / 10), 5 * (Game.Window.ClientBounds.Height / 10), 2 * (Game.Window.ClientBounds.Width / 10), (Game.Window.ClientBounds.Height / 10));
            SpriteHostMenu StartGameButton = new SpriteHostMenu(Game, positionStartGameButton, "StartGame");
            Game.Components.Add(StartGameButton);
            

            positionBackButton = new Rectangle(7 * (Game.Window.ClientBounds.Width / 10), 7 * (Game.Window.ClientBounds.Height / 10), 2 * (Game.Window.ClientBounds.Width / 10), (Game.Window.ClientBounds.Height / 10));
            SpriteHostMenu BackButton = new SpriteHostMenu(Game,positionBackButton, "BackButton");
            Game.Components.Add(BackButton);

            //titre
            Rectangle titre = new Rectangle((2*(Game.Window.ClientBounds.Width / 10)), Game.Window.ClientBounds.Height / 10, 6 * (Game.Window.ClientBounds.Width / 10), (Game.Window.ClientBounds.Height / 10));
            SpriteHostMenu titreHostGame = new SpriteHostMenu(Game, titre, "HostGame");
            Game.Components.Add(titreHostGame);

            base.Initialize();
        }
        public override void Update(GameTime gameTime)
        {
            GérerSouris();
        }
        void GérerSouris()
        {
            positionSouris = GestionnaireInputs.GetPositionSouris();

            if (positionBackButton.Contains(positionSouris))
            {
                if (GestionnaireInputs.EstNouveauClicGauche())
                {
                    ((Game1)Game).ChangerDÉtat(0);
                }
            }  
            if(positionCreateServerButton.Contains(positionSouris))
            {
                if(GestionnaireInputs.EstNouveauClicGauche())
                {

                }
            }   
            if(positionStartGameButton.Contains(positionSouris))
            {
                if(GestionnaireInputs.EstNouveauClicGauche())
                {
                    ((Game1)Game).ChangerDÉtat(3);
                    
                }
            }  
        }
    }
}
