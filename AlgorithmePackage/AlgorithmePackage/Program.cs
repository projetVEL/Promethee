using Constellation;
using Constellation.Package;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using ClassesAlgorithm;
using System.Threading.Tasks;
using System.Threading;

namespace AlgorithmePackage
{
    public class Program : PackageBase
    {
        private static Algorithme algo1; //used 4 tests
       
        private Dictionary<String, TimeSlot> m_reactivationAlgo = new Dictionary<String, TimeSlot>();
        private List<Algorithme> m_algorithmes = new List<Algorithme>();
        private List<Algorithme> m_pausedAlgorithmes = new List<Algorithme>();
        static void Main(string[] args)
        {
            //test : un package envoyant myValue = rand(0,10) et mValue = "a" -> fonctionne
            Condition cond1 = new Condition();
            Dictionary<String, String> var1 = new Dictionary<String, String>();
            var1["sentinel"] = "DESKTOP-FQMIBUN";
            var1["package"] = "ConstellationPackageConsole1";
            var1["variable"] = "myValue";            
            cond1.variables = var1;
            cond1.Value = 6;
            cond1.OperationTested = Operations.StrictlyLower;
            Condition cond2 = new Condition();
            Dictionary<String, String> var11 = new Dictionary<String, String>();
            var11["sentinel"] = "DESKTOP-FQMIBUN";
            var11["package"] = "ConstellationPackageConsole1";
            var11["variable"] = "mValue";
            cond2.variables = var11;
            cond2.Value = "2";
            cond2.OperationTested = Operations.Different;
            Condition cond3 = new Condition();
            Dictionary<String, String> var111 = new Dictionary<String, String>();
            var111["sentinel"] = "DESKTOP-FQMIBUN";
            var111["package"] = "ConstellationPackageConsole1";
            var111["variable"] = "myValue";
            cond3.variables = var111;
            cond3.Value = 4;
            cond3.OperationTested = Operations.Upper;

            Execution real1 = new Execution();
            Dictionary<String, String> var2 = new Dictionary<String, String>();
            var2["sentinel"] = "DESKTOP-FQMIBUN";
            var2["package"] = "ConstellationPackageConsole1";
            var2["callBack"] = "changeVal";
            real1.Variables = var2;
            List<dynamic> maListe = new List<dynamic>();
            maListe.Add(42);
            maListe.Add("new");
            real1.Arguments = maListe;

            Execution real2 = new Execution();
            Dictionary<String, String> var22 = new Dictionary<String, String>();
            var22["sentinel"] = "DESKTOP-FQMIBUN";
            var22["package"] = "ConstellationPackageConsole1";
            var22["callBack"] = "changeVal";
            real2.Variables = var22;
            List<dynamic> maListe2 = new List<dynamic>();
            maListe2.Add(24);
            maListe2.Add("wen");
            real2.Arguments = maListe2;

            List<Condition> conditions = new List<Condition>();
            conditions.Add(cond1);
            conditions.Add(cond2);
            conditions.Add(cond3);

            List<Execution> realisations = new List<Execution>();
            realisations.Add(real1);
            realisations.Add(real2);


            algo1 = new Algorithme(conditions, realisations, "1er algo", true);
            algo1.Waiting = 2;

            Console.WriteLine(algo1.toString(false));

            //ci-git un exemple de sérialisation à aller rechercher dans ../Debug/myfile.json
            //Constellation.Utils.SerializationHelper.SerializeToFile<Algorithme>(algo1, "myfile.json");

            PackageHost.Start<Program>(args);
        }

