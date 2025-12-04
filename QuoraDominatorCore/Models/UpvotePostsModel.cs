using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using System.Collections.Generic;

namespace QuoraDominatorCore.Models
{
    internal interface IUpvotePostsModel
    {
        RangeUtilities FollowMaxBetween { get; set; }
    }
    public class UpvotePostsModel : ModuleSetting, IUpvotePostsModel, IGeneralSettings
    {
        public JobConfiguration FastSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(266, 400),
            ActivitiesPerHour = new RangeUtilities(26, 40),
            ActivitiesPerWeek = new RangeUtilities(1600, 2400),
            ActivitiesPerJob = new RangeUtilities(33, 50),
            DelayBetweenJobs = new RangeUtilities(81, 122),
            DelayBetweenActivity = new RangeUtilities(15, 30),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration MediumSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(133, 200),
            ActivitiesPerHour = new RangeUtilities(13, 20),
            ActivitiesPerWeek = new RangeUtilities(800, 1200),
            ActivitiesPerJob = new RangeUtilities(16, 25),
            DelayBetweenJobs = new RangeUtilities(73, 110),
            DelayBetweenActivity = new RangeUtilities(30, 60),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration SlowSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(66, 100),
            ActivitiesPerHour = new RangeUtilities(6, 10),
            ActivitiesPerWeek = new RangeUtilities(400, 600),
            ActivitiesPerJob = new RangeUtilities(8, 12),
            DelayBetweenJobs = new RangeUtilities(81, 122),
            DelayBetweenActivity = new RangeUtilities(60, 60),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };


        public JobConfiguration SuperfastSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(400, 600),
            ActivitiesPerHour = new RangeUtilities(40, 60),
            ActivitiesPerWeek = new RangeUtilities(2400, 3600),
            ActivitiesPerJob = new RangeUtilities(50, 75),
            DelayBetweenJobs = new RangeUtilities(65, 97),
            DelayBetweenActivity = new RangeUtilities(7, 15),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };
        JobConfiguration IGeneralSettings.JobConfiguration { get; set; }
        public List<string> ListQueryType { get; set; } = new List<string>();

        public override PostFilterModel PostFilterModel { get; set; } = new PostFilterModel();

        private RangeUtilities _followMaxBetween = new RangeUtilities(40, 50);



        private bool _isCheckPostOwnerFollowerCount;

        public bool IsCheckPostOwnerFollowerCount
        {
            get { return _isCheckPostOwnerFollowerCount; }
            set 
            { if (_isCheckPostOwnerFollowerCount == value)
                    return;
                SetProperty(ref _isCheckPostOwnerFollowerCount , value); 
            }
        }
        private bool _isCheckShareCount;

        public bool IsCheckShareCount
        {
            get { return _isCheckShareCount; }
            set
            {
                if (_isCheckShareCount == value)
                    return;
                SetProperty(ref _isCheckShareCount, value);
            }
        }
        private bool _isCheckViewsCount;

        public bool IsCheckViewsCount
        {
            get { return _isCheckViewsCount; }
            set
            {
                if (_isCheckViewsCount == value)
                    return;
                SetProperty(ref _isCheckViewsCount, value);
            }
        }
        private RangeUtilities _shareCount = new RangeUtilities(10, 20);
        public RangeUtilities ShareCountRange
        {
            get => _shareCount;
            set
            {
                if (value == _shareCount) return;
                SetProperty(ref _shareCount, value);
            }
        }
        private RangeUtilities _viewsCount = new RangeUtilities(100, 200);
        public RangeUtilities ViewsCountRange
        {
            get => _viewsCount;
            set
            {
                if (value == _viewsCount) return;
                SetProperty(ref _viewsCount, value);
            }
        }
        private RangeUtilities _postOwnerFollowerCount = new RangeUtilities(10, 20);
        public RangeUtilities PostOwnerFollowerCount
        {
            get => _postOwnerFollowerCount;
            set
            {
                if (value == _postOwnerFollowerCount) return;
                SetProperty(ref _postOwnerFollowerCount, value);
            }
        }
        public RangeUtilities FollowMaxBetween
        {
            get => _followMaxBetween;
            set
            {
                if (value == _followMaxBetween) return;
                SetProperty(ref _followMaxBetween, value);
            }
        }
        #region Manage BlackListUser
        private bool _isChkGroupBlacklist;

        public bool IsChkUpvotePostGroupBlackList
        {
            get => _isChkGroupBlacklist;
            set
            {
                if (value == _isChkGroupBlacklist) return;
                SetProperty(ref _isChkGroupBlacklist, value);
            }
        }


        private bool _isChkPrivateBlacklist;

        public bool IsChkUpvotePostPrivateBlacklist
        {
            get => _isChkPrivateBlacklist;
            set
            {
                if (value == _isChkPrivateBlacklist) return;
                SetProperty(ref _isChkPrivateBlacklist, value);
            }
        }
        private bool _isChkSkipPrivateBlackList;
        private bool _isChkSkipGroupBlackList;
        public bool IsChkUpvotePostSkipPrivateBlacklist
        {
            get => _isChkSkipPrivateBlackList;
            set
            {
                if (_isChkSkipPrivateBlackList != value)
                    SetProperty(ref _isChkSkipPrivateBlackList, value);
            }
        }

        public bool IsChkUpvotePostSkipGroupBlacklist
        {
            get => _isChkSkipGroupBlackList;
            set
            {
                if (_isChkSkipGroupBlackList != value)
                    SetProperty(ref _isChkSkipGroupBlackList, value);
            }
        }
        #endregion
    }
}
