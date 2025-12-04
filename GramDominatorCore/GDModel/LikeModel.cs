using System.Collections.Generic;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;
using DominatorHouseCore.Interfaces;
using GramDominatorCore.GDLibrary;

namespace GramDominatorCore.GDModel
{
    public interface ILikeModel
    {
       
    }

    [ProtoContract]

    public class LikeModel : ModuleSetting, ILikeModel, IGeneralSettings
    {
        public List<string> ListQueryType { get; set; } = new List<string>();

        [ProtoMember(1)]
        JobConfiguration IGeneralSettings.JobConfiguration { get; set; }

        [ProtoMember(2)]
        public override UserFilterModel UserFilterModel { get; set; } = new UserFilterModel();

        [ProtoMember(3)]
        public override PostFilterModel PostFilterModel { get; set; } = new PostFilterModel();

        [ProtoMember(4)]
        public override MangeBlacklist.SkipBlacklist SkipBlacklist { get; set; } = new MangeBlacklist.SkipBlacklist();


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


        #region ILikeModel

        //private bool _chkRelikePost;
        //[ProtoMember(5)]
        //public bool ChkRelikePost
        //{
        //    get
        //    {
        //        return _chkRelikePost;
        //    }
        //    set
        //    {
        //        if (value == _chkRelikePost)
        //            return;
        //        SetProperty(ref _chkRelikePost, value);
        //    }
        //}

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

        //private bool _chkRemovePoorQualitySources;
        //[ProtoMember(7)]
        //public bool ChkRemovePoorQualitySources
        //{
        //    get
        //    {
        //        return _chkRemovePoorQualitySources;
        //    }
        //    set
        //    {
        //        if (_chkRemovePoorQualitySources == value)
        //            return;
        //        SetProperty(ref _chkRemovePoorQualitySources, value);
        //    }
        //}

        //private bool _chkRemoveSourceIfFollowRatioLower;
        //[ProtoMember(8)]
        //public bool ChkRemoveSourceIfFollowRatioLower
        //{
        //    get
        //    {
        //        return _chkRemoveSourceIfFollowRatioLower;
        //    }
        //    set
        //    {
        //        if (_chkRemoveSourceIfFollowRatioLower == value)
        //            return;
        //        SetProperty(ref _chkRemoveSourceIfFollowRatioLower, value);
        //    }
        //}
        [ProtoMember(9)]
        public RangeUtilities CommentToBeLikeAfterEachLikedPost { get; set; } = new RangeUtilities();
        //[ProtoMember(10)]
        //public RangeUtilities FollowBackRatio { get; set; } = new RangeUtilities();

        private bool _isCheckedLikeCountPerUser;
        [ProtoMember(12)]
        public bool IsCheckedLikeCountPerUser
        {
            get
            {
                return _isCheckedLikeCountPerUser;
            }
            set
            {
                if (_isCheckedLikeCountPerUser == value)
                    return;
                SetProperty(ref _isCheckedLikeCountPerUser, value);
            }
        }

        private RangeUtilities _likeCountPerUser = new RangeUtilities(1, 1);
        [ProtoMember(13)]
        public RangeUtilities LikeCountPerUser
        {
            get
            {
                return _likeCountPerUser;
            }
            set
            {
                if (_likeCountPerUser == value)
                    return;
                SetProperty(ref _likeCountPerUser, value);
            }
        }


        private RangeUtilities _delayBetweenLikeComments = new RangeUtilities(15,30);
        [ProtoMember(14)]
        public RangeUtilities DelayBetweenLikeComments
        {
            get
            {
                return _delayBetweenLikeComments;
            }
            set
            {
                if (_isAddedToCampaign && _delayBetweenLikeComments == value)
                    return;
                SetProperty(ref _delayBetweenLikeComments, value);
            }

        }

        #endregion

        private bool _isAddedToCampaign;
        [ProtoMember(11)]
        // ReSharper disable once UnusedMember.Global
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
    }
}
