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
                    ((Game1)Game).ChangerDÉtat(0);
                }


                else if (p == Protocoles.PlayerMovement)
                {
                    foreach (EntitéEnnemie EntitéeEnnemie in Game.Components.Where(x => x is EntitéEnnemie))
                    {
                        float px = reader.ReadSingle();
                        float py = reader.ReadSingle();
                        float pz = reader.ReadSingle();
                        Vector3 positionEnnemie = new Vector3(px, py, pz);

                        EntitéeEnnemie.DéplacerEnnemie(positionEnnemie);
                    }
                }

                else if (p == Protocoles.MinionMovement)
                {
                    int numPéon = reader.ReadInt32();
                    foreach (EntitéPéonEnnemie péon in Game.Components.Where(x => x is EntitéPéonEnnemie))
                    {
                        if (péon.NumPéon == numPéon)
                        {
                            float px = reader.ReadSingle();
                            float py = reader.ReadSingle();
                            float pz = reader.ReadSingle();
                            Vector3 positionEnnemie = new Vector3(px, py, pz);
                            péon.GérerDéplacement(positionEnnemie);
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
                    Entité theEntité = null;
                    foreach (TheGame game in Game.Components.Where(x => x is TheGame))
                    {

                        float px = reader.ReadSingle();
                        float py = reader.ReadSingle();
                        float pz = reader.ReadSingle();
                        int force = reader.ReadInt32();
                        int précision = reader.ReadInt32();
                        int typeEnemmie = reader.ReadInt32();
                        int numEnnemie = reader.ReadInt32();
                        int dégat = reader.ReadInt32();

                        if (typeEnemmie == 1)
                        {
                            foreach (EntitéPéonAlliée entité in Game.Components.Where(x => x is EntitéPéonAlliée))
                            {
                                if (entité.NumPéon == numEnnemie)
                                {
                                    theEntité = entité;
                                }
                            }
                        }
                        else if (typeEnemmie == 2)
                        {
                            foreach (EntitéTourAlliée entité in Game.Components.Where(x => x is EntitéTourAlliée))
                            {
                                if (entité.NumTour == numEnnemie)
                                {
                                    theEntité = entité;
                                }
                            }
                        }
                        else
                        {
                            foreach (EntitéJoueur entité in Game.Components.Where(x => x is EntitéJoueur))
                            {
                                theEntité = entité;

                            }
                            if (theEntité != null)
                            {
                                ProjectileAttaqueDeBase projectile = new ProjectileAttaqueDeBase(Game, "rocket", game.ÉCHELLE_PROJECTILE_ATTAQUE_DE_BASE, game.RotationInitialeProjectielADB, new Vector3(px, py, pz), game.DirectionInitialeProjectileADB, force, précision, theEntité, dégat, game.INTERVALLEMAJ);
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
                        foreach (EntitéPéonAlliée entité in Game.Components.Where(x => x is EntitéPéonAlliée))
                        {
                            if (entité.NumPéon == numEnnemie)
                            {
                                entité.PointDeVie = 0;
                            }
                        }
                    }
                    else if (typeEnemmie == 2)
                    {
                        foreach (EntitéTourAlliée entité in Game.Components.Where(x => x is EntitéTourAlliée))
                        {
                            if (entité.NumTour == numEnnemie)
                            {
                                entité.PointDeVie = 0;
                            }
                        }
                    }
                    else
                    {
                        foreach (EntitéJoueur entité in Game.Components.Where(x => x is EntitéJoueur))
                        {
                            entité.PointDeVie = 0;
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
        public void EnvoyerPositionPéon(Vector3 position, int numPéon)
        {
            writeStream.Position = 0;
            writer.Write((Byte)Protocoles.MinionMovement);
            //Envoi du numéro de péon
            writer.Write(numPéon);
            //Envoi de la position du péon
            writer.Write(position.X);
            writer.Write(position.Y);
            writer.Write(position.Z);

            SendData(GetDataFromMemoryStream(writeStream));
        }
        public void EnvoyerAttaque(Vector3 position, int force, int précision, int typeEnnemie, int numEnnemie, int dégat)
        {
            writeStream.Position = 0;
            writer.Write((Byte)Protocoles.BasicAttaque);
            //Envoi position joueurA
            writer.Write(position.X);
            writer.Write(position.Y);
            writer.Write(position.Z);
            //Envoi de la force 
            writer.Write(force);
            //Envoi de la précision 
            writer.Write(précision);
            //Envoi du type de l'ennemie
            writer.Write(typeEnnemie);
            //Envoi du numéro de l'ennemie
            writer.Write(numEnnemie);
            //Envoie du dégat
            writer.Write(dégat);

            SendData(GetDataFromMemoryStream(writeStream));
        }
        public void EnvoyerEnnemiMort(int typeEnnemie, int numEnnemie)
        {
            writeStream.Position = 0;
            writer.Write((Byte)Protocoles.ValidationDeadEnnemi);
            //Envoi du type de l'ennemie
            writer.Write(typeEnnemie);
            //Envoi du numéro de l'ennemie
            writer.Write(numEnnemie);
            SendData(GetDataFromMemoryStream(writeStream));
        }
    }
}
