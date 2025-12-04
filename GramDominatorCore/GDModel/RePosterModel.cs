using System.Collections.Generic;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;
using GramDominatorCore.GDLibrary;

namespace GramDominatorCore.GDModel
{
    public interface IRePoster
    {
        //bool ChkEnableLikeCommentsAfterPostIsLiked { get; set; }
        //bool ChkRemovePoorQualitySources { get; set; }
        //bool ChkRemoveSourceIfFollowRatioLower { get; set; }
        //RangeUtilities LikeRange { get; set; }
        //RangeUtilities FollowBackRatio { get; set; }
        //bool IsChkRelikePost { get; set; }
        //bool IsCheckedRepostCountPerUser { get; set; }
        //RangeUtilities RepostCountPerUser { get; set; }
    }
    [ProtoContract]
    public class RePosterModel : ModuleSetting, IRePoster, IGeneralSettings
    {
        private bool _isCheckedCropMedia;
        private List<string> _MediaResolution = new List<string> { "Original", "1:1", "4:5", "16:9" };
        private string _SelectedResolution = "Original";
        //public RePosterModel()
        //{
        //    //ListQueryType = Enum.GetNames(typeof(GdPostQuery)).ToList();
        //}

        public List<string> ListQueryType { get; set; } = new List<string>();

        [ProtoMember(2)]
        public override UserFilterModel UserFilterModel { get; set; } = new UserFilterModel();

        [ProtoMember(3)]
        public override PostFilterModel PostFilterModel { get; set; } = new PostFilterModel();

        [ProtoMember(4)]
        JobConfiguration IGeneralSettings.JobConfiguration { get; set; }

        [ProtoMember(10)]
        public override MangeBlacklist.SkipBlacklist SkipBlacklist { get; set; } = new MangeBlacklist.SkipBlacklist();

        #region Set Job Configuration speed
        public JobConfiguration SlowSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(3, 5),
            ActivitiesPerHour = new RangeUtilities(1, 1),
            ActivitiesPerWeek = new RangeUtilities(20, 30),
            ActivitiesPerJob = new RangeUtilities(1, 1),
            DelayBetweenJobs = new RangeUtilities(89, 134),
            DelayBetweenActivity = new RangeUtilities(60, 120),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration MediumSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(6, 10),
            ActivitiesPerHour = new RangeUtilities(1, 1),
            ActivitiesPerWeek = new RangeUtilities(40, 60),
            ActivitiesPerJob = new RangeUtilities(1, 1),
            DelayBetweenJobs = new RangeUtilities(89, 134),
            DelayBetweenActivity = new RangeUtilities(30, 60),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration FastSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(10, 15),
            ActivitiesPerHour = new RangeUtilities(1, 1),
            ActivitiesPerWeek = new RangeUtilities(60, 90),
            ActivitiesPerJob = new RangeUtilities(1, 1),
            DelayBetweenJobs = new RangeUtilities(88, 133),
            DelayBetweenActivity = new RangeUtilities(13, 25),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };


