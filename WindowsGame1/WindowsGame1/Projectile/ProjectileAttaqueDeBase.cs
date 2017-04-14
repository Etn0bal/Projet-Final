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
    public class ProjectileAttaqueDeBase : Projectile, IDestructible
    {
        const float FACTEUR_VITESSE = 0.5f;

        Entité Cible { get; set; }
        public bool ÀDétruire { get; set; }

        public ProjectileAttaqueDeBase(Game game, string nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale,
                                       int force, int précision, Entité cible, float intervalleMAJ)
            : base(game, nomModèle, échelleInitiale, rotationInitiale, positionInitiale, force, précision, intervalleMAJ)
        {
            Cible = cible;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            ÀDétruire = false;

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            DoCalculerMonde = false;

            float tempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJ += tempsÉcoulé;
            if (TempsÉcouléDepuisMAJ >= IntervalleMAJ)
            {
                CibleAtteinte();
                GestionDéplacement();
                if (DoCalculerMonde) { CalculerMonde(); DoCalculerMonde = false; }
                TempsÉcouléDepuisMAJ = 0;
            }

            base.Update(gameTime);
        }

        void GestionDéplacement()
        {
            if (!(ÀDétruire))
            {
                Direction = Vector3.Normalize(Cible.Position - Position);

                if (Direction.X >= 0 || Direction.X <= 0) 
                {
                    Position += Direction * FACTEUR_VITESSE;
                    DoCalculerMonde = true;
                }
                else { ÀDétruire = true;  }
               
            }
        }

        void CibleAtteinte()
        {
            if((Cible.Position - Position).Length() <= Cible.RayonCollision)
            {
                ÀDétruire = true;
                Cible.RecevoirAttaque(Dégat);
            }
        }
    }
}
