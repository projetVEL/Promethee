using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassesAlgorithm
{
    public class Execution : ICloneable
    {
        public List<dynamic> Arguments { set; get; }
        public String toString()
        {
            String str = "variables : ";
            foreach (KeyValuePair<String, String> variable in Variables)
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
        public Dictionary<String, String> Variables { set; get; }
        public object Clone()
        {
            Execution returnedExecution = new Execution();
            if (Variables != null)
            {
                returnedExecution.Variables = new Dictionary<string, string>(Variables);
            }
            if (Arguments != null)
            {
                returnedExecution.Arguments = new List<dynamic>(Arguments);
            }
            return returnedExecution;
        }
    }
}
