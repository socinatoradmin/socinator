using Dominator.Tests.Utils;
using DominatorHouse.ThreadUtils;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.GdQuery;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.ExecutionCounters;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Settings;
using DominatorHouseCore.Utility;
using GramDominatorCore.Factories;
using GramDominatorCore.GDFactories;
using GramDominatorCore.GDLibrary;
using GramDominatorCore.GDLibrary.DAL;
using GramDominatorCore.GDLibrary.InstagramBrowser;
using GramDominatorCore.GDModel;
using GramDominatorCore.Request;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Threading;
using Unity;

namespace GramDominatorCore.UnitTests.Tests.Library.ProcessorTest
{
    [TestClass]
    public class BaseGdProcessTest : UnityInitializationTests
    {
        protected IProcessScopeModel ProcessScopeModel;
        protected IInstaFunction InstaFunct;
        protected IInstaFunctFactory instaFunctionFectory;
        protected IGdJobProcess GdJobProcess;
        protected ISoftwareSettings SoftwareSettings;
        protected IDelayService IDelayService;
        protected IJobActivityConfigurationManager JobActivityConfigurationManager;
        protected BlackListWhitelistHandler GdBlackWhiteListHandler;
        protected ModuleSetting GdModuleSetting;
        protected DominatorAccountModel GdDominatorAccountModel;
        protected IDateProvider GdDateProvider;
        protected IDbAccountService DbAccountService;
        protected IGDAccountUpdateFactory GdAccountUpdateFactory;
        protected IDbCampaignService CampaignService;
        protected IAccountsCacheService AccountsCacheService;
        protected IAccountsFileManager AccountsFileManager;
        protected IDbOperations GdDbOperations;
        protected IDominatorScheduler DominatorScheduler;
        protected IDbGlobalService DbGlobalService;
        protected string AccountId;
        protected ITemplatesFileManager TemplatesFileManager;
        protected IGenericFileManager GdGenericFileManager;
        protected IGdHttpHelper GdHttpHelper;
        protected IAccountScopeFactory GdAccountScopeFactory;
        protected AccountModel AccountModel;
        protected IDbAccountServiceScoped AccountServiceScoped;
        protected IExecutionLimitsManager ExecutionLimitsManager;
        protected IGdQueryScraperFactory GdQueryScraperFactory;
        protected IGdLogInProcess GdLogInProcess;
        protected IGdBrowserManager GdBrowserManager;
        protected string CampaignId;
        protected JobConfiguration jobConfiguration;
        protected IRunningJobsHolder RunningJobsHolder;
        protected IJobCountersManager JobCountersManager;
        protected GdJobProcess IgJobProcesss;
        protected Enum obj;
        protected ICampaignsFileManager CampaignsFileManager;
        protected IGlobalInteractionDetails GlobalInteractionDetails;
        protected TemplateModel templateModel;
        protected JobProcessResult jobProcessResult;
        protected ISoftwareSettingsFileManager SoftwareSettingFileManager;
       // private IDelayService delayService;
        //executionLimitsManager

        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();

            #region Substitute Defines

            AccountId = Utilities.GetGuid();
            CampaignId =Utilities.GetGuid();
            GdJobProcess = Substitute.For<IGdJobProcess>();
            DbAccountService = Substitute.For<IDbAccountService>();
            GdAccountUpdateFactory = Substitute.For<IGDAccountUpdateFactory>();
            CampaignService = Substitute.For<IDbCampaignService>();
            jobProcessResult = Substitute.For<JobProcessResult>();
            SoftwareSettings = Substitute.For<ISoftwareSettings>();
            instaFunctionFectory = Substitute.For<IInstaFunctFactory>();
            InstaFunct = Substitute.For<IInstaFunction>();
            GdDateProvider = Substitute.For<IDateProvider>();
            DelayService = Substitute.For<IDelayService>();
            JobActivityConfigurationManager = Substitute.For<IJobActivityConfigurationManager>();
            AccountsCacheService = Substitute.For<IAccountsCacheService>();
            AccountsFileManager = Substitute.For<IAccountsFileManager>();
            GdDbOperations = Substitute.For<IDbOperations>();
            ProcessScopeModel = Substitute.For<IProcessScopeModel>();
            DominatorScheduler = Substitute.For<IDominatorScheduler>();
            DbGlobalService = Substitute.For<IDbGlobalService>();
            TemplatesFileManager = Substitute.For<ITemplatesFileManager>();
            GdGenericFileManager = Substitute.For<IGenericFileManager>();
            GdHttpHelper = Substitute.For<IGdHttpHelper>();
            GdAccountScopeFactory = Substitute.For<IAccountScopeFactory>();
            
