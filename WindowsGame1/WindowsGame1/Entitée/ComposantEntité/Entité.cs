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
        const int PRÉCISION_MIN = 0;
        const int DOMMAGE_MIN = 0;

        int précision;

        public int PointDeVie { get; set; }
        public int Portée { get; set; }
        protected int Force { get; set; }
        protected int Armure { get; set; }
        protected int Précision
        {
            get { return précision; }
            private set
            {
                if (value > PRÉCISION_MAX) { précision = value; }
                else if(value < PRÉCISION_MIN) { précision = value; }
                else { précision = value; }
                
            }
        }
        protected bool EstAttaqué { get; set; }
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
            EstAttaqué = false;
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
            PointDeVie =- Math.Max((dégats - Armure),DOMMAGE_MIN);
        }

    }
}
