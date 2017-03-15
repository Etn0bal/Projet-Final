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
    public class EntitéeJoueur : EntitéeMobile, IControlable, ICollisionable
    {
        public BoundingSphere SphèreDeCollision { get; private set; }
        Vector3 Déplacement { get; set; }
        InputManager GestionInputs { get; set; }
        Caméra CaméraJeu { get; set; }



        public EntitéeJoueur(Game jeu, string nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale,
                           float intervalleMAJ, int pointDeVie, int portée, int force, int armure)
            : base(jeu, nomModèle, échelleInitiale, rotationInitiale, positionInitiale, intervalleMAJ, pointDeVie, portée, force, armure)
        {

        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            GestionInputs = Game.Services.GetService(typeof(InputManager)) as InputManager;
            CaméraJeu = Game.Services.GetService(typeof(Caméra)) as Caméra;

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            GestionDesContrôles();
            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }


        public void GestionDesContrôles()
        {
            GérerDéplacement();
        }

        protected override void GérerDéplacement()
        {
            if(GestionInputs.EstSourisActive)
            {
                Nullable<float> résultat;
                if (GestionInputs.EstNouveauClicDroit() ) //// Regarder S'il n'y a pas d'autre entitée
                {
                    Ray pickRay = GetPickRay();
                    

                }

                    
            }

        }

        private Ray GetPickRay()
        {
            foreach (GameComponent jeu in Game.Components.Where(x => x is TheGame))
            {
                jeu.
            }
            Point positionSouris = GestionInputs.GetPositionSouris();
            Vector2 vecteurPosition = new Vector2(positionSouris.X, positionSouris.Y);
            Vector3 nearSource = new Vector3(vecteurPosition, 0);
            Vector3 farSource = new Vector3(vecteurPosition, 1);

            Vector3 nearPoint = Game.GraphicsDevice.Viewport.Unproject(nearSource, CaméraJeu.Projection, CaméraJeu.Vue, Matrix.Identity);  ////World de la caméra????     
            Vector3 farPoint = Game.GraphicsDevice.Viewport.Unproject(farSource, CaméraJeu.Projection, CaméraJeu.Vue, Matrix.Identity);  ////World de la caméra????     
            Vector3 direction = farPoint - nearPoint;
            direction = Vector3.Normalize(direction);

            return new Ray(nearPoint, direction);

        }

        public bool EstEnCollision(object autreObjet)
        {
            return false;
        }
    }
}
