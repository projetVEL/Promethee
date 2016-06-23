using Constellation;
using Constellation.Package;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Timer
{
    public class Program : PackageBase
    {
        public DateTime starting, now;
        public TimeSpan  lastState;
        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        public override void OnStart()
        {
            starting = DateTime.Now;

            Task.Factory.StartNew(() =>
            {
                var lastTime = DateTime.MinValue;
                
                while (PackageHost.IsRunning)
                {
                    
                    if (DateTime.Now.Subtract(lastTime).TotalMilliseconds >= 1)
                    {
                        now = DateTime.Now;
                        TimeSpan susbstract = now - starting;
                        if (lastState.Seconds != susbstract.Seconds)
                            PackageHost.PushStateObject("ChronometerSeconds", susbstract.Seconds);
                        if (lastState.Minutes != susbstract.Minutes)
                            PackageHost.PushStateObject("ChronometerMinutes", susbstract.Minutes);
                        if (lastState.Hours != susbstract.Hours)
                            PackageHost.PushStateObject("ChronometerHours", susbstract.Hours);

                        lastState = susbstract;
                        lastTime = DateTime.Now;
                    }
                    Thread.Sleep(1);
                }
            });
        }  


        [MessageCallback(Key = "ResetTimer")]
        void ResetTimer(bool restart)
        {
            PackageHost.WriteInfo("Callback ok");
            if (restart)
                starting = DateTime.Now;
        }
    }
}
