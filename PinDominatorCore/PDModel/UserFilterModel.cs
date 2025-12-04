using System.Collections.Generic;
using System.Collections.ObjectModel;
using DominatorHouseCore.Models;
using DominatorHouseCore.Settings;
using DominatorHouseCore.Utility;
using PinDominatorCore.Interface;
using ProtoBuf;

namespace PinDominatorCore.PDModel
{
    [ProtoContract]
    public class UserFilterModel : BindableBase, IUserFilter
    {
        private ObservableCollection<string> _bioRestrictedWords = new ObservableCollection<string>();

        private string _captionPosts;


        private bool _filterBioRestrictedWords;


        private bool _filterFollowersCount;


        private bool _filterFollowingsCount;

        private bool _filterInvaildWord;


        private bool _filterMaximumFollowRatio;


        private bool _filterMinimumCharacterInBio;


        private bool _filterMinimumFollowRatio;


        private bool _filterPostCounts;


        private bool _filterPostedInRecentDays;


        private bool _filterSpecificFollowRatio;


        private RangeUtilities _followersCount = new RangeUtilities(100, 200);

        private RangeUtilities _followingsCount = new RangeUtilities(100, 200);


        private GenderFilter _genderFilters = new GenderFilter();


        private bool _ignoreBusinessUser;


        private bool _ignoreCurrentUsersFollowers;


        private bool _ignoreNonEnglishUser;

        private bool _ignoreNoProfilePicUsers;


        private bool _ignorePrivateUser;


        private bool _ignoreVerifiedUser;

        private string _invalidWords;


        private List<string> _lstInvalidWord = new List<string>();


        private List<string> _lstPostCaption = new List<string>();


        private double _maximumFollowRatio = 2f;

        private int _minimumCharacterInBio = 5;

        private double _minimumFollowRatio = 1f;


        private RangeUtilities _pinCounts = new RangeUtilities(10, 20);


        private int _postedInRecentDays = 10;


        private BlacklistSettings _restrictedGrouplist = new BlacklistSettings();


        private BlacklistSettings _restrictedProfilelist = new BlacklistSettings();

        private bool _saveCloseButtonVisible;


        private RangeUtilities _specificFollowRatio = new RangeUtilities(1, 2);


        private bool _userHasInvalidWord;

        [ProtoMember(31)]
        public bool FilterInvaildWord
        {
            get => _filterInvaildWord;
            set => SetProperty(ref _filterInvaildWord, value);
        }

        [ProtoMember(32)]
        public string InvalidWords
        {
            get => _invalidWords;
            set => SetProperty(ref _invalidWords, value);
        }

        [ProtoMember(33)]
        public string CaptionPosts
        {
            get => _captionPosts;
            set => SetProperty(ref _captionPosts, value);
        }

        [ProtoMember(1)]
        public int MinimumCharacterInBio
        {
            get => _minimumCharacterInBio;
            set
            {
                if (value == _minimumCharacterInBio)
                    return;
                SetProperty(ref _minimumCharacterInBio, value);
            }
        }

        [ProtoMember(2)]
        public bool FilterBioRestrictedWords
        {
            get => _filterBioRestrictedWords;
            set
            {
                if (value == _filterBioRestrictedWords)
                    return;
                SetProperty(ref _filterBioRestrictedWords, value);
            }
        }

        [ProtoMember(3)]
        public ObservableCollection<string> BioRestrictedWords
        {
            get => _bioRestrictedWords;
            set
            {
                if (value == _bioRestrictedWords)
                    return;
                SetProperty(ref _bioRestrictedWords, value);
            }
        }

        [ProtoMember(4)]
        public bool FilterFollowersCount
        {
            get => _filterFollowersCount;
            set
            {
                if (value == _filterFollowersCount) return;
                SetProperty(ref _filterFollowersCount, value);
            }
        }

        [ProtoMember(5)]
        public RangeUtilities FollowersCount
        {
            get => _followersCount;
            set
            {
                if (value == _followersCount) return;
                SetProperty(ref _followersCount, value);
            }
        }

        [ProtoMember(6)]
        public bool FilterFollowingsCount
        {
            get => _filterFollowingsCount;
            set
            {
                if (value == _filterFollowingsCount) return;
                SetProperty(ref _filterFollowingsCount, value);
            }
        }

        [ProtoMember(7)]
        public RangeUtilities FollowingsCount
        {
            get => _followingsCount;
            set
            {
                if (value == _followingsCount) return;
                SetProperty(ref _followingsCount, value);
            }
        }

        [ProtoMember(8)]
        public bool FilterPostedInRecentDays
        {
            get => _filterPostedInRecentDays;
            set
            {
                if (value == _filterPostedInRecentDays) return;
                SetProperty(ref _filterPostedInRecentDays, value);
            }
        }

