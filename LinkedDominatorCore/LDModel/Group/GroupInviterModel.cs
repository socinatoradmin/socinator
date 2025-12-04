using System.Collections.Generic;
using System.Collections.ObjectModel;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace LinkedDominatorCore.LDModel
{
    [ProtoContract]
    public class GroupInviterModel : ModuleSetting
    {
        private List<GroupSelectDestination> _accountPagesBoardsPair = new List<GroupSelectDestination>();


        private bool _isSelectGroup;

        private List<string> _listOfGroupUrl = new List<string>();

        private ObservableCollection<GroupCreateDestinationSelectModel> _listSelectDestination =
            new ObservableCollection<GroupCreateDestinationSelectModel>();

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

        [ProtoMember(4)] public override JobConfiguration JobConfiguration { get; set; } = new JobConfiguration();

        public List<string> ListOfGroupUrl
        {
            get => _listOfGroupUrl;
            set
            {
                if (value == _listOfGroupUrl)
                    return;
                SetProperty(ref _listOfGroupUrl, value);
            }
        }

        public ObservableCollection<GroupCreateDestinationSelectModel> ListSelectDestination
        {
            get => _listSelectDestination;
            set
            {
                if (_listSelectDestination == value)
                    return;
                _listSelectDestination = value;
                OnPropertyChanged(nameof(ListSelectDestination));
            }
        }

        public List<GroupSelectDestination> AccountPagesBoardsPair
        {
            get => _accountPagesBoardsPair;
            set => SetProperty(ref _accountPagesBoardsPair, value);
        }

        [ProtoMember(1)]
        public bool IsSelectGroup
        {
            get => _isSelectGroup;
            set
            {
                if (_isSelectGroup == value)
                    return;
                SetProperty(ref _isSelectGroup, value);
            }
        }
    }

    public class GroupSelectDestination
    {
        public string AccountId { get; set; }

        public KeyValuePair<string, List<GroupQueryContent>> ListofGroupsofAccounts { get; set; }

        public string Label { get; set; }

        public bool IsSelected { get; set; }
    }
}