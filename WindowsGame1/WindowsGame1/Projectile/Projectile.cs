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
    public abstract class Projectile : ObjetDeDémo
    {
        int Force { get; set; }
        protected int Précision { get; private set; }
        public int Dégat { get; protected set; }
        protected Vector3 DirectionDéplacement { get; set; }
        Random GénérateurAléatoire { get; set; }
        protected Vector3 Direction { get; set; }


        public Projectile(Game game, string nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale,Vector3 direction,
                          int force, int Précision, float intervalleMAJ)
            : base(game, nomModèle, échelleInitiale, rotationInitiale, positionInitiale, intervalleMAJ)
        {
            Force = force;
            GénérateurAléatoire = Game.Services.GetService(typeof(Random)) as Random;
            GénérerDégat();
            Direction = direction;
        }

        public Projectile(Game game, string nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, Vector3 direction,
                          int force, int Précision, int dégat, float intervalleMAJ)
            : base(game, nomModèle, échelleInitiale, rotationInitiale, positionInitiale, intervalleMAJ)
        {
            Force = force;
            Dégat = dégat;
            Direction = direction;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
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
