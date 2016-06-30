using Constellation;
using Constellation.Package;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Beeper
{/// <summary>
/// On se retrouve ici en présence d'un Beeper, l'intêret est de beeper à une fréquence donnée pendant une durée donnée tout en renvoyant l'état de beep
/// </summary>
    public class Program : PackageBase
    {
        /// <summary>
        /// L'idée est de mettre un booléen isBeeping tant que le système beep en précisant l'ancien état (précédente itération) ainsi on push le SO seulement quand c'est nécessaire pour
        /// ne pas encombrer le serveur constellation
        /// </summary>
        public bool isBeeping = false, previousState = false;
        /// <summary>
        /// Durée de beep afin de retourner l'état de beep
        /// </summary>
        public int beepTime;
        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }
        /// <summary>
        /// On push d'abord un état faux pour le beep, puisque ça ne beep pas. 
        /// Ensuite si il y a modification de l'état de beep, on changera uniquement si l'état est différent de celui d'avant (d'où l'intêret des deux booléens)
        /// </summary>
        public override void OnStart()
        {

            Task.Factory.StartNew(() =>
            {
                var lastTime = DateTime.MinValue;
                PackageHost.PushStateObject("Beep", isBeeping);
                while (PackageHost.IsRunning)
                {
                    if (DateTime.Now.Subtract(lastTime).TotalMilliseconds >= 1)
                    {
                        if (previousState != isBeeping)
                            PackageHost.PushStateObject("Beep", isBeeping);
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
        /// <summary>
        /// On lance le beep pour la fréquence donnée et la durée donnée par l'utilisateur
        /// </summary>
        /// <param name="frequency">Fréquence</param>
        /// <param name="duration">durée</param>
        [MessageCallback(Key = "Beep")]
        void BeepMe(int frequency, int duration)
        {
            Console.Beep(frequency, duration);
            beepTime = duration;

        }
    }



}
