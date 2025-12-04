using DominatorHouseCore.Utility;
using System.Collections.Generic;
using TumblrDominatorCore.Interface;

namespace TumblrDominatorCore.Models
{
    public class UserFilterModel : BindableBase, IUserFilter
    {
        private RangeUtilities _filterFollowersRange = new RangeUtilities { StartValue = 0, EndValue = 1 };

        private RangeUtilities _filterFollowingsRange = new RangeUtilities { StartValue = 0, EndValue = 1 };

        private int _filterFollowRatioGreaterThanValue;

        private int _filterFollowRatioSmallerThanValue;

        private int _filterMinimumCharactersValue;
        private int _filterMinimumNumberOfTweetsValue;

        private int _filterTweetedWithinTheLastValue;


        private string _invalidWordsInBio;
        private bool _isFilterActiveVerfiedUser;
        private bool _isFilterActiveWhoAreNotVerified = true;
        private bool _isFilterBioHasMinimumCharacters;
        private bool _isFilterByVerification;
        private bool _isFilterEnglishCharactersInBio;
        private bool _isFilterFollowersRange;
        private bool _isFilterFollowingsRange;
        private bool _isFilterFollowRatioGreaterThan;

        private bool _isFilterFollowRatioSmallerThan;
        private bool _isFilterMinimumNumberOfTweets;

        private bool _isFilterMustContainSpecificWords;

        private bool _isFilterMustNotContainInvalidWords;
        private bool _isFilterMutedUser;

        private bool _isFilterPrivateUser;

        private bool _isFilterProfileImage;
        private bool _isFilterTweetedWithinLastDays;
        private bool _isFilterUserIsNotFollowingThisAccount;
        private bool _isSkipUsersWhoWereAlreadySentAMessageFromSoftware;

        private List<string> _lstInvalidWords = new List<string>();
        private List<string> _lstSpecificAccountNotFollowing = new List<string>();
        private List<string> _lstSpecificWords = new List<string>();
        private bool _saveCloseButtonVisible;
        private string _specificAccountNotFollowing;
        private string _specificWordsInBio;

        public bool IsSkipUsersWhoWereAlreadySentAMessageFromSoftware
        {
            get => _isSkipUsersWhoWereAlreadySentAMessageFromSoftware;
            set => SetProperty(ref _isSkipUsersWhoWereAlreadySentAMessageFromSoftware, value);
        }


        public bool IsFilterEnglishCharactersInBio
        {
            get => _isFilterEnglishCharactersInBio;
            set
            {
                if (value == _isFilterEnglishCharactersInBio)
                    return;
                SetProperty(ref _isFilterEnglishCharactersInBio, value);
            }
        }

        public bool IsFilterProfileImage
        {
            get => _isFilterProfileImage;
            set
            {
                if (value == _isFilterProfileImage)
                    return;
                SetProperty(ref _isFilterProfileImage, value);
            }
        }

        public bool IsFilterMinimumNumberOfTweets
        {
            get => _isFilterMinimumNumberOfTweets;
            set
            {
                if (value == _isFilterMinimumNumberOfTweets)
                    return;
                SetProperty(ref _isFilterMinimumNumberOfTweets, value);
            }
        }

        public bool IsFilterFollowersRange
        {
            get => _isFilterFollowersRange;
            set
            {
                if (value == _isFilterFollowersRange)
                    return;
                SetProperty(ref _isFilterFollowersRange, value);
            }
        }

        public bool IsFilterFollowingsRange
        {
            get => _isFilterFollowingsRange;
            set
            {
                if (value == _isFilterFollowingsRange)
                    return;
                SetProperty(ref _isFilterFollowingsRange, value);
            }
        }

        public bool IsFilterFollowRatioGreaterThan
        {
            get => _isFilterFollowRatioGreaterThan;
            set
            {
                if (value == _isFilterFollowRatioGreaterThan)
                    return;
                SetProperty(ref _isFilterFollowRatioGreaterThan, value);
            }
        }

        public bool IsFilterFollowRatioSmallerThan
        {
            get => _isFilterFollowRatioSmallerThan;
            set
            {
                if (value == _isFilterFollowRatioSmallerThan)
                    return;
                SetProperty(ref _isFilterFollowRatioSmallerThan, value);
            }
        }

        public bool IsFilterBioHasMinimumCharacters
        {
            get => _isFilterBioHasMinimumCharacters;
            set
            {
                if (value == _isFilterBioHasMinimumCharacters)
                    return;
                SetProperty(ref _isFilterBioHasMinimumCharacters, value);
            }
        }

        public bool IsFilterMustNotContainInvalidWords
        {
            get => _isFilterMustNotContainInvalidWords;
            set
            {
                if (value == _isFilterMustNotContainInvalidWords)
                    return;
                SetProperty(ref _isFilterMustNotContainInvalidWords, value);
            }
        }

        public bool IsFilterMustContainSpecificWords
        {
            get => _isFilterMustContainSpecificWords;
            set
            {
                if (value == _isFilterMustContainSpecificWords)
                    return;
                SetProperty(ref _isFilterMustContainSpecificWords, value);
            }
        }

