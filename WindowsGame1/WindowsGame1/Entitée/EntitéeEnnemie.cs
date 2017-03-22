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
    public class EntitéeEnnemie : EntitéeMobile, IControlable, ICollisionable
    {
        public BoundingSphere SphèreDeCollision { get; private set; }
        Vector3 DirectionDéplacement { get; set; }
        Vector3 Direction { get; set; }
        Vector3 Destination { get; set; }
        Plane PlanReprésentantCarte { get; set; }
        InputManager GestionInputs { get; set; }
        Caméra CaméraJeu { get; set; }
        public bool EnMouvement { get; set; }



        public EntitéeEnnemie(Game jeu, string nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale,
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
            CalculerMonde();
        }
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }




       

        public bool EstEnCollision(object autreObjet)
        {
            return false;
        }
    }

}
