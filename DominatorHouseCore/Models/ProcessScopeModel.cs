#region

using System.Collections.Generic;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Process.JobConfigurations;
using Newtonsoft.Json;

#endregion

namespace DominatorHouseCore.Models
{
    public interface IProcessScopeModel
    {
        DominatorAccountModel Account { get; }
        ActivityType ActivityType { get; }
        TimingRange TimingRange { get; }
        string TemplateId { get; }
        SocialNetworks Network { get; }
        CampaignDetails CampaignDetails { get; }
        string CampaignId { get; }
        bool IsNeedToSchedule { get; }
        JobConfiguration JobConfiguration { get; }
        List<QueryInfo> SavedQueries { get; }

        T GetActivitySettingsAs<T>();
    }

    public class ProcessScopeModel : IProcessScopeModel
    {
        private readonly string _activitySettings;
        public DominatorAccountModel Account { get; }
        public ActivityType ActivityType { get; }
        public TimingRange TimingRange { get; }
        public string TemplateId { get; }
        public SocialNetworks Network { get; }
        public CampaignDetails CampaignDetails { get; }
        public string CampaignId { get; }
        public bool IsNeedToSchedule { get; }
        public JobConfiguration JobConfiguration { get; }
        public List<QueryInfo> SavedQueries { get; }

        public T GetActivitySettingsAs<T>()
        {
            return JsonConvert.DeserializeObject<T>(_activitySettings);
        }

        public ProcessScopeModel(DominatorAccountModel account, ActivityType activityType, TimingRange timingRange,
            string templateId, SocialNetworks network, CampaignDetails campaignDetails, string campaignId,
            CommonJobConfiguration commonJobConfiguration, string activitySettings)
        {
            Account = account;
            ActivityType = activityType;
            TimingRange = timingRange;
            TemplateId = templateId;
            Network = network;
            CampaignDetails = campaignDetails;
            CampaignId = campaignId;
            _activitySettings = activitySettings;

            IsNeedToSchedule = commonJobConfiguration.IsNeedToSchedule;
            JobConfiguration = commonJobConfiguration.JobConfiguration;
            SavedQueries = commonJobConfiguration.SavedQueries;
        }
    }
}