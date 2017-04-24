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
    public class EntitéPéon : Entité, IDestructible
    {
        protected Vector3 PointMaxBDC = new Vector3(5f/2f , 3, 3f / 2f);
        protected Vector3 PointMinBDC = new Vector3(-(5/2f), 0, -(3f / 2f));


        public bool ÀDétruire { get;  set; }
        protected bool EnRechercheDEnnemi { get; set; }

        public EntitéPéon(Game jeu, string nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale,
                           float intervalleMAJ, int pointDeVie, int portée, int force, int armure, int précision)
            : base(jeu, nomModèle, échelleInitiale, rotationInitiale, positionInitiale, intervalleMAJ, pointDeVie, portée, force, armure, précision)
        {
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            Cible = null;
            BoiteDeCollision = new BoundingBox(Position + PointMinBDC, Position + PointMaxBDC);
            RayonCollision = 1;
            EnRechercheDEnnemi = true;
            ÀDétruire = false;

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if(PointDeVie== 0)
            {
                ÀDétruire = true;
            }

            base.Update(gameTime);
        }
    }
}
