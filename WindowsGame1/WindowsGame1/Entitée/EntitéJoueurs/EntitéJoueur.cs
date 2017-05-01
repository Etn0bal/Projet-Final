﻿using System;
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
    public class EntitéJoueur : Entité, IControlable,IDestructible
    {
        const float FACTEUR_VITESSE = 0.35f;
        
        Vector3 PointMaxBDC = new Vector3(2, 16.2f, 5f / 2f);
        Vector3 PointMinBDC = new Vector3(-2, 0, -(5f / 2f));
       




        Vector3 DirectionDéplacement { get; set; }
        Vector3 Direction { get; set; }
        Vector3 Destination { get; set; }
        Vector3 DestinationTampon { get; set; }
        Plane PlanReprésentantCarte { get; set; }
        InputManager GestionInputs { get; set; }
        CaméraTypéMoba CaméraJeu { get; set; }
        Murs Murs { get; set; }
        Ray RayonPicking { get; set; }
        TheGame LeGame { get; set; }
        float TempsÉcouléDepuisDernierQ { get; set; }
        float CoolDownQ { get; set; }
        public bool EnMouvement { get; set; }
        public bool ÀDétruire { get; set;}





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
            ÀDétruire = false;
            EnMouvement = true;
            EstAlliée = true;
            EnMouvement = true;
            RayonCollision = 3;
            CoolDownQ = 10;
            BoiteDeCollision = new BoundingBox(Position + PointMinBDC, Position + PointMaxBDC);
            Murs = Game.Services.GetService(typeof(Murs)) as Murs;
            LeGame = Game.Components.First(x => x is TheGame) as TheGame;
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            //Rajouter mécanique gestion du temps
            
            float tempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJ += tempsÉcoulé;
            TempsÉcouléDepuisDernierQ += tempsÉcoulé;
            if (TempsÉcouléDepuisMAJ >= IntervalleMAJ)
            {
                GestionDéplacement();
                GestionVie();
                TempsÉcouléDepuisMAJ -= IntervalleMAJ;
            }

            if (GestionInputs.EstSourisActive && GestionInputs.EstNouveauClicDroit())
            {
                GetDestination();
                DirectionDéplacement = Vector3.Normalize(Destination - Position);
                GérerRotation();

                try
                {
                    Cible = Game.Components.OfType<Entité>().First(x => x.BoiteDeCollision.Intersects(RayonPicking) != null && !x.EstAlliée);
                }
                catch { }

                if (Cible != null) { GestionAttaque(); }
            }

            if (TempsÉcouléDepuisDernierQ >= CoolDownQ && GestionInputs.EstNouvelleTouche(Keys.Q))
            {
                GestionQ();
                TempsÉcouléDepuisDernierQ = 0;
            }

            if (DoCalculerMonde)
            {
                CalculerMonde();
                LeGame.EnvoyerMatrice(Monde);
                DoCalculerMonde = false;
            }
            base.Update(gameTime);

        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        private void GestionVie()
        {
            if (PointDeVie == 0)
            {
                ÀDétruire = true;
            }
        }

        void GestionAttaque()
        {
            ProjectileAttaqueDeBase attaque = new ProjectileAttaqueDeBase(Game, "rocket", ÉCHELLE_PROJECTILE_ATTAQUE_DE_BASE,
                                                                              RotationInitialeProjectielADB, Position, DirectionInitialeProjectileADB,
                                                                              Force, Précision, Cible, IntervalleMAJ);
            Game.Components.Add(attaque);

            int typeEnnemie = 3;
            int numEnnemie = 0;
            if (Cible is EntitéPéonEnnemie)
            {
                numEnnemie = (Cible as EntitéPéonEnnemie).NumPéon;
                typeEnnemie = 1;
            }
            if (Cible is EntitéTourEnnemie)
            {
                numEnnemie = (Cible as EntitéTourEnnemie).NumTour;
                typeEnnemie = 2;
            }
            LeGame.EnvoyerAttaqueAuServeur(Position, Force, Précision, typeEnnemie, numEnnemie, attaque.Dégat);

            Cible = null;
            Destination = Position;
        }

        void GestionQ()
        {
            DestinationTampon = Destination;
            GetDestination();
            float distanceEntreLesDeux = (float)Math.Sqrt(Math.Pow((Destination.X - Position.X), 2) + Math.Pow((Destination.Z - Position.Z), 2));

            if (distanceEntreLesDeux <= Portée)
            {
                DirectionDéplacement = Vector3.Normalize(Destination - Position);
                GérerRotation();
                Position = Destination;
                CaméraJeu.DonnerPositionJoueur(Position);
                BoiteDeCollision = new BoundingBox(Position + PointMinBDC, Position + PointMaxBDC);
                DoCalculerMonde = true;
            }
            else { Destination = DestinationTampon; }
        }

        public void GestionDéplacement()
        {
            if ((Destination - Position).Length() > FACTEUR_VITESSE * DirectionDéplacement.Length())
            {
                NouvellePosition = Position + FACTEUR_VITESSE * DirectionDéplacement;
                NouvelleBoiteDeCollision = new BoundingBox(NouvellePosition + PointMinBDC, NouvellePosition + PointMaxBDC);

                if (!Murs.EnCollision(this) && !EnCollisionAvecTour())
                {
                    EnMouvement = true;
                    Position = NouvellePosition;
                    BoiteDeCollision = NouvelleBoiteDeCollision;
                    DoCalculerMonde = true;
                }
            }

            CaméraJeu.DonnerPositionJoueur(Position);
        }

        bool EnCollisionAvecTour()
        {
            foreach(EntitéTour tour in Game.Components.Where(x=> x is EntitéTour))
            {
                if(tour.EstEnCollision(this)) { return true; }
            }

            return false;
        }


        private void GetDestination()
        {
            Point positionSouris = GestionInputs.GetPositionSouris();
            Vector2 vecteurPosition = new Vector2(positionSouris.X, positionSouris.Y);
            Vector3 nearSource = new Vector3(vecteurPosition, 0);
            Vector3 farSource = new Vector3(vecteurPosition, 1);

            Vector3 nearPoint = Game.GraphicsDevice.Viewport.Unproject(nearSource, CaméraJeu.Projection, CaméraJeu.Vue, Matrix.Identity);    
            Vector3 farPoint = Game.GraphicsDevice.Viewport.Unproject(farSource, CaméraJeu.Projection, CaméraJeu.Vue, Matrix.Identity);    
            Vector3 direction = farPoint - nearPoint;
            direction = Vector3.Normalize(direction);

            RayonPicking = new Ray(nearPoint, direction);

            Destination = (Vector3)(nearPoint + direction * RayonPicking.Intersects(PlanReprésentantCarte));
        }

        void GérerRotation()
        {
            //Le if est là pour vérifier que les valeur de DirectionDéplacement sont des valeur numérique, car si Destination-Position égale
            //le vecteur 0 alors le normalize donne un vecteur avec des valeurs non numériques
            if (DirectionDéplacement.X >= 0 || DirectionDéplacement.X <= 0)
            {
                float Angle = (float)Math.Acos(Math.Min(Math.Max(Vector3.Dot(DirectionDéplacement, Direction) / (DirectionDéplacement.Length() * Direction.Length()),-1),1));
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
