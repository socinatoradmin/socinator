using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using TwtDominatorCore.Database;
using TwtDominatorCore.TDLibrary;
using TwtDominatorCore.TDLibrary.GeneralLibrary.DAL;
using Unity;

namespace TwtDominatorCore.Factories
{
    public class TdJobProcessFactory : IJobProcessFactory
    {
        private readonly IProcessScopeModel _processScopeModel;
        private readonly IUnityContainer _unityContainer;

        public TdJobProcessFactory(IUnityContainer unityContainer, IProcessScopeModel processScopeModel)
        {
            _unityContainer = unityContainer;
            _processScopeModel = processScopeModel;
        }

        public IJobProcess Create(string account, string template, TimingRange currentJobTimeRange, string module,
            SocialNetworks network)
        {
            _unityContainer.RegisterInstance<IDbCampaignService>(new DbCampaignService(_processScopeModel.CampaignId));
            _unityContainer.RegisterInstance<IDbInsertionHelper>(_unityContainer.Resolve<DbInsertionHelper>());
            var jp = _unityContainer.Resolve<IJobProcess>($"{SocialNetworks.Twitter}{module}");
            _unityContainer.RegisterInstance((ITdJobProcess) jp);

            return jp;
        }
    }
}