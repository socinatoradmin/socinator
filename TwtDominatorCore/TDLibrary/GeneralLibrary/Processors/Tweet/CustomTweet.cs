using System;
using System.Collections.Generic;
using System.Linq;
using CommonServiceLocator;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using Newtonsoft.Json;
using TwtDominatorCore.Database;
using TwtDominatorCore.TDFactories;
using TwtDominatorCore.TDLibrary.GeneralLibrary.DAL;
using TwtDominatorCore.TDModels;
using DominatorHouseCore.Utility;

namespace TwtDominatorCore.TDLibrary.GeneralLibrary.Processors.Tweet
{
    internal class CustomTweet : BaseTwitterTweetsProcessor, IQueryProcessor
    {
        public CustomTweet(ITdJobProcess jobProcess, IBlackWhiteListHandler blackWhiteListHandler,
            IDbCampaignService campaignService, ITwitterFunctionFactory twitterFunctionFactory,
            IDelayService threadUtility, IDbInsertionHelper dbInsertionHelper)
            : base(jobProcess, blackWhiteListHandler, campaignService, twitterFunctionFactory.TwitterFunctions,
                threadUtility, dbInsertionHelper)
        {
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            jobProcessResult = new JobProcessResult();
            if (_jobProcess.checkJobCompleted())
                return;
            CheckUserFilterActiveForCurrentQuery(queryInfo);
            _jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            // checking already liked
            try
            {
                if (ActivityType == ActivityType.TweetScraper &&
                    (_dbAccountService.IsActivtyDoneWithThisTweetId(queryInfo.QueryValue, ActivityType) ||
                     !IsUniqueTweet(new TagDetails {Id = queryInfo.QueryValue})))
                    return;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            if (ActivityType.Equals(ActivityType.Comment))
                try
                {
                    var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
                    JsonConvert.DeserializeObject<CommentModel>(templatesFileManager.Get()
                        .FirstOrDefault(x => x.Id == _jobProcess.TemplateId).ActivitySettings);
                }
                catch (Exception ex)
                {
                    ex.ErrorLog();
                }

            var TweetInfo =
                TwitterFunction.GetSingleTweetDetails(_jobProcess.DominatorAccountModel, queryInfo.QueryValue);

            #region BlackList Filter

            var tagList = new List<TagDetails> {TweetInfo.TweetDetails};
            tagList = SkipBlackListOrWhiteList(tagList);

            #endregion

            if (TweetInfo.Success && tagList.Count > 0)
                FinalProcessForEachTag(queryInfo, out jobProcessResult, TweetInfo.TweetDetails);
        }
    }
}