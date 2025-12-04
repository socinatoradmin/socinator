using System;
using System.Linq;
using System.Windows.Input;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.LDModel.Messenger;

namespace LinkedDominatorCore.LDViewModel.Messenger
{
    public class DeleteConversationsViewModel : BindableBase
    {
        private DeleteConversationsModel _DeleteConversationsModel = new DeleteConversationsModel();

        public DeleteConversationsViewModel()
        {
            DeleteConversationsModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfMessagesPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfMessagesPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfMessagesPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfMessagesPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxConnectionsToMessagePerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            SaveMessageCommand = new BaseCommand<object>(sender => true, SaveMessageExecute);
        }

        public DeleteConversationsModel DeleteConversationsModel
        {
            get => _DeleteConversationsModel;
            set => SetProperty(ref _DeleteConversationsModel, value);
        }

        public DeleteConversationsModel Model => DeleteConversationsModel;

        public ICommand SaveMessageCommand { get; set; }

        private void SaveMessageExecute(object obj)
        {
            DeleteConversationsModel.UrlList = DeleteConversationsModel.UrlInput.Split('\n')
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim()).ToList();
        }
    }
}