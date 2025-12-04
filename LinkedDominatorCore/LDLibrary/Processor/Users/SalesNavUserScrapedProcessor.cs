using System;
using System.Linq;
using CommonServiceLocator;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.LdTables.Account;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using LinkedDominatorCore.Factories;
using LinkedDominatorCore.LDLibrary.DAL;
using DominatorHouseCore.Utility;

namespace LinkedDominatorCore.LDLibrary.Processor.Users
{
    public class SalesNavUserScrapedProcessor : BaseLinkedinUserProcessor, IQueryProcessor
    {
        public SalesNavUserScrapedProcessor(ILdJobProcess jobProcess,
            IDbCampaignService campaignService, ILdFunctionFactory ldFunctionFactory, IDelayService delayService,
            IProcessScopeModel processScopeModel) :
            base(jobProcess, campaignService, ldFunctionFactory, delayService, processScopeModel)
        {
        }


        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                // getting campaign details from given campaign id in queryValue

                var scrapedUserCampaignDb =
                    InstanceProvider.ResolveCampaignDbOperations(queryInfo.QueryValue, SocialNetworks.LinkedIn);

                // get all scraped users from  given campaignId
                var interactedUsers = scrapedUserCampaignDb.Get<InteractedUsers>()
                    .Where(x => x.ActivityType == ActivityType.SalesNavigatorUserScraper.ToString()).ToList();

                var activityDoneWithUserList = string.IsNullOrEmpty(LdJobProcess.CurrentCampaignId)
                    ? DbAccountService.GetInteractedUsers(ActivityType.ToString()).Select(x => x.ProfileId).ToList()
                    : DbCampaignService.GetInteractedUsers(ActivityType.ToString()).Select(x => x.ProfileId).ToList();

                interactedUsers.RemoveAll(user => activityDoneWithUserList.Contains(user.ProfileId));
                var listLinkedInUser = ClassMapper.InteractedUserToLinkedInUserMapper(interactedUsers);

                ProcessLinkedinUsersFromUserList(queryInfo, ref jobProcessResult, listLinkedInUser);

                jobProcessResult.HasNoResult = true;
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                jobProcessResult.HasNoResult = true;
                ex.DebugLog();
            }
        }
    }
}