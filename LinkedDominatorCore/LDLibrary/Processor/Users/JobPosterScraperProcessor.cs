using System;
using System.Collections.Generic;
using System.Linq;
using CommonServiceLocator;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.LdTables.Account;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.DetailedInfo;
using LinkedDominatorCore.Factories;
using LinkedDominatorCore.LDLibrary.DAL;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDUtility;
using Newtonsoft.Json;

namespace LinkedDominatorCore.LDLibrary.Processor.Users
{
    public class JobPosterScraperProcessor : BaseLinkedinUserProcessor, IQueryProcessor
    {
        public JobPosterScraperProcessor(ILdJobProcess jobProcess,
            IDbCampaignService campaignService, ILdFunctionFactory ldFunctionFactory, IDelayService delayService,
            IProcessScopeModel processScopeModel) :
            base(jobProcess, campaignService, ldFunctionFactory, delayService, processScopeModel)
        {
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                var linkedinUser = new List<LinkedinUser>();
                var scrapedJobCampaignDb =
                    InstanceProvider.ResolveCampaignDbOperations(queryInfo.QueryValue, SocialNetworks.LinkedIn);
                var interactedUsers = scrapedJobCampaignDb.Get<InteractedJobs>()
                    .Where(x => x.ActivityType == ActivityType.JobScraper.ToString()).ToList();
                var Userdetails = interactedUsers.Select(x => x.DetailedInfo).ToList();

                foreach (var item in Userdetails)
                {
                    var user = new LinkedinUser();
                    var objJobInfo =
                        JsonConvert.DeserializeObject<JobScraperDetailedInfo>(
                            Uri.UnescapeDataString(item));
                    user.ProfileUrl = objJobInfo.JobPosterProfileUrl;

                    if (!string.IsNullOrEmpty(user.ProfileUrl) && (user.ProfileUrl.Contains("N/A") || user.ProfileUrl.Contains("https://www.linkedin.com/company")))
                        continue;
                    var firstName = objJobInfo.JobPosterFirstName;
                    var lastName = objJobInfo.JobPosterLastName;
                    user.ProfileId = Utils.GetBetween($"{objJobInfo.JobPosterProfileUrl}*", "/in/", "*");
                    user.FullName = $"{firstName} {lastName}";
                    user.Location = objJobInfo.JobLocation;
                    user.HeadlineTitle = objJobInfo.JobTitle;
                    linkedinUser.Add(user);
                }

                if (linkedinUser.Count() <= 0)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        "sorry no JobPoster User found in that JobScarper Campaign");
                    jobProcessResult.HasNoResult = true;
                }
                else
                {
                    var activityDoneWithUserList = string.IsNullOrEmpty(LdJobProcess.CurrentCampaignId)
                        ? DbAccountService.GetInteractedUsers(ActivityType.ToString()).Select(x => x.ProfileId).ToList()
                        : DbCampaignService.GetInteractedUsers(ActivityType.ToString()).Select(x => x.ProfileId)
                            .ToList();

                    linkedinUser.RemoveAll(user => activityDoneWithUserList.Contains(user.ProfileId));

                    ProcessLinkedinUsersFromUserList(queryInfo, ref jobProcessResult, linkedinUser);

                    jobProcessResult.HasNoResult = true;
                }
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