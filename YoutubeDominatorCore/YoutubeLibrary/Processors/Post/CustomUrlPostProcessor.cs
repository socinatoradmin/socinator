using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using System.Text.RegularExpressions;
using YoutubeDominatorCore.YDUtility;
using YoutubeDominatorCore.YoutubeLibrary.DAL;
using YoutubeDominatorCore.YoutubeLibrary.Processes;
using YoutubeDominatorCore.YoutubeLibrary.YdFunctions;
using YoutubeDominatorCore.YoutubeModels;

namespace YoutubeDominatorCore.YoutubeLibrary.Processors.Post
{
    internal class CustomUrlPostProcessor : BaseYoutubePostProcessor
    {
        public CustomUrlPostProcessor(IYdJobProcess jobProcess,
            IBlackWhiteListHandler blackWhiteListHandler, IDbCampaignService campaignService,
            IYoutubeFunctionality youtubeFunctionality) : base(jobProcess, blackWhiteListHandler,
            campaignService, youtubeFunctionality)
        {
        }

        protected override void Process(QueryInfo queryInfo)
        {
            var jobProcessResult = new JobProcessResult();
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            JobProcess.IsCustom = true;
            if (string.IsNullOrEmpty(queryInfo.QueryValue) ||
                !(queryInfo.QueryValue.Contains("youtube.com/watch?v=") ||
                queryInfo.QueryValue.Contains("youtube.com/shorts/") ||
                queryInfo.QueryValue.Contains("m.youtube.com/watch?v=") ||
                queryInfo.QueryValue.Contains("m.youtube.com/shorts/") ||
                queryInfo.QueryValue.Contains("youtu.be/")))
            {
                CustomLog($"Please provide a valid YouTube video URL (video or short) => ( {queryInfo.QueryValue} ).");
                return;
            }

            var url = PostUrlWithCommentId(queryInfo.QueryValue); //addidng (url+"Nothing")
            url = NormalizeYouTubeUrl(url);
            var videoId = GetYouTubeVideoId(url);
            if ((ActType == ActivityType.PostScraper || ActType == ActivityType.ViewIncreaser) &&
                AlreadyDoneInDB(queryInfo, new YoutubePost { PostUrl = $"https://www.youtube.com/watch?v={videoId}" },
                    true))
            {
                CustomLog($"Skipped(already interacted or filtred) the video ( {queryInfo.QueryValue} ).");
                return;
            }
            if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                BrowserManager.LoadPageSource(JobProcess.DominatorAccountModel, url, clearandNeedResource: true);
            var postInfo = JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser
                ? BrowserManager.PostInfo(ActType,url,
                    sortCommentsByNew: ActType == ActivityType.LikeComment || ActType == ActivityType.Comment || ActType == ActivityType.CommentScraper,
                    pauseVideo: ActType != ActivityType.ViewIncreaser)
                : YoutubeFunction.GetPostDetails(JobProcess.DominatorAccountModel, url, delayBeforeHit: 2.5);

            var reasonIfFailed = ReasonToFailedPostInfoFetch(postInfo);

            if (reasonIfFailed != null)
            {
                CustomLog($"Skipped to process video ({queryInfo.QueryValue}). Reason : {reasonIfFailed}");
            }
            else
            {
                SetProvidedCommentIdIntoPostInfo(queryInfo);
                StartCustomPostProcess(queryInfo, ref jobProcessResult, postInfo.YoutubePost, true);
            }
        }
        public string GetYouTubeVideoId(string url)
        {
            try
            {
                var match = Regex.Match(url, @"(?:v=)([a-zA-Z0-9_-]{11})");
                if (match.Success)
                {
                    return match.Groups[1].Value;
                }
                return Utilities.GetBetween(url, "/watch?v=", "&"); // or throw an exception if you prefer
            }
            catch { return Utilities.GetBetween(url, "/watch?v=", "&"); }
        }
        public string NormalizeYouTubeUrl(string url)
        {
            // Regex to match shorts links and capture the video ID
            try
            {
                var match = Regex.Match(url, @"youtube\.com/shorts/([a-zA-Z0-9_-]+)");
                if (match.Success)
                {
                    string videoId = match.Groups[1].Value;
                    return $"https://www.youtube.com/watch?v={videoId}";
                }
                return url;
            }
            catch { return url; }
        }
    }
}