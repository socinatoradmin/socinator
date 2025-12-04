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
using Newtonsoft.Json;
using QuoraDominatorCore.Models;

namespace QuoraDominatorCore.ViewModel.Report
{
    public class ReportUserViewModel : BindableBase
    {
        private ReportUserModel _reportUserModel = new ReportUserModel();

        public ReportUserViewModel()
        {
            ReportUserModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName =
                    Application.Current.FindResource("LangKeyNumberOfUsersToReportPerJob")?.ToString(),
                ActivitiesPerHourDisplayName =
                    Application.Current.FindResource("LangKeyNumberOfUsersToReportPerHour")?.ToString(),
                ActivitiesPerDayDisplayName =
                    Application.Current.FindResource("LangKeyNumberOfUsersToReportPerDay")?.ToString(),
                ActivitiesPerWeekDisplayName =
                    Application.Current.FindResource("LangKeyNumberOfUsersToReportPerWeek")?.ToString(),
                IncreaseActivityDisplayName =
                    Application.Current.FindResource("LangKeyMaxUsersToReportPerDay")?.ToString(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            ReportUserModel.ListQueryType = Enum.GetNames(typeof(UserQueryParameters)).ToList();
            ReportUserModel.ListQueryType.Remove("OwnFollowers");
            ReportUserModel.ListQueryType.Clear();

            Enum.GetValues(typeof(DominatorHouseCore.Enums.QdQuery.UserQueryParameters))
                .Cast<DominatorHouseCore.Enums.QdQuery.UserQueryParameters>().ToList().ForEach(query =>
                {
                    ReportUserModel.ListQueryType.Add(Application.Current.FindResource(query.GetDescriptionAttr())
                        ?.ToString());
                });
            ReportUserModel.ReportOptions.Clear();
            ReportUserModel.ReportOptions.Add("User Credential");
            ReportUserModel.ReportOptions.Add("User description");
            ReportUserModel.ReportOptions.Add("User photo");
            ReportUserModel.ReportOptions.Add("User name");
            if(string.IsNullOrEmpty(ReportUserModel.SelectedOption))
                ReportUserModel.SelectedOption=ReportUserModel.ReportOptions.FirstOrDefault();
            AddQueryCommand = new BaseCommand<object>(sender => true, AddQuery);
            CustomFilterCommand = new BaseCommand<object>(sender => true, CustomFilter);
            SaveReports = new BaseCommand<object>(sender => true, SaveReportsExecute);
        }
        public ReportUserModel ReportUserModel
        {
            get => _reportUserModel;
            set
            {
                if ((_reportUserModel == null) & (_reportUserModel == value))
                    return;
                SetProperty(ref _reportUserModel, value);
            }
        }

        public ReportUserModel Model => ReportUserModel;

        #region Commands

        public ICommand AddQueryCommand { get; set; }
        public ICommand CustomFilterCommand { get; set; }
        public ICommand SaveReports { get;set; }
        #endregion

        #region Methods
        public void SaveReportsExecute(object sender)
        {

            try
            {
                if (Model.SavedQueries.Count > 0)
                {
                    var Description = Model.ReportDescription.ToString();
                    Model.SavedQueries.ForEach(query =>
                    {
                        Model.SelectedSubOption.ReportOptionTitle = Model.SelectedOption;
                        Model.SelectedSubOption.ReportDescription = Description;
                        query.CustomFilters = JsonConvert.SerializeObject(Model.SelectedSubOption);
                    });
                    ToasterNotification.ShowInfomation("Reports Saved Successfully");
                    Model.ReportDescription = string.Empty;
                }
                else
                    ToasterNotification.ShowInfomation("Please Add Atleast One Query");
                
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
                    sender as ModuleSettingsUserControl<ReportUserViewModel, ReportUserModel>;
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
                    sender as ModuleSettingsUserControl<ReportUserViewModel, ReportUserModel>;
                moduleSettingsUserControl?.AddQuery(typeof(UserQueryParameters));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #endregion
    }
}