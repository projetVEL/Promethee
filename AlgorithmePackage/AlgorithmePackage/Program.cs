using Constellation;
using Constellation.Package;
using System;
using System.Collections.Generic;

using ClassesAlgorithme;

namespace AlgorithmePackage
{
    public class Program : PackageBase
    {
        private static Algorithme algo1;
        private List<Algorithme> m_algorithmes = new List<Algorithme>();
        static void Main(string[] args)
        {
            //test : un package envoyant myValue = rand(0,10) et mValue = "a" : fonctionne
            Condition cond1 = new Condition();
            Dictionary<String, String> var1 = new Dictionary<String, String>();
            var1["sentinelle"] = "DESKTOP-FQMIBUN";
            var1["package"] = "ConstellationPackageConsole1";
            var1["variable"] = "myValue";            
            cond1.variables = var1;
            cond1.Valeure = 5;
            Condition cond2 = new Condition();
            Dictionary<String, String> var11 = new Dictionary<String, String>();
            var11["sentinelle"] = "DESKTOP-FQMIBUN";
            var11["package"] = "ConstellationPackageConsole1";
            var11["variable"] = "mValue";
            cond2.variables = var11;
            cond2.Valeure = "a";

            Realisation real1 = new Realisation();
            Dictionary<String, String> var2 = new Dictionary<String, String>();
            var2["sentinelle"] = "DESKTOP-FQMIBUN";
            var2["package"] = "ConstellationPackageConsole1";
            var2["callBack"] = "changeVal";
            real1.variables = var2;
            List<dynamic> maListe = new List<dynamic>();
            maListe.Add(42);
            maListe.Add("test");
            real1.Arguments = maListe;

            Realisation real2 = new Realisation();
            Dictionary<String, String> var22 = new Dictionary<String, String>();
            var22["sentinelle"] = "DESKTOP-FQMIBUN";
            var22["package"] = "ConstellationPackageConsole1";
            var22["callBack"] = "changeVal";
            real2.variables = var22;
            List<dynamic> maListe2 = new List<dynamic>();
            maListe2.Add(24);
            maListe2.Add("tset");
            real2.Arguments = maListe2;

            List<Condition> conditions = new List<Condition>();
            conditions.Add(cond1);
            conditions.Add(cond2);

            List<Realisation> realisations = new List<Realisation>();
            realisations.Add(real1);
            realisations.Add(real2);


            algo1 = new Algorithme(conditions, realisations, "1er algo", true);
            algo1.changeRestrictionHoraire(2);

            Console.WriteLine(algo1.toString(false));

            //ci-git un exemple de sérialisation à aller rechercher dans ../Debug/myfile.json
            //Constellation.Utils.SerializationHelper.SerializeToFile<Algorithme>(algo1, "myfile.json");

            PackageHost.Start<Program>(args);
        }

        public override void OnStart()
        { //get the algos from the bdd or constellation
            //on souscrit aux algos en memoire           
            m_algorithmes.Add(algo1);
            foreach (Algorithme algo in m_algorithmes)
            {
                foreach (Condition cond in algo.getConditions())
                {
                    subscribeStateObject(cond.variables);
                }
            }
            //lorsqu'une des valeures souscrites change
            PackageHost.StateObjectUpdated += (s, e) =>
            {
                checkAlgorithmes(e.StateObject);
            };
        }

        public void addAlgorithme(Algorithme algo)
        {
            if (algo != null)
            {
                m_algorithmes.Add(algo);
            }
        }
        public void deleteAlgorithme(String name)
        {
            foreach (Algorithme algo in m_algorithmes)
            {
                if (algo.Name == name)
                {
                    //suppression de l'algo depuis la bdd ou constellation
                    //suppression de l'algo de la bdd et du package en cours
                    m_algorithmes.Remove(algo);
                    foreach (Condition cond in algo.getConditions())
                    {
                        unSubscribeStateObject(cond.variables);
                    }
                }
            }
        }
        public void checkAlgorithmes(StateObject SO) //on ne peut utiliser de LinkObject car sentinelle,... variables
        {
            foreach (Algorithme algo in m_algorithmes)
            {
                if (algo.estActif() && !algo.estRestreintHorairement &&
                    algo.setDynamicValue(SO.SentinelName, SO.PackageName, SO.Name, SO.DynamicValue))
                {
                    algo.resetDerniereRealisation();
                    realiseAlgo(algo.getRealisations());
                }
            }
        }
        private void subscribeStateObject(Dictionary<String, String> var)
        {
            PackageHost.SubscribeStateObjects(sentinel: var["sentinelle"], package: var["package"], name: var["variable"]);
        }
        private void unSubscribeStateObject(Dictionary<String, String> var)
        {
            PackageHost.UnSubscribeStateObjects(sentinel: var["sentinelle"], package: var["package"], name: var["variable"]);
        }

        private void realiseAlgo(List<Realisation> realisations)
        {
            //pour toutes les realisations d'une liste, on appel les callbacks avec arguments qui correspondent
            foreach (Realisation real in realisations)
            {
                dynamic send;
                switch (real.Arguments.Count)
                {
                    case 0:
                        send = null;
                        break;
                    case 1:
                        send = real.Arguments[0];
                        break;
                    default:
                        send = real.Arguments;
                        break;
                }
                PackageHost.SendMessage(MessageScope.Create(MessageScope.ScopeType.Package,
                    $"{real.variables["sentinelle"]}/{real.variables["package"]}"), real.variables["callBack"], send);
            }
        }
        public override void OnPreShutdown()
        {
            //push the algos on constellation             
            base.OnPreShutdown();
        }
        public override void OnShutdown()
        {
            //push algos in mémories on the bdd
            //close the connexion and the thread with the bdd/web
            base.OnShutdown();
        }
    }
}
