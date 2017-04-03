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
using System.Net;
using System.Net.Sockets;


namespace AtelierXNA
{
    public class HostMenu : Microsoft.Xna.Framework.DrawableGameComponent
    {
        Rectangle positionBackButton { get; set; }
        Rectangle positionCreateServerButton { get; set; }
        Rectangle positionStartGameButton { get; set; }

        Point positionSouris { get; set; }
        InputManager GestionnaireInputs { get; set; }
        Server ServeurDeJeu { get; set; }
        bool ServeurCr�� { get; set; }
        bool AutreClientConnect� { get; set; }
        string IP { get; set; }

        public HostMenu(Game game)
            : base(game)
        { }

        public override void Initialize()
        {
            GestionnaireInputs = Game.Services.GetService(typeof(InputManager)) as InputManager;
            positionSouris = new Point(0, 0);
            ServeurCr�� = false;
            bool AutreClientConnect� = false;


            //Arriere plan
            Rectangle arri�rePlan = new Rectangle(0, 0, Game.Window.ClientBounds.Width, Game.Window.ClientBounds.Height);
            SpriteHostMenu fondd�cran = new SpriteHostMenu(Game, arri�rePlan, "imagedefond");
            Game.Components.Add(fondd�cran);

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
            G�rerSouris();
        }
        void G�rerSouris()
        {
            positionSouris = GestionnaireInputs.GetPositionSouris();

            if (positionBackButton.Contains(positionSouris))
            {
                if (GestionnaireInputs.EstNouveauClicGauche())
                {
                    EnleverIp();
                    ((Game1)Game).ChangerD�tat(0);
                }
            }  
            if(positionCreateServerButton.Contains(positionSouris))
            {
                
                if(GestionnaireInputs.EstNouveauClicGauche())
                {
                    string sHostName = Dns.GetHostName();
                    IPHostEntry ipE = Dns.GetHostEntry(sHostName);
                    IPAddress[] IpA = ipE.AddressList;
                    IP = IpA[1].ToString();
                    Rectangle PositionTxt = new Rectangle((2 * (Game.Window.ClientBounds.Width / 10)), 5 * Game.Window.ClientBounds.Height / 10, 3 * (Game.Window.ClientBounds.Width / 10), (Game.Window.ClientBounds.Height / 10));
                    TexteHostMenu IpAAfficher = new TexteHostMenu(Game, IP, "Arial", PositionTxt, new Vector2(3 * Game.Window.ClientBounds.Width / 10, Game.Window.ClientBounds.Height / 2), Color.White, 0);
                    Game.Components.Add(IpAAfficher);

                    if (ServeurCr�� == false)
                    {
                        ServeurDeJeu = new Server(5011);
                        ServeurCr�� = true;
                        ServeurClient HostClient = new ServeurClient(Game, IP);
                        Game.Services.AddService(typeof(ServeurClient), HostClient);

                    }


                }
            }   
            if(positionStartGameButton.Contains(positionSouris))
            {
                if(GestionnaireInputs.EstNouveauClicGauche())
                {
                    if(AutreClientConnect�)
                    ((Game1)Game).EnJeu = true;
                    ((Game1)Game).NumClient = 0;

                    ((Game1)Game).ChangerD�tat(3);
                    
                }
            }  
            if(ServeurDeJeu.connectedClients ==2)
            {
                AutreClientConnect� = true;
            }

        }

        private void EnleverIp()
        {
            for (int i = Game.Components.Count - 1; i >= 0; --i)
            {
                if(Game.Components[i] is TexteHostMenu)
                {
                    Game.Components.RemoveAt(i);
                }
            }
        }
    }
}
