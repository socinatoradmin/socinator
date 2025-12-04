using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDLibrary.DAL;
using GramDominatorCore.GDLibrary.InstagramBrowser;
using GramDominatorCore.Response;
using System;
using GramDominatorCore.Utility;

namespace GramDominatorCore.GDLibrary.Processor.User
{
    public class UsersWhoCommentedOnPostProcessor : BaseInstagramUserProcessor
    {
        public UsersWhoCommentedOnPostProcessor(IGdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService, IDbCampaignService campaignService, IProcessScopeModel processScopeModel, IDelayService delayService, IGdBrowserManager gdBrowserManager) :
            base(jobProcess, dbAccountService, campaignService, processScopeModel, delayService, gdBrowserManager)
        {
        }
        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                Token.ThrowIfCancellationRequested();
                QueryType = queryInfo.QueryType;  
                string idFromCode = CheckPostId(queryInfo);

                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                {
                    Token.ThrowIfCancellationRequested();
                    var mediaComments = 
                        GramStatic.IsBrowser ?
                        GdBrowserManager.GetMediaComments(DominatorAccountModel, queryInfo.QueryValue, Token)
                        : InstaFunction.GetMediaComments(DominatorAccountModel, !DominatorAccountModel.IsRunProcessThroughBrowser ? idFromCode : queryInfo.QueryValue.Trim(), Token, jobProcessResult.maxId);
                    if (!CheckingLoginRequiredResponse(mediaComments.ToString(), "", queryInfo))
                        return;
                    if (mediaComments.ToString().Contains("Media is unavailable"))
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, DominatorAccountModel.UserName, ActivityType, $"{queryInfo.QueryValue} Media is unavailable");
                        return;
                    }

                    if (mediaComments.Success)
                    {
                        var usersList = FilterWhitelistBlacklistUsers(mediaComments.UserList);
                        usersList = usersList == null ? mediaComments.UserList : usersList;
                        if (ActivityType == ActivityType.Follow)
                            usersList = GetUserInfoDetails(usersList ?? mediaComments.UserList);
                        if (ActivityType == ActivityType.UserScraper && ModuleSetting.IsTaggedPostUser)
                            GetTaggedUser(queryInfo, jobProcessResult, usersList ?? mediaComments.UserList);                       
                        else
                            jobProcessResult = FilterAndStartFinalProcess(queryInfo, jobProcessResult, usersList ?? mediaComments.UserList);
                        
                        jobProcessResult.maxId = mediaComments.MaxId;
                        if(string.IsNullOrEmpty(jobProcessResult.maxId))
                            CheckNoMoreDataForWithQuery(ref jobProcessResult);
                    }
                    else
                        jobProcessResult.maxId = null;  
                    
                    CheckNoMoreDataForWithQuery(ref jobProcessResult);
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (AggregateException e)
            {
                foreach (Exception ex in e.InnerExceptions)
                    Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}
