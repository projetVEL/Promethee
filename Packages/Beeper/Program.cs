using Constellation;
using Constellation.Package;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Beeper
{
    public class Program : PackageBase
    {
        public bool isBeeping = false, previousState = false;
        public int beepTime;
        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        public override void OnStart()
        {
            /* you may need a half second beep, but to be accurate with the state of the beeping we need a short actualisation
             * this way we will push the StateObject only when there is a difference between the one before.
             * this way we check every millisecond but we don't push every millisecond to save our network and not to flood the server */
            Task.Factory.StartNew(() =>
            {
                var lastTime = DateTime.MinValue;
                //Because we push when the state is different, we need to push the first time manually then, the thread will do it on its own
                PackageHost.PushStateObject("isBeeping?", isBeeping);
                while (PackageHost.IsRunning)
                {
                    if (DateTime.Now.Subtract(lastTime).TotalMilliseconds >= 1)
                    {
                        if (previousState != isBeeping)
                            PackageHost.PushStateObject("isBeeping?", isBeeping);
                        beepTime = beepTime - (int)DateTime.Now.Subtract(lastTime).TotalMilliseconds;
                        if (beepTime > 0)
                        {
                            isBeeping = true;
                            if (previousState != isBeeping)
                                previousState = isBeeping;
                        }
                        else
                        {
                            isBeeping = false;
                            if (previousState != isBeeping)
                                previousState = isBeeping;
                        }

                        lastTime = DateTime.Now;
                    }
                    Thread.Sleep(1);
                }
                
            });
        }

        [MessageCallback(Key = "Beep")]
        void BeepMe(int frequency, int duration)
        {
            Console.Beep(frequency, duration);
            beepTime = duration;

        }
    }



}
