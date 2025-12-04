using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;
using System.Collections.Generic;

namespace YoutubeDominatorCore.YoutubeModels
{
    [ProtoContract]
    public class JobFilterModel : BindableBase, IJobConfiguration
    {
        public JobFilterModel()
        {
            //  Delay between each operations (seconds)
            DelayBetweenActivity = new RangeUtilities(30, 60);

            //  Delay between jobs (minutes)
            DelayBetweenJobs = new RangeUtilities(10, 20);

            // Number of <activities> per Job (users)
            ActivitiesPerJob = new RangeUtilities(10, 20);

            // Number of <activities> per Hour (users)
            ActivitiesPerHour = new RangeUtilities(10, 20);

            // Number of <activities> per Day (users)
            ActivitiesPerDay = new RangeUtilities(50, 60);

            // Number of <activities> per Week (users)
            ActivitiesPerWeek = new RangeUtilities(350, 420);

            // Increase each day with 10 until it reaches 100 max <activity> per day
            IncreaseActivitiesEachDay = new IncreaseActivityRange(10, 100, false);
        }


        #region IJobConfiguration

        [ProtoMember(1)] public RangeUtilities DelayBetweenActivity { get; set; }

        [ProtoMember(2)] public RangeUtilities DelayBetweenJobs { get; set; }

        [ProtoMember(3)] public RangeUtilities ActivitiesPerJob { get; set; }

        [ProtoMember(4)] public RangeUtilities ActivitiesPerHour { get; set; }

        [ProtoMember(5)] public RangeUtilities ActivitiesPerDay { get; set; }

        [ProtoMember(6)] public RangeUtilities ActivitiesPerWeek { get; set; }

        [ProtoMember(7)] public IncreaseActivityRange IncreaseActivitiesEachDay { get; set; }

        // Day of Week and Time interval when Activity will be active
        [ProtoMember(8)] public List<RunningTimes> RunningTime { get; set; }

        [ProtoMember(9)] public string ActivitiesPerJobDisplayName { get; set; } = string.Empty;

        [ProtoMember(10)] public string ActivitiesPerHourDisplayName { get; set; } = string.Empty;

        [ProtoMember(11)] public string ActivitiesPerDayDisplayName { get; set; } = string.Empty;

        [ProtoMember(12)] public string ActivitiesPerWeekDisplayName { get; set; } = string.Empty;

        [ProtoMember(13)] public string IncreaseActivityDisplayName { get; set; } = string.Empty;

        #endregion
    }
}