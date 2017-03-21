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
    public class Texte : Microsoft.Xna.Framework.DrawableGameComponent
    {
        float PourcentageZoneAffichable;
        string Texte¿Afficher { get; set; }
        string NomFont { get; set; }
        Rectangle ZoneAffichage { get; set; }
        Vector2 PositionAffichage { get; set; }
        Color CouleurTexte { get; set; }
        Vector2 Origine { get; set; }
        float …chelle { get; set; }
        SpriteFont PoliceDeCaractËres { get; set; }
        SpriteBatch GestionSprites { get; set; }
        RessourcesManager<SpriteFont> GestionnaireDeFonts { get; set; }

        public Texte(Game jeu, string texte¿Afficher, string nomFont, Rectangle zoneAffichage,Vector2 positionAffichage,
                           Color couleurTexte, float marge)
           : base(jeu)
        {
            Texte¿Afficher = texte¿Afficher;
            NomFont = nomFont;
            CouleurTexte = couleurTexte;
            ZoneAffichage = zoneAffichage;
            PourcentageZoneAffichable = 1f - marge;
            PositionAffichage = positionAffichage;
        }

        protected override void LoadContent()
        {
            GestionSprites = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;
            GestionnaireDeFonts = Game.Services.GetService(typeof(RessourcesManager<SpriteFont>)) as RessourcesManager<SpriteFont>;
            PoliceDeCaractËres = GestionnaireDeFonts.Find(NomFont);
            ModifierTexte(Texte¿Afficher);
        }

        public void ModifierTexte(string texte¿Afficher)
        {
            Texte¿Afficher = texte¿Afficher;
            Vector2 dimensionTexte = PoliceDeCaractËres.MeasureString(Texte¿Afficher);
            float ÈchelleHorizontale = MathHelper.Max(MathHelper.Min(ZoneAffichage.Width * PourcentageZoneAffichable, dimensionTexte.X), ZoneAffichage.Width * PourcentageZoneAffichable) / dimensionTexte.X;
            float ÈchelleVerticale = MathHelper.Max(MathHelper.Min(ZoneAffichage.Height * PourcentageZoneAffichable, dimensionTexte.Y), ZoneAffichage.Height * PourcentageZoneAffichable) / dimensionTexte.Y;
            …chelle = MathHelper.Min(ÈchelleHorizontale, ÈchelleVerticale);
            Origine = dimensionTexte / 2;
        }

        public override void Draw(GameTime gameTime)
        {
            GestionSprites.Begin();
            GestionSprites.DrawString(PoliceDeCaractËres, Texte¿Afficher, PositionAffichage, CouleurTexte, 0, Origine, …chelle, SpriteEffects.None, 0);
            GestionSprites.End();
        }
    }
}
