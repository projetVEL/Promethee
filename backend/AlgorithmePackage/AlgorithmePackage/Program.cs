using Constellation;
using Constellation.Package;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using ClassesAlgorithm;
using System.Threading;

namespace AlgorithmePackage
{
    public class Program : PackageBase
    {
        private static Algorithme algo1; //used 4 tests
        private static Algorithme algo2; //used 4 tests

        private Thread thread = new Thread(new ThreadStart(CheckTimeSlot_ThreadLoop));
        private static Dictionary<String, double> m_reactivationAlgo = new Dictionary<String, double>();
        private static List<Algorithme> m_algorithmes = new List<Algorithme>();
        private static List<Algorithme> m_pausedAlgorithmes = new List<Algorithme>();
        static void Main(string[] args)
        {
  //test : un package envoyant myValue = rand(0,10) et mValue = "a" -> fonctionne
            Condition cond1 = new Condition();
            Dictionary<String, String> var1 = new Dictionary<String, String>();
            var1["sentinel"] = "DESKTOP-FQMIBUN";
            var1["package"] = "ConstellationPackageConsole1";
            var1["variable"] = "myValue";            
            cond1.Variables = var1;
            cond1.Value = 6;
            cond1.OperationTested = Operations.StrictlyLower;
            Condition cond2 = new Condition();
            Dictionary<String, String> var11 = new Dictionary<String, String>();
            var11["sentinel"] = "DESKTOP-FQMIBUN";
            var11["package"] = "ConstellationPackageConsole1";
            var11["variable"] = "mValue";
            cond2.Variables = var11;
            cond2.Value = "2";
            cond2.OperationTested = Operations.Different;
            Condition cond3 = new Condition();
            Dictionary<String, String> var111 = new Dictionary<String, String>();
            var111["sentinel"] = "DESKTOP-FQMIBUN";
            var111["package"] = "ConstellationPackageConsole1";
            var111["variable"] = "myValue";
            cond3.Variables = var111;
            cond3.Value = 0;
            cond3.OperationTested = Operations.Upper;

            Execution real1 = new Execution();
            Dictionary<String, String> var2 = new Dictionary<String, String>();
            var2["sentinel"] = "DESKTOP-FQMIBUN";
            var2["package"] = "ConstellationPackageConsole1";
            var2["callBack"] = "changeVal";
            real1.Variables = var2;
            List<dynamic> maListe = new List<dynamic>();
            maListe.Add(24);
            maListe.Add("entre 0 et 5");
            real1.Arguments = maListe;

            Execution real2 = new Execution();
            Dictionary<String, String> var22 = new Dictionary<String, String>();
            var22["sentinel"] = "DESKTOP-FQMIBUN";
            var22["package"] = "ConstellationPackageConsole1";
            var22["callBack"] = "changeVal";
            real2.Variables = var22;
            List<dynamic> maListe2 = new List<dynamic>();
            maListe2.Add(42);
            maListe2.Add("a 30s");
            real2.Arguments = maListe2;

            List<Condition> conditions = new List<Condition>();
            conditions.Add(cond1);
            conditions.Add(cond2);
            conditions.Add(cond3);

            List<Execution> realisations = new List<Execution>();
            realisations.Add(real1);
            realisations.Add(real2);

            Condition cond21 = new Condition();
            Dictionary<String, String> var21 = new Dictionary<String, String>();
            var21["sentinel"] = "DESKTOP-FQMIBUN";
            var21["package"] = "ConstellationPackageConsole1";
            var21["variable"] = "myValue";
            cond21.Variables = var21;
            cond21.Value = 0;
            cond21.OperationTested = Operations.Upper;
           

            Execution real21 = new Execution();
            Dictionary<String, String> var221 = new Dictionary<String, String>();
            var221["sentinel"] = "DESKTOP-FQMIBUN";
            var221["package"] = "ConstellationPackageConsole1";
            var221["callBack"] = "changeVal";
            real21.Variables = var221;
            List<dynamic> maListe21 = new List<dynamic>();
            maListe21.Add(5);
            maListe21.Add("coucou");
            real21.Arguments = maListe21;
            

            List<Condition> conditions2 = new List<Condition>();
            //conditions2.Add(cond21);

            List<Execution> realisations2 = new List<Execution>();
            realisations2.Add(real21);


            algo1 = new Algorithme(conditions, realisations, "1er algo", true);
            algo2 = new Algorithme(conditions2, realisations2, "2e algo", true);
            algo2.Waiting = 2;
            algo1.Waiting = 2;

            TimeSlot schedule = new TimeSlot();
            schedule.Begin.Month = 1;
            schedule.End.Month = 12;
            schedule.Begin.Day = 23;
            schedule.End.Day = 23;
            schedule.Begin.Hour = 0;
            schedule.End.Hour = 23;
            schedule.Begin.Minute = 0;
            schedule.End.Minute = 59;
            schedule.Begin.Second = 0;
            schedule.End.Second = 59;
            schedule.Week = new WeekDays(false, false, true, false, false, false, false);
            schedule.ReactivationPeriode = "Minutes";
            //algo1.DisableAfterRealisation = true;
            algo1.Schedule = schedule;

            Console.WriteLine(algo1.toString(false));
//fin test            


            PackageHost.Start<Program>(args);
        }

