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
        public String ReactivationPeriode { get; set; } = null;//determine le temps de pause (jusqu'à la prochaine plage horaire :  min, heure, ...)
                                                               //si l'algo est disableAfterExecution

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
            if ((var >= begin && var <= end))
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
        /// <summary>
        /// renvoit le nombre de secondes avant la prochaine plage
        /// </summary>
        /// <param name="inSlope">vaut true si l'on cherche la prochaine plage alors qu'on se trouve dans une plage valide
        /// sinon faut false si l'on cherche la prochaine plage alors qu'on est hors schedule</param>
        /// <returns></returns>
        public double NextSlotBegin(Boolean inSlope = true)
        {
            Time time = new Time();
            time.Second = DateTime.Now.Second;
            time.Minute = DateTime.Now.Minute;
            time.Hour = DateTime.Now.Hour;
            time.Day = DateTime.Now.Day;
            time.Month = DateTime.Now.Month;

            if (inSlope)
            {
                time.Second = Begin.Second;
                time.Minute = Begin.Minute;
                time.Hour = Begin.Hour;
                time.Day = Begin.Day;
                time.Month = Begin.Month;
                switch (ReactivationPeriode)
                {
                    case "Hours":
                        if (inRange((DateTime.Now.Hour + 1) % 24, Begin.Hour, End.Hour) && (DateTime.Now.Hour + 1) % 24 != 0)
                        {
                            time.Hour = (DateTime.Now.Hour + 1) % 24;
                            time.Day = DateTime.Now.Day;
                            time.Month = DateTime.Now.Month;
                        }
                        else
                        {
                            goto case "Days";
                        }
                        break;
                    case "Days":
                        DateTime date = DateTime.Now.AddDays(Week.NumberOfDaysTillOkFromToday());
                        if (inRange(date.Day, Begin.Day, End.Day))
                        {
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
                            if (time.Month % 13 == 0)
                            {
                                time.Month = 1;
                            }
                        }
                        break;
                    default:
                        if (inRange((DateTime.Now.Minute + 1) % 60, Begin.Minute, End.Minute) && (DateTime.Now.Minute + 1) % 60 != 0)
                        {
                            time.Minute = (DateTime.Now.Minute + 1) % 60;
                            time.Hour = DateTime.Now.Hour;
                            time.Day = DateTime.Now.Day;
                            time.Month = DateTime.Now.Month;
                        }
                        else
                        {
                            goto case "Hours";
                        }
                        break;
                }
            }
            else
            {
                time = checkRangeSecondes(time);
            }
            
            DateTime date2 = new DateTime(DateTime.Now.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second);
            double summSeconds = date2.Subtract(DateTime.Now).TotalSeconds;
            if (summSeconds < 0) summSeconds = DateTime.Now.Subtract(date2).TotalSeconds; ;
            return summSeconds;
        }


        private Time checkRangeSecondes(Time time)
        {            
            if(time.Second > End.Second)
            {
                time.AddMinutes(1);
            }
            time.Second = Begin.Second;
            time = checkRangeMinutes(time);
            return time;
        }
        private Time checkRangeMinutes(Time time)
        {
            if (!inRange(time.Minute, Begin.Minute, End.Minute))
            {                
                if (time.Minute > End.Minute)
                {
                    time.AddHours(1);
                }
                time.Minute = Begin.Minute;
            }
            time = checkRangeHours(time);
            return time;
        }
        private Time checkRangeHours(Time time)
        {
            if (!inRange(time.Hour, Begin.Hour, End.Hour))
            {
                time.Minute = Begin.Minute;
                if (time.Hour > End.Hour)
                {
                    time.AddDays(1);
                }
                time.Hour = Begin.Hour;
            }
            time = checkRangeDays(time);
            return time;
        }

        private Time checkRangeDays(Time time)
        {
            if (!inRange(time.Day, Begin.Day, End.Day))
            {
                time.Hour = Begin.Hour;
                time.Minute = Begin.Minute;  
                if (time.Day > End.Day)
                {
                    time.AddMonths(1);                                        
                }
                time.Day = Begin.Day;
            }
            if (!Week.DayOk(time))
            {
                time.Hour = Begin.Hour;
                time.Minute = Begin.Minute;
                time.AddDays(Week.NumberOfDaysTillOk(time));
                time = checkRangeDays(time);
            }
            time = checkRangeMonths(time);
            return time;
        }
        private Time checkRangeMonths(Time time)
        {
            if (!inRange(time.Month, Begin.Month, End.Month))
            {
                time.Day = Begin.Day;
                time.Hour = Begin.Hour;
                time.Minute = Begin.Minute;
                time.Month = Begin.Month;                
            }
            return time;
        }    

    }
}