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
    public class EntitéEnnemie : EntitéMobile, IControlable, ICollisionable
    {
        const float FACTEUR_VITESSE = 0.05f;
        public BoundingSphere SphèreDeCollision { get; private set; }
        Vector3 DirectionDéplacement { get; set; }
        Vector3 Direction { get; set; }
        Vector3 Destination { get; set; }
        Plane PlanReprésentantCarte { get; set; }
        InputManager GestionInputs { get; set; }
        Caméra CaméraJeu { get; set; }
        bool EnMouvement { get; set; }




        public EntitéEnnemie(Game jeu, string nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale,
                           float intervalleMAJ, int pointDeVie, int portée, int force, int armure)
            : base(jeu, nomModèle, échelleInitiale, rotationInitiale, positionInitiale, intervalleMAJ, pointDeVie, portée, force, armure)
        {

        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            GestionInputs = Game.Services.GetService(typeof(InputManager)) as InputManager;
            CaméraJeu = Game.Services.GetService(typeof(Caméra)) as Caméra;
            DoCalculerMonde = false;
            Direction = new Vector3(1, 0, 0);
            PlanReprésentantCarte = new Plane(0, 1, 0, 0);
            EnMouvement = false;

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
        public void GestionDéplacement()
        {
            GérerDéplacement();
        }
        protected override void GérerDéplacement()
        {
            if (EnMouvement == true)
            {
                Vector3 BonneDestination = new Vector3(Destination.X, 0, Destination.Z);
                Vector3 BonnePosition = new Vector3(Position.X, 0, Position.Z);
                DirectionDéplacement = Vector3.Normalize(BonneDestination - BonnePosition);
                GérerRotation();
                Position += FACTEUR_VITESSE * DirectionDéplacement;
                CalculerMonde();
                if(Position.X == Destination.X && Position.Z == Destination.Z)
                {
                    EnMouvement = false;
                }
            }
        }
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
        public void DéplacerEnnemie(Vector3 destination)
        {
            EnMouvement = true;
            Destination = destination;

        }
        void GérerRotation()
        {

            float Angle = (float)Math.Acos(Vector3.Dot(DirectionDéplacement, Direction) / (DirectionDéplacement.Length() * Direction.Length()));
            if (Vector3.Cross(Direction, DirectionDéplacement).Y < 0) { Angle *= -1; }
            Rotation += new Vector3(0, Angle, 0);
            Direction = DirectionDéplacement;
            DoCalculerMonde = true;
        }





        public bool EstEnCollision(object autreObjet)
        {
            return false;
        }
    }

}