            AccountServiceScoped = Substitute.For<IDbAccountServiceScoped>();
            ExecutionLimitsManager = Substitute.For<IExecutionLimitsManager>();
            GdQueryScraperFactory = Substitute.For<IGdQueryScraperFactory>();
            GdLogInProcess = Substitute.For<IGdLogInProcess>();
            GdBrowserManager = Substitute.For<IGdBrowserManager>();
            RunningJobsHolder = Substitute.For<IRunningJobsHolder>();
            JobCountersManager = Substitute.For<IJobCountersManager>();
            CampaignsFileManager = Substitute.For<ICampaignsFileManager>();
            GlobalInteractionDetails = Substitute.For<IGlobalInteractionDetails>();
            templateModel = Substitute.For<TemplateModel>();
            SoftwareSettingFileManager = Substitute.For<ISoftwareSettingsFileManager>();
            //delayService = Substitute.For<IDelayService>();
            //IGdLogInProcess
            //query
            GdDominatorAccountModel = new DominatorAccountModel
            {
                AccountId = AccountId,
                AccountBaseModel = new DominatorAccountBaseModel()
                {
                    AccountNetwork = SocialNetworks.Instagram,
                    AccountId = AccountId,
                    UserName="jacksmith729",
                    UserId= "2071648333",
                    
                }               
            };
            AccountModel = new AccountModel(GdDominatorAccountModel);
            #endregion

            #region Container Register

            Container.RegisterInstance(InstaFunct);
            Container.RegisterInstance(GdDateProvider);
            Container.RegisterInstance(JobActivityConfigurationManager);
            Container.RegisterInstance(DelayService);
            Container.RegisterInstance(SoftwareSettings);
            Container.RegisterInstance(GdAccountUpdateFactory);
            Container.RegisterInstance(AccountsFileManager);
            Container.RegisterInstance(GdDbOperations);
            Container.RegisterInstance(ProcessScopeModel);
            Container.RegisterInstance(DominatorScheduler);
            Container.RegisterInstance(TemplatesFileManager);
            Container.RegisterInstance(GdGenericFileManager);
            Container.RegisterInstance(GdHttpHelper);
            Container.RegisterInstance(GdAccountScopeFactory);
            Container.RegisterInstance(DbAccountService);
            Container.RegisterInstance(GdJobProcess);
            Container.RegisterInstance(ExecutionLimitsManager);

            Container.RegisterInstance(RunningJobsHolder);
            Container.RegisterInstance(JobCountersManager);
            Container.RegisterInstance(CampaignsFileManager);
            Container.RegisterInstance(GlobalInteractionDetails);
            Container.RegisterInstance(templateModel);
            Container.RegisterInstance(jobProcessResult);
            Container.RegisterInstance(instaFunctionFectory);
            Container.RegisterInstance(GdBrowserManager);
            Container.RegisterInstance(SoftwareSettingFileManager);
            #endregion


            #region Returns

            SoftwareSettings.Settings.Returns(new SoftwareSettingsModel());
            GdJobProcess.DominatorAccountModel.Returns(GdDominatorAccountModel);
            GdJobProcess.JobCancellationTokenSource.Returns(new CancellationTokenSource());
            GdModuleSetting = new ModuleSetting();
            GdJobProcess.ModuleSetting.Returns(GdModuleSetting);
          
            ProcessScopeModel.Network.Returns(SocialNetworks.Instagram);
            ProcessScopeModel.Account.Returns(GdDominatorAccountModel);
            ProcessScopeModel.CampaignId.Returns(CampaignId);
            jobConfiguration = new JobConfiguration();
            InstaFunct = new InstaFunct(GdDominatorAccountModel, GdHttpHelper,GdBrowserManager,DelayService, GdDateProvider);
            ProcessScopeModel.JobConfiguration.Returns(jobConfiguration);
            GdJobProcess.AccountId.Returns(AccountId);

            #endregion
        }

    }
}
