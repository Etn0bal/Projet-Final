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
    public class EntitéPéon : EntitéMobile, IControlée, IDestructible
    {
        public bool ÀDétruire { get;  set; }
        public bool EnRechercheDEnnemi { get; set; }
        public EntitéPéon(Game jeu, string nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale,
                           float intervalleMAJ, int pointDeVie, int portée, int force, int armure)
            : base(jeu, nomModèle, échelleInitiale, rotationInitiale, positionInitiale, intervalleMAJ, pointDeVie, portée, force, armure)
        {
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

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
