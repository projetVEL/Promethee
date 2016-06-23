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
            Boolean allFalse = true;
            foreach(var item in this)
            {//evite le cas all false
                if (item.Value)
                {
                    allFalse = false;
                    break;
                }
            }
            if(allFalse)
            {
                this.Clear();
                this.Add(DayOfWeek.Sunday, true);
                this.Add(DayOfWeek.Monday, true);
                this.Add(DayOfWeek.Tuesday, true);
                this.Add(DayOfWeek.Wednesday, true);
                this.Add(DayOfWeek.Thursday, true);
                this.Add(DayOfWeek.Friday, true);
                this.Add(DayOfWeek.Saturday, true);
            }
        }
        public Boolean Today
        {
            get
            {
                return this[DateTime.Now.DayOfWeek];
            }
        }
        public int NumberOfDaysTillOkFromToday()
        {
            Time time = new Time();
            time.Second = DateTime.Now.Second;
            time.Minute = DateTime.Now.Minute;
            time.Hour = DateTime.Now.Hour;
            time.Day = DateTime.Now.Day;
            time.Month = DateTime.Now.Month;
            return NumberOfDaysTillOk(time);
        }
        public int NumberOfDaysTillOk(Time time)
        {
            int returnValue = 0;
            Boolean start = false;
            DateTime date = time.ToDate();
            foreach (var item in this)
            {
                if(item.Key == date.DayOfWeek)
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
        public Boolean DayOk(Time time)
        {
            DateTime date = time.ToDate();
            return this[date.DayOfWeek];
        }
    }
}
