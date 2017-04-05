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
using System.Net.Sockets;
using System.Net;
using System.Windows.Forms;
using System.IO;


namespace AtelierXNA
{

    public class TheGame : Microsoft.Xna.Framework.DrawableGameComponent
    {

        public CaméraTypéMoba CaméraJeu { get; private set; }
        const float INTERVALLE_MAJ = 1f / 60f;


        const float ÉCHELLE_OBJET_JOUEUR = 0.07f;
        const float ÉCHELLE_OBJET_PÉON = 0.03f;

        int NumClient { get; set; }

        Vector3 positionInitialeHost = new Vector3(-90, 0, 90);
        Vector3 positionInitialeInvite = new Vector3(270, 0, 90);
        Vector3 rotationObjetInitialeHost = new Vector3(0, MathHelper.PiOver2, 0);
        Vector3 rotationObjetInitialeInvite = new Vector3(0, 3 * MathHelper.PiOver2, 0);


        RessourcesManager<Texture2D> GestionnaireDeTexture { get; set; }
        RessourcesManager<Model> GestionnaireDeModel { get; set; }
        GraphicsDeviceManager graphics { get; set; }
        SpriteBatch GestionSprites { get; set; }
        InputManager GestionInput { get; set; }


        EntitéJoueur Joueur { get; set; }
        EntitéEnnemie JoueurEnnemie { get; set; }
        ServeurClient JoueurClient { get; set; }
        EntitéPéonAlliée PéonA1 { get; set; }
        EntitéPéonAlliée PéonA2 { get; set; }
        EntitéPéonAlliée PéonA3 { get; set; }
        EntitéPéonEnnemie PéonE1 { get; set; }
        EntitéPéonEnnemie PéonE2 { get; set; }
        EntitéPéonEnnemie PéonE3 { get; set; }

        Murs Murs { get; set; }



