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

        MemoryStream lectureFlux, écritureFlux;

        BinaryReader lecteur;
        BinaryWriter rédacteur;

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
            Client.GetStream().BeginRead(LectureTampon, 0, GRANDEUR_TAMPON, RéceptionDuFlux, null);


            lectureFlux = new MemoryStream();
            écritureFlux = new MemoryStream();

            lecteur = new BinaryReader(lectureFlux);
            rédacteur = new BinaryWriter(écritureFlux);

        }

        private void RéceptionDuFlux(IAsyncResult resultat)
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

            byte[] donnée = new byte[bytesLus];

            for (int i = 0; i < bytesLus; i++)
            {
                donnée[i] = LectureTampon[i];
            }

            TraitementDonnées(donnée);
            Client.GetStream().BeginRead(LectureTampon, 0, GRANDEUR_TAMPON, RéceptionDuFlux, null);
        }

        private void TraitementDonnées(byte[] data)
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
                            List<EntitéEnnemie> entitésEnnemies = Game.Components.OfType<EntitéEnnemie>().ToList();
                            foreach (EntitéEnnemie entitéEnnemie in entitésEnnemies)
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
                                entitéEnnemie.Monde = new Matrix(m11, m12, m13, m14, m21, m22, m23, m24, m31, m32, m33, m34, m41, m42, m43, m44);
                            }
                            break;
                        }
                    case (Protocoles.MinionMovement):
                        {
                            int numPéon = lecteur.ReadInt32();
                            List<EntitéPéonEnnemie> péons = Game.Components.OfType<EntitéPéonEnnemie>().ToList();
                            foreach (EntitéPéonEnnemie péon in péons)
                            {
                                if (péon.NumPéon == numPéon)
                                {
                                    float px = lecteur.ReadSingle();
                                    float py = lecteur.ReadSingle();
                                    float pz = lecteur.ReadSingle();
                                    Vector3 positionEnnemie = new Vector3(px, py, pz);
                                    péon.GérerDéplacement(positionEnnemie);
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
                            Entité theEntité = null;

                            GestionnaireJeu game = Game.Components.First(x => x is GestionnaireJeu) as GestionnaireJeu;

                            float px = lecteur.ReadSingle();
                            float py = lecteur.ReadSingle();
                            float pz = lecteur.ReadSingle();
                            int force = lecteur.ReadInt32();
                            int précision = lecteur.ReadInt32();
                            int typeEnemmie = lecteur.ReadInt32();
                            int numEnnemie = lecteur.ReadInt32();
                            int dégat = lecteur.ReadInt32();

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
                            break;
                        }
                    case (Protocoles.ValidationDeadEnnemi):
                        {
                            int typeEnemmie = lecteur.ReadInt32();
                            int numEnnemie = lecteur.ReadInt32();

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
                            break;
                        }
                    case (Protocoles.HealthChange):
                        {
                            int pdv = lecteur.ReadInt32();
                            List<EntitéEnnemie> entitésEnnemies = Game.Components.OfType<EntitéEnnemie>().ToList();
                            foreach (EntitéEnnemie entitéEnnemie in entitésEnnemies)
                            {
                                entitéEnnemie.PointDeVie = pdv;
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
                            int précision = lecteur.ReadInt32();
                            int dégat = lecteur.ReadInt32();
                            ProjectileAttaqueW projectile = new ProjectileAttaqueW(Game, "bomb", game.ÉCHELLE_PROJECTILE_W, game.RotationInitialeProjectielADB, new Vector3(px, py, pz), game.DirectionInitialeProjectileADB, new Vector3(dx, dy, dz), force, précision, dégat, game.INTERVALLEMAJ, 2);
                            Game.Components.Add(projectile);
                            break;
                        }

                }
            }
            catch (Exception) { }
        }


        private byte[] ObtentionDonnées(MemoryStream fluxMemoire)
        {
            byte[] resultat;

            lock (fluxMemoire)// On lock pour que l'exécution se fasse avant que d'autre donnée soit capté et analysé.
            {
                int bytesÉcrits = (int)fluxMemoire.Position;
                resultat = new byte[bytesÉcrits];

                fluxMemoire.Position = 0;
                fluxMemoire.Read(resultat, 0, bytesÉcrits);
            }

            return resultat;
        }

        public void EnvoiDonnées(byte[] tableauByte)
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
            écritureFlux.Position = 0;
            rédacteur.Write((Byte)Protocoles.StartGame);
            rédacteur.Write(true);
            EnvoiDonnées(ObtentionDonnées(écritureFlux));
        }

        public void EnvoyerPositionPéon(Vector3 position, int numPéon)
        {
            écritureFlux.Position = 0;
            rédacteur.Write((Byte)Protocoles.MinionMovement);
            //Envoi du numéro de péon
            rédacteur.Write(numPéon);
            //Envoi de la position du péon
            rédacteur.Write(position.X);
            rédacteur.Write(position.Y);
            rédacteur.Write(position.Z);

            EnvoiDonnées(ObtentionDonnées(écritureFlux));
        }
        public void EnvoyerAttaque(Vector3 position, int force, int précision, int typeEnnemie, int numEnnemie, int dégat)
        {
            écritureFlux.Position = 0;
            rédacteur.Write((Byte)Protocoles.BasicAttaque);
            //Envoi position joueurA
            rédacteur.Write(position.X);
            rédacteur.Write(position.Y);
            rédacteur.Write(position.Z);
            //Envoi de la force 
            rédacteur.Write(force);
            //Envoi de la précision 
            rédacteur.Write(précision);
            //Envoi du type de l'ennemie
            rédacteur.Write(typeEnnemie);
            //Envoi du numéro de l'ennemie
            rédacteur.Write(numEnnemie);
            //Envoie du dégat
            rédacteur.Write(dégat);

            EnvoiDonnées(ObtentionDonnées(écritureFlux));
        }
        public void EnvoyerEnnemiMort(int typeEnnemie, int numEnnemie)
        {
            écritureFlux.Position = 0;
            rédacteur.Write((Byte)Protocoles.ValidationDeadEnnemi);
            //Envoi du type de l'ennemie
            rédacteur.Write(typeEnnemie);
            //Envoi du numéro de l'ennemie
            rédacteur.Write(numEnnemie);
            EnvoiDonnées(ObtentionDonnées(écritureFlux));
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
            écritureFlux.Position = 0;
            rédacteur.Write((Byte)Protocoles.PlayerMovement);
            rédacteur.Write(m11);
            rédacteur.Write(m12);
            rédacteur.Write(m13);
            rédacteur.Write(m14);
            rédacteur.Write(m21);
            rédacteur.Write(m22);
            rédacteur.Write(m23);
            rédacteur.Write(m24);
            rédacteur.Write(m31);
            rédacteur.Write(m32);
            rédacteur.Write(m33);
            rédacteur.Write(m34);
            rédacteur.Write(m41);
            rédacteur.Write(m42);
            rédacteur.Write(m43);
            rédacteur.Write(m44);
            EnvoiDonnées(ObtentionDonnées(écritureFlux));
        }
        public void EnvoyerGainDeVie(int PointDeVie)
        {
            écritureFlux.Position = 0;
            rédacteur.Write((Byte)Protocoles.HealthChange);
            rédacteur.Write(PointDeVie);
            EnvoiDonnées(ObtentionDonnées(écritureFlux));
        }
        public void EnvoyerAttaqueW(Vector3 position, Vector3 direction, int force, int précision, int dégat)
        {
            écritureFlux.Position = 0;
            rédacteur.Write((Byte)Protocoles.WAttack);

            //Envoi position joueurA
            rédacteur.Write(position.X);
            rédacteur.Write(position.Y);
            rédacteur.Write(position.Z);
            //Envoi direction attaque
            rédacteur.Write(direction.X);
            rédacteur.Write(direction.Y);
            rédacteur.Write(direction.Z);
            //Envoi de la force 
            rédacteur.Write(force);
            //Envoi de la précision 
            rédacteur.Write(précision);
            //Envoi du dégat
            rédacteur.Write(dégat);
            EnvoiDonnées(ObtentionDonnées(écritureFlux));
        }
    }
}