        public bool IsFilterTweetedWithinLastDays
        {
            get => _isFilterTweetedWithinLastDays;
            set
            {
                if (value == _isFilterTweetedWithinLastDays)
                    return;
                SetProperty(ref _isFilterTweetedWithinLastDays, value);
            }
        }

        public bool IsFilterUserIsNotFollowingThisAccount
        {
            get => _isFilterUserIsNotFollowingThisAccount;
            set
            {
                if (value == _isFilterUserIsNotFollowingThisAccount)
                    return;
                SetProperty(ref _isFilterUserIsNotFollowingThisAccount, value);
            }
        }

        public bool IsFilterPrivateUser
        {
            get => _isFilterPrivateUser;
            set
            {
                if (value == _isFilterPrivateUser)
                    return;
                SetProperty(ref _isFilterPrivateUser, value);
            }
        }

        public int FilterMinimumNumberOfTweetsValue
        {
            get => _filterMinimumNumberOfTweetsValue;
            set
            {
                if (value == _filterMinimumNumberOfTweetsValue)
                    return;
                SetProperty(ref _filterMinimumNumberOfTweetsValue, value);
            }
        }

        public RangeUtilities FilterFollowersRange
        {
            get => _filterFollowersRange;
            set
            {
                if (value == _filterFollowersRange)
                    return;
                SetProperty(ref _filterFollowersRange, value);
            }
        }

        public RangeUtilities FilterFollowingsRange
        {
            get => _filterFollowingsRange;
            set
            {
                if (value == _filterFollowingsRange)
                    return;
                SetProperty(ref _filterFollowingsRange, value);
            }
        }

        public int FilterFollowRatioGreaterThanValue
        {
            get => _filterFollowRatioGreaterThanValue;
            set
            {
                if (value == _filterFollowRatioGreaterThanValue)
                    return;
                SetProperty(ref _filterFollowRatioGreaterThanValue, value);
            }
        }

        public int FilterFollowRatioSmallerThanValue
        {
            get => _filterFollowRatioSmallerThanValue;
            set
            {
                if (value == _filterFollowRatioSmallerThanValue)
                    return;
                SetProperty(ref _filterFollowRatioSmallerThanValue, value);
            }
        }

        public int FilterMinimumCharactersValue
        {
            get => _filterMinimumCharactersValue;
            set
            {
                if (value == _filterMinimumCharactersValue)
                    return;
                SetProperty(ref _filterMinimumCharactersValue, value);
            }
        }

        public int FilterTweetedWithinTheLastValue
        {
            get => _filterTweetedWithinTheLastValue;
            set
            {
                if (value == _filterTweetedWithinTheLastValue)
                    return;
                SetProperty(ref _filterTweetedWithinTheLastValue, value);
            }
        }

        public string InvalidWordsInBio
        {
            get => _invalidWordsInBio;
            set
            {
                if (value == _invalidWordsInBio)
                    return;
                SetProperty(ref _invalidWordsInBio, value);
            }
        }

        public string SpecificWordsInBio
        {
            get => _specificWordsInBio;
            set
            {
                if (value == _specificWordsInBio)
                    return;
                SetProperty(ref _specificWordsInBio, value);
            }
        }

        public string SpecificAccountNotFollowing
        {
            get => _specificAccountNotFollowing;
            set
            {
                if (value == _specificAccountNotFollowing)
                    return;
                SetProperty(ref _specificAccountNotFollowing, value);
            }
        }

        public List<string> LstInvalidWords
        {
            get => _lstInvalidWords;
            set
            {
                if (value == _lstInvalidWords)
                    return;
                SetProperty(ref _lstInvalidWords, value);
            }
        }

        public List<string> LstSpecificWords
        {
            get => _lstSpecificWords;
            set
            {
                if (value == _lstSpecificWords)
                    return;
                SetProperty(ref _lstSpecificWords, value);
            }
        }

        public List<string> LstSpecificAccountNotFollowing
        {
            get => _lstSpecificAccountNotFollowing;
            set
            {
                if (value == _lstSpecificAccountNotFollowing)
                    return;
                SetProperty(ref _lstSpecificAccountNotFollowing, value);
            }
        }

        public bool SaveCloseButtonVisible
        {
            get => _saveCloseButtonVisible;
            set
            {
                if (value == _saveCloseButtonVisible) return;
                SetProperty(ref _saveCloseButtonVisible, value);
            }
        }

        public bool IsFilterMutedUser
        {
            get => _isFilterMutedUser;
            set
            {
                if (value == _isFilterMutedUser) return;
                SetProperty(ref _isFilterMutedUser, value);
            }
        }

        public bool IsFilterByVerification
        {
            get => _isFilterByVerification;
            set
            {
                if (value == _isFilterByVerification) return;
                SetProperty(ref _isFilterByVerification, value);
            }
        }

        public bool IsFilterActiveVerfiedUser
        {
            get => _isFilterActiveVerfiedUser;
            set
            {
                if (value == _isFilterActiveVerfiedUser) return;
                SetProperty(ref _isFilterActiveVerfiedUser, value);
            }
        }

        public bool IsFilterActiveWhoAreNotVerified
        {
            get => _isFilterActiveWhoAreNotVerified;
            set
            {
                if (value == _isFilterActiveWhoAreNotVerified) return;
                SetProperty(ref _isFilterActiveWhoAreNotVerified, value);
            }
        }
    }
}