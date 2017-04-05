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
        InputManager GestionInputs { get; set; }
        Caméra CaméraJeu { get; set; }
        bool EnMouvement { get; set; }




        public EntitéEnnemie(Game jeu, string nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale,
                           float intervalleMAJ, int pointDeVie, int portée, int force, int armure, int précision, Vector3 direction)
            : base(jeu, nomModèle, échelleInitiale, rotationInitiale, positionInitiale, intervalleMAJ, pointDeVie, portée, force, armure, précision)
        {
            Direction = direction;
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
            EnMouvement = false;

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            if(EnMouvement)
            {
                GestionDéplacement();
            }

            if (DoCalculerMonde)
            {
                CalculerMonde();
                DoCalculerMonde = false;
            }
            base.Update(gameTime);

        }
        public void GestionDéplacement()
        {
            if ((Destination - Position).Length() > FACTEUR_VITESSE * DirectionDéplacement.Length())
            {
                Position += FACTEUR_VITESSE * DirectionDéplacement;
                DoCalculerMonde = true;
                EnMouvement = false;  
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
            DirectionDéplacement = Vector3.Normalize(Destination - Position);
            GérerRotation();


        }
        void GérerRotation()
        {
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
    }

}
