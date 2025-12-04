using DominatorHouseCore.Utility;
using ProtoBuf;
using YoutubeDominatorCore.Interface;

namespace YoutubeDominatorCore.YoutubeModels
{
    [ProtoContract]
    public class VideoFilterModel : BindableBase, IVideoFilter
    {
        private bool _isCheckedSearchVideoFilter;

        private SearchVideoFilterModel _searchVideoFilterModel;

        [ProtoMember(30)]
        public bool IsCheckedSearchVideoFilter
        {
            get => _isCheckedSearchVideoFilter;
            set => SetProperty(ref _isCheckedSearchVideoFilter, value);
        }

        [ProtoMember(31)]
        public SearchVideoFilterModel SearchVideoFilterModel
        {
            get => _searchVideoFilterModel;
            set => SetProperty(ref _searchVideoFilterModel, value);
        }

        #region IsViewsCountChecked

        private bool _isViewsCountChecked;

        [ProtoMember(1)]
        public bool IsViewsCountChecked
        {
            get => _isViewsCountChecked;
            set
            {
                if (value == _isViewsCountChecked)
                    return;
                SetProperty(ref _isViewsCountChecked, value);
            }
        }

        #endregion IsViewsCountChecked

        #region MinimumViewsCount

        private ulong _minimumViewsCount = 10;

        [ProtoMember(2)]
        public ulong MinimumViewsCount
        {
            get => _minimumViewsCount;
            set
            {
                if (value == _minimumViewsCount)
                    return;
                SetProperty(ref _minimumViewsCount, value);
            }
        }

        #endregion MinimumViewsCount

        #region MaximumViewsCount

        private ulong _maximumViewsCount = 10;

        [ProtoMember(3)]
        public ulong MaximumViewsCount
        {
            get => _maximumViewsCount;
            set
            {
                if (value == _maximumViewsCount)
                    return;
                SetProperty(ref _maximumViewsCount, value);
            }
        }

        #endregion MaximumViewsCount

        #region IsCommentsCountChecked

        private bool _isCommentsCountChecked;

        [ProtoMember(4)]
        public bool IsCommentsCountChecked
        {
            get => _isCommentsCountChecked;
            set
            {
                if (value == _isCommentsCountChecked)
                    return;
                SetProperty(ref _isCommentsCountChecked, value);
            }
        }

        #endregion IsViewsCountChecked

        #region MinimumCommentsCount

        private ulong _minimumCommentsCount = 10;

        [ProtoMember(5)]
        public ulong MinimumCommentsCount
        {
            get => _minimumCommentsCount;
            set
            {
                if (value == _minimumCommentsCount)
                    return;
                SetProperty(ref _minimumCommentsCount, value);
            }
        }

        #endregion MinimumCommentsCount

        #region MaximumCommentsCount

        private ulong _maximumCommentsCount = 10;

        [ProtoMember(6)]
        public ulong MaximumCommentsCount
        {
            get => _maximumCommentsCount;
            set
            {
                if (value == _maximumCommentsCount)
                    return;
                SetProperty(ref _maximumCommentsCount, value);
            }
        }

        #endregion MaximumCommentsCount

        #region IsLikesCountChecked

        private bool _isLikesCountChecked;

        [ProtoMember(7)]
        public bool IsLikesCountChecked
        {
            get => _isLikesCountChecked;
            set
            {
                if (value == _isLikesCountChecked)
                    return;
                SetProperty(ref _isLikesCountChecked, value);
            }
        }

        #endregion IsViewsCountChecked

        #region MinimumLikesCount

        private ulong _minimumLikesCount = 10;

        [ProtoMember(8)]
        public ulong MinimumLikesCount
        {
            get => _minimumLikesCount;
            set
            {
                if (value == _minimumLikesCount)
                    return;
                SetProperty(ref _minimumLikesCount, value);
            }
        }

        #endregion MinimumLikesCount

        #region MaximumLikesCount

        private ulong _maximumLikesCount = 10;

        [ProtoMember(9)]
        public ulong MaximumLikesCount
        {
            get => _maximumLikesCount;
            set
            {
                if (value == _maximumLikesCount)
                    return;
                SetProperty(ref _maximumLikesCount, value);
            }
        }

        #endregion MaximumLikesCount

        #region IsDislikesCountChecked

        private bool _isDislikesCountChecked;

        [ProtoMember(10)]
        public bool IsDislikesCountChecked
        {
            get => _isDislikesCountChecked;
            set
            {
                if (value == _isDislikesCountChecked)
                    return;
                SetProperty(ref _isDislikesCountChecked, value);
            }
        }

