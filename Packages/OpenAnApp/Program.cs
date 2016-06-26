using Constellation;
using Constellation.Package;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace OpenAnApp
{
    public class Program : PackageBase
    {
        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        public override void OnStart()
        {
            //Nothing to see here, go deeper in this .cs
            PackageHost.WriteInfo("Package starting - IsRunning: {0} - IsConnected: {1}", PackageHost.IsRunning, PackageHost.IsConnected);
        }


        [MessageCallback (Key="StartMe")]
        void startAProgram(string PathToYourExecutableFile)
        {
            //here we take the name/path of the program to launch it, then we launch it, such difficult much hard wow   
            try
            {
                Process toStart;
                toStart = new Process();
                toStart.StartInfo.FileName = PathToYourExecutableFile;
                toStart.Start();
            }catch(Exception e)
            {
                PackageHost.WriteInfo("Eh gogol met un bon chemin");        
            }
        }
    }
}
