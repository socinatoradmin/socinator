using DominatorHouseCore.Models;
using DominatorHouseCore.Models.FacebookModels;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDModel.CommonSettings;
using FaceDominatorCore.FDModel.FilterModel;
using FaceDominatorCore.Interface;
using ProtoBuf;
using System.Collections.Generic;

namespace FaceDominatorCore.FDModel.LikerCommentorModel
{

    public interface IPostLikerModel
    {
        string FilePath { get; set; }
    }

    public class PostLikerModel : ModuleSetting, IPostLikerModel, IFdPostModel
    {
        private bool _isActionasOwnAccountChecked = true;
        private bool _isActionasPageChecked;
        private string _ownPageUrl = string.Empty;
        private string _filePath = string.Empty;


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
        public string FilePath
        {
            get { return _filePath; }
            set
            {
                SetProperty(ref _filePath, value);
            }
        }



        [ProtoMember(8)]
        public override List<string> PostLikeCommentOptions { get; set; } = new List<string>();

        [ProtoMember(9)]
        public override SelectAccountDetailsModel SelectAccountDetailsModel { get; set; } = new SelectAccountDetailsModel();

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

        private bool _isSendFriendRequestChked;
        [ProtoMember(11)]
        public bool IsSendFriendRequestChked
        {
            get { return _isSendFriendRequestChked; }
            set
            {
                SetProperty(ref _isSendFriendRequestChked, value);
            }
        }

        private bool _commentOnPostChecked;
        [ProtoMember(12)]
        public bool CommentOnPostChecked
        {
            get { return _commentOnPostChecked; }
            set
            {
                SetProperty(ref _commentOnPostChecked, value);
            }
        }

        private string _uploadComment;
        [ProtoMember(13)]
        public string UploadComment
        {
            get { return _uploadComment; }

            set
            {
                SetProperty(ref _uploadComment, value);
            }
        }

        private List<string> _listComments = new List<string>();

        [ProtoMember(14)]
        public List<string> ListComments
        {
            get { return _listComments; }
            set
            {
                SetProperty(ref _listComments, value);

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

        private RangeUtilities _maximumCountPerEntity = new RangeUtilities(5, 10);
        [ProtoMember(16)]
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

        [ProtoMember(17)]
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

        private bool _isSpintaxChecked;

        [ProtoMember(18)]
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

        public bool IschkAllowMultipleComment { get; set; }


        public RangeUtilities MaximumCommentPerPost { get; set; }




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
