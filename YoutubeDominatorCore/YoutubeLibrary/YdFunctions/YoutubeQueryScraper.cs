using DominatorHouseCore.BusinessLogic.Scraper;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.YdQuery;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using Unity;
using YoutubeDominatorCore.YDEnums;
using YoutubeDominatorCore.YDUtility;
using YoutubeDominatorCore.YoutubeLibrary.Processes;
using YoutubeDominatorCore.YoutubeLibrary.Processors;
using YoutubeDominatorCore.YoutubeLibrary.Processors.Channel;
using YoutubeDominatorCore.YoutubeLibrary.Processors.Post;

namespace YoutubeDominatorCore.YoutubeLibrary.YdFunctions
{
    public class YoutubeQueryScraper : QueryScraper
    {
        public YoutubeQueryScraper(IYdJobProcess jobProcess,
            Dictionary<string, Action<QueryInfo>> scrapeWithQueriesActionTable,
            Dictionary<string, Action> scrapeWithoutQueriesActionTable)
            : base(jobProcess, scrapeWithQueriesActionTable, scrapeWithoutQueriesActionTable)
        {
        }
    }

    public interface IYoutubeScraperActionTables : IScraperActionTables
    {
    }

    public class YoutubeScraperActionTables : IYoutubeScraperActionTables
    {
        private readonly IYdJobProcess _jobProcess;
        private readonly IUnityContainer _unityContainer;

        public YoutubeScraperActionTables(IYdJobProcess jobProcess, IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
            _jobProcess = jobProcess;
            var activityType = _jobProcess.ActivityType;
            var scrapeWith = activityType.GetYdElementByActivityType();

            ScrapeWithQueriesActionTable = new Dictionary<string, Action<QueryInfo>>();

            switch (scrapeWith)
            {
                case YdElements.Posts:
                    ScrapeWithQueriesActionTable.Add(
                        $"{activityType}{EnumUtility.GetQueryFromEnum(YdScraperParameters.Keywords)}",
                        StartProcessToPerformPostQueriesForKeyword);
                    ScrapeWithQueriesActionTable.Add(
                        $"{activityType}{EnumUtility.GetQueryFromEnum(YdScraperParameters.CustomUrls)}",
                        StartProcessToPerformPostQueriesForCustomUrls);
                    ScrapeWithQueriesActionTable.Add(
                        $"{activityType}{EnumUtility.GetQueryFromEnum(YdScraperParameters.CustomChannel)}",
                        StartProcessToPerformPostQueriesForCustomChannel);
                    break;

                case YdElements.Channel:
                    ScrapeWithQueriesActionTable.Add(
                        $"{activityType}{EnumUtility.GetQueryFromEnum(YdScraperParameters.Keywords)}",
                        StartProcessToPerformChannelQueriesForKeyword);
                    ScrapeWithQueriesActionTable.Add(
                        $"{activityType}{EnumUtility.GetQueryFromEnum(YdScraperParameters.CustomUrls)}",
                        StartProcessToPerformChannelQueriesForCustomUrls);
                    ScrapeWithQueriesActionTable.Add(
                        $"{activityType}{EnumUtility.GetQueryFromEnum(YdScraperParameters.YTVideoCommenters)}",
                        StartProcessToPerformChannelQueriesForYTVideoCommenters);
                    break;
            }

            ScrapeWithoutQueriesActionTable = new Dictionary<string, Action>
                {{activityType.ToString(), StartProcessToUnSubscribe}};
        }

        public Dictionary<string, Action> ScrapeWithoutQueriesActionTable { get; set; }
        public Dictionary<string, Action<QueryInfo>> ScrapeWithQueriesActionTable { get; set; }

        #region Start Process For Without Queries

        private void StartProcessToUnSubscribe()
        {
            IQueryProcessor processor = _unityContainer.Resolve<UnsubscribeProcessor>();
            processor.Start(QueryInfo.NoQuery);
        }

        #endregion

        #region Global properties

        #endregion

        #region Start Process For Post Queries

        private void StartProcessToPerformPostQueriesForKeyword(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<KeywordPostProcessor>();
            processor.Start(queryInfo);
        }

        private void StartProcessToPerformPostQueriesForCustomUrls(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<CustomUrlPostProcessor>();
            processor.Start(queryInfo);
            if (!(_jobProcess.ActivityType == ActivityType.Comment ||
                  _jobProcess.ActivityType == ActivityType.LikeComment ||
                  _jobProcess.ActivityType == ActivityType.CommentScraper ||
                  _jobProcess.ActivityType == ActivityType.ViewIncreaser))
                _jobProcess.DominatorAccountModel.IsNeedToSchedule = false;
        }

        private void StartProcessToPerformChannelQueriesForYTVideoCommenters(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<YTVideoCommentersChannelProcessor>();
            processor.Start(queryInfo);
        }

        private void StartProcessToPerformPostQueriesForCustomChannel(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<CustomChannelPostScraper>();
            processor.Start(queryInfo);
        }

        #endregion

        #region Start Process For Channel Queries

        private void StartProcessToPerformChannelQueriesForKeyword(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<KeywordChannelProcessor>();
            processor.Start(queryInfo);
        }

        private void StartProcessToPerformChannelQueriesForCustomUrls(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<CustomUrlChannelProcessor>();
            processor.Start(queryInfo);
            _jobProcess.DominatorAccountModel.IsNeedToSchedule = false;
        }

        #endregion
    }
}