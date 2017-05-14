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

        MemoryStream LectureFlux, ÉcritureFlux;

        BinaryReader Lecteur;
        BinaryWriter Rédacteur;

        TcpClient Client;
        const int PORT = 5011;
        string IP { get; set; }
        const int GRANDEUR_TAMPON = 2048;
        private byte[] LectureTampon;

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
            LectureTampon = new byte[GRANDEUR_TAMPON];
            Client.GetStream().BeginRead(LectureTampon, 0, GRANDEUR_TAMPON, RéceptionDuFlux, null);


            LectureFlux = new MemoryStream();
            ÉcritureFlux = new MemoryStream();

            Lecteur = new BinaryReader(LectureFlux);
            Rédacteur = new BinaryWriter(ÉcritureFlux);

        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        private void RéceptionDuFlux(IAsyncResult ar)
        {
            int bytesLus = 0;

            try
            {
                lock (Client.GetStream())
                {
                    bytesLus = Client.GetStream().EndRead(ar);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            if (bytesLus == 0)
            {
                Client.Close();
                return;
            }

            byte[] data = new byte[bytesLus];

            for (int i = 0; i < bytesLus; i++)
                data[i] = LectureTampon[i];

            ProcessData(data);

            Client.GetStream().BeginRead(LectureTampon, 0, GRANDEUR_TAMPON, RéceptionDuFlux, null);
        }

        private void ProcessData(byte[] data)
        {
            LectureFlux.SetLength(0);
            LectureFlux.Position = 0;

            LectureFlux.Write(data, 0, data.Length);
            LectureFlux.Position = 0;

            Protocoles p;

            try
            {
                p = (Protocoles)Lecteur.ReadByte();

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
                        float m11 = Lecteur.ReadSingle();
                        float m12 = Lecteur.ReadSingle();
                        float m13 = Lecteur.ReadSingle();
                        float m14 = Lecteur.ReadSingle();
                        float m21 = Lecteur.ReadSingle();
                        float m22 = Lecteur.ReadSingle();
                        float m23 = Lecteur.ReadSingle();
                        float m24 = Lecteur.ReadSingle();
                        float m31 = Lecteur.ReadSingle();
                        float m32 = Lecteur.ReadSingle();
                        float m33 = Lecteur.ReadSingle();
                        float m34 = Lecteur.ReadSingle();
                        float m41 = Lecteur.ReadSingle();
                        float m42 = Lecteur.ReadSingle();
                        float m43 = Lecteur.ReadSingle();
                        float m44 = Lecteur.ReadSingle();
                        entitéEnnemie.Monde = new Matrix(m11, m12, m13, m14, m21, m22, m23, m24, m31, m32, m33, m34, m41, m42, m43, m44);
                    }
                }

                else if (p == Protocoles.MinionMovement)
                {
                    int numPéon = Lecteur.ReadInt32();
                    List<EntitéPéonEnnemie> péons = Game.Components.OfType<EntitéPéonEnnemie>().ToList();
                    foreach (EntitéPéonEnnemie péon in péons)
                    {
                        if (péon.NumPéon == numPéon)
                        {
                            float px = Lecteur.ReadSingle();
                            float py = Lecteur.ReadSingle();
                            float pz = Lecteur.ReadSingle();
                            Vector3 positionEnnemie = new Vector3(px, py, pz);
                            péon.GérerDéplacement(positionEnnemie);
                        }
                    }
                }
                else if (p == Protocoles.StartGame)
                {
                    bool valeur = Lecteur.ReadBoolean();
                    ((Game)Game).EnJeu = valeur;
                }

                else if (p == Protocoles.BasicAttaque)
                {
                    Entité theEntité = null;

                    GestionnaireJeu game = Game.Components.First(x => x is GestionnaireJeu) as GestionnaireJeu;

                    float px = Lecteur.ReadSingle();
                    float py = Lecteur.ReadSingle();
                    float pz = Lecteur.ReadSingle();
                    int force = Lecteur.ReadInt32();
                    int précision = Lecteur.ReadInt32();
                    int typeEnemmie = Lecteur.ReadInt32();
                    int numEnnemie = Lecteur.ReadInt32();
                    int dégat = Lecteur.ReadInt32();

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
                    int typeEnemmie = Lecteur.ReadInt32();
                    int numEnnemie = Lecteur.ReadInt32();

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
                    int pdv = Lecteur.ReadInt32();
                    List<EntitéEnnemie> entitésEnnemies = Game.Components.OfType<EntitéEnnemie>().ToList();
                    foreach (EntitéEnnemie entitéEnnemie in entitésEnnemies)
                    {
                        entitéEnnemie.PointDeVie = pdv;
                    }
                }
                else if (p == Protocoles.WAttack)
                {
                    GestionnaireJeu game = Game.Components.First(x => x is GestionnaireJeu) as GestionnaireJeu;
                    float px = Lecteur.ReadSingle();
                    float py = Lecteur.ReadSingle();
                    float pz = Lecteur.ReadSingle();
                    float dx = Lecteur.ReadSingle();
                    float dy = Lecteur.ReadSingle();
                    float dz = Lecteur.ReadSingle();
                    int force = Lecteur.ReadInt32();
                    int précision = Lecteur.ReadInt32();
                    int dégat = Lecteur.ReadInt32();
                    ProjectileAttaqueW projectile = new ProjectileAttaqueW(Game, "bomb", game.ÉCHELLE_PROJECTILE_W, game.RotationInitialeProjectielADB, new Vector3(px, py, pz), game.DirectionInitialeProjectileADB, new Vector3(dx, dy, dz), force, précision,dégat,game.INTERVALLEMAJ, 2);
                    Game.Components.Add(projectile);
                }
                
            }
            catch (Exception ex)
            {
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
                //TODO
            }
        }
        public void StartGame()
        {
            ÉcritureFlux.Position = 0;
            Rédacteur.Write((Byte)Protocoles.StartGame);
            Rédacteur.Write(true);
            SendData(GetDataFromMemoryStream(ÉcritureFlux));
        }
        //public void EnvoyerDestination(Vector3 destination)
        //{
        //    writeStream.Position = 0;
        //    writer.Write((Byte)Protocoles.PlayerMovement);
        //    //Envoi de la position
        //    writer.Write(destination.X);
        //    writer.Write(destination.Y);
        //    writer.Write(destination.Z);
        //    SendData(GetDataFromMemoryStream(writeStream));
        //}
        public void EnvoyerPositionPéon(Vector3 position, int numPéon)
        {
            ÉcritureFlux.Position = 0;
            Rédacteur.Write((Byte)Protocoles.MinionMovement);
            //Envoi du numéro de péon
            Rédacteur.Write(numPéon);
            //Envoi de la position du péon
            Rédacteur.Write(position.X);
            Rédacteur.Write(position.Y);
            Rédacteur.Write(position.Z);

            SendData(GetDataFromMemoryStream(ÉcritureFlux));
        }
        public void EnvoyerAttaque(Vector3 position, int force, int précision, int typeEnnemie, int numEnnemie, int dégat)
        {
            ÉcritureFlux.Position = 0;
            Rédacteur.Write((Byte)Protocoles.BasicAttaque);
            //Envoi position joueurA
            Rédacteur.Write(position.X);
            Rédacteur.Write(position.Y);
            Rédacteur.Write(position.Z);
            //Envoi de la force 
            Rédacteur.Write(force);
            //Envoi de la précision 
            Rédacteur.Write(précision);
            //Envoi du type de l'ennemie
            Rédacteur.Write(typeEnnemie);
            //Envoi du numéro de l'ennemie
            Rédacteur.Write(numEnnemie);
            //Envoie du dégat
            Rédacteur.Write(dégat);

            SendData(GetDataFromMemoryStream(ÉcritureFlux));
        }
        public void EnvoyerEnnemiMort(int typeEnnemie, int numEnnemie)
        {
            ÉcritureFlux.Position = 0;
            Rédacteur.Write((Byte)Protocoles.ValidationDeadEnnemi);
            //Envoi du type de l'ennemie
            Rédacteur.Write(typeEnnemie);
            //Envoi du numéro de l'ennemie
            Rédacteur.Write(numEnnemie);
            SendData(GetDataFromMemoryStream(ÉcritureFlux));
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
            ÉcritureFlux.Position = 0;
            Rédacteur.Write((Byte)Protocoles.PlayerMovement);
            Rédacteur.Write(m11);
            Rédacteur.Write(m12);
            Rédacteur.Write(m13);
            Rédacteur.Write(m14);
            Rédacteur.Write(m21);
            Rédacteur.Write(m22);
            Rédacteur.Write(m23);
            Rédacteur.Write(m24);
            Rédacteur.Write(m31);
            Rédacteur.Write(m32);
            Rédacteur.Write(m33);
            Rédacteur.Write(m34);
            Rédacteur.Write(m41);
            Rédacteur.Write(m42);
            Rédacteur.Write(m43);
            Rédacteur.Write(m44);
            SendData(GetDataFromMemoryStream(ÉcritureFlux));
        }
        public void EnvoyerGainDeVie(int PointDeVie)
        {
            ÉcritureFlux.Position = 0;
            Rédacteur.Write((Byte)Protocoles.HealthChange);
            Rédacteur.Write(PointDeVie);
            SendData(GetDataFromMemoryStream(ÉcritureFlux));
        }
        public void EnvoyerAttaqueW(Vector3 position, Vector3 direction, int force, int précision, int dégat)
        {
            ÉcritureFlux.Position = 0;
            Rédacteur.Write((Byte)Protocoles.WAttack);

            //Envoi position joueurA
            Rédacteur.Write(position.X);
            Rédacteur.Write(position.Y);
            Rédacteur.Write(position.Z);
            //Envoi direction attaque
            Rédacteur.Write(direction.X);
            Rédacteur.Write(direction.Y);
            Rédacteur.Write(direction.Z);
            //Envoi de la force 
            Rédacteur.Write(force);
            //Envoi de la précision 
            Rédacteur.Write(précision);
            //Envoi du dégat
            Rédacteur.Write(dégat);
            SendData(GetDataFromMemoryStream(ÉcritureFlux));
        }
    }
}
