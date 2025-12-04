using System.Collections.Generic;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;
using System.Collections.ObjectModel;
using DominatorHouseCore.Interfaces;
using GramDominatorCore.GDLibrary;

namespace GramDominatorCore.GDModel
{
    interface IComment
    {       
        string MentionUsers { get; set; }        
    }

    [ProtoContract]
    public class CommentModel : ModuleSetting, IComment, IGeneralSettings
    {
       
        public List<string> ListQueryType { get; set; } = new List<string>();

        [ProtoMember(1)]
        public override ObservableCollection<QueryInfo> SavedQueries { get; set; } = new ObservableCollection<QueryInfo>();

        [ProtoMember(2)]
        public override UserFilterModel UserFilterModel { get; set; } = new UserFilterModel();

        [ProtoMember(3)]
        public override PostFilterModel PostFilterModel { get; set; } = new PostFilterModel();

        [ProtoMember(4)]
        JobConfiguration IGeneralSettings.JobConfiguration { get; set; }

        [ProtoMember(20)]
        public override MangeBlacklist.SkipBlacklist SkipBlacklist { get; set; } = new MangeBlacklist.SkipBlacklist();

        [ProtoMember(21)]
        public ObservableCollection<ManageCommentModel> LstDisplayManageCommentModel { get; set; } = new ObservableCollection<ManageCommentModel>();

        public ManageCommentModel ManageCommentModel { get; set; } = new ManageCommentModel();

        //public ObservableCollection<ManageCommentModel> LstManageCommentModel { get; set; } = new ObservableCollection<ManageCommentModel>();


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


        #region IComment

        private bool _isChkAfterCommentAction;
        [ProtoMember(5)]
        public bool IsChkAfterCommentAction
        {
            get
            {
                return _isChkAfterCommentAction;
            }
            set
            {
                if (_isChkAfterCommentAction == value)
                    return;
                SetProperty(ref _isChkAfterCommentAction, value);
            }

        }
     
        private bool _isChkMentionRandomUsers;
        [ProtoMember(7)]
        public bool IsChkMentionRandomUsers
        {
            get
            {
                return _isChkMentionRandomUsers;
            }
            set
            {
                if (_isChkMentionRandomUsers == value)
                    return;
                SetProperty(ref _isChkMentionRandomUsers, value);
            }

        }

        private bool _isChkMultipleCommentsOnSamePost;
        [ProtoMember(8)]
        public bool IsChkMultipleCommentsOnSamePost
        {
            get
            {
                return _isChkMultipleCommentsOnSamePost;
            }
            set
            {
                if (_isChkMultipleCommentsOnSamePost == value)
                    return;
                SetProperty(ref _isChkMultipleCommentsOnSamePost, value);
            }

        }

        private bool _isChkCommentsOnOwnPost;
        [ProtoMember(9)]
        public bool IsChkCommentsOnOwnPost
        {
            get
            {
                return _isChkCommentsOnOwnPost;
            }
            set
            {
                if (_isChkCommentsOnOwnPost == value)
                    return;
                SetProperty(ref _isChkCommentsOnOwnPost, value);
            }

        }

        private bool _isChkLikePostAfterComment;
        [ProtoMember(10)]
        public bool IsChkLikePostAfterComment
        {
            get
            {
                return _isChkLikePostAfterComment;
            }
            set
            {
                if (_isChkLikePostAfterComment == value)
                    return;
                SetProperty(ref _isChkLikePostAfterComment, value);
            }

        }

        private bool _isChkFollowUserAfterComment;
        [ProtoMember(11)]
        public bool IsChkFollowUserAfterComment
        {
            get
            {
                return _isChkFollowUserAfterComment;
            }
            set
            {
                if (_isChkFollowUserAfterComment == value)
                    return;
                SetProperty(ref _isChkFollowUserAfterComment, value);
            }

        }
            
        [ProtoMember(15)]
        public RangeUtilities RandomlyGeneratedUsers { get; set; } = new RangeUtilities(1,1);

        private bool _isCheckedCommentPerUser;
        [ProtoMember(22)]
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
        [ProtoMember(23)]
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



        private string _mentionUsers;
        [ProtoMember(24)]
        public string MentionUsers
        {
            get
            {
                return _mentionUsers;
            }
            set
            {
                if (_mentionUsers == value)
                    return;
                SetProperty(ref _mentionUsers, value);
            }
        }

        private RangeUtilities _delayBetweenLikesForAfterActivity = new RangeUtilities(15, 30);
        [ProtoMember(25)]
        public RangeUtilities DelayBetweenLikesForAfterActivity
        {
            get
            {
                return _delayBetweenLikesForAfterActivity;
            }
            set
            {
                if (_delayBetweenLikesForAfterActivity == value)
                    return;
                SetProperty(ref _delayBetweenLikesForAfterActivity, value);
            }
        }


        private RangeUtilities _delayBetweenFollowForAfterActivity = new RangeUtilities(15, 30);
        [ProtoMember(26)]
        public RangeUtilities DelayBetweenFollowForAfterActivity
        {
            get
            {
                return _delayBetweenFollowForAfterActivity;
            }
            set
            {
                if (_delayBetweenFollowForAfterActivity == value)
                    return;
                SetProperty(ref _delayBetweenFollowForAfterActivity, value);
            }
        }

        #endregion
        private bool _IsChkMultipleMentionOnSamePost;
        [ProtoMember(27)]
        public bool IsChkMultipleMentionOnSamePost
        {
            get
            {
                return _IsChkMultipleMentionOnSamePost;
            }
            set
            {
                if (_IsChkMultipleMentionOnSamePost == value)
                    return;
                SetProperty(ref _IsChkMultipleMentionOnSamePost, value);

            }

        }
    }
}
