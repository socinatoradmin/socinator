using ThreadUtils;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using GramDominatorCore.GDLibrary.DAL;
using GramDominatorCore.GDModel;
using System;
using GramDominatorCore.GDFactories;
using GramDominatorCore.GDLibrary.InstagramBrowser;
using Newtonsoft.Json;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Request;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Utility;
using GramDominatorCore.Utility;
using PuppeteerSharp;

namespace GramDominatorCore.GDLibrary.Processor.User
{
    public class KeywordProcessor : BaseInstagramUserProcessor
    {
        public BroadcastMessagesModel BroadcastMessagesModel { get; set; }
        public KeywordProcessor(IGdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService, IDbCampaignService campaignService, IProcessScopeModel processScopeModel, IDelayService _delayService, IGdBrowserManager gdBrowserManager)
            : base(jobProcess, dbAccountService, campaignService, processScopeModel, _delayService, gdBrowserManager)
        {

            BroadcastMessagesModel = JsonConvert.DeserializeObject<BroadcastMessagesModel>(TemplateModel.ActivitySettings);

        }
        public string MaxId { get; set; }
        public string RankToken { get; set; }
        public bool HasMax { get; set; }
        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                Token.ThrowIfCancellationRequested();
                if (CheckQueryValueOnMessageList(BroadcastMessagesModel, queryInfo)) return;
                var keyword = queryInfo.QueryValue;
                var searchKeywordIgResponseHandler =
                GramStatic.IsBrowser ?
                GdBrowserManager.SearchForkeyword(DominatorAccountModel, keyword, Token)
                : InstaFunction.SearchForkeyword(DominatorAccountModel, keyword, Token).Result;
                if (!CheckingLoginRequiredResponse(searchKeywordIgResponseHandler.ToString(), "", queryInfo))
                    return;

                if (searchKeywordIgResponseHandler.Success)
                {
                    Token.ThrowIfCancellationRequested();
                    var usersList = FilterWhitelistBlacklistUsers(searchKeywordIgResponseHandler.UsersList);
                    GetInteractedUserAccrossAllFor(usersList ?? searchKeywordIgResponseHandler.UsersList);
                    GetInteractedCampaignUser(usersList ?? searchKeywordIgResponseHandler.UsersList);
                    CheckUserInDatabase(usersList ?? searchKeywordIgResponseHandler.UsersList);
                    if (ActivityType == ActivityType.Follow)
                        usersList = GetUserInfoDetails(usersList ?? searchKeywordIgResponseHandler.UsersList);
                    if (ModuleSetting.IsTaggedPostUser)
                        GetTaggedUser(queryInfo, jobProcessResult, usersList ?? searchKeywordIgResponseHandler.UsersList);
                    else
                        FilterAndStartFinalProcess(queryInfo, jobProcessResult, usersList ?? searchKeywordIgResponseHandler.UsersList);
                }
                if (searchKeywordIgResponseHandler.HasMore)
                {
                    HasMax = searchKeywordIgResponseHandler.HasMore;
                    MaxId = searchKeywordIgResponseHandler.PageId;
                    RankToken = searchKeywordIgResponseHandler.RankId;
                }
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
            finally { jobProcessResult.IsProcessCompleted = true; }
        }
    }
}
