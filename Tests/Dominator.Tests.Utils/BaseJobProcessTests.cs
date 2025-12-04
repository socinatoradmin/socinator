using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.ExecutionCounters;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Unity;

namespace Dominator.Tests.Utils
{
    [TestClass]
    public class BaseJobProcessTests : UnityInitializationTests
    {
        protected IRunningJobsHolder RunningJobsHolder;
        protected IJobCountersManager JobCountersManager;
        protected IDominatorScheduler DominatorScheduler;
        protected IExecutionLimitsManager ExecutionLimitsManager;

        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();

            DominatorScheduler = Substitute.For<IDominatorScheduler>();
            this.Container.RegisterInstance<IDominatorScheduler>(DominatorScheduler);

            RunningJobsHolder = Substitute.For<IRunningJobsHolder>();
            this.Container.RegisterInstance<IRunningJobsHolder>(RunningJobsHolder);

            JobCountersManager = Substitute.For<IJobCountersManager>();
            this.Container.RegisterInstance<IJobCountersManager>(JobCountersManager);

            ExecutionLimitsManager = Substitute.For<IExecutionLimitsManager>();
            this.Container.RegisterInstance<IExecutionLimitsManager>(ExecutionLimitsManager);

            this.Container.RegisterInstance<IDateProvider>(Substitute.For<IDateProvider>());

        }
    }
}
