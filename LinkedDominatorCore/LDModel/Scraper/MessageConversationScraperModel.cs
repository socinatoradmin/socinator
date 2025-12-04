using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace LinkedDominatorCore.LDModel.Scraper
{
    [ProtoContract]
    public class MessageConversationScraperModel : ModuleSetting, IGeneralSettings
    {
        private DateTime? _endDateForMessageConversation;
        private bool _isAllFirstConnection = true;
        private bool _isCheckedLangKeyCustomUserList;

        private bool _IsChkGroupBlackList;
        private bool _isChkMessageDateMustBeInSpecificRange;

        private bool _IsChkPrivateBlackList;

        private bool _IsChkSkipBlackListedUser;
        private bool _isDeleteAllConversations;
        private bool _isDonwloadAttachments = true;
        private bool _isDownloadAllAttachmetinOneFolder;
        private bool _isTakeMinimumConnectionConversation;

        private int _NumberOfConnectionConversations;
        private DateTime? _startDateForMessageConversation;
        private string _UrlInput;

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

        public List<string> UrlList = new List<string>();

        [ProtoMember(1)]
        public override ObservableCollection<QueryInfo> SavedQueries { get; set; } =
            new ObservableCollection<QueryInfo>();

        public List<string> ListQueryType { get; set; } = new List<string>();


        [ProtoMember(3)]
        public bool IsAllFirstConnection
        {
            get => _isAllFirstConnection;
            set => SetProperty(ref _isAllFirstConnection, value);
        }


        [ProtoMember(4)]
        public bool IsCheckedLangKeyCustomUserList
        {
            get => _isCheckedLangKeyCustomUserList;
            set => SetProperty(ref _isCheckedLangKeyCustomUserList, value);
        }


        [ProtoMember(5)]
        public string UrlInput
        {
            get => _UrlInput;
            set => SetProperty(ref _UrlInput, value);
        }


        [ProtoMember(6)]
        public DateTime? StartDateForMessageConversation
        {
            get => _startDateForMessageConversation;
            set => SetProperty(ref _startDateForMessageConversation, value);
        }

        [ProtoMember(7)]
        public DateTime? EndDateForMessageConversation
        {
            get => _endDateForMessageConversation;
            set => SetProperty(ref _endDateForMessageConversation, value);
        }

        [ProtoMember(8)]
        public bool IsChkMessageDateMustBeInSpecificRange
        {
            get => _isChkMessageDateMustBeInSpecificRange;
            set => SetProperty(ref _isChkMessageDateMustBeInSpecificRange, value);
        }

        [ProtoMember(9)]
        public bool IsChkSkipBlackListedUser
        {
            get => _IsChkSkipBlackListedUser;
            set
            {
                if (value == _IsChkSkipBlackListedUser)
                    return;
                SetProperty(ref _IsChkSkipBlackListedUser, value);
            }
        }

        [ProtoMember(10)]
        public bool IsChkPrivateBlackList
        {
            get => _IsChkPrivateBlackList;
            set
            {
                if (value == _IsChkPrivateBlackList)
                    return;
                SetProperty(ref _IsChkPrivateBlackList, value);
            }
        }

        public int NumberOfConnectionConversations
        {
            get => _NumberOfConnectionConversations;
            set
            {
                if (value == _NumberOfConnectionConversations)
                    return;
                SetProperty(ref _NumberOfConnectionConversations, value);
            }
        }


        public bool IsDonwloadAttachments
        {
            get => _isDonwloadAttachments;
            set => SetProperty(ref _isDonwloadAttachments, value);
        }

        public bool IsDeleteAllConversations
        {
            get => _isDeleteAllConversations;
            set => SetProperty(ref _isDeleteAllConversations, value);
        }

        public bool IsDownloadAllAttachmetinOneFolder
        {
            get => _isDownloadAllAttachmetinOneFolder;
            set => SetProperty(ref _isDownloadAllAttachmetinOneFolder, value);
        }

        public bool IsTakeMinimumConnectionConversation
        {
            get => _isTakeMinimumConnectionConversation;
            set => SetProperty(ref _isTakeMinimumConnectionConversation, value);
        }

        [ProtoMember(11)]
        public bool IsChkGroupBlackList
        {
            get => _IsChkGroupBlackList;
            set
            {
                if (value == _IsChkGroupBlackList)
                    return;
                SetProperty(ref _IsChkGroupBlackList, value);
            }
        }

        [ProtoMember(2)] public override JobConfiguration JobConfiguration { get; set; } = new JobConfiguration();
    }
}