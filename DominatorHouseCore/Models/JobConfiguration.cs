#region

using System.Collections.Generic;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models
{
    /// <summary>
    ///     Stores schedule of JobProcess and its activities: delays, timing range, limits per job/hour/day/week
    /// </summary>
    [ProtoContract]
    public class JobConfiguration : BindableBase, IJobConfiguration
    {
        private RangeUtilities _activitiesPerDay;
        private RangeUtilities _delayBetweenActivity;
        private RangeUtilities _delayBetweenJobs;
        private RangeUtilities _activitiesPerJob;
        private RangeUtilities _activitiesPerHour;
        private RangeUtilities _activitiesPerWeek;
        private RangeUtilities _delayBetweenAccounts;
        private IncreaseActivityRange _increaseActivitiesEachDay;
        private List<RunningTimes> _runningTime;
        private string _selectedItem = string.Empty;
        private bool _isAdvanceSetting;
        private List<string> _speeds = new List<string>();

        public JobConfiguration()
        {
            //  Delay between each operations (seconds)
            DelayBetweenActivity = new RangeUtilities(0, 0);

            //  Delay between jobs (minutes)
            DelayBetweenJobs = new RangeUtilities(0, 0);

            // Number of <activities> per Job (users)
            ActivitiesPerJob = new RangeUtilities(1, 1);

            // Number of <activities> per Hour (users)
            ActivitiesPerHour = new RangeUtilities(1, 1);

            // Number of <activities> per Day (users)
            ActivitiesPerDay = new RangeUtilities(0, 0);

            // Number of <activities> per Week (users)
            ActivitiesPerWeek = new RangeUtilities(0, 0);

            // Delay between Accounts (Seconds)
            DelayBetweenAccounts = new RangeUtilities(0, 0);

            // Increase each day with 10 until it reaches 100 max <activity> per day
            IncreaseActivitiesEachDay = new IncreaseActivityRange(0, 0, false);

            SelectedItem = "Slow";
        }


        #region IJobConfiguration

        [ProtoMember(1)]
        public RangeUtilities DelayBetweenActivity
        {
            get => _delayBetweenActivity;
            set
            {
                if (_delayBetweenActivity == value)
                    return;
                SetProperty(ref _delayBetweenActivity, value);
            }
        }

        [ProtoMember(2)]
        public RangeUtilities DelayBetweenJobs
        {
            get => _delayBetweenJobs;
            set
            {
                if (_delayBetweenJobs == value)
                    return;
                SetProperty(ref _delayBetweenJobs, value);
            }
        }

        [ProtoMember(3)]
        public RangeUtilities ActivitiesPerJob
        {
            get
            {
                if (_activitiesPerJob.StartValue > ActivitiesPerDay.StartValue)
                    ActivitiesPerDay.StartValue = _activitiesPerJob.StartValue;
                if (_activitiesPerJob.EndValue > ActivitiesPerDay.StartValue)
                    ActivitiesPerDay.StartValue = _activitiesPerJob.EndValue;
                if (_activitiesPerJob.StartValue > ActivitiesPerWeek.StartValue)
                    ActivitiesPerWeek.StartValue = _activitiesPerJob.StartValue;
                return _activitiesPerJob;
            }
            set
            {
                if (_activitiesPerJob == value)
                    return;
                SetProperty(ref _activitiesPerJob, value);
            }
        }

        [ProtoMember(4)]
        public RangeUtilities ActivitiesPerHour
        {
            get
            {
                if (_activitiesPerHour.EndValue > _activitiesPerDay.EndValue)
                    _activitiesPerHour.EndValue = _activitiesPerDay.EndValue;
                if (_activitiesPerHour.EndValue > _activitiesPerDay.StartValue)
                    _activitiesPerHour.EndValue = _activitiesPerDay.StartValue;
                if (_activitiesPerHour.StartValue > _activitiesPerDay.StartValue)
                {
                    _activitiesPerHour.StartValue = _activitiesPerDay.StartValue;
                    _activitiesPerHour.EndValue = _activitiesPerDay.StartValue;
                }

                return _activitiesPerHour;
            }
            set
            {
                if (_activitiesPerHour == value)
                    return;
                SetProperty(ref _activitiesPerHour, value);
            }
        }

        [ProtoMember(5)]
        public RangeUtilities ActivitiesPerDay
        {
            get
            {
                if (_activitiesPerDay.StartValue > ActivitiesPerWeek.StartValue)
                    ActivitiesPerWeek.StartValue = _activitiesPerDay.StartValue;
                if (_activitiesPerDay.EndValue > ActivitiesPerWeek.StartValue)
                    ActivitiesPerWeek.StartValue = _activitiesPerDay.EndValue;
                //if (_activitiesPerHour.EndValue > _activitiesPerDay.StartValue && _activitiesPerDay.StartValue < _activitiesPerJob.EndValue)
                //    _activitiesPerHour.EndValue = _activitiesPerDay.StartValue;

                return _activitiesPerDay;
            }
            set
            {
                if (_activitiesPerDay == value)
                    return;
                SetProperty(ref _activitiesPerDay, value);
            }
        }

        [ProtoMember(6)]
        public RangeUtilities ActivitiesPerWeek
        {
            get => _activitiesPerWeek;
            set
            {
                if (_activitiesPerWeek == value)
                    return;
                SetProperty(ref _activitiesPerWeek, value);
            }
        }

        [ProtoMember(7)]
        public IncreaseActivityRange IncreaseActivitiesEachDay
        {
            get => _increaseActivitiesEachDay;
            set
            {
                if (_increaseActivitiesEachDay == value)
                    return;
                SetProperty(ref _increaseActivitiesEachDay, value);
            }
        }

        // Day of Week and Time interval when Activity will be active
        [ProtoMember(8)]
        public List<RunningTimes> RunningTime
        {
            get => _runningTime;
            set
            {
                if (_runningTime == value)
                    return;
                SetProperty(ref _runningTime, value);
            }
        }

        [ProtoMember(9)] public string ActivitiesPerJobDisplayName { get; set; } = string.Empty;

        [ProtoMember(10)] public string ActivitiesPerHourDisplayName { get; set; } = string.Empty;

        [ProtoMember(11)] public string ActivitiesPerDayDisplayName { get; set; } = string.Empty;

        [ProtoMember(12)] public string ActivitiesPerWeekDisplayName { get; set; } = string.Empty;

        [ProtoMember(13)] public string IncreaseActivityDisplayName { get; set; } = string.Empty;

        [ProtoMember(14)]
        public bool IsAdvanceSetting
        {
            get => _isAdvanceSetting;
            set
            {
                if (_isAdvanceSetting == value)
                    return;
                SetProperty(ref _isAdvanceSetting, value);
            }
        }

        [ProtoMember(15)]
        public string SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (_selectedItem == value)
                    return;
                SetProperty(ref _selectedItem, value);
            }
        }

        public List<string> Speeds
        {
            get => _speeds;
            set
            {
                if (_speeds == value)
                    return;
                SetProperty(ref _speeds, value);
            }
        }

        [ProtoMember(16)]
        public RangeUtilities DelayBetweenAccounts
        {
            get => _delayBetweenAccounts;
            set
            {
                if (_delayBetweenAccounts == value)
                    return;
                SetProperty(ref _delayBetweenAccounts, value);
            }
        }

        #endregion
    }
}