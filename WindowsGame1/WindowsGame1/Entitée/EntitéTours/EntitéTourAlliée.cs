﻿using System;
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
    class EntitéTourAlliée: EntitéTour, IControlée, ICollisionable, IDestructible
    {
        public bool ÀDétruire { get; set; }
        public BoundingSphere SphèreDeCollision { get; private set; }
        public Entité Cible { get; set; }
        public EntitéTourAlliée(Game jeu, string nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale,
                           float intervalleMAJ, int pointDeVie, int portée, int force, int armure, int précision)
            : base(jeu, nomModèle, échelleInitiale, rotationInitiale, positionInitiale, intervalleMAJ, pointDeVie, portée, force, armure, précision)
        {

        }
        public override void Initialize()
        {
            RayonCollision = 4;
            ÀDétruire = false;
            base.Initialize();
        }

       
        public override void Update(GameTime gameTime)
        {
            float tempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJ += tempsÉcoulé;
            if (TempsÉcouléDepuisMAJ >= IntervalleMAJ)
            {
                RegarderSiCibleEstMortOuHorsRange();
                ControlerLEntitée();
                GestionVie();
                TempsÉcouléDepuisMAJ = 0;


            }
            base.Update(gameTime);
        }

        public void ControlerLEntitée()
        {
            if(Cible!=null)
            {
                AttaquerCible();
            }
            else { RechercherEntité(); }
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
        private void RechercherEntité()
        {
            foreach(Entité entité in Game.Components.Where(x=> x is Entité))
            {
                if(Cible ==null)
                {
                    float distanceEntreLesDeux = (float)Math.Sqrt(Math.Pow((entité.Position.X - Position.X), 2) + Math.Pow((entité.Position.Z - Position.Z), 2));
                    if (distanceEntreLesDeux <= Portée)
                    {
                        if (entité is EntitéPéon)
                        {
                            Cible = entité;
                        }
                        if (entité is EntitéEnnemie)
                        {
                            Cible = entité;
                        }
                    }
                }

            }
        }
        private void AttaquerCible()
        {
          
        }
        private void GestionVie()
        {
            if (PointDeVie == 0)
            {
                ÀDétruire = true;
            }
        }

        public bool EstEnCollision(object autreObjet)
        {
            return false;
        }
    }

}
