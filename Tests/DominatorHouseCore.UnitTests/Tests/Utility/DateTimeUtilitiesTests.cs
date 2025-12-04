using Dominator.Tests.Utils;
using DominatorHouseCore.Utility;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using Unity;

namespace DominatorHouseCore.UnitTests.Tests.Utility
{
    [TestClass]
    public class DateTimeUtilitiesTests : UnityInitializationTests
    {
        private IDateProvider _dateProvider;

        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();
            _dateProvider = Substitute.For<IDateProvider>();
            Container.RegisterInstance<IDateProvider>(_dateProvider);
        }

        [TestMethod]
        public void should_current_epoch_time()
        {
            // arrange
            var currentDate = new DateTime(2018, 01, 01, 23, 22, 33, DateTimeKind.Utc);

            // act
            var result = currentDate.GetCurrentEpochTime();

            // assert
            result.Should().Be(1514848953);
        }
    }
}
