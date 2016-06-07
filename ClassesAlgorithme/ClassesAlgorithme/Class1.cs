using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace ClassesAlgorithme
{
    public class Condition : ICloneable
    {
        private Dictionary<String, String> m_variables = new Dictionary<String, String>();
        private dynamic m_valeure;
        private Boolean m_true = false;

        public dynamic dynamicValue { get; set; }

        public String toString()
        {
            String chaine = "variables : ";
            foreach (KeyValuePair<String, String> variable in m_variables)
            {
                chaine += $"{variable} ";
            }
            chaine += $" valeure : {m_valeure}";
            return chaine;
        }
        public dynamic Valeure
        {
            set { m_valeure = value; }
            get { return m_valeure; }
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


    public class Realisation : ICloneable
    {
        private Dictionary<String, String> m_variables = new Dictionary<String, String>();
        private List<dynamic> m_arguments = null;
        public List<dynamic> Arguments
        {
            set { m_arguments = value; }
            get { return m_arguments; }
        }
        public String toString()
        {
            String chaine = "variables : ";
            foreach (KeyValuePair<String, String> variable in m_variables)
            {
                chaine += $"{variable} ";
            }
            chaine += $" arguments : ";
            foreach (dynamic arg in m_arguments)
            {
                chaine += $"{arg}, ";
            }
            return chaine;
        }
        public Dictionary<String, String> variables
        {
            set { m_variables = value; }
            get { return m_variables; }
        }
        public object Clone()
        {
            Realisation returnedRealisation = new Realisation();
            if (m_variables != null)
            {
                returnedRealisation.variables = new Dictionary<string, string>(m_variables);
            }
            if (m_arguments != null)
            {
                returnedRealisation.Arguments = new List<dynamic>(m_arguments);
            }
            return returnedRealisation;
        }
    }
    public class Algorithme : ICloneable
    {
        private DateTime m_derniereRealisation = new DateTime();
        private int m_attentes = 0;
        private String m_name = "NoName";
        private Boolean m_actif = false;
        public List<Condition> m_conditions { get; set; }
        public List<Realisation> m_realisations { get; set; }
        public bool estRestreintHorairement
        {
            get
            {
                if (m_attentes == 0) return true;
                if (m_derniereRealisation.Year == 1)
                {
                    m_derniereRealisation = DateTime.Now;
                    return false;
                }
                double delta = DateTime.Now.Subtract(m_derniereRealisation).TotalSeconds;
                if (delta >= m_attentes)
                {                    
                    return false;
                }
                return true;
            }
        }

        public void resetDerniereRealisation()
        {
            m_derniereRealisation = DateTime.Now;
        }
        public void changeRestrictionHoraire(int val)
        {
            if (val != 0)
            {
                m_attentes = val;
            }
            else
            {
                m_attentes = 0;
            }

        }
        public Boolean estActif()
        {
            return m_actif;
        }
        public Boolean setDynamicValue(String sentinelle, String package, String name, dynamic dynamicValue)
        {//retourne true si toutes les conditions sont vérifiées
            foreach (Condition cond in m_conditions)
            {
                if (cond.variables["sentinelle"] == sentinelle && cond.variables["package"] ==
                    package && cond.variables["variable"] == name)
                {
                    cond.dynamicValue = dynamicValue;
                    if (toutesConditionsVraies())
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public Boolean toutesConditionsVraies()
        {
            foreach (Condition cond in m_conditions)
            {
                if (cond.dynamicValue != cond.Valeure) return false;
            }
            return true;
        }
        public Algorithme(List<Condition> conditions, List<Realisation> realisations, String name, Boolean actif = false)
        {
            m_name = name;
            m_actif = actif;
            m_conditions = conditions;
            m_realisations = realisations;
        }
        public String Name
        {
            set { m_name = value; }
            get { return m_name; }
        }
        public String toString(Boolean uneSeuleLigne = true)
        {
            String chaine = $"Nom : {m_name}, actif : {m_actif}, attente minimale : {m_attentes}s";
            if (!uneSeuleLigne) chaine += "\n";
            chaine += " Conditions : ";
            foreach (Condition condition in m_conditions)
            {
                if (!uneSeuleLigne) chaine += "\n";
                chaine += condition.toString();
            }
            if (!uneSeuleLigne) chaine += "\n";
            chaine += " Realisations : ";
            foreach (Realisation condition in m_realisations)
            {
                if (!uneSeuleLigne) chaine += "\n";
                chaine += condition.toString();
            }

            return chaine;
        }
        public void activeOuDesactiveAlgorithme()
        {
            if (m_actif)
            {
                m_actif = false;
            }
            else
            {
                m_actif = true;
            }
        }
        public void addCondition(Condition condition)
        {
            if (condition != null)
            {
                m_conditions.Add(condition);
            }
        }
        public void addRealisation(Realisation realisation)
        {
            if (realisation != null)
            {
                m_realisations.Add(realisation);
            }
        }
        public List<Condition> getConditions()
        {
            List<Condition> returnedConditions = new List<Condition>();
            foreach (Condition cond in m_conditions)
            {
                returnedConditions.Add((Condition)cond.Clone());
            }
            return returnedConditions;
        }
        public List<Realisation> getRealisations()
        {
            List<Realisation> returnedRealisations = new List<Realisation>();
            foreach (Realisation real in m_realisations)
            {
                returnedRealisations.Add((Realisation)real.Clone());
            }
            return returnedRealisations;
        }
        public object Clone()
        {
            Algorithme returnedAlgorithme = new Algorithme(null, null, m_name, m_actif);
            foreach (Condition condition in m_conditions)
            {
                if (condition != null)
                {
                    returnedAlgorithme.addCondition((Condition)condition.Clone());
                }
            }
            foreach (Realisation realisation in m_realisations)
            {
                if (realisation != null)
                {
                    returnedAlgorithme.addRealisation((Realisation)realisation.Clone());
                }
            }
            return returnedAlgorithme;
        }
        public void createHashName()
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] digest = md5.ComputeHash(Encoding.UTF8.GetBytes(toString()));
            string base64digest = Convert.ToBase64String(digest, 0, digest.Length);
            m_name = base64digest;
        }
    }
}
