using ThreadUtils;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using GramDominatorCore.GDLibrary.DAL;
using GramDominatorCore.GDModel;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using GramDominatorCore.GDLibrary.InstagramBrowser;

namespace GramDominatorCore.GDLibrary.Processor.User
{
    public class CustomUsersProcessors : BaseInstagramUserProcessor
    {
        public BroadcastMessagesModel BroadcastMessagesModel { get; set; }
        private List<DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedUsers> LstCampaignIntractedUsersForUserScraper { get; set; } = new List<DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedUsers>();
        public CustomUsersProcessors(IGdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService, IDbCampaignService campaignService, IProcessScopeModel processScopeModel, IDelayService delayService, IGdBrowserManager gdBrowserManager) :
            base(jobProcess, dbAccountService, campaignService, processScopeModel, delayService,gdBrowserManager)
        {
            BroadcastMessagesModel = JsonConvert.DeserializeObject<BroadcastMessagesModel>(TemplateModel.ActivitySettings);
        }
        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                Token.ThrowIfCancellationRequested();
                if (CheckQueryValueOnMessageList(BroadcastMessagesModel, queryInfo)) return;
                List<InstagramUser> forUserTag = new List<InstagramUser>();
                string customUsername = CheckCustomUsername(queryInfo);
                var userInfo = InstaFunction.SearchUsername(DominatorAccountModel, customUsername, Token);
                //if (!DominatorAccountModel.IsRunProcessThroughBrowser)
                //{
                //    userInfo = InstaFunction.SearchUsername(DominatorAccountModel, customUsername, Token); 
                //}
                //else
                //{
                //    userInfo = GdBrowserManager.GetUserInfo(DominatorAccountModel, customUsername, Token);
                //}
                if (userInfo.IsPrivate && (userInfo?.instaUserDetails != null && userInfo.instaUserDetails.IsFollowing))
                {
                    jobProcessResult.IsProcessCompleted = true;
                    return;
                }
                if (!CheckingLoginRequiredResponse(userInfo.ToString(), "", queryInfo))
                    return;

                if (CheckInteractedUserDbData(LstInteractedUsers, userInfo, LstCampaignIntractedUsersForUserScraper) && queryInfo.QueryType == "Custom Users List")
                {
                    jobProcessResult.HasNoResult = true;
                    return;
                }

                if (CheckInteractedUserDbData(LstInteractedUsers, userInfo, LstCampaignIntractedUsersForUserScraper))
                    return;

                if (ModuleSetting.IsTaggedPostUser && ActivityType == ActivityType.UserScraper)
                {
                    forUserTag.Add(userInfo);
                    GetTaggedUser(queryInfo, jobProcessResult, forUserTag);
                }
                else
                    FilterAndStartFinalProcessForOneUser(queryInfo, ref jobProcessResult, userInfo);


      //          DelayForScraperActivity();

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
            catch (Exception)
            {
                // ex.DebugLog();
            }
            finally
            {
                jobProcessResult.IsProcessCompleted = true;
            }
        }

    }
}
