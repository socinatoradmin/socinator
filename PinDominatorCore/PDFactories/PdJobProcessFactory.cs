using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using PinDominatorCore.PDLibrary;
using PinDominatorCore.PDLibrary.DAL;
using Unity;

namespace PinDominatorCore.PDFactories
{
    public class PdJobProcessFactory : IJobProcessFactory
    {
        private readonly IProcessScopeModel _processScopeModel;
        private readonly IUnityContainer _unityContainer;

        public PdJobProcessFactory(IUnityContainer unityContainer, IProcessScopeModel processScopeModel)
        {
            _unityContainer = unityContainer;
            _processScopeModel = processScopeModel;
        }

        public IJobProcess Create(string account, string template, TimingRange currentJobTimeRange, string module,
            SocialNetworks network)
        {
            _unityContainer.RegisterInstance<IDbCampaignService>(new DbCampaignService(_processScopeModel.CampaignId));

            var jp = _unityContainer.Resolve<IJobProcess>($"{SocialNetworks.Pinterest}{module}");
            _unityContainer.RegisterInstance((IPdJobProcess) jp);

            return jp;
        }
    }
}