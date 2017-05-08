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
    public class CaméraTypéMoba : Caméra
    {
        const float VITESSE_INITIALE_TRANSLATION = 0.5f;
        Vector3 VariationEntrePositionCamJoueur = new Vector3(0, 30, 30);


        Vector3 Direction { get; set; }
        Vector3 Latéral { get; set; }
        Vector3 PositionJoueur { get; set; }
        float VitesseTranslation { get; set; }
        float IntervalleMAJ { get; set; }
        float TempsÉcouléDepuisMAJ { get; set; }
        InputManager GestionInput { get; set; }



        public CaméraTypéMoba(Microsoft.Xna.Framework.Game game, Vector3 positionCaméra, Vector3 direction, Vector3 orientation, float intervalleMAJ)
            : base(game)
        {
            IntervalleMAJ = intervalleMAJ;
            Direction = direction;
            CréerVolumeDeVisualisation(OUVERTURE_OBJECTIF, DISTANCE_PLAN_RAPPROCHÉ, DISTANCE_PLAN_ÉLOIGNÉ);
            CréerPointDeVue(positionCaméra, direction, orientation);
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            
            VitesseTranslation = VITESSE_INITIALE_TRANSLATION;
            TempsÉcouléDepuisMAJ = 0;
            base.Initialize();
            GestionInput = Game.Services.GetService(typeof(InputManager)) as InputManager;
        }

        protected override void CréerPointDeVue()
        {
            //Création de la matrice de vue (point de vue)
            Vue = Matrix.CreateLookAt(Position, Position+Direction, OrientationVerticale);
        }


        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            float TempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJ += TempsÉcoulé;
            if (TempsÉcouléDepuisMAJ >= IntervalleMAJ)
            {
                TempsÉcouléDepuisMAJ = 0;
            }
            base.Update(gameTime);
        }

        private int GérerTouche(Keys touche)
        {
            return GestionInput.EstEnfoncée(touche) ? 1 : 0;
        }

        private void GérerDéplacement()
        {
            Position = PositionJoueur + VariationEntrePositionCamJoueur;
        }

        public void DonnerPositionJoueur(Vector3 positionJoueur)
        {
            PositionJoueur = positionJoueur;
            GérerDéplacement();
            CréerPointDeVue();
        }
    }
}
