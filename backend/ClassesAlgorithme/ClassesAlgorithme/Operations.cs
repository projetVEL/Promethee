using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassesAlgorithm
{    public enum Operations : short
    {
        Boolean = 0, //bool -> isTrue ?
        Equal = 1, //string/int/float/double : a==b ?
        Different = 2, //string/int/float/double : a!=b ?
        StrictlyLower = 3, //int/float/double : a<b ?
        Lower = 4, //int/float/double : a<=b ?
        StrictlyUpper = 5, //int/float/double : a>b ?
        Upper = 6 //int/float/double : a>=b ?
    }
}
