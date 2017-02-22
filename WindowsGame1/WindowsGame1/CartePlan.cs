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
    public class CartePlan : PrimitiveDeBaseAnimée
    {
        const int NB_TRIANGLES_PAR_TUILE = 2;
        const int NB_SOMMETS_PAR_TRIANGLE = 3;


        string NomCartePlan { get; set; }
        BasicEffect EffetDeBase { get; set; }
        VertexPositionTexture[] Sommets { get; set; }
        Vector2[,] PtsTexture { get; set; }
        Vector3[,] PtsSommets { get; set; }
        Vector3 Étendue { get; set; }
        Vector3 Origine { get; set; }
        Vector3 DeltaPoint { get; set; }
        RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }
        Color[,] DataTexture { get; set; }
        Texture2D CartePlanTexture { get; set; }
        int NbColonnes { get; set; }
        int NbRangées { get; set; }

        public CartePlan(Game game, float homothétieInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, Vector3 étendue, string nomCartePlan, 
                         float intervalleMAJ)

            : base(game, homothétieInitiale, rotationInitiale, positionInitiale, intervalleMAJ)
        {
            NomCartePlan = nomCartePlan;
            Étendue = étendue;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            GestionnaireDeTextures = Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;
            CartePlanTexture = GestionnaireDeTextures.Find(NomCartePlan);
            InitialiserDonnéesCarte();
            Origine = new Vector3(-Étendue.X / 2, 0, Étendue.Z / 2); //pour centrer la primitive au point (0,0,0)
            AllouerTableaux();
            CréerTableauPoints();
            CréerTableauPointsTexture();

            base.Initialize();
        }

        //
        // à partir de la texture servant de carte de hauteur (HeightMap), on initialise les données
        // relatives à la structure de la carte
        //
        void InitialiserDonnéesCarte()
        {
            NbColonnes = CartePlanTexture.Width;
            NbRangées = CartePlanTexture.Height;

            DeltaPoint = new Vector3(Étendue.Z / NbRangées, Étendue.Y, Étendue.X / NbColonnes);

            Color[] dataTexture = new Color[NbRangées * NbColonnes];
            CartePlanTexture.GetData<Color>(dataTexture);
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

            Sommets = new VertexPositionTexture[NB_TRIANGLES_PAR_TUILE * (NbRangées - 1) * (NbColonnes - 1) * NB_SOMMETS_PAR_TRIANGLE];

            PtsTexture = new Vector2[NbColonnes, NbRangées];
        }

        private void CréerTableauPoints()
        {
            for (int rangée = 0; rangée < PtsSommets.GetLength(1); rangée++)
            {
                for (int colonne = 0; colonne < PtsSommets.GetLength(0); colonne++)
                {
                    if (DataTexture[colonne, rangée].G <= 50)
                    {
                        PtsSommets[colonne, rangée] = new Vector3(Origine.X + (rangée * DeltaPoint.X),
                                                                  Origine.Y ,
                                                                  Origine.Z - (colonne * DeltaPoint.Z));
                    }
                    else
                    {
                        PtsSommets[colonne, rangée] = new Vector3(Origine.X + (rangée * DeltaPoint.X),
                                                                  Origine.Y ,
                                                                  Origine.Z - (colonne * DeltaPoint.Z));
                    }
                }
            }
        }

        private void CréerTableauPointsTexture()
        {

            for (int i = 0; i < PtsSommets.GetLength(0); ++i)
            {
                for (int j = 0; j < PtsSommets.GetLength(1); ++j)
                {
                    PtsTexture[i, j] = new Vector2((float)i / NbColonnes, (float)(NbRangées - j) / NbRangées);
                }
            }
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            EffetDeBase = new BasicEffect(GraphicsDevice);
            InitialiserParamètresEffetDeBase();
        }

        void InitialiserParamètresEffetDeBase()
        {
            EffetDeBase.TextureEnabled = true;
            EffetDeBase.Texture = CartePlanTexture;
        }

        protected override void InitialiserSommets()
        {
            int NoSommet = -1;
            for (int j = 0; j < NbRangées-1; ++j)
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
            EffetDeBase.View = CaméraJeu.Vue;
            EffetDeBase.Projection = CaméraJeu.Projection;

            foreach (EffectPass passeEffet in EffetDeBase.CurrentTechnique.Passes)
            {
                passeEffet.Apply();
                GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleList, Sommets, 0, NB_TRIANGLES_PAR_TUILE * (NbRangées - 1) * (NbColonnes - 1));
            }
        }
    }
}
