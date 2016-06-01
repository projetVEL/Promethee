using Constellation;
using Constellation.Package;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            var1["package"] = "package1";
            var1["sentinelle"] = "sent1";
            var1["variable"] = "var1" ;
            cond1.variables = var1;
            cond1.Valeure = 42;
            Realisation real1 = new Realisation();
            real1.variables = var1;
            List<dynamic> maListe = new List<dynamic>();
            maListe.Add(18);
            maListe.Add("test");
            real1.Arguments = maListe;

            List<Condition> conditions = new List<Condition>();
            conditions.Add(cond1);
            List<Realisation> realisations = new List<Realisation>();
            realisations.Add(real1);


            Algorithme algo1 = new Algorithme(conditions, realisations, "algo1", true);            
            Console.WriteLine(algo1.toString(false));


            PackageHost.Start<Program>(args);
        }      

        public override void OnStart()
        {
            PackageHost.WriteInfo("Package starting - IsRunning: {0} - IsConnected: {1}", PackageHost.IsRunning, PackageHost.IsConnected);
        }

        public void addAlgorithme(Algorithme algo)
        {
            if (algo != null)
            {
                m_algorithmes.Add(algo);
            }
        }
        public void checkAlgorithmes()
        {
            foreach (Algorithme algo in m_algorithmes)
            {
                Boolean conditionsOk = true;
                foreach(Condition condition in algo.getConditions())
                {
                    if (getValue(condition.variables) != "condition.variables.valeureVariable")
                    {
                        conditionsOk = false;
                        break;
                    }                    
                }
                if (conditionsOk)
                {
                    foreach (Realisation realisation in algo.getRealisations())
                    {
                        setValue(realisation.variables);
                    }
                }
            }
        }
        private dynamic getValue(Dictionary<String, String> var)
        {
            //recherche sur constellation la valeure de la variable sur le duo package/sentinelle indiqué
            PackageHost.RequestStateObjects(sentinel: var["sentinelle"], package: var["package"], name: var["variable"]);
            return "ee";
        }
        private void setValue(Dictionary<String, String> var)
        {
            //met a jour la variable sur constellation indiqué par le trio nom variable/package/sentinelle
        }
    }
}
