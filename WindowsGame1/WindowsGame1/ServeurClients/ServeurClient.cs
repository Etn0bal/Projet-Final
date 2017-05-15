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
    public class ServeurClient : Microsoft.Xna.Framework.GameComponent
    {

        MemoryStream lectureFlux, �critureFlux;

        BinaryReader lecteur;
        BinaryWriter r�dacteur;

        TcpClient Client;
        const int PORT = 5011;
        string IP { get; set; }
        const int GRANDEUR_TAMPON = 2048;
        private byte[] LectureTampon;

        public ServeurClient(Microsoft.Xna.Framework.Game game, string iP)
            : base(game)
        {
            IP = iP;
            InitialiserClient();
        }

        private void InitialiserClient()
        {
            Client = new TcpClient();
            Client.NoDelay = true;
            Client.Connect(IP, PORT);
            LectureTampon = new byte[GRANDEUR_TAMPON];
            Client.GetStream().BeginRead(LectureTampon, 0, GRANDEUR_TAMPON, R�ceptionDuFlux, null);


            lectureFlux = new MemoryStream();
            �critureFlux = new MemoryStream();

            lecteur = new BinaryReader(lectureFlux);
            r�dacteur = new BinaryWriter(�critureFlux);

        }

        private void R�ceptionDuFlux(IAsyncResult resultat)
        {
            int bytesLus = 0;

            try
            {
                lock (Client.GetStream())
                {
                    bytesLus = Client.GetStream().EndRead(resultat);
                }
            }
            catch (Exception) { }

            if (bytesLus == 0)
            {
                Client.Close();
                return;
            }

            byte[] donn�e = new byte[bytesLus];

            for (int i = 0; i < bytesLus; i++)
            {
                donn�e[i] = LectureTampon[i];
            }

            TraitementDonn�es(donn�e);
            Client.GetStream().BeginRead(LectureTampon, 0, GRANDEUR_TAMPON, R�ceptionDuFlux, null);
        }

        private void TraitementDonn�es(byte[] data)
        {
            lectureFlux.SetLength(0);
            lectureFlux.Position = 0;

            lectureFlux.Write(data, 0, data.Length);
            lectureFlux.Position = 0;

            Protocoles p;

            try
            {

                p = (Protocoles)lecteur.ReadByte();
                switch (p)
                {
                    case (Protocoles.PlayerMovement):
                        {
                            List<Entit�Ennemie> entit�sEnnemies = Game.Components.OfType<Entit�Ennemie>().ToList();
                            foreach (Entit�Ennemie entit�Ennemie in entit�sEnnemies)
                            {
                                float m11 = lecteur.ReadSingle();
                                float m12 = lecteur.ReadSingle();
                                float m13 = lecteur.ReadSingle();
                                float m14 = lecteur.ReadSingle();
                                float m21 = lecteur.ReadSingle();
                                float m22 = lecteur.ReadSingle();
                                float m23 = lecteur.ReadSingle();
                                float m24 = lecteur.ReadSingle();
                                float m31 = lecteur.ReadSingle();
                                float m32 = lecteur.ReadSingle();
                                float m33 = lecteur.ReadSingle();
                                float m34 = lecteur.ReadSingle();
                                float m41 = lecteur.ReadSingle();
                                float m42 = lecteur.ReadSingle();
                                float m43 = lecteur.ReadSingle();
                                float m44 = lecteur.ReadSingle();
                                entit�Ennemie.Monde = new Matrix(m11, m12, m13, m14, m21, m22, m23, m24, m31, m32, m33, m34, m41, m42, m43, m44);
                            }
                            break;
                        }
                    case (Protocoles.MinionMovement):
                        {
                            int numP�on = lecteur.ReadInt32();
                            List<Entit�P�onEnnemie> p�ons = Game.Components.OfType<Entit�P�onEnnemie>().ToList();
                            foreach (Entit�P�onEnnemie p�on in p�ons)
                            {
                                if (p�on.NumP�on == numP�on)
                                {
                                    float px = lecteur.ReadSingle();
                                    float py = lecteur.ReadSingle();
                                    float pz = lecteur.ReadSingle();
                                    Vector3 positionEnnemie = new Vector3(px, py, pz);
                                    p�on.G�rerD�placement(positionEnnemie);
                                }
                            }
                            break;
                        }
                    case (Protocoles.StartGame):
                        {
                            bool valeur = lecteur.ReadBoolean();
                            ((Game)Game).EnJeu = valeur;
                            break;
                        }
                    case (Protocoles.BasicAttaque):
                        {
                            Entit� theEntit� = null;

                            GestionnaireJeu game = Game.Components.First(x => x is GestionnaireJeu) as GestionnaireJeu;

                            float px = lecteur.ReadSingle();
                            float py = lecteur.ReadSingle();
                            float pz = lecteur.ReadSingle();
                            int force = lecteur.ReadInt32();
                            int pr�cision = lecteur.ReadInt32();
                            int typeEnemmie = lecteur.ReadInt32();
                            int numEnnemie = lecteur.ReadInt32();
                            int d�gat = lecteur.ReadInt32();

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
                            break;
                        }
                    case (Protocoles.ValidationDeadEnnemi):
                        {
                            int typeEnemmie = lecteur.ReadInt32();
                            int numEnnemie = lecteur.ReadInt32();

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
                            break;
                        }
                    case (Protocoles.HealthChange):
                        {
                            int pdv = lecteur.ReadInt32();
                            List<Entit�Ennemie> entit�sEnnemies = Game.Components.OfType<Entit�Ennemie>().ToList();
                            foreach (Entit�Ennemie entit�Ennemie in entit�sEnnemies)
                            {
                                entit�Ennemie.PointDeVie = pdv;
                            }
                            break;
                        }
                    case (Protocoles.WAttack):
                        {
                            GestionnaireJeu game = Game.Components.First(x => x is GestionnaireJeu) as GestionnaireJeu;
                            float px = lecteur.ReadSingle();
                            float py = lecteur.ReadSingle();
                            float pz = lecteur.ReadSingle();
                            float dx = lecteur.ReadSingle();
                            float dy = lecteur.ReadSingle();
                            float dz = lecteur.ReadSingle();
                            int force = lecteur.ReadInt32();
                            int pr�cision = lecteur.ReadInt32();
                            int d�gat = lecteur.ReadInt32();
                            ProjectileAttaqueW projectile = new ProjectileAttaqueW(Game, "bomb", game.�CHELLE_PROJECTILE_W, game.RotationInitialeProjectielADB, new Vector3(px, py, pz), game.DirectionInitialeProjectileADB, new Vector3(dx, dy, dz), force, pr�cision, d�gat, game.INTERVALLEMAJ, 2);
                            Game.Components.Add(projectile);
                            break;
                        }

                }
            }
            catch (Exception) { }
        }


        private byte[] ObtentionDonn�es(MemoryStream fluxMemoire)
        {
            byte[] resultat;

            lock (fluxMemoire)// On lock pour que l'ex�cution se fasse avant que d'autre donn�e soit capt� et analys�.
            {
                int bytes�crits = (int)fluxMemoire.Position;
                resultat = new byte[bytes�crits];

                fluxMemoire.Position = 0;
                fluxMemoire.Read(resultat, 0, bytes�crits);
            }

            return resultat;
        }

        public void EnvoiDonn�es(byte[] tableauByte)
        {
            try
            {
                lock (Client.GetStream())
                {
                    Client.GetStream().BeginWrite(tableauByte, 0, tableauByte.Length, null, null);
                }
            }
            catch (Exception) { }
        }
        public void PartirPartie()
        {
            �critureFlux.Position = 0;
            r�dacteur.Write((Byte)Protocoles.StartGame);
            r�dacteur.Write(true);
            EnvoiDonn�es(ObtentionDonn�es(�critureFlux));
        }

        public void EnvoyerPositionP�on(Vector3 position, int numP�on)
        {
            �critureFlux.Position = 0;
            r�dacteur.Write((Byte)Protocoles.MinionMovement);
            //Envoi du num�ro de p�on
            r�dacteur.Write(numP�on);
            //Envoi de la position du p�on
            r�dacteur.Write(position.X);
            r�dacteur.Write(position.Y);
            r�dacteur.Write(position.Z);

            EnvoiDonn�es(ObtentionDonn�es(�critureFlux));
        }
        public void EnvoyerAttaque(Vector3 position, int force, int pr�cision, int typeEnnemie, int numEnnemie, int d�gat)
        {
            �critureFlux.Position = 0;
            r�dacteur.Write((Byte)Protocoles.BasicAttaque);
            //Envoi position joueurA
            r�dacteur.Write(position.X);
            r�dacteur.Write(position.Y);
            r�dacteur.Write(position.Z);
            //Envoi de la force 
            r�dacteur.Write(force);
            //Envoi de la pr�cision 
            r�dacteur.Write(pr�cision);
            //Envoi du type de l'ennemie
            r�dacteur.Write(typeEnnemie);
            //Envoi du num�ro de l'ennemie
            r�dacteur.Write(numEnnemie);
            //Envoie du d�gat
            r�dacteur.Write(d�gat);

            EnvoiDonn�es(ObtentionDonn�es(�critureFlux));
        }
        public void EnvoyerEnnemiMort(int typeEnnemie, int numEnnemie)
        {
            �critureFlux.Position = 0;
            r�dacteur.Write((Byte)Protocoles.ValidationDeadEnnemi);
            //Envoi du type de l'ennemie
            r�dacteur.Write(typeEnnemie);
            //Envoi du num�ro de l'ennemie
            r�dacteur.Write(numEnnemie);
            EnvoiDonn�es(ObtentionDonn�es(�critureFlux));
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
            r�dacteur.Write((Byte)Protocoles.PlayerMovement);
            r�dacteur.Write(m11);
            r�dacteur.Write(m12);
            r�dacteur.Write(m13);
            r�dacteur.Write(m14);
            r�dacteur.Write(m21);
            r�dacteur.Write(m22);
            r�dacteur.Write(m23);
            r�dacteur.Write(m24);
            r�dacteur.Write(m31);
            r�dacteur.Write(m32);
            r�dacteur.Write(m33);
            r�dacteur.Write(m34);
            r�dacteur.Write(m41);
            r�dacteur.Write(m42);
            r�dacteur.Write(m43);
            r�dacteur.Write(m44);
            EnvoiDonn�es(ObtentionDonn�es(�critureFlux));
        }
        public void EnvoyerGainDeVie(int PointDeVie)
        {
            �critureFlux.Position = 0;
            r�dacteur.Write((Byte)Protocoles.HealthChange);
            r�dacteur.Write(PointDeVie);
            EnvoiDonn�es(ObtentionDonn�es(�critureFlux));
        }
        public void EnvoyerAttaqueW(Vector3 position, Vector3 direction, int force, int pr�cision, int d�gat)
        {
            �critureFlux.Position = 0;
            r�dacteur.Write((Byte)Protocoles.WAttack);

            //Envoi position joueurA
            r�dacteur.Write(position.X);
            r�dacteur.Write(position.Y);
            r�dacteur.Write(position.Z);
            //Envoi direction attaque
            r�dacteur.Write(direction.X);
            r�dacteur.Write(direction.Y);
            r�dacteur.Write(direction.Z);
            //Envoi de la force 
            r�dacteur.Write(force);
            //Envoi de la pr�cision 
            r�dacteur.Write(pr�cision);
            //Envoi du d�gat
            r�dacteur.Write(d�gat);
            EnvoiDonn�es(ObtentionDonn�es(�critureFlux));
        }
    }
}
