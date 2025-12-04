using ThreadUtils;
using DominatorHouseCore.DatabaseHandler.GdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using GramDominatorCore.GDLibrary.DAL;
using GramDominatorCore.GDLibrary.InstagramBrowser;
using GramDominatorCore.GDModel;
using GramDominatorCore.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using GramDominatorCore.Utility;

namespace GramDominatorCore.GDLibrary.Processor.Post
{
    public class CustomPostProcessor : BaseInstagramPostProcessor
    {
        private List<InteractedPosts> LstInteractedPosts { get; set; } = new List<InteractedPosts>();
        public CustomPostProcessor(IGdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService, IDbCampaignService campaignService, IProcessScopeModel processScopeModel, IDelayService _delayService, IGdBrowserManager gdBrowserManager) 
            : base(jobProcess, dbAccountService, campaignService, processScopeModel, _delayService,gdBrowserManager)
        {
            CommentModel = JsonConvert.DeserializeObject<CommentModel>(TemplateModel.ActivitySettings);
        }    
        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                Token.ThrowIfCancellationRequested();
                var idFromCode = CheckPostId(queryInfo);
                var mediaInfo = 
                    GramStatic.IsBrowser ?
                    GdBrowserManager.MediaInfo(DominatorAccountModel, queryInfo.QueryValue.Trim(), Token)
                    : InstaFunction.MediaInfo(DominatorAccountModel, AccountModel, DominatorAccountModel.IsRunProcessThroughBrowser ? queryInfo.QueryValue.Trim() : idFromCode, Token).Result;
                if (CheckInteractedPostDbData(mediaInfo, LstInteractedPosts))
                {
                    if(queryInfo.QueryType == "Custom Photos")
                    {
                        jobProcessResult.IsProcessCompleted = jobProcessResult.HasNoResult = true;
                    }
                    return;
                }
                    

                int actCount = 0;
                SetQuantityIfMaxIdEmpty(jobProcessResult ?? (jobProcessResult = new JobProcessResult()), ++actCount);
                Token.ThrowIfCancellationRequested();
                if (ActivityType != ActivityType.CommentScraper && ActivityType != ActivityType.LikeComment && ActivityType != ActivityType.ReplyToComment)
                    FilterAndStartFinalProcessForOnePost(queryInfo, ref jobProcessResult, mediaInfo.InstagramPost);
                else
                    FilterAndStartFinalProcessForOneComment(queryInfo, ref jobProcessResult, mediaInfo.InstagramPost);
                
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
            catch (Exception )
            {
               // ex.DebugLog();
            }
            finally { jobProcessResult.IsProcessCompleted = true; }
        }
    }
}