        #endregion IsViewsCountChecked

        #region MinimumDislikesCount

        private ulong _minimumDislikesCount = 10;

        [ProtoMember(11)]
        public ulong MinimumDislikesCount
        {
            get => _minimumDislikesCount;
            set
            {
                if (value == _minimumDislikesCount)
                    return;
                SetProperty(ref _minimumDislikesCount, value);
            }
        }

        #endregion MinimumDislikesCount

        #region MaximumDislikesCount

        private ulong _maximumDislikesCount = 10;

        [ProtoMember(12)]
        public ulong MaximumDislikesCount
        {
            get => _maximumDislikesCount;
            set
            {
                if (value == _maximumDislikesCount)
                    return;
                SetProperty(ref _maximumDislikesCount, value);
            }
        }

        #endregion MaximumDislikesCount

        #region IsSubscribersCountChecked

        private bool _isSubscribersCountChecked;

        [ProtoMember(13)]
        public bool IsSubscribersCountChecked
        {
            get => _isSubscribersCountChecked;
            set
            {
                if (value == _isSubscribersCountChecked)
                    return;
                SetProperty(ref _isSubscribersCountChecked, value);
            }
        }

        #endregion IsSubscribersCountChecked

        #region MinimumSubscribersCount

        private ulong _minimumSubscribersCount = 10;

        [ProtoMember(14)]
        public ulong MinimumSubscribersCount
        {
            get => _minimumSubscribersCount;
            set
            {
                if (value == _minimumSubscribersCount)
                    return;
                SetProperty(ref _minimumSubscribersCount, value);
            }
        }

        #endregion MinimumSubscribersCount

        #region MaximumSubscribersCount

        private ulong _maximumSubscribersCount = 10;

        [ProtoMember(15)]
        public ulong MaximumSubscribersCount
        {
            get => _maximumSubscribersCount;
            set
            {
                if (value == _maximumSubscribersCount)
                    return;
                SetProperty(ref _maximumSubscribersCount, value);
            }
        }

        #endregion MaximumSubscribersCount

        #region IsVideoLengthInSecondsChecked

        private bool _isVideoLengthInSecondsChecked;

        [ProtoMember(16)]
        public bool IsVideoLengthInSecondsChecked
        {
            get => _isVideoLengthInSecondsChecked;
            set
            {
                if (value == _isVideoLengthInSecondsChecked)
                    return;
                SetProperty(ref _isVideoLengthInSecondsChecked, value);
            }
        }

        #endregion IsVideoLengthInSecondsChecked

        #region MinimumVideoLengthInSeconds

        private int _minimumVideoLengthInSeconds = 10;

        [ProtoMember(17)]
        public int MinimumVideoLengthInSeconds
        {
            get => _minimumVideoLengthInSeconds;
            set
            {
                if (value == _minimumVideoLengthInSeconds)
                    return;
                SetProperty(ref _minimumVideoLengthInSeconds, value);
            }
        }

        #endregion MinimumVideoLengthInSeconds

        #region MaximumVideoLengthInSeconds

        private int _maximumVideoLengthInSeconds = 10;

        [ProtoMember(18)]
        public int MaximumVideoLengthInSeconds
        {
            get => _maximumVideoLengthInSeconds;
            set
            {
                if (value == _maximumVideoLengthInSeconds)
                    return;
                SetProperty(ref _maximumVideoLengthInSeconds, value);
            }
        }

        #endregion MinimumVideoLengthInSeconds

        #region IsIgnoreAdVideoChecked

        private bool _isIgnoreAdVideoChecked;

        [ProtoMember(19)]
        public bool IsIgnoreAdVideoChecked
        {
            get => _isIgnoreAdVideoChecked;
            set
            {
                if (value == _isIgnoreAdVideoChecked)
                    return;
                SetProperty(ref _isIgnoreAdVideoChecked, value);
            }
        }

        #endregion IsIgnoreAdVideoChecked

        #region IsPublishedOnDaysBeforeChecked

        private bool _isPublishedOnDaysBeforeChecked;

        [ProtoMember(20)]
        public bool IsPublishedOnDaysBeforeChecked
        {
            get => _isPublishedOnDaysBeforeChecked;
            set
            {
                if (value == _isPublishedOnDaysBeforeChecked)
                    return;
                SetProperty(ref _isPublishedOnDaysBeforeChecked, value);
            }
        }

        #endregion IsPublishedOnDaysBeforeChecked

        #region PublishedOnDaysBefore

        private ulong _publishedOnDaysBefore = 90;

