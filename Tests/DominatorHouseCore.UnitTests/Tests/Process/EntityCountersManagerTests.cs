using Dominator.Tests.Utils;
using DominatorHouseCore.DatabaseHandler.Common.EntityCounters;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Process.ExecutionCounters;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Unity;

namespace DominatorHouseCore.UnitTests.Tests.Process
{
    [TestClass]
    public class EntityCountersManagerTests : UnityInitializationTests

    {
        private IEntityCountersManager _sut;

        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();
            _sut = new EntityCountersManager();
        }

        [TestMethod]
        public void should_init_and_return_count()
        {
            // arrange
            var accountId = "accountId";
            var keyFactory = Substitute.For<ICounterKeyFactory<DummyEntity>>();
            keyFactory.Create(accountId, ActivityType.AnswerOnQuestions).Returns("a key");
            Container.RegisterInstance<ICounterKeyFactory<DummyEntity>>(keyFactory);
            var counterFunction = Substitute.For<IEntityCounterFunction<DummyEntity>>();
            var cnt = new EntityCounter(3, 3, 3);
            counterFunction.GetCounter(accountId, SocialNetworks.Twitter, ActivityType.AnswerOnQuestions).Returns(cnt);
            Container.RegisterInstance<IEntityCounterFunction<DummyEntity>>(counterFunction);

            // act
            var result = _sut.GetCounter<DummyEntity>(accountId, SocialNetworks.Twitter, ActivityType.AnswerOnQuestions);

            // assert
            result.Should().Be(cnt);
        }

        [TestMethod]
        public void shold_increment_and_return()
        {
            // arrange
            var accountId = "accountId";
            var keyFactory = Substitute.For<ICounterKeyFactory<DummyEntity>>();
            keyFactory.Create(accountId, ActivityType.AnswerOnQuestions).Returns("a key");
            Container.RegisterInstance<ICounterKeyFactory<DummyEntity>>(keyFactory);
            var counterFunction = Substitute.For<IEntityCounterFunction<DummyEntity>>();
            counterFunction.GetCounter(accountId, SocialNetworks.Twitter, ActivityType.AnswerOnQuestions).Returns(new EntityCounter(3, 3, 3));
            Container.RegisterInstance<IEntityCounterFunction<DummyEntity>>(counterFunction);

            // act
            _sut.IncrementFor<DummyEntity>(accountId, SocialNetworks.Twitter, ActivityType.AnswerOnQuestions);
            var cnt = _sut.GetCounter<DummyEntity>(accountId, SocialNetworks.Twitter, ActivityType.AnswerOnQuestions);

            // assert
            cnt.NoOfActionPerformedCurrentDay.Should().Be(4);
            cnt.NoOfActionPerformedCurrentHour.Should().Be(4);
            cnt.NoOfActionPerformedCurrentWeek.Should().Be(4);
        }

        public class DummyEntity
        {
            public int Id { get; set; }
            public int InteractionDate { get; set; }
        }
    }
}
