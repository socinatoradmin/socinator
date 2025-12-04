using System;
using System.Linq;
using System.Windows;
using CommonServiceLocator;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Views.ViewModel.Startup.ModuleConfig;
using Prism.Commands;
using Prism.Regions;
using ProtoBuf;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    [ProtoContract]
    public class UnLike : BindableBase
    {
        private string _CustomTweets = string.Empty;
        private bool _IsCustomTweets;
        private bool _IsLikedTweets;

        [ProtoMember(1)]
        public bool IsLikedTweets
        {
            get => _IsLikedTweets;
            set => SetProperty(ref _IsLikedTweets, value);
        }

        [ProtoMember(2)]
        public bool IsCustomTweets
        {
            get => _IsCustomTweets;
            set => SetProperty(ref _IsCustomTweets, value);
        }

        [ProtoMember(3)]
        public string CustomTweets
        {
            get => _CustomTweets;
            set => SetProperty(ref _CustomTweets, value);
        }
    }

    public interface IUnlikeViewModel : ITwitterVisibilityModel, IInstagramVisibilityModel
    {
        bool IsCheckedUnlikeMedia { get; set; }
        UnLike UnLike { get; set; }
    }

    public class UnlikeViewModel : StartupBaseViewModel, IUnlikeViewModel
    {
        private bool _IsCheckedUnlikeMedia = true;

        private UnLike _UnLike = new UnLike();

        public UnlikeViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig {Model = this, ActivityType = ActivityType.Unlike});
            ElementsVisibility.NetworkElementsVisibilty(this);
            IsNonQuery = true;
            NextCommand = new DelegateCommand(validateAndNevigate);
            PreviousCommand = new DelegateCommand(NavigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);
            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfUnLikesPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfUnLikesPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfUnLikesPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfUnLikesPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxUnLikesPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            ListQueryType.Clear();
        }

        public Visibility InstagramElementsVisibility { get; set; } = Visibility.Collapsed;
        public Visibility TwitterElementsVisibility { get; set; } = Visibility.Collapsed;

        public bool IsCheckedUnlikeMedia
        {
            get => _IsCheckedUnlikeMedia;
            set => SetProperty(ref _IsCheckedUnlikeMedia, value);
        }

        public UnLike UnLike
        {
            get => _UnLike;
            set => SetProperty(ref _UnLike, value);
        }

        private void validateAndNevigate()
        {
            var network = InstanceProvider.GetInstance<ISelectActivityViewModel>().SelectAccount.AccountBaseModel
                .AccountNetwork;
            if (network == SocialNetworks.Twitter)
            {
                if (!UnLike.IsLikedTweets && !UnLike.IsCustomTweets)
                {
                    Dialog.ShowDialog("Error", "Please select at least one source type.");
                    return;
                }

                if (UnLike.IsCustomTweets && string.IsNullOrEmpty(UnLike.CustomTweets.Trim()))
                {
                    Dialog.ShowDialog("Error", "Please type some Tweets.");
                    return;
                }
            }
            else if (network == SocialNetworks.Instagram)
            {
                if (!IsCheckedUnlikeMedia)
                {
                    Dialog.ShowDialog("Error", "Please check Source Type");
                    return;
                }
            }

            NavigateNext();
        }
    }
}