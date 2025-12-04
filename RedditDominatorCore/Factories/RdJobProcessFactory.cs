using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using RedditDominatorCore.RDLibrary;
using RedditDominatorCore.RDLibrary.DAL;
using Unity;

namespace RedditDominatorCore.Factories
{
    public class RdJobProcessFactory : IJobProcessFactory
    {
        private readonly IProcessScopeModel _processScopeModel;
        private readonly IUnityContainer _unityContainer;

        public RdJobProcessFactory(IUnityContainer unityContainer, IProcessScopeModel processScopeModel)
        {
            _unityContainer = unityContainer;
            _processScopeModel = processScopeModel;
        }

        public IJobProcess Create(string account, string template, TimingRange currentJobTimeRange, string module,
            SocialNetworks networks)
        {
            _unityContainer.RegisterInstance<IDbCampaignService>(new DbCampaignService(_processScopeModel.CampaignId));
            var jp = _unityContainer.Resolve<IJobProcess>($"{SocialNetworks.Reddit}{module}");
            _unityContainer.RegisterInstance((IRdJobProcess)jp);
            return jp;
        }
    }
}