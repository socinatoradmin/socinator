using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDModel.CommonSettings;
using FaceDominatorCore.FDModel.FilterModel;
using FaceDominatorCore.FDModel.LikerCommentorModel;
using ProtoBuf;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FaceDominatorCore.FDModel.MessageModel
{
    public interface IMessageToFanpagesModel
    {
        //        bool IsLikePostOfPage { get; set; }
        //        bool IsCommentOnPostOfPage { get; set; }
        //        bool IsUnlikePage { get; set; }
        //
        //        bool IsActionasPageChecked { get; set; }
        //
        //        RangeUtilities UnlikeBetween { get; set; }
        //        RangeUtilities CommentOnPostBetween { get; set; }
        //        RangeUtilities LikePostsBetween { get; set; }

        List<string> ListComments { get; set; }
    }

    public class MessageToFanpagesModel : ModuleSetting, IFanpageLikerModel
    {
        //        private RangeUtilities _commentOnPostBetween = new RangeUtilities();
        //        private bool _isCommentOnPostOfPage;
        //        
        //
        //        private bool _isLikePostOfPage;
        //        private bool _isUnlikePage;



        //        private RangeUtilities _likePostsBetween = new RangeUtilities();
        //        private List<string> _listFanpageCategory = new List<string>();

        [ProtoMember(1)]
        public override ObservableCollection<QueryInfo> SavedQueries { get; set; } = new ObservableCollection<QueryInfo>();


        [ProtoMember(2)]
        public override JobConfiguration JobConfiguration { get; set; }

        //        private RangeUtilities _unlikeBetween = new RangeUtilities();


        public List<string> ListQueryType { get; set; } = new List<string>();



        //        [ProtoMember(3)]
        //        public List<string> ListFanpageCategory
        //        {
        //            get { return _listFanpageCategory; }
        //            set
        //            {
        //                if (_listFanpageCategory != null && _listFanpageCategory == value)
        //                    return;
        //                SetProperty(ref _listFanpageCategory, value);
        //            }
        //        }

        /*[ProtoMember(4)]
        public bool IsLikePostOfPage
        {
            get { return _isLikePostOfPage; }
            set
            {
                if (_isLikePostOfPage == value)
                    return;
                SetProperty(ref _isLikePostOfPage, value);
            }
        }

        [ProtoMember(5)]
        public bool IsUnlikePage
        {
            get { return _isUnlikePage; }
            set
            {
                if (_isUnlikePage == value)
                    return;
                SetProperty(ref _isUnlikePage, value);
            }
        }

        [ProtoMember(6)]
        public bool IsCommentOnPostOfPage
        {
            get { return _isCommentOnPostOfPage; }
            set
            {
                if (_isCommentOnPostOfPage == value)
                    return;
                SetProperty(ref _isCommentOnPostOfPage, value);
            }
        }

        [ProtoMember(7)]
        public RangeUtilities LikePostsBetween
        {
            get { return _likePostsBetween; }
            set
            {
                if (_likePostsBetween != null && _likePostsBetween == value)
                    return;
                SetProperty(ref _likePostsBetween, value);
            }
        }

        [ProtoMember(8)]
        public RangeUtilities CommentOnPostBetween
        {
            get { return _commentOnPostBetween; }
            set
            {
                if (_commentOnPostBetween != null && _commentOnPostBetween == value)
                    return;
                SetProperty(ref _commentOnPostBetween, value);
            }
        }

        [ProtoMember(9)]
        public RangeUtilities UnlikeBetween
        {
            get { return _unlikeBetween; }
            set
            {
                if (_unlikeBetween != null && _unlikeBetween == value)
                    return;
                SetProperty(ref _unlikeBetween, value);
            }
        }*/

        //        private bool _isEnableAdvancedFanPageMode;
        //        [ProtoMember(10)]
        //        public bool IsEnableAdvancedFanPageMode
        //        {
        //            get { return _isEnableAdvancedFanPageMode; }
        //            set {
        //                if (_isEnableAdvancedFanPageMode == value)
        //                    return;
        //                SetProperty(ref _isEnableAdvancedFanPageMode,value);
        //                }
        //        }


        private bool _isChkLikeFanPageLatestPost;
        [ProtoMember(11)]
        public bool IsChkLikeFanPageLatestPost
        {
            get { return _isChkLikeFanPageLatestPost; }
            set
            {
                SetProperty(ref _isChkLikeFanPageLatestPost, value);
            }
        }

        private RangeUtilities _likeBetweenJobs = new RangeUtilities(1, 2);
        [ProtoMember(12)]
        public RangeUtilities LikeBetweenJobs
        {
            get { return _likeBetweenJobs; }
            set
            {
                SetProperty(ref _likeBetweenJobs, value);
            }
        }

        private RangeUtilities _delayBetweenLikesForAfterActivity = new RangeUtilities(1, 2);
        [ProtoMember(13)]
        public RangeUtilities DelayBetweenLikesForAfterActivity
        {
            get
            {
                return _delayBetweenLikesForAfterActivity;
            }

            set
            {
                SetProperty(ref _delayBetweenLikesForAfterActivity, value);
            }
        }


        private bool _chkCommentOnFanPageLatestPostsChecked;
        [ProtoMember(14)]
        public bool ChkCommentOnFanPageLatestPostsChecked
        {
            get
            {
                return _chkCommentOnFanPageLatestPostsChecked;
            }

            set
            {
                SetProperty(ref _chkCommentOnFanPageLatestPostsChecked, value);
            }
        }

        private RangeUtilities _delayBetweenCommentsForAfterActivity = new RangeUtilities(1, 2);
        [ProtoMember(15)]
        public RangeUtilities DelayBetweenCommentsForAfterActivity
        {
            get
            {
                return _delayBetweenCommentsForAfterActivity;
            }

            set
            {
                SetProperty(ref _delayBetweenCommentsForAfterActivity, value);
            }
        }

        private RangeUtilities _commentsPerFanPage = new RangeUtilities(1, 2);
        [ProtoMember(16)]
        public RangeUtilities CommentsPerFanPage
        {
            get
            {
                return _commentsPerFanPage;
            }

            set
            {
                SetProperty(ref _commentsPerFanPage, value);
            }
        }

        private string _uploadComment;
        [ProtoMember(17)]
        public string UploadComment
        {
            get
            {
                return _uploadComment;
            }

            set
            {
                SetProperty(ref _uploadComment, value);
            }
        }

        /*private bool _isChkCommentPercentage;
        [ProtoMember(18)]
        public bool IsChkCommentPercentage
        {
            get
            {
                return _isChkCommentPercentage;
            }

            set
            {
                if (value == _isChkCommentPercentage)
                    return;
                SetProperty(ref _isChkCommentPercentage, value);
            }
        }*/

        /*private int _commentPercentage = 50;
        [ProtoMember(19)]
        public int CommentPercentage
        {
            get
            {
                return _commentPercentage;
            }

            set
            {
                if (value == _commentPercentage)
                    return;
                SetProperty(ref _commentPercentage, value);
            }
        }*/

        private bool _isActionasOwnAccountChecked;
        [ProtoMember(20)]
        public bool IsActionasOwnAccountChecked
        {
            get { return _isActionasOwnAccountChecked; }
            set
            {
                SetProperty(ref _isActionasOwnAccountChecked, value);
            }
        }

        private bool _isActionasPageChecked;
        [ProtoMember(21)]
        public bool IsActionasPageChecked
        {
            get { return _isActionasPageChecked; }
            set
            {
                SetProperty(ref _isActionasPageChecked, value);
            }
        }

        private List<string> _listOwnPageUrl = new List<string>();
        [ProtoMember(22)]
        public List<string> ListOwnPageUrl
        {
            get { return _listOwnPageUrl; }
            set
            {
                SetProperty(ref _listOwnPageUrl, value);
            }
        }


        private string _ownPageUrl;
        [ProtoMember(23)]
        public string OwnPageUrl
        {
            get { return _ownPageUrl; }
            set
            {
                SetProperty(ref _ownPageUrl, value);
            }
        }

        private RangeUtilities _delayBetweenMessagesForAfterActivity = new RangeUtilities(0, 2);
        [ProtoMember(24)]
        public RangeUtilities DelayBetweenMessagesForAfterActivity
        {
            get
            {
                return _delayBetweenMessagesForAfterActivity;
            }

            set
            {
                SetProperty(ref _delayBetweenMessagesForAfterActivity, value);
            }
        }

        private bool _isActionasAllPageChecked;
        [ProtoMember(25)]
        public bool IsActionasAllPageChecked
        {
            get { return _isActionasAllPageChecked; }
            set
            {
                SetProperty(ref _isActionasAllPageChecked, value);
            }
        }


        [ProtoMember(26)]
        public override FdFanpageFilterModel FanpageFilterModel { get; set; } = new FdFanpageFilterModel();


        private List<string> _listComments = new List<string>();

        [ProtoMember(27)]
        public List<string> ListComments
        {
            get { return _listComments; }
            set
            {
                SetProperty(ref _listComments, value);

            }
        }

        private bool _isTagChecked;

        [ProtoMember(28)]
        public bool IsTagChecked
        {
            get
            {
                return _isTagChecked;
            }
            set
            {
                SetProperty(ref _isTagChecked, value);
            }
        }

        private bool _isSpintaxChecked;

        [ProtoMember(29)]
        public bool IsSpintaxChecked
        {
            get
            {
                return _isSpintaxChecked;
            }
            set
            {
                SetProperty(ref _isSpintaxChecked, value);
            }
        }

        private bool _isMessageAsPreview;

        [ProtoMember(29)]
        public bool IsMessageAsPreview
        {
            get
            {
                return _isMessageAsPreview;
            }
            set
            {
                SetProperty(ref _isMessageAsPreview, value);
            }
        }

        [ProtoMember(30)]
        public override ObservableCollection<ManageMessagesModel> LstDisplayManageMessageModel { get; set; } = new ObservableCollection<ManageMessagesModel>();


        [ProtoMember(31)]
        public ManageMessagesModel ManageMessagesModel { get; set; } = new ManageMessagesModel();

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

    }
}