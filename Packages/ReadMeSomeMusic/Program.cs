using Constellation;
using Constellation.Package;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ReadMeSomeMusic
{
    public class Program : PackageBase
    {
        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        public override void OnStart()
        {
            PackageHost.WriteInfo("Package starting - IsRunning: {0} - IsConnected: {1}", PackageHost.IsRunning, PackageHost.IsConnected);
        }


        [MessageCallback(Key = "PleaseDontStopTheMusic")]
        void startAProgram(string pathToVLC, string musicFilePath)
        {
            //here we take the Path to VLC, if it is not written, we will take the one by default, same way for the music path, will take the one in your windows files
            Process toStart;
            toStart = new Process();
            try
            {

                toStart.StartInfo.FileName = pathToVLC;
                toStart.StartInfo.Arguments = musicFilePath;
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
                    PackageHost.WriteInfo("Eh gogol");
                }
            }
        }
    }
}
