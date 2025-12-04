using YoutubeDominatorCore.YoutubeModels;

namespace YoutubeDominatorCore.Interface
{
    public interface IVideoFilter
    {
        bool IsCheckedSearchVideoFilter { get; set; }
        SearchVideoFilterModel SearchVideoFilterModel { get; set; }
        bool IsViewsCountChecked { get; set; }
        ulong MinimumViewsCount { get; set; }

        ulong MaximumViewsCount { get; set; }
        bool IsCommentsCountChecked { get; set; }

        ulong MinimumCommentsCount { get; set; }

        ulong MaximumCommentsCount { get; set; }
        bool IsLikesCountChecked { get; set; }

        ulong MinimumLikesCount { get; set; }

        ulong MaximumLikesCount { get; set; }
        bool IsDislikesCountChecked { get; set; }
        ulong MinimumDislikesCount { get; set; }

        ulong MaximumDislikesCount { get; set; }

        bool IsSubscribersCountChecked { get; set; }

        ulong MinimumSubscribersCount { get; set; }

        ulong MaximumSubscribersCount { get; set; }
        bool IsVideoLengthInSecondsChecked { get; set; }

        int MinimumVideoLengthInSeconds { get; set; }

        int MaximumVideoLengthInSeconds { get; set; }
        bool IsIgnoreAdVideoChecked { get; set; }

        bool IsPublishedOnDaysBeforeChecked { get; set; }

        ulong PublishedOnDaysBefore { get; set; }

        bool IsTitleShouldContainsWordPhraseChecked { get; set; }
        string TitleShouldContainsWordPhrase { get; set; }
        bool IsTitleShouldNotContainsWordPhraseChecked { get; set; }
        string TitleShouldNotContainsWordPhrase { get; set; }
        bool IsDescriptionShouldContainsWordPhraseChecked { get; set; }
        string DescriptionShouldContainsWordPhrase { get; set; }
        bool IsDescriptionShouldNotContainsWordPhraseChecked { get; set; }
        string DescriptionShouldNotContainsWordPhrase { get; set; }
    }
}