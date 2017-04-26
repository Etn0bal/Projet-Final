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
    class EntitéPéonAlliée : EntitéPéon, IDestructible
    {
        const float FACTEUR_VITESSE = 0.01f;
       


        public bool EnMouvement { get; set; }
        bool EstPremierMinion { get; set; }
        float TempsÉcouléDepuisAttaqueMAJ { get; set; }
        Vector3 Direction { get; set; }
        public int NumPéon { get; set; }
        InputManager GestionInput { get; set; }
        Minuteur LeMinuteur;


        public EntitéPéonAlliée(Game jeu, string nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale,
                           float intervalleMAJ, int pointDeVie, int portée, int force, int armure, int précision,  Vector3 direction, int numPéon,bool estPremierMinion)
            : base(jeu, nomModèle, échelleInitiale, rotationInitiale, positionInitiale, intervalleMAJ, pointDeVie, portée, force, armure, précision)
        {
            Direction = direction;
            NumPéon = numPéon;
            EstPremierMinion = estPremierMinion;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            EnMouvement = false;
            EnRechercheDEnnemi = true;
            EstAlliée = true;

            GestionInput = Game.Services.GetService(typeof(InputManager)) as InputManager;
            LeMinuteur = Game.Services.GetService(typeof(Minuteur)) as Minuteur;

            if (EstPremierMinion == false && EnRechercheDEnnemi)
            {
                EnMouvement = true;
            }
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
                float tempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
                TempsÉcouléDepuisMAJ += tempsÉcoulé;
                if(TempsÉcouléDepuisAttaqueMAJ >= 1)
                {
                    TrouverCible();
                    TempsÉcouléDepuisAttaqueMAJ -= 1;

                }
                if (TempsÉcouléDepuisMAJ >= IntervalleMAJ)
                {
                    GestionVie();
                    TempsÉcouléDepuisMAJ -= IntervalleMAJ;
                }
            }

            if (Cible != null && !CibleEstMortOuHorsRange())
            {
                ProjectileAttaqueDeBase attaque = new ProjectileAttaqueDeBase(Game, "rocket", ÉCHELLE_PROJECTILE_ATTAQUE_DE_BASE,
                                                                RotationInitialeProjectielADB, Position, DirectionInitialeProjectileADB,
                                                                Force, Précision, Cible, IntervalleMAJ);
                Game.Components.Add(attaque);
                foreach (TheGame thegame in Game.Components.Where(x => x is TheGame))
                {
                    int typeEnnemie = 3;
                    int numEnnemie = 0;
                    if (Cible is EntitéPéonEnnemie)
                    {
                        numEnnemie = (Cible as EntitéPéonEnnemie).NumPéon;
                        typeEnnemie = 1;
                    }
                    if (Cible is EntitéTourEnnemie)
                    {
                        numEnnemie = (Cible as EntitéTourEnnemie).NumTour;
                        typeEnnemie = 2;
                    }
                    thegame.EnvoyerAttaqueAuServeur(Position, Force, Précision, typeEnnemie, numEnnemie, attaque.Dégat);
                }
            }
            else
            {
                GérerDéplacement();
            }
            




            if (LeMinuteur.Secondes == 5 && EstPremierMinion)
            {
                EnMouvement = true;
            }
            if (EstPremierMinion == false && EnRechercheDEnnemi)
            {
                EnMouvement = true;
            }
            base.Update(gameTime);
        }

        private void TrouverCible()
        {
            if(Cible == null)
            {
                try
                {
                    Cible = Game.Components.OfType<Entité>().First(x => Math.Sqrt(Math.Pow((x.Position.X - Position.X), 2) + Math.Pow((x.Position.Z - Position.Z), 2)) <= Portée && !x.EstAlliée);
                }
                catch { }

                if (Cible != null)
                {
                    EnMouvement = false;
                    EnRechercheDEnnemi = false;
                }         
            }
        }

        private void GestionVie()
        {
            if (PointDeVie == 0)
            {
                ÀDétruire = true;
            }
        }

        private bool CibleEstMortOuHorsRange()
        {
            bool ind = false;
            float distanceEntreLesDeux = (float)Math.Sqrt(Math.Pow((Cible.Position.X - Position.X), 2) + Math.Pow((Cible.Position.Z - Position.Z), 2));

            if (Cible.PointDeVie == 0 || distanceEntreLesDeux > Portée)
            {
                Cible = null;
                EnMouvement = true;
                EnRechercheDEnnemi = true;
                ind = true;
            }
            return ind;
        }

        protected void GérerDéplacement()
        {
            if(EnMouvement)
            {
                Position += Direction * FACTEUR_VITESSE;
                BoiteDeCollision = new BoundingBox(Position + PointMinBDC, Position + PointMaxBDC);
                CalculerMonde();
            }
        }
        public Vector3 AvoirPosition()
        {
            return Position;
        }
    }
}
