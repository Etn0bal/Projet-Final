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
    public class EntitéTour : Entité
    {
        protected Vector3 PointMaxBDC = new Vector3(5, 7.6f, 9f / 2f);
        protected Vector3 PointMinBDC = new Vector3(-5, 0, -(9f / 2f));
        
        protected float TempsÉcouléDepuisAttaqueMAJ { get; set; }
        public int NumTour { get; set; }
        public EntitéTour(Game jeu, string nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale,
                           float intervalleMAJ, int pointDeVie, int portée, int force, int armure, int précision,int numTour)
            : base(jeu, nomModèle, échelleInitiale, rotationInitiale, positionInitiale, intervalleMAJ, pointDeVie, portée, force, armure, précision)
        {
            NumTour = numTour;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            RayonCollision = 4;
            BoiteDeCollision = new BoundingBox(Position + PointMinBDC, Position + PointMaxBDC);
            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            float tempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisAttaqueMAJ += tempsÉcoulé;
            if (TempsÉcouléDepuisAttaqueMAJ >= 1.2f)
            {
            }
            base.Update(gameTime);
        }


       
        

        public bool EstEnCollision(Entité ent)
        {
            return BoiteDeCollision.Intersects(ent.NouvelleBoiteDeCollision);
        }
    }
}
