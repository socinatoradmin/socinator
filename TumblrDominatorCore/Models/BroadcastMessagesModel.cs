using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TumblrDominatorCore.Models
{
    [ProtoContract]
    public class BroadcastMessagesModel : ModuleSetting, IGeneralSettings
    {
        private bool _isCheckSkipUsersWhoWereAlreadySentAMessageFromTheSoftware;
        private bool _isGroupBlacklists;


        private bool _isPrivateBlacklists;

        private bool _isSkipBlacklistsUser;

        public JobConfiguration FastSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(66, 100),
            ActivitiesPerHour = new RangeUtilities(6, 10),
            ActivitiesPerWeek = new RangeUtilities(300, 500),
            ActivitiesPerJob = new RangeUtilities(9, 14),
            DelayBetweenJobs = new RangeUtilities(47, 80),
            DelayBetweenActivity = new RangeUtilities(25, 35)
        };

        public JobConfiguration MediumSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(33, 50),
            ActivitiesPerHour = new RangeUtilities(5, 6),
            ActivitiesPerWeek = new RangeUtilities(200, 300),
            ActivitiesPerJob = new RangeUtilities(4, 6),
            DelayBetweenJobs = new RangeUtilities(60, 100),
            DelayBetweenActivity = new RangeUtilities(30, 40)
        };

        public JobConfiguration SlowSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(16, 25),
            ActivitiesPerHour = new RangeUtilities(3, 4),
            ActivitiesPerWeek = new RangeUtilities(100, 150),
            ActivitiesPerJob = new RangeUtilities(2, 3),
            DelayBetweenJobs = new RangeUtilities(75, 120),
            DelayBetweenActivity = new RangeUtilities(30, 60)
        };


        public JobConfiguration SuperfastSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(133, 200),
            ActivitiesPerHour = new RangeUtilities(15, 22),
            ActivitiesPerWeek = new RangeUtilities(800, 1200),
            ActivitiesPerJob = new RangeUtilities(16, 28),
            DelayBetweenJobs = new RangeUtilities(73, 110),
            DelayBetweenActivity = new RangeUtilities(35, 50)
        };

        public List<string> ListQueryType { get; set; } = new List<string>();

        [ProtoMember(2)]
        public ObservableCollection<ManageMessagesModel> LstDisplayManageMessageModel { get; set; } =
            new ObservableCollection<ManageMessagesModel>();

        [ProtoMember(3)] public ManageMessagesModel ManageMessagesModel { get; set; } = new ManageMessagesModel();

        public bool IsCheckSkipUsersWhoWereAlreadySentAMessageFromTheSoftware
        {
            get => _isCheckSkipUsersWhoWereAlreadySentAMessageFromTheSoftware;
            set
            {
                if (value == _isCheckSkipUsersWhoWereAlreadySentAMessageFromTheSoftware)
                    return;
                SetProperty(ref _isCheckSkipUsersWhoWereAlreadySentAMessageFromTheSoftware, value);
            }
        }

        public bool IsPrivateBlacklists
        {
            get => _isPrivateBlacklists;

            set
            {
                if (value == _isPrivateBlacklists)
                    return;
                SetProperty(ref _isPrivateBlacklists, value);
            }
        }

        public bool IsGroupBlacklists
        {
            get => _isGroupBlacklists;

            set
            {
                if (value == _isGroupBlacklists)
                    return;
                SetProperty(ref _isGroupBlacklists, value);
            }
        }

        public bool IsSkipBlacklistsUser
        {
            get => _isSkipBlacklistsUser;
            set
            {
                if (value == _isSkipBlacklistsUser)
                    return;
                SetProperty(ref _isSkipBlacklistsUser, value);
            }
        }

        [ProtoMember(1)] JobConfiguration IGeneralSettings.JobConfiguration { get; set; }
    }
}