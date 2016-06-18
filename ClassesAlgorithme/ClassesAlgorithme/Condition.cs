using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassesAlgorithm
{
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
                switch (OperationTested)
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
}
