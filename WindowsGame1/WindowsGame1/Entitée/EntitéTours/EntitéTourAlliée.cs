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

            base.Initialize();
        }

       
        public override void Update(GameTime gameTime)
        {
            float tempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJ += tempsÉcoulé;
            if (TempsÉcouléDepuisMAJ >= IntervalleMAJ)
            {
                ControlerLEntitée();
            }
                base.Update(gameTime);
        }

        public void ControlerLEntitée()
        {
            if(Cible!=null)
            {
                
            }
            else { RechercherEntité(); }
        }

        private void RechercherEntité()
        {
            foreach(Entité entité in Game.Components.Where(x=> x is Entité))
            {
                
            }
        }

        public bool EstEnCollision(object autreObjet)
        {
            return false;
        }
    }

}
