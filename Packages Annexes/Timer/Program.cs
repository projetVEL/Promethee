using Constellation;
using Constellation.Package;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Timer
{/// <summary>
/// On est ici en présence d'un timer qui peut être relancé. L'idée est juste de récupérer du temps à partir d'un certain moment.
/// </summary>
    public class Program : PackageBase
    {/// <summary>
     /// Starting : démarrage du chrono
     /// now : moment actuel afin de faire la soustraction du départ pour trouver le temps écoulé depuis le début du chrono
     /// </summary>
        public DateTime starting, now;
        /// <summary>
        /// Dernier état afin de push uniquement les SO correspondant aux changement.
        /// </summary>
        public TimeSpan lastState;
        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }
        /// <summary>
        /// On va faire la différence entre le départ du chrono et le DateTime actuel afin de renvoyer uniquement l'écart en heures/minutes/secondes quand c'est nécessaire
        /// ainsi on va uniquement push ce qu'il nous faut pour ne pas surcharger le serveur
        /// </summary>
        public override void OnStart()
        {
            starting = DateTime.Now;

            PackageHost.PushStateObject("SecondesChrono", 0);
            PackageHost.PushStateObject("MinutesChrono", 0);
            PackageHost.PushStateObject("HeuresChrono", 0);
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
                            PackageHost.PushStateObject("SecondesChrono", susbstract.Seconds);
                        if (lastState.Minutes != susbstract.Minutes)
                            PackageHost.PushStateObject("MinutesChrono", susbstract.Minutes);
                        if (lastState.Hours != susbstract.Hours)
                            PackageHost.PushStateObject("HeuresChrono", susbstract.Hours);

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
            if (restart)
                starting = DateTime.Now;
        }
    }
}
