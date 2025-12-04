using System.Collections.Generic;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace LinkedDominatorCore.LDModel.Messenger
{
    [ProtoContract]
    public class DeleteConversationsModel : ModuleSetting, IGeneralSettings
    {
        private bool _isAllUser = true;
        private bool _IsChkGroupBlackList;
        private bool _IsChkPrivateBlackList;
        private bool _IsChkSkipBlackListedUser;
        private bool _isCustomUser;
        private string _urlInput = string.Empty;
        private List<string> _urlList = new List<string>();

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

        [ProtoMember(2)] public OtherConfigModel OtherConfigModel { get; set; } = new OtherConfigModel();

        [ProtoMember(3)]
        public bool IsChkSkipBlackListedUser
        {
            get => _IsChkSkipBlackListedUser;
            set => SetProperty(ref _IsChkSkipBlackListedUser, value);
        }

        [ProtoMember(4)]
        public bool IsChkPrivateBlackList
        {
            get => _IsChkPrivateBlackList;
            set => SetProperty(ref _IsChkPrivateBlackList, value);
        }

        [ProtoMember(5)]
        public bool IsChkGroupBlackList
        {
            get => _IsChkGroupBlackList;
            set => SetProperty(ref _IsChkGroupBlackList, value);
        }

        [ProtoMember(6)]
        public bool IsCustomUser
        {
            get => _isCustomUser;
            set => SetProperty(ref _isCustomUser, value);
        }

        [ProtoMember(7)]
        public bool IsAllUser
        {
            get => _isAllUser;
            set => SetProperty(ref _isAllUser, value);
        }

        [ProtoMember(8)]
        public string UrlInput
        {
            get => _urlInput;
            set => SetProperty(ref _urlInput, value);
        }

        [ProtoMember(9)]
        public List<string> UrlList
        {
            get => _urlList;
            set => SetProperty(ref _urlList, value);
        }

        [ProtoMember(1)] JobConfiguration IGeneralSettings.JobConfiguration { get; set; } = new JobConfiguration();
    }
}