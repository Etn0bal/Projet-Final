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
    public class ProjectileAttaqueW : Projectile, IDestructible
    {
        const float FACTEUR_VITESSE = 0.5f;
        const float PORTEE_MAX =100f ;

        protected Vector3 PointMaxBDC = new Vector3(1,1,1);
        protected Vector3 PointMinBDC = new Vector3(-1,-1,-1);

        public Entit� Cible { get; private set; }
        public bool �D�truire { get; set; }
        Vector3 PositionInitiale { get;set;}
        BoundingBox BoiteDeCollision { get; set; }
        int LanceurOuReceveur { get; set; }

        public ProjectileAttaqueW(Microsoft.Xna.Framework.Game game, string nomMod�le, float �chelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale,
                                       Vector3 direction,Vector3 directionD�placement, int force, int pr�cision,float intervalleMAJ,int lanceurOuReceveur)
            : base(game, nomMod�le, �chelleInitiale, rotationInitiale, positionInitiale, direction, force, pr�cision, intervalleMAJ)
        {
            DirectionD�placement = directionD�placement;
            PositionInitiale = positionInitiale;
            LanceurOuReceveur = lanceurOuReceveur;
        }
        public ProjectileAttaqueW(Microsoft.Xna.Framework.Game game, string nomMod�le, float �chelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale,
                               Vector3 direction, Vector3 directionD�placement, int force, int pr�cision, int d�gat, float intervalleMAJ, int lanceurOuReceveur)
        : base(game, nomMod�le, �chelleInitiale, rotationInitiale, positionInitiale, direction, force, pr�cision,d�gat, intervalleMAJ)
        {
            DirectionD�placement = directionD�placement;
            PositionInitiale = positionInitiale;
            LanceurOuReceveur = lanceurOuReceveur;
            
        }

        public override void Initialize()
        {
            �D�truire = false;
            Monde�Recalculer = false;
            BoiteDeCollision = new BoundingBox(Position + PointMinBDC, Position + PointMaxBDC);
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            float temps�coul� = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Temps�coul�DepuisMAJ += temps�coul�;
            if (Temps�coul�DepuisMAJ >= IntervalleMAJ)
            {
                G�rerRotation();
                GestionD�placement();
                if (Monde�Recalculer) { CalculerMonde(); Monde�Recalculer = false; }
                Temps�coul�DepuisMAJ = 0;
            }

            base.Update(gameTime);
        }

        void GestionD�placement()
        {
            float distanceEntre2Positions = (float)Math.Sqrt(Math.Pow((PositionInitiale.X - Position.X), 2) + Math.Pow((PositionInitiale.Z - Position.Z), 2));
            if (!(�D�truire) && distanceEntre2Positions <= PORTEE_MAX)
            {
                if (DirectionD�placement.X >= 0 || DirectionD�placement.X <= 0)
                {
                    Position += DirectionD�placement * FACTEUR_VITESSE;
                    BoiteDeCollision = new BoundingBox(Position + PointMinBDC, Position + PointMaxBDC);
                    Monde�Recalculer = true;

                    List<Entit�> entit�s = Game.Components.OfType<Entit�>().ToList();
                    foreach (Entit� entit� in entit�s)
                    {
                        if(BoiteDeCollision.Intersects(entit�.BoiteDeCollision))
                        {
                            if(LanceurOuReceveur ==1 && !entit�.EstAlli�e)
                            {
                                    Cible = entit�;
                                    �D�truire = true;
                            }
                            else if(LanceurOuReceveur == 2 && entit�.EstAlli�e)
                            {
                                Cible = entit�;
                                �D�truire = true;
                            }

                        }
                    }
                    if(Cible!= null)
                    {
                        Cible.RecevoirAttaque(D�gat);
                    }
                }

            }
            else { �D�truire = true; }
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
    }
}
