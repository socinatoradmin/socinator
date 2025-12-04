using System.Collections.Generic;
using System.Collections.ObjectModel;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.LDModel.Filters;
using ProtoBuf;

namespace LinkedDominatorCore.LDModel.GrowConnection
{
   
    public class SendInvitationToGroupMemberModel : ModuleSetting, IGeneralSettings
    {
        private bool _IsCheckedLangKeyCustomAdminGroupList;

        private bool _IsCheckedGroupSource;

        private bool _IsChkGroupBlackList;

        private bool _IsChkPrivateBlackList;

        private bool _IsChkSkipBlackListedUser;

        private bool _IsUniqueOperationChecked;

        private string _UrlInput;

        private List<string> _UrlList = new List<string>();


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


        [ProtoMember(3)] public override LDUserFilterModel LDUserFilterModel { get; set; } = new LDUserFilterModel();

        [ProtoMember(4)]
        public bool IsChkSkipBlackListedUser
        {
            get => _IsChkSkipBlackListedUser;
            set => SetProperty(ref _IsChkSkipBlackListedUser, value);
        }

        [ProtoMember(5)]
        public bool IsChkPrivateBlackList
        {
            get => _IsChkPrivateBlackList;
            set => SetProperty(ref _IsChkPrivateBlackList, value);
        }

        [ProtoMember(6)]
        public bool IsChkGroupBlackList
        {
            get => _IsChkGroupBlackList;
            set => SetProperty(ref _IsChkGroupBlackList, value);
        }

        [ProtoMember(7)]
        public bool IsUniqueOperationChecked
        {
            get => _IsUniqueOperationChecked;
            set => SetProperty(ref _IsUniqueOperationChecked, value);
        }

        [ProtoMember(8)]
        public bool IsCheckedGroupSource
        {
            get => _IsCheckedGroupSource;
            set => SetProperty(ref _IsCheckedGroupSource, value);
        }

        [ProtoMember(9)]
        public string UrlInput
        {
            get => _UrlInput;
            set
            {
                if (_UrlInput == value)
                    return;
                SetProperty(ref _UrlInput, value);
            }
        }

        [ProtoMember(10)]
        public bool IsCheckedLangKeyCustomAdminGroupList
        {
            get => _IsCheckedLangKeyCustomAdminGroupList;
            set
            {
                if (value == _IsCheckedLangKeyCustomAdminGroupList) return;
                SetProperty(ref _IsCheckedLangKeyCustomAdminGroupList, value);
            }
        }

        [ProtoMember(11)]
        public List<string> UrlList
        {
            get => _UrlList;
            set => SetProperty(ref _UrlList, value);
        }

        [ProtoMember(2)] public override JobConfiguration JobConfiguration { get; set; } = new JobConfiguration();
    }
}
