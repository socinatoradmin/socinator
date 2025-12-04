using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.FdQuery;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDModel.ScraperModel;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace FaceDominatorCore.FDViewModel.ScraperViewModel
{
    public class GroupScraperViewModel : BindableBase
    {
        public GroupScraperViewModel()
        {
            GroupScraperModel.ListQueryType.Clear();

            Enum.GetValues(typeof(GroupScraperParameter)).Cast<GroupScraperParameter>().ToList().ForEach(query =>
            {
                GroupScraperModel.ListQueryType.Add(Application.Current.FindResource(query.GetDescriptionAttr())?.ToString());
            });




            GroupScraperModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = Application.Current.FindResource("LangKeyScrapNumberOfGroupsPerJob")?.ToString(),
                ActivitiesPerHourDisplayName = Application.Current.FindResource("LangKeyScrapNumberOfGroupsPerHour")?.ToString(),
                ActivitiesPerDayDisplayName = Application.Current.FindResource("LangKeyScrapNumberOfGroupsPerDay")?.ToString(),
                ActivitiesPerWeekDisplayName = Application.Current.FindResource("LangKeyScrapNumberOfGroupsPerWeek")?.ToString(),
                IncreaseActivityDisplayName = Application.Current.FindResource("LangKeyScrapMaxGroupsPerDay")?.ToString(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };

            AddQueryCommand = new BaseCommand<object>((sender) => true, AddQuery);
            CustomFilterCommand = new BaseCommand<object>((sender) => true, CustomFilter);
            DeleteQueryCommand = new BaseCommand<object>((sender) => true, DeleteQuery);
            AddMessagesCommand = new BaseCommand<object>((sender) => true, AddMessages);
            DeleteMulipleCommand = new BaseCommand<object>((sender) => true, DeleteMuliple);
        }

        #region Commands

        public ICommand AddQueryCommand { get; set; }
        public ICommand CustomFilterCommand { get; set; }
        public ICommand DeleteQueryCommand { get; set; }
        public ICommand AddMessagesCommand { get; set; }
        public ICommand DeleteMulipleCommand { get; set; }

        #endregion

        #region Methods

        private void CustomFilter(object sender)
        {
            try
            {
                var moduleSettingsUserControl = sender as DominatorUIUtility.CustomControl.ModuleSettingsUserControl<GroupScraperViewModel, GroupScraperModel>;
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
                var moduleSettingsUserControl = sender as DominatorUIUtility.CustomControl.ModuleSettingsUserControl<GroupScraperViewModel, GroupScraperModel>;

                moduleSettingsUserControl?.AddQuery(typeof(GroupScraperParameter));
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

        private void AddMessages(object sender)
        {
            //var messageData = sender as MessagesControl;

            //if (messageData == null) return;

            //messageData.Messages.SelectedQuery = new ObservableCollection<QueryContent>(messageData.Messages.LstQueries.Where(x => x.IsContentSelected));
            //// messageData.Messages.MessageId = ObjViewModel.BroadcastMessagesModel.LstDisplayManageMessageModel.Count + 1;
            //messageData.Messages.SelectedQuery.Remove(messageData.Messages.SelectedQuery.FirstOrDefault(x => x.Content.QueryValue == "All"));



            //messageData.Messages.LstQueries.Select(query => { query.IsContentSelected = false; return query; }).ToList();


        }

        private void DeleteMuliple(object sender)
        {
            var selectedQuery = Model.SavedQueries.Where(x => x.IsQuerySelected).ToList();
            try
            {
                foreach (var currentQuery in selectedQuery)
                {
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
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

        }

        #endregion


        public GroupScraperModel Model => GroupScraperModel;

        private GroupScraperModel _groupScraperModel = new GroupScraperModel();

        public GroupScraperModel GroupScraperModel
        {
            get
            {
                return _groupScraperModel;
            }
            set
            {
                if (_groupScraperModel == null & _groupScraperModel == value)
                    return;
                SetProperty(ref _groupScraperModel, value);
            }
        }
    }
}
