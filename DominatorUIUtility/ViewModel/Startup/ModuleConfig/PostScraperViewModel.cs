using System;
using System.Linq;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.FacebookModels;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Views.ViewModel.Startup.ModuleConfig;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public interface IPostScraperViewModel
    {
    }

    public class PostScraperViewModel : StartupBaseViewModel, IPostScraperViewModel
    {
        private PostLikeCommentorModel _postLikeCommentorModel = new PostLikeCommentorModel();

        public PostScraperViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig { Model = this, ActivityType = ActivityType.PostScraper });

            NextCommand = new DelegateCommand(PostScraperValidate);
            PreviousCommand = new DelegateCommand(NavigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);

            ElementsVisibility.NetworkElementsVisibilty(this);

            //For Facebook Reply to all message is not required
            if (FacebookElementsVisibility == Visibility.Visible)
                AllVisibility = Visibility.Collapsed;
            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyScrapNumberOfPostsPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyScrapNumberOfPostsPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyScrapNumberOfPostsPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyScrapNumberOfPostsPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyScrapMaxPostsPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            ListQueryType.Clear();
        }

        public Visibility FacebookElementsVisibility { get; set; } = Visibility.Collapsed;
        public Visibility AllVisibility { get; set; } = Visibility.Visible;

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

        private void PostScraperValidate()
        {
            if (FacebookElementsVisibility == Visibility.Visible)
            {
                IsNonQuery = true;
                if (!PostLikeCommentorModel.IsOwnWallChecked && !PostLikeCommentorModel.IsNewsfeedChecked &&
                    !PostLikeCommentorModel.IsFriendTimeLineChecked && !PostLikeCommentorModel.IsCustomPostListChecked
                    && !PostLikeCommentorModel.IsCampaignChecked && !PostLikeCommentorModel.IsGroupChecked
                    && !PostLikeCommentorModel.IsPageChecked && !PostLikeCommentorModel.IsCampaignChked)
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

                if (PostLikeCommentorModel.IsCampaignChecked &&
                    PostLikeCommentorModel.ListFaceDominatorCampaign.Count == 0)
                {
                    Dialog.ShowDialog("Error", "Please add at least one query.");
                    return;
                }

                if (PostLikeCommentorModel.IsCustomPostListChecked &&
                    PostLikeCommentorModel.ListCustomPostList.Count == 0)
                {
                    Dialog.ShowDialog("Error", "Please add at least one query.");
                    return;
                }

                if (PostLikeCommentorModel.IsCampaignChked && PostLikeCommentorModel.ListCampaign.Count == 0)
                {
                    Dialog.ShowDialog("Error", "Please add at least one query And save.");
                    return;
                }

                if (PostLikeCommentorModel.IsHashtagChecked && PostLikeCommentorModel.ListHashtags.Count == 0)
                {
                    Dialog.ShowDialog("Error", "Please add atleast one query");
                }

                if (PostLikeCommentorModel.IsPostSharerChecked && PostLikeCommentorModel.ListPostSharer.Count == 0)
                {
                    Dialog.ShowDialog("Error", "Please add atleast one query");
                }
            }

            NavigateNext();
        }
    }
}