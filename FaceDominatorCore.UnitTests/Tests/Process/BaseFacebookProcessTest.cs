using Dominator.Tests.Utils;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.ExecutionCounters;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Settings;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDFactories;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDLibrary.FdProcesses;
using FaceDominatorCore.FDRequest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using DominatorHouse.ThreadUtils;
using Unity;
using DominatorHouseCore.FileManagers;

namespace FaceDominatorCore.UnitTests.Tests.Process
{
     [TestClass]
    public class BaseFacebookProcessTest : UnityInitializationTests
    {
        protected IBinFileHelper BinFileHelper;
        protected IProcessScopeModel ProcessScopeModel;
        protected IFdQueryScraperFactory FdQueryScraperFactory;
        protected IFdHttpHelper FdHttpHelper;
        protected IDbAccountServiceScoped DbAccountServiceScoped;
        protected IDbCampaignServiceScoped DbCampaignServiceScoped;
        protected IDbCampaignService DbCampaignService;
        protected IFdRequestLibrary FdRequestLibrary;
        protected ISoftwareSettings SoftwareSettings;
        protected IRunningJobsHolder RunningJobsHolder;
        protected IJobCountersManager JobCountersManager;
        protected IJobActivityConfigurationManager JobActivityConfigurationManager;
        protected IDominatorScheduler DominatorScheduler;
        protected IExecutionLimitsManager ExecutionLimitsManager;
        protected DominatorAccountModel FdDominatorAccountModel;
        protected IDelayService ThreadUtility;
        protected string AccountId;
        protected IDbOperations DbOperations;
        protected IDbGlobalService GlobalService;
        protected IFdLoginProcess FdLoginProcess;
        protected IDbAccountService DbAccountService;
        protected ICampaignInteractionDetails CampaignInteractionDetails;
        protected IJobLimitsHolder JobLimitsHolder;
        protected IDelayService DelayService;
        protected ISoftwareSettingsFileManager SoftwareSettingsFileManager;
        protected IAccountScopeFactory AccountScopeFactory;

        [TestInitialize]
        public override void SetUp()
        {
            #region Substitute Defines

            base.SetUp();
            AccountId = new Guid().ToString();
            ProcessScopeModel = Substitute.For<IProcessScopeModel>();
            FdQueryScraperFactory = Substitute.For<IFdQueryScraperFactory>();
            FdHttpHelper = Substitute.For<IFdHttpHelper>();
            DbAccountServiceScoped = Substitute.For<IDbAccountServiceScoped>();
            DbCampaignServiceScoped = Substitute.For<IDbCampaignServiceScoped>();
            DbCampaignService = Substitute.For<IDbCampaignService>();
            FdRequestLibrary = Substitute.For<IFdRequestLibrary>();
            SoftwareSettings = Substitute.For<ISoftwareSettings>();
            RunningJobsHolder = Substitute.For<IRunningJobsHolder>();
            JobCountersManager = Substitute.For<IJobCountersManager>();
            DominatorScheduler = Substitute.For<IDominatorScheduler>();
            ExecutionLimitsManager = Substitute.For<IExecutionLimitsManager>();
            JobActivityConfigurationManager = Substitute.For<IJobActivityConfigurationManager>();
            Container.RegisterInstance(JobActivityConfigurationManager);
            ThreadUtility = Substitute.For<IDelayService>();
            DbOperations = Substitute.For<IDbOperations>();
            FdLoginProcess = Substitute.For<IFdLoginProcess>();
            DbAccountService = Substitute.For<IDbAccountService>();
            Container.RegisterInstance(DbAccountService);
            BinFileHelper = Substitute.For<IBinFileHelper>();
            CampaignInteractionDetails = Substitute.For<ICampaignInteractionDetails>();
            JobLimitsHolder = Substitute.For<IJobLimitsHolder>();
            SoftwareSettingsFileManager = Substitute.For<ISoftwareSettingsFileManager>();
            AccountScopeFactory = Substitute.For<IAccountScopeFactory>();

            FdDominatorAccountModel = new DominatorAccountModel
            {
                AccountId = AccountId,
                AccountBaseModel = new DominatorAccountBaseModel
                {
                    AccountNetwork = SocialNetworks.Facebook,
                    UserName = "Facebook Account Name"
                },
            };

            #endregion

            #region Container Register
            Container.RegisterInstance(BinFileHelper);
            Container.RegisterInstance(ProcessScopeModel);
            Container.RegisterInstance(SoftwareSettings);
            Container.RegisterInstance(RunningJobsHolder);
            Container.RegisterInstance(JobCountersManager);
            Container.RegisterInstance(DominatorScheduler);
            Container.RegisterInstance(ExecutionLimitsManager);
            Container.RegisterInstance(JobActivityConfigurationManager);
            Container.RegisterInstance(DbOperations);
            Container.RegisterInstance(CampaignInteractionDetails);
            Container.RegisterInstance(DbAccountServiceScoped);
            Container.RegisterInstance(DbCampaignServiceScoped);
            Container.RegisterInstance(JobLimitsHolder);
            Container.RegisterInstance(SoftwareSettingsFileManager);
            Container.RegisterInstance(AccountScopeFactory);
            #endregion

            #region Returns

            SoftwareSettings.Settings.Returns(new SoftwareSettingsModel());
            ProcessScopeModel.Account.Returns(FdDominatorAccountModel);

            #endregion

        }
    }
}
