using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassesAlgorithm
{
    public class WeekDays : Dictionary<DayOfWeek, Boolean>
    {
        public WeekDays(Boolean sun = true, Boolean mon = true, Boolean tue = true, Boolean wed = true,
            Boolean thu = true, Boolean fri = true, Boolean sat = true)
        {
            this.Add(DayOfWeek.Sunday, sun);
            this.Add(DayOfWeek.Monday, mon);
            this.Add(DayOfWeek.Tuesday, tue);
            this.Add(DayOfWeek.Wednesday, wed);
            this.Add(DayOfWeek.Thursday, thu);
            this.Add(DayOfWeek.Friday, fri);
            this.Add(DayOfWeek.Saturday, sat);
        }
        public Boolean Today
        {
            get
            {
                return this[DateTime.Now.DayOfWeek];
            }
        }
        public int NumberOfDaysTillOk()
        {
            int returnValue = 0;
            Boolean start = false;
            foreach (var item in this)
            {
                if(item.Key == DateTime.Now.DayOfWeek)
                {
                    start = true;
                }
                if(start)
                {
                    if(item.Value && returnValue != 0)
                    {
                        return returnValue;
                    }
                    returnValue++;
                }
            }
            foreach (var item in this)
            {  
                if (item.Value && returnValue != 0)
                {
                    return returnValue;
                }
                returnValue++;                
            }
            return returnValue;
        }
    }
}
