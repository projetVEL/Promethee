using Constellation;
using Constellation.Package;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ReadMeSomeMusic
{/// <summary>
/// On crée ici un programme qui lance VLC au chemin spécifié pour lire un répertoire musical spécifique.
/// </summary>
    public class Program : PackageBase
    {
        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }
        /// <summary>
        /// rien d'utile ici, tout se fait dans le Callback
        /// </summary>
        public override void OnStart()
        {
        }

        /// <summary>
        /// On envoie ici le callback avec le chemin de VLC ainsi que son 
        /// </summary>
        /// <param name="CheminVLC"> Chemin vers VLC </param>
        /// <param name="CheminMusique">Chemin vers le dossier de musique. Rajouter "/NomMusique.Format" si vous voulez juste lire une muusique en particulier</param>
        [MessageCallback(Key = "PleaseDontStopTheMusic")]
        void startAProgram(string CheminVLC, string CheminMusique)
        {
            Process toStart;
            toStart = new Process();
            try
            {

                toStart.StartInfo.FileName = CheminVLC;
                toStart.StartInfo.Arguments = CheminMusique;
                toStart.Start();
            }
            catch (Exception e)
            {
                try
                {
                    toStart.StartInfo.FileName = "C:/Program Files (x86)/VideoLAN/VLC/vlc.exe";
                    string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                    toStart.StartInfo.Arguments = "C:/Users/" + userName + "/Music";
                    toStart.Start();
                }catch(Exception ex)
                {
                    PackageHost.WriteInfo("Chemin vers VLC ou vers le dossier de musique incorrect, merci de vérifier si vous avez bien tout copié collé... En général VLC se situe ici : C:/Program Files (x86)/VideoLAN/VLC/vlc.exe");
                }
            }
        }
    }
}
