using System.Collections.Generic;
using System.Collections.ObjectModel;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace QuoraDominatorCore.Models
{
    public interface IReportAnswerModel
    {
        #region IReportAnswerModel

        RangeUtilities IncreaseEachDayFollow { get; set; }

        #endregion
    }

    [ProtoContract]
    public class ReportAnswerModel : ModuleSetting, IReportAnswerModel, IGeneralSettings
    {
        private bool _isChkGroupBlacklist;

        private bool _isChkPrivateBlacklist;

        public JobConfiguration FastSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(266, 400),
            ActivitiesPerHour = new RangeUtilities(26, 40),
            ActivitiesPerWeek = new RangeUtilities(1600, 2400),
            ActivitiesPerJob = new RangeUtilities(33, 50),
            DelayBetweenJobs = new RangeUtilities(81, 122),
            DelayBetweenActivity = new RangeUtilities(15, 30),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration MediumSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(133, 200),
            ActivitiesPerHour = new RangeUtilities(13, 20),
            ActivitiesPerWeek = new RangeUtilities(800, 1200),
            ActivitiesPerJob = new RangeUtilities(16, 25),
            DelayBetweenJobs = new RangeUtilities(73, 110),
            DelayBetweenActivity = new RangeUtilities(30, 60),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration SlowSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(66, 100),
            ActivitiesPerHour = new RangeUtilities(6, 10),
            ActivitiesPerWeek = new RangeUtilities(400, 600),
            ActivitiesPerJob = new RangeUtilities(8, 12),
            DelayBetweenJobs = new RangeUtilities(81, 122),
            DelayBetweenActivity = new RangeUtilities(60, 60),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };


        public JobConfiguration SuperfastSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(400, 600),
            ActivitiesPerHour = new RangeUtilities(40, 60),
            ActivitiesPerWeek = new RangeUtilities(2400, 3600),
            ActivitiesPerJob = new RangeUtilities(50, 75),
            DelayBetweenJobs = new RangeUtilities(65, 97),
            DelayBetweenActivity = new RangeUtilities(7, 15),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public List<string> ListQueryType { get; set; } = new List<string>();


        [ProtoMember(1)] public override UserFilterModel UserFilterModel { get; set; } = new UserFilterModel();

        [ProtoMember(3)] public override AnswerFilterModel AnswerFilterModel { get; set; } = new AnswerFilterModel();
        [ProtoMember(15)] public override TopicFilterModel TopicFilter { get; set; }= new TopicFilterModel();
        [ProtoMember(4)]
        public bool IsChkReportAnswerPrivateBlacklist
        {
            get => _isChkPrivateBlacklist;
            set
            {
                if (_isChkPrivateBlacklist != value)
                    SetProperty(ref _isChkPrivateBlacklist, value);
            }
        }

        [ProtoMember(4)]
        public bool IsChkReportAnswerGroupBlacklist
        {
            get => _isChkGroupBlacklist;
            set
            {
                if (_isChkGroupBlacklist != value)
                    SetProperty(ref _isChkGroupBlacklist, value);
            }
        }

        [ProtoMember(2)] JobConfiguration IGeneralSettings.JobConfiguration { get; set; }

        #region IUpvoteAnswersModel

        private int _increaseUpvoteAnswersByCount = 10;

        [ProtoMember(5)]
        public int IncreaseUpvoteAnswersByCount
        {
            get => _increaseUpvoteAnswersByCount;
            set
            {
                if (value == _increaseUpvoteAnswersByCount) return;
                SetProperty(ref _increaseUpvoteAnswersByCount, value);
            }
        }

        private int _increaseUpvoteAnswersCountUntil = 500;

        [ProtoMember(6)]
        public int IncreaseUpvoteAnswersCountUntil
        {
            get => _increaseUpvoteAnswersCountUntil;
            set
            {
                if (value == _increaseUpvoteAnswersByCount) return;
                SetProperty(ref _increaseUpvoteAnswersCountUntil, value);
            }
        }


        private RangeUtilities _followBetweenJobs = new RangeUtilities(10, 40);

        [ProtoMember(7)]
        public RangeUtilities FollowBetweenJobs
        {
            get => _followBetweenJobs;
            set
            {
                if (value == _followBetweenJobs) return;
                SetProperty(ref _followBetweenJobs, value);
            }
        }

        private bool _enableFollowAnswerer;

        [ProtoMember(8)]
        public bool EnableFollowAnswerer
        {
            get => _enableFollowAnswerer;
            set
            {
                if (value == _enableFollowAnswerer) return;
                SetProperty(ref _enableFollowAnswerer, value);
            }
        }

        private RangeUtilities _followMaxBetween = new RangeUtilities(400, 500);

        [ProtoMember(9)]
        public RangeUtilities FollowMaxBetween
        {
            get => _followMaxBetween;
            set
            {
                if (value == _followMaxBetween) return;
                SetProperty(ref _followMaxBetween, value);
            }
        }

        private RangeUtilities _increaseEachDayFollow = new RangeUtilities(10, 20);

        [ProtoMember(10)]
        public RangeUtilities IncreaseEachDayFollow
        {
            get => _increaseEachDayFollow;
            set
            {
                if (value == _increaseEachDayFollow) return;
                SetProperty(ref _increaseEachDayFollow, value);
            }
        }
        private ObservableCollection<ReportOptionsModel> _reportOptionsModels=new ObservableCollection<ReportOptionsModel>();
        [ProtoMember(11)]
        public ObservableCollection<ReportOptionsModel> ReportOptions
        {
            get => _reportOptionsModels;
            set
            {
                if(_reportOptionsModels == value) return;
                SetProperty(ref _reportOptionsModels, value);
            }
        }
        private ObservableCollection<ReportOptionsModel> _reportsubOptionsModels = new ObservableCollection<ReportOptionsModel>();
        [ProtoMember(15)]
        public ObservableCollection<ReportOptionsModel> ReportSubOptions
        {
            get => _reportsubOptionsModels;
            set
            {
                if (_reportsubOptionsModels == value) return;
                SetProperty(ref _reportsubOptionsModels, value);
            }
        }
        private string _subToolTip = string.Empty;
        [ProtoMember(16)]
        public string SubToolTip
        {
            get => _subToolTip;
            set
            {
                if (_subToolTip == value) return;
                SetProperty(ref _subToolTip, value);
            }
        }
        private string _toolTipText = string.Empty;
        [ProtoMember(12)]
        public string ToolTipText
        {
            get => _toolTipText;
            set
            {
                if (_toolTipText == value) return;
                SetProperty(ref _toolTipText, value);
            }
        }
        private ReportOptionsModel _selectedReport=new ReportOptionsModel();
        [ProtoMember(13)]
        public ReportOptionsModel SelectedOption
        {
            get { return _selectedReport; }
            set
            {
                if (_selectedReport == value) return;
                ToolTipText = $"{value.Title}\n{value.Description}";
                EnableSubOption = value != null && value.Title != null && value.Title == "Inappropriate credential";
                SetProperty(ref _selectedReport, value);
            }
        }
        private ReportOptionsModel _SelectedSubOptionOption=new ReportOptionsModel();
        [ProtoMember(17)]
        public ReportOptionsModel SelectedSubOption
        {
            get { return _SelectedSubOptionOption; }
            set
            {
                if (_SelectedSubOptionOption == value) return;
                SubToolTip = $"{value.Title}\n{value.Description}";
                SetProperty(ref _SelectedSubOptionOption, value);
            }
        }
        private bool isEnableSubOption=false;
        [ProtoMember(18)]
        public bool EnableSubOption
        {
            get => isEnableSubOption;
            set=> SetProperty(ref isEnableSubOption, value);
        }
        private string _reportDescription=string.Empty;
        [ProtoMember(14)]
        public string ReportDescription
        {
            get => _reportDescription;
            set
            {
                if (_reportDescription == value) return;
                SetProperty (ref _reportDescription, value);
            }
        }
        #endregion
    }
}