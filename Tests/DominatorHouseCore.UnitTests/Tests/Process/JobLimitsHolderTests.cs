using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Utility;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace DominatorHouseCore.UnitTests.Tests.Process
{
    [TestClass]
    public class JobLimitsHolderTests
    {
        private IJobLimitsHolder _sut;

        [TestInitialize]
        public void SetUp()
        {
            _sut = new JobLimitsHolder();
        }

        [TestMethod]
        public void should_reset_limit_for_a_job()
        {
            // arrange
            var jc = Substitute.For<IJobConfiguration>();
            jc.ActivitiesPerJob.Returns(new RangeUtilities(5, 5));
            jc.ActivitiesPerHour.Returns(new RangeUtilities(6, 6));
            jc.ActivitiesPerDay.Returns(new RangeUtilities(7, 7));
            jc.ActivitiesPerWeek.Returns(new RangeUtilities(8, 8));
            var jk = new JobKey("accountId", "templateId");

            // act
            _sut.Reset(jk, jc);

            // assert
            _sut[jk].MaxNoOfActionPerJob.Should().Be(5);
            _sut[jk].MaxNoOfActionPerHour.Should().Be(6);
            _sut[jk].MaxNoOfActionPerDay.Should().Be(7);
            _sut[jk].MaxNoOfActionPerWeek.Should().Be(8);
        }
    }
}
