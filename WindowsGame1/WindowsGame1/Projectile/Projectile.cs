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
    public abstract class Projectile : ObjetDeD�mo
    {
        int Force { get; set; }
        protected int Pr�cision { get; private set; }
        int D�gat { get; set; }
        Random G�n�rateurAl�atoire { get; set; }

        public Projectile(Game game, string nomMod�le, float �chelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale,
                          int force, int Pr�cision, float intervalleMAJ)
            : base(game, nomMod�le, �chelleInitiale, rotationInitiale, positionInitiale, intervalleMAJ)
        {
            Force = force;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            G�n�rateurAl�atoire = Game.Services.GetService(typeof(Random)) as Random;
            G�n�rerD�gat();

            base.Initialize();
        }

        protected void G�n�rerD�gat()
        {
            int impr�cision = 100 - Pr�cision;
            D�gat = G�n�rateurAl�atoire.Next(Force - Force*(impr�cision/100), Force + Force*(Pr�cision/100));
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
