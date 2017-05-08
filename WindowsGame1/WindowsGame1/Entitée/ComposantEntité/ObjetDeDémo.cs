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
    public class ObjetDeD�mo : ObjetDeBase
    {
        const float INCR�MENTATION_ANGLE = MathHelper.Pi / 120;
        const float INCR�MENTATION_HOMOTH�TIE = 0.001f;
        const float HOMOTH�TIE_MAX = 1f;
        const float HOMOTH�TIE_MIN = 0.005f;

        InputManager GestionInput { get; set; }
        protected float IntervalleMAJ { get; private set; }
        protected float Temps�coul�DepuisMAJ { get; set; }
        float Variation�chelle { get; set; }
        Vector3 RotationInitiale;
        bool DoYaw { get; set; }
        bool DoPitch { get; set; }
        bool DoRoll { get; set; }
        protected bool Monde�Recalculer { get; set; }


        public ObjetDeD�mo(Game jeu, string nomMod�le, float �chelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale,
                           float intervalleMAJ)
            : base(jeu, nomMod�le, �chelleInitiale, rotationInitiale, positionInitiale)
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
            Monde�Recalculer = false;
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
            Monde�Recalculer = false;
            float temps�coul� = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Temps�coul�DepuisMAJ += temps�coul�;
            if (Temps�coul�DepuisMAJ >= IntervalleMAJ)
            {
                G�rerHomoth�tie();
                G�rerRotation();
                if (Monde�Recalculer) { CalculerMonde(); }

                Temps�coul�DepuisMAJ = 0;
            }

            base.Update(gameTime);
        }
        



        float G�rerTouche(Keys touche)
        {
            return GestionInput.EstEnfonc�e(touche) ? INCR�MENTATION_HOMOTH�TIE : 0;
        }

        bool G�rerCommutateur(Keys touche, bool DoSomething)
        {
            return GestionInput.EstNouvelleTouche(touche) ? !DoSomething : DoSomething;
        }


        void G�rerHomoth�tie()
        {
            float �chelle = MathHelper.Max(MathHelper.Min(�chelle + Variation�chelle, HOMOTH�TIE_MAX), HOMOTH�TIE_MIN);
            if(�chelle != �chelle)
            {
                �chelle = �chelle;
                Monde�Recalculer = true;
            }
        }

        void G�rerRotation()
        {
            float variationYaw = G�rerVariation(DoYaw);
            float variationPitch = G�rerVariation(DoPitch);
            float variationRoll = G�rerVariation(DoRoll);

            Vector3 rotation = Rotation + new Vector3(variationPitch, variationYaw , variationRoll);
            if( rotation != Rotation)
            {
                Rotation = rotation;
                Monde�Recalculer = true;
            }
        }

        float G�rerVariation(bool DoSomething)
        {
            return DoSomething ? INCR�MENTATION_ANGLE : 0;
        }

        public override void CalculerMonde()
        {
            Monde = Matrix.Identity;
            Monde *= Matrix.CreateScale(�chelle);
            Monde *= Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);
            Monde *= Matrix.CreateTranslation(Position);
        }
    }
}
