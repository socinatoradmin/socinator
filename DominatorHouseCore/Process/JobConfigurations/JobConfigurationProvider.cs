#region

using System.Collections.Generic;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using Newtonsoft.Json.Linq;

#endregion

namespace DominatorHouseCore.Process.JobConfigurations
{
    public interface IJobConfigurationProvider
    {
        CommonJobConfiguration GetJobConfiguration(string accountId, ActivityType activityType);
    }

    public class JobConfigurationProvider : IJobConfigurationProvider
    {
        private readonly IJobActivityConfigurationManager _jobActivityConfigurationManager;
        private readonly ITemplatesFileManager _templatesFileManager;

        public JobConfigurationProvider(IJobActivityConfigurationManager jobActivityConfigurationManager,
            ITemplatesFileManager templatesFileManager)
        {
            _jobActivityConfigurationManager = jobActivityConfigurationManager;
            _templatesFileManager = templatesFileManager;
        }

        public CommonJobConfiguration GetJobConfiguration(string accountId, ActivityType activityType)
        {
            var moduleConfiguration = _jobActivityConfigurationManager[accountId, activityType];
            var model = _templatesFileManager.GetTemplateById(moduleConfiguration.TemplateId);
            var jsonObject = JObject.Parse(model.ActivitySettings);
            var isNeedToSchedule = jsonObject["IsNeedToStart"]?.ToObject<bool>() ?? false;
            var jobConfiguration = jsonObject["JobConfiguration"]?.ToObject<JobConfiguration>();
            List<QueryInfo> savedQueries;
            try
            {
                savedQueries = jsonObject["SavedQueries"]?.ToObject<List<QueryInfo>>() ?? new List<QueryInfo>();
            }
            catch
            {
                savedQueries = new List<QueryInfo>();
            }

            return new CommonJobConfiguration(jobConfiguration, savedQueries, isNeedToSchedule);
        }
    }
}