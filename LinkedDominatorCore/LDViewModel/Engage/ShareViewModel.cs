using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.LdQuery;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using LinkedDominatorCore.LDModel.Engage;

namespace LinkedDominatorCore.LDViewModel.Engage
{
    public class ShareViewModel : BindableBase
    {
        private ShareModel _shareModel = new ShareModel();

        public ShareViewModel()
        {
            ShareModel.ListQueryType.Clear();
            Enum.GetValues(typeof(LDEngageQueryParameters)).Cast<LDEngageQueryParameters>().ForEach(QueryType =>
            {
                if (QueryType == LDEngageQueryParameters.SomeonesPosts ||
                    QueryType == LDEngageQueryParameters.MyConnectionsPosts)
                    ShareModel.ListQueryType.Add(QueryType.GetDescriptionAttr().FromResourceDictionary());
            });

            ShareModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName =
                    Application.Current.FindResource("LangKeyNumberOfSharesPerJob")?.ToString(),
                ActivitiesPerHourDisplayName =
                    Application.Current.FindResource("LangKeyNumberOfSharesPerHour")?.ToString(),
                ActivitiesPerDayDisplayName =
                    Application.Current.FindResource("LangKeyNumberOfSharesPerDay")?.ToString(),
                ActivitiesPerWeekDisplayName =
                    Application.Current.FindResource("LangKeyNumberOfSharesPerWeek")?.ToString(),
                IncreaseActivityDisplayName = Application.Current.FindResource("LangKeyMaxSharesPerDay")?.ToString(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            AddQueryCommand = new BaseCommand<object>(sender => true, AddQuery);
            DeleteQueryCommand = new BaseCommand<object>(sender => true, DeleteQuery);
            DeleteMulipleCommand = new BaseCommand<object>(sender => true, DeleteMuliple);
            CustomFilterCommand = new BaseCommand<object>(sender => true, CustomFilter);
        }

        public ShareModel ShareModel
        {
            get => _shareModel;
            set
            {
                if ((_shareModel == null) & (_shareModel == value))
                    return;
                SetProperty(ref _shareModel, value);
            }
        }

        public ShareModel Model => ShareModel;

        public void AddQuery(object sender)
        {
            try
            {
                var ModuleSettingsUserControl = sender as ModuleSettingsUserControl<ShareViewModel, ShareModel>;
                ModuleSettingsUserControl.AddQuery(typeof(LDEngageQueryParameters));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void DeleteQuery(object sender)
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

        public void DeleteMuliple(object sender)
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

        private void CustomFilter(object sender)
        {
            try
            {
                var ModuleSettingsUserControl = sender as ModuleSettingsUserControl<LikeViewModel, LikeModel>;
                ModuleSettingsUserControl.CustomFilter();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #region Commands

        public ICommand AddQueryCommand { get; set; }
        public ICommand CustomFilterCommand { get; set; }
        public ICommand DeleteQueryCommand { get; set; }
        public ICommand DeleteMulipleCommand { get; set; }

        #endregion
    }
}