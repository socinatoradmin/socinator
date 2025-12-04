using System;
using System.Linq;
using System.Net;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Utility;
using TwtDominatorCore.Factories;
using TwtDominatorCore.Requests;
using TwtDominatorCore.TDLibrary.GeneralLibrary.DAL;
using TwtDominatorCore.TDModels;
using TwtDominatorCore.TDUtility;

namespace TwtDominatorCore.TDLibrary
{
    public interface ITdJobProcess : IJobProcess
    {
        ModuleSetting ModuleSetting { get; }
        bool AddedToDb { get; set; }
        IDbAccountService DbAccountService { get; set; }
        IDbGlobalService GlobalService { get; set; }
        AccountModel AccountModel { get; set; }
        JobProcessResult FinalProcess(ScrapeResultNew ScrapedResult);
        bool checkJobCompleted();
    }

    public abstract class TdJobProcessInteracted<T> : TdJobProcess where T : class, new()
    {
        private readonly IExecutionLimitsManager _executionLimitsManager;

        protected TdJobProcessInteracted(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IDbGlobalService globalService,
            IExecutionLimitsManager executionLimitsManager, ITdQueryScraperFactory queryScraperFactory,
            ITdHttpHelper tdHttpHelper, ITwtLogInProcess twtLogInProcess) : base(
            processScopeModel, accountServiceScoped, globalService, queryScraperFactory, tdHttpHelper, twtLogInProcess)
        {
            _executionLimitsManager = executionLimitsManager;
        }

        public override ReachedLimitInfo CheckLimit()
        {
            return _executionLimitsManager.CheckIfLimitreached<T>(Id, SocialNetworks.Twitter,
                ActivityType);
        }
    }

    public abstract class TdJobProcess : JobProcess, ITdJobProcess
    {
        private readonly ITwtLogInProcess _twtLogInProcess;

        protected TdJobProcess(IProcessScopeModel processScopeModel, IDbAccountServiceScoped accountServiceScoped,
            IDbGlobalService globalService, ITdQueryScraperFactory queryScraperFactory, ITdHttpHelper httpHelper,
            ITwtLogInProcess twtLogInProcess) : base(processScopeModel,
            queryScraperFactory, httpHelper)
        {
            ModuleSetting = processScopeModel.GetActivitySettingsAs<ModuleSetting>();
            AccountModel = new AccountModel(DominatorAccountModel);
            DbAccountService = accountServiceScoped;
            GlobalService = globalService;
            _twtLogInProcess = twtLogInProcess;

            try
            {
                if (AccountModel.CsrfToken == null)
                    AccountModel.CsrfToken = DominatorAccountModel.Cookies.OfType<Cookie>()
                        .SingleOrDefault(x => x.Name == "ct0")?.Value;
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }

        public TdAccountsBrowserDetails TdAccountsBrowserDetails { get; set; }
        public IDbAccountService DbAccountService { get; set; }
        public IDbGlobalService GlobalService { get; set; }

        public ModuleSetting ModuleSetting { get; set; }

        public AccountModel AccountModel { get; set; }
        public bool AddedToDb { get; set; }


        public override JobProcessResult FinalProcess(ScrapeResultNew ScrapedResult)
        {
            var jobProcessResult = PostScrapeProcess(ScrapedResult);
            jobProcessResult.IsProcessCompleted = CheckJobProcessLimitsReached();

            try
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                if (jobProcessResult.IsProcessCompleted)
                {
                    StartOtherConfiguration(ScrapedResult);
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

        protected override bool Login()
        {
            return LoginBase(_twtLogInProcess);
        }

        protected override bool CloseAutomationBrowser()
        {
            TdAccountsBrowserDetails.CloseAllBrowser(DominatorAccountModel,
                !DominatorAccountModel.IsRunProcessThroughBrowser);
            return true;
        }
    }
}