using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CommonServiceLocator;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.Factories;
using LinkedDominatorCore.LDLibrary.DAL;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.Request;
using LinkedDominatorCore.Utility;

namespace LinkedDominatorCore.LDLibrary
{
    public interface ILdJobProcess : IJobProcess
    {
        ModuleSetting ModuleSetting { get; }
        bool AddedToDb { get; set; }
        IDbAccountService DbAccountService { get; set; }
        IDbGlobalService GlobalService { get; set; }
        CancellationToken LdCancellationToken { get; set; }
        string CurrentCampaignId { get; set; }

        int CurrentJobCount { get; set; }
        JobProcessResult FinalProcess(ScrapeResultNew scrapedResult);
        bool CheckJobCompleted();
    }

    // ReSharper disable once InconsistentNaming
    public abstract class LDJobProcessInteracted<T> : LDJobProcess where T : class, new()
    {
        private readonly IExecutionLimitsManager _executionLimitsManager;

        //  protected readonly ILdFunctions LdFunctions;

        protected LDJobProcessInteracted(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IDbGlobalService globalService,
            IExecutionLimitsManager executionLimitsManager, ILdQueryScraperFactory queryScraperFactory,
            ILdHttpHelper ldHttpHelper, ILdLogInProcess ldLogInProcess, IDbInsertionHelper dbInsertionHelper) :
            base(processScopeModel, accountServiceScoped, globalService, queryScraperFactory, ldHttpHelper,
                ldLogInProcess, dbInsertionHelper)
        {
            _executionLimitsManager = executionLimitsManager;
        }

        //protected LDJobProcessInteracted(string account, string template, ActivityType activityType,
        //    TimingRange currentJobTimeRange) : base(account, template, activityType, currentJobTimeRange)
        //{
        //    _executionLimitsManager = InstanceProvider.GetInstance<IExecutionLimitsManager>();
        //}

        public override ReachedLimitInfo CheckLimit()
        {
            CurrentJobCount = _executionLimitsManager.GetCurrentJobCount(Id);
            return _executionLimitsManager.CheckIfLimitreached<T>(Id, SocialNetworks.LinkedIn,
                ActivityType);
        }
    }

    // ReSharper disable once InconsistentNaming
    public abstract class LDJobProcess : JobProcess, ILdJobProcess
    {
        private readonly ILdLogInProcess _ldLogInProcess;
        protected readonly IDbInsertionHelper DbInsertionHelper;
        protected readonly ILdHttpHelper HttpHelper;

        protected readonly bool IsBrowser;
        protected readonly ILdUserFilterProcess LdUserFilterProcess;
        protected IDetailsFetcher DetailsFetcher;


        protected LDJobProcess(IProcessScopeModel processScopeModel, IDbAccountServiceScoped accountServiceScoped,
            IDbGlobalService globalService, ILdQueryScraperFactory queryScraperFactory, ILdHttpHelper httpHelper,
            ILdLogInProcess ldLogInProcess,
            IDbInsertionHelper dbInsertionHelper) :
            base(processScopeModel, queryScraperFactory, httpHelper)
        {
            ModuleSetting = processScopeModel.GetActivitySettingsAs<ModuleSetting>();
            AccountModel = new AccountModel(DominatorAccountModel);
            DbAccountService = accountServiceScoped;
            GlobalService = globalService;
            _ldLogInProcess = ldLogInProcess;
            HttpHelper = httpHelper;

            // this catch handling is for unitTest only
            DetailsFetcher = InstanceProvider.GetInstance<IDetailsFetcher>();
            LdUserFilterProcess = InstanceProvider.GetInstance<ILdUserFilterProcess>();
            DbInsertionHelper = dbInsertionHelper;
            CurrentCampaignId = CampaignId;
            IsBrowser = ldLogInProcess.IsBrowser;
            if (LdUserFilterProcess.IsBrowser != IsBrowser)
                LdUserFilterProcess.IsBrowser = IsBrowser;
        }

        public AccountModel AccountModel { get; set; }

        public int CurrentJobCount { get; set; }

        public override List<string> AlreadyProcessedQueryValues()
        {
            var returnList = new List<string>();
            try
            {
                switch (ActivityType)
                {
                    case ActivityType.ConnectionRequest:
                    case ActivityType.SalesNavigatorUserScraper:
                    case ActivityType.UserScraper:
                        var users = DbAccountService.GetInteractedUsers(ActivityType.ToString());
                        returnList = users.Where(x => x.QueryType == "Search Url").Select(x => x.QueryValue).ToList();
                        break;

                    case ActivityType.JobScraper:
                    case ActivityType.SalesNavigatorCompanyScraper:
                        var jobs = DbAccountService.GetInteractedJobs(ActivityType.ToString());
                        returnList = jobs.Where(x => x.QueryType == "Search Url").Select(x => x.QueryValue).ToList();
                        break;

                    case ActivityType.CompanyScraper:
                        var companies = DbAccountService.GetInteractedCompanies(ActivityType.ToString());
                        returnList = companies.Where(x => x.QueryType == "Search Url").Select(x => x.QueryValue)
                            .ToList();
                        break;
                }
            }
            catch
            {
            }

            returnList = returnList.Distinct().ToList();
            return returnList;
        }

        public ModuleSetting ModuleSetting { get; set; }


        public string CurrentCampaignId { get; set; }

        public IDbAccountService DbAccountService { get; set; }
        public IDbGlobalService GlobalService { get; set; }
        public CancellationToken LdCancellationToken { get; set; }
        public bool AddedToDb { get; set; }


        //public int MaxJobCount { get; set; }
        // private bool IsViewProfileUsingEmbeddedBrowser { get; set; }

        public bool CheckJobCompleted()
        {
            if (!CheckJobProcessLimitsReached()) return false;
            GlobusLogHelper.log.Info(Log.JobCompleted, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                DominatorAccountModel.UserName, ActivityType);
            return true;
        }

        protected override bool Login()
        {
            var ldJobProcess = (LogInProcess) _ldLogInProcess;
            ldJobProcess.IsViewProfileUsingEmbeddedBrowser = ModuleSetting.IsViewProfileUsingEmbeddedBrowser;
            ldJobProcess.ActivityType = ActivityType;
            try
            {
                if (ModuleSetting?.SavedQueries != null)
                    ldJobProcess.IsConnectionRequestSalesSearchUrl =
                        ModuleSetting.SavedQueries.Any(x => x.QueryType == "Sales Navigator SearchUrl");
            }
            catch (Exception)
            {
                //
            }

            var returnValue = LoginBase(_ldLogInProcess);
            if (DominatorAccountModel.Token.IsCancellationRequested) JobCancellationTokenSource.Cancel();
            return returnValue;
        }

        protected override bool CloseAutomationBrowser()
        {
            LDAccountsBrowserDetails.CloseBrowser(DominatorAccountModel,
                !DominatorAccountModel.IsRunProcessThroughBrowser);
            return true;
        }
    }
}