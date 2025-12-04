using System;
using System.Linq;
using System.Text;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.LdTables.Account;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.DetailedInfo;
using LinkedDominatorCore.Factories;
using LinkedDominatorCore.LDLibrary.DAL;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDUtility;
using LinkedDominatorCore.Request;
using LinkedDominatorCore.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// ReSharper disable once CheckNamespace
namespace LinkedDominatorCore.LDLibrary
{
    public class CompanyScraperProcess : LDJobProcessInteracted<
        InteractedCompanies>
    {
        private readonly IDelayService _delayService;
        private readonly LdDataHelper _ldDataHelper;
        private readonly ILdFunctions _ldFunctions;

        public CompanyScraperProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IDbGlobalService globalService,
            IExecutionLimitsManager executionLimitsManager, ILdQueryScraperFactory queryScraperFactory,
            ILdHttpHelper ldHttpHelper, ILdLogInProcess logInProcess,
            ILdFunctionFactory ldFunctionFactory, IDbInsertionHelper dbInsertionHelper, IDelayService delayService)
            : base(processScopeModel, accountServiceScoped, globalService, executionLimitsManager, queryScraperFactory,
                ldHttpHelper, logInProcess, dbInsertionHelper)
        {
            CompanyScraperModel = processScopeModel.GetActivitySettingsAs<CompanyScraperModel>();
            _ldFunctions = ldFunctionFactory.LdFunctions;
            _delayService = delayService;
            _ldDataHelper = LdDataHelper.GetInstance;
        }

        public CompanyScraperModel CompanyScraperModel { get; set; }


        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            var jobProcessResult = new JobProcessResult();
            var companyScraperDetailedInfo = new CompanyScraperDetailedInfo();
            try
            {
                var objLinkedinCompany = scrapeResult.ResultCompany;

                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                    objLinkedinCompany.CompanyUrl);
                var universalName = objLinkedinCompany.CompanyUrl?.Split('/').Last(x => x.ToString() != string.Empty);
                var resultScrapeCompanyDetails = new GetDetailedUserInfo(_delayService).ScrapeCompanyDetails(
                    _ldFunctions,
                    objLinkedinCompany.CompanyUrl, ref companyScraperDetailedInfo, DominatorAccountModel,universalName);

                var detailedCompanyInfoJasonString = JsonConvert.SerializeObject(companyScraperDetailedInfo);
                objLinkedinCompany.IsFollowed = objLinkedinCompany.IsFollowed == "N/A" ? companyScraperDetailedInfo.IsFollowing : objLinkedinCompany.IsFollowed;
                objLinkedinCompany.TotalEmployees = objLinkedinCompany.TotalEmployees == "N/A" ? companyScraperDetailedInfo.TotalEmployees : objLinkedinCompany.TotalEmployees;
                if (resultScrapeCompanyDetails)
                {
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        objLinkedinCompany.CompanyUrl);
                    IncrementCounters();
                    DbInsertionHelper.CompanyScraper(scrapeResult, objLinkedinCompany, detailedCompanyInfoJasonString);
                    //this.AccountModel.LstConnections.Add(LinkedinUser);
                    jobProcessResult.IsProcessSuceessfull = true;
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        objLinkedinCompany.CompanyUrl, "");
                    jobProcessResult.IsProcessSuceessfull = false;
                }

                DelayBeforeNextActivity();
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception e)
            {
                e.DebugLog();
            }

            return jobProcessResult;
        }

        public void GetCompanyLocations(JArray jArray, CompanyScraperDetailedInfo companyScraperDetailedInfo)
        {
            try
            {
                var jArrayHandler = JsonJArrayHandler.GetInstance;
                var confirmedLocations = jArrayHandler.GetTokenElement(jArray, "confirmedLocations");
                var stringBuilder = new StringBuilder();
                var count = 0;
                foreach (var jData in confirmedLocations)
                {
                    ++count;
                    _ldDataHelper.GetCompanyAddress(stringBuilder, count, jArrayHandler, jData);
                }

                companyScraperDetailedInfo.OtherLocations = stringBuilder.ToString().Trim();
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }
        }
    }
}