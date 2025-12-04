using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using RedditDominatorCore.RDModel;
using System;
using System.Linq;

namespace RedditDominatorCore.RDViewModel
{
    public class AutoReplyViewModel : BindableBase
    {
        private AutoReplyModel _messageModel = new AutoReplyModel();
        public AutoReplyModel MessageModel
        {
            get { return _messageModel; }
            set { SetProperty(ref _messageModel, value); }
        }
        public AutoReplyViewModel()
        {
            MessageModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfMessagesPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfMessagesPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfMessagesPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfMessagesPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxMessagePerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
        }
        public AutoReplyModel Model => MessageModel;
    }
}
