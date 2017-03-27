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
    class EntitéPéonEnnemie : EntitéPéon, IControlée, IDestructible
    {
        const float FACTEUR_VITESSE = 0.01f;
        public bool ÀDétruire { get; set; }
        protected bool EnMouvement { get; set; }
        public EntitéPéonEnnemie(Game jeu, string nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale,
                           float intervalleMAJ, int pointDeVie, int portée, int force, int armure)
            : base(jeu, nomModèle, échelleInitiale, rotationInitiale, positionInitiale, intervalleMAJ, pointDeVie, portée, force, armure)
        {
            // TODO: Construct any child components here
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
                Vector3 direction = new Vector3(-1, 0, 0);
                Position += FACTEUR_VITESSE * direction;
                CalculerMonde();
                foreach (EntitéPéonAlliée péon in Game.Components.Where(x => x is EntitéPéonAlliée))
                {
                    float distanceEntreLesDeux = (float)Math.Sqrt(Math.Pow((péon.Position.X - Position.X), 2) + Math.Pow((péon.Position.Z - Position.Z), 2));
                    if (distanceEntreLesDeux <= Portée)
                    {
                        EnMouvement = false; 
                    }
                }
            }

            base.Update(gameTime);
        }

        protected override void GérerDéplacement()
        {
            throw new NotImplementedException();
        }

        public void ControlerLEntitée()
        {

        }
    }
}
