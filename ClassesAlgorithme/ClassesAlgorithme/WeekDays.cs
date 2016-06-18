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
            Boolean thu =true, Boolean fri = true, Boolean sat = true)
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
    }
}
