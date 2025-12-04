using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDLibrary.FdFunctions.FdBrowserManager;
using FaceDominatorCore.FDLibrary.FdProcesses;
using Unity;

namespace FaceDominatorCore.FDFactories
{
    public class FdJobProcessFactory : IJobProcessFactory
    {

        private readonly IUnityContainer _unityContainer;
        private readonly IProcessScopeModel _processScopeModel;
        public FdJobProcessFactory(IUnityContainer unityContainer, IProcessScopeModel processScopeModel)
        {
            _unityContainer = unityContainer;
            _processScopeModel = processScopeModel;
        }


        public IJobProcess Create(string account, string template, TimingRange currentJobTimeRange, string module,
            SocialNetworks network)
        {

            //if (!string.IsNullOrWhiteSpace(_processScopeModel.CampaignId))
            //    _unityContainer.RegisterInstance<IDbCampaignService>(null);
            //else
            _unityContainer.RegisterInstance<IDbCampaignServiceScoped>(new DbCampaignServiceScoped(_processScopeModel.CampaignId == null ?
                     string.Empty : _processScopeModel.CampaignId));

            _unityContainer.RegisterInstance<IFdBaseBrowserManger>(new FdBaseBrowserManger());

            var jp = _unityContainer.Resolve<IJobProcess>($"{SocialNetworks.Facebook}{module}");
            _unityContainer.RegisterInstance<IFdJobProcess>((IFdJobProcess)jp);

            return jp;

        }

    }
}
