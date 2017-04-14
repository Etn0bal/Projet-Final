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

    public class Minuteur : Microsoft.Xna.Framework.DrawableGameComponent
    {
        const int MINUTE_EN_SECONDE = 60;
        const int INTERVALLEMAJ = 60;
        public int Minutes { get; set; }
        public int Secondes { get; set; }
        float Temps…coulÈDepuisMAJ { get; set; }
        string TexteMinuteur { get; set; }
        Texte Minuteur¿Affficher { get; set; }

        public Minuteur(Game game)
            : base(game)
        {

        }

        public override void Initialize()
        {
            Minutes = 0;
            Secondes = 0;
            Temps…coulÈDepuisMAJ = 0;
            InitialiserTexte();
            Rectangle positionTexte = new Rectangle(Game.Window.ClientBounds.Width,  Game.Window.ClientBounds.Height, 3 * (Game.Window.ClientBounds.Width / 20), (Game.Window.ClientBounds.Height / 15));
            Minuteur¿Affficher = new Texte(Game, TexteMinuteur, "Arial", positionTexte, new Vector2(9* Game.Window.ClientBounds.Width/10, Game.Window.ClientBounds.Height/15), Color.White, 0);
            Game.Components.Add(Minuteur¿Affficher);
            base.Initialize();
        }

        private void InitialiserTexte()
        {
            string texteSeconde;
            string texteMinute;
            if (Secondes < 10)
            {
                texteSeconde = "0" + Secondes.ToString();
            }
            else { texteSeconde = Secondes.ToString(); }
            if(Minutes < 10)
            {
                texteMinute = "0" + Minutes.ToString();
            }
            else { texteMinute = Minutes.ToString(); }
            TexteMinuteur = texteMinute + " : " + texteSeconde;
        }

        public override void Update(GameTime gameTime)
        {
            float temps…coulÈ = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Temps…coulÈDepuisMAJ += temps…coulÈ;
            if (Temps…coulÈDepuisMAJ >= 1)
            {
                Secondes++;
                if (Secondes == 59)
                {
                    Secondes = 0;
                    Minutes += 1;
                }

                Temps…coulÈDepuisMAJ = 0;
                ModifierTexte();
                
            }
            base.Update(gameTime);
        }

        private void ModifierTexte()
        {
            string texteSeconde;
            string texteMinute;
            if (Secondes < 10)
            {
                texteSeconde = "0" + Secondes.ToString();
            }
            else { texteSeconde = Secondes.ToString(); }
            if (Minutes < 10)
            {
                texteMinute = "0" + Minutes.ToString();
            }
            else { texteMinute = Minutes.ToString(); }
            TexteMinuteur = texteMinute + " : " + texteSeconde;
            Minuteur¿Affficher.ModifierTexte(TexteMinuteur);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
