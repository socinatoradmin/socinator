using System;
using System.Linq;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using QuoraDominatorCore.Models;

namespace QuoraDominatorCore.ViewModel.Messages
{
    public class AutoReplyToNewMessageViewModel : BindableBase
    {
        private AutoReplyToNewMessageModel _autoReplyToNewMessageModel = new AutoReplyToNewMessageModel();

        public AutoReplyToNewMessageViewModel()
        {
            AutoReplyToNewMessageModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfAutoReplyToNewMessegePerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfAutoReplyToNewMessegePerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfAutoReplyToNewMessegePerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfAutoReplyToNewMessegePerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxAutoReplyToNewMessegePerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
        }

        public bool IsReplyToPendingMessagesChecked { get; set; }

        public AutoReplyToNewMessageModel AutoReplyToNewMessageModel
        {
            get => _autoReplyToNewMessageModel;
            set
            {
                if ((_autoReplyToNewMessageModel == null) & (_autoReplyToNewMessageModel == value))
                    return;
                SetProperty(ref _autoReplyToNewMessageModel, value);
            }
        }

        public AutoReplyToNewMessageModel Model => AutoReplyToNewMessageModel;
    }
}