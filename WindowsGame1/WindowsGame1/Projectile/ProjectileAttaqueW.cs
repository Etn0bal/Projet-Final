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

        public EntitÈ Cible { get; private set; }
        public bool ¿DÈtruire { get; set; }
        Vector3 PositionInitiale { get;set;}
        BoundingBox BoiteDeCollision { get; set; }
        int LanceurOuReceveur { get; set; }

        public ProjectileAttaqueW(Microsoft.Xna.Framework.Game game, string nomModËle, float ÈchelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale,
                                       Vector3 direction,Vector3 directionDÈplacement, int force, int prÈcision,float intervalleMAJ,int lanceurOuReceveur)
            : base(game, nomModËle, ÈchelleInitiale, rotationInitiale, positionInitiale, direction, force, prÈcision, intervalleMAJ)
        {
            DirectionDÈplacement = directionDÈplacement;
            PositionInitiale = positionInitiale;
            LanceurOuReceveur = lanceurOuReceveur;
        }
        public ProjectileAttaqueW(Microsoft.Xna.Framework.Game game, string nomModËle, float ÈchelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale,
                               Vector3 direction, Vector3 directionDÈplacement, int force, int prÈcision, int dÈgat, float intervalleMAJ, int lanceurOuReceveur)
        : base(game, nomModËle, ÈchelleInitiale, rotationInitiale, positionInitiale, direction, force, prÈcision,dÈgat, intervalleMAJ)
        {
            DirectionDÈplacement = directionDÈplacement;
            PositionInitiale = positionInitiale;
            LanceurOuReceveur = lanceurOuReceveur;
            
        }

        public override void Initialize()
        {
            ¿DÈtruire = false;
            Monde¿Recalculer = false;
            BoiteDeCollision = new BoundingBox(Position + PointMinBDC, Position + PointMaxBDC);
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            float temps…coulÈ = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Temps…coulÈDepuisMAJ += temps…coulÈ;
            if (Temps…coulÈDepuisMAJ >= IntervalleMAJ)
            {
                GÈrerRotation();
                GestionDÈplacement();
                if (Monde¿Recalculer) { CalculerMonde(); Monde¿Recalculer = false; }
                Temps…coulÈDepuisMAJ = 0;
            }

            base.Update(gameTime);
        }

        void GestionDÈplacement()
        {
            float distanceEntre2Positions = (float)Math.Sqrt(Math.Pow((PositionInitiale.X - Position.X), 2) + Math.Pow((PositionInitiale.Z - Position.Z), 2));
            if (!(¿DÈtruire) && distanceEntre2Positions <= PORTEE_MAX)
            {
                if (DirectionDÈplacement.X >= 0 || DirectionDÈplacement.X <= 0)
                {
                    Position += DirectionDÈplacement * FACTEUR_VITESSE;
                    BoiteDeCollision = new BoundingBox(Position + PointMinBDC, Position + PointMaxBDC);
                    Monde¿Recalculer = true;

                    List<EntitÈ> entitÈs = Game.Components.OfType<EntitÈ>().ToList();
                    foreach (EntitÈ entitÈ in entitÈs)
                    {
                        if(BoiteDeCollision.Intersects(entitÈ.BoiteDeCollision))
                        {
                            if(LanceurOuReceveur ==1 && !entitÈ.EstAlliÈe)
                            {
                                    Cible = entitÈ;
                                    ¿DÈtruire = true;
                            }
                            else if(LanceurOuReceveur == 2 && entitÈ.EstAlliÈe)
                            {
                                Cible = entitÈ;
                                ¿DÈtruire = true;
                            }

                        }
                    }
                    if(Cible!= null)
                    {
                        Cible.RecevoirAttaque(DÈgat);
                    }
                }

            }
            else { ¿DÈtruire = true; }
        }

        void GÈrerRotation()
        {
            //Le if est l‡ pour vÈrifier que les valeur de DirectionDÈplacement sont des valeur numÈrique, car si Destination-Position Ègale
            //le vecteur 0 alors le normalize donne un vecteur avec des valeurs non numÈriques
            if (DirectionDÈplacement.X >= 0 || DirectionDÈplacement.X <= 0)
            {
                float Angle = (float)Math.Acos(Math.Min(Math.Max(Vector3.Dot(DirectionDÈplacement, Direction) / (DirectionDÈplacement.Length() * Direction.Length()), -1), 1));
                if (Vector3.Cross(Direction, DirectionDÈplacement).Y < 0) { Angle *= -1; }

                Rotation += new Vector3(0, Angle, 0);
                Direction = DirectionDÈplacement;
                Monde¿Recalculer = true;
            }
        }
    }
}
