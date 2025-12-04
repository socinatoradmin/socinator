using System;
using System.Linq;
using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.LDLibrary;
using LinkedDominatorCore.LDLibrary.DAL;
using Unity;

namespace LinkedDominatorCore.Factories
{
    public class LDJobProcessFactory : IJobProcessFactory
    {
        private readonly IProcessScopeModel _processScopeModel;
        private readonly IUnityContainer _unityContainer;

        public LDJobProcessFactory(IUnityContainer unityContainer, IProcessScopeModel processScopeModel)
        {
            _unityContainer = unityContainer;
            _processScopeModel = processScopeModel;
        }

        public IJobProcess Create(string account, string template, TimingRange currentJobTimeRange, string module,
            SocialNetworks network)
        {
            _unityContainer.RegisterInstance<IDbCampaignService>(new DbCampaignService(_processScopeModel.CampaignId));
            try
            {
                var _binFileHelper = InstanceProvider.GetInstance<IBinFileHelper>();
                var accountModel = _binFileHelper.GetAccountDetails()
                    .Where(x => x.UserName == account && x.AccountBaseModel.AccountNetwork == network)
                    ?.FirstOrDefault();
                var factory = _unityContainer.Resolve<ILdFunctionFactory>();
                factory.AssignFunction(accountModel);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            var jp = _unityContainer.Resolve<IJobProcess>($"{SocialNetworks.LinkedIn}{module}");
            _unityContainer.RegisterInstance((ILdJobProcess) jp);
            return jp;
        }
    }
}