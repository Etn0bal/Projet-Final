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
    public class Murs : PrimitiveDeBaseAnim�e
    {
        const int NB_TRIANGLES_PAR_TUILE = 2;
        const int NB_SOMMETS_PAR_TRIANGLE = 3;


        string NomMurs { get; set; }
        BasicEffect EffetDeBase { get; set; }
        VertexPositionColor[] Sommets { get; set; }
        Vector3[,] PtsSommets { get; set; }
        Vector3 �tendue { get; set; }
        Vector3 Origine { get; set; }
        Vector3 DeltaPoint { get; set; }
        RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }
        Color[,] DataTexture { get; set; }
        Texture2D MursTexture { get; set; }
        int NbColonnes { get; set; }
        int NbRang�es { get; set; }
        int Cpt { get; set; }

        public Murs(Game game, float homoth�tieInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, Vector3 �tendue, string nomMurs,
                         float intervalleMAJ)

            : base(game, homoth�tieInitiale, rotationInitiale, positionInitiale, intervalleMAJ)
        {
            NomMurs = nomMurs;
            �tendue = �tendue;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            Cpt = 0;
            GestionnaireDeTextures = Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;
            MursTexture = GestionnaireDeTextures.Find(NomMurs);
            InitialiserDonn�esCarte();
            Origine = new Vector3(-�tendue.X / 2, 0, �tendue.Z / 2); //pour centrer la primitive au point (0,0,0)
            AllouerTableaux();
            Cr�erTableauPoints();


            base.Initialize();
        }

        //
        // � partir de la texture servant de carte de hauteur (HeightMap), on initialise les donn�es
        // relatives � la structure de la carte
        //
        void InitialiserDonn�esCarte()
        {
            NbColonnes = MursTexture.Width;
            NbRang�es = MursTexture.Height;

            DeltaPoint = new Vector3(�tendue.Z / NbRang�es, �tendue.Y, �tendue.X / NbColonnes);

            Color[] dataTexture = new Color[NbRang�es * NbColonnes];
            MursTexture.GetData<Color>(dataTexture);
            TransformerEn2d(dataTexture);
        }

        void TransformerEn2d(Color[] dataTexture)
        {
            int cpt = 0;
            DataTexture = new Color[NbColonnes, NbRang�es];

            for (int j = 0; j < NbRang�es; j++)
            {
                for (int i = 0; i < NbColonnes; i++)
                {
                    DataTexture[i, j] = dataTexture[cpt];
                    ++cpt;
                }
            }
        }

        void AllouerTableaux()
        {
            PtsSommets = new Vector3[NbColonnes, NbRang�es];

            Sommets = new VertexPositionColor[(18+24)*30];
        }


        private void Cr�erTableauPoints()
        {
            for (int rang�e = 0; rang�e < PtsSommets.GetLength(1); rang�e++)
            {
                for (int colonne = 0; colonne < PtsSommets.GetLength(0); colonne++)
                {

                        PtsSommets[colonne, rang�e] = new Vector3(Origine.X + (rang�e * DeltaPoint.X),
                                                              Origine.Y ,
                                                              Origine.Z - (colonne * DeltaPoint.Z));
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
            for (int i = 1; i < 18 ; i++)
            {
                AjouterPanDeMur(10 * i, 10 * (i + 1));
                if(i < 6)
                {
                    AjouterPanDeMur((10 * i) + 1, (10 * (i + 1)) + 1);
                    AjouterPanDeMur((10 * i) + 2, (10 * (i + 1)) + 2);
                    AjouterPanDeMur((10 * i) + 3, (10 * (i + 1)) + 3);
                    AjouterPanDeMur((10 * i) + 4, (10 * (i + 1)) + 4);
                }
            }
            AjouterPanDeMur(180, 10);
            AjouterPanDeMur(61, 11);
            AjouterPanDeMur(62, 12);
            AjouterPanDeMur(63, 13);
            AjouterPanDeMur(64, 14);


        }

        void AjouterPanDeMur(int valeurCouleurPoint1, int valeurCouleurPoint2)
        {
            Vector3 point1 = Vector3.Zero;
            Vector3 point2 = Vector3.Zero;
            Vector3 hauteur = new Vector3(0, 3, 0);
            Vector3 direction;
            Vector3 lat�ral;

            for (int rang�e = 0; rang�e < PtsSommets.GetLength(1) - 1; ++rang�e)
            {
                for (int colonne = 0; colonne < PtsSommets.GetLength(0) - 1; ++colonne)
                {
                    if(DataTexture[colonne,rang�e].R == valeurCouleurPoint1)
                    {
                        point1 = PtsSommets[colonne, rang�e];
                    }
                    if(DataTexture[colonne,rang�e].R == valeurCouleurPoint2)
                    {
                        point2 = PtsSommets[colonne, rang�e];
                    }
                }
            }

            direction = point2 - point1;
            lat�ral = Vector3.Normalize( new Vector3(-direction.Z, 0, direction.X));

            Vector3[] pts = { 
                              point1 + lat�ral, point1+lat�ral+hauteur, point2+lat�ral, point2+lat�ral+hauteur,
                              point1 - lat�ral, point1-lat�ral+hauteur, point2-lat�ral, point2-lat�ral+hauteur};

            Sommets[Cpt++] = new VertexPositionColor(pts[0], Color.Brown);
            Sommets[Cpt++] = new VertexPositionColor(pts[1], Color.Black);
            Sommets[Cpt++] = new VertexPositionColor(pts[2], Color.Red);

            Sommets[Cpt++] = new VertexPositionColor(pts[2], Color.Brown);
            Sommets[Cpt++] = new VertexPositionColor(pts[1], Color.Black);
            Sommets[Cpt++] = new VertexPositionColor(pts[3], Color.Red);

            Sommets[Cpt++] = new VertexPositionColor(pts[2], Color.Brown);
            Sommets[Cpt++] = new VertexPositionColor(pts[3], Color.Black);
            Sommets[Cpt++] = new VertexPositionColor(pts[6], Color.Red);

            Sommets[Cpt++] = new VertexPositionColor(pts[6], Color.Brown);
            Sommets[Cpt++] = new VertexPositionColor(pts[3], Color.Black);
            Sommets[Cpt++] = new VertexPositionColor(pts[7], Color.Red);

            Sommets[Cpt++] = new VertexPositionColor(pts[6], Color.Brown);
            Sommets[Cpt++] = new VertexPositionColor(pts[7], Color.Black);
            Sommets[Cpt++] = new VertexPositionColor(pts[4], Color.Red);

            Sommets[Cpt++] = new VertexPositionColor(pts[4], Color.Brown);
            Sommets[Cpt++] = new VertexPositionColor(pts[7], Color.Black);
            Sommets[Cpt++] = new VertexPositionColor(pts[5], Color.Red);

            Sommets[Cpt++] = new VertexPositionColor(pts[4], Color.Brown);
            Sommets[Cpt++] = new VertexPositionColor(pts[5], Color.Black);
            Sommets[Cpt++] = new VertexPositionColor(pts[0], Color.Red);

            Sommets[Cpt++] = new VertexPositionColor(pts[0], Color.Brown);
            Sommets[Cpt++] = new VertexPositionColor(pts[5], Color.Black);
            Sommets[Cpt++] = new VertexPositionColor(pts[1], Color.Red);

            Sommets[Cpt++] = new VertexPositionColor(pts[1], Color.Brown);
            Sommets[Cpt++] = new VertexPositionColor(pts[5], Color.Black);
            Sommets[Cpt++] = new VertexPositionColor(pts[3], Color.Red);

            Sommets[Cpt++] = new VertexPositionColor(pts[3], Color.Brown);
            Sommets[Cpt++] = new VertexPositionColor(pts[5], Color.Black);
            Sommets[Cpt++] = new VertexPositionColor(pts[7], Color.Red);


        }




        public override void Draw(GameTime gameTime)
        {
            EffetDeBase.World = GetMonde();
            EffetDeBase.View = Cam�raJeu.Vue;
            EffetDeBase.Projection = Cam�raJeu.Projection;

            foreach (EffectPass passeEffet in EffetDeBase.CurrentTechnique.Passes)
            {
                passeEffet.Apply();
                GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, Sommets, 0, 10*(18+24));
            }
        }
    }
}
