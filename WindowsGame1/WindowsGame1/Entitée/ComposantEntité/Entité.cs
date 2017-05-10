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

    public class Entité : ObjetDeDémo, IDestructible
    {
        const int PRÉCISION_MAX = 100;
        const int CONSTANTE_MIN_ZÉRO = 0;
        protected Vector3 RotationInitialeProjectielADB = new Vector3(0, 0, (float)-Math.PI / 4);
        protected Vector3 DirectionInitialeProjectileADB = new Vector3(1, 0, 0);
        protected const float ÉCHELLE_PROJECTILE_ATTAQUE_DE_BASE = 0.000009f;
        protected const float ÉCHELLE_PROJECTILE_W = 0.05f;


        protected GestionnaireJeu LeJeu { get; set; }
        protected Entité Cible { get; set; }
        public BoundingBox BoiteDeCollision { get; protected set; }
        public BoundingBox NouvelleBoiteDeCollision { get; protected set; }
        protected Vector3 HauteurPositionBarrePV { get; set; }
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
        BarreDeVie BarrePV { get; set; }
        protected int PointDeVieInitial { get; private set; }
        int portée;
        protected int Portée
        {
            get { return portée; }
            set
            {
                if (value < CONSTANTE_MIN_ZÉRO) { portée = CONSTANTE_MIN_ZÉRO; }
                else { portée = value; }
            }
        }
        int force;
        protected int Force
        {
            get { return force; }
            set
            {
                if (value < CONSTANTE_MIN_ZÉRO) { force = CONSTANTE_MIN_ZÉRO; }
                else { force = value; }
            }
        }
        int armure;
        protected int Armure
        {
            get { return armure; }
            set
            {
                if (value < CONSTANTE_MIN_ZÉRO) { armure = CONSTANTE_MIN_ZÉRO; }
                else { armure = value; }
            }
        }
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
        public bool EstAlliée { get; protected set; }
        public bool ÀDétruire { get; set; }
        public float RayonCollision { get; protected set; }
        Vector3 nouvellePosition;
        public Vector3 NouvellePosition 
        {
            get
            {
                return new Vector3(nouvellePosition.X, nouvellePosition.Y, nouvellePosition.Z);
            }
            protected set { nouvellePosition = value; }
        }



        public Entité(Microsoft.Xna.Framework.Game jeu, string nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale,
                           float intervalleMAJ, int pointDeVie, int portée, int force, int armure, int précision)
            : base(jeu, nomModèle, échelleInitiale, rotationInitiale, positionInitiale, intervalleMAJ)
        {
            PointDeVie = pointDeVie;
            PointDeVieInitial = pointDeVie;
            Portée = portée;
            Force = force;
            Armure = armure;
            Précision = précision;
        }

        public override void Initialize()
        {
            BarrePV = new BarreDeVie(Game, 1, Vector3.Zero, Position + HauteurPositionBarrePV , new Vector3(4, 0, 1), IntervalleMAJ, PointDeVie, HauteurPositionBarrePV);
            Game.Components.Add(BarrePV);
            LeJeu = Game.Services.GetService(typeof(GestionnaireJeu)) as GestionnaireJeu;
            base.Initialize();

        }

        public override void Update(GameTime gameTime)
        {
            GestionVie();
            BarrePV.ChangerPosition(Position);

            if (PointDeVie == 0) { BarrePV.ÀDétruire = true; }
            if (BarrePV.PointDeVie != PointDeVie)
            {
                BarrePV.ChangerBarreDeVie(PointDeVie);
            }

            base.Update(gameTime);
        }

        public void RecevoirAttaque(int dégats)
        {
            PointDeVie -= Math.Max((dégats - Armure),CONSTANTE_MIN_ZÉRO);
        }

        private void GestionVie()
        {
            if (PointDeVie == 0)
            {
                ÀDétruire = true;
            }
        }

    }
}
