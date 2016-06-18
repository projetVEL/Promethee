using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace ClassesAlgorithm
{
    public class Algorithme : ICloneable
    {
        public TimeSlot Schedule { get; set; } = new TimeSlot();
        private DateTime m_lastExecution = new DateTime();
        public int Waiting { get; set; } = 0;
        public Boolean IsActive { get; set; } = false;
        public List<Condition> Conditions { get; set; }
        public List<Execution> Executions { get; set; }
        public String Name { set; get; }
        public Boolean DisableAfterRealisation { get; set; } = false;

        public bool IsTimeRestricted
        {
            get
            {
                if (Waiting == 0) return true;
                double delta = DateTime.Now.Subtract(m_lastExecution).TotalSeconds;
                if (delta >= Waiting)
                {                    
                    return false;
                }
                return true;
            }
        }
        public void ResetLastExecution()
        {
            m_lastExecution = DateTime.Now;
        }
        public Boolean IsUsingStateObject(String sentinel, String package, String name)
        {//retourne true si toutes les conditions sont vérifiées
            foreach (Condition cond in Conditions)
            {
                if (cond.variables["sentinel"] == sentinel && cond.variables["package"] ==
                    package && cond.variables["variable"] == name)
                {
                    return true;
                }
            }
            return false;
        }
        public Boolean SetDynamicValue(String sentinel, String package, String name, dynamic dynamicValue)
        {//retourne true si toutes les conditions sont vérifiées
            if (Conditions.Count == 0) return true; //liste vide, toutes conditions vraies
            Boolean isChanged = false;
            foreach (Condition cond in Conditions)
            {
                if (cond.variables["sentinel"] == sentinel && cond.variables["package"] ==
                    package && cond.variables["variable"] == name)
                {
                    cond.DynamicValue = dynamicValue;
                    isChanged = true;
                }
            }
            if(isChanged) return AllConditionsTrue(); //on a modifié des valeures, on vérifie que toutes les variables sont ok
            return false; //on a modifié aucune valeure, la valeure reçue ne correspond à aucune condition stockée
        }
        private Boolean AllConditionsTrue()
        {
            foreach (Condition cond in Conditions)
            {
                if (!cond.IsTrue) return false;
            }
            return true;
        }
        public Algorithme(List<Condition> conditions, List<Execution> executions, String name = null, Boolean active = false)
        {       
            IsActive = active;
            Conditions = conditions;
            Executions = executions;
            if (name == null) this.CreateHashName();
            else Name = name;
        }        
        public String toString(Boolean displayOnOneLine = true)
        {
            String str = $"Nom : {Name}, actif : {IsActive}, désactiver après réalisation : {DisableAfterRealisation}, "
                + $"attente minimale : {Waiting}s";
            if (!displayOnOneLine) str += "\n";
            str += $"plage horaire : {Schedule.toString()}";
            if (!displayOnOneLine) str += "\n";
            str += " Conditions : ";
            foreach (Condition condition in Conditions)
            {
                if (!displayOnOneLine) str += "\n";
                str += condition.toString();
            }
            if (!displayOnOneLine) str += "\n";
            str += " Realisations : ";
            foreach (Execution execution in Executions)
            {
                if (!displayOnOneLine) str += "\n";
                str += execution.toString();
            }

            return str;
        }
        public void EnableOrDisable()
        {
            if (IsActive)
            {
                IsActive = false;
            }
            else
            {
                IsActive = true;
            }
        }
        public void AddCondition(Condition condition)
        {
            if (condition != null)
            {
                Conditions.Add(condition);
            }
        }
        public void AddExecution(Execution execution)
        {
            if (execution != null)
            {
                Executions.Add(execution);                
            }
        }        
        public List<Condition> CloneConditions()
        {
            List<Condition> returnedConditions = new List<Condition>();
            foreach (Condition cond in Conditions)
            {
                returnedConditions.Add((Condition)cond.Clone());
            }
            return returnedConditions;
        }
        public List<Execution> CloneExecutions()
        {
            List<Execution> returnedExecutions = new List<Execution>();
            foreach (Execution real in Executions)
            {
                returnedExecutions.Add((Execution)real.Clone());
            }
            return returnedExecutions;
        }
        public object Clone()
        {
            Algorithme returnedAlgorithme = new Algorithme(null, null, Name, IsActive);
            foreach (Condition condition in Conditions)
            {
                if (condition != null)
                {
                    returnedAlgorithme.AddCondition((Condition)condition.Clone());
                }
            }
            foreach (Execution executions in Executions)
            {
                if (executions != null)
                {
                    returnedAlgorithme.AddExecution((Execution)executions.Clone());
                }
            }
            return returnedAlgorithme;
        }
        public void CreateHashName()
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] digest = md5.ComputeHash(Encoding.UTF8.GetBytes(toString()));
            string base64digest = Convert.ToBase64String(digest, 0, digest.Length);
            Name = base64digest;
        }
    }
}
