using System;
using System.Linq;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.FacebookModels;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public interface IDownloadScraperViewModel
    {
    }

    public class DownloadScraperViewModel : StartupBaseViewModel, IDownloadScraperViewModel
    {
        private PostLikeCommentorModel _postLikeCommentorModel = new PostLikeCommentorModel();

        public DownloadScraperViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig {Model = this, ActivityType = ActivityType.DownloadScraper});
            IsNonQuery = true;
            NextCommand = new DelegateCommand(DownloadScraperValidate);
            PreviousCommand = new DelegateCommand(NavigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);
            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfDownloadPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfDownloadPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfDownloadPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfDownloadPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxDownloadPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            ListQueryType.Clear();
        }

        public PostLikeCommentorModel PostLikeCommentorModel
        {
            get => _postLikeCommentorModel;
            set
            {
                if ((_postLikeCommentorModel == null) & (_postLikeCommentorModel == value))
                    return;
                SetProperty(ref _postLikeCommentorModel, value);
            }
        }


        public void DownloadScraperValidate()
        {
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

            if (PostLikeCommentorModel.IsAlbumsChecked && PostLikeCommentorModel.ListAlbums.Count == 0)
            {
                Dialog.ShowDialog("Error", "Please add at least one query.");
                return;
            }

            if (PostLikeCommentorModel.IsKeywordChecked && PostLikeCommentorModel.ListKeywords.Count == 0)
            {
                Dialog.ShowDialog("Error", "Please add at least one query.");
                return;
            }

            if (!PostLikeCommentorModel.IsOwnWallChecked && !PostLikeCommentorModel.IsNewsfeedChecked &&
                !PostLikeCommentorModel.IsFriendTimeLineChecked && !PostLikeCommentorModel.IsCustomPostListChecked
                && !PostLikeCommentorModel.IsCampaignChecked && !PostLikeCommentorModel.IsGroupChecked
                && !PostLikeCommentorModel.IsPageChecked && !PostLikeCommentorModel.IsKeywordChecked)
            {
                Dialog.ShowDialog("Error", "Please Select atleast one option.");
                return;
            }

            NavigateNext();
        }
    }
}