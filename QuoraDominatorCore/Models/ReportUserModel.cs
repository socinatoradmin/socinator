using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace QuoraDominatorCore.Models
{
    public interface IReportUserModel
    {
        #region IReportUserModel

        RangeUtilities IncreaseEachDayFollow { get; set; }
        bool EnableFollowUserer { get; set; }

        #endregion
    }

    [ProtoContract]
    public class ReportUserModel : ModuleSetting, IReportUserModel, IGeneralSettings
    {
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

        private bool _enableFollowUserer;

        [ProtoMember(11)]
        public bool EnableFollowUserer
        {
            get => _enableFollowUserer;

            set
            {
                if (value == _enableFollowUserer) return;
                SetProperty(ref _enableFollowUserer, value);
            }
        }

        private bool _isChkPrivateblacklist;

        [ProtoMember(12)]
        public bool IsChkPrivateblacklisted
        {
            get => _isChkPrivateblacklist;
            set
            {
                if (_isChkPrivateblacklist == value)
                    return;
                SetProperty(ref _isChkPrivateblacklist, value);
            }
        }

        private bool _isChkgroupblacklist;

        [ProtoMember(12)]
        public bool IsChkGroupblacklisted
        {
            get => _isChkgroupblacklist;
            set
            {
                if (_isChkgroupblacklist == value)
                    return;
                SetProperty(ref _isChkgroupblacklist, value);
            }
        }
        private ObservableCollection<string> _reportOptions = new ObservableCollection<string>();
        [ProtoMember(13)]
        public ObservableCollection<string> ReportOptions
        {
            get => _reportOptions;
            set
            {
                if (_reportOptions == value) return;
                SetProperty(ref _reportOptions, value);
            }
        }
        private string _selectedOption = string.Empty;
        [ProtoMember(14)]
        public string SelectedOption
        {
            get => _selectedOption;
            set
            {
                if (_selectedOption == value) return;
                SetProperty(ref _selectedOption, value);
                UpdateSubOptions(_selectedOption);
            }
        }
        private string _toolTipText=string.Empty;
        [ProtoMember(17)]
        public string ToolTipText
        {
            get { return _toolTipText; }
            set
            {
                if (_toolTipText == value) return;
                SetProperty(ref _toolTipText, value);
            }
        }
        private string _reportDescription=string.Empty;
        [ProtoMember(18)]
        public string ReportDescription
        {
            get => _reportDescription;
            set
            {
                if (_reportDescription == value) return;
                SetProperty(ref _reportDescription, value);
            }
        }
        private void UpdateSubOptions(string selectedOption)
        {
            ReportSubOptions.Clear();
            _selectedSubOption = new ReportOptionsModel();
            switch (selectedOption)
            {
                case "User Credential":
                case "User description":
                case "User photo":
                    ReportSubOptions.Add(new ReportOptionsModel { Title = "Spam", Description = "Selling illegal goods, money scams etc.",HasDescription=true });
                    ReportSubOptions.Add(new ReportOptionsModel { Title = "Hate Speech", Description = "Serious attack on a group",HasDescription = true });
                    ReportSubOptions.Add(new ReportOptionsModel { Title = "Harassment and bullying", Description = "Harassing or threatening an individual",HasDescription = true });
                    ReportSubOptions.Add(new ReportOptionsModel { Title = "Harmful activities", Description = "Glorifying violence including self-harm or intent to seriously harm others",HasDescription = true });
                    ReportSubOptions.Add(new ReportOptionsModel { Title = "Sexual exploitation and abuse (child safety)", Description = "Sexually explicit or suggestive imagery or writing involving minors", HasDescription = true });
                    ReportSubOptions.Add(new ReportOptionsModel { Title = "Sexual exploitation and abuse (adults and animals)", Description = "Sexually explicit or suggestive imagery or writing involving non-consenting adults or non-humans", HasDescription = true });
                    ReportSubOptions.Add(new ReportOptionsModel { Title = "Plagiarism", Description = "Reusing content without attribution (link and blockquotes)" , HasDescription = true });
                    break;
                default:
                    ReportSubOptions.Add(new ReportOptionsModel { Title = "Contains abuse or hate speech" });
                    //ReportSubOptions.Add(new ReportOptionsModel { Title = "Impersonation violation" });
                    ReportSubOptions.Add(new ReportOptionsModel { Title = "Contains profanity or obscenity" });
                    ReportSubOptions.Add(new ReportOptionsModel { Title = "Contains adult content" });
                    break;
            }
            SelectedSubOption = ReportSubOptions.FirstOrDefault();
        }

        private ObservableCollection<ReportOptionsModel> _reportSubOptions= new ObservableCollection<ReportOptionsModel>();
        [ProtoMember(15)]
        public ObservableCollection<ReportOptionsModel> ReportSubOptions
        {
            get => _reportSubOptions;
            set
            {
                if (_reportSubOptions == value) return;
                SetProperty(ref _reportSubOptions, value);
            }
        }
        private ReportOptionsModel _selectedSubOption=new ReportOptionsModel();
        [ProtoMember(16)]
        public ReportOptionsModel SelectedSubOption
        {
            get => _selectedSubOption;
            set
            {
                if (_selectedSubOption == value) return;
                if(value!=null)
                    ToolTipText = $"{value.Title}\n{value.Description}";
                SetProperty(ref _selectedSubOption, value);
            }
        }

        private bool _isChkSkipPrivateBlackList;
        private bool _isChkSkipGroupBlackList;
        public bool IsChkReportUserSkipPrivateBlacklist
        {
            get => _isChkSkipPrivateBlackList;
            set
            {
                if (_isChkSkipPrivateBlackList != value)
                    SetProperty(ref _isChkSkipPrivateBlackList, value);
            }
        }

        public bool IsChkReportUserSkipGroupBlacklist
        {
            get => _isChkSkipGroupBlackList;
            set
            {
                if (_isChkSkipGroupBlackList != value)
                    SetProperty(ref _isChkSkipGroupBlackList, value);
            }
        }
        #endregion
    }
}