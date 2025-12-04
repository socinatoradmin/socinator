namespace YoutubeDominatorCore.Interface
{
    public interface IChannelFilter
    {
        bool IsSubscribersCountChecked { get; set; }
        ulong MinimumSubscribersCount { get; set; }
        ulong MaximumSubscribersCount { get; set; }
        bool IsViewsCountChecked { get; set; }
        ulong MinimumViewsCount { get; set; }
        ulong MaximumViewsCount { get; set; }
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