        [ProtoMember(9)]
        public int PostedInRecentDays
        {
            get => _postedInRecentDays;
            set
            {
                if (value == _postedInRecentDays) return;
                SetProperty(ref _postedInRecentDays, value);
            }
        }

        [ProtoMember(10)]
        public bool FilterMaximumFollowRatio
        {
            get => _filterMaximumFollowRatio;
            set
            {
                if (value == _filterMaximumFollowRatio) return;
                SetProperty(ref _filterMaximumFollowRatio, value);
            }
        }

        [ProtoMember(11)]
        public double MaximumFollowRatio
        {
            get => _maximumFollowRatio;
            set => SetProperty(ref _maximumFollowRatio, value);
        }

        [ProtoMember(12)]
        public bool FilterMinimumFollowRatio
        {
            get => _filterMinimumFollowRatio;
            set => SetProperty(ref _filterMinimumFollowRatio, value);
        }

        [ProtoMember(13)]
        public double MinimumFollowRatio
        {
            get => _minimumFollowRatio;
            set => SetProperty(ref _minimumFollowRatio, value);
        }

        [ProtoMember(14)]
        public bool FilterSpecificFollowRatio
        {
            get => _filterSpecificFollowRatio;
            set => SetProperty(ref _filterSpecificFollowRatio, value);
        }

        [ProtoMember(15)]
        public RangeUtilities SpecificFollowRatio
        {
            get => _specificFollowRatio;
            set => SetProperty(ref _specificFollowRatio, value);
        }

        [ProtoMember(16)]
        public bool FilterPostCounts
        {
            get => _filterPostCounts;
            set => SetProperty(ref _filterPostCounts, value);
        }

        [ProtoMember(17)]
        public RangeUtilities PinCounts
        {
            get => _pinCounts;
            set => SetProperty(ref _pinCounts, value);
        }

        [ProtoMember(18)]
        public GenderFilter GenderFilters
        {
            get => _genderFilters;
            set => SetProperty(ref _genderFilters, value);
        }

        [ProtoMember(19)]
        public bool IgnoreCurrentUsersFollowers
        {
            get => _ignoreCurrentUsersFollowers;
            set => SetProperty(ref _ignoreCurrentUsersFollowers, value);
        }

        [ProtoMember(20)]
        public bool IgnoreNoProfilePicUsers
        {
            get => _ignoreNoProfilePicUsers;
            set => SetProperty(ref _ignoreNoProfilePicUsers, value);
        }

        [ProtoMember(21)]
        public bool IgnorePrivateUser
        {
            get => _ignorePrivateUser;
            set => SetProperty(ref _ignorePrivateUser, value);
        }

        [ProtoMember(22)]
        public bool IgnoreNonEnglishUser
        {
            get => _ignoreNonEnglishUser;
            set => SetProperty(ref _ignoreNonEnglishUser, value);
        }

        [ProtoMember(23)]
        public bool IgnoreBusinessUser
        {
            get => _ignoreBusinessUser;
            set => SetProperty(ref _ignoreBusinessUser, value);
        }

        [ProtoMember(24)]
        public bool IgnoreVerifiedUser
        {
            get => _ignoreVerifiedUser;
            set => SetProperty(ref _ignoreVerifiedUser, value);
        }

        [ProtoMember(25)]
        public BlacklistSettings RestrictedGrouplist
        {
            get => _restrictedGrouplist;
            set => SetProperty(ref _restrictedGrouplist, value);
        }

        [ProtoMember(26)]
        public BlacklistSettings RestrictedProfilelist
        {
            get => _restrictedProfilelist;
            set => SetProperty(ref _restrictedProfilelist, value);
        }

        [ProtoMember(27)]
        public bool UserHasInvalidWord
        {
            get => _userHasInvalidWord;
            set => SetProperty(ref _userHasInvalidWord, value);
        }

        [ProtoMember(28)]
        public List<string> LstInvalidWord
        {
            get => _lstInvalidWord;
            set => SetProperty(ref _lstInvalidWord, value);
        }

        [ProtoMember(29)]
        public List<string> LstPostCaption
        {
            get => _lstPostCaption;
            set => SetProperty(ref _lstPostCaption, value);
        }

        [ProtoMember(30)]
        public bool FilterMinimumCharacterInBio
        {
            get => _filterMinimumCharacterInBio;
            set => SetProperty(ref _filterMinimumCharacterInBio, value);
        }

        public bool SaveCloseButtonVisible
        {
            get => _saveCloseButtonVisible;
            set => SetProperty(ref _saveCloseButtonVisible, value);
        }
    }
}