using System.Collections.Generic;
using System.Collections.ObjectModel;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace LinkedDominatorCore.LDModel.SalesNavigatorScraper
{
    [ProtoContract]
    public class UserScraperModel : ModuleSetting
    {
        private bool _IsCheckedOnlyDetailsRequiredToSendConnectionRequest;

        private bool _IsCheckedVisiting = true;

        private bool _IsCheckedVisitOnly;

        private bool _IsCheckedWithoutVisiting;

        private bool _IsChkGroupBlackList;

        private bool _IsChkPrivateBlackList;

        private bool _IsChkSkipBlackListedUser;

        private bool _IsUniqueOperationChecked;

        private bool _SavePagination;

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

        [ProtoMember(2)] public List<string> ListQueryType { get; set; } = new List<string>();

        [ProtoMember(3)] public override JobConfiguration JobConfiguration { get; set; } = new JobConfiguration();

        [ProtoMember(4)]
        public bool IsCheckedVisiting
        {
            get => _IsCheckedVisiting;
            set => SetProperty(ref _IsCheckedVisiting, value);
        }

        [ProtoMember(5)]
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

        [ProtoMember(6)]
        public bool IsCheckedVisitOnly
        {
            get => _IsCheckedVisitOnly;
            set
            {
                if (_IsCheckedVisitOnly == value)
                    return;
                SetProperty(ref _IsCheckedVisitOnly, value);
            }
        }

        public bool IsCheckedOnlyDetailsRequiredToSendConnectionRequest
        {
            get => _IsCheckedOnlyDetailsRequiredToSendConnectionRequest;
            set
            {
                if (_IsCheckedOnlyDetailsRequiredToSendConnectionRequest == value)
                    return;
                SetProperty(ref _IsCheckedOnlyDetailsRequiredToSendConnectionRequest, value);
            }
        }

        [ProtoMember(7)]
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

        [ProtoMember(8)]
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

        [ProtoMember(9)]
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

        [ProtoMember(10)]
        public bool SavePagination
        {
            get => _SavePagination;
            set => SetProperty(ref _SavePagination, value);
        }

        [ProtoMember(10)]
        public bool IsUniqueOperationChecked
        {
            get => _IsUniqueOperationChecked;
            set => SetProperty(ref _IsUniqueOperationChecked, value);
        }
       
    }
}