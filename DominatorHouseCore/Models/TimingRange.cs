#region

using System;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models
{
    /// <summary>
    ///     TimingRange is used to specify the time range such as start time and end time
    /// </summary>
    [ProtoContract]
    public class TimingRange
    {
        // JsonConvert needs to find the parameterless constructor while deserializing, Don't remove this
        public TimingRange()
        {

        }

        // Constructor for initialize the start time and end time to local property
        public TimingRange(TimeSpan startTime, TimeSpan endTime)
        {
            StartTime = startTime;
            EndTime = endTime;
            TimeId = Guid.NewGuid().ToString();
            Module = string.Empty;
        }


        [ProtoMember(1)]
        // Ending time
        public TimeSpan EndTime { get; set; }

        [ProtoMember(2)]
        // starting time
        public TimeSpan StartTime { get; set; }

        public string TimeId { get; set; }


        [ProtoMember(4)]
        // Module specify the module name which is going to run on at particular time
        public string Module { get; set; }
    }
}