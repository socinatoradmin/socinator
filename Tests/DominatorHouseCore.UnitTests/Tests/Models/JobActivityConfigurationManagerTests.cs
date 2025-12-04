using Dominator.Tests.Utils;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;
using Unity;

namespace DominatorHouseCore.UnitTests.Tests.Models
{
    [TestClass]
    public class JobActivityConfigurationManagerTests : UnityInitializationTests
    {
        private IJobActivityConfigurationManager _sut;
        private IAccountsCacheService _accountsCacheService;

        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();
            _accountsCacheService = Substitute.For<IAccountsCacheService>();
            _sut = new JobActivityConfigurationManager(_accountsCacheService);
            Container.RegisterType<IDateProvider, DateProvider>();
        }

        [TestMethod]
        public void should_add_new_item_in_cache_and_account()
        {
            // arrange
            var config = new ModuleConfiguration(); ;
            var accountId = "123";
            var activityType = ActivityType.Delete;
            var account = new DominatorAccountModel();
            _accountsCacheService[accountId].Returns(account);

            // act
            _sut.AddOrUpdate(accountId, activityType, config);

            // assert
            _sut[accountId, activityType].Should().Be(config);
            account.ActivityManager.LstModuleConfiguration.Single().Should().Be(config);
        }

        [TestMethod]
        public void should_update_in_cache_and_account()
        {
            // arrange
            var config = new ModuleConfiguration() { ActivityType = ActivityType.Delete };
            var config1 = new ModuleConfiguration() { ActivityType = ActivityType.Delete }; ;
            var accountId = "123";
            var activityType = ActivityType.Delete;
            var account = new DominatorAccountModel();
            _accountsCacheService[accountId].Returns(account);
            _sut.AddOrUpdate(accountId, activityType, config);

            // act
            _sut.AddOrUpdate(accountId, activityType, config1);

            // assert
            _sut[accountId, activityType].Should().Be(config1);
            account.ActivityManager.LstModuleConfiguration.Single().Should().Be(config1);
        }

        [TestMethod]
        public void should_delete_in_cache_and_account()
        {
            // arrange
            var activityType = ActivityType.Delete;
            var config = new ModuleConfiguration() { ActivityType = activityType };
            var accountId = "123";
            var account = new DominatorAccountModel();
            _accountsCacheService[accountId].Returns(account);
            _sut.AddOrUpdate(accountId, activityType, config);

            // act
            _sut.Delete(accountId, activityType);

            // assert
            _sut[accountId, activityType].Should().BeNull();
            account.ActivityManager.LstModuleConfiguration.Should().BeEmpty();
        }

        [TestMethod]
        public void should_return_empty_collection_if_such_account()
        {
            // arrange
            var accountId = "123";
            var account = new DominatorAccountModel();
            _accountsCacheService[accountId].Returns(account);

            // act
            var result = _sut[accountId];

            // assert
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void should_return_collection__by_accountId()
        {
            // arrange
            var config = new ModuleConfiguration(); ;
            var accountId = "123";
            var activityType = ActivityType.Delete;
            var account = new DominatorAccountModel();
            _accountsCacheService[accountId].Returns(account);
            _sut.AddOrUpdate(accountId, activityType, config);

            // act
            var result = _sut[accountId];

            // assert
            result.Count.Should().Be(1);
            result.Single().Should().Be(config);
        }

        [TestMethod]
        public void should_return_enabled_accounts_initialized_LstRunningTimes()
        {
            // arrange
            var enabledConfig = new ModuleConfiguration
            {
                IsEnabled = true,
                LstRunningTimes = new List<RunningTimes>()

            };
            _accountsCacheService["1"].Returns(new DominatorAccountModel());
            _accountsCacheService["2"].Returns(new DominatorAccountModel());
            _accountsCacheService["3"].Returns(new DominatorAccountModel());
            _sut.AddOrUpdate("1", ActivityType.Delete, enabledConfig);
            _sut.AddOrUpdate("2", ActivityType.AcceptConnectionRequest, new ModuleConfiguration { IsEnabled = true, LstRunningTimes = null });
            _sut.AddOrUpdate("3", ActivityType.AnswersScraper, new ModuleConfiguration
            {
                IsEnabled = false,
                LstRunningTimes = new List<RunningTimes>()
            });

            // act
            var result = _sut.AllEnabled();

            // assert
            result.Count.Should().Be(1);
            result.Single().Should().Be(enabledConfig);
        }
    }
}
