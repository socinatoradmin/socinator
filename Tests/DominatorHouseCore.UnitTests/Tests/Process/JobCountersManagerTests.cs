using DominatorHouseCore.Process;
using DominatorHouseCore.Process.ExecutionCounters;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DominatorHouseCore.UnitTests.Tests.Process
{
    [TestClass]
    public class JobCountersManagerTests
    {
        private IJobCountersManager _sut;

        [TestInitialize]
        public void SetUp()
        {
            _sut = new JobCountersManager();
        }

        [TestMethod]
        public void should_init_value_if_not_exists()
        {
            // arrange
            var jk = new JobKey("accountId", "tempalteId");

            // act
            _sut.InitOrIncrement(jk);

            // assert
            _sut[jk].Should().Be(1);
        }

        [TestMethod]
        public void should_increment_value()
        {
            // arrange
            var jk = new JobKey("accountId", "tempalteId");
            _sut.InitOrIncrement(jk);

            // act
            _sut.InitOrIncrement(jk);

            // assert
            _sut[jk].Should().Be(2);
        }

        [TestMethod]
        public void should_reset_value()
        {
            // arrange
            var jk = new JobKey("accountId", "tempalteId");
            _sut.InitOrIncrement(jk);
            _sut.InitOrIncrement(jk);

            // act
            _sut.Reset(jk);

            // assert
            _sut[jk].Should().Be(0);
        }
    }
}
