using Constellation;
using Constellation.Package;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Packages
{
    /// <summary>
    /// Ici on s'occupera de renvoyer l'heure et la date, à la seconde près
    /// On les renvoie individuellement afin de ne pas spammer le serveur avec un gros StateObject contenant toutes les informations 
    /// en effet il est très rare de changer d'année chaque seconde !
    /// </summary>
    public class Program : PackageBase
    {
        int JetLag = 0;
        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }
        /// <summary>
        /// ainsi commence par créer une tâche, dans laquelle on créera une boucle infinie qui s'éxécute toutes les millisecondes.
        /// Dans celle-ci on éclare <c> LastTest </c> qui représente littéralement le dernier test effectué.*
        /// Ensuite on crée <c> lastTime </c> servira à faire la boucle.
        /// Commence alors le push de SO, chacun individuellements comme expliqué au dessus.
        /// Puis dans la boucle qui durera tout le temps que le package est en cours d'utilisation, on pushera les SO uniquement si il y a une différence avec la date actuelle
        /// Ainsi on économise le serveur en n'envoyant que les données "utiles" 
        /// </summary>
        public override void OnStart()
        {

            Task.Factory.StartNew(() =>
                {
                    DateTime LastTest = DateTime.Now.AddHours(JetLag);
                    var lastTime = DateTime.MinValue;
                    PackageHost.PushStateObject("Secondes", DateTime.Now.AddHours(JetLag).Second);
                    PackageHost.PushStateObject("Minutes", DateTime.Now.AddHours(JetLag).Minute);
                    PackageHost.PushStateObject("Heures", DateTime.Now.AddHours(JetLag).Hour);
                    PackageHost.PushStateObject("Jour", DateTime.Now.AddHours(JetLag).Day);
                    PackageHost.PushStateObject("Mois", DateTime.Now.AddHours(JetLag).Month);
                    PackageHost.PushStateObject("Annee", DateTime.Now.AddHours(JetLag).Year);
                    PackageHost.PushStateObject("JourDeLaSemaine", DateTime.Now.AddHours(JetLag).DayOfWeek.ToString());

                    while (PackageHost.IsRunning)
                    {
                        if (DateTime.Now.Subtract(lastTime).TotalMilliseconds >= 1)
                        {
                            if (LastTest.Second != DateTime.Now.AddHours(JetLag).Second)
                            {
                                PackageHost.PushStateObject("Secondes", DateTime.Now.AddHours(JetLag).Second);
                            }
                            if (LastTest.Minute != DateTime.Now.AddHours(JetLag).Minute)
                            {
                                PackageHost.PushStateObject("Minutes", DateTime.Now.AddHours(JetLag).Minute);
                            }
                            if (LastTest.Hour != DateTime.Now.AddHours(JetLag).Hour)
                            {
                                PackageHost.PushStateObject("Heures", DateTime.Now.AddHours(JetLag).Hour);
                            }
                            if (LastTest.Day != DateTime.Now.AddHours(JetLag).Day)
                            {
                                PackageHost.PushStateObject("Jour", DateTime.Now.AddHours(JetLag).Day);
                            }

                            if (LastTest.Month != DateTime.Now.AddHours(JetLag).Month)
                            {
                                PackageHost.PushStateObject("Mois", DateTime.Now.AddHours(JetLag).Month);
                            }
                            if (LastTest.Year != DateTime.Now.AddHours(JetLag).Year)
                            {
                                PackageHost.PushStateObject("Annee", DateTime.Now.AddHours(JetLag).Year);

                            }
                            if (LastTest.DayOfWeek != DateTime.Now.AddHours(JetLag).DayOfWeek)
                            {
                                PackageHost.PushStateObject("JourDeLaSemaine", DateTime.Now.AddHours(JetLag).DayOfWeek.ToString());
                            }
                            LastTest = DateTime.Now;
                            lastTime = DateTime.Now;
                        }
                        Thread.Sleep(1);
                    }
                });
        }


       /// <summary>
       /// On peut donc changer le fuseau horaire, pratique en cas de passage à l'heure d'été/hiver par exemple
       /// </summary>
       /// <param name="utc">décalage horaire (positif ou négatif) que l'on souhaite appliquer</param>
        [MessageCallback(Key = "Fuseau")]
        void changeUTC(int utc)
        {
            JetLag = utc;
        }
    }
}
