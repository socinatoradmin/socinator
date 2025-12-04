using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using System;
using Unity;
using YoutubeDominatorCore.YoutubeLibrary.DAL;
using YoutubeDominatorCore.YoutubeLibrary.Processes;

namespace YoutubeDominatorCore.YDFactories
{
    public class YdJobProcessFactory : IJobProcessFactory
    {
        private readonly IProcessScopeModel _processScopeModel;
        private readonly IUnityContainer _unityContainer;

        public YdJobProcessFactory(IUnityContainer unityContainer, IProcessScopeModel processScopeModel)
        {
            _unityContainer = unityContainer;
            _processScopeModel = processScopeModel;
        }

        public IJobProcess Create(string account, string template, TimingRange currentJobTimeRange, string module,
            SocialNetworks network)
        {
            try
            {
                _unityContainer.RegisterInstance<IDbCampaignService>(
                    new DbCampaignService(_processScopeModel.CampaignId));
            }
            catch (Exception)
            {
            }

            var jp = _unityContainer.Resolve<IJobProcess>($"{SocialNetworks.YouTube}{module}");
            _unityContainer.RegisterInstance((IYdJobProcess)jp);

            return jp;
        }
    }
}