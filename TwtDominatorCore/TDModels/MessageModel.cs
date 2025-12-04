using System.Collections.Generic;
using System.Collections.ObjectModel;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace TwtDominatorCore.TDModels
{
    [ProtoContract]
    public class MessageSetting : BindableBase
    {
        [ProtoMember(3)] private string _customFollowers;

        [ProtoMember(4)] private List<string> _customFollowersList;

        private bool _isChkCustomFollowers;
        private bool _isChkRandomFollowers;

        [ProtoMember(1)]
        public bool IsChkRandomFollowers
        {
            get => _isChkRandomFollowers;
            set
            {
                if (_isChkRandomFollowers == value)
                    return;
                SetProperty(ref _isChkRandomFollowers, value);
            }
        }

        [ProtoMember(2)]
        public bool IsChkCustomFollowers
        {
            get => _isChkCustomFollowers;
            set
            {
                if (_isChkCustomFollowers == value)
                    return;
                SetProperty(ref _isChkCustomFollowers, value);
            }
        }

        public string CustomFollowers
        {
            get => _customFollowers;
            set
            {
                if (_customFollowers == value)
                    return;
                SetProperty(ref _customFollowers, value);
            }
        }

        public List<string> CustomFollowersList
        {
            get => _customFollowersList;
            set => SetProperty(ref _customFollowersList, value);
        }
    }


    [ProtoContract]
    public class MessageModel : ModuleSetting, IGeneralSettings
    {
        private bool _IsReplyToMessagesThatContainSpecificWord;
        private bool _IsReplyToPendingMessagesChecked = true;
        private bool _IsSpintax;
        private bool _isTagChecked;
        private ObservableCollection<string> _ListMessagesNotReplied = new ObservableCollection<string>();
        private ObservableCollection<string> _ListMessagesReplied = new ObservableCollection<string>();
        private string _SpecificWord;


        [ProtoMember(1)] public override UserFilterModel UserFilterModel { get; set; } = new UserFilterModel();

        [ProtoMember(3)] public OtherConfigModel OtherConfigModel { get; set; } = new OtherConfigModel();

        [ProtoMember(4)]
        public ObservableCollection<ManageMessagesModel> LstDisplayManageMessageModel { get; set; } =
            new ObservableCollection<ManageMessagesModel>();

        /// <summary>
        ///     Temporary variable
        /// </summary>
        public ManageMessagesModel ManageMessagesModel { get; set; } = new ManageMessagesModel();
        //{
        //    LstQueries=new ObservableCollection<QueryContent>() {
        //        new QueryContent() { Content = new QueryInfo() { QueryValue = "Default" } },
        //        new QueryContent() { Content = new QueryInfo() { QueryValue = "Random Follower" } },
        //        new QueryContent() { Content = new QueryInfo() { QueryValue = "Custom" } },
        //    }
        //};

        [ProtoMember(5)]
        public bool IsReplyToMessagesThatContainSpecificWord
        {
            get => _IsReplyToMessagesThatContainSpecificWord;
            set => SetProperty(ref _IsReplyToMessagesThatContainSpecificWord, value);
        }

        [ProtoMember(6)]
        public string SpecificWord
        {
            get => _SpecificWord;
            set => SetProperty(ref _SpecificWord, value);
        }


        [ProtoMember(7)]
        public ObservableCollection<string> ListMessagesReplied
        {
            get => _ListMessagesReplied;
            set
            {
                if (_ListMessagesReplied != null)
                    return;
                SetProperty(ref _ListMessagesReplied, value);
            }
        }


        [ProtoMember(8)]
        public ObservableCollection<string> ListMessagesNotReplied
        {
            get => _ListMessagesNotReplied;
            set
            {
                if (_ListMessagesNotReplied != null)
                    return;
                SetProperty(ref _ListMessagesNotReplied, value);
            }
        }


        [ProtoMember(9)]
        public bool IsReplyToPendingMessagesChecked
        {
            get => _IsReplyToPendingMessagesChecked;
            set => SetProperty(ref _IsReplyToPendingMessagesChecked, value);
        }


        [ProtoMember(10)]
        public bool IsSpintax
        {
            get => _IsSpintax;
            set => SetProperty(ref _IsSpintax, value);
        }

        

        [ProtoMember(11)]
        public bool IsTagChecked
        {
            get { return _isTagChecked; }
            set
            {
                SetProperty(ref _isTagChecked, value);
            }
        }

        [ProtoMember(2)] JobConfiguration IGeneralSettings.JobConfiguration { get; set; }


        #region Manage Speed 

        /// <summary>
        ///     Slow week 150
        ///     Medium week 300
        ///     Fast week 450
        ///     SuperFast week 600
        /// </summary>
        public JobConfiguration SlowSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(15, 20),
            ActivitiesPerHour = new RangeUtilities(4, 6),
            ActivitiesPerWeek = new RangeUtilities(120, 150),
            ActivitiesPerJob = new RangeUtilities(2, 3),
            DelayBetweenJobs = new RangeUtilities(20, 30),
            DelayBetweenActivity = new RangeUtilities(40, 60),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration MediumSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(30, 45),
            ActivitiesPerHour = new RangeUtilities(8, 12),
            ActivitiesPerWeek = new RangeUtilities(250, 300),
            ActivitiesPerJob = new RangeUtilities(3, 4),
            DelayBetweenJobs = new RangeUtilities(50, 80),
            DelayBetweenActivity = new RangeUtilities(40, 60),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration FastSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(60, 70),
            ActivitiesPerHour = new RangeUtilities(10, 15),
            ActivitiesPerWeek = new RangeUtilities(400, 450),
            ActivitiesPerJob = new RangeUtilities(6, 8),
            DelayBetweenJobs = new RangeUtilities(100, 150),
            DelayBetweenActivity = new RangeUtilities(40, 60),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };


        public JobConfiguration SuperfastSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(80, 90),
            ActivitiesPerHour = new RangeUtilities(18, 25),
            ActivitiesPerWeek = new RangeUtilities(500, 600),
            ActivitiesPerJob = new RangeUtilities(10, 15),
            DelayBetweenJobs = new RangeUtilities(180, 220),
            DelayBetweenActivity = new RangeUtilities(40, 60),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        #endregion
    }
}