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
        RessourcesManager<SpriteFont> Fonts { get; set; }
        string IPÉcrit { get; set; }
        SpriteFont Font { get; set; }

        Rectangle positionBackButton;
        public JoinMenu(Game game)
            : base(game)
        { }

        public override void Initialize()
        {
            GestionnaireInputs = Game.Services.GetService(typeof(InputManager)) as InputManager;
            positionSouris = new Point(0, 0);
            IPÉcrit = "asd";

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
            //ZoneTexte
            Rectangle PositionTxt = new Rectangle((4 * (Game.Window.ClientBounds.Width / 10)), Game.Window.ClientBounds.Height / 10, 2 * (Game.Window.ClientBounds.Width / 10), (Game.Window.ClientBounds.Height / 10));
            Texte txt = new Texte(Game,IPÉcrit,"Arial", PositionTxt, Color.Yellow,0.1f, new Vector2((2 * (Game.Window.ClientBounds.Width / 10)), Game.Window.ClientBounds.Height / 10));



            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            GérerSouris();
            GérerClavier();
            
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
        void GérerClavier()
        {
            if(GestionnaireInputs.EstClavierActivé)
            {
                if(GestionnaireInputs.EstNouvelleTouche(Keys.D1) || GestionnaireInputs.EstNouvelleTouche(Keys.NumPad1))
                {
                    IPÉcrit += 1;
                }
                if (GestionnaireInputs.EstNouvelleTouche(Keys.D2) || GestionnaireInputs.EstNouvelleTouche(Keys.NumPad2))
                {
                    IPÉcrit += 2;
                }
                if (GestionnaireInputs.EstNouvelleTouche(Keys.D3) || GestionnaireInputs.EstNouvelleTouche(Keys.NumPad3))
                {
                    IPÉcrit += 3;
                }
                if (GestionnaireInputs.EstNouvelleTouche(Keys.D4) || GestionnaireInputs.EstNouvelleTouche(Keys.NumPad4))
                {
                    IPÉcrit += 4;
                }
                if (GestionnaireInputs.EstNouvelleTouche(Keys.D5) || GestionnaireInputs.EstNouvelleTouche(Keys.NumPad5))
                {
                    IPÉcrit += 5;
                }
                if (GestionnaireInputs.EstNouvelleTouche(Keys.D6) || GestionnaireInputs.EstNouvelleTouche(Keys.NumPad6))
                {
                    IPÉcrit += 6;
                }
                if (GestionnaireInputs.EstNouvelleTouche(Keys.D7) || GestionnaireInputs.EstNouvelleTouche(Keys.NumPad7))
                {
                    IPÉcrit += 7;
                }
                if (GestionnaireInputs.EstNouvelleTouche(Keys.D8) || GestionnaireInputs.EstNouvelleTouche(Keys.NumPad8))
                {
                    IPÉcrit += 8;
                }
                if (GestionnaireInputs.EstNouvelleTouche(Keys.D9) || GestionnaireInputs.EstNouvelleTouche(Keys.NumPad9))
                {
                    IPÉcrit += 9;
                }
                if (GestionnaireInputs.EstNouvelleTouche(Keys.D0) || GestionnaireInputs.EstNouvelleTouche(Keys.NumPad0))
                {
                    IPÉcrit += 0;
                }

            }
        }


    }
}
