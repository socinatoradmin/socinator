using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace GramDominatorCore.GDModel
{
    public interface ICloseFriendModel
    {

    }
    [ProtoContract]
    public class CloseFriendModel: ModuleSetting, ICloseFriendModel, IGeneralSettings
    {
        [ProtoMember(1)]
        JobConfiguration IGeneralSettings.JobConfiguration { get; set; }
        #region Set Job Configuration speed
        public JobConfiguration SlowSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(5000, 5000),
            ActivitiesPerHour = new RangeUtilities(208, 208),
            ActivitiesPerWeek = new RangeUtilities(35000,40000),
            ActivitiesPerJob = new RangeUtilities(208, 208),
            DelayBetweenJobs = new RangeUtilities(2, 5),
            DelayBetweenActivity = new RangeUtilities(10,15),
            DelayBetweenAccounts = new RangeUtilities(2, 5)
        };

        public JobConfiguration MediumSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(10000,10000),
            ActivitiesPerHour = new RangeUtilities(416,416),
            ActivitiesPerWeek = new RangeUtilities(70000,80000),
            ActivitiesPerJob = new RangeUtilities(416, 416),
            DelayBetweenJobs = new RangeUtilities(5, 10),
            DelayBetweenActivity = new RangeUtilities(3, 8),
            DelayBetweenAccounts = new RangeUtilities(5, 10)
        };

        public JobConfiguration FastSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(15000, 15000),
            ActivitiesPerHour = new RangeUtilities(625,625),
            ActivitiesPerWeek = new RangeUtilities(150000, 155000),
            ActivitiesPerJob = new RangeUtilities(625, 625),
            DelayBetweenJobs = new RangeUtilities(10,15),
            DelayBetweenActivity = new RangeUtilities(3,6),
            DelayBetweenAccounts = new RangeUtilities(10, 20)
        };


        public JobConfiguration SuperfastSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(20000,20000),
            ActivitiesPerHour = new RangeUtilities(833, 833),
            ActivitiesPerWeek = new RangeUtilities(140000, 145000),
            ActivitiesPerJob = new RangeUtilities(833, 833),
            DelayBetweenJobs = new RangeUtilities(15,20),
            DelayBetweenActivity = new RangeUtilities(2, 4),
            DelayBetweenAccounts = new RangeUtilities(20, 30)
        };
        #endregion
        private bool _IsCheckAllFollowers;
        private bool _IsCheckedCustomFollowerList;
        private string _customFollowerList;
        [ProtoMember(2)]
        public bool IsCheckAllFollowers
        {
            get=> _IsCheckAllFollowers;
            set
            {
                if (value)
                    CustomFollowerList = string.Empty;
                SetProperty(ref _IsCheckAllFollowers, value);
            }
        }
        [ProtoMember(3)]
        public bool IsCheckedCustomFollowerList
        {
            get => _IsCheckedCustomFollowerList;
            set => SetProperty(ref _IsCheckedCustomFollowerList, value);
        }
        [ProtoMember(4)]
        public string CustomFollowerList
        {
            get => _customFollowerList;
            set => SetProperty(ref _customFollowerList, value);
        }
    }
}
