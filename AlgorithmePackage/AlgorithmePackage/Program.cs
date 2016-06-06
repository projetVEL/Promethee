using Constellation;
using Constellation.Package;
using System;
using System.Collections.Generic;

using ClassesAlgorithme;

namespace AlgorithmePackage
{
    public class Program : PackageBase
    {        
        private List<Algorithme> m_algorithmes = new List<Algorithme>();
        static void Main(string[] args)
        {   
            Condition cond1 = new Condition();
            Dictionary<String, String> var1 = new Dictionary<String, String>();
            var1["sentinelle"] = "sent1";
            var1["package"] = "package1";
            var1["variable"] = "var1" ;
            cond1.variables = var1;
            cond1.Valeure = 42;
            Realisation real1 = new Realisation();
            real1.variables = var1;
            List<dynamic> maListe = new List<dynamic>();
            maListe.Add(18);
            maListe.Add("test");
            real1.Arguments = maListe;
            Condition cond2 = new Condition();
            Dictionary<string, string> vars2 = new Dictionary<string, string>();
            vars2["sentinelle"] = "sent2";
            vars2["package"] = "package";
            vars2["variable"] = "var";
            cond2.variables = vars2;
            cond2.Valeure = "test";

            List<Condition> conditions = new List<Condition>();
            conditions.Add(cond1);
            conditions.Add(cond2);
            List<Realisation> realisations = new List<Realisation>();
            realisations.Add(real1);


            Algorithme algo1 = new Algorithme(conditions, realisations, "algo1", true);            
            Console.WriteLine(algo1.toString(false));


            PackageHost.Start<Program>(args);
        }

        public override void OnStart()
        { //get the algos from the bdd or constellation
            //on souscrit aux algos en memoire
            foreach (Algorithme algo in m_algorithmes)
            {
                foreach (Condition cond in algo.getConditions())
                {
                    subscribeStateObject(cond.variables);
                }
            }
            //lorsqu'une des valeures souscrite change
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
            foreach(Algorithme algo in m_algorithmes)
            {
                if(algo.Name == name)
                {
                    //suppression de l'algo depuis la bdd ou constellation
                    //suppression de l'algo de la bdd et du package en cours
                    m_algorithmes.Remove(algo);
                    //suppression du subscribe
                }
            }
        }
        public void checkAlgorithmes(StateObject SO) //on ne peut utiliser de LinkObject car sentinelle,... variables
        {
            foreach(Algorithme algo in m_algorithmes)
            {
                if(algo.setDynamicValue(SO.SentinelName, SO.PackageName, SO.Name, SO.DynamicValue))
                {
                    realiseAlgo(algo.getRealisations());
                }
            }
        }
        private void subscribeStateObject(Dictionary<String, String> var)
        {
            PackageHost.SubscribeStateObjects(sentinel: var["sentinelle"], package: var["package"], name: var["variable"]);
        }
        private void realiseAlgo(List<Realisation> realisations)
        {
            //pour toutes les realisations d'une liste, on appel les callbacks avec arguments qui correspondent
            foreach (Realisation real in realisations)
            {
                MessageScope scope = MessageScope.Create(MessageScope.ScopeType.Package, 
                    $"{real.variables["sentinelle"]}/{real.variables["package"]}");
                PackageHost.SendMessage(scope, real.variables["variable"], real.Arguments);
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
