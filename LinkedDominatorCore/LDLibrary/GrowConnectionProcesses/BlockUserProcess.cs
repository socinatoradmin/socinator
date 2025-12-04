using System;
using System.Threading;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.LdTables.Account;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.Factories;
using LinkedDominatorCore.LDLibrary.DAL;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDUtility;
using LinkedDominatorCore.Request;
using LinkedDominatorCore.Utility;
using Newtonsoft.Json;

namespace LinkedDominatorCore.LDLibrary.GrowConnectionProcesses
{
    public class
        BlockUserProcess : LDJobProcessInteracted<InteractedUsers>
    {
        private readonly IDelayService _delayService;
        private readonly ILdFunctions _ldFunctions;

        public BlockUserProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IDbGlobalService globalService,
            IExecutionLimitsManager executionLimitsManager, ILdQueryScraperFactory queryScraperFactory,
            ILdHttpHelper ldHttpHelper, ILdLogInProcess logInProcess,
            ILdFunctionFactory ldFunctionFactory, IDbInsertionHelper dbInsertionHelper, IDelayService delayService)
            : base(processScopeModel, accountServiceScoped, globalService, executionLimitsManager, queryScraperFactory,
                ldHttpHelper, logInProcess, dbInsertionHelper)
        {
            _ldFunctions = ldFunctionFactory.LdFunctions;
            _delayService = delayService;
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            var jobProcessResult = new JobProcessResult();
            #region Block User Process.
            try
            {

                var linkedinUser = (LinkedinUser)scrapeResult.ResultUser;
                linkedinUser.PublicIdentifier = Utilities.GetBetween(linkedinUser.ProfileUrl + "**", "in/", "**");
                GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, " block user ", $"\"{linkedinUser.PublicIdentifier}\"");
                
                var responseParams = NormalProcess(linkedinUser, jobProcessResult);
                if (!string.IsNullOrEmpty(responseParams.Response) &&
                    responseParams.Response.Contains("responseCode\":200"))
                {
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, linkedinUser.ProfileUrl);
                    IncrementCounters();

                    var serializedLinkedinUserDetails = JsonConvert.SerializeObject(linkedinUser);
                    DbInsertionHelper.UserScraper(scrapeResult, linkedinUser, serializedLinkedinUserDetails);
                    jobProcessResult.IsProcessSuceessfull = true;
                }
                else
                {
                    var message = responseParams.Response.Contains("This profile is not available")
                        ? $"{linkedinUser.ProfileUrl} is not available"
                        : linkedinUser.ProfileUrl;
                    GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, message);

                    Thread.Sleep(5000);
                    jobProcessResult.IsProcessSuceessfull = false;
                }

                DelayBeforeNextActivity();
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            #endregion
            return jobProcessResult;
        }


        private IResponseParameter NormalProcess(LinkedinUser linkedinUser, JobProcessResult jobProcessResult)
        {
            IResponseParameter responseParams = null;
            string pageResponse;
            if (new GetDetailedUserInfo(_delayService).IsValidLinkJobProcessResult(linkedinUser.ProfileUrl,
                jobProcessResult, _ldFunctions,
                DominatorAccountModel, out pageResponse))
                return responseParams;           

            if(IsBrowser)
            {
                linkedinUser.FullName = GetFullName(pageResponse);                
                return _ldFunctions.BlockUserResponse(linkedinUser.ProfileUrl);
            }
            else
            {
                var ldDataHelper = LdDataHelper.GetInstance;
                DetailsFetcher.UserInformation("", false, linkedinUser.ProfileUrl,
                pageResponse, linkedinUser, _ldFunctions, DominatorAccountModel,ActivityType);
                var csrfToken = ldDataHelper.GetCsrfTokenFromCookies(_ldFunctions.GetInnerHttpHelper()
                    .GetRequestParameter());
                return _ldFunctions.BlockUserResponse(LdConstants.GetBlockUserAPI(linkedinUser.ProfileId,csrfToken));
            }
        }

        private string GetFullName(string pageResponse)
        {
            var userfullname = HtmlAgilityHelper.GetStringInnerTextFromClassName(pageResponse,LDClassesConstant.GrowConnection.UserFullNameClass);            
            return userfullname;
        }
    }
}