using System;
using System.Linq;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public interface ISendMessageToFollowerViewModel
    {
        bool IsCheckedSendMessageToNewFollowers { get; set; }
        bool IsChkMakeCaptionAsSpinText { get; set; }
        string TextMessage { get; set; }
    }

    public class SendMessageToFollowerViewModel : StartupBaseViewModel, ISendMessageToFollowerViewModel
    {
        private bool _IsCheckedSendMessageToNewFollowers;

        private bool _IsChkMakeCaptionAsSpinText;
        private string _message;


        private string _textMessage = string.Empty;

        public SendMessageToFollowerViewModel(IRegionManager region) : base(region)
        {
            IsNonQuery = true;
            ViewModelToSave.Add(new ActivityConfig {Model = this, ActivityType = ActivityType.SendMessageToFollower});
            NextCommand = new DelegateCommand(ValidateAndNevigate);
            PreviousCommand = new DelegateCommand(NavigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);

            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfMessagesPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfMessagesPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfMessagesPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfMessagesPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxMessagesPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
        }

        public string Message
        {
            get => _message;
            set
            {
                if (_message == value)
                    return;
                SetProperty(ref _message, value);
            }
        }

        public bool IsCheckedSendMessageToNewFollowers
        {
            get => _IsCheckedSendMessageToNewFollowers;

            set
            {
                if (_IsCheckedSendMessageToNewFollowers == value)
                    return;
                SetProperty(ref _IsCheckedSendMessageToNewFollowers, value);
            }
        }

        public bool IsChkMakeCaptionAsSpinText
        {
            get => _IsChkMakeCaptionAsSpinText;

            set
            {
                if (_IsChkMakeCaptionAsSpinText == value)
                    return;
                SetProperty(ref _IsChkMakeCaptionAsSpinText, value);
            }
        }

        public string TextMessage
        {
            get => _textMessage;
            set
            {
                SetProperty(ref _textMessage, value);
                Message = value;
            }
        }

        protected void ValidateAndNevigate()
        {
            if (string.IsNullOrEmpty(TextMessage))
            {
                Dialog.ShowDialog("Input Error",
                    "Please add atleast one message and then click on save button to save the message");
                return;
            }

            NavigateNext();
        }
    }
}