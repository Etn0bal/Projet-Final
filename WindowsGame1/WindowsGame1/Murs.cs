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
    public class Murs : PrimitiveDeBaseAnimée
    {
        const int NB_TRIANGLES_PAR_TUILE = 2;
        const int NB_SOMMETS_PAR_TRIANGLE = 3;


        string NomMurs { get; set; }
        BasicEffect EffetDeBase { get; set; }
        VertexPositionColor[] Sommets { get; set; }
        Vector3[,] PtsSommets { get; set; }
        Vector3 Étendue { get; set; }
        Vector3 Origine { get; set; }
        Vector3 DeltaPoint { get; set; }
        RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }
        Color[,] DataTexture { get; set; }
        Texture2D MursTexture { get; set; }
        int NbColonnes { get; set; }
        int NbRangées { get; set; }
        int Cpt { get; set; }

        public Murs(Game game, float homothétieInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, Vector3 étendue, string nomMurs,
                         float intervalleMAJ)

            : base(game, homothétieInitiale, rotationInitiale, positionInitiale, intervalleMAJ)
        {
            NomMurs = nomMurs;
            Étendue = étendue;
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
            InitialiserDonnéesCarte();
            Origine = new Vector3(-Étendue.X / 2, 0, Étendue.Z / 2); //pour centrer la primitive au point (0,0,0)
            AllouerTableaux();
            CréerTableauPoints();


            base.Initialize();
        }

        //
        // à partir de la texture servant de carte de hauteur (HeightMap), on initialise les données
        // relatives à la structure de la carte
        //
        void InitialiserDonnéesCarte()
        {
            NbColonnes = MursTexture.Width;
            NbRangées = MursTexture.Height;

            DeltaPoint = new Vector3(Étendue.Z / NbRangées, Étendue.Y, Étendue.X / NbColonnes);

            Color[] dataTexture = new Color[NbRangées * NbColonnes];
            MursTexture.GetData<Color>(dataTexture);
            TransformerEn2d(dataTexture);
        }

        void TransformerEn2d(Color[] dataTexture)
        {
            int cpt = 0;
            DataTexture = new Color[NbColonnes, NbRangées];

            for (int j = 0; j < NbRangées; j++)
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
            PtsSommets = new Vector3[NbColonnes, NbRangées];

            Sommets = new VertexPositionColor[(18+24)*30];
        }


        private void CréerTableauPoints()
        {
            for (int rangée = 0; rangée < PtsSommets.GetLength(1); rangée++)
            {
                for (int colonne = 0; colonne < PtsSommets.GetLength(0); colonne++)
                {

                        PtsSommets[colonne, rangée] = new Vector3(Origine.X + (rangée * DeltaPoint.X),
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
            Vector3 latéral;

            for (int rangée = 0; rangée < PtsSommets.GetLength(1) - 1; ++rangée)
            {
                for (int colonne = 0; colonne < PtsSommets.GetLength(0) - 1; ++colonne)
                {
                    if(DataTexture[colonne,rangée].R == valeurCouleurPoint1)
                    {
                        point1 = PtsSommets[colonne, rangée];
                    }
                    if(DataTexture[colonne,rangée].R == valeurCouleurPoint2)
                    {
                        point2 = PtsSommets[colonne, rangée];
                    }
                }
            }

            direction = point2 - point1;
            latéral = Vector3.Normalize( new Vector3(-direction.Z, 0, direction.X));

            Vector3[] pts = { 
                              point1 + latéral, point1+latéral+hauteur, point2+latéral, point2+latéral+hauteur,
                              point1 - latéral, point1-latéral+hauteur, point2-latéral, point2-latéral+hauteur};

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
            EffetDeBase.View = CaméraJeu.Vue;
            EffetDeBase.Projection = CaméraJeu.Projection;

            foreach (EffectPass passeEffet in EffetDeBase.CurrentTechnique.Passes)
            {
                passeEffet.Apply();
                GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, Sommets, 0, 10*(18+24));
            }
        }
    }
}