        public TheGame(Game game, int numClient)
            : base(game)
        {
            NumClient = numClient;
        }


        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {


            GestionnaireDeTexture = Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;
            GestionnaireDeModel = Game.Services.GetService(typeof(RessourcesManager<Model>)) as RessourcesManager<Model>;
            graphics = Game.Services.GetService(typeof(GraphicsDeviceManager)) as GraphicsDeviceManager;
            GestionInput = Game.Services.GetService(typeof(InputManager)) as InputManager;
            GestionSprites = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;
            JoueurClient = Game.Services.GetService(typeof(ServeurClient)) as ServeurClient;
            Game.Components.Add(GestionInput);
            Game.Components.Add(new Afficheur3D(Game));






            if (NumClient == 0)
            {
                CaméraJeu = new CaméraTypéMoba(Game, new Vector3(-90, 30, 120), new Vector3(0, -1, -1), Vector3.Up, INTERVALLE_MAJ);
                Game.Services.AddService(typeof(Caméra), CaméraJeu);
                Game.Components.Add(CaméraJeu);

                Game.Components.Add(new CartePlan(Game, 1f, Vector3.Zero, Vector3.Zero, new Vector3(225, 0, 400), "Carte Plan4", INTERVALLE_MAJ));
                Murs = new Murs(Game, 1f, Vector3.Zero, Vector3.Zero, new Vector3(225, 0, 400), "Carte planMur", INTERVALLE_MAJ);
                Game.Components.Add(Murs);
                Game.Services.AddService(typeof(Murs), Murs);

                Joueur = new EntitéJoueur(Game, "robot2", ÉCHELLE_OBJET_JOUEUR, rotationObjetInitialeHost, positionInitialeHost, INTERVALLE_MAJ, 1, 1, 1, 1, new Vector3(1, 0, 0));
                Game.Components.Add(Joueur);
                JoueurEnnemie = new EntitéEnnemie(Game, "robot2", ÉCHELLE_OBJET_JOUEUR, rotationObjetInitialeInvite, positionInitialeInvite, INTERVALLE_MAJ, 1, 1, 1, 1, new Vector3(-1, 0, 0));
                Game.Components.Add(JoueurEnnemie);
                //Péons :
                PéonA1 = new EntitéPéonAlliée(Game, "robot2", ÉCHELLE_OBJET_PÉON, rotationObjetInitialeHost, positionInitialeHost - new Vector3(0, 0, 5), INTERVALLE_MAJ, 1, 3, 1, 1, new Vector3(1, 0, 0), 1);
                Game.Components.Add(PéonA1);
                PéonA2 = new EntitéPéonAlliée(Game, "robot2", ÉCHELLE_OBJET_PÉON, rotationObjetInitialeHost, positionInitialeHost + new Vector3(5, 0, 0), INTERVALLE_MAJ, 1, 3, 1, 1, new Vector3(1, 0, 0), 2);
                Game.Components.Add(PéonA2);
                PéonA3 = new EntitéPéonAlliée(Game, "robot2", ÉCHELLE_OBJET_PÉON, rotationObjetInitialeHost, positionInitialeHost + new Vector3(0, 0, 5), INTERVALLE_MAJ, 1, 3, 1, 1, new Vector3(1, 0, 0), 3);
                Game.Components.Add(PéonA3);

                //Péons :
                PéonE1 = new EntitéPéonEnnemie(Game, "robot2", ÉCHELLE_OBJET_PÉON, rotationObjetInitialeInvite, positionInitialeInvite - new Vector3(0, 0, 5), INTERVALLE_MAJ, 1, 3, 1, 1, new Vector3(-1, 0, 0), 1);
                Game.Components.Add(PéonE1);
                PéonE2 = new EntitéPéonEnnemie(Game, "robot2", ÉCHELLE_OBJET_PÉON, rotationObjetInitialeInvite, positionInitialeInvite - new Vector3(5, 0, 0), INTERVALLE_MAJ, 1, 3, 1, 1, new Vector3(-1, 0, 0), 2);
                Game.Components.Add(PéonE2);
                PéonE3 = new EntitéPéonEnnemie(Game, "robot2", ÉCHELLE_OBJET_PÉON, rotationObjetInitialeInvite, positionInitialeInvite + new Vector3(0, 0, 5), INTERVALLE_MAJ, 1, 3, 1, 1, new Vector3(-1, 0, 0), 3);
                Game.Components.Add(PéonE3);
            }
            if (NumClient == 1)
            {
                CaméraJeu = new CaméraTypéMoba(Game, new Vector3(270, 30, 115), new Vector3(0, -1, -1), Vector3.Up, INTERVALLE_MAJ);
                Game.Services.AddService(typeof(Caméra), CaméraJeu);
                Game.Components.Add(CaméraJeu);

                Game.Components.Add(new CartePlan(Game, 1f, Vector3.Zero, Vector3.Zero, new Vector3(225, 0, 400), "Carte Plan4", INTERVALLE_MAJ));
                Murs = new Murs(Game, 1f, Vector3.Zero, Vector3.Zero, new Vector3(225, 0, 400), "Carte planMur", INTERVALLE_MAJ);
                Game.Components.Add(Murs);
                Game.Services.AddService(typeof(Murs), Murs);


                //Joueurs :
                Joueur = new EntitéJoueur(Game, "robot2", ÉCHELLE_OBJET_JOUEUR, rotationObjetInitialeInvite, positionInitialeInvite, INTERVALLE_MAJ, 1, 1, 1, 1, new Vector3(-1, 0, 0));
                Game.Components.Add(Joueur);
                JoueurEnnemie = new EntitéEnnemie(Game, "robot2", ÉCHELLE_OBJET_JOUEUR, rotationObjetInitialeHost, positionInitialeHost, INTERVALLE_MAJ, 1, 1, 1, 1, new Vector3(1, 0, 0));
                Game.Components.Add(JoueurEnnemie);
                //Péons :
                PéonA1 = new EntitéPéonAlliée(Game, "robot2", ÉCHELLE_OBJET_PÉON, rotationObjetInitialeInvite, positionInitialeInvite - new Vector3(0, 0, 5), INTERVALLE_MAJ, 1, 3, 1, 1, new Vector3(-1, 0, 0), 1);
                Game.Components.Add(PéonA1);
                PéonA2 = new EntitéPéonAlliée(Game, "robot2", ÉCHELLE_OBJET_PÉON, rotationObjetInitialeInvite, positionInitialeInvite - new Vector3(5, 0, 0), INTERVALLE_MAJ, 1, 3, 1, 1, new Vector3(-1, 0, 0), 2);
                Game.Components.Add(PéonA2);
                PéonA3 = new EntitéPéonAlliée(Game, "robot2", ÉCHELLE_OBJET_PÉON, rotationObjetInitialeInvite, positionInitialeInvite + new Vector3(0, 0, 5), INTERVALLE_MAJ, 1, 3, 1, 1, new Vector3(-1, 0, 0), 3);
                Game.Components.Add(PéonA3);

                //Péons :
                PéonE1 = new EntitéPéonEnnemie(Game, "robot2", ÉCHELLE_OBJET_PÉON, rotationObjetInitialeHost, positionInitialeHost - new Vector3(0, 0, 5), INTERVALLE_MAJ, 1, 3, 1, 1, new Vector3(1, 0, 0), 1);
                Game.Components.Add(PéonE1);
                PéonE2 = new EntitéPéonEnnemie(Game, "robot2", ÉCHELLE_OBJET_PÉON, rotationObjetInitialeHost, positionInitialeHost - new Vector3(5, 0, 0), INTERVALLE_MAJ, 1, 3, 1, 1, new Vector3(1, 0, 0), 2);
                Game.Components.Add(PéonE2);
                PéonE3 = new EntitéPéonEnnemie(Game, "robot2", ÉCHELLE_OBJET_PÉON, rotationObjetInitialeHost, positionInitialeHost + new Vector3(0, 0, 5), INTERVALLE_MAJ, 1, 3, 1, 1, new Vector3(1, 0, 0), 3);
                Game.Components.Add(PéonE3);
            }



            Game.Components.Add(new AfficheurFPS(Game, "Arial", Color.AliceBlue, 1f));





            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            if (Joueur.EnMouvement)
            {
                Vector3 destination = Joueur.AvoirDestination();
                JoueurClient.EnvoyerDestination(destination);
                Joueur.EnMouvement = false;
            }
            foreach (EntitéPéonAlliée entité in Game.Components.Where(x => x is EntitéPéonAlliée))
            {
                if (entité.EnMouvement == true)
                {
                    Vector3 position = entité.AvoirPosition();
                    int numPéon = entité.NumPéon;
                    JoueurClient.EnvoyerPositionPéon(position, numPéon);

                }
            }


            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {

            base.Draw(gameTime);
        }
        public void GérerDéplacementPéon(Vector3 positionPéon, int numPéon)
        {
            foreach (EntitéPéonEnnemie péon in Game.Components.Where(x => x is EntitéPéonEnnemie))
            {
                if (péon.NumPéon == numPéon)
                {
                    péon.GérerDéplacement(positionPéon);
                }

            }
        }
    }
}
