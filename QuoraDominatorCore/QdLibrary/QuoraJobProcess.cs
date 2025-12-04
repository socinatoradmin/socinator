using System;
using CommonServiceLocator;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Utility;
using QuoraDominatorCore.Factories;
using QuoraDominatorCore.Models;
using QuoraDominatorCore.QdLibrary.DAL;
using QuoraDominatorCore.QdLibrary.QdFunctions;
using QuoraDominatorCore.Request;

namespace QuoraDominatorCore.QdLibrary
{
    public interface IQdJobProcess : IJobProcess
    {
        ModuleSetting ModuleSetting { get; }
        JobProcessResult FinalProcess(ScrapeResultNew scrapedResult);
        IQuoraBrowserManager BrowserManager { get; }
        bool CheckJobCompleted();
    }

    public abstract class QdJobProcessInteracted<T> : QuoraJobProcess where T : class, new()
    {
        private readonly IExecutionLimitsManager _executionLimitsManager;

        protected QdJobProcessInteracted(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped,
            IExecutionLimitsManager executionLimitsManager, IQdQueryScraperFactory queryScraperFactory,
            IQdHttpHelper qdHttpHelper, IQdLogInProcess qdLogInProcess)
            : base(processScopeModel, accountServiceScoped, queryScraperFactory, qdHttpHelper, qdLogInProcess)
        {
            _executionLimitsManager = executionLimitsManager;
        }

        public override ReachedLimitInfo CheckLimit()
        {
            return _executionLimitsManager.CheckIfLimitreached<T>(Id, SocialNetworks.Quora, ActivityType);
        }
    }

    public abstract class QuoraJobProcess : JobProcess, IQdJobProcess
    {
        public readonly IQdLogInProcess QdLogInProcess;

        protected QuoraJobProcess(IProcessScopeModel processScopeModel, IDbAccountServiceScoped accountServiceScoped,
            IQdQueryScraperFactory queryScraperFactory, IQdHttpHelper httpHelper, IQdLogInProcess qdLogInProcess)
            : base(processScopeModel, queryScraperFactory, httpHelper)
        {
            ModuleSetting = processScopeModel.GetActivitySettingsAs<ModuleSetting>();
            DbAccountService = accountServiceScoped;
            QdLogInProcess = qdLogInProcess;
            CampaignInteractionDetails = InstanceProvider.GetInstance<ICampaignInteractionDetails>();
            GlobalInteractionDetails = InstanceProvider.GetInstance<IGlobalInteractionDetails>();
        }

        public IDbAccountService DbAccountService { get; set; }
        public ICampaignInteractionDetails CampaignInteractionDetails { get; }
        public IGlobalInteractionDetails GlobalInteractionDetails { get; }
        public ModuleSetting ModuleSetting { get; set; }

        public IQuoraBrowserManager BrowserManager { get; }

        public bool CheckJobCompleted()
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
            LoginBase(QdLogInProcess);
            return DominatorAccountModel.IsUserLoggedIn;
        }

        public int GetEpochTime()
        {
            return (int) (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local)).TotalSeconds;
        }
        protected override bool CloseAutomationBrowser()
        {
            try
            {
                if (BrowserManager != null && BrowserManager.BrowserWindow != null)
                    BrowserManager.CloseBrowser();
            }
            catch { }
            return true;
        }
    }
}