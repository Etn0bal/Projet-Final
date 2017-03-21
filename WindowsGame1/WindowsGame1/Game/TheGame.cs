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


        const float ÉCHELLE_OBJET = 0.07f;
        Vector3 positionJoueur = new Vector3(-90, 0, 90);
        Vector3 positionEnnemie = new Vector3(-85, 0, 85);
        Vector3 rotationObjet = new Vector3(0, MathHelper.PiOver2, 0);

        RessourcesManager<Texture2D> GestionnaireDeTexture { get; set; }
        RessourcesManager<Model> GestionnaireDeModel { get; set; }
        GraphicsDeviceManager graphics { get; set; }
        SpriteBatch GestionSprites { get; set; }
        InputManager GestionInput { get; set; }
        EntitéeJoueur joueur { get; set; }
        EntitéeEnnemie joueurEnnemie { get; set; }
        ServeurClient joueurClient { get; set; }



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
            Game.Components.Add(new Murs(Game, 1f, Vector3.Zero, Vector3.Zero, new Vector3(225, 0, 400), "Carte planMur", INTERVALLE_MAJ));
            Game.Components.Add(GestionInput);
            joueur = new EntitéeJoueur(Game, "robot", ÉCHELLE_OBJET, rotationObjet, positionJoueur, INTERVALLE_MAJ, 1, 1, 1, 1);
            Game.Components.Add(joueur);
            joueurEnnemie = new EntitéeEnnemie(Game, "robot", ÉCHELLE_OBJET, rotationObjet, positionEnnemie, INTERVALLE_MAJ, 1, 1, 1, 1);
            Game.Components.Add(joueurEnnemie);
            Game.Components.Add(new AfficheurFPS(Game, "Arial", Color.AliceBlue, 1f));




            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            if(joueur.EnMouvement)
            {
                joueurClient.EnvoyerDéplacement(joueur.Position);
                joueur.EnMouvement = false;
            }
            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            
            base.Draw(gameTime);
        }
    }
}
