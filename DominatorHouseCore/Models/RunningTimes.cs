#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models
{
    [ProtoContract]
    public class RunningTimes : BindableBase
    {
        [ProtoMember(1)]
        // Day is used to specify the day of the week            
        public string Day { get; set; }

        [ProtoMember(4)]
        // DayOfWeek is used to specify the day of the week    
        public DayOfWeek DayOfWeek { get; set; }

        private bool _isEnabled;

        [ProtoMember(3)]
        // IsEnabled is whether job configuration are going to run or not
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (value == _isEnabled)
                    return;
                SetProperty(ref _isEnabled, value);
            }
        }


        /// <summary>
        ///     DayWiseRunningTimeSpan property is initialize the running timespam for all days of the week
        /// </summary>
        public static List<RunningTimes> DayWiseRunningTimes
        {
            // Get the all enum values of DayOfWeek and cast them into DayOfWeek type, from that take one by one day
            // initailize the neccessary details such as the job is enabled(IsEnabled),
            // for the day how many time is going to run(Timings) 

            get
            {
                return
                    Enum.GetValues(typeof(DayOfWeek))
                        .Cast<DayOfWeek>()
                        .Select(day =>
                        {
                            var model = new RunningTimes
                            {
                                Day = day.ToString(),
                                DayOfWeek = day,
                                IsEnabled = true
                            };
                            model.Timings.Add(new TimingRange(new TimeSpan(0, 0, 0), new TimeSpan(23, 59, 59)));
                            return model;
                        })
                        // ReSharper disable once ConstantConditionalAccessQualifier
                        ?.ToList();
            }
        }


        private ObservableCollection<TimingRange> _timings = new ObservableCollection<TimingRange>();

        [ProtoMember(2)]
        // Timings which include all the time span when the job is going to start
        public ObservableCollection<TimingRange> Timings
        {
            get => _timings;
            set
            {
                if (value == _timings)
                    return;
                SetProperty(ref _timings, value);
            }
        }


        // AddTimeRange method is used to add the time range of the job configurations
        public void AddTimeRange(TimingRange range)
        {
            Timings.Add(range);
        }
    }

    public class RunningTimeComparer : IComparer<TimingRange>
    {
        public int Compare(TimingRange x, TimingRange y)
        {
            if (x == null || y == null)
                return -1;
            return x.StartTime.CompareTo(y.StartTime);
        }
    }
}