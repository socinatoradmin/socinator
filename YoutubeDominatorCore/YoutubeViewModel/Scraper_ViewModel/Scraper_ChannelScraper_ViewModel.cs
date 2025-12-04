using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.YdQuery;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using YoutubeDominatorCore.YoutubeModels.ScraperModel;

namespace YoutubeDominatorCore.YoutubeViewModel.Scraper_ViewModel
{
    public class ChannelScraperViewModel : BindableBase
    {
        public ChannelScraperViewModel()
        {
            ChannelScraperModel.ListQueryType.Clear();
            Enum.GetValues(typeof(YdScraperParameters)).Cast<YdScraperParameters>().ToList().ForEach(query =>
            {
                if (query != YdScraperParameters.CustomChannel)
                    ChannelScraperModel.ListQueryType.Add(Application.Current.FindResource(query.GetDescriptionAttr())
                        .ToString());
            });

            ChannelScraperModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyChannelsScrapPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyChannelsScrapPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyChannelsScrapPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyChannelsScrapPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaximumChannelsScrapPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            AddQueryCommand = new BaseCommand<object>(sender => true, AddQuery);
            CustomFilterCommand = new BaseCommand<object>(sender => true, CustomFilter);
            DeleteQueryCommand = new BaseCommand<object>(sender => true, DeleteQuery);
        }

        public ChannelScraperModel Model => ChannelScraperModel;

        #region Object creation logic

        private ChannelScraperModel _channelScraperModel = new ChannelScraperModel();


        public ChannelScraperModel ChannelScraperModel
        {
            get => _channelScraperModel;
            set
            {
                if ((_channelScraperModel == null) & (_channelScraperModel == value))
                    return;
                SetProperty(ref _channelScraperModel, value);
            }
        }

        #endregion

        #region Commands

        public ICommand AddQueryCommand { get; set; }
        public ICommand CustomFilterCommand { get; set; }
        public ICommand DeleteQueryCommand { get; set; }

        #endregion

        #region Methods

        private void CustomFilter(object sender)
        {
            try
            {
                var moduleSettingsUserControl =
                    sender as ModuleSettingsUserControl<ChannelScraperViewModel, ChannelScraperModel>;
                moduleSettingsUserControl.CustomFilter();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void AddQuery(object sender)
        {
            try
            {
                var moduleSettingsUserControl =
                    sender as ModuleSettingsUserControl<ChannelScraperViewModel, ChannelScraperModel>;

                moduleSettingsUserControl.AddQuery(typeof(YdScraperParameters), Model.ListQueryType);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void DeleteQuery(object sender)
        {
            try
            {
                var currentQuery = sender as QueryInfo;

                if (Model.SavedQueries.Any(x => currentQuery != null && x.Id == currentQuery.Id))
                    Model.SavedQueries.Remove(currentQuery);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #endregion
    }
}