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

        Entit� Cible { get; set; }
        public bool �D�truire { get; set; }

        public ProjectileAttaqueDeBase(Game game, string nomMod�le, float �chelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale,
                                       Vector3 direction, int force, int pr�cision, Entit� cible, float intervalleMAJ)
            : base(game, nomMod�le, �chelleInitiale, rotationInitiale, positionInitiale, direction, force, pr�cision, intervalleMAJ)
        {
            Cible = cible;
        }

        public ProjectileAttaqueDeBase(Game game, string nomMod�le, float �chelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale,
                                      Vector3 direction, int force, int pr�cision, Entit� cible, int d�gat, float intervalleMAJ)
           : base(game, nomMod�le, �chelleInitiale, rotationInitiale, positionInitiale,direction, force, pr�cision, d�gat, intervalleMAJ)
        {
            Cible = cible;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            �D�truire = false;

            base.Initialize();
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
                CibleAtteinte();
                GestionD�placement();
                G�rerRotation();
                if (Monde�Recalculer) { CalculerMonde(); Monde�Recalculer = false; }
                Temps�coul�DepuisMAJ = 0;
            }

            base.Update(gameTime);
        }

        void GestionD�placement()
        {
            if (!(�D�truire))
            {
                DirectionD�placement = Vector3.Normalize(Cible.Position - Position);

                if (DirectionD�placement.X >= 0 || DirectionD�placement.X <= 0) 
                {
                    Position += DirectionD�placement * FACTEUR_VITESSE;
                    Monde�Recalculer = true;
                }
                else { �D�truire = true;  }
               
            }
        }

        void G�rerRotation()
        {
            //Le if est l� pour v�rifier que les valeur de DirectionD�placement sont des valeur num�rique, car si Destination-Position �gale
            //le vecteur 0 alors le normalize donne un vecteur avec des valeurs non num�riques
            if (DirectionD�placement.X >= 0 || DirectionD�placement.X <= 0)
            {
                float Angle = (float)Math.Acos(Math.Min(Math.Max(Vector3.Dot(DirectionD�placement, Direction) / (DirectionD�placement.Length() * Direction.Length()), -1), 1));
                if (Vector3.Cross(Direction, DirectionD�placement).Y < 0) { Angle *= -1; }

                Rotation += new Vector3(0, Angle, 0);
                Direction = DirectionD�placement;
                Monde�Recalculer = true;
            }
        }

        void CibleAtteinte()
        {
            if((Cible.Position - Position).Length() <= Cible.RayonCollision)
            {
                �D�truire = true;
                Cible.RecevoirAttaque(D�gat);
            }
        }
    }
}
