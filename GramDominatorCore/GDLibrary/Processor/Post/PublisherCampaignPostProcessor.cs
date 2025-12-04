using ThreadUtils;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using GramDominatorCore.GDLibrary.DAL;
using GramDominatorCore.GDLibrary.InstagramBrowser;
using GramDominatorCore.GDUtility;
using GramDominatorCore.Response;
using System;
using GramDominatorCore.Utility;

namespace GramDominatorCore.GDLibrary.Processor.Post
{
    public class PublisherCampaignPostProcessor : BaseInstagramPostProcessor
    {
        public PublisherCampaignPostProcessor(IGdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService, IDbCampaignService campaignService, IProcessScopeModel processScopeModel, IDelayService _delayService, IGdBrowserManager gdBrowserManager)
            : base(jobProcess, dbAccountService, campaignService, processScopeModel, _delayService, gdBrowserManager)
        {
        }
        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                Token.ThrowIfCancellationRequested();

                var details = PublisherInitialize.GetNetworksPublishedPost(queryInfo.QueryValue, SocialNetworks.Instagram);
                var IsBrowser = DominatorAccountModel.IsRunProcessThroughBrowser;
                var browser = GramStatic.IsBrowser;
                foreach (var publishedPost in details)
                {
                    Token.ThrowIfCancellationRequested();
                    jobProcessResult = new JobProcessResult();
                    var mediaId = publishedPost.Link.GetCodeFromUrl();
                    var mediaInfo =
                        browser ?
                        InstaFunction.GdBrowserManager.MediaInfo(DominatorAccountModel, publishedPost.Link, Token)
                        : InstaFunction.MediaInfo(DominatorAccountModel, AccountModel, mediaId, Token).Result;
                    if (ActivityType == ActivityType.Like || ActivityType == ActivityType.Comment)
                        FilterAndStartFinalProcessForOnePost(queryInfo,ref jobProcessResult, mediaInfo.InstagramPost);
                    else
                        FilterAndStartFinalProcessForOneComment(queryInfo, ref jobProcessResult, mediaInfo.InstagramPost);
                }

                DelayForScraperActivity();
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (AggregateException e)
            {
                foreach (Exception ex in e.InnerExceptions)
                    Console.WriteLine(ex.Message);
            }
            catch (Exception)
            {
                //  ex.DebugLog();
            }
            finally
            {
                jobProcessResult.IsProcessCompleted = true;
            }
        }

    }
}
