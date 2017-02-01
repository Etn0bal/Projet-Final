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
    public enum States { MainMenu, JoinGame, HostGame, Game }

    public class Game1 : Game
    {

        RessourcesManager<Texture2D> GestionnaireDeTexture{ get; set; }
        InputManager GestionnaireInput { get; set; }
        GraphicsDeviceManager graphics { get; set; }
        SpriteBatch GestionSprites { get; set; }
        InputManager GestionInput { get; set; }
        States State { get; set; }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            GestionInput = new InputManager(this);
            Components.Add(GestionInput);
            Services.AddService(typeof(SpriteBatch),new SpriteBatch(GraphicsDevice));
            Services.AddService(typeof(RessourcesManager<Texture2D>), new RessourcesManager<Texture2D>(this, "Textures"));
            Services.AddService(typeof(InputManager), GestionInput);

            State = States.MainMenu;
            ChangingStateButton CreateGameButton = new ChangingStateButton(this, 0, 0, 10, 10, "Nothing", 1);
            ChangingStateButton HostGameButton = new ChangingStateButton(this, 0, 0, 10, 10, "Nothing", 2);
            Sprite TitreMainMenu = new Sprite(this, 0, 0, 0, 0, "Nothing");




            base.Initialize();
        }
        protected override void LoadContent()
        {



        }
        protected override void Update(GameTime gameTime)
        {
            if(State == States.MainMenu)
            {

            }

            if (GestionnaireInput.EstEnfoncÈe(Keys.Escape))
            {
                Exit();
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);
        }


        public void ChangerD…tat(int num…tat)
        {
            if(num…tat==(int)States.MainMenu)
            {
                State = States.MainMenu;
            }
            if (num…tat == (int)States.JoinGame)
            {
                State = States.JoinGame;
            }
            if (num…tat == (int)States.HostGame)
            {
                State = States.HostGame;
            }
            if (num…tat == (int)States.Game)
            {
                State = States.Game;
            }
        }
    }
}
