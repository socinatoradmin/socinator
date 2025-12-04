using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.PdQuery;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using PinDominatorCore.PDModel;

namespace PinDominatorCore.PDViewModel.PinScraper
{
    public class UserScraperViewModel : BindableBase
    {
        private UserScraperModel _userScraperModel = new UserScraperModel();

        public UserScraperViewModel()
        {
            UserScraperModel.ListQueryType.Clear();

            Enum.GetValues(typeof(PDUsersQueries)).Cast<PDUsersQueries>().ToList().ForEach(query =>
            {
                if(query!=PDUsersQueries.UsersWhoTriedPins)
                    UserScraperModel.ListQueryType.Add(Application.Current.FindResource(query.GetDescriptionAttr())
                    ?.ToString());
            });
             UserScraperModel.ListQueryType.Remove(Application.Current
                .FindResource(PDUsersQueries.CustomBoard.GetDescriptionAttr())?.ToString());
            UserScraperModel.ListQueryType.Remove(Application.Current
                .FindResource(PDUsersQueries.BoardsbyKeywords.GetDescriptionAttr())?.ToString());

            // Load job configuration values
            UserScraperModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfUserScrapPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfUserScrapPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfUserScrapPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfUserScrapPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxUserScrapPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            AddQueryCommand = new BaseCommand<object>(sender => true, AddQuery);
            CustomFilterCommand = new BaseCommand<object>(sender => true, CustomFilter);
        }

        public UserScraperModel Model => UserScraperModel;

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

        #region Commands

        public ICommand AddQueryCommand { get; set; }
        public ICommand CustomFilterCommand { get; set; }

        #endregion

        #region Methods

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

        private void AddQuery(object sender)
        {
            try
            {
                var moduleSettingsUserControl =
                    sender as ModuleSettingsUserControl<UserScraperViewModel, UserScraperModel>;

                moduleSettingsUserControl?.AddQuery(typeof(PDUsersQueries), Model.ListQueryType);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #endregion
    }
}