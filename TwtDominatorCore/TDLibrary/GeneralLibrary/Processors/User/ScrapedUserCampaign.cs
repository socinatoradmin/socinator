using System;
using System.Collections.Generic;
using System.Linq;
using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.TdTables.Campaign;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using TwtDominatorCore.Database;
using TwtDominatorCore.QueryHelper;
using TwtDominatorCore.TDFactories;
using TwtDominatorCore.TDLibrary.GeneralLibrary.DAL;

namespace TwtDominatorCore.TDLibrary.GeneralLibrary.Processors.User
{
    internal class ScrapedUserCampaign : BaseTwitterUserProcessor, IQueryProcessor
    {
        public ScrapedUserCampaign(ITdJobProcess jobProcess, IBlackWhiteListHandler blackWhiteListHandler,
            IDbCampaignService campaignService,
            ITwitterFunctionFactory twitterFunctionFactory, IDbInsertionHelper dbInsertionHelper)
            : base(jobProcess, blackWhiteListHandler, campaignService, twitterFunctionFactory.TwitterFunctions,
                dbInsertionHelper)
        {
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                var classMapper = new ClassMapper(_jobProcess.AccountId, _jobProcess.AccountName);

                if (_jobProcess.checkJobCompleted()) return;

                // getting campaign details from given campaign id in queryvalue
                var scrapedUserCampaignDb =
                    InstanceProvider.ResolveCampaignDbOperations(queryInfo.QueryValue, SocialNetworks.Twitter);

                // get all scraped users from  given campaignid
                var scrapedUsers = scrapedUserCampaignDb.Get<InteractedUsers>();

                List<string> activityDoneWithUserList;

                if (_campaignService != null)
                    activityDoneWithUserList = _campaignService.GetAllInteractedUsers()
                        .Select(x => x.InteractedUsername.ToLower()).ToList();
                else
                    activityDoneWithUserList = _dbAccountService.GetInteractedUsers(ActivityType)
                        .Select(x => x.InteractedUsername.ToLower()).ToList();

                scrapedUsers.RemoveAll(user => activityDoneWithUserList.Contains(user.InteractedUsername.ToLower()));

                foreach (var user in scrapedUsers)
                {
                    jobProcessResult = new JobProcessResult();
                    // Mapping(typecasting) interacteduser to TwitterUser
                    var twtUser = classMapper.InteractedUserToTwitterUserMapper(user);
                    _jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    FinalProcessForEachUser(queryInfo, out jobProcessResult, twtUser);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}