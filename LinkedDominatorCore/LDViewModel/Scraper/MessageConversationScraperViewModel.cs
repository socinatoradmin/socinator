using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.LDModel.Scraper;

namespace LinkedDominatorCore.LDViewModel.Scraper
{
    public class MessageConversationScraperViewModel : BindableBase
    {
        private MessageConversationScraperModel
            _messageConversationScraperModel = new MessageConversationScraperModel();

        public MessageConversationScraperViewModel()
        {
            MessageConversationScraperModel.ListQueryType.Clear();

            MessageConversationScraperModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName =
                    Application.Current.FindResource("LangKeyNumberOfScrapingPerJob")?.ToString(),
                ActivitiesPerHourDisplayName =
                    Application.Current.FindResource("LangKeyNumberOfScrapingPerHour")?.ToString(),
                ActivitiesPerDayDisplayName =
                    Application.Current.FindResource("LangKeyNumberOfScrapingPerDay")?.ToString(),
                ActivitiesPerWeekDisplayName =
                    Application.Current.FindResource("LangKeyNumberOfScrapingPerWeek")?.ToString(),
                IncreaseActivityDisplayName = Application.Current.FindResource("LangKeyMaxScrape")?.ToString(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };


            AddCustomUsersListCommand = new BaseCommand<object>(sender => true, AddCustomUsersListExecute);
        }

        public MessageConversationScraperModel MessageConversationScraperModel
        {
            get => _messageConversationScraperModel;
            set => SetProperty(ref _messageConversationScraperModel, value);
        }

        public MessageConversationScraperModel Model => MessageConversationScraperModel;

        #region Commands

        public ICommand AddCustomUsersListCommand { get; set; }

        #endregion

        public void AddCustomUsersListExecute(object sender)
        {
            try
            {
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}