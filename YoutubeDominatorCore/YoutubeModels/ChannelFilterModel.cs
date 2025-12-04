using DominatorHouseCore.Utility;
using ProtoBuf;
using YoutubeDominatorCore.Interface;

namespace YoutubeDominatorCore.YoutubeModels
{
    [ProtoContract]
    public class ChannelFilterModel : BindableBase, IChannelFilter
    {
        #region IsSubscribersCountChecked

        private bool _isSubscribersCountChecked;

        [ProtoMember(1)]
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

        [ProtoMember(2)]
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

        [ProtoMember(3)]
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

        #region IsViewsCountChecked

        private bool _isViewsCountChecked;

        [ProtoMember(4)]
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

        [ProtoMember(5)]
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

        [ProtoMember(6)]
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

        #region IsIgnoreAdVideoChecked

        private bool _isIgnoreAdVideoChecked;

        [ProtoMember(7)]
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

        [ProtoMember(8)]
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

        private ulong _publishedOnDaysBefore;

        [ProtoMember(9)]
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

        [ProtoMember(10)]
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

        [ProtoMember(11)]
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

        [ProtoMember(12)]
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

        [ProtoMember(13)]
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

        [ProtoMember(14)]
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

        [ProtoMember(15)]
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

        [ProtoMember(16)]
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

        [ProtoMember(17)]
        public string DescriptionShouldNotContainsWordPhrase
        {
            get => _descriptionShouldNotContainsWordPhrase;
            set
            {
                if (value == _descriptionShouldNotContainsWordPhrase)
                    return;
                SetProperty(ref _descriptionShouldNotContainsWordPhrase, value);
            }
        }

        #endregion DescriptionShouldNotContainsWordPhrase
    }
}