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
    public interface ILikeModel
    {
        bool ChkRelikePost { get; set; }
        bool ChkEnableLikeCommentsAfterPostIsLiked { get; set; }
        bool ChkRemovePoorQualitySources { get; set; }
        bool ChkRemoveSourceIfConnectionRatioLower { get; set; }
        RangeUtilities CommentToBeLikeAfterEachLikedPost { get; set; }
    }

    [ProtoContract]
    public class LikeModel : ModuleSetting, ILikeModel, IGeneralSettings
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

        private bool _chkRemoveSourceIfConnectionRatioLower;

        [ProtoMember(8)]
        public bool ChkRemoveSourceIfConnectionRatioLower
        {
            get => _chkRemoveSourceIfConnectionRatioLower;
            set
            {
                if (_chkRemoveSourceIfConnectionRatioLower == value)
                    return;
                SetProperty(ref _chkRemoveSourceIfConnectionRatioLower, value);
            }
        }

        [ProtoMember(9)] public RangeUtilities CommentToBeLikeAfterEachLikedPost { get; set; } = new RangeUtilities();

        #endregion


        #region BlacklistWhiteList

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

        private bool _isNumberOfPostToLike;

        [ProtoMember(14)]
        public bool IsNumberOfPostToLike
        {
            get => _isNumberOfPostToLike;
            set => SetProperty(ref _isNumberOfPostToLike, value);
        }

        private int _maxNumberOfPostPerUserToLike = 1;

        [ProtoMember(15)]
        public int MaxNumberOfPostPerUserToLike
        {
            get => _maxNumberOfPostPerUserToLike;
            set => SetProperty(ref _maxNumberOfPostPerUserToLike, value);
        }

        private int _maxNumberOfPostPerGroupToLike = 1;

        [ProtoMember(16)]
        public int MaxNumberOfPostPerGroupToLike
        {
            get => _maxNumberOfPostPerGroupToLike;
            set => SetProperty(ref _maxNumberOfPostPerGroupToLike, value);
        }

        private bool _isNumberOfGroupPostToLike;

        [ProtoMember(17)]
        public bool IsNumberOfGroupPostToLike
        {
            get => _isNumberOfGroupPostToLike;
            set => SetProperty(ref _isNumberOfGroupPostToLike, value);
        }

        #endregion
    }
}