        public override void OnStart()
        {
            PackageHost.LastStateObjectsReceived += (s, e) =>
            {                
                try
                {
                    Newtonsoft.Json.Linq.JArray algo = e.StateObjects[1].DynamicValue;
                    m_algorithmes = JsonConvert.DeserializeObject<List<Algorithme>>(algo.ToString());
                    algo = e.StateObjects[0].DynamicValue;
                    m_pausedAlgorithmes = JsonConvert.DeserializeObject<List<Algorithme>>(algo.ToString());
                }
                catch(Exception exp)
                {
                    PackageHost.WriteDebug(exp.ToString());
                }
                SubscribeAllStateObject();
            };
/////////
           AddAlgorithme(algo1);
/////////            
            //lorsqu'une des valeures souscrites change
            PackageHost.StateObjectUpdated += (s, e) =>
            {
                CheckAlgorithmes(e.StateObject);
            };
            SubscribeAllStateObject();

            Task.Factory.StartNew(() =>
            {
                while (PackageHost.IsRunning)
                {
                    CheckTimeSlot();
                    Thread.Sleep(700);
                }
            });
        }
        private void SubscribeAllStateObject()
        {
            foreach (Algorithme algo in m_algorithmes)
            {
                foreach (Condition cond in algo.Conditions)
                {
                    SubscribeStateObject(cond.variables);
                }
            }
        }
        /// <summary>
        /// Ajout d'un algorithme, si un algorithme au nom identique existe deja, il sera ecrase par celui-ci.
        /// </summary>
        /// <param name="algo">
        /// L'algorithme qui sera ajouté.
        /// </param>
        [MessageCallback]
        public void AddAlgorithme(Algorithme algo)
        {
            if (algo != null)
            {
                foreach(Algorithme alg in m_algorithmes)
                {
                    if (algo.Name == alg.Name)
                    {
                        DeleteAlgorithme(alg.Name);
                        break;
                    }
                }
                foreach (Algorithme alg in m_pausedAlgorithmes)
                {
                    if (algo.Name == alg.Name)
                    {
                        DeleteAlgorithme(alg.Name);
                        break;
                    }
                }
                if(algo.IsActive)
                {
                    m_algorithmes.Add(algo);
                }
                else
                {
                    m_pausedAlgorithmes.Add(algo);
                }
                
            }
        }
        /// <summary>
        /// Supprime l'algorithme correspondant au nom donne, il ne sera plus stocke ni realise
        /// </summary>
        /// <param name="name">
        /// Le nom identifiant l'algorithme
        /// </param>
        [MessageCallback]
        public void DeleteAlgorithme(String name)
        {
            foreach (Algorithme algo in m_algorithmes)
            {
                if (algo.Name == name)
                {
                    //suppression de l'algo depuis la bdd ou constellation
                    //suppression de l'algo de la bdd et du package en cours                    
                    foreach (Condition cond in algo.CloneConditions())
                    {
                        UnSubscribeStateObject(cond.variables);
                    }
                    m_algorithmes.Remove(algo);
                    return;
                }
            }
            foreach (Algorithme algo in m_pausedAlgorithmes)
            {
                if (algo.Name == name)
                {
                    //suppression de l'algo depuis la bdd ou constellation
                    //suppression de l'algo de la bdd et du package en cours                    
                    foreach (Condition cond in algo.CloneConditions())
                    {
                        UnSubscribeStateObject(cond.variables);
                    }
                    m_pausedAlgorithmes.Remove(algo);
                    return;
                }
            }
        }
        /// <summary>
        /// Met l'algorithme en pause ou le relance.
        /// Peut servir pour désactiver manuellement des action ponctuelles (alarmes, ...)
        /// </summary>
        /// <param name="name">Le nom de l'algorithme à metre en pause</param>
        [MessageCallback(Key = "PauseResumeAlgo")]
        public void EnableDisableAlgorithme(String name)
        {
            foreach (Algorithme algo in m_algorithmes)
            {
                if(algo.Name == name)
                {                    
                    algo.EnableOrDisable();
                    m_pausedAlgorithmes.Add(algo);
                    m_algorithmes.Remove(algo);
                    return;
                }
            }
            foreach (Algorithme algo in m_pausedAlgorithmes)
            {
                if (algo.Name == name)
                {
                    algo.EnableOrDisable();
                    m_algorithmes.Add(algo);
                    m_pausedAlgorithmes.Remove(algo);
                    return;
                }
            }
        }
        private void CheckAlgorithmes(StateObject SO) //on ne peut utiliser de LinkObject car sentinelle,... variables
        {
            foreach (Algorithme algo in m_algorithmes)
            {
                if(!algo.Schedule.IsInTimeSlot())
                {
                    m_reactivationAlgo.Add(algo.Name, algo.Schedule);
                    EnableDisableAlgorithme(algo.Name);
                    continue;
                }
                if (!algo.IsTimeRestricted
                    && algo.SetDynamicValue(SO.SentinelName, SO.PackageName, SO.Name, SO.DynamicValue))
                {
                    algo.ResetLastExecution();
                    RealiseAlgorithme(algo.Executions);
                    if (algo.DisableAfterRealisation) EnableDisableAlgorithme(algo.Name);
                }
            }
        }
        private void SubscribeStateObject(Dictionary<String, String> var)
        {
            PackageHost.SubscribeStateObjects(sentinel: var["sentinel"], package: var["package"], name: var["variable"]);
        }
        /// <summary>
        /// se desincrit du stateObject definit par Sentinelle/pckg/nom si ce So n'est pas utile à un autre Algorithme enabled,
        /// il faut donc bien faire attention à disable l'Algorithme qui utilise ce StateObject au préalable
        /// </summary>
        /// <param name="var">dictionnaire définissant le StateObject"</param>
        private void UnSubscribeStateObject(Dictionary<String, String> var)
        {
            foreach(Algorithme algo in m_algorithmes)
            {
                if (algo.IsUsingStateObject(sentinel: var["sentinel"], package: var["package"], name: var["variable"])) return;
            }
            PackageHost.UnSubscribeStateObjects(sentinel: var["sentinel"], package: var["package"], name: var["variable"]);
        }
        private void RealiseAlgorithme(List<Execution> executions)
        {
            //pour toutes les realisations d'une liste, on appel les callbacks avec arguments qui correspondent
            foreach (Execution exec in executions)
            {
                dynamic send;
                switch (exec.Arguments.Count)
                {
                    case 0:
                        send = null;
                        break;
                    case 1:
                        send = exec.Arguments[0];
                        break;
                    default:
                        send = exec.Arguments;
                        break;
                }
                PackageHost.SendMessage(MessageScope.Create(MessageScope.ScopeType.Package,
                    $"{exec.Variables["sentinel"]}/{exec.Variables["package"]}"), exec.Variables["callBack"], send);
            }
        }
        public override void OnPreShutdown()
        {
            //push the algos on constellation     
            PackageHost.PushStateObject("Algorithmes", m_algorithmes);
            PackageHost.PushStateObject("pauseAlgorithmes", m_pausedAlgorithmes);     
            base.OnPreShutdown();
        }
        public override void OnShutdown()
        {
            base.OnShutdown();
        }
        private void CheckTimeSlot()
        {
            foreach(var item in m_reactivationAlgo)
            {
                if(item.Value.IsInTimeSlot())
                {
                    EnableDisableAlgorithme(item.Key);
                    m_reactivationAlgo.Remove(item.Key);
                }
            }
        }
    }
}
