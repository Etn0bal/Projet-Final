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

        public Entité Cible { get; private set; }
        public bool ÀDétruire { get; set; }
        Vector3 PositionInitiale { get;set;}
        BoundingBox BoiteDeCollision { get; set; }
        int LanceurOuReceveur { get; set; }

        public ProjectileAttaqueW(Game game, string nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale,
                                       Vector3 direction,Vector3 directionDéplacement, int force, int précision,float intervalleMAJ,int lanceurOuReceveur)
            : base(game, nomModèle, échelleInitiale, rotationInitiale, positionInitiale, direction, force, précision, intervalleMAJ)
        {
            DirectionDéplacement = directionDéplacement;
            PositionInitiale = positionInitiale;
            LanceurOuReceveur = lanceurOuReceveur;
        }
        public ProjectileAttaqueW(Game game, string nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale,
                               Vector3 direction, Vector3 directionDéplacement, int force, int précision, int dégat, float intervalleMAJ, int lanceurOuReceveur)
        : base(game, nomModèle, échelleInitiale, rotationInitiale, positionInitiale, direction, force, précision,dégat, intervalleMAJ)
        {
            DirectionDéplacement = directionDéplacement;
            PositionInitiale = positionInitiale;
            LanceurOuReceveur = lanceurOuReceveur;
            
        }

        public override void Initialize()
        {
            ÀDétruire = false;
            DoCalculerMonde = false;
            BoiteDeCollision = new BoundingBox(Position + PointMinBDC, Position + PointMaxBDC);
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            float tempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJ += tempsÉcoulé;
            if (TempsÉcouléDepuisMAJ >= IntervalleMAJ)
            {
                GérerRotation();
                GestionDéplacement();
                if (DoCalculerMonde) { CalculerMonde(); DoCalculerMonde = false; }
                TempsÉcouléDepuisMAJ = 0;
            }

            base.Update(gameTime);
        }

        void GestionDéplacement()
        {
            float distanceEntre2Positions = (float)Math.Sqrt(Math.Pow((PositionInitiale.X - Position.X), 2) + Math.Pow((PositionInitiale.Z - Position.Z), 2));
            if (!(ÀDétruire) && distanceEntre2Positions <= PORTEE_MAX)
            {
                if (DirectionDéplacement.X >= 0 || DirectionDéplacement.X <= 0)
                {
                    Position += DirectionDéplacement * FACTEUR_VITESSE;
                    BoiteDeCollision = new BoundingBox(Position + PointMinBDC, Position + PointMaxBDC);
                    DoCalculerMonde = true;

                    List<Entité> entités = Game.Components.OfType<Entité>().ToList();
                    foreach (Entité entité in entités)
                    {
                        if(BoiteDeCollision.Intersects(entité.BoiteDeCollision))
                        {
                            if(LanceurOuReceveur ==1 && !entité.EstAlliée)
                            {
                                    Cible = entité;
                                    ÀDétruire = true;
                            }
                            else if(LanceurOuReceveur == 2 && entité.EstAlliée)
                            {
                                Cible = entité;
                                ÀDétruire = true;
                            }

                        }
                    }
                    if(Cible!= null)
                    {
                        Cible.RecevoirAttaque(Dégat);
                    }
                }

            }
            else { ÀDétruire = true; }
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
                DoCalculerMonde = true;
            }
        }
    }
}
