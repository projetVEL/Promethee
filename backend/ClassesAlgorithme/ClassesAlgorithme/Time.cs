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
        public int Day = 1;
        public int Month = 1;

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
        public String toString()
        {
            String str = $"{Day}/{Month} {Hour}H{Minute}min{Second}";
            return str;
        }
        public DateTime ToDate()
        {
            return new DateTime(DateTime.Now.Year, this.Month, this.Day, this.Hour, this.Minute, this.Second);
        }
        public void AddDays(int days)
        {
            DateTime date = this.ToDate();
            DateTime date2 = date.AddDays(days);
            this.Month = date2.Month;
            this.Day = date2.Day;
        }
        public void AddMonths(int months)
        {
            this.Month = (this.Month + 1) % 13;
            if (this.Month == 0) this.Month = 1;
        }
        public void AddHours(int hours)
        {
            Hour += hours;
            if(Hour > 23)
            {
                AddDays(Hour / 24);
                Hour %= 23;
            } 
        }
        public void AddMinutes(int mins)
        {
            Minute += mins;
            if(mins > 59)
            {
                AddHours(Minute/60);
                Minute %= 60;
            }
        }
        public void AddSeconds(int secs)
        {
            Second += secs;
            if (secs > 59)
            {
                AddMinutes(Second / 60);
                Second %= 60;
            }
        }
    }   
}
