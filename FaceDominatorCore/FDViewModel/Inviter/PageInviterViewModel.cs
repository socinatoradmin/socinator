using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDModel.InviterModel;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace FaceDominatorCore.FDViewModel.Inviter
{
    public class PageInviterViewModel : BindableBase
    {

        public PageInviterViewModel()
        {
            if (FanpageInviterModel.SelectAccountDetailsModel == null)
            {
                FanpageInviterModel.SelectAccountDetailsModel = new SelectAccountDetailsModel();
            }

            FanpageInviterModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = Application.Current.FindResource("LangKeyInviteNumberOfProfilesPerJob")?.ToString(),
                ActivitiesPerHourDisplayName = Application.Current.FindResource("LangKeyInviteNumberOfProfilesPerHour")?.ToString(),
                ActivitiesPerDayDisplayName = Application.Current.FindResource("LangKeyInviteNumberOfProfilesPerDay")?.ToString(),
                ActivitiesPerWeekDisplayName = Application.Current.FindResource("LangKeyInviteToNumberOfProfilesPerWeek")?.ToString(),
                IncreaseActivityDisplayName = Application.Current.FindResource("LangKeyInviteMaxProfilesPerDay")?.ToString(),
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
                var moduleSettingsUserControl = sender as DominatorUIUtility.CustomControl.ModuleSettingsUserControl<PageInviterViewModel, FanpageInviterModel>;
                moduleSettingsUserControl?.CustomFilter();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void AddQuery(object sender)
        {
            //try
            //{

            //    var ModuleSettingsUserControl = sender as DominatorUIUtility.CustomControl.ModuleSettingsUserControl<CancenSentRequestViewModel, CancelSentRequestModel>;

            //    ModuleSettingsUserControl.AddQuery(typeof(FdUserQueryParameters));
            //}
            //catch (Exception ex)
            //{
            //    ex.DebugLog();
            //}
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

        public FanpageInviterModel Model => FanpageInviterModel;

        private FanpageInviterModel _fanpageInviterModel = new FanpageInviterModel();

        public FanpageInviterModel FanpageInviterModel
        {
            get
            {
                return _fanpageInviterModel;
            }
            set
            {
                if (_fanpageInviterModel == null & _fanpageInviterModel == value)
                    return;
                SetProperty(ref _fanpageInviterModel, value);
            }
        }
    }
}
