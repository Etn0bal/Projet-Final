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
using System.Net.Sockets;
using System.Net;
using System.Windows.Forms;
using System.IO;


namespace AtelierXNA
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class TheGame : Microsoft.Xna.Framework.DrawableGameComponent
    {
        Caméra CaméraJeu { get; set; }
        const float INTERVALLE_MAJ = 1f / 60f;

        RessourcesManager<Texture2D> GestionnaireDeTexture { get; set; }
        GraphicsDeviceManager graphics { get; set; }
        SpriteBatch GestionSprites { get; set; }
        InputManager GestionInput { get; set; }

        TcpClient Client;
        const int PORT = 5011;
        string IP = "127.0.0.1";
        const int BUFFER_SIZE = 2048;
        private byte[] readbuffer;

        MemoryStream readStream;
        BinaryReader reader;


        public TheGame(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            GestionnaireDeTexture = Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;
            graphics = Game.Services.GetService(typeof(GraphicsDeviceManager)) as GraphicsDeviceManager;
            GestionInput = Game.Services.GetService(typeof(InputManager)) as InputManager;
            GestionSprites = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;
            //CaméraJeu = new CaméraSubjective(Game, new Vector3(0, 100, 250), new Vector3(0, 0, -10), Vector3.Up, INTERVALLE_MAJ);

            //Game.Services.AddService(typeof(Caméra), CaméraJeu);
            // Game.Components.Add(CaméraJeu);
            // Game.Components.Add(new CartePlan(Game, 1f, Vector3.Zero, Vector3.Zero, new Vector3(225, 0, 400), "CartePlan", INTERVALLE_MAJ));

            Client = new TcpClient();
            Client.NoDelay = true;
            Client.Connect(IP, PORT);
            readbuffer = new byte[BUFFER_SIZE];
            Client.GetStream().BeginRead(readbuffer, 0, BUFFER_SIZE, StreamReceived, null);


            readStream = new MemoryStream();
            reader = new BinaryReader(readStream);


            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CadetBlue);
            base.Draw(gameTime);
        }


        private void StreamReceived(IAsyncResult ar)
        {
            int bytesRead = 0;

            try
            {
                lock (Client.GetStream())
                {
                    Client.GetStream().EndRead(ar);
                }
            }
            catch(Exception ex )
            {
                MessageBox.Show(ex.Message);
            }
            if(bytesRead == 0)
            {
                Client.Close();
                return;
            }
            byte[] data = new byte[bytesRead];
            for (int i = 0; i < bytesRead; i++)
            {
                data[i] = readbuffer[i];
            }

            AnalyseData(data);

            Client.GetStream().BeginRead(readbuffer, 0, BUFFER_SIZE, StreamReceived, null);

        }

        private void AnalyseData(byte[] data)
        {
            readStream.SetLength(0);
            readStream.Position = 0;

            readStream.Write(data, 0, data.Length);
            readStream.Position = 0;

            Protocoles p;

            try
            {
                p = (Protocoles)readStream.ReadByte();

                if(p == Protocoles.Connected)
                {
                    MessageBox.Show("Un joueur a rejoins la partie!");
                }
                if (p == Protocoles.Disconnected)
                {
                    MessageBox.Show("Un joueur a quitté la partie!");
                }

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
