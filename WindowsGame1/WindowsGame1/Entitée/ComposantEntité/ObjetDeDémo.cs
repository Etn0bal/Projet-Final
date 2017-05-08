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
    public class ObjetDeDÈmo : ObjetDeBase
    {
        const float INCR…MENTATION_ANGLE = MathHelper.Pi / 120;
        const float INCR…MENTATION_HOMOTH…TIE = 0.001f;
        const float HOMOTH…TIE_MAX = 1f;
        const float HOMOTH…TIE_MIN = 0.005f;

        InputManager GestionInput { get; set; }
        protected float IntervalleMAJ { get; private set; }
        protected float Temps…coulÈDepuisMAJ { get; set; }
        float Variation…chelle { get; set; }
        Vector3 RotationInitiale;
        bool DoYaw { get; set; }
        bool DoPitch { get; set; }
        bool DoRoll { get; set; }
        protected bool Monde¿Recalculer { get; set; }


        public ObjetDeDÈmo(Game jeu, string nomModËle, float ÈchelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale,
                           float intervalleMAJ)
            : base(jeu, nomModËle, ÈchelleInitiale, rotationInitiale, positionInitiale)
        {
            IntervalleMAJ = intervalleMAJ;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            RotationInitiale = Rotation;
            base.Initialize();

            DoYaw = false;
            DoPitch = false;
            DoRoll = false;
            Monde¿Recalculer = false;
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            GestionInput = Game.Services.GetService(typeof(InputManager)) as InputManager;
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            Monde¿Recalculer = false;
            float temps…coulÈ = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Temps…coulÈDepuisMAJ += temps…coulÈ;
            if (Temps…coulÈDepuisMAJ >= IntervalleMAJ)
            {
                GÈrerHomothÈtie();
                GÈrerRotation();
                if (Monde¿Recalculer) { CalculerMonde(); }

                Temps…coulÈDepuisMAJ = 0;
            }

            base.Update(gameTime);
        }
        



        float GÈrerTouche(Keys touche)
        {
            return GestionInput.EstEnfoncÈe(touche) ? INCR…MENTATION_HOMOTH…TIE : 0;
        }

        bool GÈrerCommutateur(Keys touche, bool DoSomething)
        {
            return GestionInput.EstNouvelleTouche(touche) ? !DoSomething : DoSomething;
        }


        void GÈrerHomothÈtie()
        {
            float Èchelle = MathHelper.Max(MathHelper.Min(…chelle + Variation…chelle, HOMOTH…TIE_MAX), HOMOTH…TIE_MIN);
            if(Èchelle != …chelle)
            {
                …chelle = Èchelle;
                Monde¿Recalculer = true;
            }
        }

        void GÈrerRotation()
        {
            float variationYaw = GÈrerVariation(DoYaw);
            float variationPitch = GÈrerVariation(DoPitch);
            float variationRoll = GÈrerVariation(DoRoll);

            Vector3 rotation = Rotation + new Vector3(variationPitch, variationYaw , variationRoll);
            if( rotation != Rotation)
            {
                Rotation = rotation;
                Monde¿Recalculer = true;
            }
        }

        float GÈrerVariation(bool DoSomething)
        {
            return DoSomething ? INCR…MENTATION_ANGLE : 0;
        }

        public override void CalculerMonde()
        {
            Monde = Matrix.Identity;
            Monde *= Matrix.CreateScale(…chelle);
            Monde *= Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);
            Monde *= Matrix.CreateTranslation(Position);
        }
    }
}
