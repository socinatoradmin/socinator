using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using QuoraDominatorCore.QdLibrary;
using QuoraDominatorCore.QdLibrary.DAL;
using Unity;

namespace QuoraDominatorCore.Factories
{
    public class QdJobProcessFactory : IJobProcessFactory
    {
        private readonly IProcessScopeModel _processScopeModel;

        private readonly IUnityContainer _unityContainer;

        public QdJobProcessFactory(IUnityContainer unityContainer, IProcessScopeModel processScopeModel)
        {
            _unityContainer = unityContainer;
            _processScopeModel = processScopeModel;
        }

        public IJobProcess Create(string account, string template, TimingRange currentJobTimeRange, string module,
            SocialNetworks network)
        {
            _unityContainer.RegisterInstance<IDbCampaignService>(new DbCampaignService(_processScopeModel.CampaignId));

            var jp = _unityContainer.Resolve<IJobProcess>($"{SocialNetworks.Quora}{module}");
            _unityContainer.RegisterInstance((IQdJobProcess) jp);

            return jp;
        }
    }
}