using DominatorHouseCore;
using System;
using System.ComponentModel;
using System.Linq;
using YoutubeDominatorCore.Interface;
using YoutubeDominatorCore.YDUtility;
using YoutubeDominatorCore.YoutubeModels;

namespace YoutubeDominatorCore.YoutubeLibrary.Processes
{
    internal static class ScrapeFilter
    {
        internal class FilterCommon
        {
            protected bool CheckContain(string CheckIn, string text)
            {
                var list = YdStatic.GetListSplittedWithNextLine(text);

                return list.Any(x => CheckIn.ToLower().Contains(x));
            }
        }

        #region VideoFilter

        [Localizable(false)]
        public class VideoFilterModel : FilterCommon
        {
            private readonly IVideoFilter VideoFilter;

            public VideoFilterModel(YdModuleSetting ModuleSetting)
            {
                VideoFilter = ModuleSetting.VideoFilterModel;
            }

            public bool FilterViewCount(YoutubePost ydPost)
            {
                return !YdStatic.IsBetween(VideoFilter.MinimumViewsCount, ulong.Parse(ydPost.ViewsCount),
                    VideoFilter.MaximumViewsCount);
            }

            public bool FilterCommentsCount(YoutubePost ydPost)
            {
                return !YdStatic.IsBetween(VideoFilter.MinimumCommentsCount, ulong.Parse(ydPost.CommentCount),
                    VideoFilter.MaximumCommentsCount);
            }

            public bool FilterLikeCount(YoutubePost ydPost)
            {
                return !YdStatic.IsBetween(VideoFilter.MinimumLikesCount, ulong.Parse(ydPost.LikeCount),
                    VideoFilter.MaximumLikesCount);
            }

            public bool FilterDislikeCount(YoutubePost ydPost)
            {
                return !YdStatic.IsBetween(VideoFilter.MinimumDislikesCount, ulong.Parse(ydPost.DislikeCount),
                    VideoFilter.MaximumDislikesCount);
            }

            public bool FilterVideoLength(YoutubePost ydPost)
            {
                return !YdStatic.IsBetween(Convert.ToUInt64(VideoFilter.MinimumVideoLengthInSeconds),
                    Convert.ToUInt64(ydPost.VideoLength), Convert.ToUInt64(VideoFilter.MaximumVideoLengthInSeconds));
            }

            public bool FilterTitleShouldContains(YoutubePost ydPost)
            {
                return !CheckContain(ydPost.Title, VideoFilter.TitleShouldContainsWordPhrase ?? "");
            }

            public bool FilterTitleShouldNotContains(YoutubePost ydPost)
            {
                return CheckContain(ydPost.Title, VideoFilter.TitleShouldNotContainsWordPhrase ?? "");
            }

            public bool FilterDescriptionShouldContains(YoutubePost ydPost)
            {
                return !CheckContain(ydPost.Caption, VideoFilter.DescriptionShouldContainsWordPhrase ?? "");
            }

            public bool FilterDescriptionShouldNotContains(YoutubePost ydPost)
            {
                return CheckContain(ydPost.Caption, VideoFilter.DescriptionShouldNotContainsWordPhrase ?? "");
            }

            public bool FilterShouldNotBePublishedBeforeDays(YoutubePost ydPost)
            {
                try
                {
                    var date = ydPost.PublishedDate.Replace("Premiered", "").Replace("Published on ", "")
                        .Replace(",", "").Trim();
                    if (date.ToLower().Contains("minute") || date.ToLower().Contains("sec") ||
                        date.ToLower().Contains("hour"))
                        return false;

                    var year = date.StringMatches(@"\d{4}")[0].ToString().Trim();
                    date = date.Replace(year, "").Trim();
                    var day = date.StringMatches(@"\d{2}")[0].ToString().Trim();
                    date = date.Replace(day, "").Trim();
                    var month = YdStatic.MonthNumber(date);

                    var dateTime = DateTime.Parse($"{month}/{day}/{year} 12:00:00 AM");
                    var days = (DateTime.Now - dateTime).Days;
                    if ((ulong)days > VideoFilter.PublishedOnDaysBefore)
                        return true;
                }
                catch (Exception ex)
                {
                    ex.DebugLog(ydPost.PublishedDate);
                    return false;
                }

                return false;
            }

            #endregion
        }

        [Localizable(false)]
        public class ChannelFilterModel : FilterCommon
        {
            private readonly IChannelFilter ChannelFilter;

            public ChannelFilterModel(YdModuleSetting ModuleSetting)
            {
                ChannelFilter = ModuleSetting.ChannelFilterModel;
            }

            #region Filters

            public bool FilterViewCount(YoutubeChannel ydChannel)
            {
                return !YdStatic.IsBetween(ChannelFilter.MinimumViewsCount, ulong.Parse(ydChannel.ViewsCount),
                    ChannelFilter.MaximumViewsCount);
            }

            public bool FilterSubscribeCount(YoutubeChannel ydChannel)
            {
                return !YdStatic.IsBetween(ChannelFilter.MinimumSubscribersCount,
                    ulong.Parse(ydChannel.SubscriberCount), ChannelFilter.MaximumSubscribersCount);
            }

            public bool FilterTitleShouldContains(YoutubeChannel ydChannel)
            {
                return !CheckContain(ydChannel.ChannelName, ChannelFilter.TitleShouldContainsWordPhrase ?? "");
            }

            public bool FilterTitleShouldNotContains(YoutubeChannel ydChannel)
            {
                return CheckContain(ydChannel.ChannelName, ChannelFilter.TitleShouldNotContainsWordPhrase ?? "");
            }

            public bool FilterDescriptionShouldContains(YoutubeChannel ydChannel)
            {
                return !CheckContain(ydChannel.ChannelDescription,
                    ChannelFilter.DescriptionShouldContainsWordPhrase ?? "");
            }

            public bool FilterDescriptionShouldNotContains(YoutubeChannel ydChannel)
            {
                return CheckContain(ydChannel.ChannelDescription,
                    ChannelFilter.DescriptionShouldNotContainsWordPhrase ?? "");
            }

            #endregion
        }
    }
}