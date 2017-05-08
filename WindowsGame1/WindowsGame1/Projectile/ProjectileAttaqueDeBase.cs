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
    public class ProjectileAttaqueDeBase : Projectile, IDestructible
    {
        const float FACTEUR_VITESSE = 1f;

        Entité Cible { get; set; }
        public bool ÀDétruire { get; set; }

        public ProjectileAttaqueDeBase(Game game, string nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale,
                                       Vector3 direction, int force, int précision, Entité cible, float intervalleMAJ)
            : base(game, nomModèle, échelleInitiale, rotationInitiale, positionInitiale, direction, force, précision, intervalleMAJ)
        {
            Cible = cible;
        }

        public ProjectileAttaqueDeBase(Game game, string nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale,
                                      Vector3 direction, int force, int précision, Entité cible, int dégat, float intervalleMAJ)
           : base(game, nomModèle, échelleInitiale, rotationInitiale, positionInitiale,direction, force, précision, dégat, intervalleMAJ)
        {
            Cible = cible;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            ÀDétruire = false;

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            MondeÀRecalculer = false;

            float tempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJ += tempsÉcoulé;
            if (TempsÉcouléDepuisMAJ >= IntervalleMAJ)
            {
                CibleAtteinte();
                GestionDéplacement();
                GérerRotation();
                if (MondeÀRecalculer) { CalculerMonde(); MondeÀRecalculer = false; }
                TempsÉcouléDepuisMAJ = 0;
            }

            base.Update(gameTime);
        }

        void GestionDéplacement()
        {
            if (!(ÀDétruire))
            {
                DirectionDéplacement = Vector3.Normalize(Cible.Position - Position);

                if (DirectionDéplacement.X >= 0 || DirectionDéplacement.X <= 0) 
                {
                    Position += DirectionDéplacement * FACTEUR_VITESSE;
                    MondeÀRecalculer = true;
                }
                else { ÀDétruire = true;  }
               
            }
        }

        void GérerRotation()
        {
            //Le if est là pour vérifier que les valeur de DirectionDéplacement sont des valeur numérique, car si Destination-Position égale
            //le vecteur 0 alors le normalize donne un vecteur avec des valeurs non numériques
            if (DirectionDéplacement.X >= 0 || DirectionDéplacement.X <= 0)
            {
                float Angle = (float)Math.Acos(Math.Min(Math.Max(Vector3.Dot(DirectionDéplacement, Direction) / (DirectionDéplacement.Length() * Direction.Length()), -1), 1));
                if (Vector3.Cross(Direction, DirectionDéplacement).Y < 0) { Angle *= -1; }

                Rotation += new Vector3(0, Angle, 0);
                Direction = DirectionDéplacement;
                MondeÀRecalculer = true;
            }
        }

        void CibleAtteinte()
        {
            if((Cible.Position - Position).Length() <= Cible.RayonCollision)
            {
                ÀDétruire = true;
                Cible.RecevoirAttaque(Dégat);
            }
        }
    }
}
