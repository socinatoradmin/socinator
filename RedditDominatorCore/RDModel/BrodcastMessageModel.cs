using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using static RedditDominatorCore.RDModel.ManageBlacklist;

namespace RedditDominatorCore.RDModel
{
    public interface IBrodcastMessageModel
    {
        bool IsChkBroadCastPrivateBlacklist { get; set; }
        bool IsChkBroadCastGroupBlacklist { get; set; }
    }

    public class BrodcastMessageModel : ModuleSetting, IBrodcastMessageModel
    {
        private ManageMessagesModel _manageMessagesModel = new ManageMessagesModel();

        public JobConfiguration FastSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(53, 80),
            ActivitiesPerHour = new RangeUtilities(5, 8),
            ActivitiesPerWeek = new RangeUtilities(320, 480),
            ActivitiesPerJob = new RangeUtilities(7, 10),
            DelayBetweenJobs = new RangeUtilities(87, 130),
            DelayBetweenActivity = new RangeUtilities(0, 1),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration MediumSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(33, 50),
            ActivitiesPerHour = new RangeUtilities(3, 5),
            ActivitiesPerWeek = new RangeUtilities(200, 300),
            ActivitiesPerJob = new RangeUtilities(4, 6),
            DelayBetweenJobs = new RangeUtilities(87, 130),
            DelayBetweenActivity = new RangeUtilities(0, 1),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration SlowSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(13, 20),
            ActivitiesPerHour = new RangeUtilities(1, 2),
            ActivitiesPerWeek = new RangeUtilities(80, 120),
            ActivitiesPerJob = new RangeUtilities(2, 3),
            DelayBetweenJobs = new RangeUtilities(88, 132),
            DelayBetweenActivity = new RangeUtilities(0, 1),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };


        public JobConfiguration SuperfastSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(66, 100),
            ActivitiesPerHour = new RangeUtilities(7, 10),
            ActivitiesPerWeek = new RangeUtilities(400, 600),
            ActivitiesPerJob = new RangeUtilities(8, 12),
            DelayBetweenJobs = new RangeUtilities(88, 132),
            DelayBetweenActivity = new RangeUtilities(0, 1),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        [ProtoMember(1)] public override JobConfiguration JobConfiguration { get; set; }
        private bool _IsChkBroadCastPrivateBlacklist;
        private bool _IsChkBroadCastGroupBlacklist;

        [ProtoMember(2)]
        public override ObservableCollection<QueryInfo> SavedQueries { get; set; } =
            new ObservableCollection<QueryInfo>();

        [ProtoMember(3)]
        public ObservableCollection<ManageMessagesModel> LstManageMessagesModel { get; set; } =
            new ObservableCollection<ManageMessagesModel>();

        [ProtoMember(4)]
        public ManageMessagesModel ManageMessagesModel
        {
            get => _manageMessagesModel;
            set
            {
                if (value == _manageMessagesModel)
                    return;
                SetProperty(ref _manageMessagesModel, value);
            }
        }

        [ProtoMember(5)] public OtherConfigModel OtherConfigModel { get; set; } = new OtherConfigModel();

        public List<string> ListQueryType { get; set; } = new List<string>();

        [ProtoMember(6)] public SkipBlacklist SkipBlacklist { get; set; } = new SkipBlacklist();

        private bool _IsSpintax;

        [ProtoMember(7)]
        public bool IsChkBroadCastPrivateBlacklist
        {
            get => _IsChkBroadCastPrivateBlacklist;
            set
            {
                if (value == _IsChkBroadCastPrivateBlacklist)
                    return;
                SetProperty(ref _IsChkBroadCastPrivateBlacklist, value);
            }
        }
        [ProtoMember(8)]
        public bool IsChkBroadCastGroupBlacklist
        {
            get => _IsChkBroadCastGroupBlacklist;
            set
            {
                if (value == _IsChkBroadCastGroupBlacklist)
                    return;
                SetProperty(ref _IsChkBroadCastGroupBlacklist, value);
            }
        }


        [ProtoMember(9)]
        public bool IsSpintax
        {
            get => _IsSpintax;
            set => SetProperty(ref _IsSpintax, value);
        }
    }
}