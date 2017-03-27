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

        public Caméra CaméraJeu { get; private set; }
        const float INTERVALLE_MAJ = 1f / 60f;


        const float ÉCHELLE_OBJET_JOUEUR = 0.07f;
        const float ÉCHELLE_OBJET_PÉON = 0.03f;

        Vector3 positionInitiale = new Vector3(-90, 0, 90);
        Vector3 positionInitialeEnnemie = new Vector3(180, 0, 90);
        Vector3 rotationObjetInitiale = new Vector3(0, MathHelper.PiOver2, 0);
        Vector3 rotationObjetInitialeEnnemie = new Vector3(0,3* MathHelper.PiOver2, 0);


        RessourcesManager<Texture2D> GestionnaireDeTexture { get; set; }
        RessourcesManager<Model> GestionnaireDeModel { get; set; }
        GraphicsDeviceManager graphics { get; set; }
        SpriteBatch GestionSprites { get; set; }
        InputManager GestionInput { get; set; }
        EntitéJoueur joueur { get; set; }
        EntitéEnnemie joueurEnnemie { get; set; }
        ServeurClient joueurClient { get; set; }
        EntitéPéonAlliée PéonA1 { get; set; }
        EntitéPéonAlliée PéonA2 { get; set; }
        EntitéPéonAlliée PéonA3 { get; set; }
        EntitéPéonEnnemie PéonE1 { get; set; }
        EntitéPéonEnnemie PéonE2 { get; set; }
        EntitéPéonEnnemie PéonE3 { get; set; }
        Murs Murs { get; set; }



        public TheGame(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
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
            joueurClient = Game.Services.GetService(typeof(ServeurClient)) as ServeurClient;

            

            CaméraJeu = new CaméraTypéMoba(Game, new Vector3(-85, 30, 115), new Vector3(0, -1, -1), Vector3.Up, INTERVALLE_MAJ);
            Game.Services.AddService(typeof(Caméra), CaméraJeu);

           
            Game.Components.Add(new Afficheur3D(Game));
            Game.Components.Add(CaméraJeu);
            Game.Components.Add(new CartePlan(Game, 1f, Vector3.Zero, Vector3.Zero, new Vector3(225, 0, 400), "Carte Plan4", INTERVALLE_MAJ));

            Murs = new Murs(Game, 1f, Vector3.Zero, Vector3.Zero, new Vector3(225, 0, 400), "Carte planMur", INTERVALLE_MAJ);
            Game.Components.Add(Murs);
            Game.Services.AddService(typeof(Murs), Murs);

            Game.Components.Add(GestionInput);

            //Joueurs :
            joueur = new EntitéJoueur(Game, "robot", ÉCHELLE_OBJET_JOUEUR, rotationObjetInitiale, positionInitiale, INTERVALLE_MAJ, 1, 1, 1, 1);
            Game.Components.Add(joueur);
            joueurEnnemie = new EntitéEnnemie(Game, "robot", ÉCHELLE_OBJET_JOUEUR, rotationObjetInitiale, positionInitiale, INTERVALLE_MAJ, 1, 1, 1, 1);
            Game.Components.Add(joueurEnnemie);
            //Péons :
            PéonA1 = new EntitéPéonAlliée(Game, "robot", ÉCHELLE_OBJET_PÉON, rotationObjetInitiale, positionInitiale - new Vector3(0,0,5), INTERVALLE_MAJ, 1, 1, 1, 1);
            Game.Components.Add(PéonA1);
            PéonA2 = new EntitéPéonAlliée(Game, "robot", ÉCHELLE_OBJET_PÉON, rotationObjetInitiale, positionInitiale + new Vector3(5, 0, 0), INTERVALLE_MAJ, 1, 1, 1, 1);
            Game.Components.Add(PéonA2);
            PéonA3 = new EntitéPéonAlliée(Game, "robot", ÉCHELLE_OBJET_PÉON, rotationObjetInitiale, positionInitiale+ new Vector3(0, 0, 5), INTERVALLE_MAJ, 1, 1, 1, 1);
            Game.Components.Add(PéonA3);

            //Péons :
            PéonE1 = new EntitéPéonEnnemie(Game, "robot", ÉCHELLE_OBJET_PÉON, rotationObjetInitialeEnnemie, positionInitialeEnnemie - new Vector3(0, 0, 5), INTERVALLE_MAJ, 1, 1, 1, 1);
            Game.Components.Add(PéonE1);
            PéonE2 = new EntitéPéonEnnemie(Game, "robot", ÉCHELLE_OBJET_PÉON, rotationObjetInitialeEnnemie, positionInitialeEnnemie - new Vector3(5, 0, 0), INTERVALLE_MAJ, 1, 1, 1, 1);
            Game.Components.Add(PéonE2);
            PéonE3 = new EntitéPéonEnnemie(Game, "robot", ÉCHELLE_OBJET_PÉON, rotationObjetInitialeEnnemie, positionInitialeEnnemie + new Vector3(0, 0, 5), INTERVALLE_MAJ, 1, 1, 1, 1);
            Game.Components.Add(PéonE3);


            Game.Components.Add(new AfficheurFPS(Game, "Arial", Color.AliceBlue, 1f));





            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            //if(joueur.EnMouvement)
            //{
            //    Vector3 destination = joueur.AvoirDestination();
            //    joueurClient.EnvoyerDestination(destination);
            //    joueur.EnMouvement = false;
            //}
            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            
            base.Draw(gameTime);
        }
    }
}
