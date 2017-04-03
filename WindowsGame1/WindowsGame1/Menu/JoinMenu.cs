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
using System.Windows.Forms;



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
        string IP { get; set; }

        Rectangle positionBackButton;
        Rectangle positionJoinServerButton;
        bool ServerTrouv�;
        public JoinMenu(Game game)
            : base(game)
        { }

        public override void Initialize()
        {
            GestionnaireInputs = Game.Services.GetService(typeof(InputManager)) as InputManager;
            positionSouris = new Point(0, 0);
            IP�crit = "";
            ServerTrouv� = true;

            //Arriere plan
            Rectangle arri�rePlan = new Rectangle(0, 0, Game.Window.ClientBounds.Width, Game.Window.ClientBounds.Height);
            SpriteJoinMenu fondd�cran = new SpriteJoinMenu(Game, arri�rePlan, "imagedefond");
            Game.Components.Add(fondd�cran);

            //Button
            positionBackButton = new Rectangle(7 * (Game.Window.ClientBounds.Width / 10), 7 * (Game.Window.ClientBounds.Height / 10), 2 * (Game.Window.ClientBounds.Width / 10), (Game.Window.ClientBounds.Height / 10));
            SpriteJoinMenu BackButton = new SpriteJoinMenu(Game, positionBackButton, "BackButton");
            Game.Components.Add(BackButton);

            positionJoinServerButton = new Rectangle(7 * (Game.Window.ClientBounds.Width / 10), 5 * (Game.Window.ClientBounds.Height / 10), 2 * (Game.Window.ClientBounds.Width / 10), (Game.Window.ClientBounds.Height / 10));
            SpriteJoinMenu JoinServerButton = new SpriteJoinMenu(Game, positionJoinServerButton, "JoinServer");
            Game.Components.Add(JoinServerButton);

            //titre
            Rectangle titre = new Rectangle((2 * (Game.Window.ClientBounds.Width / 10)), Game.Window.ClientBounds.Height / 10, 6 * (Game.Window.ClientBounds.Width / 10), (Game.Window.ClientBounds.Height / 10));
            SpriteJoinMenu titreJoinGame = new SpriteJoinMenu(Game, titre, "JoinGame");
            Game.Components.Add(titreJoinGame);
            //ZoneTexte
            Rectangle PositionTxtAEcrire = new Rectangle((2 * (Game.Window.ClientBounds.Width / 10)), 5 * Game.Window.ClientBounds.Height / 10, 3 * (Game.Window.ClientBounds.Width / 10), (Game.Window.ClientBounds.Height / 10));
            TexteModifiable txt = new TexteModifiable(Game, IP�crit, "Arial", PositionTxtAEcrire, new Vector2(3 * Game.Window.ClientBounds.Width / 10, Game.Window.ClientBounds.Height / 2), Color.White, 0);
            Game.Components.Add(txt);

            Rectangle PositionTxt = new Rectangle((2 * (Game.Window.ClientBounds.Width / 10)), 5 * Game.Window.ClientBounds.Height / 10, 3 * (Game.Window.ClientBounds.Width / 10), (Game.Window.ClientBounds.Height / 10));
            TexteJoinMenu txt2 = new TexteJoinMenu(Game, "Entrez l'IP de votre Adversaire : ", "Arial", PositionTxt, new Vector2(3 * Game.Window.ClientBounds.Width / 10, 4*Game.Window.ClientBounds.Height / 10), Color.White, 0);
            Game.Components.Add(txt2);


            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            G�rerSouris();
            G�rerClavier(gameTime);
            IP = IP�crit;

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
            if (positionJoinServerButton.Contains(positionSouris)&& GestionnaireInputs.EstNouveauClicGauche() || GestionnaireInputs.EstNouvelleTouche(Microsoft.Xna.Framework.Input.Keys.Enter))
            {
                    try
                    {
                        ServeurClient Invit� = new ServeurClient(Game, IP);
                        Game.Services.AddService(typeof(ServeurClient), Invit�);
                        ServerTrouv� = true;                        
                    }
                    catch (Exception)
                    {
                    }
                    if (ServerTrouv� == true)
                    {
                       ((Game1)Game).NumClient = 1;
                    }
            }
        }
        void G�rerClavier(GameTime gameTime)
        {
            if(GestionnaireInputs.EstClavierActiv�)
            {
                if (IP�crit.Count() < 20)
                {

                    if (GestionnaireInputs.EstNouvelleTouche(Microsoft.Xna.Framework.Input.Keys.D1) || GestionnaireInputs.EstNouvelleTouche(Microsoft.Xna.Framework.Input.Keys.NumPad1))
                    {
                        IP�crit += "1";
                    }
                    if (GestionnaireInputs.EstNouvelleTouche(Microsoft.Xna.Framework.Input.Keys.D2) || GestionnaireInputs.EstNouvelleTouche(Microsoft.Xna.Framework.Input.Keys.NumPad2))
                    {
                        IP�crit += "2";
                    }
                    if (GestionnaireInputs.EstNouvelleTouche(Microsoft.Xna.Framework.Input.Keys.D3) || GestionnaireInputs.EstNouvelleTouche(Microsoft.Xna.Framework.Input.Keys.NumPad3))
                    {
                        IP�crit += "3";
                    }
                    if (GestionnaireInputs.EstNouvelleTouche(Microsoft.Xna.Framework.Input.Keys.D4) || GestionnaireInputs.EstNouvelleTouche(Microsoft.Xna.Framework.Input.Keys.NumPad4))
                    {
                        IP�crit += "4";
                    }
                    if (GestionnaireInputs.EstNouvelleTouche(Microsoft.Xna.Framework.Input.Keys.D5) || GestionnaireInputs.EstNouvelleTouche(Microsoft.Xna.Framework.Input.Keys.NumPad5))
                    {
                        IP�crit += "5";
                    }
                    if (GestionnaireInputs.EstNouvelleTouche(Microsoft.Xna.Framework.Input.Keys.D6) || GestionnaireInputs.EstNouvelleTouche(Microsoft.Xna.Framework.Input.Keys.NumPad6))
                    {
                        IP�crit += "6";
                    }
                    if (GestionnaireInputs.EstNouvelleTouche(Microsoft.Xna.Framework.Input.Keys.D7) || GestionnaireInputs.EstNouvelleTouche(Microsoft.Xna.Framework.Input.Keys.NumPad7))
                    {
                        IP�crit += "7";
                    }
                    if (GestionnaireInputs.EstNouvelleTouche(Microsoft.Xna.Framework.Input.Keys.D8) || GestionnaireInputs.EstNouvelleTouche(Microsoft.Xna.Framework.Input.Keys.NumPad8))
                    {
                        IP�crit += "8";
                    }
                    if (GestionnaireInputs.EstNouvelleTouche(Microsoft.Xna.Framework.Input.Keys.D9) || GestionnaireInputs.EstNouvelleTouche(Microsoft.Xna.Framework.Input.Keys.NumPad9))
                    {
                        IP�crit += "9";
                    }
                    if (GestionnaireInputs.EstNouvelleTouche(Microsoft.Xna.Framework.Input.Keys.D0) || GestionnaireInputs.EstNouvelleTouche(Microsoft.Xna.Framework.Input.Keys.NumPad0))
                    {
                        IP�crit += "0";
                    }
                    if (GestionnaireInputs.EstNouvelleTouche(Microsoft.Xna.Framework.Input.Keys.OemPeriod))
                    {
                        IP�crit += ".";
                    }
                }
                if(GestionnaireInputs.EstNouvelleTouche(Microsoft.Xna.Framework.Input.Keys.Back))
                {
                    if(IP�crit.Count()!= 0)
                    {
                        IP�crit = IP�crit.Remove(IP�crit.Count() - 1);
                    }
                }
                foreach (TexteModifiable a in Game.Components.Where(x => x is TexteModifiable))
                {
                    a.ModifierTexte(IP�crit);
                    a.Draw(gameTime);
                }
            }
        }


    }
}
