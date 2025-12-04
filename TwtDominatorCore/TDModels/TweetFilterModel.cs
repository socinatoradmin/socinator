using System.Collections.Generic;
using DominatorHouseCore.Utility;
using TwtDominatorCore.Interface;

namespace TwtDominatorCore.TDModels
{
    public class TweetFilterModel : BindableBase, ITweetFilter
    {
        private RangeUtilities _filterTweetsHaveBetweenFavorites = new RangeUtilities {StartValue = 0, EndValue = 1};

        private RangeUtilities _filterTweetsHaveBetweenRetweets = new RangeUtilities {StartValue = 0, EndValue = 1};

        private int _filterTweetsLessThanDaysOldValue = 1;
        
        private int _filterTweetsLessThanHoursOldValue = 1;

        private bool _isFilterAlreadyLiked;

        private bool _isFilterAlreadyRetweeted;


        private bool _isFilterSkipRetweets;


        private bool _isFilterSkipTweetsContainingAtSign;


        private bool _isFilterSkipTweetsContainingSpecificWords;


        private bool _isFilterSkipTweetsContainNonEnglishChar;


        private bool _isFilterSkipTweetsWithLinks;


        private bool _isFilterSkipTweetsWithoutLinks;


        private bool _isFilterTweetsHaveBetweenFavorites;

        private bool _isFilterTweetsHaveBetweenRetweets;
        private bool _isFilterTweetsLessThanSpecificDaysOld;
        private bool _isFilterTweetsLessThanSpecificHoursOld;
        private bool _isFilterUseRealTimeResults;

        private List<string> _lstSkipTweetsContainingWords = new List<string>();

        private string _skipTweetsContainingWords;

        public bool IsFilterTweetsLessThanSpecificHoursOld
        {
            get => _isFilterTweetsLessThanSpecificHoursOld;
            set
            {
                if (value == _isFilterTweetsLessThanSpecificHoursOld)
                    return;
                IsFilterTweetsLessThanSpecificDaysOld = false;
                SetProperty(ref _isFilterTweetsLessThanSpecificHoursOld, value);
            }
        }

        public bool IsFilterTweetsLessThanSpecificDaysOld
        {
            get => _isFilterTweetsLessThanSpecificDaysOld;
            set
            {
                if (value == _isFilterTweetsLessThanSpecificDaysOld)
                    return;
                IsFilterTweetsLessThanSpecificHoursOld = false;
                SetProperty(ref _isFilterTweetsLessThanSpecificDaysOld, value);
            }
        }
        

        public bool IsFilterTweetsHaveBetweenRetweets
        {
            get => _isFilterTweetsHaveBetweenRetweets;
            set
            {
                if (value == _isFilterTweetsHaveBetweenRetweets)
                    return;
                SetProperty(ref _isFilterTweetsHaveBetweenRetweets, value);
            }
        }

        public bool IsFilterTweetsHaveBetweenFavorites
        {
            get => _isFilterTweetsHaveBetweenFavorites;
            set
            {
                if (value == _isFilterTweetsHaveBetweenFavorites)
                    return;
                SetProperty(ref _isFilterTweetsHaveBetweenFavorites, value);
            }
        }

        public bool IsFilterSkipTweetsContainingSpecificWords
        {
            get => _isFilterSkipTweetsContainingSpecificWords;
            set
            {
                if (value == _isFilterSkipTweetsContainingSpecificWords)
                    return;
                SetProperty(ref _isFilterSkipTweetsContainingSpecificWords, value);
            }
        }

        public bool IsFilterSkipTweetsContainingAtSign
        {
            get => _isFilterSkipTweetsContainingAtSign;
            set
            {
                if (value == _isFilterSkipTweetsContainingAtSign)
                    return;
                SetProperty(ref _isFilterSkipTweetsContainingAtSign, value);
            }
        }

        public bool IsFilterSkipRetweets
        {
            get => _isFilterSkipRetweets;
            set
            {
                if (value == _isFilterSkipRetweets)
                    return;
                SetProperty(ref _isFilterSkipRetweets, value);
            }
        }

        public bool IsFilterSkipTweetsWithLinks
        {
            get => _isFilterSkipTweetsWithLinks;
            set
            {
                if (value == _isFilterSkipTweetsWithLinks)
                    return;
                SetProperty(ref _isFilterSkipTweetsWithLinks, value);
            }
        }

        public bool IsFilterSkipTweetsWithoutLinks
        {
            get => _isFilterSkipTweetsWithoutLinks;
            set
            {
                if (value == _isFilterSkipTweetsWithoutLinks)
                    return;
                SetProperty(ref _isFilterSkipTweetsWithoutLinks, value);
            }
        }

        public bool IsFilterSkipTweetsContainNonEnglishChar
        {
            get => _isFilterSkipTweetsContainNonEnglishChar;
            set
            {
                if (value == _isFilterSkipTweetsContainNonEnglishChar)
                    return;
                SetProperty(ref _isFilterSkipTweetsContainNonEnglishChar, value);
            }
        }

        public bool IsFilterUseRealTimeResults
        {
            get => _isFilterUseRealTimeResults;
            set
            {
                if (value == _isFilterUseRealTimeResults)
                    return;
                SetProperty(ref _isFilterUseRealTimeResults, value);
            }
        }

        public bool IsFilterAlreadyLiked
        {
            get => _isFilterAlreadyLiked;
            set
            {
                if (value == _isFilterAlreadyLiked)
                    return;
                SetProperty(ref _isFilterAlreadyLiked, value);
            }
        }

        public bool IsFilterAlreadyRetweeted
        {
            get => _isFilterAlreadyRetweeted;
            set
            {
                if (value == _isFilterAlreadyRetweeted)
                    return;
                SetProperty(ref _isFilterAlreadyRetweeted, value);
            }
        }

        public int FilterTweetsLessThanSpecificDaysOldValue
        {
            get => _filterTweetsLessThanDaysOldValue;
            set
            {
                if (value == _filterTweetsLessThanDaysOldValue)
                    return;
                SetProperty(ref _filterTweetsLessThanDaysOldValue, value);
            }
        }

        public int FilterTweetsLessThanSpecificHoursOldValue
        {
            get => _filterTweetsLessThanHoursOldValue;
            set
            {
                if (value == _filterTweetsLessThanHoursOldValue)
                    return;
                SetProperty(ref _filterTweetsLessThanHoursOldValue, value);
            }
        }

        public RangeUtilities FilterTweetsHaveBetweenRetweets
        {
            get => _filterTweetsHaveBetweenRetweets;
            set
            {
                if (value == _filterTweetsHaveBetweenRetweets)
                    return;
                SetProperty(ref _filterTweetsHaveBetweenRetweets, value);
            }
        }

        public RangeUtilities FilterTweetsHaveBetweenFavorites
        {
            get => _filterTweetsHaveBetweenFavorites;
            set
            {
                if (value == _filterTweetsHaveBetweenFavorites)
                    return;
                SetProperty(ref _filterTweetsHaveBetweenFavorites, value);
            }
        }

        public string SkipTweetsContainingWords
        {
            get => _skipTweetsContainingWords;
            set
            {
                if (value == _skipTweetsContainingWords)
                    return;
                SetProperty(ref _skipTweetsContainingWords, value);
            }
        }

        public List<string> LstSkipTweetsContainingWords
        {
            get => _lstSkipTweetsContainingWords;
            set
            {
                if (value == _lstSkipTweetsContainingWords)
                    return;
                SetProperty(ref _lstSkipTweetsContainingWords, value);
            }
        }
    }
}