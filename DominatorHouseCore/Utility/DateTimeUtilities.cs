#region

using System;
using System.Globalization;
using System.Linq;
using CommonServiceLocator;
using DominatorHouseCore.Models;

#endregion

namespace DominatorHouseCore.Utility
{
    public static class DateTimeUtilities
    {
        private static readonly DateTime DateUtc1970 = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        ///     GetCurrentEpochTime is used to get the epoch value for given date time
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static int GetCurrentEpochTime(this DateTime date)
        {
            return Convert.ToInt32(Math.Floor((date.ToUniversalTime() - DateUtc1970).TotalSeconds));
        }

        public static int ConvertToEpoch(this DateTime date)
        {
            return Convert.ToInt32(Math.Floor((date.ToUniversalTime() - DateUtc1970).TotalSeconds));
        }

        public static DateTime EpochToDateTimeUtc(this int epoch)
        {
            return DateUtc1970.AddSeconds(epoch);
        }

        public static DateTime EpochToDateTimeUtc(this long epoch)
        {
            return DateUtc1970.AddMilliseconds(epoch);
        }

        public static DateTime EpochToDateTimeUtc(this double epoch)
        {
            return DateUtc1970.AddSeconds(epoch);
        }

        public static TimeSpan EpochToTimeSpan(this int epoch)
        {
            return new TimeSpan(0, 0, epoch);
        }

        public static int GetEpochTime()
        {
            var dateProvider = InstanceProvider.GetInstance<IDateProvider>();
            return (int) (dateProvider.UtcNow() - DateUtc1970).TotalSeconds;
        }

        public static double GetEpochTimeMicroSecs()
        {
            var dateProvider = InstanceProvider.GetInstance<IDateProvider>();
            return (dateProvider.UtcNow() - DateUtc1970).TotalSeconds;
        }

        public static int GetTimezoneOffset()
        {
            var dateProvider = InstanceProvider.GetInstance<IDateProvider>();
            return (int)TimeZoneInfo.Local.GetUtcOffset(dateProvider.Now()).TotalSeconds;
        }

        public static DateTime GetStartOfWeek(this DateTime date)
        {
            var num = date.DayOfWeek - DayOfWeek.Sunday;
            if (num < 0)
                num += 7;
            return date.AddDays(-1 * num).Date;
        }

        public static DateTime GetStartTimeOfNextWeek(ModuleConfiguration moduleConfiguration)
        {
            var num = DateTime.Today.DayOfWeek - DayOfWeek.Sunday;
            if (num < 0)
                num += 7;
            var nextWeekStartDate = DateTime.Today.AddDays(-1 * num).Date.AddDays(7);
            foreach (var runningTime in moduleConfiguration.LstRunningTimes)
            {
                if (!runningTime.IsEnabled)
                    continue;

                if (runningTime.Timings.Count <= 0)
                    continue;

                var startTime = nextWeekStartDate.GetDateOfDateTime(runningTime.DayOfWeek);
                var timings = runningTime.Timings.ToList();
                timings.Sort(new RunningTimeComparer());
                nextWeekStartDate = startTime.Add(timings[0].StartTime);
                return nextWeekStartDate;
            }

            return nextWeekStartDate;
        }

        public static DateTime GetStartTimeOfTomorrow(ModuleConfiguration moduleConfiguration)
        {
            var startTimeOfTomorrow = DateTime.Today.AddDays(1);

            for (var i = 1; i < 8; i++)
            {
                var Day = DateTime.Today.AddDays(i).DayOfWeek;
                var runningTimes = moduleConfiguration.LstRunningTimes[(int) Day];
                if (!runningTimes.IsEnabled)
                    continue;

                if (runningTimes.Timings.Count <= 0)
                    continue;
                //var startTime = startTimeOfTomorrow.GetDateOfDateTime(runningTimes.DayOfWeek);
                var timings = runningTimes.Timings.ToList();
                timings.Sort(
                    new RunningTimeComparer()); //Sort the date time based on Start time, so that it picks the nearest Start time.
                startTimeOfTomorrow = DateTime.Today.AddDays(i);
                startTimeOfTomorrow = startTimeOfTomorrow.Add(timings[0].StartTime);
                return startTimeOfTomorrow;
            }

            return startTimeOfTomorrow;
        }

