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
        public float INTERVALLEMAJ = 1f / 60f;
        const int INTERVALLEMAJPÉON = 60;


        const float ÉCHELLE_OBJET_JOUEUR = 0.07f;
        const float ÉCHELLE_OBJET_PÉON = 0.003f;
        const float ÉCHELLE_OBJET_TOUR = 0.009f;
        public float ÉCHELLE_PROJECTILE_ATTAQUE_DE_BASE = 0.000009f;
        public float ÉCHELLE_PROJECTILE_W = 0.05f;

        //Constante portée
        const int PORTÉE_PÉON = 20;
        const int PORTÉE_TOUR = 30;
        const int PORTÉE_JOUEUR = 25;
        //Constante Force
        const int FORCE_PÉON = 50;
        const int FORCE_TOUR = 175;
        const int FORCE_JOUEUR = 100;
        //CONSTANTE PRÉCISION
        const int PRÉCISION_PÉON = 50;
        const int PRÉCISION_TOUR = 85;
        const int PRÉCISION_JOUEUR = 75;
        //Constante Armure
        const int ARMURE_PÉON = 10;
        const int ARMURE_TOUR = 20;
        const int ARMURE_JOUEUR = 5;
        //Constante PointDeVie
        const int PV_PÉON = 750;
        const int PV_TOUR = 3000;
        const int PV_JOUEUR = 1500;



        float TempsÉcouléDepuisMAJ = 0;
        float TempsÉcouléDepuisMAJPéon = 0;
        int NumClient { get; set; }
        int numPéonA { get; set; }
        int numTourA { get; set; }
        int numPéonE { get; set; }
        int numTourE { get; set; }

        Vector3 positionInitialeHost = new Vector3(-90, 0, 90);
        Vector3 positionInitialeInvite = new Vector3(270, 0, 90);
        Vector3 rotationObjetInitialeHost = new Vector3(0, MathHelper.PiOver2, 0);
        Vector3 rotationObjetInitialeInvite = new Vector3(0, 3 * MathHelper.PiOver2, 0);
        Vector3 rotationObjetTourHost = new Vector3(0, MathHelper.Pi, 0);
        public Vector3 RotationInitialeProjectielADB = new Vector3(0, 0, (float)-Math.PI / 4);
        public Vector3 DirectionInitialeProjectileADB = new Vector3(1, 0, 0);

        public CaméraTypéMoba CaméraJeu { get; private set; }
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
        EntitéTourAlliée TourA1 { get; set; }
        EntitéTourAlliée TourA2 { get; set; }
        Vector3 PositionInitialTourA2 = new Vector3(-50, 0, 90);
        EntitéTourAlliée TourA3 { get; set; }
        Vector3 PositionInitialTourA3 = new Vector3(48, 0, 83);
        EntitéTourEnnemie TourE1 { get; set; }
        EntitéTourEnnemie TourE2{ get; set; }
        Vector3 PositionInitialTourE2 = new Vector3(225, 0, 90);
        EntitéTourEnnemie TourE3 { get; set; }
        Vector3 PositionInitialTourE3 = new Vector3(125, 0, 83);
        Murs Murs { get; set; }
        Minuteur LeMinuteur { get; set; }


        public TheGame(Game game, int numClient)
            : base(game)
        {
            NumClient = numClient;
        }

        public override void Initialize()
        {


            GestionnaireDeTexture = Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;
            GestionnaireDeModel = Game.Services.GetService(typeof(RessourcesManager<Model>)) as RessourcesManager<Model>;
            graphics = Game.Services.GetService(typeof(GraphicsDeviceManager)) as GraphicsDeviceManager;
            GestionInput = Game.Services.GetService(typeof(InputManager)) as InputManager;
            GestionSprites = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;
            JoueurClient = Game.Services.GetService(typeof(ServeurClient)) as ServeurClient;
            Game.Components.Add(GestionInput);
            numPéonA = 1;
            numPéonE = 1;
            numTourA = 1;
            numTourE = 1;

            LeMinuteur = new Minuteur(Game);
            Game.Services.AddService(typeof(Minuteur), LeMinuteur);

            Game.Components.Add(new Afficheur3D(Game));




            if (NumClient == 0)
            {
                CaméraJeu = new CaméraTypéMoba(Game, new Vector3(-90, 30, 120), new Vector3(0, -1, -1), Vector3.Up, INTERVALLEMAJ);
                Game.Services.AddService(typeof(Caméra), CaméraJeu);
                Game.Components.Add(CaméraJeu);

                Game.Components.Add(new CartePlan(Game, 1f, Vector3.Zero, Vector3.Zero, new Vector3(225, 0, 400), "Carte Plan4", INTERVALLEMAJ));
                Murs = new Murs(Game, 1f, Vector3.Zero, Vector3.Zero, new Vector3(225, 0, 400), "Carte planMur", INTERVALLEMAJ);
                Game.Components.Add(Murs);
                Game.Services.AddService(typeof(Murs), Murs);

                //Joueurs:
                Joueur = new EntitéJoueur(Game, "robot2", ÉCHELLE_OBJET_JOUEUR, rotationObjetInitialeHost, positionInitialeHost+ new Vector3(20,0,0), INTERVALLEMAJ, PV_JOUEUR, PORTÉE_JOUEUR, FORCE_JOUEUR, ARMURE_JOUEUR, PRÉCISION_JOUEUR, new Vector3(1, 0, 0));
                Game.Components.Add(Joueur);
                JoueurEnnemie = new EntitéEnnemie(Game, "robot2", ÉCHELLE_OBJET_JOUEUR, rotationObjetInitialeInvite, positionInitialeInvite+ new Vector3(-20, 0, 0), INTERVALLEMAJ, PV_JOUEUR, PORTÉE_JOUEUR, FORCE_JOUEUR, ARMURE_JOUEUR, PRÉCISION_JOUEUR, new Vector3(-1, 0, 0));
                Game.Components.Add(JoueurEnnemie);

                //Péons Alliés:
                PéonA1 = new EntitéPéonAlliée(Game, "tank", ÉCHELLE_OBJET_PÉON, rotationObjetInitialeHost, positionInitialeHost + new Vector3(0, 1.5f, -5), INTERVALLEMAJ, PV_PÉON, PORTÉE_PÉON, FORCE_PÉON, ARMURE_PÉON, PRÉCISION_PÉON, new Vector3(1, 0, 0), numPéonA, true);
                Game.Components.Add(PéonA1);
                PéonA2 = new EntitéPéonAlliée(Game, "tank", ÉCHELLE_OBJET_PÉON, rotationObjetInitialeHost, positionInitialeHost + new Vector3(5, 1.5f, 0), INTERVALLEMAJ, PV_PÉON, PORTÉE_PÉON, FORCE_PÉON, ARMURE_PÉON, PRÉCISION_PÉON, new Vector3(1, 0, 0), ++numPéonA, true);
                Game.Components.Add(PéonA2);
                PéonA3 = new EntitéPéonAlliée(Game, "tank", ÉCHELLE_OBJET_PÉON, rotationObjetInitialeHost, positionInitialeHost + new Vector3(0, 1.5f, 5), INTERVALLEMAJ, PV_PÉON, PORTÉE_PÉON, FORCE_PÉON, ARMURE_PÉON, PRÉCISION_PÉON, new Vector3(1, 0, 0), ++numPéonA, true);
                Game.Components.Add(PéonA3);

                //Péons Ennemis :
                PéonE1 = new EntitéPéonEnnemie(Game, "tank", ÉCHELLE_OBJET_PÉON, rotationObjetInitialeInvite, positionInitialeInvite + new Vector3(0, 1.5f, -5), INTERVALLEMAJ, PV_PÉON, PORTÉE_PÉON, FORCE_PÉON, ARMURE_PÉON, PRÉCISION_PÉON, new Vector3(-1, 0, 0), numPéonE);
                Game.Components.Add(PéonE1);
                PéonE2 = new EntitéPéonEnnemie(Game, "tank", ÉCHELLE_OBJET_PÉON, rotationObjetInitialeInvite, positionInitialeInvite + new Vector3(-5, 1.5f, 0), INTERVALLEMAJ, PV_PÉON, PORTÉE_PÉON, FORCE_PÉON, ARMURE_PÉON, PRÉCISION_PÉON, new Vector3(-1, 0, 0), ++numPéonE);
                Game.Components.Add(PéonE2);
                PéonE3 = new EntitéPéonEnnemie(Game, "tank", ÉCHELLE_OBJET_PÉON, rotationObjetInitialeInvite, positionInitialeInvite + new Vector3(0, 1.5f, 5), INTERVALLEMAJ, PV_PÉON, PORTÉE_PÉON, FORCE_PÉON, ARMURE_PÉON, PRÉCISION_PÉON, new Vector3(-1, 0, 0), ++numPéonE);
                Game.Components.Add(PéonE3);

                //Tours Alliés :
                TourA1 = new EntitéTourAlliée(Game, "Defense_Tower-FBX", ÉCHELLE_OBJET_TOUR, rotationObjetTourHost, positionInitialeHost, INTERVALLEMAJ, PV_TOUR, PORTÉE_TOUR, FORCE_TOUR, ARMURE_TOUR, PRÉCISION_TOUR, numTourA);
                Game.Components.Add(TourA1);
                TourA2 = new EntitéTourAlliée(Game, "Defense_Tower-FBX", ÉCHELLE_OBJET_TOUR, rotationObjetTourHost, PositionInitialTourA2, INTERVALLEMAJ, PV_TOUR, PORTÉE_TOUR, FORCE_TOUR, ARMURE_TOUR, PRÉCISION_TOUR, ++numTourA);
                Game.Components.Add(TourA2);
                TourA3 = new EntitéTourAlliée(Game, "Defense_Tower-FBX", ÉCHELLE_OBJET_TOUR, rotationObjetTourHost, PositionInitialTourA3, INTERVALLEMAJ, PV_TOUR, PORTÉE_TOUR, FORCE_TOUR, ARMURE_TOUR, PRÉCISION_TOUR, ++numTourA);
                Game.Components.Add(TourA3);
                //Tours Ennemis
                TourE1 = new EntitéTourEnnemie(Game, "Defense_Tower-FBX", ÉCHELLE_OBJET_TOUR, Vector3.Zero, positionInitialeInvite, INTERVALLEMAJ, PV_TOUR, PORTÉE_TOUR, FORCE_TOUR, ARMURE_TOUR, PRÉCISION_TOUR, numTourE);
                Game.Components.Add(TourE1);
                TourE2 = new EntitéTourEnnemie(Game, "Defense_Tower-FBX", ÉCHELLE_OBJET_TOUR, Vector3.Zero, PositionInitialTourE2, INTERVALLEMAJ, PV_TOUR, PORTÉE_TOUR, FORCE_TOUR, ARMURE_TOUR, PRÉCISION_TOUR, ++numTourE);
                Game.Components.Add(TourE2);
                TourE3 = new EntitéTourEnnemie(Game, "Defense_Tower-FBX", ÉCHELLE_OBJET_TOUR, Vector3.Zero, PositionInitialTourE3, INTERVALLEMAJ, PV_TOUR, PORTÉE_TOUR, FORCE_TOUR, ARMURE_TOUR, PRÉCISION_TOUR, ++numTourE);
                Game.Components.Add(TourE3);
            }
            if (NumClient == 1)
            {
                CaméraJeu = new CaméraTypéMoba(Game, new Vector3(270, 30, 120), new Vector3(0, -1, -1), Vector3.Up, INTERVALLEMAJ);
                Game.Services.AddService(typeof(Caméra), CaméraJeu);
                Game.Components.Add(CaméraJeu);

                Game.Components.Add(new CartePlan(Game, 1f, Vector3.Zero, Vector3.Zero, new Vector3(225, 0, 400), "Carte Plan4", INTERVALLEMAJ));
                Murs = new Murs(Game, 1f, Vector3.Zero, Vector3.Zero, new Vector3(225, 0, 400), "Carte planMur", INTERVALLEMAJ);
                Game.Components.Add(Murs);
                Game.Services.AddService(typeof(Murs), Murs);

                //Joueurs :
                Joueur = new EntitéJoueur(Game, "robot2", ÉCHELLE_OBJET_JOUEUR, rotationObjetInitialeInvite, positionInitialeInvite + new Vector3(-20, 0, 0), INTERVALLEMAJ, PV_JOUEUR, PORTÉE_JOUEUR, FORCE_JOUEUR, ARMURE_JOUEUR, PRÉCISION_JOUEUR, new Vector3(-1, 0, 0));
                Game.Components.Add(Joueur);
                JoueurEnnemie = new EntitéEnnemie(Game, "robot2", ÉCHELLE_OBJET_JOUEUR, rotationObjetInitialeHost, positionInitialeHost + new Vector3(20, 0, 0), INTERVALLEMAJ, PV_JOUEUR, PORTÉE_JOUEUR, FORCE_JOUEUR, ARMURE_JOUEUR, PRÉCISION_JOUEUR, new Vector3(1, 0, 0));
                Game.Components.Add(JoueurEnnemie);

                //Péons Alliés:
                PéonA1 = new EntitéPéonAlliée(Game, "tank", ÉCHELLE_OBJET_PÉON, rotationObjetInitialeInvite, positionInitialeInvite + new Vector3(0, 1.5f, -5), INTERVALLEMAJ, PV_PÉON, PORTÉE_PÉON, FORCE_PÉON, ARMURE_PÉON, PRÉCISION_PÉON, new Vector3(-1, 0, 0), numPéonA, true);
                Game.Components.Add(PéonA1);
                PéonA2 = new EntitéPéonAlliée(Game, "tank", ÉCHELLE_OBJET_PÉON, rotationObjetInitialeInvite, positionInitialeInvite + new Vector3(-5, 1.5f, 0), INTERVALLEMAJ, PV_PÉON, PORTÉE_PÉON, FORCE_PÉON, ARMURE_PÉON, PRÉCISION_PÉON, new Vector3(-1, 0, 0), ++numPéonA, true);
                Game.Components.Add(PéonA2);
                PéonA3 = new EntitéPéonAlliée(Game, "tank", ÉCHELLE_OBJET_PÉON, rotationObjetInitialeInvite, positionInitialeInvite + new Vector3(0, 1.5f, 5), INTERVALLEMAJ, PV_PÉON, PORTÉE_PÉON, FORCE_PÉON, ARMURE_PÉON, PRÉCISION_PÉON, new Vector3(-1, 0, 0), ++numPéonA, true);
                Game.Components.Add(PéonA3);
                //Péons Ennemis :
                PéonE1 = new EntitéPéonEnnemie(Game, "tank", ÉCHELLE_OBJET_PÉON, rotationObjetInitialeHost, positionInitialeHost + new Vector3(0, 1.5f, -5), INTERVALLEMAJ, PV_PÉON, PORTÉE_PÉON, FORCE_PÉON, ARMURE_PÉON, PRÉCISION_PÉON, new Vector3(1, 0, 0), numPéonE);
                Game.Components.Add(PéonE1);
                PéonE2 = new EntitéPéonEnnemie(Game, "tank", ÉCHELLE_OBJET_PÉON, rotationObjetInitialeHost, positionInitialeHost + new Vector3(5, 1.5f, 0), INTERVALLEMAJ, PV_PÉON, PORTÉE_PÉON, FORCE_PÉON, ARMURE_PÉON, PRÉCISION_PÉON, new Vector3(1, 0, 0), ++numPéonE);
                Game.Components.Add(PéonE2);
                PéonE3 = new EntitéPéonEnnemie(Game, "tank", ÉCHELLE_OBJET_PÉON, rotationObjetInitialeHost, positionInitialeHost + new Vector3(0, 1.5f, 5), INTERVALLEMAJ, PV_PÉON, PORTÉE_PÉON, FORCE_PÉON, ARMURE_PÉON, PRÉCISION_PÉON, new Vector3(1, 0, 0), ++numPéonE);
                Game.Components.Add(PéonE3);

                //Tours Alliés :
                TourA1 = new EntitéTourAlliée(Game, "Defense_Tower-FBX", ÉCHELLE_OBJET_TOUR, Vector3.Zero, positionInitialeInvite, INTERVALLEMAJ, PV_TOUR, PORTÉE_TOUR, FORCE_TOUR, ARMURE_TOUR, PRÉCISION_TOUR, numTourA);
                Game.Components.Add(TourA1);
                TourA2 = new EntitéTourAlliée(Game, "Defense_Tower-FBX", ÉCHELLE_OBJET_TOUR, Vector3.Zero, PositionInitialTourE2, INTERVALLEMAJ, 1, 25, 1, 1, 1,++numTourA);
                Game.Components.Add(TourA2);
                TourA3 = new EntitéTourAlliée(Game, "Defense_Tower-FBX", ÉCHELLE_OBJET_TOUR, Vector3.Zero, PositionInitialTourE3, INTERVALLEMAJ, PV_TOUR, PORTÉE_TOUR, FORCE_TOUR, ARMURE_TOUR, PRÉCISION_TOUR, ++numTourA);
                Game.Components.Add(TourA3);
                //Tours Ennemis
                TourE1 = new EntitéTourEnnemie(Game, "Defense_Tower-FBX", ÉCHELLE_OBJET_TOUR, rotationObjetTourHost, positionInitialeHost, INTERVALLEMAJ, PV_TOUR, PORTÉE_TOUR, FORCE_TOUR, ARMURE_TOUR, PRÉCISION_TOUR, numTourE);
                Game.Components.Add(TourE1);
                TourE2 = new EntitéTourEnnemie(Game, "Defense_Tower-FBX", ÉCHELLE_OBJET_TOUR, rotationObjetTourHost, PositionInitialTourA2, INTERVALLEMAJ, 1, 25, 1, 1, 1, ++numTourE);
                Game.Components.Add(TourE2);
                TourE3 = new EntitéTourEnnemie(Game, "Defense_Tower-FBX", ÉCHELLE_OBJET_TOUR, rotationObjetTourHost, PositionInitialTourA3, INTERVALLEMAJ, PV_TOUR, PORTÉE_TOUR, FORCE_TOUR, ARMURE_TOUR, PRÉCISION_TOUR,++numTourE);
                Game.Components.Add(TourE3);
            }
            Game.Components.Add(LeMinuteur);
            Game.Components.Add(new AfficheurFPS(Game, "Arial", Color.AliceBlue, 1f));





            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            float tempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJ += tempsÉcoulé;

            if (TempsÉcouléDepuisMAJ >= INTERVALLEMAJ)
            {
                NettoyerListeComponentsEtRespawn();
                RegarderVictoireOuDéfaite();
                TempsÉcouléDepuisMAJ -= INTERVALLEMAJ;

            }
            if (TempsÉcouléDepuisMAJ >= 1)
            {
                RegarderVictoireOuDéfaite();
                TempsÉcouléDepuisMAJ -= 1;
            }
            TempsÉcouléDepuisMAJPéon += tempsÉcoulé;
            if (LeMinuteur.Minutes % 1 == 0 && LeMinuteur.Minutes != 0)
            {

                if (TempsÉcouléDepuisMAJPéon >= INTERVALLEMAJPÉON)
                {
                    InstancierNouveauPéons();
                    TempsÉcouléDepuisMAJPéon = INTERVALLEMAJ;
                }
            }

            //if (Joueur.EnMouvement)
            //{
            //    Vector3 destination = Joueur.AvoirDestination();
            //    JoueurClient.EnvoyerDestination(destination);
            //    Joueur.EnMouvement = false;
            //}
            if (Game.Components.Any(x=> x is EntitéPéonAlliée))
            {
                try
                {
                    foreach (EntitéPéonAlliée péon in Game.Components.Where(x => x is EntitéPéonAlliée))
                    {
                        if (péon.EnMouvement)
                        {
                            Vector3 laPosition = péon.Position;
                            int numPéon = péon.NumPéon;
                            JoueurClient.EnvoyerPositionPéon(laPosition, numPéon);
                        }
                    }
                }
                catch (Exception) { }
            

            }
            base.Update(gameTime);
        }


        public override void Draw(GameTime gameTime)
        {

            base.Draw(gameTime);
        }

        private void RegarderVictoireOuDéfaite()
        {
            if(TourE1.ÀDétruire||TourE1==null)
            {
                TexteCentré textefinal = new TexteCentré(Game, "Victoire!", "Arial", new Rectangle(0, 0, Game.Window.ClientBounds.Width, Game.Window.ClientBounds.Height), Color.Black, 0.2f);
                Game.Components.Add(textefinal);
                
            }
            if (TourA1.ÀDétruire || TourA1 == null)
            {
                TexteCentré textefinal = new TexteCentré(Game, "Défaite!", "Arial", new Rectangle(0, 0, Game.Window.ClientBounds.Width, Game.Window.ClientBounds.Height), Color.Black, 0.2f);
                Game.Components.Add(textefinal);
            }
        }

        private void InstancierNouveauPéons()
        {
            Game.Components.Add(new Afficheur3D(Game));
            if (NumClient == 0)
            {
                PéonA1 = new EntitéPéonAlliée(Game, "tank", ÉCHELLE_OBJET_PÉON, rotationObjetInitialeHost, positionInitialeHost + new Vector3(0, 1.5f, -5), INTERVALLEMAJ, PV_PÉON, PORTÉE_PÉON, FORCE_PÉON, ARMURE_PÉON, PRÉCISION_PÉON, new Vector3(1, 0, 0), ++numPéonA, false);
                Game.Components.Add(PéonA1);
                PéonA2 = new EntitéPéonAlliée(Game, "tank", ÉCHELLE_OBJET_PÉON, rotationObjetInitialeHost, positionInitialeHost + new Vector3(5, 1.5f, 0), INTERVALLEMAJ, PV_PÉON, PORTÉE_PÉON, FORCE_PÉON, ARMURE_PÉON, PRÉCISION_PÉON, new Vector3(1, 0, 0), ++numPéonA, false);
                Game.Components.Add(PéonA2);
                PéonA3 = new EntitéPéonAlliée(Game, "tank", ÉCHELLE_OBJET_PÉON, rotationObjetInitialeHost, positionInitialeHost + new Vector3(0, 1.5f, 5), INTERVALLEMAJ, PV_PÉON, PORTÉE_PÉON, FORCE_PÉON, ARMURE_PÉON, PRÉCISION_PÉON, new Vector3(1, 0, 0), ++numPéonA, false);
                Game.Components.Add(PéonA3);

                PéonE1 = new EntitéPéonEnnemie(Game, "tank", ÉCHELLE_OBJET_PÉON, rotationObjetInitialeInvite, positionInitialeInvite + new Vector3(0, 1.5f, -5), INTERVALLEMAJ, PV_PÉON, PORTÉE_PÉON, FORCE_PÉON, ARMURE_PÉON, PRÉCISION_PÉON, new Vector3(-1, 0, 0), ++numPéonE);
                Game.Components.Add(PéonE1);
                PéonE2 = new EntitéPéonEnnemie(Game, "tank", ÉCHELLE_OBJET_PÉON, rotationObjetInitialeInvite, positionInitialeInvite + new Vector3(-5, 1.5f, 0), INTERVALLEMAJ, PV_PÉON, PORTÉE_PÉON, FORCE_PÉON, ARMURE_PÉON, PRÉCISION_PÉON, new Vector3(-1, 0, 0), ++numPéonE);
                Game.Components.Add(PéonE2);
                PéonE3 = new EntitéPéonEnnemie(Game, "tank", ÉCHELLE_OBJET_PÉON, rotationObjetInitialeInvite, positionInitialeInvite + new Vector3(0, 1.5f, 5), INTERVALLEMAJ, PV_PÉON, PORTÉE_PÉON, FORCE_PÉON, ARMURE_PÉON, PRÉCISION_PÉON, new Vector3(-1, 0, 0), ++numPéonE);
                Game.Components.Add(PéonE3);

            }
            else
            {
                PéonA1 = new EntitéPéonAlliée(Game, "tank", ÉCHELLE_OBJET_PÉON, rotationObjetInitialeInvite, positionInitialeInvite + new Vector3(0, 1.5f, -5), INTERVALLEMAJ, PV_PÉON, PORTÉE_PÉON, FORCE_PÉON, ARMURE_PÉON, PRÉCISION_PÉON, new Vector3(-1, 0, 0), ++numPéonA, true);
                Game.Components.Add(PéonA1);
                PéonA2 = new EntitéPéonAlliée(Game, "tank", ÉCHELLE_OBJET_PÉON, rotationObjetInitialeInvite, positionInitialeInvite + new Vector3(5, 1.5f, 0), INTERVALLEMAJ, PV_PÉON, PORTÉE_PÉON, FORCE_PÉON, ARMURE_PÉON, PRÉCISION_PÉON, new Vector3(-1, 0, 0), ++numPéonA, true);
                Game.Components.Add(PéonA2);
                PéonA3 = new EntitéPéonAlliée(Game, "tank", ÉCHELLE_OBJET_PÉON, rotationObjetInitialeInvite, positionInitialeInvite + new Vector3(0, 1.5f, 5), INTERVALLEMAJ, PV_PÉON, PORTÉE_PÉON, FORCE_PÉON, ARMURE_PÉON, PRÉCISION_PÉON, new Vector3(-1, 0, 0), ++numPéonA, true);
                Game.Components.Add(PéonA3);

                PéonE1 = new EntitéPéonEnnemie(Game, "tank", ÉCHELLE_OBJET_PÉON, rotationObjetInitialeHost, positionInitialeHost + new Vector3(0, 1.5f, -5), INTERVALLEMAJ, PV_PÉON, PORTÉE_PÉON, FORCE_PÉON, ARMURE_PÉON, PRÉCISION_PÉON, new Vector3(1, 0, 0), ++numPéonE);
                Game.Components.Add(PéonE1);
                PéonE2 = new EntitéPéonEnnemie(Game, "tank", ÉCHELLE_OBJET_PÉON, rotationObjetInitialeHost, positionInitialeHost + new Vector3(-5, 1.5f, 0), INTERVALLEMAJ, PV_PÉON, PORTÉE_PÉON, FORCE_PÉON, ARMURE_PÉON, PRÉCISION_PÉON, new Vector3(1, 0, 0), ++numPéonE);
                Game.Components.Add(PéonE2);
                PéonE3 = new EntitéPéonEnnemie(Game, "tank", ÉCHELLE_OBJET_PÉON, rotationObjetInitialeHost, positionInitialeHost + new Vector3(0, 1.5f, 5), INTERVALLEMAJ, PV_PÉON, PORTÉE_PÉON, FORCE_PÉON, ARMURE_PÉON, PRÉCISION_PÉON, new Vector3(1, 0, 0), ++numPéonE);
                Game.Components.Add(PéonE3);
            }
        }


        void NettoyerListeComponentsEtRespawn()
        {
            for (int i = Game.Components.Count - 1; i >= 0; --i)
            {
                if (Game.Components[i] is IDestructible && ((IDestructible)Game.Components[i]).ÀDétruire)
                {

                    if (Game.Components[i] is EntitéJoueur)
                    {
                        if (NumClient == 0)
                        {
                            Game.Components.Add(new Afficheur3D(Game));
                            Joueur = new EntitéJoueur(Game, "robot2", ÉCHELLE_OBJET_JOUEUR, rotationObjetInitialeHost, positionInitialeHost + new Vector3(20, 0, 0), INTERVALLEMAJ, PV_JOUEUR, PORTÉE_JOUEUR, FORCE_JOUEUR, ARMURE_JOUEUR, PRÉCISION_JOUEUR, new Vector3(1, 0, 0));
                            Game.Components.Add(Joueur);
                        }
                        else
                        {
                            Game.Components.Add(new Afficheur3D(Game));
                            Joueur = new EntitéJoueur(Game, "robot2", ÉCHELLE_OBJET_JOUEUR, rotationObjetInitialeInvite, positionInitialeInvite + new Vector3(-20, 0, 0), INTERVALLEMAJ, PV_JOUEUR, PORTÉE_JOUEUR, FORCE_JOUEUR, ARMURE_JOUEUR, PRÉCISION_JOUEUR, new Vector3(-1, 0, 0));
                            Game.Components.Add(Joueur);
                        }
                    }
                    else if (Game.Components[i] is EntitéEnnemie)
                    {
                        if (NumClient == 0)
                        {
                            Game.Components.Add(new Afficheur3D(Game));
                            JoueurClient.EnvoyerEnnemiMort(0, 0);
                            JoueurEnnemie = new EntitéEnnemie(Game, "robot2", ÉCHELLE_OBJET_JOUEUR, rotationObjetInitialeInvite, positionInitialeInvite + new Vector3(-20, 0, 0), INTERVALLEMAJ, PV_JOUEUR, PORTÉE_JOUEUR, FORCE_JOUEUR, ARMURE_JOUEUR, PRÉCISION_JOUEUR, new Vector3(-1, 0, 0));
                            Game.Components.Add(JoueurEnnemie);
                        }
                        else
                        {
                            Game.Components.Add(new Afficheur3D(Game));
                            JoueurClient.EnvoyerEnnemiMort(0, 0);
                            JoueurEnnemie = new EntitéEnnemie(Game, "robot2", ÉCHELLE_OBJET_JOUEUR, rotationObjetInitialeHost, positionInitialeHost + new Vector3(20, 0, 0), INTERVALLEMAJ, PV_JOUEUR, PORTÉE_JOUEUR, FORCE_JOUEUR, ARMURE_JOUEUR, PRÉCISION_JOUEUR, new Vector3(1, 0, 0));
                            Game.Components.Add(JoueurEnnemie);
                        }               
                    }
                    else if (Game.Components[i] is EntitéPéonEnnemie)
                    {
                        int num = (Game.Components[i] as EntitéPéonEnnemie).NumPéon;
                        JoueurClient.EnvoyerEnnemiMort(1, num);
                    }
                    else if (Game.Components[i] is EntitéTourEnnemie)
                    {
                        int num = (Game.Components[i] as EntitéTourEnnemie).NumTour;
                        JoueurClient.EnvoyerEnnemiMort(2, num);
                    }
                    Game.Components.RemoveAt(i);



                }
               
            }
        }
        public void EnvoyerAttaqueAuServeur(Vector3 position, int force, int précision, int typeEnnemie, int numEnnemie, int dégat)
        {
            JoueurClient.EnvoyerAttaque(position, force, précision, typeEnnemie, numEnnemie, dégat);
        }
        public void EnvoyerMatrice(Matrix matrice)
        {
            JoueurClient.EnvoyerMatrice(matrice);
        }
        public void EnvoyerGainDeVie(int PointDeVie)
        {
            JoueurClient.EnvoyerGainDeVie(PointDeVie);
        }
        public void EnvoyerAttaqueW(Vector3 position, Vector3 direction, int force, int précision, int dégat)
        {
            JoueurClient.EnvoyerAttaqueW(position, direction, force, précision, dégat);
        }

    }
}