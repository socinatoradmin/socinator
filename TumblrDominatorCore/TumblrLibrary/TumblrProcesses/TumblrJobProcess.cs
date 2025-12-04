using DominatorHouseCore;
using DominatorHouseCore.BusinessLogic.Scraper;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Utility;
using System;
using TumblrDominatorCore.Models;
using TumblrDominatorCore.TumblrFactory;
using TumblrDominatorCore.TumblrLibrary.DAL;
using TumblrDominatorCore.TumblrLibrary.TumblrFunction.TumblrBrowserManager;
using TumblrDominatorCore.TumblrRequest;

namespace TumblrDominatorCore.TumblrLibrary.TumblrProcesses
{
    public interface ITumblrJobProcess : IJobProcess
    {
        ModuleSetting ModuleSetting { get; }
        ITumblrBrowserManager browserManager { get; set; }
        JobProcessResult FinalProcess(ScrapeResultNew ScrapedResult);
        bool checkJobCompleted();
    }

    public abstract class TumblrJobProcessInteracted<T> : TumblrJobProcess where T : class, new()
    {
        private readonly IExecutionLimitsManager _executionLimitsManager;
        protected ITumblrBrowserManager _browserManager;

        protected TumblrJobProcessInteracted(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped _accountService,
            IDbGlobalService _dbGlobalService, IExecutionLimitsManager executionLimitsManager,
            ITumblrQueryScraperFactory queryScraperFactory, ITumblrHttpHelper _httpHelper,
            ITumblrLoginProcess _tumblrLoginProcess) : base(processScopeModel, _accountService, _dbGlobalService,
            queryScraperFactory, _httpHelper, _tumblrLoginProcess)
        {
            _executionLimitsManager = executionLimitsManager;
            _browserManager = _tumblrLoginProcess._browser;
            _tumblrLoginProcess._browser.CancellationToken = JobCancellationTokenSource.Token;
        }

        public override ReachedLimitInfo CheckLimit()
        {
            return _executionLimitsManager.CheckIfLimitreached<T>(Id, SocialNetworks.Tumblr,
                ActivityType);
        }
    }

    public class TumblrJobProcess : JobProcess, ITumblrJobProcess
    {
        protected readonly DbCampaignService CampaignService;
        protected readonly IDbAccountServiceScoped DbAccountService;
        protected readonly IDbGlobalService DbglobalService;

        private readonly ITumblrLoginProcess _loginProcess;

        public TumblrJobProcess(IProcessScopeModel processScopeModel, IDbAccountServiceScoped _accountService,
            IDbGlobalService _dbGlobalService, IQueryScraperFactory queryScraperFactory,
            ITumblrHttpHelper _httpHelper, ITumblrLoginProcess _tumblrLoginProcess)
            : base(processScopeModel, queryScraperFactory, _httpHelper)
        {
            ModuleSetting = processScopeModel.GetActivitySettingsAs<ModuleSetting>();
            CampaignService = new DbCampaignService(processScopeModel.CampaignId);
            DbAccountService = _accountService;
            DbglobalService = _dbGlobalService;
            _loginProcess = _tumblrLoginProcess;
            browserManager = _tumblrLoginProcess._browser;
        }
        public ITumblrBrowserManager browserManager { get; set; }
        public bool AddedToDb { get; set; }
        public AccountModel AccountModel { get; set; }
        public ModuleSetting ModuleSetting { get; set; }

        public override ReachedLimitInfo CheckLimit()
        {
            throw new NotImplementedException();
        }

        public bool checkJobCompleted()
        {
            if (CheckJobProcessLimitsReached())
            {
                GlobusLogHelper.log.Info(Log.JobCompleted, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.UserName, ActivityType);
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Logs in to Tumblr and scrape its feed (RunScraper)
        /// </summary>
        protected override bool Login()
        {
            return LoginBase(_loginProcess);
        }

        protected override bool CloseAutomationBrowser()
        {
            try
            {
                if (DominatorAccountModel.IsRunProcessThroughBrowser)
                    _loginProcess._browser.CloseBrowser(DominatorAccountModel);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return true;
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {

            var jobProcessResult = PostScrapeProcess(scrapeResult);
            jobProcessResult.IsProcessCompleted = CheckJobProcessLimitsReached();

            try
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                if (jobProcessResult.IsProcessCompleted)
                {
                    StartOtherConfiguration(scrapeResult);
                    GlobusLogHelper.log.Info(Log.JobCompleted, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.UserName, ActivityType);
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancelled!");
            }

            return jobProcessResult;
        }
    }
}