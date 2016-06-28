using Constellation;
using Constellation.Package;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace OpenAnApp
{/// <summary>
/// l'idée est de faire un lanceur d'application basique
/// </summary>
    public class Program : PackageBase
    {
        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }
        /// <summary>
        /// Rien d'utile tout se fait dans le callback
        /// </summary>
        public override void OnStart()
        {
        }

        /// <summary>
        /// Callback pour lancer le programme avec le chemin donné. Si le chemin ne donne rien, on ne lance rien
        /// </summary>
        /// <param name="PathToYourExecutableFile">chemin vers l'executable</param>
        [MessageCallback (Key="LancerProgramme")]
        void startAProgram(string PathToYourExecutableFile)
        {
            try
            {
                Process toStart;
                toStart = new Process();
                toStart.StartInfo.FileName = PathToYourExecutableFile;
                toStart.Start();
            }catch(Exception e)
            {
                PackageHost.WriteInfo("Chemin vers l'executable incorrect, merci de vérifier ce que vous mettez.");        
            }
        }
    }
}
