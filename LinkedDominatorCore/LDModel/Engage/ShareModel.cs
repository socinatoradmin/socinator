using System;
using System.Collections.Generic;
using DominatorHouseCore.Enums.LdQuery;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.LDModel.Filters;
using ProtoBuf;

namespace LinkedDominatorCore.LDModel.Engage
{
    public interface IShareModel
    {
        bool CheckReSharePost { get; set; }
    }

    [ProtoContract]
    public class ShareModel : ModuleSetting, IGeneralSettings
    {
        private bool _isAddedToCampaign;

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

        public List<string> ListQueryType { get; set; } = new List<string>();

        [ProtoMember(2)] public override LDUserFilterModel LDUserFilterModel { get; set; } = new LDUserFilterModel();

        [ProtoMember(3)] public override LDPostFilterModel LDPostFilterModel { get; set; } = new LDPostFilterModel();

        [ProtoMember(10)]
        public bool IsAddedToCampaign
        {
            get => _isAddedToCampaign;
            set => SetProperty(ref _isAddedToCampaign, value);
        }

        [ProtoMember(1)] JobConfiguration IGeneralSettings.JobConfiguration { get; set; }

        public class QueryTypeWithTitle
        {
            public QueryTypeWithTitle(string queryType)
            {
                QueryType = queryType;
            }

            public string QueryType { get; set; }

            public string QueryDisplayName()
            {
                var value = (LDEngageQueryParameters) Enum.Parse(typeof(LDEngageQueryParameters), QueryType);

                var description = value.GetDescriptionAttr().FromResourceDictionary();
                return description;
            }

            public override string ToString()
            {
                return QueryDisplayName();
            }
        }


        #region blacklistwhitelist 

        private bool _IsChkSkipBlackListedUser;

        [ProtoMember(11)]
        public bool IsChkSkipBlackListedUser
        {
            get => _IsChkSkipBlackListedUser;
            set => SetProperty(ref _IsChkSkipBlackListedUser, value);
        }

        private bool _IsChkPrivateBlackList;

        [ProtoMember(12)]
        public bool IsChkPrivateBlackList
        {
            get => _IsChkPrivateBlackList;
            set => SetProperty(ref _IsChkPrivateBlackList, value);
        }

        private bool _IsChkGroupBlackList;

        [ProtoMember(13)]
        public bool IsChkGroupBlackList
        {
            get => _IsChkGroupBlackList;
            set => SetProperty(ref _IsChkGroupBlackList, value);
        }

        #endregion

        #region common part for engage

        private bool _isNumberOfPostToShare;

        [ProtoMember(14)]
        public bool IsNumberOfPostToShare
        {
            get => _isNumberOfPostToShare;
            set => SetProperty(ref _isNumberOfPostToShare, value);
        }

        private int _maxNumberOfPostPerUserToShare = 1;

        [ProtoMember(15)]
        public int MaxNumberOfPostPerUserToShare
        {
            get => _maxNumberOfPostPerUserToShare;
            set => SetProperty(ref _maxNumberOfPostPerUserToShare, value);
        }

        #endregion
    }
}