        public static DateTime GetStartTimeOfNextJob(ModuleConfiguration moduleConfiguration,
            int? delayBetweenJob = null)
        {
            try
            {
                var delay = 0;
                if (delayBetweenJob != null)
                    delay = (int) delayBetweenJob;

                //Calculate the start time of next job normally
                var dateProvider = InstanceProvider.GetInstance<IDateProvider>();
                var startTimeOfNextJob = dateProvider.Now().AddMinutes(delay);

                //Get the available running time for today
                var today = DateTime.Today.DayOfWeek;
                var todayIndex = (int) today;
                var runningTimes = moduleConfiguration.LstRunningTimes[todayIndex];
                var timings = runningTimes.Timings.ToList();
                timings.Sort(
                    new RunningTimeComparer()); //Sort the date time based on Start time, so that it picks time in proper order for further foreach calculation
            
                foreach(var time in timings)
                {
                    var expectedStartTime = DateTime.Today.Date.Add(time.StartTime);

                    // if the start time between the timeRange then return the this 'startTimeOfNextJob' to get the job scheduled at
                    if (expectedStartTime <= startTimeOfNextJob && DateTime.Today.Date.Add(time.EndTime) > startTimeOfNextJob)
                        return startTimeOfNextJob;

                    else if (expectedStartTime.DayOfWeek == today && expectedStartTime>=startTimeOfNextJob)
                        return expectedStartTime;
                }

                // if today's any timing didn't match then check for next days first start time
                for (int i = 1; i < 7; i++)
                {
                    //get Nextday first timing
                    var tomorrow = DateTime.Today.AddDays(i);
                    var tomorrowDay = tomorrow.DayOfWeek;
                    var tomorrowIndex = (int)tomorrowDay;
                    var tomorrowRunningTimes = moduleConfiguration.LstRunningTimes[tomorrowIndex];
                    var tomorrowTimings = tomorrowRunningTimes.Timings.ToList();
                    if (tomorrowTimings.Count == 0) // if any day has no timing set then check for its next day timing
                        continue;
                    tomorrowTimings.Sort(new RunningTimeComparer());
                    return tomorrow.Add(tomorrowTimings.First().StartTime);
                }
                
                // Old code for getting schedulled time
                ////Get the remaining time slots available for the day
                //var availableTimingRanges = timings.Where(x =>
                //    DateTime.Today.Date.Add(x.EndTime) > startTimeOfNextJob ||
                //    DateTime.Today.Date.Add(x.StartTime) > startTimeOfNextJob).ToList();
                //if (availableTimingRanges.Count > 0)
                //{
                //    availableTimingRanges.Sort(new RunningTimeComparer());
                //    var calculatedStartTime = DateTime.Today.Add(availableTimingRanges[0].StartTime);
                //    if (calculatedStartTime > startTimeOfNextJob) startTimeOfNextJob = calculatedStartTime;
                //}
                //else
                //{
                //    return startTimeOfNextJob;
                //    // commented the next line temporarily and returning startTimeOfNextJob instead. if there is next job time crossing today's time then let it to take nextday according to its jobConfig. if the selected time will not be between the time of the next day then it will select next run by executing next scheduling login wtitten in line no. 364 in method 'ScheduleActivityForNextJob' in class 'DominatorScheduler' 
                //    //return GetStartTimeOfTomorrow(moduleConfiguration);//If no time slot is available for the day, calculate the start time for tomorrow
                //}

                return startTimeOfNextJob;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return DateTime.MinValue;
            }
        }

        public static DateTime GetStartTimeForHourly(ModuleConfiguration moduleConfiguration,
            int? delayBetweenJob = null)
        {
            var dateProvider = InstanceProvider.GetInstance<IDateProvider>();
            var minutes = 60 - dateProvider.Now().Minute; //To get the remaining minutes for completion of current hour.
            return GetStartTimeOfNextJob(moduleConfiguration, minutes);
        }

        public static DateTime GetDateOfDateTime(this DateTime startDate, DayOfWeek requiredDay)
        {
            for (var countIndex = 0; countIndex < 7; countIndex++)
            {
                var currentDate = startDate.AddDays(countIndex);
                if (currentDate.DayOfWeek == requiredDay)
                    return currentDate;
            }

            return startDate;
        }

        public static long GetCurrentEpochTimeMilliSeconds(this DateTime date)
        {
            return Convert.ToInt64(Math.Floor((date.ToUniversalTime() - DateUtc1970).TotalMilliseconds));
        }

