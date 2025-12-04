using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using QuoraDominatorCore.Models;
using UserQueryParameters = DominatorHouseCore.Enums.QdQuery.UserQueryParameters;

namespace QuoraDominatorCore.ViewModel.Scrape
{
    public class UserScraperViewModel : BindableBase
    {
        private UserScraperModel _userScraperModel = new UserScraperModel();

        public UserScraperViewModel()
        {
            UserScraperModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = Application.Current.FindResource("LangKeyScrapeUserPerJob")?.ToString(),
                ActivitiesPerHourDisplayName = Application.Current.FindResource("LangKeyScrapeUserPerHour")?.ToString(),
                ActivitiesPerDayDisplayName = Application.Current.FindResource("LangKeyScrapeUserPerDay")?.ToString(),
                ActivitiesPerWeekDisplayName = Application.Current.FindResource("LangKeyScrapeUserPerWeek")?.ToString(),
                IncreaseActivityDisplayName =
                    Application.Current.FindResource("LangKeyMaxScrapeUserPerDay")?.ToString(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };


            UserScraperModel.ListQueryType.Clear();

            Enum.GetValues(typeof(UserQueryParameters)).Cast<UserQueryParameters>().ToList().ForEach(query =>
            {
                UserScraperModel.ListQueryType.Add(Application.Current.FindResource(query.GetDescriptionAttr())
                    ?.ToString());
            });
            UserScraperModel.ListQueryType.Remove("Custom Users");
            AddQueryCommand = new BaseCommand<object>(sender => true, AddQuery);
            CustomFilterCommand = new BaseCommand<object>(sender => true, CustomFilter);
        }

        public ICommand AddQueryCommand { get; set; }
        public ICommand CustomFilterCommand { get; set; }

        public UserScraperModel UserScraperModel
        {
            get => _userScraperModel;
            set
            {
                if ((_userScraperModel == null) & (_userScraperModel == value))
                    return;
                SetProperty(ref _userScraperModel, value);
            }
        }

        public UserScraperModel Model => UserScraperModel;

        private void AddQuery(object sender)
        {
            try
            {
                var moduleSettingsUserControl =
                    sender as ModuleSettingsUserControl<UserScraperViewModel, UserScraperModel>;
                moduleSettingsUserControl?.AddQuery(typeof(UserQueryParameters));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void CustomFilter(object sender)
        {
            try
            {
                var moduleSettingsUserControl =
                    sender as ModuleSettingsUserControl<UserScraperViewModel, UserScraperModel>;
                moduleSettingsUserControl?.CustomFilter();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}