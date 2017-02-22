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
    public enum Stade { Menu,Inloading,InGame,EndOfTheGame}

    public class TheGame : Microsoft.Xna.Framework.DrawableGameComponent
    {

        Caméra CaméraJeu { get; set; }
        const float INTERVALLE_MAJ = 1f / 60f;

        RessourcesManager<Texture2D> GestionnaireDeTexture { get; set; }
        GraphicsDeviceManager graphics { get; set; }
        SpriteBatch GestionSprites { get; set; }
        InputManager GestionInput { get; set; }

        MemoryStream readStream, writeStream;

        BinaryReader reader;
        BinaryWriter writer;

        TcpClient Client;
        const int PORT = 5011;
        string IP = "172.17.106.102";
        const int BUFFER_SIZE = 2048;
        private byte[] readbuffer;


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
            CaméraJeu = new CaméraTypéMoba(Game, new Vector3(-85, 30, 115), new Vector3(0, -1, -1), Vector3.Up, INTERVALLE_MAJ);

            Game.Services.AddService(typeof(Caméra), CaméraJeu);
            Game.Components.Add(CaméraJeu);
            Game.Components.Add(new CartePlan(Game, 1f, Vector3.Zero, Vector3.Zero, new Vector3(225, 0, 400), "Carte Plan4", INTERVALLE_MAJ));
            Game.Components.Add(new Murs(Game, 1f, Vector3.Zero, Vector3.Zero, new Vector3(225, 0, 400), "Carte planMur", INTERVALLE_MAJ));

            Client = new TcpClient();
            Client.NoDelay = true;
            Client.Connect(IP, PORT);
            readbuffer = new byte[BUFFER_SIZE];
            Client.GetStream().BeginRead(readbuffer, 0, BUFFER_SIZE, StreamReceived, null);


            readStream = new MemoryStream();
            writeStream = new MemoryStream();

            reader = new BinaryReader(readStream);
            writer = new BinaryWriter(writeStream);


            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }


        private void StreamReceived(IAsyncResult ar)
        {
            int bytesRead = 0;

            try
            {
                lock (Client.GetStream())
                {
                    bytesRead = Client.GetStream().EndRead(ar);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            if (bytesRead == 0)
            {
                Client.Close();
                return;
            }

            byte[] data = new byte[bytesRead];

            for (int i = 0; i < bytesRead; i++)
                data[i] = readbuffer[i];

            //ProcessData(data);

            Client.GetStream().BeginRead(readbuffer, 0, BUFFER_SIZE, StreamReceived, null);
        }

        //private void ProcessData(byte[] data)
        //{
        //    readStream.SetLength(0);
        //    readStream.Position = 0;

        //    readStream.Write(data, 0, data.Length);
        //    readStream.Position = 0;

        //    Protocoles p;

        //    try
        //    {
        //        p = (Protocoles)reader.ReadByte();

        //        if (p == Protocoles.Connected)
        //        {
        //        }
        //        else if (p == Protocoles.Disconnected)
        //        {
        //        }

        //        else if (p == Protocoles.PlayerMoved)
        //        {
        //        }
        //        else if (p == Protocoles)
        //        {
        //        }
        //        else if (p == Protocoles)
        //        {
        //        }
        //        else if (p == Protocoles)
        //        {
        //        }
        //        else if (p == Protocoles.Validation)
        //        {
        //            //    }
        //        }
        //        catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //}


        private byte[]GetDataFromMemoryStream(MemoryStream ms)
        {
            byte[] result;
    
            lock (ms)
            {
                int bytesWritten = (int)ms.Position;
                result = new byte[bytesWritten];

                ms.Position = 0;
                ms.Read(result, 0, bytesWritten);
            }

            return result;
        }

        /// <summary>
        /// Code to actually send the data to the client
        /// </summary>
        /// <param name="b">Data to send</param>
        public void SendData(byte[] b)
        {
           
            try
            {
                lock (Client.GetStream())
                {
                    Client.GetStream().BeginWrite(b, 0, b.Length, null, null);
                }
            }
            catch (Exception e)
            {

            }
        }
    }
}
