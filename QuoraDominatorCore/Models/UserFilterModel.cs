using System.Collections.Generic;
using DominatorHouseCore.Models;
using DominatorHouseCore.Settings;
using DominatorHouseCore.Utility;
using ProtoBuf;
using QuoraDominatorCore.Interface;

namespace QuoraDominatorCore.Models
{
    [ProtoContract]
    public class UserFilterModel : BindableBase, IUserFilter
    {
        // User filter by gender wise
        private GenderFilter _genderFilters = new GenderFilter();


        // Ignore the user who is followers for current accounts
        private bool _ignoreCurrentUsersFollowers;


        // Ignore the user who is not an english user
        private bool _ignoreNonEnglishUser;


        private BlacklistSettings _restrictedGroupList = new BlacklistSettings();


        private BlacklistSettings _restrictedProfileList = new BlacklistSettings();

        private bool _saveCloseButtonVisible;

        [ProtoMember(36)]
        public bool IgnoreCurrentUsersFollowers
        {
            get => _ignoreCurrentUsersFollowers;
            set
            {
                if (value == _ignoreCurrentUsersFollowers) return;
                SetProperty(ref _ignoreCurrentUsersFollowers, value);
            }
        }

        [ProtoMember(37)]
        public GenderFilter GenderFilters
        {
            get => _genderFilters;
            set
            {
                if (value == _genderFilters) return;
                SetProperty(ref _genderFilters, value);
            }
        }

        [ProtoMember(38)]
        public bool IgnoreNonEnglishUser
        {
            get => _ignoreNonEnglishUser;
            set
            {
                if (value == _ignoreNonEnglishUser) return;
                SetProperty(ref _ignoreNonEnglishUser, value);
            }
        }

        [ProtoMember(39)]
        public BlacklistSettings RestrictedGrouplist
        {
            get => _restrictedGroupList;
            set
            {
                if (value == _restrictedGroupList) return;
                SetProperty(ref _restrictedGroupList, value);
            }
        }

        [ProtoMember(40)]
        public BlacklistSettings RestrictedProfileList
        {
            get => _restrictedProfileList;
            set
            {
                if (value == _restrictedProfileList) return;
                SetProperty(ref _restrictedProfileList, value);
            }
        }

        [ProtoMember(41)]
        public bool SaveCloseButtonVisible
        {
            get => _saveCloseButtonVisible;
            set
            {
                if (value == _saveCloseButtonVisible) return;
                SetProperty(ref _saveCloseButtonVisible, value);
            }
        }

        #region To filter users with no profile picture

        private bool _ignoreNoProfilePicUsers;

        [ProtoMember(1)]
        public bool IgnoreNoProfilePicUsers
        {
            get => _ignoreNoProfilePicUsers;
            set
            {
                if (value == _ignoreNoProfilePicUsers) return;
                SetProperty(ref _ignoreNoProfilePicUsers, value);
            }
        }

        #endregion

        #region Filter Users By Answers Count

        private bool _filterAnswersCount;

        [ProtoMember(2)]
        public bool FilterAnswersCount
        {
            get => _filterAnswersCount;
            set
            {
                if (value == _filterAnswersCount) return;
                SetProperty(ref _filterAnswersCount, value);
            }
        }

        private RangeUtilities _answersCount = new RangeUtilities(0, 1000);

        [ProtoMember(3)]
        public RangeUtilities AnswersCount
        {
            get => _answersCount;
            set
            {
                if (value == _answersCount) return;
                SetProperty(ref _answersCount, value);
            }
        }

        #endregion

        #region Filter Users By Questions Count

        private bool _filterQuestionsCount;

        [ProtoMember(4)]
        public bool FilterQuestionsCount
        {
            get => _filterQuestionsCount;
            set
            {
                if (value == _filterQuestionsCount) return;
                SetProperty(ref _filterQuestionsCount, value);
            }
        }

        private RangeUtilities _questionsCount = new RangeUtilities(0, 1000);

