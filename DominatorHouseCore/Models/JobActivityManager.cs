#region

using System.Collections.Generic;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models
{
    [ProtoContract]
    public class JobActivityManager : BindableBase
    {
        private List<ModuleConfiguration> _lstModuleConfiguration = new List<ModuleConfiguration>();

        [ProtoMember(1)]
        public List<ModuleConfiguration> LstModuleConfiguration
        {
            get => _lstModuleConfiguration;
            set
            {
                if (_lstModuleConfiguration == value) return;
                SetProperty(ref _lstModuleConfiguration, value);
            }
        }

        [ProtoMember(2)] public List<RunningTimes> RunningTime { get; set; } = new List<RunningTimes>();

        public void AddOrUpdateModuleConfig(ModuleConfiguration moduleConfiguration)
        {
            var index = FindIndexByActivityType(moduleConfiguration.ActivityType);
            if (index >= 0)
                _lstModuleConfiguration[index] = moduleConfiguration;
            else
                _lstModuleConfiguration.Add(moduleConfiguration);
        }

        public void DeleteModuleConfig(ActivityType activityType)
        {
            var index = FindIndexByActivityType(activityType);
            if (index >= 0) _lstModuleConfiguration.RemoveAt(index);
        }

        private int FindIndexByActivityType(ActivityType activityType)
        {
            return _lstModuleConfiguration.FindIndex(a => a.ActivityType == activityType);
        }
    }
}