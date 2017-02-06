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
    public class Sprite : DrawableGameComponent
    {
        protected Rectangle Position { get; set; }
        SpriteBatch GestionSprites { get; set; }
        string TextureName { get; set; }
        Texture2D Image { get; set; }
        RessourcesManager<Texture2D> GestionnaireTextures { get; set; }



        public Sprite(Game game, Rectangle position,string textureName)
        :base(game)
        {
            Position = position;
            TextureName = textureName;
        }
        public override void Initialize()
        {
            GestionnaireTextures = Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;
            GestionSprites = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;

            base.Initialize();
        }
        protected override void LoadContent()
        {
            Image = GestionnaireTextures.Find(TextureName);
        }

        public override void Draw(GameTime gameTime)
        {
            GestionSprites.Begin();
            if (Enabled == true)
            {
                GestionSprites.Draw(Image, Position, Color.White);
            } 
            GestionSprites.End();
        }


    }
}
