using System;
using System.Collections.Generic;
using System.Linq;

namespace QuoraDominatorCore.QdUtility
{
    public class QdTimeStampUtility
    {
        public static Dictionary<string , int> dayValueDictionary = new Dictionary<string,int>() { { "Mon",1 },{"Tue",2},{"Wed",3 },{"Thu",4 },{"Fri",5 },{"Sat",6 },{"Sun",7 } };
        public static DateTime ConvertTimestamp(string input)
        {
            if(string.IsNullOrEmpty(input)) return DateTime.Now;
            var splitDate = input.Split(' ');
            if (splitDate.Count() < 2)
            {
                var array = splitDate[0].ToCharArray();
                if (array[1] == 'y')
                {
                    var splitByYear = splitDate[0].ToString().Split('y');
                    var date = DateTime.Now.AddYears(-int.Parse(splitByYear[0]));
                    return date;
                }else if (array[1] == 'h')
                {
                    var splitByYear = splitDate[0].ToString().Split('h');
                    var date = DateTime.Now.AddHours(-int.Parse(splitByYear[0]));
                    return date;
                }
                else
                {
                    int currentDayValue = (int)DateTime.Now.DayOfWeek;
                    if (currentDayValue == 0) currentDayValue = 7;

                    int daysToSubtract = currentDayValue - dayValueDictionary[splitDate[0]];
                    if (daysToSubtract < 0)
                        daysToSubtract += 7;
                    return DateTime.Now.AddDays(-daysToSubtract);
                }
            }
            var year = DateTime.Now.Year;
            var month = Month(splitDate[0]);
            var day = splitDate.Count() == 2 ? int.Parse(splitDate[1].Trim(',')) : 1;
            if (month == 0)
                month = Month("Jan");
            var dt = new DateTime(year, month, day);
            return dt;
        }
        public static DateTime ConvertIntoDate(string input)
        {
            var splitDate = input.Split(' ');
            DateTime currentDate = DateTime.Now;
            var currentYear = currentDate.Year;
            var currentMon = currentDate.ToString("MMM");
            var Day = currentDate.Day;
            if (input.Length == 3)
            {
                var daysBack =  dayValueDictionary[currentDate.ToString("ddd")] - dayValueDictionary[input];
                Day = Day - ((daysBack>0)?daysBack: (7+daysBack));
            }
            else if (input.Length > 3)
            {
                currentYear = (Month(splitDate[0]) > Month(currentMon))? currentYear-1 :currentYear;
                var month = Month(splitDate[0]);
                var day = splitDate.Count() == 2 ? int.Parse(splitDate[1].Trim()) : 1;
                return new DateTime(currentYear, month, day);
            }
            return new DateTime(currentYear, currentDate.Month, Day);
        }
        public static int GetNumOfDaysFromUtc(long utcValue)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddTicks(utcValue * 10);
            DateTime currentDate = DateTime.Now;
            TimeSpan timeDifference = currentDate - dateTime;
            return timeDifference.Days;
        }

        public static int Month(string monthdata)
        {
            var month = 0;
            month = monthdata.Contains("Jan") ? 01 : month;
            month = monthdata.Contains("Feb") ? 02 : month;
            month = monthdata.Contains("Mar") ? 03 : month;
            month = monthdata.Contains("Apr") ? 04 : month;
            month = monthdata.Contains("May") ? 05 : month;
            month = monthdata.Contains("Jun") ? 06 : month;
            month = monthdata.Contains("Jul") ? 07 : month;
            month = monthdata.Contains("Aug") ? 08 : month;
            month = monthdata.Contains("Sep") ? 09 : month;
            month = monthdata.Contains("Oct") ? 10 : month;
            month = monthdata.Contains("Nov") ? 11 : month;
            month = monthdata.Contains("Dec") ? 12 : month;

            return month;
        }
    }
}
