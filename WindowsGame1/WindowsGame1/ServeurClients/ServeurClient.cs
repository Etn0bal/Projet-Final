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
    public class ServeurClient : Microsoft.Xna.Framework.GameComponent
    {

        MemoryStream readStream, writeStream;

        BinaryReader reader;
        BinaryWriter writer;

        TcpClient Client;
        const int PORT = 5011;
        string IP { get; set; }
        const int BUFFER_SIZE = 2048;
        private byte[] readbuffer;

        public ServeurClient(Game game, string iP)
            : base(game)
        {
            IP = iP;
            InializeClient();
        }

        private void InializeClient()
        {
            Client = new TcpClient();
            Client.NoDelay = true;
            Client.Connect(IP, PORT);
            readbuffer = new byte[BUFFER_SIZE];
            Client.GetStream().BeginRead(readbuffer, 0, BUFFER_SIZE, StreamReceived, null);


            readStream = new MemoryStream();
            writeStream = new MemoryStream();

            reader = new BinaryReader(readStream);
            writer = new BinaryWriter(writeStream);

        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
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

            ProcessData(data);

            Client.GetStream().BeginRead(readbuffer, 0, BUFFER_SIZE, StreamReceived, null);
        }

        private void ProcessData(byte[] data)
        {
            readStream.SetLength(0);
            readStream.Position = 0;

            readStream.Write(data, 0, data.Length);
            readStream.Position = 0;

            Protocoles p;

            try
            {
                p = (Protocoles)reader.ReadByte();

                if (p == Protocoles.Connected)
                {

                }
                else if (p == Protocoles.Disconnected)
                {
                    ((Game1)Game).ChangerD�tat(0);
                }


                else if (p == Protocoles.PlayerMovement)
                {
                    foreach (Entit�Ennemie Entit�eEnnemie in Game.Components.Where(x => x is Entit�Ennemie))
                    {
                        float px = reader.ReadSingle();
                        float py = reader.ReadSingle();
                        float pz = reader.ReadSingle();
                        Vector3 positionEnnemie = new Vector3(px, py, pz);

                        Entit�eEnnemie.D�placerEnnemie(positionEnnemie);
                    }
                }

                else if (p == Protocoles.MinionMovement)
                {
                    int numP�on = reader.ReadInt32();
                    foreach (Entit�P�onEnnemie p�on in Game.Components.Where(x => x is Entit�P�onEnnemie))
                    {
                        if (p�on.NumP�on == numP�on)
                        {
                            float px = reader.ReadSingle();
                            float py = reader.ReadSingle();
                            float pz = reader.ReadSingle();
                            Vector3 positionEnnemie = new Vector3(px, py, pz);
                            p�on.G�rerD�placement(positionEnnemie);
                        }
                    }
                }
                else if (p == Protocoles.StartGame)
                {
                    bool valeur = reader.ReadBoolean();
                    ((Game1)Game).EnJeu = valeur;
                }

                else if (p == Protocoles.BasicAttaque)
                {
                    Entit� theEntit� = null;
                    foreach (TheGame game in Game.Components.Where(x => x is TheGame))
                    {

                        float px = reader.ReadSingle();
                        float py = reader.ReadSingle();
                        float pz = reader.ReadSingle();
                        int force = reader.ReadInt32();
                        int pr�cision = reader.ReadInt32();
                        int typeEnemmie = reader.ReadInt32();
                        int numEnnemie = reader.ReadInt32();
                        int d�gat = reader.ReadInt32();

                        if (typeEnemmie == 1)
                        {
                            foreach (Entit�P�onAlli�e entit� in Game.Components.Where(x => x is Entit�P�onAlli�e))
                            {
                                if (entit�.NumP�on == numEnnemie)
                                {
                                    theEntit� = entit�;
                                }
                            }
                        }
                        else if (typeEnemmie == 2)
                        {
                            foreach (Entit�TourAlli�e entit� in Game.Components.Where(x => x is Entit�TourAlli�e))
                            {
                                if (entit�.NumTour == numEnnemie)
                                {
                                    theEntit� = entit�;
                                }
                            }
                        }
                        else
                        {
                            foreach (Entit�Joueur entit� in Game.Components.Where(x => x is Entit�Joueur))
                            {
                                theEntit� = entit�;

                            }
                            if (theEntit� != null)
                            {
                                ProjectileAttaqueDeBase projectile = new ProjectileAttaqueDeBase(Game, "rocket", game.�CHELLE_PROJECTILE_ATTAQUE_DE_BASE, game.RotationInitialeProjectielADB, new Vector3(px, py, pz), game.DirectionInitialeProjectileADB, force, pr�cision, theEntit�, d�gat, game.INTERVALLEMAJ);
                                Game.Components.Add(projectile);
                            }
                        }
                    }
                    
                }

                else if (p == Protocoles.ValidationDeadEnnemi)
                {
                    int typeEnemmie = reader.ReadInt32();
                    int numEnnemie = reader.ReadInt32();

                    if (typeEnemmie == 1)
                    {
                        foreach (Entit�P�onAlli�e entit� in Game.Components.Where(x => x is Entit�P�onAlli�e))
                        {
                            if (entit�.NumP�on == numEnnemie)
                            {
                                entit�.PointDeVie = 0;
                            }
                        }
                    }
                    else if (typeEnemmie == 2)
                    {
                        foreach (Entit�TourAlli�e entit� in Game.Components.Where(x => x is Entit�TourAlli�e))
                        {
                            if (entit�.NumTour == numEnnemie)
                            {
                                entit�.PointDeVie = 0;
                            }
                        }
                    }
                    else
                    {
                        foreach (Entit�Joueur entit� in Game.Components.Where(x => x is Entit�Joueur))
                        {
                            entit�.PointDeVie = 0;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private byte[] GetDataFromMemoryStream(MemoryStream ms)
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
            catch (Exception)
            {

            }
        }
        public void StartGame()
        {
            writeStream.Position = 0;
            writer.Write((Byte)Protocoles.StartGame);
            writer.Write(true);
            SendData(GetDataFromMemoryStream(writeStream));
        }
        public void EnvoyerDestination(Vector3 destination)
        {
            writeStream.Position = 0;
            writer.Write((Byte)Protocoles.PlayerMovement);
            //Envoi de la position
            writer.Write(destination.X);
            writer.Write(destination.Y);
            writer.Write(destination.Z);
            SendData(GetDataFromMemoryStream(writeStream));
        }
        public void EnvoyerPositionP�on(Vector3 position, int numP�on)
        {
            writeStream.Position = 0;
            writer.Write((Byte)Protocoles.MinionMovement);
            //Envoi du num�ro de p�on
            writer.Write(numP�on);
            //Envoi de la position du p�on
            writer.Write(position.X);
            writer.Write(position.Y);
            writer.Write(position.Z);

            SendData(GetDataFromMemoryStream(writeStream));
        }
        public void EnvoyerAttaque(Vector3 position, int force, int pr�cision, int typeEnnemie, int numEnnemie, int d�gat)
        {
            writeStream.Position = 0;
            writer.Write((Byte)Protocoles.BasicAttaque);
            //Envoi position joueurA
            writer.Write(position.X);
            writer.Write(position.Y);
            writer.Write(position.Z);
            //Envoi de la force 
            writer.Write(force);
            //Envoi de la pr�cision 
            writer.Write(pr�cision);
            //Envoi du type de l'ennemie
            writer.Write(typeEnnemie);
            //Envoi du num�ro de l'ennemie
            writer.Write(numEnnemie);
            //Envoie du d�gat
            writer.Write(d�gat);

            SendData(GetDataFromMemoryStream(writeStream));
        }
        public void EnvoyerEnnemiMort(int typeEnnemie, int numEnnemie)
        {
            writeStream.Position = 0;
            writer.Write((Byte)Protocoles.ValidationDeadEnnemi);
            //Envoi du type de l'ennemie
            writer.Write(typeEnnemie);
            //Envoi du num�ro de l'ennemie
            writer.Write(numEnnemie);
            SendData(GetDataFromMemoryStream(writeStream));
        }
    }
}
