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
        ServeurClient Client { get; set; }
        bool ServeurCréé { get; set; }
        bool AutreClientConnecté { get; set; }
        string IP { get; set; }

        public HostMenu(Microsoft.Xna.Framework.Game game)
            : base(game)
        { }

        public override void Initialize()
        {
            GestionnaireInputs = Game.Services.GetService(typeof(InputManager)) as InputManager;
            positionSouris = new Point(0, 0);
            ServeurCréé = false;
            AutreClientConnecté = false;


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
            SpriteHostMenu BackButton = new SpriteHostMenu(Game, positionBackButton, "BackButton");
            Game.Components.Add(BackButton);

            //titre
            Rectangle titre = new Rectangle((2 * (Game.Window.ClientBounds.Width / 10)), Game.Window.ClientBounds.Height / 10, 6 * (Game.Window.ClientBounds.Width / 10), (Game.Window.ClientBounds.Height / 10));
            SpriteHostMenu titreHostGame = new SpriteHostMenu(Game, titre, "HostGame");
            Game.Components.Add(titreHostGame);




            base.Initialize();
        }
        public override void Update(GameTime gameTime)
        {
            GérerSouris();
            GestionServeur();
        }

        private void GestionServeur()
        {
            if (ServeurDeJeu != null)
            {
                if (ServeurDeJeu.connectedClients == 2 && AutreClientConnecté == false)
                {
                    AutreClientConnecté = true;
                    GérerHostTxt();
                }
            }
        }
        private void GérerHostTxt()
        {
            EnleverTxt();
            Rectangle positionTxt = new Rectangle((2 * (Game.Window.ClientBounds.Width / 10)), 7 * Game.Window.ClientBounds.Height / 10, 4 * (Game.Window.ClientBounds.Width / 10), (Game.Window.ClientBounds.Height / 10));
            TexteHostMenu txt = new TexteHostMenu(Game, "Nombre de client connecté : 2 / 2", "Arial", positionTxt, new Vector2(4 * Game.Window.ClientBounds.Width / 10, 7 * Game.Window.ClientBounds.Height / 10), Color.White, 0);
            Game.Components.Add(txt);
        }

        void GérerSouris()
        {
            positionSouris = GestionnaireInputs.GetPositionSouris();

            if (positionBackButton.Contains(positionSouris))
            {
                if (GestionnaireInputs.EstNouveauClicGauche())
                {
                    EnleverTxt();
                    ((Game)Game).ChangerDÉtat(0);
                    
                }
            }
            if (positionCreateServerButton.Contains(positionSouris))
            {

                if (GestionnaireInputs.EstNouveauClicGauche())
                {
                    string sHostName = Dns.GetHostName();
                    IPHostEntry ipE = Dns.GetHostEntry(sHostName);
                    IPAddress[] IpA = ipE.AddressList;
                    IP = IpA[1].ToString();
                    Rectangle positionIP = new Rectangle((2 * (Game.Window.ClientBounds.Width / 10)), 5 * Game.Window.ClientBounds.Height / 10, 3 * (Game.Window.ClientBounds.Width / 10), (Game.Window.ClientBounds.Height / 10));
                    TexteHostMenu IpAAfficher = new TexteHostMenu(Game, IP, "Arial", positionIP, new Vector2(3 * Game.Window.ClientBounds.Width / 10, Game.Window.ClientBounds.Height / 2), Color.White, 0);
                    Rectangle positionTxt = new Rectangle((2 * (Game.Window.ClientBounds.Width / 10)), 7 * Game.Window.ClientBounds.Height / 10, 4 * (Game.Window.ClientBounds.Width / 10), (Game.Window.ClientBounds.Height / 10));
                    TexteHostMenu txt = new TexteHostMenu(Game, "Nombre de client connecté : 1 / 2", "Arial", positionTxt, new Vector2(4 * Game.Window.ClientBounds.Width / 10, 7 * Game.Window.ClientBounds.Height / 10), Color.White, 0);
                    Game.Components.Add(IpAAfficher);
                    Game.Components.Add(txt);

                    if (!ServeurCréé)
                    {
                        ServeurDeJeu = new Server(5011);
                        ServeurCréé = true;
                    }
                    if(Client == null)
                    {
                        Client = new ServeurClient(Game, IP);
                        Game.Services.AddService(typeof(ServeurClient), Client);
                    }
                }
            }
            if (positionStartGameButton.Contains(positionSouris))
            {
                if (GestionnaireInputs.EstNouveauClicGauche())
                {

                    if (AutreClientConnecté && !((Game)Game).EnJeu)
                    {

                        ((Game)Game).NumClient = 0;
                        Client.PartirPartie();
                        ((Game)Game).EnJeu = true;

                    }

                }
                if (((Game)Game).EnJeu)
                {
                    ((Game)Game).ChangerDÉtat(3);
                }


            }
        }




        private void EnleverTxt()
        {
            for (int i = Game.Components.Count - 1; i >= 0; --i)
            {
                if (Game.Components[i] is TexteHostMenu)
                {
                    Game.Components.RemoveAt(i);
                }
            }
        }
    }
}
