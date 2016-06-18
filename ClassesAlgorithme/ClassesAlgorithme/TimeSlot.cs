using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassesAlgorithm
{
    public class TimeSlot
    {
        public WeekDays Week { get; set; } = new WeekDays();
        public Time Begin { get; set; } = new Time();
        public Time End { get; set; } = new Time();
        public String ReactivationPeriode { get; set; } = null;

        /// <summary>
        /// determine au moment de l'appel si la date actuel respecte les contraintes horaires
        /// </summary>
        /// <returns></returns>
        public Boolean IsInTimeSlot()
        {
            if (
                inRange(DateTime.Now.Month, Begin.Month, End.Month)
                && Week.Today
                && inRange(DateTime.Now.Day, Begin.Day, End.Day)
                && inRange(DateTime.Now.Hour, Begin.Hour, End.Hour)
                && inRange(DateTime.Now.Minute, Begin.Minute, End.Minute)
                && inRange(DateTime.Now.Second, Begin.Second, End.Second)
                ) return true;
            return false;
        }
        /// <summary>
        /// retourne true si var entre Item1 et 2 ou que item 1 et 2 non définis (0-0)
        /// </summary>
        /// <param name="var">variable a tester</param>
        /// <param name="range">range : de Item1 a Item2</param>
        /// <returns></returns>
        private Boolean inRange(int var, int begin, int end)
        {
            if ((begin == 0 && end == 0) || (var >= begin && var <= end))
            {
                return true;
            }
            return false;
        }
        public String toString()
        {
            String str = $"de {Begin.Hour}H{Begin.Minute}min{Begin.Second} à {End.Hour}H{End.Minute}min{End.Second}"
                + $" du {Begin.Day} au {End.Day}, du mois {Begin.Month} au mois {End.Month},"
                + " jours de la semaine :";
            foreach (var item in Week)
            {
                if (item.Value) str += $"{item.Key},";
            }
            return str;
        }
        public Time NextSlotBegin()
        {
            Time time = new Time();
            time.Second = Begin.Second;
            time.Minute = Begin.Minute;
            time.Hour = Begin.Hour;
            time.Day = DateTime.Now.Day;
            time.Month = DateTime.Now.Month;

            switch (ReactivationPeriode)
            {
                case "Minutes":
                    if (inRange(DateTime.Now.Minute + 1, Begin.Minute, End.Minute))
                    {                        
                            time.Minute = (DateTime.Now.Minute + 1)%60;
                    }
                    else
                    {
                        goto case "Hours";
                    }
                    break;
                case "Hours":
                    if (inRange(DateTime.Now.Hour + 1, Begin.Hour, End.Hour))
                    {
                            time.Hour = (DateTime.Now.Hour + 1)%24;
                    }
                    else
                    {
                        goto case "Days";
                    }
                    break;
                case "Days":
                    if (inRange(DateTime.Now.Day + 1, Begin.Day, End.Day))
                    {
                        DateTime date = DateTime.Now.AddDays(1);
                        time.Day = date.Day;
                        time.Month = date.Month;
                    }
                    else
                    {
                        goto case "Months";
                    }
                    break;               
                case "Months":
                    if (inRange(DateTime.Now.Month + 1, Begin.Month, End.Month))
                    {
                        time.Month = DateTime.Now.Month + 1;
                        if(time.Month % 13 == 0)
                        {
                            time.Month = 1;
                        }
                    }
                    break;
                default:
                    break;
            }

            return time;
        }
    }
}
