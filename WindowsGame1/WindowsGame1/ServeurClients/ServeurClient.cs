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

        MemoryStream LectureFlux, �critureFlux;

        BinaryReader Lecteur;
        BinaryWriter R�dacteur;

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
            Client.GetStream().BeginRead(LectureTampon, 0, GRANDEUR_TAMPON, R�ceptionDuFlux, null);


            LectureFlux = new MemoryStream();
            �critureFlux = new MemoryStream();

            Lecteur = new BinaryReader(LectureFlux);
            R�dacteur = new BinaryWriter(�critureFlux);

        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        private void R�ceptionDuFlux(IAsyncResult ar)
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

            Client.GetStream().BeginRead(LectureTampon, 0, GRANDEUR_TAMPON, R�ceptionDuFlux, null);
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
                    List<Entit�Ennemie> entit�sEnnemies = Game.Components.OfType<Entit�Ennemie>().ToList();
                    foreach (Entit�Ennemie entit�Ennemie in entit�sEnnemies)
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
                        entit�Ennemie.Monde = new Matrix(m11, m12, m13, m14, m21, m22, m23, m24, m31, m32, m33, m34, m41, m42, m43, m44);
                    }
                }

                else if (p == Protocoles.MinionMovement)
                {
                    int numP�on = Lecteur.ReadInt32();
                    List<Entit�P�onEnnemie> p�ons = Game.Components.OfType<Entit�P�onEnnemie>().ToList();
                    foreach (Entit�P�onEnnemie p�on in p�ons)
                    {
                        if (p�on.NumP�on == numP�on)
                        {
                            float px = Lecteur.ReadSingle();
                            float py = Lecteur.ReadSingle();
                            float pz = Lecteur.ReadSingle();
                            Vector3 positionEnnemie = new Vector3(px, py, pz);
                            p�on.G�rerD�placement(positionEnnemie);
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
                    Entit� theEntit� = null;

                    GestionnaireJeu game = Game.Components.First(x => x is GestionnaireJeu) as GestionnaireJeu;

                    float px = Lecteur.ReadSingle();
                    float py = Lecteur.ReadSingle();
                    float pz = Lecteur.ReadSingle();
                    int force = Lecteur.ReadInt32();
                    int pr�cision = Lecteur.ReadInt32();
                    int typeEnemmie = Lecteur.ReadInt32();
                    int numEnnemie = Lecteur.ReadInt32();
                    int d�gat = Lecteur.ReadInt32();

                    if (typeEnemmie == 1)
                    {
                        List<Entit�P�onAlli�e> entit�s = Game.Components.OfType<Entit�P�onAlli�e>().ToList();
                        foreach (Entit�P�onAlli�e entit� in entit�s)
                        {
                            if (entit�.NumP�on == numEnnemie)
                            {
                                theEntit� = entit�;
                            }
                        }
                    }
                    else if (typeEnemmie == 2)
                    {
                        List<Entit�TourAlli�e> entit�s = Game.Components.OfType<Entit�TourAlli�e>().ToList();
                        foreach (Entit�TourAlli�e entit� in entit�s)
                        {
                            if (entit�.NumTour == numEnnemie)
                            {
                                theEntit� = entit�;
                            }
                        }
                    }
                    else
                    {
                        List<Entit�Joueur> entit�s = Game.Components.OfType<Entit�Joueur>().ToList();
                        foreach (Entit�Joueur entit� in entit�s)
                        {
                            theEntit� = entit�;

                        }
                    }
                    if (theEntit� != null)
                    {
                        ProjectileAttaqueDeBase projectile = new ProjectileAttaqueDeBase(Game, "rocket", game.�CHELLE_PROJECTILE_ATTAQUE_DE_BASE, game.RotationInitialeProjectielADB, new Vector3(px, py, pz) + new Vector3(0, 5, 0), game.DirectionInitialeProjectileADB, force, pr�cision, theEntit�, d�gat, game.INTERVALLEMAJ);
                        Game.Components.Add(projectile);
                    }
                }



                else if (p == Protocoles.ValidationDeadEnnemi)
                {
                    int typeEnemmie = Lecteur.ReadInt32();
                    int numEnnemie = Lecteur.ReadInt32();

                    if (typeEnemmie == 1)
                    {
                        List<Entit�P�onAlli�e> entit�s = Game.Components.OfType<Entit�P�onAlli�e>().ToList();
                        foreach (Entit�P�onAlli�e entit� in entit�s)
                        {
                            if (entit�.NumP�on == numEnnemie)
                            {
                                entit�.PointDeVie = 0;
                            }
                        }
                    }
                    else if (typeEnemmie == 2)
                    {
                        List<Entit�TourAlli�e> entit�s = Game.Components.OfType<Entit�TourAlli�e>().ToList();
                        foreach (Entit�TourAlli�e entit� in entit�s)
                        {
                            if (entit�.NumTour == numEnnemie)
                            {
                                entit�.PointDeVie = 0;
                            }
                        }
                    }
                    else if (typeEnemmie == 0)
                    {
                        List<Entit�Joueur> entit�s = Game.Components.OfType<Entit�Joueur>().ToList();
                        foreach (Entit�Joueur entit� in entit�s)
                        {
                            entit�.PointDeVie = 0;
                        }
                    }

                }
                else if (p == Protocoles.HealthChange)
                {
                    int pdv = Lecteur.ReadInt32();
                    List<Entit�Ennemie> entit�sEnnemies = Game.Components.OfType<Entit�Ennemie>().ToList();
                    foreach (Entit�Ennemie entit�Ennemie in entit�sEnnemies)
                    {
                        entit�Ennemie.PointDeVie = pdv;
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
                    int pr�cision = Lecteur.ReadInt32();
                    int d�gat = Lecteur.ReadInt32();
                    ProjectileAttaqueW projectile = new ProjectileAttaqueW(Game, "bomb", game.�CHELLE_PROJECTILE_W, game.RotationInitialeProjectielADB, new Vector3(px, py, pz), game.DirectionInitialeProjectileADB, new Vector3(dx, dy, dz), force, pr�cision,d�gat,game.INTERVALLEMAJ, 2);
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
            �critureFlux.Position = 0;
            R�dacteur.Write((Byte)Protocoles.StartGame);
            R�dacteur.Write(true);
            SendData(GetDataFromMemoryStream(�critureFlux));
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
        public void EnvoyerPositionP�on(Vector3 position, int numP�on)
        {
            �critureFlux.Position = 0;
            R�dacteur.Write((Byte)Protocoles.MinionMovement);
            //Envoi du num�ro de p�on
            R�dacteur.Write(numP�on);
            //Envoi de la position du p�on
            R�dacteur.Write(position.X);
            R�dacteur.Write(position.Y);
            R�dacteur.Write(position.Z);

            SendData(GetDataFromMemoryStream(�critureFlux));
        }
        public void EnvoyerAttaque(Vector3 position, int force, int pr�cision, int typeEnnemie, int numEnnemie, int d�gat)
        {
            �critureFlux.Position = 0;
            R�dacteur.Write((Byte)Protocoles.BasicAttaque);
            //Envoi position joueurA
            R�dacteur.Write(position.X);
            R�dacteur.Write(position.Y);
            R�dacteur.Write(position.Z);
            //Envoi de la force 
            R�dacteur.Write(force);
            //Envoi de la pr�cision 
            R�dacteur.Write(pr�cision);
            //Envoi du type de l'ennemie
            R�dacteur.Write(typeEnnemie);
            //Envoi du num�ro de l'ennemie
            R�dacteur.Write(numEnnemie);
            //Envoie du d�gat
            R�dacteur.Write(d�gat);

            SendData(GetDataFromMemoryStream(�critureFlux));
        }
        public void EnvoyerEnnemiMort(int typeEnnemie, int numEnnemie)
        {
            �critureFlux.Position = 0;
            R�dacteur.Write((Byte)Protocoles.ValidationDeadEnnemi);
            //Envoi du type de l'ennemie
            R�dacteur.Write(typeEnnemie);
            //Envoi du num�ro de l'ennemie
            R�dacteur.Write(numEnnemie);
            SendData(GetDataFromMemoryStream(�critureFlux));
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
            �critureFlux.Position = 0;
            R�dacteur.Write((Byte)Protocoles.PlayerMovement);
            R�dacteur.Write(m11);
            R�dacteur.Write(m12);
            R�dacteur.Write(m13);
            R�dacteur.Write(m14);
            R�dacteur.Write(m21);
            R�dacteur.Write(m22);
            R�dacteur.Write(m23);
            R�dacteur.Write(m24);
            R�dacteur.Write(m31);
            R�dacteur.Write(m32);
            R�dacteur.Write(m33);
            R�dacteur.Write(m34);
            R�dacteur.Write(m41);
            R�dacteur.Write(m42);
            R�dacteur.Write(m43);
            R�dacteur.Write(m44);
            SendData(GetDataFromMemoryStream(�critureFlux));
        }
        public void EnvoyerGainDeVie(int PointDeVie)
        {
            �critureFlux.Position = 0;
            R�dacteur.Write((Byte)Protocoles.HealthChange);
            R�dacteur.Write(PointDeVie);
            SendData(GetDataFromMemoryStream(�critureFlux));
        }
        public void EnvoyerAttaqueW(Vector3 position, Vector3 direction, int force, int pr�cision, int d�gat)
        {
            �critureFlux.Position = 0;
            R�dacteur.Write((Byte)Protocoles.WAttack);

            //Envoi position joueurA
            R�dacteur.Write(position.X);
            R�dacteur.Write(position.Y);
            R�dacteur.Write(position.Z);
            //Envoi direction attaque
            R�dacteur.Write(direction.X);
            R�dacteur.Write(direction.Y);
            R�dacteur.Write(direction.Z);
            //Envoi de la force 
            R�dacteur.Write(force);
            //Envoi de la pr�cision 
            R�dacteur.Write(pr�cision);
            //Envoi du d�gat
            R�dacteur.Write(d�gat);
            SendData(GetDataFromMemoryStream(�critureFlux));
        }
    }
}
