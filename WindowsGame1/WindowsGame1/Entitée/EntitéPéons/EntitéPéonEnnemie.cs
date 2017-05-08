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
    class EntitéPéonEnnemie : EntitéPéon
    {
        const float FACTEUR_VITESSE = 0.01f;
        protected bool EnMouvement { get; set; }
        Vector3 Direction { get; set; }
        public int NumPéon { get; set; }
        Vector3 Destination { get; set; }
        Vector3 DirectionDéplacement { get; set; }
        public EntitéPéonEnnemie(Game jeu, string nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale,
                           float intervalleMAJ, int pointDeVie, int portée, int force, int armure, int précision, Vector3 direction,int numPéon)
            : base(jeu, nomModèle, échelleInitiale, rotationInitiale, positionInitiale, intervalleMAJ, pointDeVie, portée, force, armure, précision)
        {
            Direction = direction;
            NumPéon = numPéon;

        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            EstAlliée = false;
            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            float tempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJ += tempsÉcoulé;
            if (TempsÉcouléDepuisMAJ >= IntervalleMAJ)
            {
                GestionDéplacement();
                TempsÉcouléDepuisMAJ = 0;

            }
            if (MondeÀRecalculer)
            {
                CalculerMonde();
                MondeÀRecalculer = false;
            }
            base.Update(gameTime);
        }
        public override void GestionDéplacement()
        {
            if (EnMouvement)
            {
                Position += Direction * FACTEUR_VITESSE;
                BoiteDeCollision = new BoundingBox(Position + PointMinBDC, Position + PointMaxBDC);
                CalculerMonde();
                EnMouvement = false;
            }

        }
        public void GérerDéplacement(Vector3 position)
        {
            Position = position;
            BoiteDeCollision = new BoundingBox(Position + PointMinBDC, Position + PointMaxBDC);
            EnMouvement = true;
            CalculerMonde();
        }

    }
}
