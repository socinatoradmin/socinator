using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.TdQuery;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using TwtDominatorCore.TDModels;

namespace TwtDominatorCore.TDViewModel.GrowFollower
{
    public class MuteViewModel : BindableBase
    {
        private MuteModel _muteModel = new MuteModel();

        public MuteViewModel()
        {
            MuteModel.ListQueryType.Clear();

            Enum.GetValues(typeof(TdUserInteractionQueryEnum)).Cast<TdUserInteractionQueryEnum>().ToList().ForEach(
                query =>
                {
                    MuteModel.ListQueryType.Add(Application.Current.FindResource(query.GetDescriptionAttr())
                        .ToString());
                });

            //  MuteModel.ListQueryType = Enum.GetNames(typeof(TdUserInteractionQueryEnum)).ToList();
            MuteModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfMutesPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfMutesPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfMutesPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfMutesPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxMutePerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };

            AddQueryCommand = new BaseCommand<object>(sender => true, AddQuery);
            CustomFilterCommand = new BaseCommand<object>(sender => true, CustomFilter);
            DeleteQueryCommand = new BaseCommand<object>(sender => true, DeleteQuery);
            DeleteMulipleCommand = new BaseCommand<object>(sender => true, DeleteMuliple);
        }

        public MuteModel MuteModel
        {
            get => _muteModel;
            set
            {
                if ((_muteModel == null) & (_muteModel == value))
                    return;
                SetProperty(ref _muteModel, value);
            }
        }

        public MuteModel Model => MuteModel;

        #region  ICommands

        public ICommand AddQueryCommand { get; set; }
        public ICommand CustomFilterCommand { get; set; }
        public ICommand DeleteQueryCommand { get; set; }
        public ICommand DeleteMulipleCommand { get; set; }

        #endregion

        #region Methods

        private void CustomFilter(object sender)
        {
            try
            {
                var ModuleSettingsUserControl = sender as ModuleSettingsUserControl<MuteViewModel, MuteModel>;
                ModuleSettingsUserControl.CustomFilter();
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
                var ModuleSettingsUserControl = sender as ModuleSettingsUserControl<MuteViewModel, MuteModel>;
                ModuleSettingsUserControl.AddQuery(typeof(TdUserInteractionQueryEnum));
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

        private void DeleteMuliple(object sender)
        {
            var selectedQuery = Model.SavedQueries.Where(x => x.IsQuerySelected).ToList();
            try
            {
                foreach (var currentQuery in selectedQuery)
                    try
                    {
                        if (Model.SavedQueries.Any(x => currentQuery != null && x.Id == currentQuery.Id))
                            Model.SavedQueries.Remove(currentQuery);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #endregion
    }
}