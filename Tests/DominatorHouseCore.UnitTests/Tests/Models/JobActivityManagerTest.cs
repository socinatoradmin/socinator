using DominatorHouseCore.Utility;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DominatorHouseCore.Models;
using Dominator.Tests.Utils;
using Unity;
using DominatorHouseCore.Enums;
using System;

namespace DominatorHouseCore.UnitTests.Tests.Models
{
    [TestClass]
    public class JobActivityManagerTest : UnityInitializationTests
    {
        JobActivityManager JobActivityManager;
        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();
            JobActivityManager = new JobActivityManager();
            Container.RegisterType<IDateProvider, DateProvider>();
        }
        [TestMethod]
        public void AddOrUpdateModuleConfig_method_should_add_module_configuration_if_not_exist()
        {
            var moduleConfig = new ModuleConfiguration
            {
                ActivityType = ActivityType.Follow
            };
            JobActivityManager.AddOrUpdateModuleConfig(moduleConfig);
            JobActivityManager.LstModuleConfiguration.Count.Should().Be(1);
        }
        [TestMethod]
        public void AddOrUpdateModuleConfig_method_should_update_module_configuration_if_exist()
        {
            var moduleConfig = new ModuleConfiguration
            {
                ActivityType = ActivityType.Follow
            };
            JobActivityManager.LstModuleConfiguration.Add(moduleConfig);
            
            //before AddOrUpdateModuleConfig method called
            JobActivityManager.LstModuleConfiguration[0].IsEnabled.Should().BeFalse();
            JobActivityManager.LstModuleConfiguration[0].MaximumCountPerDay.Should().Be(30);

            moduleConfig.IsEnabled = true;
            moduleConfig.MaximumCountPerDay = 10;

            JobActivityManager.AddOrUpdateModuleConfig(moduleConfig);
           
            //after AddOrUpdateModuleConfig method called
            JobActivityManager.LstModuleConfiguration.Count.Should().Be(1);
            JobActivityManager.LstModuleConfiguration[0].IsEnabled.Should().BeTrue();
            JobActivityManager.LstModuleConfiguration[0].MaximumCountPerDay.Should().Be(10);
        }
        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void AddOrUpdateModuleConfig_method_should_throw_NullReferenceException_if_moduleConfig_is_null()
        {
            ModuleConfiguration moduleConfig = null;
            JobActivityManager.LstModuleConfiguration.Count.Should().Be(0);

            JobActivityManager.AddOrUpdateModuleConfig(moduleConfig);
            JobActivityManager.LstModuleConfiguration.Count.Should().Be(1);
        }

        [TestMethod]
        public void DeleteModuleConfig_method_should_delete_module_configuration_if_exist()
        {
            var moduleConfig = new ModuleConfiguration
            {
                ActivityType = ActivityType.Follow
            };
            JobActivityManager.LstModuleConfiguration.Add(moduleConfig);

            //before DeleteModuleConfig method called
            JobActivityManager.LstModuleConfiguration[0].IsEnabled.Should().BeFalse();
            JobActivityManager.LstModuleConfiguration[0].MaximumCountPerDay.Should().Be(30);
            JobActivityManager.LstModuleConfiguration.Count.Should().Be(1);
            
            JobActivityManager.DeleteModuleConfig(ActivityType.Follow);

            //after DeleteModuleConfig method called
            JobActivityManager.LstModuleConfiguration.Count.Should().Be(0);
          
        }
        [TestMethod]
        public void DeleteModuleConfig_method_should_not_delete_module_configuration_if_not_exist()
        {
            var moduleConfig = new ModuleConfiguration
            {
                ActivityType = ActivityType.Unfollow
            };
            JobActivityManager.LstModuleConfiguration.Add(moduleConfig);

            //before DeleteModuleConfig method called
            JobActivityManager.LstModuleConfiguration[0].IsEnabled.Should().BeFalse();
            JobActivityManager.LstModuleConfiguration[0].MaximumCountPerDay.Should().Be(30);
            JobActivityManager.LstModuleConfiguration.Count.Should().Be(1);

            JobActivityManager.DeleteModuleConfig(ActivityType.Follow);

            //after DeleteModuleConfig method called
            JobActivityManager.LstModuleConfiguration.Count.Should().Be(1);
            JobActivityManager.LstModuleConfiguration[0].IsEnabled.Should().BeFalse();
            JobActivityManager.LstModuleConfiguration[0].MaximumCountPerDay.Should().Be(30);
      }
    }
}
