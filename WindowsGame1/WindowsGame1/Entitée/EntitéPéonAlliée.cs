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
    class EntitéPéonAlliée : EntitéPéon, IControlée, IDestructible
    {
        const float FACTEUR_VITESSE = 0.01f;
        public bool EnMouvement { get; set; }
        Vector3 Direction { get; set; }
        public int NumPéon { get; set; }
        
        public EntitéPéonAlliée(Game jeu, string nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale,
                           float intervalleMAJ, int pointDeVie, int portée, int force, int armure, int précision,  Vector3 direction, int numPéon)
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
            EnMouvement = true;

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if (EnMouvement)
            {
                GérerDéplacement();
            }
            if (Cible != null)
            {
                AttaquerLaCible();
                RegarderSiCibleEstMortOuHorsRange();
            }

            base.Update(gameTime);
        }

        private void AttaquerLaCible()
        {
            Cible.RecevoirAttaque(Force);
        }

        private void RegarderSiCibleEstMortOuHorsRange()
        {
            float distanceEntreLesDeux = (float)Math.Sqrt(Math.Pow((Cible.Position.X - Position.X), 2) + Math.Pow((Cible.Position.Z - Position.Z), 2));

            if (Cible.PointDeVie == 0 || distanceEntreLesDeux > Portée)
            {
                Cible = null;
            }
    
        }

        protected void GérerDéplacement()
        {
            Position += Direction * FACTEUR_VITESSE;
            CalculerMonde();
            foreach (EntitéPéonEnnemie péon in Game.Components.Where(x => x is EntitéPéonEnnemie))
            {
                float distanceEntreLesDeux = (float)Math.Sqrt(Math.Pow((péon.Position.X - Position.X), 2) + Math.Pow((péon.Position.Z - Position.Z), 2));
                if (distanceEntreLesDeux <= Portée && EnRechercheDEnnemi == true)
                {
                    EnMouvement = false;
                    EnRechercheDEnnemi = false;
                    Cible = péon;

                }
            }
        }
        public Vector3 AvoirPosition()
        {
            return Position;
        }
    }
}
