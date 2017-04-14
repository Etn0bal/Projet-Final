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
    public class ObjetDeDémo : ObjetDeBase
    {
        const float INCRÉMENTATION_ANGLE = MathHelper.Pi / 120;
        const float INCRÉMENTATION_HOMOTHÉTIE = 0.001f;
        const float HOMOTHÉTIE_MAX = 1f;
        const float HOMOTHÉTIE_MIN = 0.005f;

        InputManager GestionInput { get; set; }
        protected float IntervalleMAJ { get; private set; }
        protected float TempsÉcouléDepuisMAJ { get; set; }
        float VariationÉchelle { get; set; }
        Vector3 RotationInitiale;
        bool DoYaw { get; set; }
        bool DoPitch { get; set; }
        bool DoRoll { get; set; }
        protected bool DoCalculerMonde { get; set; }


        public ObjetDeDémo(Game jeu, string nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale,
                           float intervalleMAJ)
            : base(jeu, nomModèle, échelleInitiale, rotationInitiale, positionInitiale)
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
            DoCalculerMonde = false;
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
            DoCalculerMonde = false;
            GérerClavier();
            float tempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJ += tempsÉcoulé;
            if (TempsÉcouléDepuisMAJ >= IntervalleMAJ)
            {
                GérerHomothétie();
                GérerRotation();
                if (DoCalculerMonde) { CalculerMonde(); }

                TempsÉcouléDepuisMAJ = 0;
            }

            base.Update(gameTime);
        }
        

        void GérerClavier()
        {
            VariationÉchelle = GérerTouche(Keys.OemPlus) - GérerTouche(Keys.OemMinus);

            DoYaw = GérerCommutateur(Keys.NumPad1, DoYaw);
            DoPitch = GérerCommutateur(Keys.NumPad2, DoPitch);
            DoRoll = GérerCommutateur(Keys.NumPad3, DoRoll);

            GérerReset();
        }

        float GérerTouche(Keys touche)
        {
            return GestionInput.EstEnfoncée(touche) ? INCRÉMENTATION_HOMOTHÉTIE : 0;
        }

        bool GérerCommutateur(Keys touche, bool DoSomething)
        {
            return GestionInput.EstNouvelleTouche(touche) ? !DoSomething : DoSomething;
        }

        void GérerReset()
        {
            if (GestionInput.EstNouvelleTouche(Keys.Space) && Rotation != RotationInitiale)
            {
                Rotation = RotationInitiale;
                DoCalculerMonde = true;
            }
        }

        void GérerHomothétie()
        {
            float échelle = MathHelper.Max(MathHelper.Min(Échelle + VariationÉchelle, HOMOTHÉTIE_MAX), HOMOTHÉTIE_MIN);
            if(échelle != Échelle)
            {
                Échelle = échelle;
                DoCalculerMonde = true;
            }
        }

        void GérerRotation()
        {
            float variationYaw = GérerVariation(DoYaw);
            float variationPitch = GérerVariation(DoPitch);
            float variationRoll = GérerVariation(DoRoll);

            Vector3 rotation = Rotation + new Vector3(variationPitch, variationYaw , variationRoll);
            if( rotation != Rotation)
            {
                Rotation = rotation;
                DoCalculerMonde = true;
            }
        }

        float GérerVariation(bool DoSomething)
        {
            return DoSomething ? INCRÉMENTATION_ANGLE : 0;
        }

        public override void CalculerMonde()
        {
            Monde = Matrix.Identity;
            Monde *= Matrix.CreateScale(Échelle);
            Monde *= Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);
            Monde *= Matrix.CreateTranslation(Position);
        }
    }
}
