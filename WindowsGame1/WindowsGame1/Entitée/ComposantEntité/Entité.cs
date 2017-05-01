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

    public class Entité : ObjetDeDémo
    {
        const int PRÉCISION_MAX = 100;
        const int CONSTANTE_MIN_ZÉRO = 0;
        protected Vector3 RotationInitialeProjectielADB = new Vector3(0, 0, (float)-Math.PI / 4);
        protected Vector3 DirectionInitialeProjectileADB = new Vector3(1, 0, 0);
        protected const float ÉCHELLE_PROJECTILE_ATTAQUE_DE_BASE = 0.000009f;


        protected Entité Cible { get; set; }
        public BoundingBox BoiteDeCollision { get; protected set; }
        public BoundingBox NouvelleBoiteDeCollision { get; protected set; }
        int pointDeVie;
        public int PointDeVie
        {
            get { return pointDeVie; }
            set
            {
                
                if (value < CONSTANTE_MIN_ZÉRO) { pointDeVie = CONSTANTE_MIN_ZÉRO; }
                else { pointDeVie = value; }

            }
        }
        public int Portée { get; set; }
        protected int Force { get; set; }
        protected int Armure { get; set; }
        int précision;
        protected int Précision
        {
            get { return précision; }
            private set
            {
                if (value > PRÉCISION_MAX) { précision = PRÉCISION_MAX; }
                else if(value < CONSTANTE_MIN_ZÉRO) { précision = CONSTANTE_MIN_ZÉRO; }
                else { précision = value; }
                
            }
        }
        public bool EstAlliée { get; set; }
        public float RayonCollision { get; protected set; }
        public Vector3 NouvellePosition { get; protected set; }



        public Entité(Game jeu, string nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale,
                           float intervalleMAJ, int pointDeVie, int portée, int force, int armure, int précision)
            : base(jeu, nomModèle, échelleInitiale, rotationInitiale, positionInitiale, intervalleMAJ)
        {
            PointDeVie = pointDeVie;
            Portée = portée;
            Force = force;
            Armure = armure;
            Précision = précision;
        }

        public override void Initialize()
        {
            base.Initialize();

        }

        public override void Update(GameTime gameTime)
        {

            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
        public void RecevoirAttaque(int dégats)
        {
            PointDeVie -= Math.Max((dégats - Armure),CONSTANTE_MIN_ZÉRO);
        }

    }
}
