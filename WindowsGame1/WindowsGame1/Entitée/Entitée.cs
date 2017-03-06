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

    public class Entitée : ObjetDeDémo
    {
        int PointDeVie { get; set; }
        Vector2 Position { get; set; }
        public int Portée { get; set; }
        int Force { get; set; }
        int Armure { get; set; }
        bool EstAttaqué { get; set; }
        public Entitée(Game jeu, string nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale,
                           float intervalleMAJ,int pointDeVie,Vector2 position,int portée,int force,int armure)
            : base(jeu,nomModèle,échelleInitiale,rotationInitiale,positionInitiale,intervalleMAJ)
        {
            PointDeVie = pointDeVie;
            Position = position;
            Portée = portée;
            Force = force;
            Armure = armure;
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
        protected void Défendre(int Attaque)
        {
            int ptDeDégat = Math.Max(Attaque - Armure, 0);
            PointDeVie -= ptDeDégat;
        }

    }
}
