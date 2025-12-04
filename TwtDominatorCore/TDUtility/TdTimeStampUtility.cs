using System;
using Newtonsoft.Json.Linq;

namespace TwtDominatorCore.TDUtility
{
    public class TdTimeStampUtility
    {
        public static DateTime ConvertTimestamp(Jsonhandler jHand, JToken token)
        {
            try
            {
                var timeStamp = jHand.GetJTokenValue(token, "retweeted_status_result", "result", "legacy", "created_at");
                var dateTime = string.IsNullOrEmpty(timeStamp) ? jHand.GetJTokenValue(token, "created_at") : timeStamp;
                var splitDate = dateTime.Split(' ');

                var year = int.Parse(splitDate[5]);
                var month = Month(splitDate[1]);
                var day = int.Parse(splitDate[2]);
                var time = splitDate[3];
                var splitTime = time.Split(':');
                var hour = int.Parse(splitTime[0]);
                var minute = int.Parse(splitTime[1]);
                var second = int.Parse(splitTime[2]);
                var dt = new DateTime(year, month, day, hour, minute, second);
                dt = dt.ToLocalTime();
                return dt;
            }
            catch { return new DateTime(); }
        }

        public static int Month(string monthdata)
        {
            var month = 0;
            month = monthdata.Contains("Jan") || monthdata.Contains("January") ? 01 : month;
            month = monthdata.Contains("Feb") || monthdata.Contains("February") ? 02 : month;
            month = monthdata.Contains("Mar") || monthdata.Contains("March") ? 03 : month;
            month = monthdata.Contains("Apr") || monthdata.Contains("April") ? 04 : month;
            month = monthdata.Contains("May") || monthdata.Contains("May") ? 05 : month;
            month = monthdata.Contains("Jun") || monthdata.Contains("June") ? 06 : month;
            month = monthdata.Contains("Jul") || monthdata.Contains("July") ? 07 : month;
            month = monthdata.Contains("Aug") || monthdata.Contains("August") ? 08 : month;
            month = monthdata.Contains("Sep") || monthdata.Contains("September") ? 09 : month;
            month = monthdata.Contains("Oct") || monthdata.Contains("October") ? 10 : month;
            month = monthdata.Contains("Nov") || monthdata.Contains("November") ? 11 : month;
            month = monthdata.Contains("Dec") || monthdata.Contains("December") ? 12 : month;

            return month;
        }
    }
}