        public override void OnStart()
        {
            PackageHost.LastStateObjectsReceived += (s, e) =>
            {                
                try
                {
                    Newtonsoft.Json.Linq.JArray algo = e.StateObjects[1].DynamicValue;
                    List<Algorithme> algorithmes = JsonConvert.DeserializeObject<List<Algorithme>>(algo.ToString());
                    foreach(Algorithme alg in algorithmes)
                    {
                        AddAlgorithme(alg);
                    }
                    algo = e.StateObjects[0].DynamicValue;
                    algorithmes = JsonConvert.DeserializeObject<List<Algorithme>>(algo.ToString());
                    foreach (Algorithme alg in algorithmes)
                    {
                        AddAlgorithme(alg);
                    }
                }
                catch(Exception exp)
                {
                    PackageHost.WriteDebug(exp.ToString());
                }
                SubscribeAllStateObject();
                PackageHost.PushStateObject("Algorithmes", m_algorithmes);
                PackageHost.PushStateObject("PausedAlgorithmes", m_pausedAlgorithmes);
            };
/////////
          // AddAlgorithme(algo1);
          // AddAlgorithme(algo2);
/////////            
            //lorsqu'une des valeures souscrites change
            PackageHost.StateObjectUpdated += (s, e) =>
            {
                CheckAlgorithmes(e.StateObject);
            };
            SubscribeAllStateObject();

            //thread pour la reactivation des algos en pause
            thread.Start();
        }
        private void SubscribeAllStateObject()
        {
            foreach (Algorithme algo in m_algorithmes)
            {
                foreach (Condition cond in algo.Conditions)
                {
                    SubscribeStateObject(cond.Variables);
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
            if (algo.Description == null)
            {
                algo.Description = algo.toString(false);
            }
            PackageHost.PushStateObject("Algorithmes", m_algorithmes);
            PackageHost.PushStateObject("PausedAlgorithmes", m_pausedAlgorithmes);
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
                        UnSubscribeStateObject(cond.Variables);
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
                        UnSubscribeStateObject(cond.Variables);
                    }
                    m_pausedAlgorithmes.Remove(algo);
                    return;
                }
            }
            PackageHost.PushStateObject("Algorithmes", m_algorithmes);
            PackageHost.PushStateObject("PausedAlgorithmes", m_pausedAlgorithmes);
        }
        /// <summary>
        /// Met l'algorithme en pause ou le relance.
        /// Peut servir pour désactiver manuellement des action ponctuelles (alarmes, ...)
        /// </summary>
        /// <param name="name">Le nom de l'algorithme à metre en pause</param>
        /// <param name="enableByHoraire">permet de changer le boolan IsActiv de l'algo, ainsi on ne le change que lorsque 
        /// l'utilisateur demande a mettre en pause l'algo et non lorsqu'il se met en pause tout seul en sortant de sa Schedule
        ///  ou apres un Wainting</param>
        [MessageCallback(Key = "PauseResumeAlgo")]
        public static void EnableDisableAlgorithme(String name, Boolean enableByHoraire = false)
        {
            Boolean paused = true;
            foreach (Algorithme algo in m_algorithmes)
            {
                if (algo.Name == name)
                {
                    paused = false;
                    if (!enableByHoraire) algo.EnableOrDisable();
                    m_pausedAlgorithmes.Add(algo);
                    m_algorithmes.Remove(algo);                    
                    foreach (Condition cond in algo.Conditions)
                    {
                        UnSubscribeStateObject(cond.Variables);
                    }
                    break;  
                }
            }
            if(paused)
            {
                foreach (Algorithme algo in m_pausedAlgorithmes)
                {
                    if (algo.Name == name)
                    {
                        if (!enableByHoraire) algo.EnableOrDisable();
                        m_algorithmes.Add(algo);
                        m_pausedAlgorithmes.Remove(algo);
                        foreach (Condition cond in algo.Conditions)
                        {
                            SubscribeStateObject(cond.Variables);
                        }
                        break;
                    }
                }
            }
                        
            PackageHost.PushStateObject("Algorithmes", m_algorithmes);
            PackageHost.PushStateObject("PausedAlgorithmes", m_pausedAlgorithmes);
        }
        private static void CheckAlgorithmes(StateObject SO) //on ne peut utiliser de LinkObject car sentinelle,... variables
        {
            dynamic dynamicValue = null;
            String sentinel = null;
            String package = null;
            String name = null;

            if (SO != null)
            {
                dynamicValue = SO.DynamicValue;
                sentinel = SO.SentinelName;
                package = SO.PackageName;
                name = SO.Name;
            }            
            List<String> algoToDisable = new List<string>(); //on ne peux modifier la liste m_algo pendant son traitement foreach
            foreach (Algorithme algo in m_algorithmes)
            {
                if(algo.Schedule != null && !algo.Schedule.IsInTimeSlot())
                {
                    if (algo.Schedule.NextSlotBegin(false)<1.2) continue;
  /**/      //        PackageHost.WriteError($"not in schedule, reactive in {algo.Schedule.NextSlotBegin(false)}");
                    algoToDisable.Add(algo.Name);
                    m_reactivationAlgo.Add(algo.Name, algo.Schedule.NextSlotBegin(false));
                    continue;
                }
                if ( 
                    algo.SetDynamicValue(sentinel, package, name, dynamicValue))
                {                    
                    ExecuteAlgorithme(algo.Executions);
                    if (algo.Waiting != 0)
                    {//si on doit attendre X sec entre chaque execution
   /**/   //              PackageHost.WriteError($"wait for {algo.Waiting}");
                        algoToDisable.Add(algo.Name);
                        m_reactivationAlgo.Add(algo.Name, algo.Waiting);
                        continue;
                    }                    
                    if (algo.DisableAfterRealisation)
                    {//si l'algo doit se desactiver apres execution
                        algoToDisable.Add(algo.Name);
                        if(algo.Schedule.ReactivationPeriode != null)
                        {//si l'algo a une plage de restriction horaire, il se reactivera alors pour la prochaine plage                           
      /**/   //               PackageHost.WriteError($"reactive in {algo.Schedule.NextSlotBegin(true)}");
                            m_reactivationAlgo.Add(algo.Name, algo.Schedule.NextSlotBegin(true));                            
                        }
                    }
                }               
            }
            foreach(String algoName in algoToDisable)
            {
                EnableDisableAlgorithme(algoName, true);
            }
        }
        private static void SubscribeStateObject(Dictionary<String, String> var)
        {
            PackageHost.SubscribeStateObjects(sentinel: var["sentinel"], package: var["package"], name: var["variable"]);
        }
        /// <summary>
        /// se desincrit du stateObject definit par Sentinelle/pckg/nom si ce So n'est pas utile à un autre Algorithme enabled,
        /// il faut donc bien faire attention à disable l'Algorithme qui utilise ce StateObject au préalable
        /// </summary>
        /// <param name="var">dictionnaire définissant le StateObject"</param>
        private static void UnSubscribeStateObject(Dictionary<String, String> var)
        {
            foreach(Algorithme algo in m_algorithmes)
            {
                if (algo.IsUsingStateObject(sentinel: var["sentinel"], package: var["package"], name: var["variable"])) return;
            }
            PackageHost.UnSubscribeStateObjects(sentinel: var["sentinel"], package: var["package"], name: var["variable"]);
        }
        private static void ExecuteAlgorithme(List<Execution> executions)
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
            PackageHost.PushStateObject("PauseAlgorithmes", m_pausedAlgorithmes);     
            base.OnPreShutdown();
        }
        public override void OnShutdown()
        {
            thread.Abort();
            base.OnShutdown();
        }
        private static void CheckTimeSlot_ThreadLoop()
        {
            int count = 0;
            while(Thread.CurrentThread.IsAlive)
            {
                List<String> myKeys = new List<string>();
                
                List<String> algoToEnable = new List<string>();
                foreach (var item in m_reactivationAlgo)
                {
                    myKeys.Add(item.Key);
                    if (item.Value < 0.2)
                    {
                        algoToEnable.Add(item.Key);
                    }
                }
                foreach (String key in myKeys)
                {
                    m_reactivationAlgo[key] -= 0.2;
                }
                foreach (String algoName in algoToEnable)
                {
  /**/              PackageHost.WriteWarn("reAble");
                    m_reactivationAlgo.Remove(algoName);
                    EnableDisableAlgorithme(algoName, true);                    
                }                
                Thread.Sleep(200);
                count++;
                if (count % 5 == 0)
                {
                    CheckAlgorithmes(null);
                }                
            }            
        }
    }
}
