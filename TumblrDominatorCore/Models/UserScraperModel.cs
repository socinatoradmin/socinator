using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;
using System;
using System.Collections.Generic;

namespace TumblrDominatorCore.Models
{
    public interface IUserScraperModel
    {
        bool ChkRelikePost { get; set; }
        bool ChkEnableLikeCommentsAfterPostIsLiked { get; set; }
        bool ChkRemovePoorQualitySources { get; set; }
        bool ChkRemoveSourceIfFollowRatioLower { get; set; }
        RangeUtilities CommentToBeLikeAfterEachLikedPost { get; set; }
    }

    [ProtoContract]
    public class UserScraperModel : ModuleSetting, IUserScraperModel, IGeneralSettings
    {
        private bool _isGroupBlacklists;
        private bool _isPrivateBlacklists;
        private bool _isSkipBlacklistsUser;

        public JobConfiguration FastSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(500, 750),
            ActivitiesPerHour = new RangeUtilities(50, 75),
            ActivitiesPerWeek = new RangeUtilities(2000, 3000),
            ActivitiesPerJob = new RangeUtilities(60, 90),
            DelayBetweenJobs = new RangeUtilities(52, 70),
            DelayBetweenActivity = new RangeUtilities(25, 35)
        };

        public JobConfiguration MediumSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(250, 400),
            ActivitiesPerHour = new RangeUtilities(25, 45),
            ActivitiesPerWeek = new RangeUtilities(1000, 1200),
            ActivitiesPerJob = new RangeUtilities(40, 60),
            DelayBetweenJobs = new RangeUtilities(48, 66),
            DelayBetweenActivity = new RangeUtilities(30, 40)
        };

        public JobConfiguration SlowSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(150, 200),
            ActivitiesPerHour = new RangeUtilities(16, 25),
            ActivitiesPerWeek = new RangeUtilities(500, 800),
            ActivitiesPerJob = new RangeUtilities(20, 31),
            DelayBetweenJobs = new RangeUtilities(60, 72),
            DelayBetweenActivity = new RangeUtilities(30, 60)
        };


        public JobConfiguration SuperfastSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(666, 1000),
            ActivitiesPerHour = new RangeUtilities(66, 90),
            ActivitiesPerWeek = new RangeUtilities(3000, 5000),
            ActivitiesPerJob = new RangeUtilities(83, 125),
            DelayBetweenJobs = new RangeUtilities(45, 70),
            DelayBetweenActivity = new RangeUtilities(35, 60)
        };

        public bool IsSkipBlacklistsUser
        {
            get => _isSkipBlacklistsUser;

            set
            {
                if (value == _isSkipBlacklistsUser)
                    return;
                SetProperty(ref _isSkipBlacklistsUser, value);
            }
        }

        public bool IsPrivateBlacklists
        {
            get => _isPrivateBlacklists;

            set
            {
                if (value == _isPrivateBlacklists)
                    return;
                SetProperty(ref _isPrivateBlacklists, value);
            }
        }

        public bool IsGroupBlacklists
        {
            get => _isGroupBlacklists;

            set
            {
                if (value == _isGroupBlacklists)
                    return;
                SetProperty(ref _isGroupBlacklists, value);
            }
        }

        public List<string> ListQueryType { get; set; } = new List<string>();

        [ProtoMember(11)]
        public bool IsAddedToCampaign
        {
            get => _isAddedToCampaign;
            set
            {
                if (_isAddedToCampaign && _isAddedToCampaign == value)
                    return;
                SetProperty(ref _isAddedToCampaign, value);
            }
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
                var value = (UserQueryParameters)Enum.Parse(typeof(UserQueryParameters), QueryType);

                var description = value.GetDescriptionAttr().FromResourceDictionary();
                return description;
            }

            public override string ToString()
            {
                return QueryDisplayName();
            }
        }


        [ProtoMember(3)]

        #region ILikeModel

        private bool _chkRelikePost;

        [ProtoMember(5)]
        public bool ChkRelikePost
        {
            get => _chkRelikePost;
            set
            {
                if (value == _chkRelikePost)
                    return;
                SetProperty(ref _chkRelikePost, value);
            }
        }

        private bool _chkEnableLikeCommentsAfterPostIsLiked;

        [ProtoMember(6)]
        public bool ChkEnableLikeCommentsAfterPostIsLiked
        {
            get => _chkEnableLikeCommentsAfterPostIsLiked;
            set
            {
                if (_chkEnableLikeCommentsAfterPostIsLiked == value)
                    return;
                SetProperty(ref _chkEnableLikeCommentsAfterPostIsLiked, value);
            }
        }

        private bool _chkRemovePoorQualitySources;

        [ProtoMember(7)]
        public bool ChkRemovePoorQualitySources
        {
            get => _chkRemovePoorQualitySources;
            set
            {
                if (_chkRemovePoorQualitySources == value)
                    return;
                SetProperty(ref _chkRemovePoorQualitySources, value);
            }
        }

        private bool _chkRemoveSourceIfFollowRatioLower;

        [ProtoMember(8)]
        public bool ChkRemoveSourceIfFollowRatioLower
        {
            get => _chkRemoveSourceIfFollowRatioLower;
            set
            {
                if (_chkRemoveSourceIfFollowRatioLower == value)
                    return;
                SetProperty(ref _chkRemoveSourceIfFollowRatioLower, value);
            }
        }

        [ProtoMember(9)] public RangeUtilities CommentToBeLikeAfterEachLikedPost { get; set; } = new RangeUtilities();

        [ProtoMember(10)]

        #endregion

        private bool _isAddedToCampaign;
    }
}