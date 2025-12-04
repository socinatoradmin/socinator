using System;
using System.Linq;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.GdQuery;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDModel;
using System.Windows.Input;
using DominatorHouseCore.Command;
using DominatorHouseCore;

namespace GramDominatorCore.GDViewModel.InstaPoster
{
    public class RePosterViewModel : BindableBase
    {
        public RePosterViewModel()
        {
            // RePosterModel.ListQueryType = Enum.GetNames(typeof(GdPostQuery)).ToList();

            Enum.GetValues(typeof(GdPostQuery)).Cast<GdPostQuery>().ToList().ForEach(query =>
            {
                RePosterModel.ListQueryType.Add(Application.Current.FindResource(query.GetDescriptionAttr())?.ToString());
            });
         
            RePosterModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = Application.Current.FindResource("LangKeyNumberOfRepostPerJob")?.ToString(),
                ActivitiesPerHourDisplayName = Application.Current.FindResource("LangKeyNumberOfRepostPerHour")?.ToString(),
                ActivitiesPerDayDisplayName = Application.Current.FindResource("LangKeyNumberOfRepostPerDay")?.ToString(),
                ActivitiesPerWeekDisplayName = Application.Current.FindResource("LangKeyNumberOfRepostPerWeek")?.ToString(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };

            AddQueryCommand = new BaseCommand<object>((sender) => true, AddQuery);
            CustomFilterCommand = new BaseCommand<object>((sender) => true, CustomFilter);
        }

        #region Commands

        public ICommand AddQueryCommand { get; set; }
        public ICommand CustomFilterCommand { get; set; }
       // public ICommand DeleteQueryCommand { get; set; }
       // public ICommand AddMessagesCommand { get; set; }
       // public ICommand DeleteMultipleCommand { get; set; }

        #endregion


        #region Command Implemented Methods

        private void AddQuery(object sender)
        {
            try
            {
                var ModuleSettingsUserControl = sender as DominatorUIUtility.CustomControl.ModuleSettingsUserControl<RePosterViewModel, RePosterModel>;
                if (ModuleSettingsUserControl != null) ModuleSettingsUserControl.AddQuery(typeof(GdUserQuery));
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
                var ModuleSettingsUserControl = sender as DominatorUIUtility.CustomControl.ModuleSettingsUserControl<RePosterViewModel, RePosterModel>;
                if (ModuleSettingsUserControl != null) ModuleSettingsUserControl.CustomFilter();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #endregion


        private RePosterModel _rePosterModel = new RePosterModel();

        public RePosterModel RePosterModel
        {
            get
            {
                return _rePosterModel;
            }
            set
            {
                if (_rePosterModel == null & _rePosterModel == value)
                    return;
                SetProperty(ref _rePosterModel, value);
            }
        }
        public RePosterModel Model => RePosterModel;

    }
}
