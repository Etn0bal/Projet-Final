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
    class EntitéTourAlliée: EntitéTour
    {
        public BoundingSphere SphèreDeCollision { get; private set; }
        float TempsÉcouléDepuisAttaqueMAJ { get; set; }
        float TempsDeRechargeAttaque { get; set; }

        public EntitéTourAlliée(Microsoft.Xna.Framework.Game jeu, string nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale,
                           float intervalleMAJ, int pointDeVie, int portée, int force, int armure, int précision, int numTour)
            : base(jeu, nomModèle, échelleInitiale, rotationInitiale, positionInitiale, intervalleMAJ, pointDeVie, portée, force, armure, précision, numTour)
        {

        }
        public override void Initialize()
        {
            RayonCollision = 4;
            ÀDétruire = false;
            EstAlliée = true;
            TempsDeRechargeAttaque = 1.2f;
            base.Initialize();
        }

       
        public override void Update(GameTime gameTime)
        {
            float tempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJ += tempsÉcoulé;
            if (TempsÉcouléDepuisMAJ >= IntervalleMAJ)
            {
                RegarderSiCibleEstMortOuHorsRange();
                TempsÉcouléDepuisMAJ -= IntervalleMAJ;
            }

            TempsÉcouléDepuisAttaqueMAJ += tempsÉcoulé;
            if (TempsÉcouléDepuisAttaqueMAJ >= TempsDeRechargeAttaque)
            {
                //RegarderSiCibleEstMortOuHorsRange();
                //if (Cible == null)
                //{
                    GestionAttaque();
                //}
            }
            base.Update(gameTime);
            base.Update(gameTime);
        }

        protected void GestionAttaque()
        {
            Cible = Game.Components.OfType<Entité>().FirstOrDefault(x => Math.Sqrt(Math.Pow(x.Position.X - Position.X, 2) +
                                                                    Math.Pow(x.Position.Z - Position.Z, 2)) <= Portée && !x.EstAlliée);

            if (Cible != null)
            {
                ProjectileAttaqueDeBase attaque = new ProjectileAttaqueDeBase(Game, "rocket", ÉCHELLE_PROJECTILE_ATTAQUE_DE_BASE,
                                                                                      RotationInitialeProjectielADB, Position, DirectionInitialeProjectileADB,
                                                                                      Force, Précision, Cible, IntervalleMAJ);
                Game.Components.Add(attaque);
                
                    int typeEnnemie = 3;
                    int numEnnemie = 0;
                    if (Cible is EntitéPéonEnnemie)
                    {
                        numEnnemie = (Cible as EntitéPéonEnnemie).NumPéon;
                        typeEnnemie = 1;
                    }

                    LeJeu.EnvoyerAttaqueAuServeur(Position, Force, Précision, typeEnnemie, numEnnemie, attaque.Dégat);
                    Cible = null;
                TempsÉcouléDepuisAttaqueMAJ = 0;
                
            }
        }

  

        private void RegarderSiCibleEstMortOuHorsRange()
        {
            if (Cible != null)
            {
                float distanceEntreLesDeux = (float)Math.Sqrt(Math.Pow((Cible.Position.X - Position.X), 2) + Math.Pow((Cible.Position.Z - Position.Z), 2));

                if (Cible.PointDeVie == 0 || distanceEntreLesDeux > Portée)
                {
                    Cible = null;
                }
            }
        }
       

    }

}
