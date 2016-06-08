using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace ClassAlgorithm
{
    public enum Operations : short
    {
        Boolean = 0, //bool -> isTrue ?
        Equal = 1, //string/int/float/double : a==b ?
        Different = 2, //string/int/float/double : a!=b ?
        StrictlyLower = 3, //int/float/double : a<b ?
        Lower = 4, //int/float/double : a<=b ?
        StrictlyUpper = 5, //int/float/double : a>b ?
        Upper = 6 //int/float/double : a>=b ?
    }
    public class Condition : ICloneable
    {
        private Dictionary<String, String> m_variables = new Dictionary<String, String>();
        public dynamic Value { get; set; }
        public dynamic DynamicValue { get; set; }
        public Operations OperationTested { get; set; }
        public Boolean IsTrue
        {
            get 
            {
               switch(OperationTested)
                {
                    case Operations.Different:
                        if (DynamicValue != Value) return true;
                        return false;
                    case Operations.Lower:
                        if (DynamicValue <= Value) return true;
                        return false;
                    case Operations.StrictlyLower:
                        if (DynamicValue < Value) return true;
                        return false;
                    case Operations.Upper:
                        if (DynamicValue >= Value) return true;
                        return false;
                    case Operations.StrictlyUpper:
                        if (DynamicValue > Value) return true;
                        return false;
                    default:
                        if (DynamicValue == Value) return true;
                        return false;
                }
            }
        }

        public String toString()
        {
            String str = "variables : ";
            foreach (KeyValuePair<String, String> variable in m_variables)
            {
                str += $"{variable} ";
            }
            switch (OperationTested)
            {
                case Operations.Different:
                    str += " != ";
                    break;
                case Operations.Lower:
                    str += " <= ";
                    break;
                case Operations.StrictlyLower:
                    str += " < ";
                    break;
                case Operations.Upper:
                    str += " >= ";
                    break;
                case Operations.StrictlyUpper:
                    str += " >= ";
                    break;
                default:
                    str += " == ";
                    break;
            }
            str += $"{Value}";
            return str;
        }
        public Dictionary<String, String> variables
        {
            set { m_variables = value; }
            get { return m_variables; }
        }
        public object Clone()
        {
            Condition returnedCondition = new Condition();
            if (m_variables != null)
            {
                returnedCondition.variables = new Dictionary<string, string>(m_variables);
            }
            return returnedCondition;
        }
    }


    public class Execution : ICloneable
    {
        private Dictionary<String, String> m_variables = new Dictionary<String, String>();
        public List<dynamic> Arguments { set; get; }
        public String toString()
        {
            String str = "variables : ";
            foreach (KeyValuePair<String, String> variable in m_variables)
            {
                str += $"{variable} ";
            }
            str += $" arguments : ";
            foreach (dynamic arg in Arguments)
            {
                str += $"{arg}, ";
            }
            return str;
        }
        public Dictionary<String, String> variables
        {
            set { m_variables = value; }
            get { return m_variables; }
        }
        public object Clone()
        {
            Execution returnedExecution = new Execution();
            if (m_variables != null)
            {
                returnedExecution.variables = new Dictionary<string, string>(m_variables);
            }
            if (Arguments != null)
            {
                returnedExecution.Arguments = new List<dynamic>(Arguments);
            }
            return returnedExecution;
        }
    }

    public class Algorithme : ICloneable
    {
        private DateTime m_lastExecution = new DateTime();
        private int m_waiting = 0;
        private Boolean m_activ = false;
        public List<Condition> m_conditions { get; set; }
        public List<Execution> m_executions { get; set; }
        public String Name { set; get; }
        public bool IsTimeRestricted
        {
            get
            {
                if (m_waiting == 0) return true;
                double delta = DateTime.Now.Subtract(m_lastExecution).TotalSeconds;
                if (delta >= m_waiting)
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
        public void ChangeTimeRestriction(int val)
        {
            if (val != 0)
            {
                m_waiting = val;
            }
            else
            {
                m_waiting = 0;
            }

        }
        public Boolean IsActiv()
        {
            return m_activ;
        }
        public Boolean SetDynamicValue(String sentinel, String package, String name, dynamic dynamicValue)
        {//retourne true si toutes les conditions sont vérifiées
            Boolean isChanged = false;
            foreach (Condition cond in m_conditions)
            {
                if (cond.variables["sentinel"] == sentinel && cond.variables["package"] ==
                    package && cond.variables["variable"] == name)
                {
                    cond.DynamicValue = dynamicValue;
                    isChanged = true;
                }
            }
            if(isChanged) return AllConditionsTrue();
            return false;
        }
        private Boolean AllConditionsTrue()
        {
            foreach (Condition cond in m_conditions)
            {
                if (!cond.IsTrue) return false;
            }
            return true;
        }
        public Algorithme(List<Condition> conditions, List<Execution> executions, String name = null, Boolean activ = false)
        {       
            m_activ = activ;
            m_conditions = conditions;
            m_executions = executions;
            if (name == null) this.CreateHashName();
            else Name = name;
        }        
        public String toString(Boolean displayOnOneLine = true)
        {
            String str = $"Nom : {Name}, actif : {m_activ}, attente minimale : {m_waiting}s";
            if (!displayOnOneLine) str += "\n";
            str += " Conditions : ";
            foreach (Condition condition in m_conditions)
            {
                if (!displayOnOneLine) str += "\n";
                str += condition.toString();
            }
            if (!displayOnOneLine) str += "\n";
            str += " Realisations : ";
            foreach (Execution execution in m_executions)
            {
                if (!displayOnOneLine) str += "\n";
                str += execution.toString();
            }

            return str;
        }
        public void EnableOrDisable()
        {
            if (m_activ)
            {
                m_activ = false;
            }
            else
            {
                m_activ = true;
            }
        }
        public void AddCondition(Condition condition)
        {
            if (condition != null)
            {
                m_conditions.Add(condition);
            }
        }
        public void AddExecution(Execution execution)
        {
            if (execution != null)
            {
                m_executions.Add(execution);                
            }
        }        
        public List<Condition> GetConditions()
        {
            List<Condition> returnedConditions = new List<Condition>();
            foreach (Condition cond in m_conditions)
            {
                returnedConditions.Add((Condition)cond.Clone());
            }
            return returnedConditions;
        }
        public List<Execution> GetExecutions()
        {
            List<Execution> returnedExecutions = new List<Execution>();
            foreach (Execution real in m_executions)
            {
                returnedExecutions.Add((Execution)real.Clone());
            }
            return returnedExecutions;
        }
        public object Clone()
        {
            Algorithme returnedAlgorithme = new Algorithme(null, null, Name, m_activ);
            foreach (Condition condition in m_conditions)
            {
                if (condition != null)
                {
                    returnedAlgorithme.AddCondition((Condition)condition.Clone());
                }
            }
            foreach (Execution executions in m_executions)
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
