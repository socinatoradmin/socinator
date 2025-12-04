using System.Collections.Generic;
using DominatorHouseCore.Models;
using DominatorHouseCore.Settings;
using DominatorHouseCore.Utility;
using GramDominatorCore.Interface;
using ProtoBuf;

namespace GramDominatorCore.GDModel
{
    [ProtoContract]
    public class UserFilterModel : BindableBase, IUserFilter
    {

        private int _minimumCharacterInBio = 5;
        [ProtoMember(1)]
        public int MinimumCharacterInBio
        {
            get
            {
                return _minimumCharacterInBio;
            }
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
            get
            {
                return _filterBioRestrictedWords;
            }
            set
            {
                if (value == _filterBioRestrictedWords)
                    return;
                SetProperty(ref _filterBioRestrictedWords, value);
            }
        }

        private bool _filterFollowersCount;
        [ProtoMember(4)]
        public bool FilterFollowersCount
        {
            get
            {
                return _filterFollowersCount;
            }
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
            get
            {
                return _followersCount;
            }
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
            get
            {
                return _filterFollowingsCount;
            }
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
            get
            {
                return _followingsCount;
            }
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
            get
            {
                return _filterPostedInRecentDays;
            }
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
            get
            {
                return _postedInRecentDays;
            }
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
            get
            {
                return _filterMaximumFollowRatio;
            }
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
            get
            {
                return _maximumFollowRatio;
            }
            set
            {
                if (value == _maximumFollowRatio) return;
                SetProperty(ref _maximumFollowRatio, value);
            }
        }


        private bool _filterMinimumFollowRatio;
        [ProtoMember(12)]
        public bool FilterMinimumFollowRatio
        {
            get
            {
                return _filterMinimumFollowRatio;
            }
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
            get
            {
                return _minimumFollowRatio;
            }
            set
            {
                if (value == _minimumFollowRatio) return;
                SetProperty(ref _minimumFollowRatio, value);
            }
        }


        private bool _filterSpecificFollowRatio;
        [ProtoMember(14)]
        public bool FilterSpecificFollowRatio
        {
            get
            {
                return _filterSpecificFollowRatio;
            }
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
            get
            {
                return _specificFollowRatio;
            }
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
            get
            {
                return _filterPostCounts;
            }
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
            get
            {
                return _postCounts;
            }
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
            get
            {
                return _genderFilters;
            }
            set
            {
                if (value == _genderFilters) return;
                SetProperty(ref _genderFilters, value);
            }
        }

        private bool _ignoreNoProfilePicUsers;
        [ProtoMember(20)]
        public bool IgnoreNoProfilePicUsers
        {
            get
            {
                return _ignoreNoProfilePicUsers;
            }
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
            get
            {
                return _ignorePrivateUser;
            }
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
            get
            {
                return _ignoreNonEnglishUser;
            }
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
            get
            {
                return _ignoreBusinessUser;
            }
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
            get
            {
                return _ignoreVerifiedUser;
            }
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
            get
            {
                return _restrictedGrouplist;
            }
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
            get
            {
                return _restrictedProfilelist;
            }
            set
            {
                if (value == _restrictedProfilelist) return;
                SetProperty(ref _restrictedProfilelist, value);
            }
        }

        private List<string> _lstInvalidWord = new List<string>();
        [ProtoMember(28)]
        public List<string> LstInvalidWord
        {
            get
            {
                return _lstInvalidWord;
            }
            set
            {
                if (value == _lstInvalidWord) return;
                SetProperty(ref _lstInvalidWord, value);
            }
        }

        private List<string> _lstvalidWord = new List<string>();
        [ProtoMember(39)]
        public List<string> LstvalidWord
        {
            get
            {
                return _lstvalidWord;
            }
            set
            {
                if (value == _lstvalidWord) return;
                SetProperty(ref _lstvalidWord, value);
            }
        }

