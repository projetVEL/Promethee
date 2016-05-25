using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassesAlgorithme
{
    public class Valeure
    {
        private ValueType m_valeure = null;
        public System.ValueType valeure
        {
            get { return m_valeure; }
            set { m_valeure = value; }
        }
    }
    public class Variables
    {
        private string m_sentinelle = null;
        private string m_package = null;
        private string m_variable = null;
        private Valeure m_valeureVariable = null; 
        public string sentinelle
        {
            get { return m_sentinelle; }
            set { m_sentinelle = value; }
        }
        public string package
        {
            get { return m_package; }
            set { m_package = value; }
        }
        public string variable
        {
            get { return m_variable; }
            set { m_variable = value; }    
        }
        public Valeure valeureVariable
        {
            get { return m_valeureVariable; }
            set { m_valeureVariable = value; }
        }
    }

    public class Condition
    {
    }


    public class Realisation
    {
    }
}
