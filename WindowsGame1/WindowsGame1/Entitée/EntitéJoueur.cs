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
    public class EntitéJoueur : EntitéMobile, IControlable, ICollisionable
    {
        const float FACTEUR_VITESSE = 0.05f;
        const float ÉCHELLE_PROJECTILE_ATTAQUE_DE_BASE = 0.009f;

        public BoundingSphere SphèreDeCollision { get; private set; }
        Entité Cible { get; set; }
        Vector3 DirectionDéplacement { get; set; }
        Vector3 Direction { get; set; }
        Vector3 Destination { get; set; }
        Plane PlanReprésentantCarte { get; set; }
        InputManager GestionInputs { get; set; }
        CaméraTypéMoba CaméraJeu { get; set; }
        Murs Murs { get; set; }
        public bool EnMouvement { get; set; }




        public EntitéJoueur(Game jeu, string nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale,
                           float intervalleMAJ, int pointDeVie, int portée, int force, int armure, int précision, Vector3 direction)
            : base(jeu, nomModèle, échelleInitiale, rotationInitiale, positionInitiale, intervalleMAJ, pointDeVie, portée, force, armure, précision)
        {
            Direction = direction;
        }

        public override void Initialize()
        {
            GestionInputs = Game.Services.GetService(typeof(InputManager)) as InputManager;
            CaméraJeu = Game.Services.GetService(typeof(Caméra)) as CaméraTypéMoba;
            DoCalculerMonde = false;
            Destination = Position;
            PlanReprésentantCarte = new Plane(0, 1, 0, 0);
            EnMouvement = false;
            RayonCollision = 3;
            Murs = Game.Services.GetService(typeof(Murs)) as Murs; 

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {

            GestionDéplacement();

            if (DoCalculerMonde)
            {
                CalculerMonde();
                DoCalculerMonde = false;
            }
            base.Update(gameTime);

        }
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }


        public void GestionDéplacement()
        {
            if (GestionInputs.EstSourisActive)
            {
                if (GestionInputs.EstNouveauClicDroit()) //// Regarder S'il n'y a pas d'autre entitée
                {
                    GetDestination();
                    try
                    {
                        Cible = Game.Components.OfType<Entité>().First(x => (x.Position - Destination).Length() <= x.RayonCollision);
                    }
                    catch { }

                    if (Cible == null)
                    {
                    DirectionDéplacement = Vector3.Normalize(Destination - Position);
                    GérerRotation();
                    EnMouvement = true;

                    }
                    else
                    {
                        Game.Components.Add(new ProjectileAttaqueDeBase(Game, "Robot2", ÉCHELLE_PROJECTILE_ATTAQUE_DE_BASE, Vector3.Zero, Position, Force, Précision, Cible, IntervalleMAJ));
                    }
                }
            }           

            if ((Destination - Position).Length() > FACTEUR_VITESSE * DirectionDéplacement.Length())
            {
                NouvellePosition = Position + FACTEUR_VITESSE * DirectionDéplacement;

                if (Murs.EnCollision(this))
                {
                    Destination = Position;
                }
                else
                {
                    Position = NouvellePosition;
                    DoCalculerMonde = true;
                }
            }
            if (GestionInputs.EstNouvelleTouche(Keys.Q))
            {

                GetDestination();
                float distanceEntreLesDeux = (float)Math.Sqrt(Math.Pow((Destination.X - Position.X), 2) + Math.Pow((Destination.Z - Position.Z), 2));
                DirectionDéplacement = Vector3.Normalize(Destination - Position);
                if (distanceEntreLesDeux <= Portée)
                {
                    GérerRotation();
                Position = Destination;
                CalculerMonde();
            }

            }

            CaméraJeu.DonnerPositionJoueur(Position);
        }


        private void GetDestination()
        {
            Point positionSouris = GestionInputs.GetPositionSouris();
            Vector2 vecteurPosition = new Vector2(positionSouris.X, positionSouris.Y);
            Vector3 nearSource = new Vector3(vecteurPosition, 0);
            Vector3 farSource = new Vector3(vecteurPosition, 1);

            Vector3 nearPoint = Game.GraphicsDevice.Viewport.Unproject(nearSource, CaméraJeu.Projection, CaméraJeu.Vue, Matrix.Identity);  ////World de la caméra????     
            Vector3 farPoint = Game.GraphicsDevice.Viewport.Unproject(farSource, CaméraJeu.Projection, CaméraJeu.Vue, Matrix.Identity);  ////World de la caméra????     
            Vector3 direction = farPoint - nearPoint;
            direction = Vector3.Normalize(direction);

            Ray Rayon = new Ray(nearPoint, direction);

            Destination = (Vector3)(nearPoint + direction * Rayon.Intersects(PlanReprésentantCarte));
        }

        void GérerRotation()
        {
            //Le if est là pour vérifier que les valeur de DirectionDéplacement sont des valeur numérique, car si Destination-Position égale
            //le vecteur 0 alors le normalize donne un vecteur avec des valeurs non numériques
            if (DirectionDéplacement.X >= 0 || DirectionDéplacement.X <= 0)
            {
                float Angle = (float)Math.Acos(Vector3.Dot(DirectionDéplacement, Direction) / (DirectionDéplacement.Length() * Direction.Length()));
                if (Vector3.Cross(Direction, DirectionDéplacement).Y < 0) { Angle *= -1; }
                Rotation += new Vector3(0, Angle, 0);
                Direction = DirectionDéplacement;
                DoCalculerMonde = true;
            }
        }

        public bool EstEnCollision(object autreObjet)
        {
            return false;
        }
        public Vector3 AvoirDestination()
        { return Destination; }
    }
}
