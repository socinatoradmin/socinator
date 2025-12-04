using System.Collections.Generic;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using GramDominatorCore.GDLibrary;
using ProtoBuf;
using DominatorHouseCore.Utility;

namespace GramDominatorCore.GDModel
{
    public interface ILikeCommentModel
    {
        bool ChkRelikePost { get; set; }
        bool ChkEnableLikeCommentsAfterPostIsLiked { get; set; }
        bool ChkRemovePoorQualitySources { get; set; }
        bool ChkRemoveSourceIfFollowRatioLower { get; set; }
        RangeUtilities CommentToBeLikeAfterEachLikedPost { get; set; }
        RangeUtilities FollowBackRatio { get; set; }


    }

    [ProtoContract]
    public class LikeCommentModel : ModuleSetting, ILikeCommentModel, IGeneralSettings
    {

        #region Set Job Configuration speed
        public JobConfiguration SlowSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(66, 100),
            ActivitiesPerHour = new RangeUtilities(6, 10),
            ActivitiesPerWeek = new RangeUtilities(400, 600),
            ActivitiesPerJob = new RangeUtilities(8, 12),
            DelayBetweenJobs = new RangeUtilities(81, 122),
            DelayBetweenActivity = new RangeUtilities(30, 60),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration MediumSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(200, 300),
            ActivitiesPerHour = new RangeUtilities(20, 30),
            ActivitiesPerWeek = new RangeUtilities(1200, 1800),
            ActivitiesPerJob = new RangeUtilities(25, 37),
            DelayBetweenJobs = new RangeUtilities(72, 108),
            DelayBetweenActivity = new RangeUtilities(21, 42),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration FastSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(333, 500),
            ActivitiesPerHour = new RangeUtilities(33, 50),
            ActivitiesPerWeek = new RangeUtilities(2000, 3000),
            ActivitiesPerJob = new RangeUtilities(41, 62),
            DelayBetweenJobs = new RangeUtilities(77, 116),
            DelayBetweenActivity = new RangeUtilities(8, 15),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };


        public JobConfiguration SuperfastSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(533, 800),
            ActivitiesPerHour = new RangeUtilities(53, 80),
            ActivitiesPerWeek = new RangeUtilities(3200, 4800),
            ActivitiesPerJob = new RangeUtilities(66, 100),
            DelayBetweenJobs = new RangeUtilities(73, 110),
            DelayBetweenActivity = new RangeUtilities(8, 15),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };
        #endregion
        public List<string> ListQueryType { get; set; } = new List<string>();

        [ProtoMember(1)]
        JobConfiguration IGeneralSettings.JobConfiguration { get; set; }

        [ProtoMember(2)]
        public override UserFilterModel UserFilterModel { get; set; } = new UserFilterModel();

        [ProtoMember(3)]
        public override PostFilterModel PostFilterModel { get; set; } = new PostFilterModel();

        [ProtoMember(4)]
        public override MangeBlacklist.SkipBlacklist SkipBlacklist { get; set; } = new MangeBlacklist.SkipBlacklist();

        #region ILikeCommentModel

        private bool _chkRelikePost;
        [ProtoMember(5)]
        public bool ChkRelikePost
        {
            get
            {
                return _chkRelikePost;
            }
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
            get
            {
                return _chkEnableLikeCommentsAfterPostIsLiked;
            }
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
            get
            {
                return _chkRemovePoorQualitySources;
            }
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
            get
            {
                return _chkRemoveSourceIfFollowRatioLower;
            }
            set
            {
                if (_chkRemoveSourceIfFollowRatioLower == value)
                    return;
                SetProperty(ref _chkRemoveSourceIfFollowRatioLower, value);
            }
        }


        private RangeUtilities _commentToBeLikeAfterEachLikedPost = new RangeUtilities();

        [ProtoMember(9)]
        public RangeUtilities CommentToBeLikeAfterEachLikedPost
        {
            get
            {
                return _commentToBeLikeAfterEachLikedPost;
            }
            set
            {
                if (_commentToBeLikeAfterEachLikedPost == value)
                    return;
                SetProperty(ref _commentToBeLikeAfterEachLikedPost, value);
            }
        }


        [ProtoMember(10)]
        public RangeUtilities FollowBackRatio { get; set; } = new RangeUtilities();

        #endregion

        private bool _isAddedToCampaign;
        [ProtoMember(11)]
        public bool IsAddedToCampaign
        {
            get
            {
                return _isAddedToCampaign;
            }
            set
            {
                if (_isAddedToCampaign && _isAddedToCampaign == value)
                    return;
                SetProperty(ref _isAddedToCampaign, value);
            }

        }

        private bool _isCheckedCommentPerUser;
        [ProtoMember(12)]
        public bool IsCheckedCommentPerUser
        {
            get
            {
                return _isCheckedCommentPerUser;
            }
            set
            {
                if (_isCheckedCommentPerUser == value)
                    return;
                SetProperty(ref _isCheckedCommentPerUser, value);
            }
        }

        private RangeUtilities _commentCountPerUser = new RangeUtilities(2, 3);
        [ProtoMember(13)]
        public RangeUtilities CommentCountPerUser
        {
            get
            {
                return _commentCountPerUser;
            }
            set
            {
                if (_commentCountPerUser == value)
                    return;
                SetProperty(ref _commentCountPerUser, value);
            }
        }

        private bool _isCheckedCommentPerPost;
        [ProtoMember(14)]
        public bool IsCheckedCommentPerPost
        {
            get
            {
                return _isCheckedCommentPerPost;
            }
            set
            {
                if (_isCheckedCommentPerPost == value)
                    return;
                SetProperty(ref _isCheckedCommentPerPost, value);
            }
        }

        private RangeUtilities _commentCountPerPost = new RangeUtilities(2, 3);
        [ProtoMember(15)]
        public RangeUtilities CommentCountPerPost
        {
            get
            {
                return _commentCountPerPost;
            }
            set
            {
                if (_commentCountPerPost == value)
                    return;
                SetProperty(ref _commentCountPerPost, value);
            }
        }
    }
}
