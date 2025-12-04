using Dominator.Tests.Utils;
using DominatorHouseCore.DatabaseHandler.Common.EntityCounters;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using NSubstitute.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Unity;

namespace DominatorHouseCore.UnitTests.Tests.Process
{
    [TestClass]
    public class EntityCounterFunctionTests : UnityInitializationTests
    {
        private IDbOperations _dbOperations;
        private IDateProvider _dateProvider;
        private string _accountId;
        private SocialNetworks _socialNetworks;

        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();
            _dbOperations = Substitute.For<IDbOperations>();
            _dateProvider = Substitute.For<IDateProvider>();
            _accountId = Guid.NewGuid().ToString();
            _socialNetworks = SocialNetworks.Twitter;
            Container.RegisterInstance(_dbOperations);
            Container.RegisterInstance(_dateProvider);
        }

        [TestMethod]
        public void should_filter_entities_accourding_to_date_predicate()
        {
            // arrange
            var predicate = new DateEpochFilterPredicate<DummyEntity>(a => a.InteractedDate);
            var sut = new EntityCounterFunction<DummyEntity>(predicate);
            var localTime = new DateTime(2018, 12, 26, 19, 22, 33, DateTimeKind.Local); // UTC +4

            _dateProvider.Now().Returns(localTime);

            var list = new List<DummyEntity>
            {
                new DummyEntity {InteractedDate = localTime.AddMinutes(-21).ConvertToEpoch()}, // should go into hour day and week
                new DummyEntity {InteractedDate = localTime.AddHours(-8).ConvertToEpoch()}, // should go into day and week
                new DummyEntity {InteractedDate = localTime.AddDays(-1).ConvertToEpoch()}, // should go into week
                new DummyEntity {InteractedDate = localTime.AddDays(-8).ConvertToEpoch()}
            };

            _dbOperations.Count(Arg.Any<Expression<Func<DummyEntity, bool>>>()).Returns((CallInfo info) =>
            {
                var expressio = info.Arg<Expression<Func<DummyEntity, bool>>>();
                return list.Count(expressio.Compile());
            });

            // act
            var result = sut.GetCounter(_accountId, _socialNetworks, null);

            // assert
            result.NoOfActionPerformedCurrentHour.Should().Be(1);
            result.NoOfActionPerformedCurrentDay.Should().Be(2);
            result.NoOfActionPerformedCurrentWeek.Should().Be(3);
        }

        [TestMethod]
        public void should_filter_entities_accourding_to_date_and_activity_predicates()
        {
            // arrange
            var predicate = new DateEpochFilterPredicate<DummyEntity>(a => a.InteractedDate);
            var activityPredicate = new ActivityTypeFilterPredicate<DummyEntity, ActivityType>(a => a.ActivityType, type => type);
            var sut = new EntityCounterFunction<DummyEntity>(predicate, activityPredicate);
            var utcNow = new DateTime(2018, 12, 26, 19, 22, 33, DateTimeKind.Local); // UTC +4

            _dateProvider.Now().Returns(utcNow);

            var list = new List<DummyEntity>
            {
                new DummyEntity {InteractedDate = utcNow.AddMinutes(-21).ConvertToEpoch(), ActivityType = ActivityType.BlockFollower}, // should go into hour day and week
                new DummyEntity {InteractedDate = utcNow.AddHours(-8).ConvertToEpoch(), ActivityType = ActivityType.BlockFollower}, // should go into day and week
                new DummyEntity {InteractedDate = utcNow.AddDays(-1).ConvertToEpoch(), ActivityType = ActivityType.BlockFollower}, // should go into week
                new DummyEntity {InteractedDate = utcNow.AddDays(-8).ConvertToEpoch(), ActivityType = ActivityType.BlockFollower},

                // should be counted because of activity type
                new DummyEntity {InteractedDate = utcNow.AddMinutes(-21).ConvertToEpoch(), ActivityType = ActivityType.Delete},
                new DummyEntity {InteractedDate = utcNow.AddHours(-8).ConvertToEpoch(), ActivityType = ActivityType.AnswersScraper},
                new DummyEntity {InteractedDate = utcNow.AddDays(-1).ConvertToEpoch(), ActivityType = ActivityType.AutoReplyToNewMessage},
                new DummyEntity {InteractedDate = utcNow.AddDays(-8).ConvertToEpoch(), ActivityType = ActivityType.BoardScraper}
            };

            _dbOperations.Count(Arg.Any<Expression<Func<DummyEntity, bool>>>()).Returns((CallInfo info) =>
            {
                var expressio = info.Arg<Expression<Func<DummyEntity, bool>>>();
                return list.Count(a => expressio.Compile()(a));
            });

            // act
            var result = sut.GetCounter(_accountId, _socialNetworks, ActivityType.BlockFollower);

            // assert
            result.NoOfActionPerformedCurrentHour.Should().Be(1);
            result.NoOfActionPerformedCurrentDay.Should().Be(2);
            result.NoOfActionPerformedCurrentWeek.Should().Be(3);
        }


        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void should_throw_exception_if_activity_type_is_NOT_provided_but_predicate_is_set()
        {
            // arrange
            var predicate = new DateEpochFilterPredicate<DummyEntity>(a => a.InteractedDate);
            var activityPredicate = new ActivityTypeFilterPredicate<DummyEntity, ActivityType>(a => a.ActivityType, type => type);
            var sut = new EntityCounterFunction<DummyEntity>(predicate, activityPredicate);
            var utcNow = new DateTime(2018, 12, 26, 19, 22, 33, DateTimeKind.Local); // UTC +4

            _dateProvider.Now().Returns(utcNow);

            // act
            sut.GetCounter(_accountId, _socialNetworks, null);

            // assert
            // exception is thrown
        }

        private class DummyEntity
        {
            public int InteractedDate { get; set; }
            public ActivityType ActivityType { get; set; }
        }
    }
}
