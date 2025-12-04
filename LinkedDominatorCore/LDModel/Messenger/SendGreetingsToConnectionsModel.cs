using System.Collections.ObjectModel;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.LDModel.Filters;
using ProtoBuf;

namespace LinkedDominatorCore.LDModel.Messenger
{
    public class SendGreetingsToConnectionsModel : ModuleSetting, IGeneralSettings
    {
        private bool _IsChkGroupBlackList;

        private bool _IsChkPrivateBlackList;

        private bool _IsChkSkipBlackListedUser;

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


        [ProtoMember(2)] public LDUserFilterModel UserFilterModel { get; set; } = new LDUserFilterModel();

        [ProtoMember(3)] public OtherConfigModel OtherConfigModel { get; set; } = new OtherConfigModel();

        [ProtoMember(4)]
        public bool IsCheckedBirthdayGreeting
        {
            get => _IsCheckedBirthdayGreeting;

            set
            {
                if (value == _IsCheckedBirthdayGreeting)
                    return;
                SetProperty(ref _IsCheckedBirthdayGreeting, value);
            }
        }

        [ProtoMember(5)]
        public bool IsCheckedNewJobGreeting
        {
            get => _IsCheckedNewJobGreeting;

            set
            {
                if (value == _IsCheckedNewJobGreeting)
                    return;
                SetProperty(ref _IsCheckedNewJobGreeting, value);
            }
        }

        [ProtoMember(6)]
        public bool IsCheckedWorkAnniversaryGreeting
        {
            get => _IsCheckedWorkAnniversaryGreeting;

            set
            {
                if (value == _IsCheckedWorkAnniversaryGreeting)
                    return;
                SetProperty(ref _IsCheckedWorkAnniversaryGreeting, value);
            }
        }

        [ProtoMember(7)]
        public bool IsChkSpintaxChecked
        {
            get => _IsChkSpintaxChecked;

            set
            {
                if (value == _IsChkSpintaxChecked)
                    return;
                SetProperty(ref _IsChkSpintaxChecked, value);
            }
        }

        [ProtoMember(8)]
        public bool IsChkTagChecked
        {
            get => _IsChkTagChecked;

            set
            {
                if (value == _IsChkTagChecked)
                    return;
                SetProperty(ref _IsChkTagChecked, value);
            }
        }

        [ProtoMember(9)]
        public ObservableCollection<ManageMessagesModel> LstDisplayManageMessagesModel { get; set; } =
            new ObservableCollection<ManageMessagesModel>();

        public ManageMessagesModel ManageMessagesModel { get; set; } = new ManageMessagesModel();

        public ObservableCollection<ManageMessagesModel> LstManageMessagesModel { get; set; } =
            new ObservableCollection<ManageMessagesModel>();

        [ProtoMember(10)]
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

        [ProtoMember(11)]
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

        [ProtoMember(12)]
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

        [ProtoMember(1)] JobConfiguration IGeneralSettings.JobConfiguration { get; set; } = new JobConfiguration();

        #region MyRegion

        private bool _IsCheckedBirthdayGreeting;
        private bool _IsCheckedNewJobGreeting;
        private bool _IsCheckedWorkAnniversaryGreeting;
        private bool _IsChkSpintaxChecked;
        private bool _IsChkTagChecked;

        #endregion
    }
}