        [ProtoMember(5)]
        public RangeUtilities QuestionsCount
        {
            get => _questionsCount;
            set
            {
                if (value == _questionsCount) return;
                SetProperty(ref _questionsCount, value);
            }
        }

        #endregion


        // Filter the user by whose post count lies between specified range

        #region Filter Users By Posts Count

        private bool _filterPostsCounts;

        [ProtoMember(6)]
        public bool FilterPostsCounts
        {
            get => _filterPostsCounts;
            set
            {
                if (value == _filterPostsCounts) return;
                SetProperty(ref _filterPostsCounts, value);
            }
        }

        // User by whose post count lies between specified range
        private RangeUtilities _postCounts = new RangeUtilities(0, 1000);

        [ProtoMember(7)]
        public RangeUtilities PostCounts
        {
            get => _postCounts;
            set
            {
                if (value == _postCounts) return;
                SetProperty(ref _postCounts, value);
            }
        }

        #endregion

        #region Filter Users By Blogs Count

        private bool _filterBlogsCount;

        [ProtoMember(8)]
        public bool FilterBlogsCounts
        {
            get => _filterBlogsCount;
            set
            {
                if (value == _filterBlogsCount) return;
                SetProperty(ref _filterBlogsCount, value);
            }
        }

        // User by whose blogs count lies between specified range
        private RangeUtilities _blogsCounts = new RangeUtilities(0, 1000);

        [ProtoMember(9)]
        public RangeUtilities BlogsCounts
        {
            get => _blogsCounts;
            set
            {
                if (value == _blogsCounts) return;
                SetProperty(ref _blogsCounts, value);
            }
        }

        #endregion

        #region Filter Users By Topics Count

        private bool _filterTopicsCount;

        [ProtoMember(10)]
        public bool FilterTopicsCounts
        {
            get => _filterTopicsCount;
            set
            {
                if (value == _filterTopicsCount) return;
                SetProperty(ref _filterTopicsCount, value);
            }
        }

        // User by whose topics count lies between specified range
        private RangeUtilities _topicsCount = new RangeUtilities(0, 1000);

        [ProtoMember(11)]
        public RangeUtilities TopicsCounts
        {
            get => _topicsCount;
            set
            {
                if (value == _topicsCount) return;
                SetProperty(ref _topicsCount, value);
            }
        }

        #endregion

        #region Filter Users By Edits Count

        private bool _filterEditsCount;

        [ProtoMember(12)]
        public bool FilterEditsCounts
        {
            get => _filterEditsCount;
            set
            {
                if (value == _filterEditsCount) return;
                SetProperty(ref _filterEditsCount, value);
            }
        }

        // User by whose edit count lies between specified range
        private RangeUtilities _editsCounts = new RangeUtilities(0, 1000);

        [ProtoMember(13)]
        public RangeUtilities EditsCounts
        {
            get => _editsCounts;
            set
            {
                if (value == _editsCounts) return;
                SetProperty(ref _editsCounts, value);
            }
        }

        #endregion

        #region Filter Users By Answer Views Count

        private bool _filterAnswerViewsCount;

        [ProtoMember(14)]
        public bool FilterAnswerViewsCounts
        {
            get => _filterAnswerViewsCount;
            set
            {
                if (value == _filterAnswerViewsCount) return;
                SetProperty(ref _filterAnswerViewsCount, value);
            }
        }

        // User by whose answer views count lies between specified range
        private RangeUtilities _answerViewsCount = new RangeUtilities(0, 1000);

        [ProtoMember(15)]
        public RangeUtilities AnswerViewsCounts
        {
            get => _answerViewsCount;
            set
            {
                if (value == _answerViewsCount) return;
                SetProperty(ref _answerViewsCount, value);
            }
        }

        #endregion

        #region Filter Users Which Are Have Not Lived In Mentioned Places

        // Is Filter by the user who should not contain restiricted words(BlackListWords)
        private bool _filterBlacklistedLivesIn;

        [ProtoMember(16)]
        public bool FilterBlacklistedLivesIn
        {
            get => _filterBlacklistedLivesIn;
            set
            {
                if (value == _filterBlacklistedLivesIn) return;
                SetProperty(ref _filterBlacklistedLivesIn, value);
            }
        }


