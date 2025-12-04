using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using GramDominatorCore.GDLibrary.DAL;
using GramDominatorCore.GDLibrary.InstagramBrowser;
using GramDominatorCore.GDModel;
using GramDominatorCore.Utility;
using Newtonsoft.Json;
using System;
using ThreadUtils;

namespace GramDominatorCore.GDLibrary.Processor.User
{
    public class SuggestedUsersProcessor : BaseInstagramUserProcessor
    {
        public BroadcastMessagesModel BroadcastMessagesModel { get; set; }          
        public SuggestedUsersProcessor(IGdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService, IDbCampaignService campaignService, IProcessScopeModel processScopeModel, IDelayService _delayService, IGdBrowserManager gdBrowserManager) : 
            base(jobProcess, dbAccountService, campaignService ,processScopeModel,_delayService,gdBrowserManager)
        {
            BroadcastMessagesModel = JsonConvert.DeserializeObject<BroadcastMessagesModel>(TemplateModel.ActivitySettings);
        }
        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                Token.ThrowIfCancellationRequested();
                var suggestedUsers = 
                    GramStatic.IsBrowser ?
                    GdBrowserManager.GetSuggestedUsers(DominatorAccountModel, Token) 
                    :InstaFunction.GetSuggestedUsers(DominatorAccountModel, AccountModel,string.Empty, Token, jobProcessResult.maxId).Result;
                jobProcessResult.maxId = suggestedUsers?.MaxID;
                jobProcessResult.HasNoResult = !suggestedUsers?.HasMoreResult ?? false;
                if (suggestedUsers.Success)
                {
                    #region Process for "SuggestedUsers" query parameter
                    var usersList = FilterWhitelistBlacklistUsers(suggestedUsers.UsersList);
                    GetInteractedUserAccrossAllFor(usersList ?? suggestedUsers.UsersList);
                    GetInteractedCampaignUser(usersList ?? suggestedUsers.UsersList);
                    CheckUserInDatabase(usersList ?? suggestedUsers.UsersList);
                    if (ActivityType == ActivityType.Follow)
                        usersList = GetUserInfoDetails(usersList ?? suggestedUsers.UsersList);
                    if (ModuleSetting.IsTaggedPostUser && ActivityType == ActivityType.UserScraper)
                        GetTaggedUser(queryInfo, jobProcessResult, usersList ?? suggestedUsers.UsersList);
                    else
                        FilterAndStartFinalProcess(queryInfo, jobProcessResult, usersList ?? suggestedUsers.UsersList);
                    #endregion
                }

                #region OLD Code for suggest user
                //if (DominatorAccountModel.IsRunProcessThroughBrowser)
                //{
                //    SuggestedUsersIgResponseHandler suggestedUsers = GdBrowserManager.GetSuggestedUsers(DominatorAccountModel,Token);
                //    if (!suggestedUsers.Success)
                //    {
                //        CommonIgResponseHandler getTimelineData = GdBrowserManager.GetFeedTimeLineData(DominatorAccountModel, Token);
                //    }
                //    if (suggestedUsers.Success)
                //    {
                //        #region Process for "SuggestedUsers" query parameter
                //        var usersList = FilterWhitelistBlacklistUsers(suggestedUsers.UsersList);
                //        GetInteractedUserAccrossAllFor(usersList ?? suggestedUsers.UsersList);
                //        GetInteractedCampaignUser(usersList ?? suggestedUsers.UsersList);
                //        CheckUserInDatabase(usersList ?? suggestedUsers.UsersList);
                //        if (ActivityType == ActivityType.Follow)
                //            usersList=GetUserInfoDetails(usersList ?? suggestedUsers.UsersList);
                //        if (ModuleSetting.IsTaggedPostUser && ActivityType == ActivityType.UserScraper)
                //            GetTaggedUser(queryInfo, jobProcessResult, usersList ?? suggestedUsers.UsersList);
                //        else
                //            FilterAndStartFinalProcess(queryInfo, jobProcessResult, usersList ?? suggestedUsers.UsersList);
                //        #endregion
                //    }
                //    else
                //    {
                //        GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                //        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                //        "No Suggested Users Found");
                //        jobProcessResult.HasNoResult = true;
                //        return;
                //    }
                //}
                //else
                //{
                //    if (CheckQueryValueOnMessageList(BroadcastMessagesModel, queryInfo)) return;
                //    QueryType = queryInfo.QueryType;
                //    UsernameInfoIgResponseHandler usernameInfo = InstaFunction.SearchUsername(DominatorAccountModel, queryInfo.QueryValue, Token);
                //    if (!CheckingLoginRequiredResponse(usernameInfo.ToString(), "", queryInfo))
                //        return;
                //    if (usernameInfo.Success)
                //    {
                //        SuggestedUsersIgResponseHandler suggestedUsers = InstaFunction.GetSuggestedUsers(DominatorAccountModel, AccountModel, usernameInfo.Pk, Token, jobProcessResult.maxId).Result;
                //        if (!CheckingLoginRequiredResponse(suggestedUsers.ToString(), "", queryInfo))
                //            return;
                //        if (suggestedUsers.Success)
                //        {
                //            #region Process for "SuggestedUsers" query parameter
                //            var usersList = FilterWhitelistBlacklistUsers(suggestedUsers.UsersList);
                //            GetInteractedUserAccrossAllFor(usersList ?? suggestedUsers.UsersList);
                //            GetInteractedCampaignUser(usersList ?? suggestedUsers.UsersList);
                //            CheckUserInDatabase(usersList ?? suggestedUsers.UsersList);
                //            if (ActivityType == ActivityType.Follow)
                //                usersList = GetUserInfoDetails(usersList ?? suggestedUsers.UsersList);
                //            if (ModuleSetting.IsTaggedPostUser && ActivityType == ActivityType.UserScraper)
                //                GetTaggedUser(queryInfo, jobProcessResult, usersList ?? suggestedUsers.UsersList);
                //            else
                //                FilterAndStartFinalProcess(queryInfo, jobProcessResult, usersList ?? suggestedUsers.UsersList);
                //            #endregion
                //        }
                //    }
                //    else
                //        GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName, ActivityType, $"Username not exists : {queryInfo.QueryValue}");
                //}
                #endregion
                Token.ThrowIfCancellationRequested();

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
            catch (Exception )
            {
               // ex.DebugLog();
            }
        }
    }
}
