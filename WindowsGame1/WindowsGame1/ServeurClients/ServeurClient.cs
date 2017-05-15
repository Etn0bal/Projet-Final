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

        public ServeurClient(Microsoft.Xna.Framework.Game game, string iP)
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

                }


                else if (p == Protocoles.PlayerMovement)
                {
                    List<EntitéEnnemie> entitésEnnemies = Game.Components.OfType<EntitéEnnemie>().ToList();
                    foreach (EntitéEnnemie entitéEnnemie in entitésEnnemies)
                    {
                        float m11 = reader.ReadSingle();
                        float m12 = reader.ReadSingle();
                        float m13 = reader.ReadSingle();
                        float m14 = reader.ReadSingle();
                        float m21 = reader.ReadSingle();
                        float m22 = reader.ReadSingle();
                        float m23 = reader.ReadSingle();
                        float m24 = reader.ReadSingle();
                        float m31 = reader.ReadSingle();
                        float m32 = reader.ReadSingle();
                        float m33 = reader.ReadSingle();
                        float m34 = reader.ReadSingle();
                        float m41 = reader.ReadSingle();
                        float m42 = reader.ReadSingle();
                        float m43 = reader.ReadSingle();
                        float m44 = reader.ReadSingle();
                        entitéEnnemie.Monde = new Matrix(m11, m12, m13, m14, m21, m22, m23, m24, m31, m32, m33, m34, m41, m42, m43, m44);
                    }
                }

                else if (p == Protocoles.MinionMovement)
                {
                    int numPéon = reader.ReadInt32();
                    List<EntitéPéonEnnemie> péons = Game.Components.OfType<EntitéPéonEnnemie>().ToList();
                    foreach (EntitéPéonEnnemie péon in péons)
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
                    ((Game)Game).EnJeu = valeur;
                }

                else if (p == Protocoles.BasicAttaque)
                {
                    Entité theEntité = null;

                    GestionnaireJeu game = Game.Components.First(x => x is GestionnaireJeu) as GestionnaireJeu;

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
                        List<EntitéPéonAlliée> entités = Game.Components.OfType<EntitéPéonAlliée>().ToList();
                        foreach (EntitéPéonAlliée entité in entités)
                        {
                            if (entité.NumPéon == numEnnemie)
                            {
                                theEntité = entité;
                            }
                        }
                    }
                    else if (typeEnemmie == 2)
                    {
                        List<EntitéTourAlliée> entités = Game.Components.OfType<EntitéTourAlliée>().ToList();
                        foreach (EntitéTourAlliée entité in entités)
                        {
                            if (entité.NumTour == numEnnemie)
                            {
                                theEntité = entité;
                            }
                        }
                    }
                    else
                    {
                        List<EntitéJoueur> entités = Game.Components.OfType<EntitéJoueur>().ToList();
                        foreach (EntitéJoueur entité in entités)
                        {
                            theEntité = entité;

                        }
                    }
                    if (theEntité != null)
                    {
                        ProjectileAttaqueDeBase projectile = new ProjectileAttaqueDeBase(Game, "rocket", game.ÉCHELLE_PROJECTILE_ATTAQUE_DE_BASE, game.RotationInitialeProjectielADB, new Vector3(px, py, pz) + new Vector3(0, 5, 0), game.DirectionInitialeProjectileADB, force, précision, theEntité, dégat, game.INTERVALLEMAJ);
                        Game.Components.Add(projectile);
                    }
                }



                else if (p == Protocoles.ValidationDeadEnnemi)
                {
                    int typeEnemmie = reader.ReadInt32();
                    int numEnnemie = reader.ReadInt32();

                    if (typeEnemmie == 1)
                    {
                        List<EntitéPéonAlliée> entités = Game.Components.OfType<EntitéPéonAlliée>().ToList();
                        foreach (EntitéPéonAlliée entité in entités)
                        {
                            if (entité.NumPéon == numEnnemie)
                            {
                                entité.PointDeVie = 0;
                            }
                        }
                    }
                    else if (typeEnemmie == 2)
                    {
                        List<EntitéTourAlliée> entités = Game.Components.OfType<EntitéTourAlliée>().ToList();
                        foreach (EntitéTourAlliée entité in entités)
                        {
                            if (entité.NumTour == numEnnemie)
                            {
                                entité.PointDeVie = 0;
                            }
                        }
                    }
                    else if (typeEnemmie == 0)
                    {
                        List<EntitéJoueur> entités = Game.Components.OfType<EntitéJoueur>().ToList();
                        foreach (EntitéJoueur entité in entités)
                        {
                            entité.PointDeVie = 0;
                        }
                    }

                }
                else if (p == Protocoles.HealthChange)
                {
                    int pdv = reader.ReadInt32();
                    List<EntitéEnnemie> entitésEnnemies = Game.Components.OfType<EntitéEnnemie>().ToList();
                    foreach (EntitéEnnemie entitéEnnemie in entitésEnnemies)
                    {
                        entitéEnnemie.PointDeVie = pdv;
                    }
                }
                else if (p == Protocoles.WAttack)
                {
                    GestionnaireJeu game = Game.Components.First(x => x is GestionnaireJeu) as GestionnaireJeu;
                    float px = reader.ReadSingle();
                    float py = reader.ReadSingle();
                    float pz = reader.ReadSingle();
                    float dx = reader.ReadSingle();
                    float dy = reader.ReadSingle();
                    float dz = reader.ReadSingle();
                    int force = reader.ReadInt32();
                    int précision = reader.ReadInt32();
                    int dégat = reader.ReadInt32();
                    ProjectileAttaqueW projectile = new ProjectileAttaqueW(Game, "bomb", game.ÉCHELLE_PROJECTILE_W, game.RotationInitialeProjectielADB, new Vector3(px, py, pz), game.DirectionInitialeProjectileADB, new Vector3(dx, dy, dz), force, précision,dégat,game.INTERVALLEMAJ, 2);
                    Game.Components.Add(projectile);
                }
                
            }
            catch (Exception)
            {   }
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
                //TODO
            }
        }
        public void StartGame()
        {
            writeStream.Position = 0;
            writer.Write((Byte)Protocoles.StartGame);
            writer.Write(true);
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
        public void EnvoyerMatrice(Matrix matrice)
        {
            float m11 = matrice.M11;
            float m12 = matrice.M12;
            float m13 = matrice.M13;
            float m14 = matrice.M14;
            float m21 = matrice.M21;
            float m22 = matrice.M22;
            float m23 = matrice.M23;
            float m24 = matrice.M24;
            float m31 = matrice.M31;
            float m32 = matrice.M32;
            float m33 = matrice.M33;
            float m34 = matrice.M34;
            float m41 = matrice.M41;
            float m42 = matrice.M42;
            float m43 = matrice.M43;
            float m44 = matrice.M44;
            writeStream.Position = 0;
            writer.Write((Byte)Protocoles.PlayerMovement);
            writer.Write(m11);
            writer.Write(m12);
            writer.Write(m13);
            writer.Write(m14);
            writer.Write(m21);
            writer.Write(m22);
            writer.Write(m23);
            writer.Write(m24);
            writer.Write(m31);
            writer.Write(m32);
            writer.Write(m33);
            writer.Write(m34);
            writer.Write(m41);
            writer.Write(m42);
            writer.Write(m43);
            writer.Write(m44);
            SendData(GetDataFromMemoryStream(writeStream));
        }
        public void EnvoyerGainDeVie(int PointDeVie)
        {
            writeStream.Position = 0;
            writer.Write((Byte)Protocoles.HealthChange);
            writer.Write(PointDeVie);
            SendData(GetDataFromMemoryStream(writeStream));
        }
        public void EnvoyerAttaqueW(Vector3 position, Vector3 direction, int force, int précision, int dégat)
        {
            writeStream.Position = 0;
            writer.Write((Byte)Protocoles.WAttack);

            //Envoi position joueurA
            writer.Write(position.X);
            writer.Write(position.Y);
            writer.Write(position.Z);
            //Envoi direction attaque
            writer.Write(direction.X);
            writer.Write(direction.Y);
            writer.Write(direction.Z);
            //Envoi de la force 
            writer.Write(force);
            //Envoi de la précision 
            writer.Write(précision);
            //Envoi du dégat
            writer.Write(dégat);
            SendData(GetDataFromMemoryStream(writeStream));
        }
    }
}
