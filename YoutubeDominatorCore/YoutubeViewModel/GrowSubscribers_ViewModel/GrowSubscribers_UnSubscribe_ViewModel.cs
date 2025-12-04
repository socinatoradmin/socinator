using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.YdQuery;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using YoutubeDominatorCore.YoutubeModels.GrowSubscribersModel;

namespace YoutubeDominatorCore.YoutubeViewModel.GrowSubscribers_ViewModel
{
    public class UnsubscribeViewModel : BindableBase
    {
        public UnsubscribeViewModel()
        {
            UnsubscribeModel.ListQueryType.Clear();
            UnsubscribeModel.ListQueryType.Add(Application.Current
                .FindResource(YdScraperParameters.CustomUrls.GetDescriptionAttr())?.ToString());

            UnsubscribeModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyUnsubscribesPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyUnsubscribesPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyUnsubscribesPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyUnsubscribesPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaximumUnsubscribesPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            AddQueryCommand = new BaseCommand<object>(sender => true, AddQuery);
            CustomFilterCommand = new BaseCommand<object>(sender => true, CustomFilter);
            DeleteQueryCommand = new BaseCommand<object>(sender => true, DeleteQuery);
            UploadChannelsListCommand = new BaseCommand<object>(sender => true, CustomChannel);
        }

        public UnsubscribeModel Model => UnsubscribeModel;

        #region Object creation logic

        private UnsubscribeModel _unsubscribeModel = new UnsubscribeModel();

        public UnsubscribeModel UnsubscribeModel
        {
            get => _unsubscribeModel;
            set
            {
                if ((_unsubscribeModel == null) & (_unsubscribeModel == value))
                    return;
                SetProperty(ref _unsubscribeModel, value);
            }
        }

        #endregion

        #region Commands

        public ICommand AddQueryCommand { get; set; }
        public ICommand CustomFilterCommand { get; set; }
        public ICommand DeleteQueryCommand { get; set; }
        public ICommand UploadChannelsListCommand { get; set; }

        #endregion

        #region Methods

        private void CustomFilter(object sender)
        {
            try
            {
                var moduleSettingsUserControl =
                    sender as ModuleSettingsUserControl<UnsubscribeViewModel, UnsubscribeModel>;
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
                    sender as ModuleSettingsUserControl<UnsubscribeViewModel, UnsubscribeModel>;

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

        private void CustomChannel(object sender)
        {
            try
            {
                UnsubscribeModel.ListCustomChannels = Regex.Split(UnsubscribeModel.CustomChannelsList, "\r\n").ToList();
                ToasterNotification.ShowSuccess(
                    $"Successfully added {UnsubscribeModel.ListCustomChannels.Count} Channel urls.");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #endregion
    }
}