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
    public enum States { MainMenu, JoinGame, HostGame, Game ,Waiting,EnAttenteDeLaPartie}

    public class Game : Microsoft.Xna.Framework.Game
    {

        GraphicsDeviceManager graphics { get; set; }
        SpriteBatch GestionSprites { get; set; }
        InputManager GestionInput { get; set; }
        States State { get; set; }
        public bool EnJeu { get; set; }
        public int NumClient {get;set;}
        JoinMenu JoinMenu { get; set; }
        HostMenu HostMenu { get; set; }
        MainMenu MainMenu { get; set; }


        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;
            IsMouseVisible = true;
            graphics.IsFullScreen = true;

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
            Services.AddService(typeof(Random), new Random());

            State = States.MainMenu;
            JoinMenu = new JoinMenu(this);
            Components.Add(JoinMenu);
            HostMenu = new HostMenu(this);
            Components.Add(HostMenu);
            MainMenu = new MainMenu(this);
            Components.Add(MainMenu);

            EnJeu = false;

           


            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            if (State != States.Waiting)
            {
                if (State == States.MainMenu)
                {
                    InitialiserMainMenu();
                    State = States.Waiting;
                }
                if (State == States.HostGame)
                {
                    InitialiserHostMenu();
                    State = States.Waiting;
                }
                if (State == States.JoinGame)
                {
                    InitialiserJoinMenu();
                    State = States.Waiting;
                }
                if (State == States.Game)
                {
                    InitialiserGame();
                    State = States.Waiting;
                }
                if (State == States.EnAttenteDeLaPartie)
                {
                    InitialiserAttentePartie();
                    State = States.Waiting;
                }
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
            if (numÉtat == (int)States.EnAttenteDeLaPartie)
            {
                State = States.EnAttenteDeLaPartie;
            }
        }



        void InitialiserMainMenu()
        {
            
            foreach (GameComponent Mm in Components.Where(x => x is MainMenu))
            {
                MainMenu.Enabled = true;
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
            foreach (GameComponent gc in Components.Where(x => x is Texte))
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
            foreach (GameComponent txt in Components.Where(x => x is TexteHostMenu))
            {
                txt.Enabled = true;
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
            foreach (GameComponent Jm in Components.Where(x => x is JoinMenu))
            {
                Jm.Enabled = true;
            }
            foreach (GameComponent gc in Components.Where(x => x is SpriteJoinMenu))
            {
                gc.Enabled = true;
            }
            foreach (GameComponent txt in Components.Where(x => x is TexteJoinMenu))
            {
                txt.Enabled = true;
            }




        }
        void InitialiserGame()
        {
            NettoyerListeComponents();
            GestionnaireJeu game = new GestionnaireJeu(this,NumClient);
            Services.AddService(typeof(GestionnaireJeu), game);
            Components.Add(game);

        }

        private void InitialiserAttentePartie()
        {
            NettoyerListeComponents();
            Sprite FondDécranDattente = new Sprite(this, new Rectangle(0, 0, Window.ClientBounds.Width, Window.ClientBounds.Height), "imagedattente");
            Components.Add(FondDécranDattente);
        }

        void NettoyerListeComponents()
        {
            for (int i = Components.Count - 1; i >= 0; --i)
            {
                Components.RemoveAt(i);
            }
        }

    }
}
