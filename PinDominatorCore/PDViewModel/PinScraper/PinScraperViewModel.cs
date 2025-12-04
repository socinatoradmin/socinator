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
    public class PinScraperViewModel : BindableBase
    {
        private PinScraperModel _pinScraperModel = new PinScraperModel();

        public PinScraperViewModel()
        {
            PinScraperModel.ListQueryType.Clear();

            Enum.GetValues(typeof(PDPinQueries)).Cast<PDPinQueries>().ToList().ForEach(query =>
            {
                PinScraperModel.ListQueryType.Add(Application.Current
                    .FindResource(query.GetDescriptionAttr())?.ToString());
            });
            // Load job configuration values
            PinScraperModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfPinScrapPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfPinScrapPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfPinScrapPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfPinScrapPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxPinScrapPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            AddQueryCommand = new BaseCommand<object>(sender => true, AddQuery);
            CustomFilterCommand = new BaseCommand<object>(sender => true, CustomFilter);
            DownloadedPathCommand = new BaseCommand<object>(sender => true, DownloadedPathExecute);
        }

        public PinScraperModel Model => PinScraperModel;

        public PinScraperModel PinScraperModel
        {
            get => _pinScraperModel;
            set
            {
                if ((_pinScraperModel == null) & (_pinScraperModel == value))
                    return;
                SetProperty(ref _pinScraperModel, value);
            }
        }

        #region Commands

        public ICommand AddQueryCommand { get; set; }
        public ICommand CustomFilterCommand { get; set; }
        public ICommand DownloadedPathCommand { get; set; }

        #endregion

        #region Methods

        private void CustomFilter(object sender)
        {
            try
            {
                var moduleSettingsUserControl =
                    sender as ModuleSettingsUserControl<PinScraperViewModel, PinScraperModel>;
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
                    sender as ModuleSettingsUserControl<PinScraperViewModel, PinScraperModel>;

                moduleSettingsUserControl?.AddQuery(typeof(PDPinQueries));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void DownloadedPathExecute(object sender)
        {
            PinScraperModel.DownloadedFolderPath = FileUtilities.GetExportPath();
        }

        #endregion
    }
}