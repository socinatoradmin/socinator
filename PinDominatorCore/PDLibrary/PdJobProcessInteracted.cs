using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process.JobLimits;
using PinDominatorCore.PDFactories;
using PinDominatorCore.PDLibrary.DAL;
using PinDominatorCore.PDUtility;
using PinDominatorCore.Request;

namespace PinDominatorCore.PDLibrary
{
    public abstract class PdJobProcessInteracted<T> : PdJobProcess where T : class, new()
    {
        private readonly IExecutionLimitsManager _executionLimitsManager;

        protected PdJobProcessInteracted(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IDbGlobalService globalService,
            IExecutionLimitsManager executionLimitsManager, IPdQueryScraperFactory queryScraperFactory,
            IPdHttpHelper qdHttpHelper,
            IPdLogInProcess qdLogInProcess) : base(
            processScopeModel, accountServiceScoped, globalService, queryScraperFactory, qdHttpHelper, qdLogInProcess)
        {
            _executionLimitsManager = executionLimitsManager;
        }

        public override ReachedLimitInfo CheckLimit()
        {
            return _executionLimitsManager.CheckIfLimitreached<T>(Id, SocialNetworks.Pinterest,
                ActivityType);
        }

        protected string GetMessageWithUserName(string message, string userName, string fullName) =>
       message.Replace(Macros.UserName, userName).Replace(Macros.AccountUserName, DominatorAccountModel.AccountBaseModel.ProfileId)
           .Replace(Macros.FullName, fullName);
    }
}