using System;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Enums;
using GramDominatorCore.GDLibrary;
using DominatorHouseCore.BusinessLogic.Scheduler;
using Unity;
using GramDominatorCore.GDLibrary.DAL;

namespace GramDominatorCore.Factories
{
    /// <summary>
    /// Creates respective job process by activity type for Instagram
    /// </summary>
    public class GdJobProcessFactory : IJobProcessFactory
    {

        private readonly IUnityContainer _unityContainer;
        private readonly IProcessScopeModel _processScopeModel;
        public GdJobProcessFactory(IUnityContainer unityContainer, IProcessScopeModel processScopeModel)
        {
            _unityContainer = unityContainer;
            _processScopeModel = processScopeModel;
        }
        public IJobProcess Create(string account, string template, TimingRange currentJobTimeRange, string module,
            SocialNetworks network)
        {
            try
            {
                _unityContainer.RegisterInstance<IDbCampaignService>(new DbCampaignService(_processScopeModel.CampaignId));
            }
            catch (Exception)
            {
                //ignored
            }
             var jp = _unityContainer.Resolve<IJobProcess>($"{SocialNetworks.Instagram}{module}");
            _unityContainer.RegisterInstance<IGdJobProcess>((IGdJobProcess)jp);

            return jp;
        }
    }
}