using System.ComponentModel;

namespace YoutubeDominatorCore.YDEnums.VideoSearchFilterEnums
{
    public enum UploadDate
    {
        [Description("LangKeyNone")] None = 0,
        [Description("LangKeyLastHour")] LastHour = 1,
        [Description("LangKeyToday")] Today = 2,
        [Description("LangKeyThisWeek")] ThisWeek = 3,
        [Description("LangKeyThisMonth")] ThisMonth = 4,
        [Description("LangKeyThisYear")] ThisYear = 5
    }

    public enum Duration
    {
        [Description("LangKeyNone")] None = 0,

        [Description("LangKeyShortDurationYT")]
        Short = 1,
        [Description("LangKeyLongDurationYT")] Long = 2
    }

    public enum SortBy
    {
        [Description("LangKeyRelevance")] Relevance = 0,
        [Description("LangKeyUploadDate")] UploadDate = 1,
        [Description("LangKeyViewCount")] ViewCount = 2,
        [Description("LangKeyRating")] Rating = 3
    }
}