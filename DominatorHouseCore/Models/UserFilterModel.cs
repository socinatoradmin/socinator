#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Settings
{
    [ProtoContract]
    public class UserFilterModel : BindableBase
    {
        private int _minimumCharacterInBio = 5;

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


        private bool _filterBioRestrictedWords;

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


        private ObservableCollection<string> _bioRestrictedWords = new ObservableCollection<string>();

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


        private bool _filterFollowersCount;

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


        private RangeUtilities _followersCount = new RangeUtilities(100, 200);

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


        private bool _filterFollowingsCount;

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

        private RangeUtilities _followingsCount = new RangeUtilities(100, 200);

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


        private bool _filterPostedInRecentDays;

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


        private int _postedInRecentDays = 10;

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


        private bool _filterMaximumFollowRatio;

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


        private double _maximumFollowRatio = 2f;

        [ProtoMember(11)]
        public double MaximumFollowRatio
        {
            get => _maximumFollowRatio;
            set
            {
                if (Math.Abs(value - _maximumFollowRatio) < 0.000001) return;
                SetProperty(ref _maximumFollowRatio, value);
            }
        }


        private bool _filterMinimumFollowRatio;

        [ProtoMember(12)]
        public bool FilterMinimumFollowRatio
        {
            get => _filterMinimumFollowRatio;
            set
            {
                if (value == _filterMinimumFollowRatio) return;
                SetProperty(ref _filterMinimumFollowRatio, value);
            }
        }

        private double _minimumFollowRatio = 1f;

        [ProtoMember(13)]
        public double MinimumFollowRatio
        {
            get => _minimumFollowRatio;
            set
            {
                if (Math.Abs(value - _minimumFollowRatio) < 0.000001) return;
                SetProperty(ref _minimumFollowRatio, value);
            }
        }


        private bool _filterSpecificFollowRatio;

        [ProtoMember(14)]
        public bool FilterSpecificFollowRatio
        {
            get => _filterSpecificFollowRatio;
            set
            {
                if (value == _filterSpecificFollowRatio) return;
                SetProperty(ref _filterSpecificFollowRatio, value);
            }
        }


        private RangeUtilities _specificFollowRatio = new RangeUtilities(1, 2);

        [ProtoMember(15)]
        public RangeUtilities SpecificFollowRatio
        {
            get => _specificFollowRatio;
            set
            {
                if (value == _specificFollowRatio) return;
                SetProperty(ref _specificFollowRatio, value);
            }
        }


        private bool _filterPostCounts;

        [ProtoMember(16)]
        public bool FilterPostCounts
        {
            get => _filterPostCounts;
            set
            {
                if (value == _filterPostCounts) return;
                SetProperty(ref _filterPostCounts, value);
            }
        }


        private RangeUtilities _postCounts = new RangeUtilities(10, 20);

        [ProtoMember(17)]
        public RangeUtilities PostCounts
        {
            get => _postCounts;
            set
            {
                if (value == _postCounts) return;
                SetProperty(ref _postCounts, value);
            }
        }


        private GenderFilter _genderFilters = new GenderFilter();

        [ProtoMember(18)]
        public GenderFilter GenderFilters
        {
            get => _genderFilters;
            set
            {
                if (value == _genderFilters) return;
                SetProperty(ref _genderFilters, value);
            }
        }


        private bool _ignoreCurrentUsersFollowers;

        [ProtoMember(19)]
        public bool IgnoreCurrentUsersFollowers
        {
            get => _ignoreCurrentUsersFollowers;
            set
            {
                if (value == _ignoreCurrentUsersFollowers) return;
                SetProperty(ref _ignoreCurrentUsersFollowers, value);
            }
        }

        private bool _ignoreNoProfilePicUsers;

        [ProtoMember(20)]
        public bool IgnoreNoProfilePicUsers
        {
            get => _ignoreNoProfilePicUsers;
            set
            {
                if (value == _ignoreNoProfilePicUsers) return;
                SetProperty(ref _ignoreNoProfilePicUsers, value);
            }
        }


        private bool _ignorePrivateUser;

        [ProtoMember(21)]
        public bool IgnorePrivateUser
        {
            get => _ignorePrivateUser;
            set
            {
                if (value == _ignorePrivateUser) return;
                SetProperty(ref _ignorePrivateUser, value);
            }
        }


        private bool _ignoreNonEnglishUser;

        [ProtoMember(22)]
        public bool IgnoreNonEnglishUser
        {
            get => _ignoreNonEnglishUser;
            set
            {
                if (value == _ignoreNonEnglishUser) return;
                SetProperty(ref _ignoreNonEnglishUser, value);
            }
        }


        private bool _ignoreBusinessUser;

        [ProtoMember(23)]
        public bool IgnoreBusinessUser
        {
            get => _ignoreBusinessUser;
            set
            {
                if (value == _ignoreBusinessUser) return;
                SetProperty(ref _ignoreBusinessUser, value);
            }
        }


        private bool _ignoreVerifiedUser;

        [ProtoMember(24)]
        public bool IgnoreVerifiedUser
        {
            get => _ignoreVerifiedUser;
            set
            {
                if (value == _ignoreVerifiedUser) return;
                SetProperty(ref _ignoreVerifiedUser, value);
            }
        }


        private BlacklistSettings _restrictedGrouplist = new BlacklistSettings();

        [ProtoMember(25)]
        public BlacklistSettings RestrictedGrouplist
        {
            get => _restrictedGrouplist;
            set
            {
                if (value == _restrictedGrouplist) return;
                SetProperty(ref _restrictedGrouplist, value);
            }
        }


        private BlacklistSettings _restrictedProfilelist = new BlacklistSettings();

        [ProtoMember(26)]
        public BlacklistSettings RestrictedProfilelist
        {
            get => _restrictedProfilelist;
            set
            {
                if (value == _restrictedProfilelist) return;
                SetProperty(ref _restrictedProfilelist, value);
            }
        }


        private bool _userHasInvalidWord;

        [ProtoMember(27)]
        public bool UserHasInvalidWord
        {
            get => _userHasInvalidWord;
            set
            {
                if (value == _userHasInvalidWord) return;
                SetProperty(ref _userHasInvalidWord, value);
            }
        }


        private List<string> _lstInvalidWord = new List<string>();

        [ProtoMember(28)]
        public List<string> LstInvalidWord
        {
            get => _lstInvalidWord;
            set
            {
                if (value == _lstInvalidWord) return;
                SetProperty(ref _lstInvalidWord, value);
            }
        }


        private List<string> _lstPostCaption = new List<string>();

        [ProtoMember(29)]
        public List<string> LstPostCaption
        {
            get => _lstPostCaption;
            set
            {
                if (value == _lstPostCaption) return;
                SetProperty(ref _lstPostCaption, value);
            }
        }


        private bool _filterMinimumCharacterInBio;

        [ProtoMember(30)]
        public bool FilterMinimumCharacterInBio
        {
            get => _filterMinimumCharacterInBio;
            set
            {
                if (value == _filterMinimumCharacterInBio) return;
                SetProperty(ref _filterMinimumCharacterInBio, value);
            }
        }

        private bool _filterInvaildWord;

        [ProtoMember(31)]
        public bool FilterInvaildWord
        {
            get => _filterInvaildWord;
            set
            {
                if (value == _filterInvaildWord) return;
                SetProperty(ref _filterInvaildWord, value);
            }
        }

        private string _invalidWords;

        [ProtoMember(32)]
        public string InvalidWords
        {
            get => _invalidWords;
            set
            {
                if (value == _invalidWords)
                    return;
                SetProperty(ref _invalidWords, value);
            }
        }

        private string _captionPosts;

        [ProtoMember(33)]
        public string CaptionPosts
        {
            get => _captionPosts;
            set
            {
                if (value == _captionPosts)
                    return;
                SetProperty(ref _captionPosts, value);
            }
        }

        private bool _saveCloseButtonVisible;

        public bool SaveCloseButtonVisible
        {
            get => _saveCloseButtonVisible;
            set
            {
                if (value == _saveCloseButtonVisible) return;
                SetProperty(ref _saveCloseButtonVisible, value);
            }
        }

        public bool FilterMaxDaysSinceLastPost { get; set; }
    }
}