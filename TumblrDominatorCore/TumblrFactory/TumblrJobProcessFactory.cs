using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using TumblrDominatorCore.TumblrLibrary.DAL;
using TumblrDominatorCore.TumblrLibrary.TumblrProcesses;
using Unity;

namespace TumblrDominatorCore.TumblrFactory
{
    public class TumblrJobProcessFactory : IJobProcessFactory
    {
        private readonly IProcessScopeModel _processScopeModel;
        private readonly IUnityContainer _unityContainer;

        public TumblrJobProcessFactory(IUnityContainer unityContainer, IProcessScopeModel processScopeModel)
        {
            _unityContainer = unityContainer;
            _processScopeModel = processScopeModel;
        }

        public IJobProcess Create(string account, string template, TimingRange currentJobTimeRange, string module,
            SocialNetworks networks)
        {
            if (networks != SocialNetworks.Tumblr)
                return null;

            _unityContainer.RegisterInstance<IDbCampaignService>(new DbCampaignService(_processScopeModel.CampaignId));

            var jp = _unityContainer.Resolve<IJobProcess>($"{SocialNetworks.Tumblr}{module}");
            _unityContainer.RegisterInstance((ITumblrJobProcess)jp);

            return jp;
        }
    }
}