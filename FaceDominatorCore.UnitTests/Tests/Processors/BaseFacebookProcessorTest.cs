using System;
using Dominator.Tests.Utils;
using DominatorHouse.ThreadUtils;
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
using FaceDominatorCore.UnitTests.Tests.FdLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Unity;
using DominatorHouseCore.FileManagers;
using FaceDominatorCore.FDLibrary.FdFunctions.FdBrowserManager;

namespace FaceDominatorCore.UnitTests.Tests.Processors
{
    [TestClass]
    public class BaseFacebookProcessorTest : UnityInitializationTests
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
        protected IDelayService DelayService;
        protected string AccountId;
        protected IDbOperations DbOperations;
        protected IDbGlobalService GlobalService;
        protected IFdLoginProcess FdLoginProcess;
        protected IDbAccountService DbAccountService;
        protected ICampaignInteractionDetails CampaignInteractionDetails;
        protected IFdRequestLibrary _fdRequestLibrary;
        protected FdLibraryTest FdLibraryTest;
        protected IJobLimitsHolder JobLimitsHolder;
        protected IJobConfiguration JobConfiguration;
        protected IFdJobProcess FdJobProcess;
        protected IFdUpdateAccountProcess FdUpdateAccountProcess;
        protected IDelayService _delayService;
        protected IFdBrowserManager BrowserManager;
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
            DelayService = Substitute.For<IDelayService>();
            DbOperations = Substitute.For<IDbOperations>();
            FdLoginProcess = Substitute.For<IFdLoginProcess>();
            DbAccountService = Substitute.For<IDbAccountService>();
            Container.RegisterInstance(DbAccountService);
            BinFileHelper = Substitute.For<IBinFileHelper>();
            CampaignInteractionDetails = Substitute.For<ICampaignInteractionDetails>();
            FdLibraryTest = Substitute.For<FdLibraryTest>();
            _fdRequestLibrary = Substitute.For<IFdRequestLibrary>();
            JobLimitsHolder = Substitute.For<IJobLimitsHolder>();
            JobConfiguration = Substitute.For<IJobConfiguration>();
            FdJobProcess = Substitute.For<IFdJobProcess>();
            FdUpdateAccountProcess = Substitute.For<IFdUpdateAccountProcess>();
            _delayService = Substitute.For<IDelayService>();
            BrowserManager = Substitute.For<IFdBrowserManager>();
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
            Container.RegisterInstance(FdLibraryTest);
            Container.RegisterInstance(_fdRequestLibrary);
            Container.RegisterInstance(JobLimitsHolder);
            Container.RegisterInstance(JobConfiguration);
            Container.RegisterInstance(FdJobProcess);
            Container.RegisterInstance(FdUpdateAccountProcess);
            Container.RegisterInstance(_delayService);
            Container.RegisterInstance(BrowserManager);
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