        // Collection of words for Filter by the user who should not contain restiricted words(BlackListWords)
        private List<string> _blacklistedLivesInPlaces = new List<string>();

        [ProtoMember(17)]
        public List<string> BlacklistedLivesInPlaces
        {
            get => _blacklistedLivesInPlaces;
            set
            {
                if (value == _blacklistedLivesInPlaces) return;
                SetProperty(ref _blacklistedLivesInPlaces, value);
            }
        }

        #endregion

        #region Filter Users Which Are Have Not Studied In Mentioned Places

        // Is Filter by the user who should not contain restiricted words(BlackListWords)
        private bool _filterBlacklistedStudiedPlaces;

        [ProtoMember(18)]
        public bool FilterBlacklistedStudiedPlaces
        {
            get => _filterBlacklistedStudiedPlaces;
            set
            {
                if (value == _filterBlacklistedStudiedPlaces) return;
                SetProperty(ref _filterBlacklistedStudiedPlaces, value);
            }
        }


        // Collection of words for Filter by the user who should not contain restiricted words(BlackListWords)
        private List<string> _blacklistedStudiedPlaces = new List<string>();

        [ProtoMember(19)]
        public List<string> BlacklistedStudiedPlaces
        {
            get => _blacklistedStudiedPlaces;
            set
            {
                if (value == _blacklistedStudiedPlaces) return;
                SetProperty(ref _blacklistedStudiedPlaces, value);
            }
        }

        #endregion

        #region Filter Users Who has not worked In Mentioned Places

        // Is Filter by the user who should not contain restiricted words(BlackListWords)
        private bool _filterBlacklistedWorkPlaces;

        [ProtoMember(20)]
        public bool FilterBlacklistedWorkPlaces
        {
            get => _filterBlacklistedWorkPlaces;
            set
            {
                if (value == _filterBlacklistedWorkPlaces) return;
                SetProperty(ref _filterBlacklistedWorkPlaces, value);
            }
        }


        // Collection of words for Filter by the user who should not contain restiricted words(BlackListWords)
        private List<string> _blacklistedWorkPlaces = new List<string>();

        [ProtoMember(21)]
        public List<string> BlacklistedWorkPlaces
        {
            get => _blacklistedWorkPlaces;
            set
            {
                if (value == _blacklistedWorkPlaces) return;
                SetProperty(ref _blacklistedWorkPlaces, value);
            }
        }

        #endregion

        #region Filter By Minimum Characters in Bio Description

        private bool _filterMinimumCharacterInBio;

        [ProtoMember(22)]
        public bool FilterMinimumCharacterInBio
        {
            get => _filterMinimumCharacterInBio;
            set
            {
                if (value == _filterMinimumCharacterInBio) return;
                SetProperty(ref _filterMinimumCharacterInBio, value);
            }
        }

        // In User Bio graphy should contain minimum count of character
        private int _minimumCharacterInBio = 100;

        [ProtoMember(23)]
        public int MinimumCharacterInBio
        {
            get => _minimumCharacterInBio;
            set
            {
                if (value == _minimumCharacterInBio) return;
                SetProperty(ref _minimumCharacterInBio, value);
            }
        }

        #endregion


        // Is Filter by the user who should contains minimum to maximum specific following counts

        #region Filter By Following Count

        private bool _filterFollowingsCount;

        [ProtoMember(24)]
        public bool FilterFollowingsCount
        {
            get => _filterFollowingsCount;
            set
            {
                if (value == _filterFollowingsCount) return;
                SetProperty(ref _filterFollowingsCount, value);
            }
        }


        //User following counts should be lies between specific range values   
        private RangeUtilities _followingsCount = new RangeUtilities(0, 1000);

        [ProtoMember(25)]
        public RangeUtilities FollowingsCount
        {
            get => _followingsCount;
            set
            {
                if (value == _followingsCount) return;
                SetProperty(ref _followingsCount, value);
            }
        }

        #endregion


        // Is Filter by the user who should contains minimum to maximum specific followers counts

