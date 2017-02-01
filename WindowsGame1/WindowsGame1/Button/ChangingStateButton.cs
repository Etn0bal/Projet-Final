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
    public class ChangingStateButton : Sprite
    {
        int NextState { get; set; }
        InputManager GestionnaireInputs { get; set; }

        public ChangingStateButton(Game game, float posX, float posY, float lenght, float height, string textureName, int nextState)
            : base(game,posX,posY,lenght,height,textureName)
        {
            NextState = nextState;
        }

        public override void Initialize()
        {
            GestionnaireInputs = Game.Services.GetService(typeof(InputManager)) as InputManager;
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            GérerSouris();          
            base.Update(gameTime);
        }
        void GérerSouris()
        {
            if(GestionnaireInputs.EstNouveauClicGauche())
            {
                if(Position.Contains(GestionnaireInputs.GetPositionSouris()))
                {
                    Game1.ChangerDÉtat(NextState);
                }
            }
        }
    }
}
