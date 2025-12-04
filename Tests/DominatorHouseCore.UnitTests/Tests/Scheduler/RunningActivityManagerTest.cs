using Dominator.Tests.Utils;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.ExecutionCounters;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Utility;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity;

namespace DominatorHouseCore.UnitTests.Tests.Scheduler
{
    [TestClass]
    public class RunningActivityManagerTest : UnityInitializationTests
    {
        private RunningActivityManager _runningActivityManager;
        private IDominatorScheduler _dominatorScheduler;
        ISoftwareSettingsFileManager softwareFileManager;
        List<DominatorAccountModel> lstAccount;

        private ISchedulerProxy _schedulerProxy;
        private IJobProcessFactory _jobProcessFactory;
        private IJobLimitsHolder _jobLimitsHolder;
        private IAccountsCacheService _accountsCacheService;
        private IJobProcessScopeFactory _jobProcessScopeFactory;
        private IJobCountersManager _jobCountersManager;
        private IJobActivityConfigurationManager _jobActivityConfigurationManager;
        private IRunningJobsHolder _runningJobsHolder;
        [TestInitialize]

        public override void SetUp()
        {
            base.SetUp();
            _runningActivityManager = new RunningActivityManager();

            _dominatorScheduler = Substitute.For<IDominatorScheduler>();
            Container.RegisterInstance(_dominatorScheduler);
            softwareFileManager = Substitute.For<ISoftwareSettingsFileManager>();
            Container.RegisterInstance(softwareFileManager);
            var campaignsFileManager = Substitute.For<ICampaignsFileManager>();
            Container.RegisterInstance(campaignsFileManager);

            _schedulerProxy = Substitute.For<ISchedulerProxy>();
            _jobLimitsHolder = Substitute.For<IJobLimitsHolder>();
            _accountsCacheService = Substitute.For<IAccountsCacheService>();
            _jobCountersManager = Substitute.For<IJobCountersManager>();
            _jobActivityConfigurationManager = Substitute.For<IJobActivityConfigurationManager>();
            Container.RegisterInstance(_jobActivityConfigurationManager);
            _runningJobsHolder = Substitute.For<IRunningJobsHolder>();

            _jobProcessScopeFactory = Substitute.For<IJobProcessScopeFactory>();
            lstAccount = new List<DominatorAccountModel>
            {
                new DominatorAccountModel()
                {
                    AccountId = "1bc",
                    ActivityManager = new JobActivityManager
                    {
                        LstModuleConfiguration = new List<ModuleConfiguration>
                        {
                            new ModuleConfiguration {ActivityType = ActivityType.Follow, IsEnabled = true}
                        }
                    }
                }
            };

        }
        [TestMethod]
        public void should_call_ScheduleEachActivity_if_IsEnableParallelActivitiesChecked_is_true()
        {
            softwareFileManager.GetSoftwareSettings().ReturnsForAnyArgs(new SoftwareSettingsModel
            {
                IsEnableParallelActivitiesChecked = true
            });
            var task = _runningActivityManager.Initialize(lstAccount);
            task.Wait();
            _dominatorScheduler.Received(1).ScheduleEachActivity(lstAccount[0]);
            softwareFileManager.Received(1).GetSoftwareSettings();
        }
        [TestMethod]
        public void should_call_StartNextRound_if_IsEnableParallelActivitiesChecked_is_false()
        {
            softwareFileManager.GetSoftwareSettings().ReturnsForAnyArgs(new SoftwareSettingsModel
            {
                IsEnableParallelActivitiesChecked = false
            });
            var moduleConfig = new List<ModuleConfiguration>
            { new ModuleConfiguration { IsEnabled = true,NextRun= DateTime.Now,ActivityType=ActivityType.Follow } };
            _jobActivityConfigurationManager[lstAccount[0].AccountId].Where(x => x.IsEnabled).ReturnsForAnyArgs(moduleConfig);

            _runningActivityManager.Initialize(lstAccount);
            softwareFileManager.Received(1).GetSoftwareSettings();
        }
        [TestMethod]
        public void should_call_ScheduleActivityForNextJob_if_IsEnabled_is_true()
        {
         
            var moduleConfig = new List<ModuleConfiguration>
                               {
                                     new ModuleConfiguration
                                     {
                                         IsEnabled = true,NextRun= DateTime.Now,ActivityType=ActivityType.Follow
                                     }
                                };
            _jobActivityConfigurationManager[lstAccount[0].AccountId].Where(x => x.IsEnabled).ReturnsForAnyArgs(moduleConfig);

            _runningActivityManager.StartNextRound(lstAccount[0]);
            _dominatorScheduler.Received(1).ScheduleActivityForNextJob(lstAccount[0], moduleConfig[0].ActivityType);
           
        }
        [TestMethod]
        public void should_call_ScheduleActivityForNextJob_if_IsEnabled_is_false()
        {

            var moduleConfig = new List<ModuleConfiguration>
                               {
                                     new ModuleConfiguration
                                     {
                                         IsEnabled = false
                                     }
                                };
            _jobActivityConfigurationManager[lstAccount[0].AccountId].Where(x => x.IsEnabled).ReturnsForAnyArgs(moduleConfig);

            _runningActivityManager.StartNextRound(lstAccount[0]);
            _dominatorScheduler.Received(0).ScheduleActivityForNextJob(lstAccount[0], moduleConfig[0].ActivityType);

        }
    }
}