        private List<string> _lstPostCaption = new List<string>();
        [ProtoMember(29)]
        public List<string> LstPostCaption
        {
            get
            {
                return _lstPostCaption;
            }
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
            get
            {
                return _filterMinimumCharacterInBio;
            }
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
            get
            {
                return _filterInvaildWord;
            }
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
            get
            {
                return _invalidWords;
            }
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
            get
            {
                return _captionPosts;
            }
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
            get { return _saveCloseButtonVisible; }
            set
            {
                if (value == _saveCloseButtonVisible) return;
                SetProperty(ref _saveCloseButtonVisible, value);
            }
        }

        private bool _filterNotPostedInRecentdDays;
        [ProtoMember(34)]
        public bool FilterNotPostedInRecentdDays
        {
            get
            {
                return _filterNotPostedInRecentdDays;
            }
            set
            {
                if (value == _filterNotPostedInRecentdDays) return;
                SetProperty(ref _filterNotPostedInRecentdDays, value);
            }
        }


        private int _notPostedInRecentDays = 10;
        [ProtoMember(35)]
        public int NotPostedInRecentDays
        {
            get
            {
                return _notPostedInRecentDays;
            }
            set
            {
                if (value == _notPostedInRecentDays) return;
                SetProperty(ref _notPostedInRecentDays, value);
            }
        }


        private bool _userMustHaveEmailIdAndPhoneNO;
        [ProtoMember(36)]
        public bool IsUserMustHaveEmailIdAndPhoneNO
        {
            get
            {
                return _userMustHaveEmailIdAndPhoneNO;
            }
            set
            {
                if (value == _userMustHaveEmailIdAndPhoneNO) return;
                SetProperty(ref _userMustHaveEmailIdAndPhoneNO, value);
            }
        }

        private bool _FilterBioNotRestrictedWords;
        [ProtoMember(37)]
        public bool FilterBioNotRestrictedWords
        {
            get
            {
                return _FilterBioNotRestrictedWords;
            }
            set
            {
                if (value == _FilterBioNotRestrictedWords)
                    return;
                SetProperty(ref _FilterBioNotRestrictedWords, value);
            }
        }

        private string _validWords;

        [ProtoMember(38)]
        public string ValidWords
        {
            get
            {
                return _validWords;
            }
            set
            {
                if (value == _validWords)
                    return;
                SetProperty(ref _validWords, value);
            }
        }
        private bool _userMustHaveBusinessAccount;
        [ProtoMember(40)]
        public bool IsUserMustHaveBusinessAccount
        {
            get
            {
                return _userMustHaveBusinessAccount;
            }
            set
            {
                if (value == _userMustHaveBusinessAccount) return;
                SetProperty(ref _userMustHaveBusinessAccount, value);
            }
        }


        private bool _IsVerifiedAccount;
        [ProtoMember(41)]
        public bool IsVerifiedAccount
        {
            get
            {
                return _IsVerifiedAccount;
            }
            set
            {
                if (value == _IsVerifiedAccount) return;
                SetProperty(ref _IsVerifiedAccount, value);
            }
        }
        //IgnoreBusinessUser
        private bool _VerifiedAccount;
        [ProtoMember(42)]
        public bool VerifiedAccount
        {
            get
            {
                return _VerifiedAccount;
            }
            set
            {
                if (value == _VerifiedAccount) return;
                SetProperty(ref _VerifiedAccount, value);
                IsVerifiedAccount = _VerifiedAccount;
            }
        }

        private bool _userMustHaveEmailId;
        [ProtoMember(43)]
        public bool IsUserMustHaveEmailId
        {
            get
            {
                return _userMustHaveEmailId;
            }
            set
            {
                if (value == _userMustHaveEmailId) return;
                SetProperty(ref _userMustHaveEmailId, value);
            }
        }

        private bool _userMustHavePhoneNO;
        [ProtoMember(44)]
        public bool IsUserMustHavePhoneNO
        {
            get
            {
                return _userMustHavePhoneNO;
            }
            set
            {
                if (value == _userMustHavePhoneNO) return;
                SetProperty(ref _userMustHavePhoneNO, value);
            }
        }
    }
}
