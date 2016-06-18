using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassesAlgorithm
{
    public class Time
    {
        public int Second = 0;
        public int Minute = 0;
        public int Hour = 0;
        public int Day = 0;
        public int Month = 0;

        public Boolean IsPassed()
        {
            DateTime compareDate = new DateTime();
            compareDate = compareDate.AddYears(DateTime.Now.Year - 1);
            compareDate = compareDate.AddMonths(Month-1);
            compareDate = compareDate.AddDays(Day-1);
            compareDate = compareDate.AddHours(Hour);
            compareDate = compareDate.AddMinutes(Minute);
            compareDate = compareDate.AddSeconds(Second);

            if (DateTime.Now.Subtract(compareDate).Milliseconds >= 0)
            { 
                return true;
            }
            return false;
        }
    }   
}
