using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.FdQuery;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using FaceDominatorCore.FDModel.LikerCommentorModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FaceDominatorCore.FDViewModel.LikerCommentorViewModel
{
    public class PostCommentorViewModel : BindableBase
    {
        public PostCommentorViewModel()
        {
            Model.LikerCommentorConfigModel.ManageCommentModel.LstQueries.Add(new QueryContent { Content = new QueryInfo() { QueryType = "All", QueryValue = "All" } });



            PostCommentorModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = Application.Current.FindResource("LangKeyNumberOfCommentsPerJob")?.ToString(),
                ActivitiesPerHourDisplayName = Application.Current.FindResource("LangKeyNumberOfCommentsPerHour")?.ToString(),
                ActivitiesPerDayDisplayName = Application.Current.FindResource("LangKeyNumberOfCommentsPerDay")?.ToString(),
                ActivitiesPerWeekDisplayName = Application.Current.FindResource("LangKeyNumberOfCommentsPerWeek")?.ToString(),
                IncreaseActivityDisplayName = Application.Current.FindResource("LangKeyMaxCommentPerDay")?.ToString(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };

            AddQueryCommand = new BaseCommand<object>((sender) => true, AddQuery);
            CustomFilterCommand = new BaseCommand<object>((sender) => true, CustomFilter);
            DeleteQueryCommand = new BaseCommand<object>((sender) => true, DeleteQuery);
            AddMessagesCommand = new BaseCommand<object>((sender) => true, AddMessages);
            DeleteMulipleCommand = new BaseCommand<object>((sender) => true, DeleteMuliple);
            CommentCheckedChangedCommand = new BaseCommand<object>((sender) => true, CheckedChangedExecute);
            SpecificWordListCommand = new BaseCommand<object>((sender) => true, SpecificWordListChangedExecute);
            ClearListCommand = new BaseCommand<object>((sender) => true, ClearListCommandExecute);
        }

        private void ClearListCommandExecute(object sender)
        {

            if (sender is InputBoxControl control)
            {
                var controlName = control.Name ?? "";

                switch (controlName)
                {
                    case "InputBoxFriends":
                        RemoveMessagesFromModel(PostOptions.FriendWall);
                        break;
                    case "InputBoxGroups":
                        RemoveMessagesFromModel(PostOptions.Group);
                        break;
                    case "InputBoxPages":
                        RemoveMessagesFromModel(PostOptions.Pages);
                        break;
                    case "InputBoxCustom":
                        RemoveMessagesFromModel(PostOptions.CustomPostList);
                        break;
                    case "InputBoxCampaign":
                        RemoveMessagesFromModel(PostOptions.Campaign);
                        break;
                    case "InputBoxKeyword":
                        RemoveMessagesFromModel(PostOptions.Keyword);
                        break;
                    case "InputBoxProfileScraperCampaign":
                        RemoveMessagesFromModel(PostOptions.ProfileScraper);
                        break;
                    case "InputBoxPostScraperCampaign":
                        RemoveMessagesFromModel(PostOptions.PostScraperCampaign);
                        break;
                }
            }
        }

        private void CheckedChangedExecute(object sender)
        {
            var control = ((CheckBox)sender).Name;

            var likerCommentorModel = PostCommentorModel.PostLikeCommentorModel;

            switch (control)
            {
                case "ChkOwnwall":
                    if (!likerCommentorModel.IsOwnWallChecked)
                        RemoveMessagesToModel(PostOptions.OwnWall.ToString(), PostOptions.OwnWall.ToString());
                    else
                        ManageAddComments(PostOptions.OwnWall);
                    break;
                case "ChkNewsfeed":
                    if (!likerCommentorModel.IsNewsfeedChecked)
                        RemoveMessagesToModel(PostOptions.NewsFeed.ToString(), PostOptions.NewsFeed.ToString());
                    else
                        ManageAddComments(PostOptions.NewsFeed);
                    break;
                case "ChkFriend":
                    if (!likerCommentorModel.IsFriendTimeLineChecked)
                        RemoveMessagesFromModel(PostOptions.FriendWall);
                    else
                        ManageAddComments(PostOptions.FriendWall);
                    break;
                case "ChkGroups":
                    if (!likerCommentorModel.IsGroupChecked)
                        RemoveMessagesFromModel(PostOptions.Group);
                    else
                        ManageAddComments(PostOptions.Group);
                    break;
                case "ChkPages":
                    if (!likerCommentorModel.IsPageChecked)
                        RemoveMessagesFromModel(PostOptions.Pages);
                    else
                        ManageAddComments(PostOptions.Pages);
                    break;
                case "ChkPostList":
                    if (!likerCommentorModel.IsCustomPostListChecked)
                        RemoveMessagesFromModel(PostOptions.CustomPostList);
                    else
                        ManageAddComments(PostOptions.CustomPostList);
                    break;
                case "ChkCampaign":
                    if (!likerCommentorModel.IsCampaignChecked)
                        RemoveMessagesFromModel(PostOptions.Campaign);
                    else
                        ManageAddComments(PostOptions.Campaign);
                    break;
                case "ChkKeyWord":
                    if (!likerCommentorModel.IsKeywordChecked)
                        RemoveMessagesFromModel(PostOptions.Keyword);
                    else
                        ManageAddComments(PostOptions.Keyword);
                    break;
                case "ChkProfileScraperCampaign":
                    if (!likerCommentorModel.IsCampaignChked)
                        RemoveMessagesFromModel(PostOptions.ProfileScraper);
                    else
                        ManageAddComments(PostOptions.ProfileScraper);
                    break;
                case "ChkPostSharer":
                    if (!likerCommentorModel.IsPostSharerChecked)
                        RemoveMessagesFromModel(PostOptions.PostSharer);
                    else
                        ManageAddComments(PostOptions.PostSharer);
                    break;
                case "ChkPostScraperCampaign":
                    if (!likerCommentorModel.IsPostScraperCampaignChecked)
                        RemoveMessagesFromModel(PostOptions.PostScraperCampaign);
                    else
                        ManageAddComments(PostOptions.PostScraperCampaign);
                    break;

            }
        }

        private void SpecificWordListChangedExecute(object sender)
        {
            var control = ((InputBoxControl)sender).Name;

            var likerCommentorModel = PostCommentorModel.PostLikeCommentorModel;

            switch (control)
            {
                case "InputBoxFriends":
                    likerCommentorModel.ListFriendProfileUrl = Regex.Split(likerCommentorModel.FriendProfileUrl, "\r\n").Where(x => !string.IsNullOrEmpty(x)).ToList();
                    ManageAddComments(PostOptions.FriendWall);
                    break;
                case "InputBoxGroups":
                    likerCommentorModel.ListGroupUrl = Regex.Split(likerCommentorModel.GroupUrl, "\r\n").Where(x => !string.IsNullOrEmpty(x)).ToList();
                    ManageAddComments(PostOptions.Group);
                    break;
                case "InputBoxPages":
                    likerCommentorModel.ListPageUrl = Regex.Split(likerCommentorModel.PageUrl, "\r\n").Where(x => !string.IsNullOrEmpty(x)).ToList();
                    ManageAddComments(PostOptions.Pages);
                    break;
                case "InputBoxCustom":
                    likerCommentorModel.ListCustomPostList = Regex.Split(likerCommentorModel.CustomPostList, "\r\n").Where(x => !string.IsNullOrEmpty(x)).ToList();
                    ManageAddComments(PostOptions.CustomPostList);
                    break;
                case "InputBoxCampaign":
                    likerCommentorModel.ListFaceDominatorCampaign = Regex.Split(likerCommentorModel.Campaign, "\r\n").Where(x => !string.IsNullOrEmpty(x)).ToList();
                    ManageAddComments(PostOptions.Campaign);
                    break;
                case "InputBoxKeyword":
                    likerCommentorModel.ListKeywords = Regex.Split(likerCommentorModel.Keyword, "\r\n").Where(x => !string.IsNullOrEmpty(x)).ToList();
                    ManageAddComments(PostOptions.Keyword);
                    break;
                case "InputBoxProfileScraperCampaign":
                    likerCommentorModel.ListCampaign = Regex.Split(likerCommentorModel.NrlCampaign, "\r\n").Where(x => !string.IsNullOrEmpty(x)).ToList();
                    ManageAddComments(PostOptions.ProfileScraper);
                    break;
                case "InputBoxHashtag":
                    likerCommentorModel.ListHashtags = Regex.Split(likerCommentorModel.HashtagUrl, "\r\n").Where(x => !string.IsNullOrEmpty(x)).ToList();
                    ManageAddComments(PostOptions.Hashtag);
                    break;
                case "InputBoxPostSharer":
                    likerCommentorModel.ListPostSharer = Regex.Split(likerCommentorModel.PostSharerUrl, "\r\n").Where(x => !string.IsNullOrEmpty(x)).ToList();
                    ManageAddComments(PostOptions.PostSharer);
                    break;
                case "InputBoxPostScraperCampaign":
                    likerCommentorModel.ListPostScraperCampaign = Regex.Split(likerCommentorModel.PostScraperCampaignUrl, "\r\n").Where(x => !string.IsNullOrEmpty(x)).ToList();
                    ManageAddComments(PostOptions.PostScraperCampaign);
                    break;

            }

            DominatorHouseCore.LogHelper.GlobusLogHelper.log.Info(DominatorHouseCore.Utility.Log.CustomMessage,
                DominatorHouseCore.Enums.SocialNetworks.Facebook, "N/A", "N/A", "LangKeyDataSaved".FromResourceDictionary());
        }

        //public void GetListFromString([NotNull] List<string> lstItems, string input)
        //{
        //    if (lstItems == null) throw new ArgumentNullException(nameof(lstItems));
        //    try
        //    {
        //        if (!string.IsNullOrEmpty(input))
        //        {
        //            lstItems = Regex.Split(input, "\r\n").ToList();
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        e.DebugLog();
        //    }
        //}

        #region Commands

        public ICommand AddQueryCommand { get; set; }
        public ICommand CustomFilterCommand { get; set; }
        public ICommand DeleteQueryCommand { get; set; }
        public ICommand AddMessagesCommand { get; set; }
        public ICommand DeleteMulipleCommand { get; set; }

        public ICommand CommentCheckedChangedCommand { get; set; }

        public ICommand SpecificWordListCommand { get; set; }

        public ICommand ClearListCommand { get; set; }

        #endregion

        #region Methods

        private void CustomFilter(object sender)
        {
            try
            {
                var moduleSettingsUserControl = sender as ModuleSettingsUserControl<PostCommentorViewModel, PostCommentorModel>;
                moduleSettingsUserControl?.CustomFilter();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void AddQuery(object sender)
        {

        }

        private void ManageAddComments(PostOptions postOptions)
        {
            if (postOptions != PostOptions.OwnWall && postOptions != PostOptions.NewsFeed)
            {
                AddMessagesToModel(postOptions);
            }
            else
            {
                AddMessagesToModel(postOptions.ToString(), postOptions.ToString());
            }

        }

        private void AddMessagesToModel(PostOptions postOption)
        {
            List<QueryContent> queryToDelete;

            switch (postOption)
            {
                case PostOptions.Group:
                    Model.PostLikeCommentorModel.ListGroupUrl.ForEach(x =>
                    {
                        if (Model.LikerCommentorConfigModel.ManageCommentModel.LstQueries.FirstOrDefault(y =>
                            y.Content.QueryType == PostOptions.Group.ToString() && y.Content.QueryValue == x) == null)
                            AddMessagesToModel(PostOptions.Group.ToString(), x);
                    });
                    queryToDelete = Model.LikerCommentorConfigModel.ManageCommentModel.LstQueries.Where(x => x.Content.QueryType == postOption.ToString()).ToList();
                    if (queryToDelete.Count != Model.PostLikeCommentorModel.ListGroupUrl.Count)
                        DeleteQuery(queryToDelete, Model.PostLikeCommentorModel.ListGroupUrl);
                    break;
                case PostOptions.Pages:
                    Model.PostLikeCommentorModel.ListPageUrl.ForEach(x =>
                    {
                        if (Model.LikerCommentorConfigModel.ManageCommentModel.LstQueries.FirstOrDefault(y =>
                            y.Content.QueryType == PostOptions.Pages.ToString() && y.Content.QueryValue == x) == null)
                            AddMessagesToModel(PostOptions.Pages.ToString(), x);
                    });
                    queryToDelete = Model.LikerCommentorConfigModel.ManageCommentModel.LstQueries.Where(x => x.Content.QueryType == postOption.ToString()).ToList();
                    if (queryToDelete.Count != Model.PostLikeCommentorModel.ListPageUrl.Count)
                        DeleteQuery(queryToDelete, Model.PostLikeCommentorModel.ListPageUrl);
                    break;
                case PostOptions.Campaign:
                    Model.PostLikeCommentorModel.ListFaceDominatorCampaign.ForEach(x =>
                    {
                        if (Model.LikerCommentorConfigModel.ManageCommentModel.LstQueries.FirstOrDefault(y =>
                            y.Content.QueryType == PostOptions.Campaign.ToString() && y.Content.QueryValue == x) == null)
                            AddMessagesToModel(PostOptions.Campaign.ToString(), x);
                    });
                    queryToDelete = Model.LikerCommentorConfigModel.ManageCommentModel.LstQueries.Where(x => x.Content.QueryType == postOption.ToString()).ToList();
                    if (queryToDelete.Count != Model.PostLikeCommentorModel.ListFaceDominatorCampaign.Count)
                        DeleteQuery(queryToDelete, Model.PostLikeCommentorModel.ListFaceDominatorCampaign);
                    break;
                case PostOptions.CustomPostList:
                    Model.PostLikeCommentorModel.ListCustomPostList.ForEach(x =>
                    {
                        if (Model.LikerCommentorConfigModel.ManageCommentModel.LstQueries.FirstOrDefault(y =>
                            y.Content.QueryType == PostOptions.CustomPostList.ToString() && y.Content.QueryValue == x) == null)
                            AddMessagesToModel(PostOptions.CustomPostList.ToString(), x);
                    });
                    queryToDelete = Model.LikerCommentorConfigModel.ManageCommentModel.LstQueries.Where(x => x.Content.QueryType == postOption.ToString()).ToList();
                    if (queryToDelete.Count != Model.PostLikeCommentorModel.ListCustomPostList.Count)
                        DeleteQuery(queryToDelete, Model.PostLikeCommentorModel.ListCustomPostList);
                    break;
                case PostOptions.FriendWall:
                    Model.PostLikeCommentorModel.ListFriendProfileUrl.ForEach(x =>
                    {
                        if (Model.LikerCommentorConfigModel.ManageCommentModel.LstQueries.FirstOrDefault(y =>
                            y.Content.QueryType == PostOptions.FriendWall.ToString() && y.Content.QueryValue == x) == null)
                            AddMessagesToModel(PostOptions.FriendWall.ToString(), x);
                    });
                    queryToDelete = Model.LikerCommentorConfigModel.ManageCommentModel.LstQueries.Where(x => x.Content.QueryType == postOption.ToString()).ToList();
                    if (queryToDelete.Count != Model.PostLikeCommentorModel.ListFriendProfileUrl.Count)
                        DeleteQuery(queryToDelete, Model.PostLikeCommentorModel.ListFriendProfileUrl);
                    break;
                case PostOptions.Keyword:
                    Model.PostLikeCommentorModel.ListKeywords.ForEach(x =>
                    {
                        if (Model.LikerCommentorConfigModel.ManageCommentModel.LstQueries.FirstOrDefault(y =>
                                y.Content.QueryType == PostOptions.Keyword.ToString() && y.Content.QueryValue == x) == null)
                            AddMessagesToModel(PostOptions.Keyword.ToString(), x);
                    });
                    queryToDelete = Model.LikerCommentorConfigModel.ManageCommentModel.LstQueries.Where(x => x.Content.QueryType == postOption.ToString()).ToList();
                    if (queryToDelete.Count != Model.PostLikeCommentorModel.ListKeywords.Count)
                        DeleteQuery(queryToDelete, Model.PostLikeCommentorModel.ListKeywords);
                    break;
                case PostOptions.ProfileScraper:
                    Model.PostLikeCommentorModel.ListCampaign.ForEach(x =>
                    {
                        if (Model.LikerCommentorConfigModel.ManageCommentModel.LstQueries.FirstOrDefault(y =>
                                y.Content.QueryType == PostOptions.ProfileScraper.ToString() && y.Content.QueryValue == x) == null)
                            AddMessagesToModel(PostOptions.ProfileScraper.ToString(), x);
                    });
                    queryToDelete = Model.LikerCommentorConfigModel.ManageCommentModel.LstQueries.Where(x => x.Content.QueryType == postOption.ToString()).ToList();
                    if (queryToDelete.Count != Model.PostLikeCommentorModel.ListCampaign.Count)
                        DeleteQuery(queryToDelete, Model.PostLikeCommentorModel.ListCampaign);
                    break;
                case PostOptions.PostSharer:
                    Model.PostLikeCommentorModel.ListPostSharer.ForEach(x =>
                    {
                        if (Model.LikerCommentorConfigModel.ManageCommentModel.LstQueries.FirstOrDefault(y =>
                         y.Content.QueryType == PostOptions.PostSharer.ToString() && y.Content.QueryValue == x) == null)
                            AddMessagesToModel(PostOptions.PostSharer.ToString(), x);
                    });
                    queryToDelete = Model.LikerCommentorConfigModel.ManageCommentModel.LstQueries.Where(x => x.Content.QueryType == postOption.ToString()).ToList();
                    if (queryToDelete.Count != Model.PostLikeCommentorModel.ListPostSharer.Count)
                        DeleteQuery(queryToDelete, Model.PostLikeCommentorModel.ListPostSharer);
                    break;
                case PostOptions.PostScraperCampaign:
                    Model.PostLikeCommentorModel.ListPostScraperCampaign.ForEach(x =>
                    {
                        if (Model.LikerCommentorConfigModel.ManageCommentModel.LstQueries.FirstOrDefault(y =>
                         y.Content.QueryType == PostOptions.PostScraperCampaign.ToString() && y.Content.QueryValue == x) == null)
                            AddMessagesToModel(PostOptions.PostScraperCampaign.ToString(), x);
                    });
                    queryToDelete = Model.LikerCommentorConfigModel.ManageCommentModel.LstQueries.Where(x => x.Content.QueryValue == postOption.ToString()).ToList();
                    if (queryToDelete.Count != Model.PostLikeCommentorModel.ListPostScraperCampaign.Count)
                        DeleteQuery(queryToDelete, Model.PostLikeCommentorModel.ListPostScraperCampaign);
                    break;
            }
        }

        private void DeleteQuery(List<QueryContent> queryToDelete, List<string> listGroupUrl)
        {
            try
            {
                foreach (var currentQuery in queryToDelete)
                {
                    try
                    {
                        if (listGroupUrl.FirstOrDefault(x => x == currentQuery.Content.QueryValue.ToString()) == null)
                        {
                            var queryDelete = Model.LikerCommentorConfigModel.ManageCommentModel.LstQueries.FirstOrDefault(x =>
                               x.Content.QueryValue == currentQuery.Content.QueryValue
                               && x.Content.QueryType == currentQuery.Content.QueryType);


                            Model.LikerCommentorConfigModel.ManageCommentModel.LstQueries.Remove(queryDelete);
                            foreach (var message in Model.LikerCommentorConfigModel.LstManageCommentModel.ToList())
                            {
                                var selectedQueryToDelete = message.SelectedQuery.FirstOrDefault(x => queryDelete != null && (x.Content.QueryType == queryDelete.Content.QueryType &&
                                                                                                                              x.Content.QueryValue == queryDelete.Content.QueryValue));
                                message.SelectedQuery.Remove(selectedQueryToDelete);
                                if (message.SelectedQuery.Count == 0)
                                    Model.LikerCommentorConfigModel.LstManageCommentModel.Remove(message);
                            }
                        }


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



        private void RemoveMessagesFromModel(PostOptions postOption)
        {
            switch (postOption)
            {
                case PostOptions.Group:
                    Model.PostLikeCommentorModel.ListGroupUrl.ForEach(x =>
                    {
                        if (Model.LikerCommentorConfigModel.ManageCommentModel.LstQueries.FirstOrDefault(y =>
                            y.Content.QueryType == PostOptions.Group.ToString() && y.Content.QueryValue == x) != null)
                            RemoveMessagesToModel(PostOptions.Group.ToString(), x);
                    });
                    break;
                case PostOptions.Pages:
                    Model.PostLikeCommentorModel.ListPageUrl.ForEach(x =>
                    {
                        if (Model.LikerCommentorConfigModel.ManageCommentModel.LstQueries.FirstOrDefault(y =>
                            y.Content.QueryType == PostOptions.Pages.ToString() && y.Content.QueryValue == x) != null)
                            RemoveMessagesToModel(PostOptions.Pages.ToString(), x);
                    });
                    break;
                case PostOptions.Campaign:
                    Model.PostLikeCommentorModel.ListFaceDominatorCampaign.ForEach(x =>
                    {
                        if (Model.LikerCommentorConfigModel.ManageCommentModel.LstQueries.FirstOrDefault(y =>
                            y.Content.QueryType == PostOptions.Campaign.ToString() && y.Content.QueryValue == x) != null)
                            RemoveMessagesToModel(PostOptions.Campaign.ToString(), x);
                    });
                    break;
                case PostOptions.CustomPostList:
                    Model.PostLikeCommentorModel.ListCustomPostList.ForEach(x =>
                    {
                        if (Model.LikerCommentorConfigModel.ManageCommentModel.LstQueries.FirstOrDefault(y =>
                            y.Content.QueryType == PostOptions.CustomPostList.ToString() && y.Content.QueryValue == x) != null)
                            RemoveMessagesToModel(PostOptions.CustomPostList.ToString(), x);
                    });
                    break;
                case PostOptions.FriendWall:
                    Model.PostLikeCommentorModel.ListFriendProfileUrl.ForEach(x =>
                    {
                        if (Model.LikerCommentorConfigModel.ManageCommentModel.LstQueries.FirstOrDefault(y =>
                            y.Content.QueryType == PostOptions.FriendWall.ToString() && y.Content.QueryValue == x) != null)
                            RemoveMessagesToModel(PostOptions.FriendWall.ToString(), x);
                    });
                    break;
                case PostOptions.Keyword:
                    Model.PostLikeCommentorModel.ListKeywords.ForEach(x =>
                    {
                        if (Model.LikerCommentorConfigModel.ManageCommentModel.LstQueries.FirstOrDefault(y =>
                                y.Content.QueryType == PostOptions.Keyword.ToString() && y.Content.QueryValue == x) != null)
                            RemoveMessagesToModel(PostOptions.Keyword.ToString(), x);
                    });
                    break;

                case PostOptions.ProfileScraper:
                    Model.PostLikeCommentorModel.ListFaceDominatorCampaign.ForEach(x =>
                    {
                        if (Model.LikerCommentorConfigModel.ManageCommentModel.LstQueries.FirstOrDefault(y =>
                                y.Content.QueryType == PostOptions.ProfileScraper.ToString() && y.Content.QueryValue == x) != null)
                            RemoveMessagesToModel(PostOptions.ProfileScraper.ToString(), x);
                    });
                    break;

                case PostOptions.PostScraperCampaign:
                    Model.PostLikeCommentorModel.ListFaceDominatorCampaign.ForEach(x =>
                    {
                        if (Model.LikerCommentorConfigModel.ManageCommentModel.LstQueries.FirstOrDefault(y =>
                         y.Content.QueryType == PostOptions.PostScraperCampaign.ToString() && y.Content.QueryValue == x) != null)
                            RemoveMessagesToModel(PostOptions.PostScraperCampaign.ToString(), x);
                    });
                    break;
            }
        }

        public void AddMessagesToModel(string queryType, string queryValue)
        {
            Model.LikerCommentorConfigModel.ManageCommentModel.LstQueries.Add(new QueryContent { Content = new QueryInfo() { QueryType = queryType, QueryValue = queryValue } });
        }

        public void RemoveMessagesToModel(string queryType, string queryValue)
        {
            var queryToDelete = Model.LikerCommentorConfigModel.ManageCommentModel.LstQueries.FirstOrDefault(x => x.Content.QueryType == queryType);
            Model.LikerCommentorConfigModel.ManageCommentModel.LstQueries.Remove(queryToDelete);
            Model.LikerCommentorConfigModel.ManageCommentModel.LstQueries.Remove(queryToDelete);
            foreach (var message in Model.LikerCommentorConfigModel.LstManageCommentModel.ToList())
            {
                var selectedQueryToDelete = message.SelectedQuery.FirstOrDefault(x => queryToDelete != null && (x.Content.QueryType == queryToDelete.Content.QueryType &&
                                                                                                                x.Content.QueryValue == queryToDelete.Content.QueryValue));
                message.SelectedQuery.Remove(selectedQueryToDelete);
                if (message.SelectedQuery.Count == 0)
                    Model.LikerCommentorConfigModel.LstManageCommentModel.Remove(message);
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

        public PostCommentorModel Model => PostCommentorModel;

        private PostCommentorModel _postLikerCommentorModel = new PostCommentorModel();

        public PostCommentorModel PostCommentorModel
        {
            get
            {
                return _postLikerCommentorModel;
            }
            set
            {
                if (_postLikerCommentorModel == null & _postLikerCommentorModel == value)
                    return;
                SetProperty(ref _postLikerCommentorModel, value);
            }
        }
    }


}
