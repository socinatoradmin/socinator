using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.FdQuery;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using FaceDominatorCore.FDModel.FriendsModel;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace FaceDominatorCore.FDViewModel.FriendsViewModel
{
    public class SendFrinedRequestViewModel : BindableBase
    {
        public SendFrinedRequestViewModel()
        {
            SendFriendRequestModel.ListQueryType.Clear();

            Enum.GetValues(typeof(FdUserQueryParameters)).Cast<FdUserQueryParameters>().ToList().ForEach(query =>
            {
                SendFriendRequestModel.ListQueryType.Add(Application.Current.FindResource(query.GetDescriptionAttr())?.ToString());
            });

            //We CAnnot Send Request To Already Friends will is filter of group member
            SendFriendRequestModel.GenderAndLocationCancelFilter.IsFriendsDropdownVisible = false;

            SendFriendRequestModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = Application.Current.FindResource("LangKeyNumberOfRequestPerJob")?.ToString(),
                ActivitiesPerHourDisplayName = Application.Current.FindResource("LangKeyNumberOfRequestPerHour")?.ToString(),
                ActivitiesPerDayDisplayName = Application.Current.FindResource("LangKeyNumberOfRequestPerDay")?.ToString(),
                ActivitiesPerWeekDisplayName = Application.Current.FindResource("LangKeyNumberOfRequestPerWeek")?.ToString(),
                IncreaseActivityDisplayName = Application.Current.FindResource("LangKeyMaxRequestPerDay")?.ToString(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };

            SendFriendRequestModel.GenderAndLocationFilter.IsFriendsDropdownVisible = false;

            AddQueryCommand = new BaseCommand<object>((sender) => true, AddQuery);
            CustomFilterCommand = new BaseCommand<object>((sender) => true, CustomFilter);
            DeleteQueryCommand = new BaseCommand<object>((sender) => true, DeleteQuery);
            AddMessagesCommand = new BaseCommand<object>((sender) => true, AddMessages);
            DeleteMulipleCommand = new BaseCommand<object>((sender) => true, DeleteMuliple);
            QuryTypeSelectionChangedCommand = new BaseCommand<object>((sender) => true, SelectionChangedCommandExecute);
        }


        //Get Global Database

        #region Commands

        public ICommand AddQueryCommand { get; set; }
        public ICommand CustomFilterCommand { get; set; }
        public ICommand DeleteQueryCommand { get; set; }
        public ICommand AddMessagesCommand { get; set; }
        public ICommand DeleteMulipleCommand { get; set; }

        public ICommand QuryTypeSelectionChangedCommand { get; set; }

        #endregion

        #region Methods

        private void SelectionChangedCommandExecute(object sender)
        {
            var messageData = sender as SearchQueryControl;

            if (messageData == null) return;

            var data = messageData.CurrentQuery.QueryType;

            if (data == Application.Current.FindResource("LangKeyGroupMembers")?.ToString()
                || messageData.ListQueryInfo.Any(x => x.QueryType == Application.Current.FindResource("LangKeyGroupMembers")?.ToString()))
                SendFriendRequestModel.GenderAndLocationFilter.IsGroupCategoryEnabled = true;
            else
                SendFriendRequestModel.GenderAndLocationFilter.IsGroupCategoryEnabled = false;
        }

        private void CustomFilter(object sender)
        {
            try
            {
                var moduleSettingsUserControl = sender as DominatorUIUtility.CustomControl.ModuleSettingsUserControl<SendFrinedRequestViewModel, SendFriendRequestModel>;
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
                var moduleSettingsUserControl = sender as ModuleSettingsUserControl<SendFrinedRequestViewModel, SendFriendRequestModel>;

                moduleSettingsUserControl?.AddQuery(typeof(FdUserQueryParameters));

                SendFriendRequestModel.GenderAndLocationFilter.IsGroupCategoryEnabled =
                        SendFriendRequestModel.SavedQueries.Any(x => x.QueryType == Application.Current.FindResource("LangKeyGroupMembers")?.ToString());
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

                SendFriendRequestModel.GenderAndLocationFilter.IsGroupCategoryEnabled =
                        SendFriendRequestModel.SavedQueries.Any(x => x.QueryType == Application.Current.FindResource("LangKeyGroupMembers")?.ToString());

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

                        SendFriendRequestModel.GenderAndLocationFilter.IsGroupCategoryEnabled =
                        SendFriendRequestModel.SavedQueries.Any(x => x.QueryType == Application.Current.FindResource("LangKeyGroupMembers")?.ToString());


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

        public SendFriendRequestModel Model => SendFriendRequestModel;

        private SendFriendRequestModel _sendFriendRequestModel = new SendFriendRequestModel();

        public SendFriendRequestModel SendFriendRequestModel
        {
            get
            {
                return _sendFriendRequestModel;
            }
            set
            {
                if (_sendFriendRequestModel == null & _sendFriendRequestModel == value)
                    return;
                SetProperty(ref _sendFriendRequestModel, value);
            }
        }

    }

}
