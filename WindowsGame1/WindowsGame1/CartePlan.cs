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
    public class CartePlan : PrimitiveDeBaseAnim�e
    {
        const int NB_TRIANGLES_PAR_TUILE = 2;
        const int NB_SOMMETS_PAR_TRIANGLE = 3;


        string NomCartePlan { get; set; }
        BasicEffect EffetDeBase { get; set; }
        VertexPositionTexture[] Sommets { get; set; }
        Vector2[,] PtsTexture { get; set; }
        Vector3[,] PtsSommets { get; set; }
        Vector3 �tendue { get; set; }
        Vector3 Origine { get; set; }
        Vector3 DeltaPoint { get; set; }
        RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }
        Color[,] DataTexture { get; set; }
        Texture2D CartePlanTexture { get; set; }
        int NbColonnes { get; set; }
        int NbRang�es { get; set; }

        public CartePlan(Game game, float homoth�tieInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, Vector3 �tendue, string nomCartePlan, 
                         float intervalleMAJ)

            : base(game, homoth�tieInitiale, rotationInitiale, positionInitiale, intervalleMAJ)
        {
            NomCartePlan = nomCartePlan;
            �tendue = �tendue;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            GestionnaireDeTextures = Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;
            CartePlanTexture = GestionnaireDeTextures.Find(NomCartePlan);
            InitialiserDonn�esCarte();
            Origine = new Vector3(-�tendue.X / 2, 0, �tendue.Z / 2); //pour centrer la primitive au point (0,0,0)
            AllouerTableaux();
            Cr�erTableauPoints();
            Cr�erTableauPointsTexture();

            base.Initialize();
        }

        //
        // � partir de la texture servant de carte de hauteur (HeightMap), on initialise les donn�es
        // relatives � la structure de la carte
        //
        void InitialiserDonn�esCarte()
        {
            NbColonnes = CartePlanTexture.Width;
            NbRang�es = CartePlanTexture.Height;

            DeltaPoint = new Vector3(�tendue.Z / NbRang�es, �tendue.Y, �tendue.X / NbColonnes);

            Color[] dataTexture = new Color[NbRang�es * NbColonnes];
            CartePlanTexture.GetData<Color>(dataTexture);
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

            Sommets = new VertexPositionTexture[NB_TRIANGLES_PAR_TUILE * (NbRang�es - 1) * (NbColonnes - 1) * NB_SOMMETS_PAR_TRIANGLE];

            PtsTexture = new Vector2[NbColonnes, NbRang�es];
        }

        private void Cr�erTableauPoints()
        {
            for (int rang�e = 0; rang�e < PtsSommets.GetLength(1); rang�e++)
            {
                for (int colonne = 0; colonne < PtsSommets.GetLength(0); colonne++)
                {
                    if (DataTexture[colonne, rang�e].G <= 50)
                    {
                        PtsSommets[colonne, rang�e] = new Vector3(Origine.X + (rang�e * DeltaPoint.X),
                                                                  Origine.Y ,
                                                                  Origine.Z - (colonne * DeltaPoint.Z));
                    }
                    else
                    {
                        PtsSommets[colonne, rang�e] = new Vector3(Origine.X + (rang�e * DeltaPoint.X),
                                                                  Origine.Y ,
                                                                  Origine.Z - (colonne * DeltaPoint.Z));
                    }
                }
            }
        }

        private void Cr�erTableauPointsTexture()
        {

            for (int i = 0; i < PtsSommets.GetLength(0); ++i)
            {
                for (int j = 0; j < PtsSommets.GetLength(1); ++j)
                {
                    PtsTexture[i, j] = new Vector2((float)i / NbColonnes, (float)(NbRang�es - j) / NbRang�es);
                }
            }
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            EffetDeBase = new BasicEffect(GraphicsDevice);
            InitialiserParam�tresEffetDeBase();
        }

        void InitialiserParam�tresEffetDeBase()
        {
            EffetDeBase.TextureEnabled = true;
            EffetDeBase.Texture = CartePlanTexture;
        }

        protected override void InitialiserSommets()
        {
            int NoSommet = -1;
            for (int j = 0; j < NbRang�es-1; ++j)
            {
                for (int i = 0; i < NbColonnes-1; ++i)
                {
                    Sommets[++NoSommet] = new VertexPositionTexture(PtsSommets[i, j], PtsTexture[i, j]);
                    Sommets[++NoSommet] = new VertexPositionTexture(PtsSommets[i + 1, j], PtsTexture[i + 1, j]);
                    Sommets[++NoSommet] = new VertexPositionTexture(PtsSommets[i, j + 1], PtsTexture[i, j + 1]);
                    

                    Sommets[++NoSommet] = new VertexPositionTexture(PtsSommets[i, j+1], PtsTexture[i, j+1]);
                    Sommets[++NoSommet] = new VertexPositionTexture(PtsSommets[i + 1, j], PtsTexture[i + 1, j]);
                    Sommets[++NoSommet] = new VertexPositionTexture(PtsSommets[i + 1, j + 1], PtsTexture[i + 1, j + 1]);

                }
            }
        }

        
        public override void Draw(GameTime gameTime)
        {
            EffetDeBase.World = GetMonde();
            EffetDeBase.View = Cam�raJeu.Vue;
            EffetDeBase.Projection = Cam�raJeu.Projection;

            foreach (EffectPass passeEffet in EffetDeBase.CurrentTechnique.Passes)
            {
                passeEffet.Apply();
                GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleList, Sommets, 0, NB_TRIANGLES_PAR_TUILE * (NbRang�es - 1) * (NbColonnes - 1));
            }
        }
    }
}