        public static DateTime GetNextStartTime(ModuleConfiguration moduleConfiguration,
            ReachedLimitType reachedLimitType, int? delayBetweenJob = null)
        {
            var dateProvider = InstanceProvider.GetInstance<IDateProvider>();
            var nextStartTime = dateProvider.Now();
            switch (reachedLimitType)
            {
                case ReachedLimitType.Weekly:
                    return GetStartTimeOfNextWeek(moduleConfiguration);
                case ReachedLimitType.Daily:
                    return GetStartTimeOfTomorrow(moduleConfiguration);
                case ReachedLimitType.Hourly:
                    return GetStartTimeForHourly(moduleConfiguration);
                case ReachedLimitType.Job:
                    return GetStartTimeOfNextJob(moduleConfiguration, delayBetweenJob);
                case ReachedLimitType.NoLimit:
                default:
                    throw new ArgumentOutOfRangeException(nameof(reachedLimitType), reachedLimitType, null);
            }
        }

        /// <summary>
        ///     Get Next Start time for stop time reached
        /// </summary>
        /// <param name="moduleConfiguration"></param>
        /// <returns></returns>
        public static DateTime GetNextStartTime(ModuleConfiguration moduleConfiguration)
        {
            var dateProvider = InstanceProvider.GetInstance<IDateProvider>();
            var nextStartTime = dateProvider.Now();
            var nextRunningDate = DateTime.Now.Date;

            try
            {
                var currentDayTimings = moduleConfiguration.LstRunningTimes
                    .Where(x => x.DayOfWeek == nextStartTime.DayOfWeek)
                    .FirstOrDefault()
                    ?.Timings;

                var nextRunningTimeForToday =
                    currentDayTimings.FirstOrDefault(x => x.StartTime >= nextStartTime.TimeOfDay);

                var nextDayRunTimes =
                    moduleConfiguration.LstRunningTimes.FirstOrDefault(x =>
                        x.DayOfWeek > nextStartTime.DayOfWeek && x.Timings.Count > 0) ??
                    moduleConfiguration.LstRunningTimes.FirstOrDefault(x =>
                        x.DayOfWeek < nextStartTime.DayOfWeek && x.Timings.Count > 0);

                if (nextRunningTimeForToday != null)
                    nextStartTime = nextRunningDate.Add(nextRunningTimeForToday.StartTime);
                else if (nextDayRunTimes != null)
                    nextStartTime = GetNextWeekday(nextRunningDate, nextDayRunTimes.DayOfWeek)
                        .Add(nextDayRunTimes.Timings.FirstOrDefault().StartTime);
                else if (nextDayRunTimes == null && nextRunningTimeForToday == null)
                    nextStartTime = GetNextWeekday(nextRunningDate.AddDays(1), nextRunningDate.DayOfWeek)
                        .Add(currentDayTimings.FirstOrDefault().StartTime);
            }
            catch (ArgumentException ex)
            {
                ex.DebugLog();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return nextStartTime;
        }


        public static DateTime GetNextWeekday(DateTime start, DayOfWeek day)
        {
            var daysToAdd = ((int) day - (int) start.DayOfWeek + 7) % 7;
            return start.AddDays(daysToAdd);
        }

        public static TimeSpan GetRandomTime(TimeSpan start, TimeSpan end, Random random)
        {
            try
            {
                var totalSeconds = (int) (end - start).TotalSeconds;
                var nextSeconds = random.Next(totalSeconds);
                return start.Add(TimeSpan.FromSeconds(nextSeconds));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return start;
            }
        }

        public static DateTime EpochToDateTimeLocal(this int epoch)
        {
            var dateProvider = InstanceProvider.GetInstance<IDateProvider>();
            return EpochToDateTimeUtc(epoch) + (dateProvider.Now() - dateProvider.UtcNow());
        }

        public static DateTime EpochToDateTimeLocal(this long epoch)
        {
            var dateProvider = InstanceProvider.GetInstance<IDateProvider>();
            return EpochToDateTimeUtc(epoch) + (dateProvider.Now() - dateProvider.UtcNow());
        }

        public static long GetCurrentEpochTimeSeconds(this DateTime date)
        {
            return Convert.ToInt64(Math.Floor((date.ToUniversalTime() - DateUtc1970).TotalSeconds));
        }
        public static string GetCurrentDate(this DateTime DateNow,string DateTimeFormat= "dd-MMM-yyyy")
        {
            //dd-MMM-yyyy hh:mm:yyyy tt
            return DateNow.ToString(DateTimeFormat);
        }
    }
}