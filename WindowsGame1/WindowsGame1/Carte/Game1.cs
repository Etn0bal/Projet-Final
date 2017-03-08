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
using System.Windows.Forms;

namespace AtelierXNA
{
    public enum States { MainMenu, JoinGame, HostGame, Game ,Waiting}

    public class Game1 : Game
    {

        GraphicsDeviceManager graphics { get; set; }
        SpriteBatch GestionSprites { get; set; }
        InputManager GestionInput { get; set; }
        States State { get; set; }
        public bool EnJeu { get; set; }

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
            Services.AddService(typeof(SpriteBatch), new SpriteBatch(GraphicsDevice));
            Services.AddService(typeof(RessourcesManager<Texture2D>), new RessourcesManager<Texture2D>(this, "Textures"));
            Services.AddService(typeof(InputManager), GestionInput);
            Services.AddService(typeof(RessourcesManager<Model>), new RessourcesManager<Model>(this, "Models"));
            Services.AddService(typeof(RessourcesManager<SpriteFont>), new RessourcesManager<SpriteFont>(this, "Fonts"));

            State = States.MainMenu;
            JoinMenu joinMenu = new JoinMenu(this);
            Components.Add(joinMenu);
            HostMenu hostMenu = new HostMenu(this);
            Components.Add(hostMenu);
            MainMenu mainMenu = new MainMenu(this);
            Components.Add(mainMenu);

            EnJeu = false;


            base.Initialize();
        }
        protected override void LoadContent()
        {



        }
        protected override void Update(GameTime gameTime)
        {
            if (State == States.MainMenu)
            {
                InitialiserMainMenu();
                State = States.Waiting;
            }
            if (State==States.HostGame)
            {
                InitialiserHostMenu();
                State = States.Waiting;
            }
            if (State == States.JoinGame)
            {
                InitialiserJoinMenu();
                State = States.Waiting;
            }
            if(State == States.Game)
            {
                InitialiserGame();
                State = States.Waiting;
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            base.Draw(gameTime);
        }


        public void ChangerDÉtat(int numÉtat)
        {

            if (numÉtat == (int)States.MainMenu)
            {
                State = States.MainMenu;
            }
            if (numÉtat == (int)States.JoinGame)
            {
                State = States.JoinGame;
            }
            if (numÉtat == (int)States.HostGame)
            {
                State = States.HostGame;

            }
            if (numÉtat == (int)States.Game)
            {
                State = States.Game;
            }
        }



        void InitialiserMainMenu()
        {
            foreach (GameComponent Mm in Components.Where(x => x is MainMenu))
            {
                Mm.Enabled = true;
            }
            foreach (GameComponent gc in Components.Where(x => x is SpriteMainMenu))
            {
                gc.Enabled = true;       
                        
            }
            foreach (GameComponent Hm in Components.Where(x => x is HostMenu))
            {
                Hm.Enabled = false;
            }
            foreach (GameComponent gc in Components.Where(x => x is SpriteHostMenu))
            {
                gc.Enabled = false;
            }
            foreach (GameComponent Jm in Components.Where(x => x is JoinMenu))
            {
                Jm.Enabled = false;
            }
            foreach (GameComponent gc in Components.Where(x => x is SpriteJoinMenu))
            {
                gc.Enabled = false;
            }
        }
        void InitialiserHostMenu()
        {
            foreach (GameComponent Mm in Components.Where(x => x is MainMenu))
            {
                Mm.Enabled = false;
            }
            foreach (GameComponent gc in Components.Where(x => x is SpriteMainMenu))
            {
                gc.Enabled = false;
            }
            foreach (GameComponent Hm in Components.Where(x => x is HostMenu))
            {
                Hm.Enabled = true;
            }
            foreach (GameComponent gc in Components.Where(x => x is SpriteHostMenu))
            {
                gc.Enabled = true;
            }
            foreach (GameComponent Jm in Components.Where(x => x is JoinMenu))
            {
                Jm.Enabled = false;
            }
            foreach (GameComponent gc in Components.Where(x => x is SpriteJoinMenu))
            {
                gc.Enabled = false;
            }
        }
        void InitialiserJoinMenu()
        {
            foreach (GameComponent Mm in Components.Where(x => x is MainMenu))
            {
                Mm.Enabled = false;
            }
            foreach (GameComponent gc in Components.Where(x => x is SpriteMainMenu))
            {
                gc.Enabled = false;
            }
            foreach (GameComponent Hm in Components.Where(x => x is HostMenu))
            {
                Hm.Enabled = false;
            }
            foreach (GameComponent gc in Components.Where(x => x is SpriteHostMenu))
            {
                gc.Enabled = false;
            }
            foreach (GameComponent Jm in Components.Where(x => x is JoinMenu))
            {
                Jm.Enabled = true;
            }
            foreach (GameComponent gc in Components.Where(x => x is SpriteJoinMenu))
            {
                gc.Enabled = true;
            }
        }
        void InitialiserGame()
        {
            foreach (GameComponent gc in Components.Where(x => x is SpriteHostMenu))
            {
                gc.Enabled = false;
            }
            TheGame game = new TheGame(this);
            Components.Add(game);

        }
    }
}
