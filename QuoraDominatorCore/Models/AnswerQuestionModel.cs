using System.Collections.Generic;
using System.Collections.ObjectModel;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace QuoraDominatorCore.Models
{
    public class AnswerQuestionModel : ModuleSetting, IGeneralSettings
    {
        private bool _ischkGroupList;

        private bool _ischkPrivate;

        private bool _isHashChecked;


        private bool _isSpintaxChecked;

        private ObservableCollection<ManageCommentModel> _lstManageCommentModel =
            new ObservableCollection<ManageCommentModel>();


        private ManageCommentModel _manageCommentModel = new ManageCommentModel();


        private string _mediaPath = string.Empty;

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


        [ProtoMember(1)]
        public override QuestionFilterModel QuestionFilterModel { get; set; } = new QuestionFilterModel();

        [ProtoMember(3)] public override AnswerFilterModel AnswerFilterModel { get; set; } = new AnswerFilterModel();
        [ProtoMember(8)]public override TopicFilterModel TopicFilter { get; set; }= new TopicFilterModel();
        [ProtoMember(4)]
        public bool IsChkAnswerScraperPrivateList
        {
            get => _ischkPrivate;
            set
            {
                if (_ischkPrivate != value)
                    SetProperty(ref _ischkPrivate, value);
            }
        }

        [ProtoMember(5)]
        public bool IsChkAnswerScraperGroupList
        {
            get => _ischkGroupList;
            set
            {
                if (_ischkGroupList != value)
                    SetProperty(ref _ischkGroupList, value);
            }
        }

        [ProtoMember(6)]
        public ManageCommentModel ManageCommentModel
        {
            get => _manageCommentModel;
            set
            {
                if (_manageCommentModel == value)
                    return;
                SetProperty(ref _manageCommentModel, value);
            }
        }

        [ProtoMember(7)]
        public ObservableCollection<ManageCommentModel> LstManageCommentModel
        {
            get => _lstManageCommentModel;
            set
            {
                if (_lstManageCommentModel == value)
                    return;
                SetProperty(ref _lstManageCommentModel, value);
            }
        }

        public bool IsHashChecked
        {
            get => _isHashChecked;
            set
            {
                if (_isHashChecked != value)
                    SetProperty(ref _isHashChecked, value);
            }
        }

        public string MediaPath
        {
            get => _mediaPath;
            set
            {
                if (_mediaPath == value)
                    return;
                SetProperty(ref _mediaPath, value);
            }
        }

        public bool IsSpintaxChecked
        {
            get => _isSpintaxChecked;

            set
            {
                if (_isSpintaxChecked == value)
                    return;
                SetProperty(ref _isSpintaxChecked, value);
            }
        }
        private bool _isChkSkipPrivateBlackList;
        private bool _isChkSkipGroupBlackList;
        public bool IsChkAnswerOnQuestionSkipPrivateBlacklist
        {
            get => _isChkSkipPrivateBlackList;
            set
            {
                if (_isChkSkipPrivateBlackList != value)
                    SetProperty(ref _isChkSkipPrivateBlackList, value);
            }
        }

        public bool IsChkAnswerOnQuestionSkipGroupBlacklist
        {
            get => _isChkSkipGroupBlackList;
            set
            {
                if (_isChkSkipGroupBlackList != value)
                    SetProperty(ref _isChkSkipGroupBlackList, value);
            }
        }
        [ProtoMember(2)] JobConfiguration IGeneralSettings.JobConfiguration { get; set; }
    }
}