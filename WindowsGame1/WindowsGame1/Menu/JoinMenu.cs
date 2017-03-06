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
        string IP�crit { get; set; }
        SpriteFont Font { get; set; }

        Rectangle positionBackButton;
        public JoinMenu(Game game)
            : base(game)
        { }

        public override void Initialize()
        {
            GestionnaireInputs = Game.Services.GetService(typeof(InputManager)) as InputManager;
            positionSouris = new Point(0, 0);
            IP�crit = "asd";

            //Arriere plan
            Rectangle arri�rePlan = new Rectangle(0, 0, Game.Window.ClientBounds.Width, Game.Window.ClientBounds.Height);
            SpriteJoinMenu fondd�cran = new SpriteJoinMenu(Game, arri�rePlan, "imagedefond");
            Game.Components.Add(fondd�cran);

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
            Texte txt = new Texte(Game,IP�crit,"Arial", PositionTxt, Color.Yellow,0.1f, new Vector2((2 * (Game.Window.ClientBounds.Width / 10)), Game.Window.ClientBounds.Height / 10));



            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            G�rerSouris();
            G�rerClavier();
            
            base.Update(gameTime);
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
        void G�rerClavier()
        {
            if(GestionnaireInputs.EstClavierActiv�)
            {
                if(GestionnaireInputs.EstNouvelleTouche(Keys.D1) || GestionnaireInputs.EstNouvelleTouche(Keys.NumPad1))
                {
                    IP�crit += 1;
                }
                if (GestionnaireInputs.EstNouvelleTouche(Keys.D2) || GestionnaireInputs.EstNouvelleTouche(Keys.NumPad2))
                {
                    IP�crit += 2;
                }
                if (GestionnaireInputs.EstNouvelleTouche(Keys.D3) || GestionnaireInputs.EstNouvelleTouche(Keys.NumPad3))
                {
                    IP�crit += 3;
                }
                if (GestionnaireInputs.EstNouvelleTouche(Keys.D4) || GestionnaireInputs.EstNouvelleTouche(Keys.NumPad4))
                {
                    IP�crit += 4;
                }
                if (GestionnaireInputs.EstNouvelleTouche(Keys.D5) || GestionnaireInputs.EstNouvelleTouche(Keys.NumPad5))
                {
                    IP�crit += 5;
                }
                if (GestionnaireInputs.EstNouvelleTouche(Keys.D6) || GestionnaireInputs.EstNouvelleTouche(Keys.NumPad6))
                {
                    IP�crit += 6;
                }
                if (GestionnaireInputs.EstNouvelleTouche(Keys.D7) || GestionnaireInputs.EstNouvelleTouche(Keys.NumPad7))
                {
                    IP�crit += 7;
                }
                if (GestionnaireInputs.EstNouvelleTouche(Keys.D8) || GestionnaireInputs.EstNouvelleTouche(Keys.NumPad8))
                {
                    IP�crit += 8;
                }
                if (GestionnaireInputs.EstNouvelleTouche(Keys.D9) || GestionnaireInputs.EstNouvelleTouche(Keys.NumPad9))
                {
                    IP�crit += 9;
                }
                if (GestionnaireInputs.EstNouvelleTouche(Keys.D0) || GestionnaireInputs.EstNouvelleTouche(Keys.NumPad0))
                {
                    IP�crit += 0;
                }

            }
        }


    }
}