        /// <summary>
        ///     post Should not be published on days before
        /// </summary>
        [ProtoMember(21)]
        public ulong PublishedOnDaysBefore
        {
            get => _publishedOnDaysBefore;
            set
            {
                if (value == _publishedOnDaysBefore)
                    return;
                SetProperty(ref _publishedOnDaysBefore, value);
            }
        }

        #endregion PublishedOnDaysBefore

        #region IsTitleShouldContainsWordPhraseChecked

        private bool _isTitleShouldContainsWordPhraseChecked;

        [ProtoMember(22)]
        public bool IsTitleShouldContainsWordPhraseChecked
        {
            get => _isTitleShouldContainsWordPhraseChecked;
            set
            {
                if (value == _isTitleShouldContainsWordPhraseChecked)
                    return;
                SetProperty(ref _isTitleShouldContainsWordPhraseChecked, value);
            }
        }

        #endregion IsTitleShouldContainsWordPhraseChecked

        #region TitleShouldContainsWordPhrase

        private string _titleShouldContainsWordPhrase;

        [ProtoMember(23)]
        public string TitleShouldContainsWordPhrase
        {
            get => _titleShouldContainsWordPhrase;
            set
            {
                if (value == _titleShouldContainsWordPhrase)
                    return;
                SetProperty(ref _titleShouldContainsWordPhrase, value);
            }
        }

        #endregion TitleShouldContainsWordPhrase

        #region IsTitleShouldNotContainsWordPhraseChecked

        private bool _isTitleShouldNotContainsWordPhraseChecked;

        [ProtoMember(24)]
        public bool IsTitleShouldNotContainsWordPhraseChecked
        {
            get => _isTitleShouldNotContainsWordPhraseChecked;
            set
            {
                if (value == _isTitleShouldNotContainsWordPhraseChecked)
                    return;
                SetProperty(ref _isTitleShouldNotContainsWordPhraseChecked, value);
            }
        }

        #endregion IsTitleShouldNotContainsWordPhraseChecked

        #region TitleShouldNotContainsWordPhrase

        private string _titleShouldNotContainsWordPhrase;

        [ProtoMember(25)]
        public string TitleShouldNotContainsWordPhrase
        {
            get => _titleShouldNotContainsWordPhrase;
            set
            {
                if (value == _titleShouldNotContainsWordPhrase)
                    return;
                SetProperty(ref _titleShouldNotContainsWordPhrase, value);
            }
        }

        #endregion TitleShouldNotContainsWordPhrase

        #region IsDescriptionShouldContainsWordPhraseChecked

        private bool _isDescriptionShouldContainsWordPhraseChecked;

        [ProtoMember(26)]
        public bool IsDescriptionShouldContainsWordPhraseChecked
        {
            get => _isDescriptionShouldContainsWordPhraseChecked;
            set
            {
                if (value == _isDescriptionShouldContainsWordPhraseChecked)
                    return;
                SetProperty(ref _isDescriptionShouldContainsWordPhraseChecked, value);
            }
        }

        #endregion IsDescriptionShouldContainsWordPhraseChecked

        #region DescriptionShouldContainsWordPhrase

        private string _descriptionShouldContainsWordPhrase;

        [ProtoMember(27)]
        public string DescriptionShouldContainsWordPhrase
        {
            get => _descriptionShouldContainsWordPhrase;
            set
            {
                if (value == _descriptionShouldContainsWordPhrase)
                    return;
                SetProperty(ref _descriptionShouldContainsWordPhrase, value);
            }
        }

        #endregion DescriptionShouldContainsWordPhrase

        #region IsDescriptionShouldNotContainsWordPhraseChecked

        private bool _isDescriptionShouldNotContainsWordPhraseChecked;

        [ProtoMember(28)]
        public bool IsDescriptionShouldNotContainsWordPhraseChecked
        {
            get => _isDescriptionShouldNotContainsWordPhraseChecked;
            set
            {
                if (value == _isDescriptionShouldNotContainsWordPhraseChecked)
                    return;
                SetProperty(ref _isDescriptionShouldNotContainsWordPhraseChecked, value);
            }
        }

        #endregion IsDescriptionShouldContainsWordPhraseChecked

        #region DescriptionShouldNotContainsWordPhrase

        private string _descriptionShouldNotContainsWordPhrase;

        [ProtoMember(29)]
        public string DescriptionShouldNotContainsWordPhrase
        {
            get => _descriptionShouldNotContainsWordPhrase;
            set => SetProperty(ref _descriptionShouldNotContainsWordPhrase, value);
        }

        #endregion DescriptionShouldNotContainsWordPhrase
    }
}