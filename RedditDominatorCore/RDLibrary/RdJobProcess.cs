using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Utility;
using RedditDominatorCore.RDFactories;
using RedditDominatorCore.RDModel;
using RedditDominatorCore.RDRequest;
using System;

namespace RedditDominatorCore.RDLibrary
{
    public interface IRdJobProcess : IJobProcess
    {
        string CampaignId { get; }
        ModuleSetting ModuleSetting { get; }
        CampaignDetails CampaignDetails { get; }
        bool AddedToDb { get; set; }
        JobProcessResult FinalProcess(ScrapeResultNew scrapeResultNew);
    }

    public abstract class RdJobProcessInteracted<T> : RdJobProcess where T : class, new()
    {
        private readonly IExecutionLimitsManager _executionLimitsManager;

        protected RdJobProcessInteracted(IProcessScopeModel processScopeModel,
            IRdQueryScraperFactory queryScraperFactor, IRedditLogInProcess redditLogInProcess,
            IRdHttpHelper rdHttpHelper) : base(processScopeModel, queryScraperFactor, redditLogInProcess,
            rdHttpHelper)
        {
            _executionLimitsManager = InstanceProvider.GetInstance<IExecutionLimitsManager>();
        }


        public override ReachedLimitInfo CheckLimit()
        {
            return _executionLimitsManager.CheckIfLimitreached<T>(Id, SocialNetworks.Reddit,
                ActivityType);
        }
    }

    public abstract class RdJobProcess : JobProcess, IRdJobProcess
    {
        private readonly IRedditLogInProcess _redditLogInProcess;

        protected RdJobProcess(IProcessScopeModel processScopeModel, IRdQueryScraperFactory queryScraperFactor,
            IRedditLogInProcess redditLogInProcess, IRdHttpHelper rdHttpHelper)
            : base(processScopeModel, queryScraperFactor, rdHttpHelper)
        {
            _redditLogInProcess = redditLogInProcess;
            ModuleSetting = processScopeModel.GetActivitySettingsAs<ModuleSetting>();
        }

        public bool AddedToDb { get; set; }
        public ModuleSetting ModuleSetting { get; set; }

        protected override bool Login()
        {
            try
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                LoginBase(_redditLogInProcess);
                return DominatorAccountModel.IsUserLoggedIn;
            }
            catch (OperationCanceledException ex)
            {
                ex.DebugLog("Operation Cancelled!");
            }
            catch (AggregateException ae)
            {
                ae.HandleOperationCancellation();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;
        }
    }
}