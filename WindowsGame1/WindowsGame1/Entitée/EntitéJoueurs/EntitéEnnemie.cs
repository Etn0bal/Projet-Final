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
    public class EntitéEnnemie : Entité, ICollisionable, IMobile
    {
        Vector3 PointMaxBDC = new Vector3(2, 16.2f, 5f / 2f);
        Vector3 PointMinBDC = new Vector3(-2, 0, -(5f / 2f));
        const float FACTEUR_VITESSE = 0.05f;


        public BoundingSphere SphèreDeCollision { get; private set; }
        Vector3 DirectionDéplacement { get; set; }
        Vector3 Direction { get; set; }
        Vector3 Destination { get; set; }
        Vector3 PositionInitiale { get; set; }
        InputManager GestionInputs { get; set; }
        Caméra CaméraJeu { get; set; }





        public EntitéEnnemie(Microsoft.Xna.Framework.Game jeu, string nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale,
                           float intervalleMAJ, int pointDeVie, int portée, int force, int armure, int précision, Vector3 direction)
            : base(jeu, nomModèle, échelleInitiale, rotationInitiale, positionInitiale, intervalleMAJ, pointDeVie, portée, force, armure, précision)
        {
            Direction = direction;
            PositionInitiale = positionInitiale;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            GestionInputs = Game.Services.GetService(typeof(InputManager)) as InputManager;
            CaméraJeu = Game.Services.GetService(typeof(Caméra)) as Caméra;
            BoiteDeCollision = new BoundingBox(Position + PointMinBDC, Position + PointMaxBDC);
            RayonCollision = 3;
            MondeÀRecalculer = false;
            ÀDétruire = false;
            EstAlliée = false;
            HauteurPositionBarrePV = new Vector3(0, 13, 0);

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            
            
            float tempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJ += tempsÉcoulé;
            if (TempsÉcouléDepuisMAJ >= IntervalleMAJ)
            {
                GestionDéplacement();
                TempsÉcouléDepuisMAJ = 0;
            }

            base.Update(gameTime);

        }

        public void GestionDéplacement()
        {
            Position = Monde.Translation;
            BoiteDeCollision = new BoundingBox(Position + PointMinBDC, Position + PointMaxBDC);
            MondeÀRecalculer = true;
        }
       
        public bool EstEnCollision(object autreObjet)
        {
            return false;
        }
    }

}