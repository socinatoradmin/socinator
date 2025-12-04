#region

using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Process.ExecutionCounters;
using DominatorHouseCore.Process.JobConfigurations;
using DominatorHouseCore.Process.JobLimits;
using Unity;
using Unity.Extension;

#endregion

namespace DominatorHouseCore.Process
{
    internal class JobProcessUnityExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Container.RegisterSingleton<IEntityCountersManager, EntityCountersManager>();
            Container.RegisterSingleton<IJobCountersManager, JobCountersManager>();
            Container.RegisterSingleton<IExecutionLimitsManager, ExecutionLimitsManager>();
            Container.RegisterSingleton<IJobLimitsHolder, JobLimitsHolder>();
            Container.RegisterSingleton<IJobConfigurationProvider, JobConfigurationProvider>();
            Container.RegisterSingleton<IRunningActivityManager, RunningActivityManager>();
            Container.RegisterSingleton<IJobProcessScopeFactory, JobProcessScopeFactory>();
        }
    }
}