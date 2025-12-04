using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;
using System.Collections.Generic;

namespace TumblrDominatorCore.Models
{
    public interface IReblogModel
    {
        bool ChkEnableLikeCommentsAfterPostIsLiked { get; set; }
        bool ChkRemoveSourceIfFollowRatioLower { get; set; }
        RangeUtilities CommentToBeLikeAfterEachLikedPost { get; set; }
    }

    public class ReblogModel : ModuleSetting, IReblogModel, IGeneralSettings
    {
        private bool _isAddedToCampaign;
        private bool _isGroupBlacklists;
        private bool _isPrivateBlacklists;

        private bool _isSkipBlacklistsUser;

        public JobConfiguration FastSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(100, 150),
            ActivitiesPerHour = new RangeUtilities(10, 15),
            ActivitiesPerWeek = new RangeUtilities(600, 900),
            ActivitiesPerJob = new RangeUtilities(12, 18),
            DelayBetweenJobs = new RangeUtilities(82, 123),
            DelayBetweenActivity = new RangeUtilities(25, 35)
        };

        public JobConfiguration MediumSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(66, 100),
            ActivitiesPerHour = new RangeUtilities(6, 10),
            ActivitiesPerWeek = new RangeUtilities(400, 600),
            ActivitiesPerJob = new RangeUtilities(8, 12),
            DelayBetweenJobs = new RangeUtilities(81, 122),
            DelayBetweenActivity = new RangeUtilities(30, 40)
        };

        public JobConfiguration SlowSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(30, 50),
            ActivitiesPerHour = new RangeUtilities(3, 5),
            ActivitiesPerWeek = new RangeUtilities(200, 300),
            ActivitiesPerJob = new RangeUtilities(4, 6),
            DelayBetweenJobs = new RangeUtilities(81, 122),
            DelayBetweenActivity = new RangeUtilities(30, 60)
        };


        public JobConfiguration SuperfastSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(166, 250),
            ActivitiesPerHour = new RangeUtilities(16, 25),
            ActivitiesPerWeek = new RangeUtilities(1000, 1500),
            ActivitiesPerJob = new RangeUtilities(20, 31),
            DelayBetweenJobs = new RangeUtilities(72, 123),
            DelayBetweenActivity = new RangeUtilities(30, 40)
        };

        public List<string> ListQueryType { get; set; } = new List<string>();


        [ProtoMember(3)] public override PostFilterModel BlogFilterModel { get; set; } = new PostFilterModel();
        [ProtoMember(12)] public SearchFilterModel SearchFilter { get; set; } = new SearchFilterModel();

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

        [ProtoMember(1)] JobConfiguration IGeneralSettings.JobConfiguration { get; set; }

        #region IReblogModel

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

        [ProtoMember(10)] public RangeUtilities FollowBackRatio { get; set; } = new RangeUtilities();

        #endregion
    }
}