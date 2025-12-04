using System.Collections.Generic;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace LinkedDominatorCore.LDModel.Profilling
{
    [ProtoContract]
    public class ProfileEndorsementModel : ModuleSetting
    {
        private bool _IsChkGroupBlackList;

        private bool _IsChkPrivateBlackList;

        private bool _IsChkSkipBlackListedUser;

        [ProtoMember(5)] private string _UrlInput;

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

        [ProtoMember(1)] public override JobConfiguration JobConfiguration { get; set; } = new JobConfiguration();

        [ProtoMember(2)]
        public bool IsCheckedBySoftware
        {
            get => _IsCheckedBySoftware;
            set
            {
                if (value == _IsCheckedBySoftware) return;
                SetProperty(ref _IsCheckedBySoftware, value);
            }
        }

        [ProtoMember(3)]
        public bool IsCheckedOutSideSoftware
        {
            get => _IsCheckedOutSideSoftware;
            set
            {
                if (value == _IsCheckedOutSideSoftware) return;
                SetProperty(ref _IsCheckedOutSideSoftware, value);
            }
        }

        [ProtoMember(4)]
        public bool IsCheckedLangKeyCustomUserList
        {
            get => _IsCheckedLangKeyCustomUserList;
            set
            {
                if (value == _IsCheckedLangKeyCustomUserList) return;
                SetProperty(ref _IsCheckedLangKeyCustomUserList, value);
            }
        }

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

        [ProtoMember(6)]
        public List<string> UrlList
        {
            get => _UrlList;
            set => SetProperty(ref _UrlList, value);
        }

        [ProtoMember(7)]
        public int NumberOfSkillsToBeEndorsed
        {
            get => _NumberOfSkillsToBeEndorsed;
            set => SetProperty(ref _NumberOfSkillsToBeEndorsed, value);
        }

        [ProtoMember(8)]
        public bool IsChkSkipBlackListedUser
        {
            get => _IsChkSkipBlackListedUser;
            set => SetProperty(ref _IsChkSkipBlackListedUser, value);
        }

        [ProtoMember(9)]
        public bool IsChkPrivateBlackList
        {
            get => _IsChkPrivateBlackList;
            set => SetProperty(ref _IsChkPrivateBlackList, value);
        }

        [ProtoMember(10)]
        public bool IsChkGroupBlackList
        {
            get => _IsChkGroupBlackList;
            set => SetProperty(ref _IsChkGroupBlackList, value);
        }

        [ProtoMember(11)]
        public bool IsCheckedConnectedBefore
        {
            get => _IsCheckedConnectedBefore;
            set => SetProperty(ref _IsCheckedConnectedBefore, value);
        }

        [ProtoMember(12)]
        public int Days
        {
            get => _days;
            set => SetProperty(ref _days, value);
        }

        [ProtoMember(13)]
        public int Hours
        {
            get => _hours;
            set => SetProperty(ref _hours, value);
        }

        #region MyRegion

        private bool _IsCheckedBySoftware;
        private bool _IsCheckedOutSideSoftware;
        private bool _IsCheckedLangKeyCustomUserList;
        private List<string> _UrlList;
        private int _NumberOfSkillsToBeEndorsed = 5;

        private int _days;
        private int _hours;
        private bool _IsCheckedConnectedBefore;

        #endregion
    }
}