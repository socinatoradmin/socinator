using System.Collections.Generic;
using System.Collections.ObjectModel;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.LDModel.Filters;
using ProtoBuf;
using DominatorHouseCore.Models.LinkedinModel;

namespace LinkedDominatorCore.LDModel.GrowConnection
{
    public interface IConnectionRequestModel
    {
        #region IConnectionRequestModel

        bool IsChkAddPersonalNoteChecked { get; set; }

        bool IsChkSpintaxChecked { get; set; }

        bool IsChkTagChecked { get; set; }

        string PersonalNote { get; set; }

        List<string> LstPersonalNote { get; set; }

        #endregion
    }

    [ProtoContract]
    public class ConnectionRequestModel : ModuleSetting, IConnectionRequestModel
    {
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

        [ProtoMember(1)]
        public override ObservableCollection<QueryInfo> SavedQueries { get; set; } =
            new ObservableCollection<QueryInfo>();

        public List<string> ListQueryType { get; set; } = new List<string>();

        [ProtoMember(2)] public override JobConfiguration JobConfiguration { get; set; } = new JobConfiguration();

        [ProtoMember(3)] public override LDUserFilterModel LDUserFilterModel { get; set; } = new LDUserFilterModel();

        public ObservableCollection<ManagePersonalNoteModel> LstDisplayManagePersonalNoteModel { get; set; } =
           new ObservableCollection<ManagePersonalNoteModel>();

        public ManagePersonalNoteModel ManagePersonalNoteModel { get; set; } = new ManagePersonalNoteModel();

        public ObservableCollection<ManagePersonalNoteModel> LstManagePersonalNoteModel { get; set; } =
            new ObservableCollection<ManagePersonalNoteModel>();

        #region Variables

        private bool _IsChkAddPersonalNoteChecked;
        private bool _IsChkSpintaxChecked;
        private bool _IsChkTagChecked;
        private string _PersonalNote;
        private List<string> _LstPersonalNote;
        private bool _IsChkMultilineMessage;

        #endregion

        #region IConnectionRequestModel

        [ProtoMember(4)]
        public bool IsChkAddPersonalNoteChecked
        {
            get => _IsChkAddPersonalNoteChecked;
            set => SetProperty(ref _IsChkAddPersonalNoteChecked, value);
        }

        [ProtoMember(5)]
        public bool IsChkSpintaxChecked
        {
            get => _IsChkSpintaxChecked;
            set => SetProperty(ref _IsChkSpintaxChecked, value);
        }

        [ProtoMember(6)]
        public bool IsChkTagChecked
        {
            get => _IsChkTagChecked;
            set => SetProperty(ref _IsChkTagChecked, value);
        }

        [ProtoMember(7)]
        public string PersonalNote
        {
            get => _PersonalNote;
            set => SetProperty(ref _PersonalNote, value);
        }

        //LstPersonalNote
        public List<string> LstPersonalNote
        {
            get => _LstPersonalNote;
            set => SetProperty(ref _LstPersonalNote, value);
        }

        private bool _IsUniqueOperationChecked;

        [ProtoMember(8)]
        public bool IsUniqueOperationChecked
        {
            get => _IsUniqueOperationChecked;
            set => SetProperty(ref _IsUniqueOperationChecked, value);
        }

        private bool _IsCheckedEnableAutoWithdrawConnectionRequest;

        [ProtoMember(9)]
        public bool IsCheckedEnableAutoWithdrawConnectionRequest
        {
            get => _IsCheckedEnableAutoWithdrawConnectionRequest;
            set => SetProperty(ref _IsCheckedEnableAutoWithdrawConnectionRequest, value);
        }


        private bool _IsCheckedStartWithdrawConnectionRequestWhenLimitReach;

        [ProtoMember(10)]
        public bool IsCheckedStartWithdrawConnectionRequestWhenLimitReach
        {
            get => _IsCheckedStartWithdrawConnectionRequestWhenLimitReach;
            set => SetProperty(ref _IsCheckedStartWithdrawConnectionRequestWhenLimitReach, value);
        }

        private RangeUtilities _StartWithdrawConnectionRequestWhenLimitReach = new RangeUtilities(3500, 4000);

        [ProtoMember(11)]
        public RangeUtilities StartWithdrawConnectionRequestWhenLimitReach
        {
            get => _StartWithdrawConnectionRequestWhenLimitReach;
            set => SetProperty(ref _StartWithdrawConnectionRequestWhenLimitReach, value);
        }

        private bool _IsCheckedConnectionRequestToolGetsTemporaryBlocked;

        [ProtoMember(12)]
        public bool IsCheckedConnectionRequestToolGetsTemporaryBlocked
        {
            get => _IsCheckedConnectionRequestToolGetsTemporaryBlocked;
            set => SetProperty(ref _IsCheckedConnectionRequestToolGetsTemporaryBlocked, value);
        }

        private bool _IsCheckedStartWithdrawConnectionRequestBetween;

        [ProtoMember(13)]
        public bool IsCheckedStartWithdrawConnectionRequestBetween
        {
            get => _IsCheckedStartWithdrawConnectionRequestBetween;
            set => SetProperty(ref _IsCheckedStartWithdrawConnectionRequestBetween, value);
        }

        private RangeUtilities _StartWithdrawConnectionRequestAfter = new RangeUtilities(30, 60);

        [ProtoMember(14)]
        public RangeUtilities StartWithdrawConnectionRequestAfter
        {
            get => _StartWithdrawConnectionRequestAfter;
            set => SetProperty(ref _StartWithdrawConnectionRequestAfter, value);
        }

        private bool _IsChkSkipBlackListedUser;

        [ProtoMember(15)]
        public bool IsChkSkipBlackListedUser
        {
            get => _IsChkSkipBlackListedUser;
            set => SetProperty(ref _IsChkSkipBlackListedUser, value);
        }

        private bool _IsChkPrivateBlackList;

        [ProtoMember(16)]
        public bool IsChkPrivateBlackList
        {
            get => _IsChkPrivateBlackList;
            set => SetProperty(ref _IsChkPrivateBlackList, value);
        }

        private bool _IsChkGroupBlackList;

        [ProtoMember(17)]
        public bool IsChkGroupBlackList
        {
            get => _IsChkGroupBlackList;
            set => SetProperty(ref _IsChkGroupBlackList, value);
        }

        private bool _IsStopSendConnectionRequestOnFailed;

        [ProtoMember(18)]
        public bool IsStopSendConnectionRequestOnFailed
        {
            get => _IsStopSendConnectionRequestOnFailed;
            set => SetProperty(ref _IsStopSendConnectionRequestOnFailed, value);
        }

        private int _StopSendConnectionRequestOnCount = 3;

        [ProtoMember(19)]
        public int StopSendConnectionRequestOnCount
        {
            get => _StopSendConnectionRequestOnCount;
            set => SetProperty(ref _StopSendConnectionRequestOnCount, value);
        }

        private bool _IsCheckedWithoutVisiting;

        [ProtoMember(20)]
        public bool IsCheckedWithoutVisiting
        {
            get => _IsCheckedWithoutVisiting;
            set
            {
                if (_IsCheckedWithoutVisiting == value)
                    return;
                SetProperty(ref _IsCheckedWithoutVisiting, value);
            }
        }
        [ProtoMember(21)]
        public bool IsChkMultilineMessage
        {
            get => _IsChkMultilineMessage;
            set => SetProperty(ref _IsChkMultilineMessage, value);
        }

        #endregion
    }
}