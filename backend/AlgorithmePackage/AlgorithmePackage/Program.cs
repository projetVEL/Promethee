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
        private bool receiveLastSO = false;
        private Thread thread;
        private Thread attenteReponseConstellation;
        private static Dictionary<String, double> m_reactivationAlgo = new Dictionary<String, double>();
        private static List<Algorithme> m_algorithmes = new List<Algorithme>();
        private static List<Algorithme> m_pausedAlgorithmes = new List<Algorithme>();
        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        public override void OnStart()
        {
            //lance le thread qui verifie si un algorithme doit etre reactive ou non
            thread = new Thread(new ThreadStart(CheckTimeSlot_ThreadLoop));
            //lance le thread attendant une reponse de Constellation : reçoit-on un l'event LastSOReceive ? Si oui, on repush les SO 
            //sinon on push une liste vide au bout de 2.5sec
            attenteReponseConstellation = new Thread(new ThreadStart(WaitConstellation_ThreadLoop));
            PackageHost.LastStateObjectsReceived += (s, e) =>
            {
                receiveLastSO = true;
                try
                {
                    Newtonsoft.Json.Linq.JArray algo = e.StateObjects[1].DynamicValue;
                    List<Algorithme> algorithmes = JsonConvert.DeserializeObject<List<Algorithme>>(algo.ToString());
                    foreach (Algorithme alg in algorithmes)
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
                catch (Exception exp)
                {
                    PackageHost.WriteDebug(exp.ToString());
                }
                SubscribeAllStateObject();
            };

            //lorsqu'une des valeures souscrites change
            PackageHost.StateObjectUpdated += (s, e) =>
            {
                CheckAlgorithmes(e.StateObject);
            };
            SubscribeAllStateObject();

            //thread pour la reactivation des algos en pause
            thread.Start();
            attenteReponseConstellation.Start();
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
            //  PackageHost.WriteError("receive algo :" + algo.toString());
            if (algo.URLPhotoDescription == "" || algo.URLPhotoDescription == null)
            {
                algo.URLPhotoDescription = "http://www.laboiteverte.fr/wp-content/uploads/2015/01/scene-film-animation-05.gif";
            }
            if (algo.Name == "" || algo.Name == null)
            {
                algo.CreateHashName();
            }
            if (algo.Description == "" || algo.Description == null)
            {
                algo.Description = algo.toString(false);
            }
            if (algo != null)
            {
                String algoDelete = null;
                foreach (Algorithme alg in m_algorithmes)
                {
                    if (algo.Name == alg.Name)
                    {
                        //  PackageHost.WriteWarn("delete enable");
                        algoDelete = alg.Name;
                        break;
                    }
                }
                foreach (Algorithme alg in m_pausedAlgorithmes)
                {
                    if (algo.Name == alg.Name)
                    {
                        //   PackageHost.WriteWarn("replace paused");
                        algoDelete = alg.Name;
                        break;
                    }

                }
                if (algoDelete != null)
                {
                    DeleteAlgorithme(algoDelete);
                }
                // PackageHost.WriteWarn($"algo {algo.Name} isActiv : {algo.IsActive}");
                if (algo.IsActive)
                {
                    m_algorithmes.Add(algo);
                    foreach (Condition cond in algo.Conditions)
                    {
                        SubscribeStateObject(cond.Variables);
                    }
                }
                else
                {
                    m_pausedAlgorithmes.Add(algo);
                }
            }
            //   PackageHost.WriteWarn("pushing add ");
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
            //PackageHost.WriteError("deleting " + name);
            var isInReactivation = false;
            foreach (var item in m_reactivationAlgo)
            {
                if (item.Key == name)
                {
                    isInReactivation = true;
                }
            }
            if (isInReactivation)
            {
                m_reactivationAlgo.Remove(name);
            }
            foreach (Algorithme algo in m_algorithmes)
            {
                if (algo.Name == name)
                {
                    //suppression de l'algo de la bdd et du package en cours                    
                    foreach (Condition cond in algo.CloneConditions())
                    {
                        UnSubscribeStateObject(cond.Variables);
                    }
                    m_algorithmes.Remove(algo);
                    //         PackageHost.WriteWarn("pushing del enable");
                    PackageHost.PushStateObject("Algorithmes", m_algorithmes);
                    PackageHost.PushStateObject("PausedAlgorithmes", m_pausedAlgorithmes);
                    return;
                }
            }
            foreach (Algorithme algo in m_pausedAlgorithmes)
            {
                if (algo.Name == name)
                {
                    //suppression de l'algo de la bdd et du package en cours                    
                    foreach (Condition cond in algo.CloneConditions())
                    {
                        UnSubscribeStateObject(cond.Variables);
                    }
                    m_pausedAlgorithmes.Remove(algo);
                    //         PackageHost.WriteWarn("pushing del paused");
                    PackageHost.PushStateObject("Algorithmes", m_algorithmes);
                    PackageHost.PushStateObject("PausedAlgorithmes", m_pausedAlgorithmes);
                    return;
                }
            }
        }
        /// <summary>
        /// Met l'algorithme en pause ou le relance.
        /// Peut servir pour désactiver manuellement des action ponctuelles (alarmes, ...)
        /// </summary>
        /// <param name="name">Le nom de l'algorithme à metre en pause</param>
        /// <param name="enableByHoraire">permet de changer le boolan IsActiv de l'algo, ainsi on ne le change que lorsque 
        /// l'utilisateur demande a mettre en pause l'algo et non lorsqu'il se met en pause tout seul en sortant de sa Schedule
        ///  ou apres un Wainting</param>
        public void EnableDisableAlgorithme(String name, Boolean enableByHoraire = false)
        {
            Boolean paused = true;
            foreach (Algorithme algo in m_algorithmes)
            {
                if (algo.Name == name)
                {
                    algo.ResetDynamicValue();
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
            if (paused)
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
        }
        private void CheckAlgorithmes(StateObject SO)
        {
            //  PackageHost.WriteError("check begin");
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
                //    PackageHost.WriteWarn($"{algo.Name} in check loop");
                //   PackageHost.WriteInfo($"{algo.Name} SO checked : {sentinel} {package} {name} {dynamicValue}");
                if (algo.Schedule != null && !algo.Schedule.IsInTimeSlot())
                {
                    //     PackageHost.WriteInfo($"{algo.Name} pas dans le timeSlot");
                    if (algo.Schedule.NextSlotBegin(false) < 1.2) continue;
                    //        PackageHost.WriteError($"not in schedule, reactive in {algo.Schedule.NextSlotBegin(false)}");
                    algoToDisable.Add(algo.Name);
                    m_reactivationAlgo.Add(algo.Name, algo.Schedule.NextSlotBegin(false));
                    continue;
                }
                //  PackageHost.WriteInfo($"{algo.Name} in timeSlot");
                if (algo.SetDynamicValue(sentinel, package, name, dynamicValue))
                {
                    //      PackageHost.WriteInfo($"{algo.Name} executed");
                    ExecuteAlgorithme(algo.Executions);
                    if (algo.Waiting != 0)
                    {//si on doit attendre X sec entre chaque executio
                     //         PackageHost.WriteError($"{algo.Name} waits for {algo.Waiting}");
                        algoToDisable.Add(algo.Name);
                        m_reactivationAlgo.Add(algo.Name, algo.Waiting);
                        continue;
                    }
                    if (algo.DisableAfterRealisation)
                    {//si l'algo doit se desactiver apres execution
                     //      PackageHost.WriteInfo($"{algo.Name} disable");
                        algoToDisable.Add(algo.Name);
                        if (algo.Schedule.ReactivationPeriode != null)
                        {//si l'algo a une plage de restriction horaire, il se reactivera alors pour la prochaine plage                           
                         //   PackageHost.WriteError($"{algo.Name} reactived in {algo.Schedule.NextSlotBegin(true)}");
                            m_reactivationAlgo.Add(algo.Name, algo.Schedule.NextSlotBegin(true));
                        }
                    }
                }
               /* else
                {
                       PackageHost.WriteInfo($"algo {algo.Name} didn't passed setDynVal");
                }*/
                //      PackageHost.WriteInfo($"fin boucle");
            }
            //    PackageHost.WriteInfo($"sortie boucle");
            foreach (String algoName in algoToDisable)
            {
                //  PackageHost.WriteError($"{algoName} goes in reactivation");
                this.EnableDisableAlgorithme(algoName, true);
            }
        }
        private static void SubscribeStateObject(Dictionary<String, String> var)
        {
            //    PackageHost.WriteWarn("subscribe");
            PackageHost.SubscribeStateObjects(sentinel: var["sentinel"], package: var["package"], name: var["variable"]);
        }
        /// <summary>
        /// se desincrit du stateObject definit par Sentinelle/pckg/nom si ce So n'est pas utile à un autre Algorithme enabled,
        /// il faut donc bien faire attention à disable l'Algorithme qui utilise ce StateObject au préalable
        /// </summary>
        /// <param name="var">dictionnaire définissant le StateObject"</param>
        private static void UnSubscribeStateObject(Dictionary<String, String> var)
        {
            //         PackageHost.WriteWarn("unsubscribe");
            foreach (Algorithme algo in m_algorithmes)
            {
                if (algo.IsUsingStateObject(sentinel: var["sentinel"], package: var["package"], name: var["variable"])) return;
            }
            PackageHost.UnSubscribeStateObjects(sentinel: var["sentinel"], package: var["package"], name: var["variable"]);
        }
        private static void ExecuteAlgorithme(List<Execution> executions)
        {
            //     PackageHost.WriteInfo("execution");
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
            //      PackageHost.WriteWarn("pushing preshut");
            PackageHost.PushStateObject("Algorithmes", m_algorithmes);
            PackageHost.PushStateObject("PausedAlgorithmes", m_pausedAlgorithmes);
            base.OnPreShutdown();
        }
        public override void OnShutdown()
        {
            thread.Abort();
            base.OnShutdown();
        }
        private void CheckTimeSlot_ThreadLoop()
        {
            //      PackageHost.WriteWarn("debut thread chackTimeSlot");
            int count = 0;
            while (Thread.CurrentThread.IsAlive)
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
                    /**/   //           PackageHost.WriteWarn("reAble");
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
        private void WaitConstellation_ThreadLoop()
        {
            var tour = 0;
            while (tour < 5 && !receiveLastSO)
            {
                tour++;
                Thread.Sleep(500);
            }
            PackageHost.PushStateObject("Algorithmes", m_algorithmes);
            PackageHost.PushStateObject("PausedAlgorithmes", m_pausedAlgorithmes);
            attenteReponseConstellation.Abort();
        }
    }
}
