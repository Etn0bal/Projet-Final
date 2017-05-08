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

    public class BarreDeVie : PrimitiveDeBaseAnim�e
    {
        const int POINT1 = 0;
        const int POINT2 = 1;
        const int DIRECTION = 2;
        const int VECTEUR_�QUATION_DROITE_ABC = 3;

        const int NB_TRIANGLES_PAR_TUILE = 2;
        const int NB_SOMMETS_PAR_TRIANGLE = 3;
        public bool �D�truire { get; set; }

        
        BasicEffect EffetDeBase { get; set; }
        VertexPositionColor[] Sommets { get; set; }
        Vector3[,] PtsSommets { get; set; }
        Vector3[,] TableauDeDroites { get; set; }
        Vector3 �tendue { get; set; }
        Vector3 Origine { get; set; }
        Vector3 DeltaPoint { get; set; }
        Vector3 HauteurPosition { get; set; }
        int NbColonnes { get; set; }
        int NbRang�es { get; set; }
        int Cpt { get; set; }

        public int PointDeVie { get; private set; }

        public BarreDeVie(Microsoft.Xna.Framework.Game game, float homoth�tieInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, Vector3 �tendue,
                         float intervalleMAJ,int pointDeVie, Vector3 hauteurPosition)

            : base(game, homoth�tieInitiale, rotationInitiale, positionInitiale, intervalleMAJ)
        {
            �tendue = �tendue;
            PointDeVie = pointDeVie;
            HauteurPosition = hauteurPosition;

        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            Cpt = 0;
            InitialiserDonn�es();
            Origine = new Vector3(-�tendue.X / 2, 0, �tendue.Z / 2); //pour centrer la primitive au point (0,0,0)
            AllouerTableaux();
            Cr�erTableauPoints();
            �D�truire = false;


            base.Initialize();
        }


        void InitialiserDonn�es()
        {
            NbColonnes = PointDeVie;
            NbRang�es = 1;

            DeltaPoint = new Vector3(�tendue.X / NbColonnes, �tendue.Y, �tendue.Z / NbRang�es);


        }

        void AllouerTableaux()
        {
            PtsSommets = new Vector3[NbColonnes+1, NbRang�es+1];



        }


        private void Cr�erTableauPoints()
        {
            for (int rang�e = 0; rang�e < PtsSommets.GetLength(1); rang�e++)
            {
                for (int colonne = 0; colonne < PtsSommets.GetLength(0); colonne++)
                {

                    PtsSommets[colonne, rang�e] = new Vector3(Origine.X + (colonne * DeltaPoint.X),
                                                          Origine.Y,
                                                          Origine.Z - (rang�e * DeltaPoint.Z));
                }
            }
        }



        protected override void LoadContent()
        {
            base.LoadContent();
            EffetDeBase = new BasicEffect(GraphicsDevice);
            EffetDeBase.VertexColorEnabled = true;

        }



        protected override void InitialiserSommets()
        {
            Sommets = new VertexPositionColor[6 * PointDeVie];
            for (int colonne = 0; colonne < PointDeVie; ++colonne)
            {
                Sommets[Cpt++] = new VertexPositionColor(PtsSommets[colonne, 0], Color.Red);
                //Sommets[Cpt++] = new VertexPositionColor(PtsSommets[colonne + 1, 0], Color.Red);
                Sommets[Cpt++] = new VertexPositionColor(PtsSommets[colonne, 1], Color.Red);
                Sommets[Cpt++] = new VertexPositionColor(PtsSommets[colonne + 1, 0], Color.Red);

                Sommets[Cpt++] = new VertexPositionColor(PtsSommets[colonne, 1], Color.Red);
                //Sommets[Cpt++] = new VertexPositionColor(PtsSommets[colonne + 1, 0], Color.Red);
                Sommets[Cpt++] = new VertexPositionColor(PtsSommets[colonne + 1, 1], Color.Red);
                Sommets[Cpt++] = new VertexPositionColor(PtsSommets[colonne + 1, 0], Color.Red);
            }
            Cpt = 0;
        }

        public void ChangerBarreDeVie(int PV)
        {
            PointDeVie = PV;
            InitialiserSommets();
        }

        public void ChangerPosition(Vector3 nouvellePosition)
        {
            Position = nouvellePosition + HauteurPosition;
            CalculerMatriceMonde();
        }

        public override void Draw(GameTime gameTime)
        {
            EffetDeBase.World = GetMonde();
            EffetDeBase.View = Cam�raJeu.Vue;
            EffetDeBase.Projection = Cam�raJeu.Projection;
            if (PointDeVie != 0)
            {
                foreach (EffectPass passeEffet in EffetDeBase.CurrentTechnique.Passes)
                {
                    passeEffet.Apply();
                    GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, Sommets, 0, NB_TRIANGLES_PAR_TUILE * PointDeVie);
                }
            }
        }
    }
}
