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
    public class EntitéJoueur : Entité, IMobile
    {
        const float FACTEUR_VITESSE = 0.35f;

        Vector3 PointMaxBDC = new Vector3(2, 16.2f, 5f / 2f);
        Vector3 PointMinBDC = new Vector3(-2, 0, -(5f / 2f));
        Vector3 HauteurAttaque = new Vector3(0, 5, 0);





        Vector3 DirectionDéplacement { get; set; }
        Vector3 Direction { get; set; }
        Vector3 Destination { get; set; }
        Vector3 DestinationTampon { get; set; }
        Plane PlanReprésentantCarte { get; set; }
        InputManager GestionInputs { get; set; }
        CaméraTypéMoba CaméraJeu { get; set; }
        Murs Murs { get; set; }
        Ray Rayon { get; set; }

        float TempsÉcouléDepuisDernièreAttaque { get; set; }
        float TempsDeRechargeAttaque { get; set; }
        float TempsÉcouléDepuisDernierQ { get; set; }
        float TempsDeRechargeQ { get; set; }
        float TempsÉcouléDepuisDernierW { get; set; }
        float TempDeRechargeW { get; set; }
        float TempsÉcouléDepuisDernierE { get; set; }
        float TempsDeRechargeE { get; set; }
        int PointDeVieRedonné { get; set; }







        public EntitéJoueur(Microsoft.Xna.Framework.Game jeu, string nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale,
                           float intervalleMAJ, int pointDeVie, int portée, int force, int armure, int précision, Vector3 direction)
            : base(jeu, nomModèle, échelleInitiale, rotationInitiale, positionInitiale, intervalleMAJ, pointDeVie, portée, force, armure, précision)
        {
            Direction = direction;
        }

        public override void Initialize()
        {
            GestionInputs = Game.Services.GetService(typeof(InputManager)) as InputManager;
            CaméraJeu = Game.Services.GetService(typeof(Caméra)) as CaméraTypéMoba;
            Murs = Game.Services.GetService(typeof(Murs)) as Murs;
            LeJeu = Game.Services.GetService(typeof(GestionnaireJeu)) as GestionnaireJeu;
            MondeÀRecalculer = false;
            Destination = Position;
            PlanReprésentantCarte = new Plane(0, 1, 0, 0);
            ÀDétruire = false;
            EstAlliée = true;
            RayonCollision = 3;
            TempsDeRechargeQ = 10;
            TempDeRechargeW = 7;
            TempsDeRechargeE = 5;
            TempsDeRechargeAttaque = 0.2f;

            HauteurPositionBarrePV = new Vector3(0, 15, 0);

            BoiteDeCollision = new BoundingBox(Position + PointMinBDC, Position + PointMaxBDC);
            base.Initialize();
            PointDeVieRedonné = (int)(0.3f * PointDeVieInitial);
        }

        public override void Update(GameTime gameTime)
        {
            //Rajouter mécanique gestion du temps
            
            float tempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJ += tempsÉcoulé;
            TempsÉcouléDepuisDernièreAttaque += tempsÉcoulé;
            TempsÉcouléDepuisDernierQ += tempsÉcoulé;
            TempsÉcouléDepuisDernierW += tempsÉcoulé;
            TempsÉcouléDepuisDernierE += tempsÉcoulé;
            if (TempsÉcouléDepuisMAJ >= IntervalleMAJ)
            {
                GestionDéplacement();
                TempsÉcouléDepuisMAJ -= IntervalleMAJ;
            }

            if (GestionInputs.EstSourisActive && GestionInputs.EstNouveauClicDroit())
            {
                ObtenirDestination();
                DirectionDéplacement = Vector3.Normalize(Destination - Position);
                GérerRotation();

                Cible = Game.Components.OfType<Entité>().FirstOrDefault(x => x.BoiteDeCollision.Intersects(Rayon) != null && !x.EstAlliée &&
                                                                        Math.Sqrt(Math.Pow(x.Position.X - Position.X,2) - Math.Pow(x.Position.Z 
                                                                        -Position.Z,2)) <= Portée);

                if (Cible != null)
                {
                    if(TempsÉcouléDepuisDernièreAttaque >= TempsDeRechargeAttaque)
                    {
                        GestionAttaqueDeBase();
                        TempsÉcouléDepuisDernièreAttaque = 0;
                    }
                }
            }

            if (TempsÉcouléDepuisDernierQ >= TempsDeRechargeQ && GestionInputs.EstNouvelleTouche(Keys.Q))
            {
                GestionQ();

            }

            if (TempsÉcouléDepuisDernierW >= TempDeRechargeW && GestionInputs.EstNouvelleTouche(Keys.W))
            {
                GestionW();
                TempsÉcouléDepuisDernierW = 0;
            }

            if (TempsÉcouléDepuisDernierE >= TempsDeRechargeE && GestionInputs.EstNouvelleTouche(Keys.E))
            {
                GestionE();
                TempsÉcouléDepuisDernierE = 0;
            }

            if (MondeÀRecalculer)
            {
                CalculerMonde();
                LeJeu.EnvoyerMatrice(Monde);
                MondeÀRecalculer = false;
            }
            base.Update(gameTime);

        }

        private void GestionW()
        {
            ObtenirDestination();
            Vector3 directionAttaqueW = Vector3.Normalize(Destination - Position);
            directionAttaqueW.Y = 0;
            GérerRotation();
            ProjectileAttaqueW attaque = new ProjectileAttaqueW(Game, "bomb", ÉCHELLE_PROJECTILE_W,
                                                                RotationInitialeProjectielADB, Position + HauteurAttaque,DirectionInitialeProjectileADB, directionAttaqueW,
                                                                Force+1500, Précision, IntervalleMAJ,1);
            LeJeu.EnvoyerAttaqueW(Position + new Vector3(0, 5, 0), directionAttaqueW, Force, Précision, attaque.Dégat);

            Game.Components.Add(attaque);
            Destination = Position;

        }

        private void GestionE()
        {
            PointDeVie = Math.Min(PointDeVie+PointDeVieRedonné, PointDeVieInitial);
            LeJeu.EnvoyerGainDeVie(PointDeVie);
        }


        void GestionAttaqueDeBase()
        {
            ProjectileAttaqueDeBase attaque = new ProjectileAttaqueDeBase(Game, "rocket", ÉCHELLE_PROJECTILE_ATTAQUE_DE_BASE,
                                                                              RotationInitialeProjectielADB, Position + HauteurAttaque, DirectionInitialeProjectileADB,
                                                                              Force, Précision, Cible, IntervalleMAJ);
            Game.Components.Add(attaque);

            int typeEnnemie = 0;
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
            LeJeu.EnvoyerAttaqueAuServeur(Position, Force, Précision, typeEnnemie, numEnnemie, attaque.Dégat);

            Cible = null;
            Destination = Position;
        }

        void GestionQ()
        {
            DestinationTampon = Destination;
            ObtenirDestination();
            float distanceEntreLesDeux = (float)Math.Sqrt(Math.Pow((Destination.X - Position.X), 2) + Math.Pow((Destination.Z - Position.Z), 2));

            if (distanceEntreLesDeux <= Portée)
            {
                DirectionDéplacement = Vector3.Normalize(Destination - Position);
                GérerRotation();
                Position = Destination;
                CaméraJeu.DonnerPositionJoueur(Position);
                BoiteDeCollision = new BoundingBox(Position + PointMinBDC, Position + PointMaxBDC);
                MondeÀRecalculer = true;
                TempsÉcouléDepuisDernierQ = 0;
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
                    Position = NouvellePosition;
                    BoiteDeCollision = NouvelleBoiteDeCollision;
                    MondeÀRecalculer = true;
                }
            }
            //Pour pouvoir garder la caméra centrée sur le joueur
            CaméraJeu.DonnerPositionJoueur(Position);
        }

        bool EnCollisionAvecTour()
        {
            List<EntitéTour> tours = Game.Components.OfType<EntitéTour>().ToList();
            foreach (EntitéTour tour in tours)
            {
                if(tour.EstEnCollision(this)) { return true; }
            }

            return false;
        }


        private void ObtenirDestination()
        {
            Point positionSouris = GestionInputs.GetPositionSouris();
            Vector2 vecteurPosition = new Vector2(positionSouris.X, positionSouris.Y);
            Vector3 nearSource = new Vector3(vecteurPosition, 0);
            Vector3 farSource = new Vector3(vecteurPosition, 1);

            Vector3 nearPoint = Game.GraphicsDevice.Viewport.Unproject(nearSource, CaméraJeu.Projection, CaméraJeu.Vue, Matrix.Identity);    
            Vector3 farPoint = Game.GraphicsDevice.Viewport.Unproject(farSource, CaméraJeu.Projection, CaméraJeu.Vue, Matrix.Identity);    
            Vector3 direction = farPoint - nearPoint;
            direction = Vector3.Normalize(direction);

            Rayon = new Ray(nearPoint, direction);

            Destination = (Vector3)(nearPoint + direction * Rayon.Intersects(PlanReprésentantCarte));
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
                MondeÀRecalculer = true;
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
