using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.FdQuery;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using FaceDominatorCore.FDModel.ScraperModel;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace FaceDominatorCore.FDViewModel.ScraperViewModel
{
    public class ProfileScraperViewModel : BindableBase
    {

        public ProfileScraperViewModel()
        {
            ProfileScraperModel.ListQueryType.Clear();

            Enum.GetValues(typeof(FdProfileQueryParameters)).Cast<FdProfileQueryParameters>().ToList().ForEach(query =>
            {
                ProfileScraperModel.ListQueryType.Add(Application.Current.FindResource(query.GetDescriptionAttr())?.ToString());
            });


            ProfileScraperModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = Application.Current.FindResource("LangKeyScrapNumberOfProfilesPerJob")?.ToString(),
                ActivitiesPerHourDisplayName = Application.Current.FindResource("LangKeyScrapNumberOfProfilesPerHour")?.ToString(),
                ActivitiesPerDayDisplayName = Application.Current.FindResource("LangKeyScrapNumberOfProfilesPerDay")?.ToString(),
                ActivitiesPerWeekDisplayName = Application.Current.FindResource("LangKeyScrapNumberOfProfilesPerWeek")?.ToString(),
                IncreaseActivityDisplayName = Application.Current.FindResource("LangKeyScrapMaxProfilesPerDay")?.ToString(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };


            AddQueryCommand = new BaseCommand<object>((sender) => true, AddQuery);
            CustomFilterCommand = new BaseCommand<object>((sender) => true, CustomFilter);
            DeleteQueryCommand = new BaseCommand<object>((sender) => true, DeleteQuery);
            AddMessagesCommand = new BaseCommand<object>((sender) => true, AddMessages);
            DeleteMulipleCommand = new BaseCommand<object>((sender) => true, DeleteMuliple);
            QuryTypeSelectionChangedCommand = new BaseCommand<object>((sender) => true, SelectionChangedCommandExecute);
        }

        private void SelectionChangedCommandExecute(object sender)
        {
            var messageData = sender as SearchQueryControl;

            if (messageData == null) return;

            var data = messageData.CurrentQuery.QueryType;

            if (data == Application.Current.FindResource("LangKeyGroupMembers")?.ToString()
                || messageData.ListQueryInfo.Any(x => x.QueryType == Application.Current.FindResource("LangKeyGroupMembers")?.ToString()))
                ProfileScraperModel.GenderAndLocationFilter.IsGroupCategoryEnabled = true;
            else
                ProfileScraperModel.GenderAndLocationFilter.IsGroupCategoryEnabled = false;
        }

        #region Commands

        public ICommand AddQueryCommand { get; set; }
        public ICommand CustomFilterCommand { get; set; }
        public ICommand DeleteQueryCommand { get; set; }
        public ICommand AddMessagesCommand { get; set; }
        public ICommand DeleteMulipleCommand { get; set; }
        public ICommand QuryTypeSelectionChangedCommand { get; set; }
        #endregion

        #region Methods

        private void CustomFilter(object sender)
        {
            try
            {
                var moduleSettingsUserControl = sender as ModuleSettingsUserControl<ProfileScraperViewModel, ProfileScraperModel>;
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
                ProfileScraperModel.GenderAndLocationFilter.IsTimeLimitChecked
                    = ProfileScraperModel.SavedQueries.Any(x => x.QueryType == Application.Current.FindResource("LangKeyPeopleConnectedInMessenger")?.ToString());

                var moduleSettingsUserControl = sender as ModuleSettingsUserControl<ProfileScraperViewModel, ProfileScraperModel>;

                moduleSettingsUserControl?.AddQuery(typeof(FdProfileQueryParameters));

                ProfileScraperModel.GenderAndLocationFilter.IsGroupCategoryEnabled
                    = ProfileScraperModel.SavedQueries.Any(x => x.QueryType == Application.Current.FindResource("LangKeyGroupMembers")?.ToString());
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


        public ProfileScraperModel Model => ProfileScraperModel;

        private ProfileScraperModel _profileScraperModel = new ProfileScraperModel();

        public ProfileScraperModel ProfileScraperModel
        {
            get
            {
                return _profileScraperModel;
            }
            set
            {
                if (_profileScraperModel == null & _profileScraperModel == value)
                    return;
                SetProperty(ref _profileScraperModel, value);
            }
        }
    }
}
