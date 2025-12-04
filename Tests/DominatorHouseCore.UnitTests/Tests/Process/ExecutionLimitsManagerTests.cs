using DominatorHouseCore.Enums;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.ExecutionCounters;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Utility;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace DominatorHouseCore.UnitTests.Tests.Process
{
    [TestClass]
    public class ExecutionLimitsManagerTests
    {
        private IExecutionLimitsManager _sut;
        private IEntityCountersManager _entityCountersManager;
        private IJobLimitsHolder _jobLimitsHolder;
        private IJobCountersManager _jobCountersManager;

        [TestInitialize]
        public void SetUp()
        {
            _entityCountersManager = Substitute.For<IEntityCountersManager>();
            _jobLimitsHolder = Substitute.For<IJobLimitsHolder>();
            _jobCountersManager = Substitute.For<IJobCountersManager>();
            _sut = new ExecutionLimitsManager(_entityCountersManager, _jobLimitsHolder, _jobCountersManager);
        }

        [TestMethod]
        public void should_return_NoLimit_if_none_of_max_values_are_reached()
        {
            // arrange
            var jk = new JobKey("accountid", "templateId");
            _jobCountersManager[jk].Returns(0);
            _entityCountersManager.GetCounter<DummyEntity>(jk.AccountId, SocialNetworks.Twitter, Arg.Any<ActivityType>())
                .Returns(new EntityCounter(0, 0, 0));
            _jobLimitsHolder[jk].Returns(new JobLimits(1, 1, 1, 1));

            // act
            var result = _sut.CheckIfLimitreached<DummyEntity>(jk, SocialNetworks.Twitter, ActivityType.Delete);

            // assert
            result.ReachedLimitType.Should().Be(ReachedLimitType.NoLimit);
            result.LimitValue.Should().Be(0);
        }

        [TestMethod]
        public void should_return_Weekly_if_week_max_value_exceeded()
        {
            // arrange
            var jk = new JobKey("accountid", "templateId");
            _jobCountersManager[jk].Returns(0);
            _entityCountersManager.GetCounter<DummyEntity>(jk.AccountId, SocialNetworks.Twitter, Arg.Any<ActivityType>())
                .Returns(new EntityCounter(2, 0, 0));
            _jobLimitsHolder[jk].Returns(new JobLimits(1, 1, 1, 1));

            // act
            var result = _sut.CheckIfLimitreached<DummyEntity>(jk, SocialNetworks.Twitter, ActivityType.Delete);

            // assert
            result.ReachedLimitType.Should().Be(ReachedLimitType.Weekly);
            result.LimitValue.Should().Be(1);
        }

        [TestMethod]
        public void should_return_Daily_if_day_max_value_exceeded()
        {
            // arrange
            var jk = new JobKey("accountid", "templateId");
            _jobCountersManager[jk].Returns(0);
            _entityCountersManager.GetCounter<DummyEntity>(jk.AccountId, SocialNetworks.Twitter, Arg.Any<ActivityType>())
                .Returns(new EntityCounter(0, 2, 0));
            _jobLimitsHolder[jk].Returns(new JobLimits(1, 1, 1, 1));

            // act
            var result = _sut.CheckIfLimitreached<DummyEntity>(jk, SocialNetworks.Twitter, ActivityType.Delete);

            // assert
            result.ReachedLimitType.Should().Be(ReachedLimitType.Daily);
            result.LimitValue.Should().Be(1);
        }

        [TestMethod]
        public void should_return_Hourly_if_hour_max_value_exceeded()
        {
            // arrange
            var jk = new JobKey("accountid", "templateId");
            _jobCountersManager[jk].Returns(0);
            _entityCountersManager.GetCounter<DummyEntity>(jk.AccountId, SocialNetworks.Twitter, Arg.Any<ActivityType>())
                .Returns(new EntityCounter(0, 0, 2));
            _jobLimitsHolder[jk].Returns(new JobLimits(1, 1, 1, 1));

            // act
            var result = _sut.CheckIfLimitreached<DummyEntity>(jk, SocialNetworks.Twitter, ActivityType.Delete);

            // assert
            result.ReachedLimitType.Should().Be(ReachedLimitType.Hourly);
            result.LimitValue.Should().Be(1);
        }

        private class DummyEntity
        {
            public int Id { get; set; }
            public int InteractionDate { get; set; }
        }
    }
}
