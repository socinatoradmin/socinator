using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.FdQuery;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.FacebookModels;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public interface IPostCommentorViewModel
    {
    }

    public class PostCommentorViewModel : StartupBaseViewModel, IPostCommentorViewModel
    {
        private bool _isActionasOwnAccountChecked = true;

        private bool _isActionasPageChecked;

        private LikerCommentorConfigModel _likerCommentorConfigModel = new LikerCommentorConfigModel();

        private PostLikeCommentorModel _postLikeCommentorModel = new PostLikeCommentorModel();

        public PostCommentorViewModel(IRegionManager region) : base(region)
        {
            LikerCommentorConfigModel.ManageCommentModel.LstQueries.Add(new QueryContent
            { Content = new QueryInfo { QueryType = "All", QueryValue = "All" } });

            IsNonQuery = true;
            ViewModelToSave.Add(new ActivityConfig { Model = this, ActivityType = ActivityType.PostCommentor });
            NextCommand = new DelegateCommand(PostCommentorValidate);
            PreviousCommand = new DelegateCommand(NavigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);
            CommentCheckedChangedCommand = new DelegateCommand<object>(CheckedChangedExecute);
            SpecificWordListCommand = new DelegateCommand<object>(SpecificWordListChangedExecute);

            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfCommentsPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfCommentsPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfCommentsPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfCommentsPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxCommentPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
        }

        public ICommand CommentCheckedChangedCommand { get; set; }
        public ICommand SpecificWordListCommand { get; set; }

        public PostLikeCommentorModel PostLikeCommentorModel
        {
            get => _postLikeCommentorModel;
            set
            {
                if ((_postLikeCommentorModel == value) & (_postLikeCommentorModel == null))
                    return;
                SetProperty(ref _postLikeCommentorModel, value);
            }
        }

        public LikerCommentorConfigModel LikerCommentorConfigModel
        {
            get => _likerCommentorConfigModel;
            set
            {
                if ((_likerCommentorConfigModel == value) & (_likerCommentorConfigModel == null))
                    return;
                SetProperty(ref _likerCommentorConfigModel, value);
            }
        }

        public bool IsActionasOwnAccountChecked
        {
            get => _isActionasOwnAccountChecked;
            set
            {
                if (value == _isActionasOwnAccountChecked)
                    return;
                SetProperty(ref _isActionasOwnAccountChecked, value);
            }
        }

        public bool IsActionasPageChecked
        {
            get => _isActionasPageChecked;
            set
            {
                if (value == _isActionasPageChecked)
                    return;
                SetProperty(ref _isActionasPageChecked, value);
            }
        }

        private void PostCommentorValidate()
        {
            if (!PostLikeCommentorModel.IsOwnWallChecked && !PostLikeCommentorModel.IsNewsfeedChecked &&
                !PostLikeCommentorModel.IsFriendTimeLineChecked && !PostLikeCommentorModel.IsCustomPostListChecked
                && !PostLikeCommentorModel.IsCampaignChecked && !PostLikeCommentorModel.IsGroupChecked
                && !PostLikeCommentorModel.IsPageChecked && !PostLikeCommentorModel.IsKeywordChecked
                && !PostLikeCommentorModel.IsCampaignChked && !PostLikeCommentorModel.IsPostScraperCampaignChecked)
            {
                Dialog.ShowDialog("Error", "Please Select atleast one option.");
                return;
            }

            if (PostLikeCommentorModel.IsFriendTimeLineChecked &&
                PostLikeCommentorModel.ListFriendProfileUrl.Count == 0)
            {
                Dialog.ShowDialog("Error", "Please add at least one query.");
                return;
            }

            if (PostLikeCommentorModel.IsGroupChecked && PostLikeCommentorModel.ListGroupUrl.Count == 0)
            {
                Dialog.ShowDialog("Error", "Please add at least one query.");
                return;
            }

            if (PostLikeCommentorModel.IsPageChecked && PostLikeCommentorModel.ListPageUrl.Count == 0)
            {
                Dialog.ShowDialog("Error", "Please add at least one query.");
                return;
            }

            if (PostLikeCommentorModel.IsCampaignChecked && PostLikeCommentorModel.ListFaceDominatorCampaign.Count == 0)
            {
                Dialog.ShowDialog("Error", "Please add at least one query.");
                return;
            }

            if (PostLikeCommentorModel.IsCustomPostListChecked && PostLikeCommentorModel.ListCustomPostList.Count == 0)
            {
                Dialog.ShowDialog("Error", "Please add at least one query.");
                return;
            }

            if (PostLikeCommentorModel.IsKeywordChecked && PostLikeCommentorModel.ListKeywords.Count == 0)
            {
                Dialog.ShowDialog("Error", "Please add at least one query.");
                return;
            }

            if (PostLikeCommentorModel.IsCampaignChked && PostLikeCommentorModel.ListCampaign.Count == 0)
            {
                Dialog.ShowDialog("Error", "Please add at least one query.");
                return;
            }

            if (LikerCommentorConfigModel.LstManageCommentModel.Count == 0)
            {
                Dialog.ShowDialog("Error", "Please add at least one comment.");
                return;
            }

            if (!IsActionasPageChecked && !IsActionasOwnAccountChecked)
            {
                Dialog.ShowDialog("Error", "Please select atleast one action as option.");
                return;
            }

            NavigateNext();
        }

        private void CheckedChangedExecute(object sender)
        {
            var control = ((CheckBox)sender).Name;

            var likerCommentorModel = PostLikeCommentorModel;

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
            }
        }

        private void SpecificWordListChangedExecute(object sender)
        {
            var control = ((InputBoxControl)sender).Name;

            var likerCommentorModel = PostLikeCommentorModel;

            switch (control)
            {
                case "InputBoxFriends":
                    likerCommentorModel.ListFriendProfileUrl = Regex.Split(likerCommentorModel.FriendProfileUrl, "\r\n")
                        .Where(x => !string.IsNullOrEmpty(x)).ToList();
                    ManageAddComments(PostOptions.FriendWall);
                    break;
                case "InputBoxGroups":
                    likerCommentorModel.ListGroupUrl = Regex.Split(likerCommentorModel.GroupUrl, "\r\n")
                        .Where(x => !string.IsNullOrEmpty(x)).ToList();
                    ManageAddComments(PostOptions.Group);
                    break;
                case "InputBoxPages":
                    likerCommentorModel.ListPageUrl = Regex.Split(likerCommentorModel.PageUrl, "\r\n")
                        .Where(x => !string.IsNullOrEmpty(x)).ToList();
                    ManageAddComments(PostOptions.Pages);
                    break;
                case "InputBoxCustom":
                    likerCommentorModel.ListCustomPostList = Regex.Split(likerCommentorModel.CustomPostList, "\r\n")
                        .Where(x => !string.IsNullOrEmpty(x)).ToList();
                    ManageAddComments(PostOptions.CustomPostList);
                    break;
                case "InputBoxCampaign":
                    likerCommentorModel.ListFaceDominatorCampaign = Regex.Split(likerCommentorModel.Campaign, "\r\n")
                        .Where(x => !string.IsNullOrEmpty(x)).ToList();
                    ManageAddComments(PostOptions.Campaign);
                    break;
                case "InputBoxKeyword":
                    likerCommentorModel.ListKeywords = Regex.Split(likerCommentorModel.Keyword, "\r\n")
                        .Where(x => !string.IsNullOrEmpty(x)).ToList();
                    ManageAddComments(PostOptions.Keyword);
                    break;
                case "InputBoxProfileScraperCampaign":
                    likerCommentorModel.ListCampaign = Regex.Split(likerCommentorModel.NrlCampaign, "\r\n")
                        .Where(x => !string.IsNullOrEmpty(x)).ToList();
                    ManageAddComments(PostOptions.ProfileScraper);
                    break;
            }
        }

        private void RemoveMessagesFromModel(PostOptions postOption)
        {
            switch (postOption)
            {
                case PostOptions.Group:
                    PostLikeCommentorModel.ListGroupUrl.ForEach(x =>
                    {
                        if (LikerCommentorConfigModel.ManageCommentModel.LstQueries.FirstOrDefault(y =>
                                y.Content.QueryType == PostOptions.Group.ToString() && y.Content.QueryValue == x) !=
                            null)
                            RemoveMessagesToModel(PostOptions.Group.ToString(), x);
                    });
                    break;
                case PostOptions.Pages:
                    PostLikeCommentorModel.ListPageUrl.ForEach(x =>
                    {
                        if (LikerCommentorConfigModel.ManageCommentModel.LstQueries.FirstOrDefault(y =>
                                y.Content.QueryType == PostOptions.Pages.ToString() && y.Content.QueryValue == x) !=
                            null)
                            RemoveMessagesToModel(PostOptions.Pages.ToString(), x);
                    });
                    break;
                case PostOptions.Campaign:
                    PostLikeCommentorModel.ListFaceDominatorCampaign.ForEach(x =>
                    {
                        if (LikerCommentorConfigModel.ManageCommentModel.LstQueries.FirstOrDefault(y =>
                                y.Content.QueryType == PostOptions.Campaign.ToString() && y.Content.QueryValue == x) !=
                            null)
                            RemoveMessagesToModel(PostOptions.Campaign.ToString(), x);
                    });
                    break;
                case PostOptions.CustomPostList:
                    PostLikeCommentorModel.ListCustomPostList.ForEach(x =>
                    {
                        if (LikerCommentorConfigModel.ManageCommentModel.LstQueries.FirstOrDefault(y =>
                                y.Content.QueryType == PostOptions.CustomPostList.ToString() &&
                                y.Content.QueryValue == x) != null)
                            RemoveMessagesToModel(PostOptions.CustomPostList.ToString(), x);
                    });
                    break;
                case PostOptions.FriendWall:
                    PostLikeCommentorModel.ListFriendProfileUrl.ForEach(x =>
                    {
                        if (LikerCommentorConfigModel.ManageCommentModel.LstQueries.FirstOrDefault(y =>
                                y.Content.QueryType == PostOptions.FriendWall.ToString() &&
                                y.Content.QueryValue == x) != null)
                            RemoveMessagesToModel(PostOptions.FriendWall.ToString(), x);
                    });
                    break;
                case PostOptions.Keyword:
                    PostLikeCommentorModel.ListKeywords.ForEach(x =>
                    {
                        if (LikerCommentorConfigModel.ManageCommentModel.LstQueries.FirstOrDefault(y =>
                                y.Content.QueryType == PostOptions.Keyword.ToString() && y.Content.QueryValue == x) !=
                            null)
                            RemoveMessagesToModel(PostOptions.Keyword.ToString(), x);
                    });
                    break;

                case PostOptions.ProfileScraper:
                    PostLikeCommentorModel.ListFaceDominatorCampaign.ForEach(x =>
                    {
                        if (LikerCommentorConfigModel.ManageCommentModel.LstQueries.FirstOrDefault(y =>
                                y.Content.QueryType == PostOptions.ProfileScraper.ToString() &&
                                y.Content.QueryValue == x) != null)
                            RemoveMessagesToModel(PostOptions.ProfileScraper.ToString(), x);
                    });
                    break;
            }
        }

        public void AddMessagesToModel(string queryType, string queryValue)
        {
            LikerCommentorConfigModel.ManageCommentModel.LstQueries.Add(new QueryContent
            { Content = new QueryInfo { QueryType = queryType, QueryValue = queryValue } });
        }

        public void RemoveMessagesToModel(string queryType, string queryValue)
        {
            var queryToDelete =
                LikerCommentorConfigModel.ManageCommentModel.LstQueries.FirstOrDefault(x =>
                    x.Content.QueryType == queryType);
            LikerCommentorConfigModel.ManageCommentModel.LstQueries.Remove(queryToDelete);
            LikerCommentorConfigModel.ManageCommentModel.LstQueries.Remove(queryToDelete);
            foreach (var message in LikerCommentorConfigModel.LstManageCommentModel.ToList())
            {
                var selectedQueryToDelete = message.SelectedQuery.FirstOrDefault(x =>
                    queryToDelete != null && x.Content.QueryType == queryToDelete.Content.QueryType &&
                    x.Content.QueryValue == queryToDelete.Content.QueryValue);
                message.SelectedQuery.Remove(selectedQueryToDelete);
                if (message.SelectedQuery.Count == 0)
                    LikerCommentorConfigModel.LstManageCommentModel.Remove(message);
            }
        }

        private void ManageAddComments(PostOptions postOptions)
        {
            if (postOptions != PostOptions.OwnWall && postOptions != PostOptions.NewsFeed)
                AddMessagesToModel(postOptions);
            else
                AddMessagesToModel(postOptions.ToString(), postOptions.ToString());
        }

        private void AddMessagesToModel(PostOptions postOption)
        {
            List<QueryContent> queryToDelete;

            switch (postOption)
            {
                case PostOptions.Group:
                    PostLikeCommentorModel.ListGroupUrl.ForEach(x =>
                    {
                        if (LikerCommentorConfigModel.ManageCommentModel.LstQueries.FirstOrDefault(y =>
                                y.Content.QueryType == PostOptions.Group.ToString() && y.Content.QueryValue == x) ==
                            null)
                            AddMessagesToModel(PostOptions.Group.ToString(), x);
                    });
                    queryToDelete = LikerCommentorConfigModel.ManageCommentModel.LstQueries
                        .Where(x => x.Content.QueryType == postOption.ToString()).ToList();
                    if (queryToDelete.Count != PostLikeCommentorModel.ListGroupUrl.Count)
                        DeleteQuery(queryToDelete, PostLikeCommentorModel.ListGroupUrl);
                    break;
                case PostOptions.Pages:
                    PostLikeCommentorModel.ListPageUrl.ForEach(x =>
                    {
                        if (LikerCommentorConfigModel.ManageCommentModel.LstQueries.FirstOrDefault(y =>
                                y.Content.QueryType == PostOptions.Pages.ToString() && y.Content.QueryValue == x) ==
                            null)
                            AddMessagesToModel(PostOptions.Pages.ToString(), x);
                    });
                    queryToDelete = LikerCommentorConfigModel.ManageCommentModel.LstQueries
                        .Where(x => x.Content.QueryType == postOption.ToString()).ToList();
                    if (queryToDelete.Count != PostLikeCommentorModel.ListPageUrl.Count)
                        DeleteQuery(queryToDelete, PostLikeCommentorModel.ListPageUrl);
                    break;
                case PostOptions.Campaign:
                    PostLikeCommentorModel.ListFaceDominatorCampaign.ForEach(x =>
                    {
                        if (LikerCommentorConfigModel.ManageCommentModel.LstQueries.FirstOrDefault(y =>
                                y.Content.QueryType == PostOptions.Campaign.ToString() && y.Content.QueryValue == x) ==
                            null)
                            AddMessagesToModel(PostOptions.Campaign.ToString(), x);
                    });
                    queryToDelete = LikerCommentorConfigModel.ManageCommentModel.LstQueries
                        .Where(x => x.Content.QueryType == postOption.ToString()).ToList();
                    if (queryToDelete.Count != PostLikeCommentorModel.ListFaceDominatorCampaign.Count)
                        DeleteQuery(queryToDelete, PostLikeCommentorModel.ListFaceDominatorCampaign);
                    break;
                case PostOptions.CustomPostList:
                    PostLikeCommentorModel.ListCustomPostList.ForEach(x =>
                    {
                        if (LikerCommentorConfigModel.ManageCommentModel.LstQueries.FirstOrDefault(y =>
                                y.Content.QueryType == PostOptions.CustomPostList.ToString() &&
                                y.Content.QueryValue == x) == null)
                            AddMessagesToModel(PostOptions.CustomPostList.ToString(), x);
                    });
                    queryToDelete = LikerCommentorConfigModel.ManageCommentModel.LstQueries
                        .Where(x => x.Content.QueryType == postOption.ToString()).ToList();
                    if (queryToDelete.Count != PostLikeCommentorModel.ListCustomPostList.Count)
                        DeleteQuery(queryToDelete, PostLikeCommentorModel.ListCustomPostList);
                    break;
                case PostOptions.FriendWall:
                    PostLikeCommentorModel.ListFriendProfileUrl.ForEach(x =>
                    {
                        if (LikerCommentorConfigModel.ManageCommentModel.LstQueries.FirstOrDefault(y =>
                                y.Content.QueryType == PostOptions.FriendWall.ToString() &&
                                y.Content.QueryValue == x) == null)
                            AddMessagesToModel(PostOptions.FriendWall.ToString(), x);
                    });
                    queryToDelete = LikerCommentorConfigModel.ManageCommentModel.LstQueries
                        .Where(x => x.Content.QueryType == postOption.ToString()).ToList();
                    if (queryToDelete.Count != PostLikeCommentorModel.ListFriendProfileUrl.Count)
                        DeleteQuery(queryToDelete, PostLikeCommentorModel.ListFriendProfileUrl);
                    break;
                case PostOptions.Keyword:
                    PostLikeCommentorModel.ListKeywords.ForEach(x =>
                    {
                        if (LikerCommentorConfigModel.ManageCommentModel.LstQueries.FirstOrDefault(y =>
                                y.Content.QueryType == PostOptions.Keyword.ToString() && y.Content.QueryValue == x) ==
                            null)
                            AddMessagesToModel(PostOptions.Keyword.ToString(), x);
                    });
                    queryToDelete = LikerCommentorConfigModel.ManageCommentModel.LstQueries
                        .Where(x => x.Content.QueryType == postOption.ToString()).ToList();
                    if (queryToDelete.Count != PostLikeCommentorModel.ListKeywords.Count)
                        DeleteQuery(queryToDelete, PostLikeCommentorModel.ListKeywords);
                    break;
                case PostOptions.ProfileScraper:
                    PostLikeCommentorModel.ListCampaign.ForEach(x =>
                    {
                        if (LikerCommentorConfigModel.ManageCommentModel.LstQueries.FirstOrDefault(y =>
                                y.Content.QueryType == PostOptions.ProfileScraper.ToString() &&
                                y.Content.QueryValue == x) == null)
                            AddMessagesToModel(PostOptions.ProfileScraper.ToString(), x);
                    });
                    queryToDelete = LikerCommentorConfigModel.ManageCommentModel.LstQueries
                        .Where(x => x.Content.QueryType == postOption.ToString()).ToList();
                    if (queryToDelete.Count != PostLikeCommentorModel.ListCampaign.Count)
                        DeleteQuery(queryToDelete, PostLikeCommentorModel.ListCampaign);
                    break;
            }
        }

        private void DeleteQuery(List<QueryContent> queryToDelete, List<string> listGroupUrl)
        {
            try
            {
                foreach (var currentQuery in queryToDelete)
                    try
                    {
                        if (listGroupUrl.FirstOrDefault(x => x == currentQuery.Content.QueryValue.ToString()) == null)
                        {
                            var queryDelete = LikerCommentorConfigModel.ManageCommentModel.LstQueries.FirstOrDefault(
                                x =>
                                    x.Content.QueryValue == currentQuery.Content.QueryValue
                                    && x.Content.QueryType == currentQuery.Content.QueryType);


                            LikerCommentorConfigModel.ManageCommentModel.LstQueries.Remove(queryDelete);
                            foreach (var message in LikerCommentorConfigModel.LstManageCommentModel.ToList())
                            {
                                var selectedQueryToDelete = message.SelectedQuery.FirstOrDefault(x =>
                                    queryDelete != null && x.Content.QueryType == queryDelete.Content.QueryType &&
                                    x.Content.QueryValue == queryDelete.Content.QueryValue);
                                message.SelectedQuery.Remove(selectedQueryToDelete);
                                if (message.SelectedQuery.Count == 0)
                                    LikerCommentorConfigModel.LstManageCommentModel.Remove(message);
                            }
                        }
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
    }
}