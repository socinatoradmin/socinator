using DominatorHouseCore.Models;
using DominatorHouseCore.Models.FacebookModels;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDModel.AccountSelectorModel;
using FaceDominatorCore.FDModel.CommonSettings;
using FaceDominatorCore.FDModel.FilterModel;
using FaceDominatorCore.Interface;
using ProtoBuf;
using System.Collections.Generic;

namespace FaceDominatorCore.FDModel.LikerCommentorModel
{

    //    public interface IPostCommentorModel
    //    {
    //        bool IschkUniqueCommentChecked { get; set; }
    //
    //        bool IschkUniqueCommentForEachPostChecked { get; set; }
    //
    //        bool IsMentionUsersChecked { get; set; }
    //
    //
    //    }

    public class PostCommentorModel : ModuleSetting, IPostLikerCommentorModel, IFdPostModel
    {
        private bool _isActionasOwnAccountChecked = true;
        private bool _isActionasPageChecked;

        private string _ownPageUrl;
        private bool _ischkUniqueCommentChecked;
        private bool _ischkUniqueCommentForEachPostChecked;
        private bool _isMentionUsersChecked;





        [ProtoMember(1)]
        public override JobConfiguration JobConfiguration { get; set; }

        [ProtoMember(2)]
        public override PostFilterModel PostFilterModel { get; set; } = new PostFilterModel();

        [ProtoMember(3)]
        public override LikerCommentorConfigModel LikerCommentorConfigModel { get; set; } = new LikerCommentorConfigModel();


        [ProtoMember(4)]
        public override PostLikeCommentorModel PostLikeCommentorModel { get; set; } = new PostLikeCommentorModel();


        [ProtoMember(5)]
        public bool IsActionasOwnAccountChecked
        {
            get { return _isActionasOwnAccountChecked; }
            set
            {
                SetProperty(ref _isActionasOwnAccountChecked, value);
            }
        }
        [ProtoMember(6)]
        public bool IsActionasPageChecked
        {
            get { return _isActionasPageChecked; }
            set
            {
                SetProperty(ref _isActionasPageChecked, value);
                if (!value)
                {
                    OwnPageUrl = string.Empty;
                    ListOwnPageUrl.Clear();
                }
            }
        }


        private List<string> _listOwnPageUrl = new List<string>();

        [ProtoMember(7)]
        public List<string> ListOwnPageUrl
        {
            get { return _listOwnPageUrl; }
            set
            {
                SetProperty(ref _listOwnPageUrl, value);
            }
        }



        [ProtoMember(8)]
        public override List<string> PostLikeCommentOptions { get; set; } = new List<string>();

        [ProtoMember(10)]
        // ReSharper disable once UnusedMember.Global
        public string OwnPageUrl
        {
            get { return _ownPageUrl; }
            set
            {
                SetProperty(ref _ownPageUrl, value);
            }
        }

        [ProtoMember(11)]
        public bool IschkUniqueCommentChecked
        {
            get { return _ischkUniqueCommentChecked; }
            set
            {
                SetProperty(ref _ischkUniqueCommentChecked, value);
            }
        }

        [ProtoMember(12)]
        public bool IschkUniqueCommentForEachPostChecked
        {
            get { return _ischkUniqueCommentForEachPostChecked; }
            set
            {
                SetProperty(ref _ischkUniqueCommentForEachPostChecked, value);
            }
        }

        private SelectOptionModel _selectOptionModel = new SelectOptionModel();

        [ProtoMember(13)]
        public SelectOptionModel SelectOptionModel
        {
            get { return _selectOptionModel; }
            set
            {
                SetProperty(ref _selectOptionModel, value);
            }
        }

        private RangeUtilities _noOfUserMention = new RangeUtilities(0, 5);

        [ProtoMember(14)]
        public RangeUtilities NoOfUserMention
        {
            get { return _noOfUserMention; }
            set
            {
                SetProperty(ref _noOfUserMention, value);
            }
        }

        [ProtoMember(15)]
        public bool IsMentionUsersChecked
        {
            get { return _isMentionUsersChecked; }
            set
            {
                SetProperty(ref _isMentionUsersChecked, value);
            }
        }

        private bool _isLikePostChecked;

        [ProtoMember(16)]
        public bool IsLikePostChecked
        {
            get { return _isLikePostChecked; }
            set
            {
                SetProperty(ref _isLikePostChecked, value);
            }
        }

        private string _uploadComment;
        [ProtoMember(17)]
        public string UploadComment
        {
            get { return _uploadComment; }

            set
            {
                SetProperty(ref _uploadComment, value);
            }
        }

        private List<string> _listComments = new List<string>();

        [ProtoMember(18)]
        public List<string> ListComments
        {
            get { return _listComments; }
            set
            {
                SetProperty(ref _listComments, value);

            }
        }

        private RangeUtilities _delayBetweenCommentsForAfterActivity = new RangeUtilities(1, 2);
        [ProtoMember(19)]
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

        private bool _isSendFriendRequestChked;
        [ProtoMember(20)]
        public bool IsSendFriendRequestChked
        {
            get { return _isSendFriendRequestChked; }
            set
            {
                SetProperty(ref _isSendFriendRequestChked, value);
            }
        }

        private RangeUtilities _maximumCountPerEntity = new RangeUtilities(5, 10);
        [ProtoMember(21)]
        public RangeUtilities MaximumCountPerEntity
        {
            get
            {
                return _maximumCountPerEntity;
            }
            set
            {
                SetProperty(ref _maximumCountPerEntity, value);
            }
        }

        private bool _isPerEntityRangeChecked;

        [ProtoMember(22)]
        public bool IsPerEntityRangeChecked
        {
            get
            {
                return _isPerEntityRangeChecked;
            }
            set
            {
                SetProperty(ref _isPerEntityRangeChecked, value);
            }
        }


        public bool _ischkAllowMultipleComment;

        [ProtoMember(23)]
        public bool IschkAllowMultipleComment
        {
            get
            {
                return _ischkAllowMultipleComment;
            }
            set
            {
                SetProperty(ref _ischkAllowMultipleComment, value);
            }
        }

        private RangeUtilities _maximumCommentPerPost = new RangeUtilities(2, 5);
        [ProtoMember(21)]
        public RangeUtilities MaximumCommentPerPost
        {
            get
            {
                return _maximumCommentPerPost;
            }
            set
            {
                SetProperty(ref _maximumCommentPerPost, value);
            }
        }
        [ProtoMember(22)]
        private bool _ischkUniqueCommentOptionChecked;
        public bool IschkUniqueCommentOptionChecked
        {
            get
            {
                return _ischkUniqueCommentOptionChecked;
            }
            set
            {
                SetProperty(ref _ischkUniqueCommentOptionChecked, value);
            }
        }


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
