using System.Collections.Generic;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace LinkedDominatorCore.LDModel.GrowConnection
{
    [ProtoContract]
    public class RemoveConnectionModel : ModuleSetting
    {
        private bool _IsChkAddToBlackList;

        private bool _IsChkAddToGroupBlackList;

        private bool _IsChkAddToPrivateBlackList;

        private bool _IsChkSkipWhiteListedUser;

        private bool _IsChkUseGroupWhiteList;

        private bool _IsChkUsePrivateWhiteList;

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
            set => SetProperty(ref _IsCheckedOutSideSoftware, value);
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
            set
            {
                if (value == _UrlList) return;
                SetProperty(ref _UrlList, value);
            }
        }

        [ProtoMember(7)]
        public bool IsCheckedConnectedBefore
        {
            get => _IsCheckedConnectedBefore;
            set
            {
                if (value == _IsCheckedConnectedBefore) return;
                SetProperty(ref _IsCheckedConnectedBefore, value);
            }
        }

        [ProtoMember(8)]
        public int Days
        {
            get => _Days;
            set
            {
                if (value == _Days) return;
                SetProperty(ref _Days, value);
            }
        }

        [ProtoMember(9)]
        public int Hours
        {
            get => _Hours;
            set
            {
                if (value == _Hours) return;
                SetProperty(ref _Hours, value);
            }
        }

        [ProtoMember(10)]
        public bool IsChkSkipWhiteListedUser
        {
            get => _IsChkSkipWhiteListedUser;
            set
            {
                if (value == _IsChkSkipWhiteListedUser) return;
                SetProperty(ref _IsChkSkipWhiteListedUser, value);
            }
        }

        [ProtoMember(11)]
        public bool IsChkUsePrivateWhiteList
        {
            get => _IsChkUsePrivateWhiteList;
            set
            {
                if (value == _IsChkUsePrivateWhiteList) return;
                SetProperty(ref _IsChkUsePrivateWhiteList, value);
            }
        }

        [ProtoMember(12)]
        public bool IsChkUseGroupWhiteList
        {
            get => _IsChkUseGroupWhiteList;
            set
            {
                if (value == _IsChkUseGroupWhiteList) return;
                SetProperty(ref _IsChkUseGroupWhiteList, value);
            }
        }

        [ProtoMember(13)]
        public bool IsChkAddToBlackList
        {
            get => _IsChkAddToBlackList;
            set
            {
                if (value == _IsChkAddToBlackList) return;
                SetProperty(ref _IsChkAddToBlackList, value);
            }
        }

        [ProtoMember(14)]
        public bool IsChkAddToPrivateBlackList
        {
            get => _IsChkAddToPrivateBlackList;
            set
            {
                if (value == _IsChkAddToPrivateBlackList) return;
                SetProperty(ref _IsChkAddToPrivateBlackList, value);
            }
        }

        [ProtoMember(15)]
        public bool IsChkAddToGroupBlackList
        {
            get => _IsChkAddToGroupBlackList;
            set
            {
                if (value == _IsChkAddToGroupBlackList) return;
                SetProperty(ref _IsChkAddToGroupBlackList, value);
            }
        }

        #region MyRegion

        private bool _IsCheckedBySoftware;
        private bool _IsCheckedOutSideSoftware;
        private bool _IsCheckedLangKeyCustomUserList;
        private List<string> _UrlList;
        private bool _IsCheckedConnectedBefore;
        private int _Days;
        private int _Hours;

        #endregion
    }
}