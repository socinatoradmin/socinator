#region

using System.Linq;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process.JobConfigurations;
using Unity;

#endregion

namespace DominatorHouseCore.BusinessLogic.Scheduler
{
    public interface IJobProcessScopeFactory
    {
        IUnityContainer GetScope(DominatorAccountModel account, ActivityType activityType, string templateId,
            TimingRange timingRange, SocialNetworks module);
    }

    public class JobProcessScopeFactory : IJobProcessScopeFactory
    {
        private readonly IUnityContainer _unityContainer;
        private readonly ICampaignsFileManager _campaignsFileManager;
        private readonly IJobConfigurationProvider _jobConfigurationProvider;
        private readonly ITemplatesFileManager _templatesFileManager;

        public JobProcessScopeFactory(IUnityContainer unityContainer, ICampaignsFileManager campaignsFileManager,
            IJobConfigurationProvider jobConfigurationProvider, ITemplatesFileManager templatesFileManager)
        {
            _unityContainer = unityContainer;
            _campaignsFileManager = campaignsFileManager;
            _jobConfigurationProvider = jobConfigurationProvider;
            _templatesFileManager = templatesFileManager;
        }

        public IUnityContainer GetScope(DominatorAccountModel account, ActivityType activityType, string templateId,
            TimingRange timingRange, SocialNetworks module)
        {
            var scope = _unityContainer.CreateChildContainer();
            var commonConfiguration =
                _jobConfigurationProvider.GetJobConfiguration(account.AccountId, activityType);

            var campaignDetails = _campaignsFileManager.FirstOrDefault(x => x.TemplateId == templateId);
            var templateModel = _templatesFileManager[templateId];

            scope.RegisterInstance<IProcessScopeModel>(new ProcessScopeModel(account, activityType, timingRange,
                templateId, module, campaignDetails, campaignDetails?.CampaignId, commonConfiguration,
                templateModel.ActivitySettings));
            return scope;
        }
    }
}