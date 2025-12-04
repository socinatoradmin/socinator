using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDModel.CommonSettings;
using FaceDominatorCore.FDModel.FilterModel;
using ProtoBuf;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FaceDominatorCore.FDModel.GroupsModel
{
    public interface IGroupJoinerModel
    {
        //        bool IsChkStopGroupUnjoinerToolWhenReachChecked { get; set; }
        //
        //        bool IsChkEnableAutoGroupJoinerUnJoinerChecked { get; set; }
        //
        //        RangeUtilities StopFollowToolWhenReach { get; set; }
        //
        //        bool IsUniqueGroupsShouldBeJoinedFromEachAccount { get; set; }
    }

    public class GroupJoinerModel : ModuleSetting, IGroupJoinerModel
    {
        [ProtoMember(1)]
        public override JobConfiguration JobConfiguration { get; set; }

        [ProtoMember(2)]
        public override ObservableCollection<QueryInfo> SavedQueries { get; set; } = new ObservableCollection<QueryInfo>();

        [ProtoMember(3)]
        public override FdGroupFilterModel GroupFilterModel { get; set; } = new FdGroupFilterModel();

        public List<string> ListQueryType { get; set; } = new List<string>();


        private bool _isChkStopGroupUnjoinerToolWhenReachChecked;


        [ProtoMember(4)]
        public bool IsChkStopGroupUnjoinerToolWhenReachChecked
        {
            get
            {
                return _isChkStopGroupUnjoinerToolWhenReachChecked;
            }

            set
            {
                SetProperty(ref _isChkStopGroupUnjoinerToolWhenReachChecked, value);
            }
        }

        private bool _isChkEnableAutoGroupJoinerUnJoinerChecked;

        [ProtoMember(5)]
        public bool IsChkEnableAutoGroupJoinerUnJoinerChecked
        {
            get
            {
                return _isChkEnableAutoGroupJoinerUnJoinerChecked;
            }

            set
            {
                SetProperty(ref _isChkEnableAutoGroupJoinerUnJoinerChecked, value);
            }
        }


        private RangeUtilities _stopFollowToolWhenReach = new RangeUtilities();

        [ProtoMember(6)]

        public RangeUtilities StopFollowToolWhenReach
        {
            get
            {
                return _stopFollowToolWhenReach;
            }

            set
            {
                SetProperty(ref _stopFollowToolWhenReach, value);
            }
        }

        // ReSharper disable once UnusedMember.Global
        public JobConfiguration SlowSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(10, 15),
            ActivitiesPerHour = new RangeUtilities(1, 2),
            ActivitiesPerWeek = new RangeUtilities(60, 90),
            ActivitiesPerJob = new RangeUtilities(1, 2),
            DelayBetweenJobs = new RangeUtilities(88, 133),
            DelayBetweenActivity = new RangeUtilities(30, 60),
            DelayBetweenAccounts = new RangeUtilities(0, 0)

        };

        // ReSharper disable once UnusedMember.Global
        public JobConfiguration MediumSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(27, 40),
            ActivitiesPerHour = new RangeUtilities(3, 4),
            ActivitiesPerWeek = new RangeUtilities(160, 240),
            ActivitiesPerJob = new RangeUtilities(3, 5),
            DelayBetweenJobs = new RangeUtilities(87, 131),
            DelayBetweenActivity = new RangeUtilities(23, 45),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        // ReSharper disable once UnusedMember.Global
        public JobConfiguration FastSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(53, 80),
            ActivitiesPerHour = new RangeUtilities(5, 8),
            ActivitiesPerWeek = new RangeUtilities(320, 480),
            ActivitiesPerJob = new RangeUtilities(7, 10),
            DelayBetweenJobs = new RangeUtilities(87, 130),
            DelayBetweenActivity = new RangeUtilities(15, 30),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };


        // ReSharper disable once IdentifierTypo
        // ReSharper disable once UnusedMember.Global
        public JobConfiguration SuperfastSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(67, 100),
            ActivitiesPerHour = new RangeUtilities(7, 10),
            ActivitiesPerWeek = new RangeUtilities(400, 600),
            ActivitiesPerJob = new RangeUtilities(8, 12),
            DelayBetweenJobs = new RangeUtilities(88, 132),
            DelayBetweenActivity = new RangeUtilities(8, 15),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        private bool _isUniqueGroupsShouldBeJoinedFromEachAccount;

        [ProtoMember(7)]
        public bool IsUniqueGroupsShouldBeJoinedFromEachAccount
        {
            get { return _isUniqueGroupsShouldBeJoinedFromEachAccount; }
            set
            {
                SetProperty(ref _isUniqueGroupsShouldBeJoinedFromEachAccount, value);
            }
        }
    }
}