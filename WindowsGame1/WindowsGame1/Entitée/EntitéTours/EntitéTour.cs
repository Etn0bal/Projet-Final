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
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class EntitéTour : Entité
    {
        protected Vector3 PointMaxBDC = new Vector3(4, 7.6f, 7.5f / 2f);
        protected Vector3 PointMinBDC = new Vector3(-4, 0, -(7.5f / 2f));
        
        public int NumTour { get; set; }
        public EntitéTour(Microsoft.Xna.Framework.Game jeu, string nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale,
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

            HauteurPositionBarrePV = new Vector3(0, 8, 0);
            base.Initialize();
        }

       

       
        

        public bool EstEnCollision(Entité ent)
        {
            return BoiteDeCollision.Intersects(ent.NouvelleBoiteDeCollision);
        }
    }
}
