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

        Rectangle positionBackButton;
        public JoinMenu(Game game)
            : base(game)
        { }

        public override void Initialize()
        {
            GestionnaireInputs = Game.Services.GetService(typeof(InputManager)) as InputManager;
            positionSouris = new Point(0, 0);

            //Arriere plan
            Rectangle arrièrePlan = new Rectangle(0, 0, Game.Window.ClientBounds.Width, Game.Window.ClientBounds.Height);
            SpriteJoinMenu fonddÉcran = new SpriteJoinMenu(Game, arrièrePlan, "imagedefond");
            Game.Components.Add(fonddÉcran);

            //Button
            positionBackButton = new Rectangle(7 * (Game.Window.ClientBounds.Width / 10), 7 * (Game.Window.ClientBounds.Height / 10), 2 * (Game.Window.ClientBounds.Width / 10), (Game.Window.ClientBounds.Height / 10));
            SpriteJoinMenu BackButton = new SpriteJoinMenu(Game, positionBackButton, "BackButton");
            Game.Components.Add(BackButton);

            //titre
            Rectangle titre = new Rectangle((2 * (Game.Window.ClientBounds.Width / 10)), Game.Window.ClientBounds.Height / 10, 6 * (Game.Window.ClientBounds.Width / 10), (Game.Window.ClientBounds.Height / 10));
            SpriteJoinMenu titreJoinGame = new SpriteJoinMenu(Game, titre, "JoinGame");
            Game.Components.Add(titreJoinGame);


            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            GérerSouris();
            base.Update(gameTime);
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
        }
    }
}
