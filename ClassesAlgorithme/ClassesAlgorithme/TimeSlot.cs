using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassesAlgorithm
{
    public class TimeSlot
    {
        public Tuple<ushort, ushort> Hours { get; set; } = new Tuple<ushort, ushort>(0, 0);
        public Tuple<ushort, ushort> Minutes { get; set; } = new Tuple<ushort, ushort>(0, 0);
        public Tuple<ushort, ushort> Seconds { get; set; } = new Tuple<ushort, ushort>(0, 0);
        public WeekDays Week { get; set; } = new WeekDays();
        public Tuple<ushort, ushort> FromDayToDay { get; set; } = new Tuple<ushort, ushort>(0,0);
        public Tuple<ushort, ushort> FromMonthToMonth { get; set; } = new Tuple<ushort, ushort>(0,0);
        /// <summary>
        /// determine au moment de l'appel si la date actuel respecte les contraintes horaires
        /// </summary>
        /// <returns></returns>
        public Boolean IsInTimeSlot()
        {
            if (
                inRange(DateTime.Now.Month, FromMonthToMonth)
                && Week.Today
                && inRange(DateTime.Now.Day, FromDayToDay)
                && inRange(DateTime.Now.Hour, Hours)
                && inRange(DateTime.Now.Minute, Minutes)
                && inRange(DateTime.Now.Second, Seconds)
                ) return true;
            return false;
        }
        /// <summary>
        /// retourne true si var entre Item1 et 2 ou que item 1 et 2 non définis (0-0)
        /// </summary>
        /// <param name="var">variable a tester</param>
        /// <param name="range">range : de Item1 a Item2</param>
        /// <returns></returns>
        private Boolean inRange(int var, Tuple<ushort, ushort> range)
        {
            if((range.Item1 == 0 && range.Item2 == 0) || (var >= range.Item1 && var <= range.Item2))
            {
                return true;
            }
            return false;
        }
        public String toString()
        {
            String str = $"de {Hours.Item1}H{Minutes.Item1}min{Seconds.Item1} à {Hours.Item2}H{Minutes.Item2}min{Seconds.Item2}"
                + $" du {FromDayToDay.Item1} au {FromDayToDay.Item2}, du mois {FromMonthToMonth.Item1} au mois {FromMonthToMonth.Item2},"
                + " jours de la semaine :";
            foreach(var item in Week)
            {
                if (item.Value) str += $"{item.Key},";
            }
            return str;
        }
        public TimeSlot NextTimeSlot()
        {

        }
    }
}
