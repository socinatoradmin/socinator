using System;
using System.Linq;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;
using ProtoBuf;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    [ProtoContract]
    public class DeleteSetting : BindableBase
    {
        private RangeUtilities _deleteCommentRange = new RangeUtilities {StartValue = 1, EndValue = 1};
        private RangeUtilities _deleteTweetRange = new RangeUtilities {StartValue = 1, EndValue = 1};

        private DateTime? _endDateForComment;

        private DateTime? _endDateForRetweet;

        private DateTime? _endDateForTeeet;

        private bool _isChkCommentedDateMustBeInSpecificRange;

        private bool _isChkDeleteComment;
        private bool _isChkDeleteTweet;

        private bool _isChkRetweetedDateMustBeInSpecificRange;

        private bool _isChkTweetedDateMustBeInSpecificRange;


        private bool _isChkUndoRetweet;

        private bool _IsDeleteRandomComments;

        private bool _IsDeleteRandomTweets;

        private bool _IsUndoRandomRetweets;


        private DateTime? _startDateForComment;

        private DateTime? _startDateForRetweet;

        private DateTime? _startDateForTeeet;
        private RangeUtilities _undoRetweetRange = new RangeUtilities {StartValue = 1, EndValue = 1};

        [ProtoMember(1)]
        public bool IsChkDeleteTweet
        {
            get => _isChkDeleteTweet;
            set
            {
                if (_isChkDeleteTweet == value)
                    return;
                SetProperty(ref _isChkDeleteTweet, value);
            }
        }

        [ProtoMember(2)]
        public RangeUtilities DeleteTweetRange
        {
            get => _deleteTweetRange;
            set
            {
                if (_deleteTweetRange == value)
                    return;
                SetProperty(ref _deleteTweetRange, value);
            }
        }

        [ProtoMember(3)]
        public bool IsChkTweetedDateMustBeInSpecificRange
        {
            get => _isChkTweetedDateMustBeInSpecificRange;
            set
            {
                if (_isChkTweetedDateMustBeInSpecificRange == value)
                    return;
                SetProperty(ref _isChkTweetedDateMustBeInSpecificRange, value);
            }
        }

        [ProtoMember(4)]
        public DateTime? StartDateForTweet
        {
            get => _startDateForTeeet;
            set
            {
                if (_startDateForTeeet == value)
                    return;
                SetProperty(ref _startDateForTeeet, value);
            }
        }

        [ProtoMember(5)]
        public DateTime? EndDateForTweet
        {
            get => _endDateForTeeet;
            set
            {
                if (_endDateForTeeet == value)
                    return;
                SetProperty(ref _endDateForTeeet, value);
            }
        }

        [ProtoMember(6)]
        public bool IsChkDeleteComment
        {
            get => _isChkDeleteComment;
            set
            {
                if (_isChkDeleteComment == value)
                    return;
                SetProperty(ref _isChkDeleteComment, value);
            }
        }

        [ProtoMember(7)]
        public RangeUtilities DeleteCommentRange
        {
            get => _deleteCommentRange;
            set
            {
                if (_deleteCommentRange == value)
                    return;
                SetProperty(ref _deleteCommentRange, value);
            }
        }

        [ProtoMember(8)]
        public bool IsChkCommentedDateMustBeInSpecificRange
        {
            get => _isChkCommentedDateMustBeInSpecificRange;
            set
            {
                if (_isChkCommentedDateMustBeInSpecificRange == value)
                    return;
                SetProperty(ref _isChkCommentedDateMustBeInSpecificRange, value);
            }
        }

        [ProtoMember(9)]
        public DateTime? StartDateForComment
        {
            get => _startDateForComment;
            set
            {
                if (_startDateForComment == value)
                    return;
                SetProperty(ref _startDateForComment, value);
            }
        }

        [ProtoMember(10)]
        public DateTime? EndDateForComment
        {
            get => _endDateForComment;
            set
            {
                if (_endDateForComment == value)
                    return;
                SetProperty(ref _endDateForComment, value);
            }
        }

        [ProtoMember(11)]
        public bool IsChkUndoRetweet
        {
            get => _isChkUndoRetweet;
            set
            {
                if (_isChkUndoRetweet == value)
                    return;
                SetProperty(ref _isChkUndoRetweet, value);
            }
        }

        [ProtoMember(12)]
        public RangeUtilities UndoRetweetRange
        {
            get => _undoRetweetRange;
            set
            {
                if (_undoRetweetRange == value)
                    return;
                SetProperty(ref _undoRetweetRange, value);
            }
        }

        [ProtoMember(13)]
        public bool IsChkRetweetedDateMustBeInSpecificRange
        {
            get => _isChkRetweetedDateMustBeInSpecificRange;
            set
            {
                if (_isChkRetweetedDateMustBeInSpecificRange == value)
                    return;
                SetProperty(ref _isChkRetweetedDateMustBeInSpecificRange, value);
            }
        }

        [ProtoMember(14)]
        public DateTime? StartDateForRetweet
        {
            get => _startDateForRetweet;
            set
            {
                if (_startDateForRetweet == value)
                    return;
                SetProperty(ref _startDateForRetweet, value);
            }
        }

        [ProtoMember(15)]
        public DateTime? EndDateForRetweet
        {
            get => _endDateForRetweet;
            set
            {
                if (_endDateForRetweet == value)
                    return;
                SetProperty(ref _endDateForRetweet, value);
            }
        }

        [ProtoMember(16)]
        public bool IsDeleteRandomTweets
        {
            get => _IsDeleteRandomTweets;
            set => SetProperty(ref _IsDeleteRandomTweets, value);
        }

        [ProtoMember(17)]
        public bool IsUndoRandomRetweets
        {
            get => _IsUndoRandomRetweets;
            set => SetProperty(ref _IsUndoRandomRetweets, value);
        }

        [ProtoMember(18)]
        public bool IsDeleteRandomComments
        {
            get => _IsDeleteRandomComments;
            set => SetProperty(ref _IsDeleteRandomComments, value);
        }
    }

    public interface IDeleteViewModel
    {
        DeleteSetting DeleteSetting { get; set; }
    }

    public class DeleteViewModel : StartupBaseViewModel, IDeleteViewModel
    {
        private DeleteSetting _deleteSetting = new DeleteSetting();

        public DeleteViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig {Model = this, ActivityType = ActivityType.Delete});
            NextCommand = new DelegateCommand(ValidateAndNavigate);
            PreviousCommand = new DelegateCommand(NavigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);
            IsNonQuery = true;
            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfDeletesPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfDeletesPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfDeletesPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfDeletesPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxDeletePerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
        }

        public DeleteSetting DeleteSetting
        {
            get => _deleteSetting;
            set => SetProperty(ref _deleteSetting, value);
        }

        private void ValidateAndNavigate()
        {
            if (!DeleteSetting.IsChkDeleteTweet && !DeleteSetting.IsChkDeleteComment
                                                && !DeleteSetting.IsChkUndoRetweet)
            {
                Dialog.ShowDialog("Error", "Please select atleast one Delete source");
                return;
            }

            NavigateNext();
        }
    }
}