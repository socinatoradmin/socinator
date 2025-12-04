using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using static RedditDominatorCore.RDModel.ManageBlacklist;

namespace RedditDominatorCore.RDModel
{
    public interface IEditCommentModel
    {
    }

    public class EditCommentModel : ModuleSetting, IEditCommentModel
    {
        private ObservableCollectionBase<EditCommentInfo> _commentDetails =
            new ObservableCollectionBase<EditCommentInfo>();

        private EditCommentInfo _editCommentInfo = new EditCommentInfo();

        private bool _isChkCommentOnceFromEachAccount;

        private bool _isChkPostUniqueCommentOnPostFromEachAccount = true;


        //IsUniqueComment

        private bool _isUniqueComment;

        private List<string> _lstAccounts;

        private List<EditCommentInfo> _lstCommentDetails = new List<EditCommentInfo>();

        private List<string> _lstImportComment = new List<string>();

        private bool _makeCommentAsSpinText;

        private ManageCommentModel _manageCommentModel = new ManageCommentModel();

        public JobConfiguration FastSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(53, 80),
            ActivitiesPerHour = new RangeUtilities(5, 8),
            ActivitiesPerWeek = new RangeUtilities(320, 480),
            ActivitiesPerJob = new RangeUtilities(7, 10),
            DelayBetweenJobs = new RangeUtilities(87, 130),
            DelayBetweenActivity = new RangeUtilities(50, 60),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration MediumSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(33, 50),
            ActivitiesPerHour = new RangeUtilities(3, 5),
            ActivitiesPerWeek = new RangeUtilities(200, 300),
            ActivitiesPerJob = new RangeUtilities(4, 6),
            DelayBetweenJobs = new RangeUtilities(87, 130),
            DelayBetweenActivity = new RangeUtilities(30, 50),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration SlowSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(13, 20),
            ActivitiesPerHour = new RangeUtilities(1, 2),
            ActivitiesPerWeek = new RangeUtilities(80, 120),
            ActivitiesPerJob = new RangeUtilities(2, 3),
            DelayBetweenJobs = new RangeUtilities(88, 132),
            DelayBetweenActivity = new RangeUtilities(20, 30),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };


        public JobConfiguration SuperfastSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(66, 100),
            ActivitiesPerHour = new RangeUtilities(7, 10),
            ActivitiesPerWeek = new RangeUtilities(400, 600),
            ActivitiesPerJob = new RangeUtilities(8, 12),
            DelayBetweenJobs = new RangeUtilities(88, 132),
            DelayBetweenActivity = new RangeUtilities(70, 80),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        [ProtoMember(1)] public override JobConfiguration JobConfiguration { get; set; }

        [ProtoMember(2)]
        public override ObservableCollection<QueryInfo> SavedQueries { get; set; } =
            new ObservableCollection<QueryInfo>();

        [ProtoMember(3)]
        public ObservableCollection<ManageCommentModel> LstManageCommentModel { get; set; } =
            new ObservableCollection<ManageCommentModel>();

        [ProtoMember(4)]
        public ManageCommentModel ManageCommentModel
        {
            get => _manageCommentModel;
            set
            {
                if (value == _manageCommentModel)
                    return;
                SetProperty(ref _manageCommentModel, value);
            }
        }

        [ProtoMember(5)] public OtherConfigModel OtherConfigModel { get; set; } = new OtherConfigModel();

        public List<string> ListQueryType { get; set; } = new List<string>();

        [ProtoMember(6)] public SkipBlacklist SkipBlacklist { get; set; } = new SkipBlacklist();

        [ProtoMember(7)]
        public bool IsUniqueComment
        {
            get => _isUniqueComment;
            set => SetProperty(ref _isUniqueComment, value);
        }

        [ProtoMember(8)]
        public bool IsChkPostUniqueCommentOnPostFromEachAccount
        {
            get => _isChkPostUniqueCommentOnPostFromEachAccount;
            set => SetProperty(ref _isChkPostUniqueCommentOnPostFromEachAccount, value);
        }

        [ProtoMember(9)]
        public bool IsChkCommentOnceFromEachAccount
        {
            get => _isChkCommentOnceFromEachAccount;
            set => SetProperty(ref _isChkCommentOnceFromEachAccount, value);
        }

        [ProtoMember(10)]
        public bool MakeCommentAsSpinText
        {
            get => _makeCommentAsSpinText;
            set => SetProperty(ref _makeCommentAsSpinText, value);
        }

        [ProtoMember(11)]
        public EditCommentInfo EditCommentInfo
        {
            get => _editCommentInfo;
            set => SetProperty(ref _editCommentInfo, value);
        }

        [ProtoMember(12)]
        public List<string> LstAccounts
        {
            get => _lstAccounts;
            set => SetProperty(ref _lstAccounts, value);
        }

        [ProtoMember(13)]
        public ObservableCollectionBase<EditCommentInfo> CommentDetails
        {
            get => _commentDetails;
            set => SetProperty(ref _commentDetails, value);
        }

        [ProtoMember(14)]
        public List<EditCommentInfo> LstCommentDetails
        {
            get => _lstCommentDetails;
            set
            {
                if (_lstCommentDetails != null && _lstCommentDetails == value)
                    return;
                SetProperty(ref _lstCommentDetails, value);
            }
        }

        [ProtoMember(15)]
        public List<string> LstImportComment
        {
            get => _lstImportComment;
            set => SetProperty(ref _lstImportComment, value);
        }
    }
}