        public JobConfiguration SuperfastSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(13, 20),
            ActivitiesPerHour = new RangeUtilities(1, 2),
            ActivitiesPerWeek = new RangeUtilities(80, 120),
            ActivitiesPerJob = new RangeUtilities(1, 1),
            DelayBetweenJobs = new RangeUtilities(89, 134),
            DelayBetweenActivity = new RangeUtilities(7, 15),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };
        #endregion


        #region IRePoster

        // private bool _chkEnableLikeCommentsAfterPostIsLiked;
        //private bool _chkRemovePoorQualitySources { get; set; }
        // private bool _chkRemoveSourceIfFollowRatioLower { get; set; }
        //  private RangeUtilities _likeRange=new RangeUtilities();
        // private readonly RangeUtilities _followBackRatio=new RangeUtilities();


        //[ProtoMember(5)]
        //public bool ChkEnableLikeCommentsAfterPostIsLiked
        //{
        //    get
        //    {
        //        return _chkEnableLikeCommentsAfterPostIsLiked;
        //    }
        //    set
        //    {
        //        if (value == _chkEnableLikeCommentsAfterPostIsLiked)
        //            return;
        //        SetProperty(ref _chkEnableLikeCommentsAfterPostIsLiked, value);
        //    }
        //}


        //[ProtoMember(6)]
        //public bool ChkRemovePoorQualitySources
        //{
        //    get
        //    {
        //        return _chkRemovePoorQualitySources;
        //    }
        //    set
        //    {
        //        if (value == _chkRemovePoorQualitySources)
        //            return;
        //        SetProperty(ref _chkRemovePoorQualitySources, value);
        //    }
        //}
        //[ProtoMember(7)]
        //public bool ChkRemoveSourceIfFollowRatioLower
        //{
        //    get
        //    {
        //        return _chkRemoveSourceIfFollowRatioLower;
        //    }
        //    set
        //    {
        //        if (value == _chkRemoveSourceIfFollowRatioLower)
        //            return;
        //        SetProperty(ref _chkRemoveSourceIfFollowRatioLower, value);
        //    }
        //}
        //[ProtoMember(8)]
        //public RangeUtilities LikeRange
        //{
        //    get
        //    {
        //        return _likeRange;
        //    }
        //    set {
        //        if (value == _likeRange)
        //            return;
        //        SetProperty(ref _likeRange, value);
        //    }
        //}
        //[ProtoMember(9)]
        //public RangeUtilities FollowBackRatio
        //{
        //    get
        //    {
        //        return _followBackRatio;
        //    }
        //    set
        //    {
        //        if (value == _likeRange)
        //            return;
        //        SetProperty(ref _likeRange, value);
        //    }
        //}

        //private bool _isChkRelikePost;
        //[ProtoMember(10)]
        //public bool IsChkRelikePost
        //{
        //    get
        //    {
        //        return _isChkRelikePost;
        //    }
        //    set
        //    {
        //        if (value == _isChkRelikePost)
        //            return;
        //        SetProperty(ref _isChkRelikePost, value);
        //    }
        //}

        private string _OriginalPostCaptionInputText = string.Empty;
        [ProtoMember(11)]
        public string OriginalPostCaptionInputText
        {
            get
            {
                return _OriginalPostCaptionInputText;
            }
            set
            {
                if (value == _OriginalPostCaptionInputText)
                    return;
                SetProperty(ref _OriginalPostCaptionInputText, value);
            }
        }


        private bool _isCheckedRepostCountPerUser;
        [ProtoMember(12)]
        public bool IsCheckedRepostCountPerUser
        {
            get
            {
                return _isCheckedRepostCountPerUser;
            }
            set
            {
                if (_isCheckedRepostCountPerUser == value)
                    return;
                SetProperty(ref _isCheckedRepostCountPerUser, value);
            }
        }

        private RangeUtilities _repostCountPerUser = new RangeUtilities(2, 3);
        [ProtoMember(13)]
        public RangeUtilities RepostCountPerUser
        {
            get
            {
                return _repostCountPerUser;
            }
            set
            {
                if (_repostCountPerUser == value)
                    return;
                SetProperty(ref _repostCountPerUser, value);
            }
        }


        private string _UserTagInputText = string.Empty;
        [ProtoMember(14)]
        public string UserTagInputText
        {
            get
            {
                return _UserTagInputText;
            }
            set
            {
                if (value == _UserTagInputText)
                    return;
                SetProperty(ref _UserTagInputText, value);
            }
        }
        private bool _isCommentAfterRepost;
        [ProtoMember(15)]
        public bool IsCommentAfterRepost
        {
            get
            {
                return _isCommentAfterRepost;
            }
            set
            {
                if (_isCommentAfterRepost == value)
                    return;
                SetProperty(ref _isCommentAfterRepost, value);
            }
        }

        private string _originalCommentAfterRepostInputText = string.Empty;
        [ProtoMember(16)]
        public string OriginalCommentAfterRepostInputText
        {
            get
            {
                return _originalCommentAfterRepostInputText;
            }
            set
            {
                if (value == _originalCommentAfterRepostInputText)
                    return;
                SetProperty(ref _originalCommentAfterRepostInputText, value);
            }
        }
        private bool _CommentWithPostCaptionAndUsername = false;
        [ProtoMember(17)]
        public bool CommentWithPostCaptionAndUsername
        {
            get
            {
                return _CommentWithPostCaptionAndUsername;
            }
            set
            {
                if (value == _CommentWithPostCaptionAndUsername)
                    return;
                SetProperty(ref _CommentWithPostCaptionAndUsername, value);
            }
        }

        #endregion

        [ProtoMember(18)]
        public bool IsCheckedCropMedias
        {
            get => _isCheckedCropMedia;
            set
            {
                if (_isCheckedCropMedia == value)
                    return;

                SetProperty(ref _isCheckedCropMedia, value);
            }
        }
        [ProtoMember(19)]
        public List<string> MediaResolution
        {
            get => _MediaResolution;
            set
            {
                if (_MediaResolution == value)
                    return;

                SetProperty(ref _MediaResolution, value);
            }
        }
        [ProtoMember(20)]
        public string SelectedResolution
        {
            get => _SelectedResolution;
            set
            {
                if (_SelectedResolution == value)
                    return;

                SetProperty(ref _SelectedResolution, value);
            }
        }
        private bool _RepostAsStory;
        [ProtoMember(21)]
        public bool RepostAsStory
        {
            get => _RepostAsStory;
            set
            {
                if (_RepostAsStory == value)
                    return;
                SetProperty(ref _RepostAsStory, value);
            }
        }
    }
}
