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
    public class BoardScraperViewModel : BindableBase
    {
        private BoardScraperModel _boardScraperModel = new BoardScraperModel();

        public BoardScraperViewModel()
        {
            BoardScraperModel.ListQueryType.Add(Application.Current
                .FindResource(PDUsersQueries.Keywords.GetDescriptionAttr())?.ToString());
            BoardScraperModel.ListQueryType.Add(Application.Current
                .FindResource(PDUsersQueries.Customusers.GetDescriptionAttr())?.ToString());
            BoardScraperModel.ListQueryType.Add(Application.Current
                .FindResource(PDUsersQueries.CustomBoard.GetDescriptionAttr())?.ToString());

            // Load job configuration values
            BoardScraperModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfBoardScrapPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfBoardScrapPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfBoardScrapPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfBoardScrapPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxBoardScrapPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            AddQueryCommand = new BaseCommand<object>(sender => true, AddQuery);
            CustomFilterCommand = new BaseCommand<object>(sender => true, CustomFilter);
        }

        public BoardScraperModel Model => BoardScraperModel;

        public BoardScraperModel BoardScraperModel
        {
            get => _boardScraperModel;
            set
            {
                if ((_boardScraperModel == null) & (_boardScraperModel == value))
                    return;
                SetProperty(ref _boardScraperModel, value);
            }
        }

        #region Commands

        public ICommand AddQueryCommand { get; set; }
        public ICommand CustomFilterCommand { get; set; }
        public ICommand AddMessagesCommand { get; set; }

        #endregion

        #region Methods

        private void CustomFilter(object sender)
        {
            try
            {
                var moduleSettingsUserControl =
                    sender as ModuleSettingsUserControl<BoardScraperViewModel, BoardScraperModel>;
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
                    sender as ModuleSettingsUserControl<BoardScraperViewModel, BoardScraperModel>;

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