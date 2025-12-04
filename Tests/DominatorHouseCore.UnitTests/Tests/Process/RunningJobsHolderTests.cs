using DominatorHouseCore.Process;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace DominatorHouseCore.UnitTests.Tests.Process
{
    [TestClass]
    public class RunningJobsHolderTests
    {
        private IRunningJobsHolder _sut;

        [TestInitialize]
        public void SetUp()
        {
            _sut = new RunningJobsHolder();
        }

        [TestMethod]
        public void StartIfNotRunning_should_TRUE_if_NOT_running()
        {
            // arrange
            var accountId = "accountId";
            var templateId = "templateId";
            var jp = Substitute.For<IJobProcess>();

            // act
            var result = _sut.StartIfNotRunning(new JobKey(accountId, templateId), jp);

            // assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void StartIfNotRunning_should_FALSE_if_running()
        {
            // arrange
            var accountId = "accountId";
            var templateId = "templateId";
            var jp = Substitute.For<IJobProcess>();
            _sut.StartIfNotRunning(new JobKey(accountId, templateId), jp);

            // act
            var result = _sut.StartIfNotRunning(new JobKey(accountId, templateId), jp);

            // assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsRunning_should_return_TRUE_if_job_running()
        {
            // arrange
            var accountId = "accountId";
            var templateId = "templateId";
            var jp = Substitute.For<IJobProcess>();
            _sut.StartIfNotRunning(new JobKey(accountId, templateId), jp);

            // act
            var result = _sut.IsRunning(new JobKey(accountId, templateId));

            // assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void IsRunning_should_return_FALSE_if_job_NOT_running()
        {
            // arrange
            var accountId = "accountId";
            var templateId = "templateId";

            // act
            var result = _sut.IsRunning(new JobKey(accountId, templateId));

            // assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsActivityRunningForAccount_should_return_FALSE_if_nothing_running_for_this_account()
        {
            // arrange
            var accountId = "accountId";

            // act
            var result = _sut.IsActivityRunningForAccount(accountId);

            // assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsActivityRunningForAccount_should_return_TRUE_if_some_job_is_running_for_this_account()
        {
            // arrange
            var accountId = "accountId";
            var templateId = "templateId";
            var jp = Substitute.For<IJobProcess>();
            _sut.StartIfNotRunning(new JobKey(accountId, templateId), jp);

            // act
            var result = _sut.IsActivityRunningForAccount(accountId);

            // assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void should_STOP_running_job()
        {
            // arrange
            var accountId = "accountId";
            var templateId = "templateId";
            var jp = Substitute.For<IJobProcess>();
            var id = new JobKey(accountId, templateId);
            _sut.StartIfNotRunning(id, jp);

            // act
            var result = _sut.Stop(new JobKey(accountId, templateId));

            // assert
            result.Should().BeTrue();
            _sut.IsRunning(id).Should().Be(false);
            _sut.IsActivityRunningForAccount(id.AccountId).Should().Be(false);
            jp.Received(1).Stop();
        }
    }
}
