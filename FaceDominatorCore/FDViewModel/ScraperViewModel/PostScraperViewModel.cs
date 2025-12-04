using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDModel.ScraperModel;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace FaceDominatorCore.FDViewModel.ScraperViewModel
{
    public class PostScraperViewModel : BindableBase
    {
        public PostScraperViewModel()
        {

            PostScraperModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = Application.Current.FindResource("LangKeyScrapNumberOfPostsPerJob")?.ToString(),
                ActivitiesPerHourDisplayName = Application.Current.FindResource("LangKeyScrapNumberOfPostsPerHour")?.ToString(),
                ActivitiesPerDayDisplayName = Application.Current.FindResource("LangKeyScrapNumberOfPostsPerDay")?.ToString(),
                ActivitiesPerWeekDisplayName = Application.Current.FindResource("LangKeyScrapNumberOfPostsPerWeek")?.ToString(),
                IncreaseActivityDisplayName = Application.Current.FindResource("LangKeyScrapMaxPostsPerDay")?.ToString(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };

            AddQueryCommand = new BaseCommand<object>((sender) => true, AddQuery);
            CustomFilterCommand = new BaseCommand<object>((sender) => true, CustomFilter);
            DeleteQueryCommand = new BaseCommand<object>((sender) => true, DeleteQuery);
            AddMessagesCommand = new BaseCommand<object>((sender) => true, AddMessages);
            DeleteMulipleCommand = new BaseCommand<object>((sender) => true, DeleteMuliple);
            ClearCommand = new BaseCommand<object>((sender) => true, ClearProfileListUsers);
        }



        #region Commands

        public ICommand AddQueryCommand { get; set; }
        public ICommand CustomFilterCommand { get; set; }
        public ICommand DeleteQueryCommand { get; set; }
        public ICommand AddMessagesCommand { get; set; }
        public ICommand DeleteMulipleCommand { get; set; }
        public ICommand ClearCommand { get; set; }

        #endregion

        #region Methods
        private void ClearProfileListUsers(object obj)
        {
            if (PostScraperModel.PostLikeCommentorModel.IsFriendTimeLineChecked)
                PostScraperModel.PostLikeCommentorModel.ListFriendProfileUrl.Clear();
            if (PostScraperModel.PostLikeCommentorModel.IsGroupChecked)
                PostScraperModel.PostLikeCommentorModel.ListGroupUrl.Clear();
            if (PostScraperModel.PostLikeCommentorModel.IsCustomPostListChecked)
                PostScraperModel.PostLikeCommentorModel.ListCustomPostList.Clear();
            if (PostScraperModel.PostLikeCommentorModel.IsPageChecked)
                PostScraperModel.PostLikeCommentorModel.ListPageUrl.Clear();
            if (PostScraperModel.PostLikeCommentorModel.IsPostScraperCampaignChecked)
                PostScraperModel.PostLikeCommentorModel.ListPostScraperCampaign.Clear();
            if (PostScraperModel.PostLikeCommentorModel.IsHashtagChecked)
                PostScraperModel.PostLikeCommentorModel.ListHashtags.Clear();
            if (PostScraperModel.PostLikeCommentorModel.IsKeywordChecked)
                PostScraperModel.PostLikeCommentorModel.ListKeywords.Clear();
            if (PostScraperModel.PostLikeCommentorModel.IsCampaignChecked)
                PostScraperModel.PostLikeCommentorModel.ListFaceDominatorCampaign.Clear();
            if (PostScraperModel.PostLikeCommentorModel.IsCampaignChked)
                PostScraperModel.PostLikeCommentorModel.ListCampaign.Clear();
        }
        private void CustomFilter(object sender)
        {
            try
            {
                var moduleSettingsUserControl = sender as DominatorUIUtility.CustomControl.ModuleSettingsUserControl<PostScraperViewModel, PostScraperModel>;
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

                //var ModuleSettingsUserControl = sender as DominatorUIUtility.CustomControl.ModuleSettingsUserControl<PostScraperViewModel, PostScraperModel>;

                //ModuleSettingsUserControl.AddQuery(typeof(FdUserQueryParameters));
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


        public PostScraperModel Model => PostScraperModel;

        private PostScraperModel _postScraperModel = new PostScraperModel();

        public PostScraperModel PostScraperModel
        {
            get
            {
                return _postScraperModel;
            }
            set
            {
                if (_postScraperModel == null & _postScraperModel == value)
                    return;
                SetProperty(ref _postScraperModel, value);
            }
        }
    }
}
