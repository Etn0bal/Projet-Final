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


namespace AtelierXNA.Projectile
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public abstract class Projectile : ObjetDeDémo
    {
        int Force { get; set; }
        protected int Précision { get; private set; }
        int Dégat { get; set; }
        Random GénérateurAléatoire { get; set; }

        public Projectile(Game game, string nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale,
                          int force, int Précision, float intervalleMAJ)
            : base(game, nomModèle, échelleInitiale, rotationInitiale, positionInitiale, intervalleMAJ)
        {
            Force = force;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            GénérateurAléatoire = Game.Services.GetService(typeof(Random)) as Random;
            GénérerDégat();

            base.Initialize();
        }

        protected void GénérerDégat()
        {
            int imprécision = 100 - Précision;
            Dégat = GénérateurAléatoire.Next(Force - Force*(imprécision/100), Force + Force*(Précision/100));
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
    }
}
