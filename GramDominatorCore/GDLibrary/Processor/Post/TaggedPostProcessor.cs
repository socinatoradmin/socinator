using ThreadUtils;
using DominatorHouseCore.DatabaseHandler.GdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using GramDominatorCore.GDLibrary.DAL;
using GramDominatorCore.GDLibrary.InstagramBrowser;
using GramDominatorCore.GDModel;
using System;
using System.Collections.Generic;

namespace GramDominatorCore.GDLibrary.Processor.Post
{
    public class TaggedPostProcessor : BaseInstagramPostProcessor
    {
        private List<InteractedPosts> LstInteractedPosts { get; set; } = new List<InteractedPosts>();
        public TaggedPostProcessor(IGdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService, IDbCampaignService campaignService, IProcessScopeModel processScopeModel, IDelayService _delayService, IGdBrowserManager gdBrowserManager) : 
            base(jobProcess, dbAccountService, campaignService, processScopeModel, _delayService, gdBrowserManager)
        {
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            Token.ThrowIfCancellationRequested();
            List<InstagramPost> allFeedDetails = new List<InstagramPost>();
            try
            {
                var userResponse = InstaFunction.SearchUsername(DominatorAccountModel, queryInfo.QueryValue, Token);
                if (!CheckingLoginRequiredResponse(userResponse.ToString(), "", queryInfo))
                    return;
                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                {
                    var taggedPostResponse = InstaFunction.SomeoneTaggedPost(DominatorAccountModel, DominatorAccountModel.IsRunProcessThroughBrowser ? userResponse.Username : userResponse.Pk, Token, jobProcessResult.maxId);
                    if (!CheckingLoginRequiredResponse(taggedPostResponse.ToString(), "", queryInfo))
                        return;
                    if (taggedPostResponse.Success)
                    {
                        if (taggedPostResponse.Items.Count != 0)
                            allFeedDetails = taggedPostResponse.Items;
                        
                        CheckInteractedPostsData(LstInteractedPosts, allFeedDetails);
                        //allFeedDetails = FilterPostAge(allFeedDetails);
                        allFeedDetails = FilterAllImages(allFeedDetails);
                        if (ModuleSetting.PostFilterModel.FilterPostAge && !ModuleSetting.PostFilterModel.FilterBeforePostAge && allFeedDetails.Count == 0)
                            break;

                        foreach (InstagramPost eachPost in allFeedDetails)
                        {
                            if (ActivityType != ActivityType.CommentScraper && ActivityType != ActivityType.LikeComment && ActivityType != ActivityType.ReplyToComment)
                                FilterAndStartFinalProcessForOnePost(queryInfo, ref jobProcessResult, eachPost);
                            else
                                FilterAndStartFinalProcessForOneComment(queryInfo, ref jobProcessResult, eachPost);
                        }
                          

                        Token.ThrowIfCancellationRequested();
                        jobProcessResult.maxId = taggedPostResponse.MaxId;
                    }
                    else
                        jobProcessResult.maxId = null;
                    
                    CheckNoMoreDataForWithQuery(ref jobProcessResult);
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}
