using System;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.LdTables.Account;
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
using CompanyScraperModel = LinkedDominatorCore.LDModel.SalesNavigatorScraper.CompanyScraperModel;

namespace LinkedDominatorCore.LDLibrary.SalesNavigatorScraperProcesses
{
    public class CompanyScraperProcess : LDJobProcessInteracted<
        InteractedCompanies>
    {
        private readonly ILdFunctions _ldFunctions;


        public CompanyScraperProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IDbGlobalService globalService,
            IExecutionLimitsManager executionLimitsManager, ILdQueryScraperFactory queryScraperFactory,
            ILdHttpHelper ldHttpHelper, ILdLogInProcess logInProcess,
            ILdFunctionFactory ldFunctionFactory, IDbInsertionHelper dbInsertionHelper)
            : base(processScopeModel, accountServiceScoped, globalService, executionLimitsManager, queryScraperFactory,
                ldHttpHelper, logInProcess, dbInsertionHelper)
        {
            CompanyScraperModel = processScopeModel.GetActivitySettingsAs<CompanyScraperModel>();
            _ldFunctions = ldFunctionFactory.LdFunctions;
        }

        public CompanyScraperModel CompanyScraperModel { get; set; }


        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            var jobProcessResult = new JobProcessResult();
            try
            {
                var objLinkedinCompany = scrapeResult.ResultCompany;

                try
                {
                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objLinkedinCompany.CompanyName);
                    //var companyDetailApi = "https://www.linkedin.com/sales/api/v1/account?companyId=" + objLinkedinCompany.CompanyId;
                    var companyDetailApi = IsBrowser
                        ? objLinkedinCompany.CompanyUrl
                        : LdDataHelper.GetInstance.GetSalesCompanyDetailsApi(objLinkedinCompany.CompanyId);

                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    var objSalesNavigatorDetailsResponseHandler =
                        _ldFunctions.GetSalesNavigatorCompanyDetails(companyDetailApi, objLinkedinCompany.CompanyUrl);
                    if (objSalesNavigatorDetailsResponseHandler.Success)
                    {
                        GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            objLinkedinCompany.CompanyName);
                        IncrementCounters();
                        JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                        DbInsertionHelper.SalesNavCompany(scrapeResult, objLinkedinCompany,
                            objSalesNavigatorDetailsResponseHandler.JsonObject);
                        //this.AccountModel.LstConnections.Add(LinkedinUser);
                        jobProcessResult.IsProcessSuceessfull = true;
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(Log.ActivityFailed,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            objLinkedinCompany.CompanyName, "");
                        jobProcessResult.IsProcessSuceessfull = false;
                    }
                }
                catch (OperationCanceledException)
                {
                    throw new OperationCanceledException("Operation Cancelled!");
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
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

            return jobProcessResult;
        }
    }
}