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
    class TexteModifiable : Texte
    {
        public TexteModifiable(Game jeu, string texteÀAfficher, string nomFont, Rectangle zoneAffichage, Vector2 positionAffichage,
                   Color couleurTexte, float marge)
           : base(jeu,texteÀAfficher,nomFont,zoneAffichage,positionAffichage,couleurTexte,marge)
        {
        }
    }
}
