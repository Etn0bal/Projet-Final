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
    public class ProjectileAttaqueDeBase : Projectile
    {
        Entit� Cible { get; set; }

        public ProjectileAttaqueDeBase(Game game, string nomMod�le, float �chelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale,
                                       int force, int pr�cision, Entit� cible, float intervalleMAJ)
            : base(game, nomMod�le, �chelleInitiale, rotationInitiale, positionInitiale, force, pr�cision, intervalleMAJ)
        {
            Cible = cible;
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
            DoCalculerMonde = false;

            float temps�coul� = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Temps�coul�DepuisMAJ += temps�coul�;
            if (Temps�coul�DepuisMAJ >= IntervalleMAJ)
            {
                if (DoCalculerMonde) { CalculerMonde(); }

                Temps�coul�DepuisMAJ = 0;
            }

            base.Update(gameTime);
        }
    }
}
