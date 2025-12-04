using System;
using System.Collections.Generic;
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
    public class UnFollower : BindableBase
    {
        private string _customUsers;
        private int _followedBeforeDay;
        private int _followedBeforeHour;
        private bool _isChkCustomUsersListChecked;
        private bool _isChkPeopleFollowedBySoftwareCheecked;
        private bool _isChkPeopleFollowedOutsideSoftwareChecked;
        private bool _isUserFollowedBeforeChecked;
        private bool _isWhoDoNotFollowBackChecked;
        private bool _isWhoFollowBackChecked;
        private List<string> _listOfCustomUsers = new List<string>();

        [ProtoMember(3)]
        public bool IsChkPeopleFollowedBySoftwareCheecked
        {
            get => _isChkPeopleFollowedBySoftwareCheecked;
            set => SetProperty(ref _isChkPeopleFollowedBySoftwareCheecked, value);
        }


        [ProtoMember(4)]
        public bool IsChkPeopleFollowedOutsideSoftwareChecked
        {
            get => _isChkPeopleFollowedOutsideSoftwareChecked;
            set => SetProperty(ref _isChkPeopleFollowedOutsideSoftwareChecked, value);
        }


        [ProtoMember(5)]
        public bool IsChkCustomUsersListChecked
        {
            get => _isChkCustomUsersListChecked;
            set => SetProperty(ref _isChkCustomUsersListChecked, value);
        }


        [ProtoMember(6)]
        public bool IsWhoDoNotFollowBackChecked
        {
            get => _isWhoDoNotFollowBackChecked;
            set => SetProperty(ref _isWhoDoNotFollowBackChecked, value);
        }


        [ProtoMember(7)]
        public bool IsWhoFollowBackChecked
        {
            get => _isWhoFollowBackChecked;
            set => SetProperty(ref _isWhoFollowBackChecked, value);
        }

        [ProtoMember(8)]
        public bool IsUserFollowedBeforeChecked
        {
            get => _isUserFollowedBeforeChecked;
            set => SetProperty(ref _isUserFollowedBeforeChecked, value);
        }


        [ProtoMember(9)]
        public int FollowedBeforeDay
        {
            get => _followedBeforeDay;
            set => SetProperty(ref _followedBeforeDay, value);
        }


        [ProtoMember(10)]
        public int FollowedBeforeHour
        {
            get => _followedBeforeHour;
            set => SetProperty(ref _followedBeforeHour, value);
        }


        [ProtoMember(11)]
        public string CustomUsers
        {
            get => _customUsers;
            set => SetProperty(ref _customUsers, value);
        }


        [ProtoMember(12)]
        public List<string> ListCustomUsers
        {
            get => _listOfCustomUsers;
            set => SetProperty(ref _listOfCustomUsers, value);
        }

        [ProtoMember(13)]
        public bool IsRdWhoDoNotFollowBackChecked
        {
            get => _isWhoDoNotFollowBackChecked;
            set
            {
                SetProperty(ref _isWhoDoNotFollowBackChecked, value);
                IsWhoDoNotFollowBackChecked = value;
            }
        }

        [ProtoMember(14)]
        public bool IsRdWhoFollowBackChecked
        {
            get => _isWhoFollowBackChecked;
            set
            {
                SetProperty(ref _isWhoFollowBackChecked, value);
                IsWhoFollowBackChecked = value;
            }
        }
    }

    public interface IUnFollowerViewModel : ITwitterVisibilityModel
    {
        bool IsChkPeopleFollowedBySoftwareChecked { get; set; }
        bool IsChkPeopleFollowedOutsideSoftwareChecked { get; set; }
        bool IsChkCustomUsersListChecked { get; set; }
        string CustomUsersList { get; set; }
        bool IsWhoDoNotFollowBackChecked { get; set; }
        bool IsWhoFollowBackChecked { get; set; }
        bool IsUserFollowedBeforeChecked { get; set; }
        int FollowedBeforeDay { get; set; }
        int FollowedBeforeHour { get; set; }

        UnFollower UnFollower { get; set; }
    }

    public class UnFollowerViewModel : StartupBaseViewModel, IUnFollowerViewModel
    {
        private string _customUsersList;
        private int _followedBeforeDay;

        private int _followedBeforeHour;
        private bool _isChkCustomUsersListChecked;
        private bool _isChkPeopleFollowedBySoftwareChecked;
        private bool _isChkPeopleFollowedOutsideSoftwareChecked;

        private bool _isUserFollowedBeforeChecked;
        private bool _isWhoDoNotFollowBackChecked;
        private bool _isWhoFollowBackChecked;
        private UnFollower _UnFollower = new UnFollower();
        private readonly SocialNetworks network;

        public UnFollowerViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig {Model = this, ActivityType = ActivityType.Unfollow});
            IsNonQuery = true;
            NextCommand = new DelegateCommand(NextValidation);
            PreviousCommand = new DelegateCommand(NavigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);

            ElementsVisibility.NetworkElementsVisibilty(this);

            if (InstagramElementsVisibility == Visibility.Visible)
                AllVisibility = Visibility.Collapsed;
            network = InstanceProvider.GetInstance<ISelectActivityViewModel>().SelectAccount.AccountBaseModel
                .AccountNetwork;
            if (network == SocialNetworks.Reddit || network == SocialNetworks.Instagram)
                IsVisible = true;
            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfUnfollowPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfUnfollowPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfUnfollowPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfUnfollowPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxUnfollowsPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
        }

        public Visibility InstagramElementsVisibility { get; set; } = Visibility.Collapsed;
        public Visibility AllVisibility { get; set; } = Visibility.Visible;
        public bool IsVisible { get; set; }

        [ProtoMember(13)]
        public bool IsRdWhoDoNotFollowBackChecked
        {
            get => _isWhoDoNotFollowBackChecked;
            set
            {
                SetProperty(ref _isWhoDoNotFollowBackChecked, value);
                IsWhoDoNotFollowBackChecked = value;
            }
        }

        [ProtoMember(14)]
        public bool IsRdWhoFollowBackChecked
        {
            get => _isWhoFollowBackChecked;
            set
            {
                SetProperty(ref _isWhoFollowBackChecked, value);
                IsWhoFollowBackChecked = value;
            }
        }

        public Visibility TwitterElementsVisibility { get; set; } = Visibility.Collapsed;

        public UnFollower UnFollower
        {
            get => _UnFollower;
            set => SetProperty(ref _UnFollower, value);
        }

        public string CustomUsersList
        {
            get => _customUsersList;
            set
            {
                SetProperty(ref _customUsersList, value);
                UnFollower.CustomUsers = value;
            }
        }

        public int FollowedBeforeDay
        {
            get => _followedBeforeDay;
            set
            {
                SetProperty(ref _followedBeforeDay, value);
                UnFollower.FollowedBeforeDay = value;
            }
        }

        public int FollowedBeforeHour
        {
            get => _followedBeforeHour;
            set
            {
                SetProperty(ref _followedBeforeHour, value);
                UnFollower.FollowedBeforeHour = value;
            }
        }

        public bool IsChkCustomUsersListChecked
        {
            get => _isChkCustomUsersListChecked;
            set
            {
                SetProperty(ref _isChkCustomUsersListChecked, value);
                UnFollower.IsChkCustomUsersListChecked = value;
            }
        }

        public bool IsChkPeopleFollowedBySoftwareChecked
        {
            get => _isChkPeopleFollowedBySoftwareChecked;
            set
            {
                SetProperty(ref _isChkPeopleFollowedBySoftwareChecked, value);
                UnFollower.IsChkPeopleFollowedBySoftwareCheecked = value;
            }
        }

        public bool IsChkPeopleFollowedOutsideSoftwareChecked
        {
            get => _isChkPeopleFollowedOutsideSoftwareChecked;
            set
            {
                SetProperty(ref _isChkPeopleFollowedOutsideSoftwareChecked, value);
                UnFollower.IsChkPeopleFollowedOutsideSoftwareChecked = value;
            }
        }

        public bool IsUserFollowedBeforeChecked
        {
            get => _isUserFollowedBeforeChecked;
            set
            {
                SetProperty(ref _isUserFollowedBeforeChecked, value);
                UnFollower.IsUserFollowedBeforeChecked = value;
            }
        }

        public bool IsWhoDoNotFollowBackChecked
        {
            get => _isWhoDoNotFollowBackChecked;
            set
            {
                SetProperty(ref _isWhoDoNotFollowBackChecked, value);
                UnFollower.IsWhoDoNotFollowBackChecked = value;
            }
        }

        public bool IsWhoFollowBackChecked
        {
            get => _isWhoFollowBackChecked;
            set
            {
                SetProperty(ref _isWhoFollowBackChecked, value);
                UnFollower.IsWhoFollowBackChecked = value;
            }
        }

        private void NextValidation()
        {
            if (_UnFollower.IsChkPeopleFollowedOutsideSoftwareChecked ||
                _UnFollower.IsChkPeopleFollowedBySoftwareCheecked || _UnFollower.IsChkCustomUsersListChecked)
            {
                if (network != SocialNetworks.Reddit)
                {
                    if (_UnFollower.IsWhoDoNotFollowBackChecked || _UnFollower.IsWhoFollowBackChecked)
                    {
                        if (CheckCustomUser())
                            NavigateNext();
                    }
                    else
                    {
                        Dialog.ShowDialog("Error", "Please select Unfollow source type.");
                    }
                }
                else if (CheckCustomUser())
                {
                    NavigateNext();
                }
            }
            else
            {
                Dialog.ShowDialog("Error", "Please select Unfollow source.");
            }
        }

        private bool CheckCustomUser()
        {
            if (!_UnFollower.IsChkCustomUsersListChecked) return true;

            if (!string.IsNullOrEmpty(_UnFollower.CustomUsers))
            {
                return true;
            }

            Dialog.ShowDialog("Error", "Please enter user list.");
            return false;
        }
    }
}