        #region Filter By Followers Count

        private bool _filterFollowersCount;

        [ProtoMember(26)]
        public bool FilterFollowersCount
        {
            get => _filterFollowersCount;
            set
            {
                if (value == _filterFollowersCount) return;
                SetProperty(ref _filterFollowersCount, value);
            }
        }


        //User followers counts should be lies between specific range values   
        private RangeUtilities _followersCount = new RangeUtilities(0, 1000);

        [ProtoMember(27)]
        public RangeUtilities FollowersCount
        {
            get => _followersCount;
            set
            {
                if (value == _followersCount) return;
                SetProperty(ref _followersCount, value);
            }
        }

        #endregion


        #region Filter Users Containing Blacklisted Words In Bio Description

        // Is Filter by the user who should not contain restiricted words(BlackListWords)
        private bool _filterBioRestrictedWords;

        [ProtoMember(28)]
        public bool FilterBioRestrictedWords
        {
            get => _filterBioRestrictedWords;
            set
            {
                if (value == _filterBioRestrictedWords) return;
                SetProperty(ref _filterBioRestrictedWords, value);
            }
        }


        // Collection of words for Filter by the user who should not contain restiricted words(BlackListWords)
        private List<string> _bioRestrictedWords = new List<string>();

        [ProtoMember(29)]
        public List<string> BioRestrictedWords
        {
            get => _bioRestrictedWords;
            set
            {
                if (value == _bioRestrictedWords) return;
                SetProperty(ref _bioRestrictedWords, value);
            }
        }

        #endregion

        #region Filter Users Containing Blacklisted Words In Username

        private bool _filterUserHasInvalidWord;

        [ProtoMember(30)]
        public bool FilterUserHasInvalidWord
        {
            get => _filterUserHasInvalidWord;
            set
            {
                if (value == _filterUserHasInvalidWord) return;
                SetProperty(ref _filterUserHasInvalidWord, value);
            }
        }

        private List<string> _userNameRestrictedWords = new List<string>();

        [ProtoMember(31)]
        public List<string> UserNameRestrictedWords
        {
            get => _userNameRestrictedWords;
            set
            {
                if (value == _userNameRestrictedWords) return;
                SetProperty(ref _userNameRestrictedWords, value);
            }
        }

        #endregion


        #region Filter with Follow Ratio Range

        // Filter the user from their (Follower/Following) ratio should exactly equal to specified ratio range
        private bool _filterSpecificFollowRatio;

        [ProtoMember(32)]
        public bool FilterSpecificFollowRatio
        {
            get => _filterSpecificFollowRatio;
            set
            {
                if (value == _filterSpecificFollowRatio) return;
                SetProperty(ref _filterSpecificFollowRatio, value);
            }
        }


        // User from their (Follower/Following) ratio should exactly equal to specified  ratio range
        private RangeUtilities _specificFollowRatio = new RangeUtilities(0, 1000);

        [ProtoMember(33)]
        public RangeUtilities SpecificFollowRatio
        {
            get => _specificFollowRatio;
            set
            {
                if (value == _specificFollowRatio) return;
                SetProperty(ref _specificFollowRatio, value);
            }
        }

        #endregion


        #region To Filter users who has not answered in recent x-y days

        // Is Filter by user who Answered on recent days
        private bool _filterAnsweredInRecentDay;

        [ProtoMember(34)]
        public bool FilterAnsweredInRecentDays
        {
            get => _filterAnsweredInRecentDay;
            set
            {
                if (value == _filterAnsweredInRecentDay) return;
                SetProperty(ref _filterAnsweredInRecentDay, value);
            }
        }


        // User who should recently posted on specific days 
        private RangeUtilities _answeredInRecentDays = new RangeUtilities(0, 1000);

        [ProtoMember(35)]
        public RangeUtilities AnsweredInRecentDays
        {
            get => _answeredInRecentDays;
            set
            {
                if (value == _answeredInRecentDays) return;
                SetProperty(ref _answeredInRecentDays, value);
            }
        }

        #endregion
    }
}