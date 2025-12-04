using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.FdQuery;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDModel.LikerCommentorModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace FaceDominatorCore.FDViewModel.LikerCommentorViewModel
{
    public class FanpageLikerViewModel : BindableBase
    {
        public FanpageLikerViewModel()
        {
            FanpageLikerModel.ListQueryType.Clear();

            Enum.GetValues(typeof(FanpageLikerQueryParameters)).Cast<FanpageLikerQueryParameters>().ToList().ForEach(query =>
            {
                FanpageLikerModel.ListQueryType.Add(Application.Current.FindResource(query.GetDescriptionAttr())?.ToString());
            });



            FanpageLikerModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = Application.Current.FindResource("LangKeyNumberOfLikesPerJob")?.ToString(),
                ActivitiesPerHourDisplayName = Application.Current.FindResource("LangKeyNumberOfLikesPerHour")?.ToString(),
                ActivitiesPerDayDisplayName = Application.Current.FindResource("LangKeyNumberOfLikesPerDay")?.ToString(),
                ActivitiesPerWeekDisplayName = Application.Current.FindResource("LangKeyNumberOfLikesPerWeek")?.ToString(),
                IncreaseActivityDisplayName = Application.Current.FindResource("LangKeyMaxLikesPerDay")?.ToString(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };

            AddQueryCommand = new BaseCommand<object>((sender) => true, AddQuery);
            CustomFilterCommand = new BaseCommand<object>((sender) => true, CustomFilter);
            DeleteQueryCommand = new BaseCommand<object>((sender) => true, DeleteQuery);
            AddMessagesCommand = new BaseCommand<object>((sender) => true, AddMessages);
            DeleteMulipleCommand = new BaseCommand<object>((sender) => true, DeleteMuliple);
            CheckUncheckCommand = new BaseCommand<object>((sender) => true, CheckUncheckOwnPage);
        }

        private void CheckUncheckOwnPage(object obj)
        {
            if (!FanpageLikerModel.IsActionasPageChecked)
            {
                FanpageLikerModel.ListOwnPageUrl = new List<string>();
                FanpageLikerModel.OwnPageUrl = string.Empty;
            }
        }

        #region Commands

        public ICommand AddQueryCommand { get; set; }
        public ICommand CustomFilterCommand { get; set; }
        public ICommand DeleteQueryCommand { get; set; }
        public ICommand AddMessagesCommand { get; set; }
        public ICommand DeleteMulipleCommand { get; set; }
        public ICommand CheckUncheckCommand { get; set; }

        #endregion

        #region Methods

        private void CustomFilter(object sender)
        {
            try
            {
                var moduleSettingsUserControl = sender as DominatorUIUtility.CustomControl.ModuleSettingsUserControl<FanpageLikerViewModel, FanpageLikerModel>;
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
                var moduleSettingsUserControl = sender as DominatorUIUtility.CustomControl.ModuleSettingsUserControl<FanpageLikerViewModel, FanpageLikerModel>;

                moduleSettingsUserControl?.AddQuery(typeof(FanpageLikerQueryParameters));
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

        public FanpageLikerModel Model => FanpageLikerModel;

        private FanpageLikerModel _fanpageLikerModel = new FanpageLikerModel();

        public FanpageLikerModel FanpageLikerModel
        {
            get
            {
                return _fanpageLikerModel;
            }
            set
            {
                if (_fanpageLikerModel == null & _fanpageLikerModel == value)
                    return;
                SetProperty(ref _fanpageLikerModel, value);
            }
        }

    }
}
