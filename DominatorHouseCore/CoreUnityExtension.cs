#region

using ThreadUtils;
using DominatorHouseCore.AppResources;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.BusinessLogic.Scraper;
using DominatorHouseCore.Dal;
using DominatorHouseCore.DatabaseHandler.Common.EntityCounters;
using DominatorHouseCore.DatabaseHandler.CoreModels;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.ProxyServerManagment;
using DominatorHouseCore.Settings;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;
using Unity;
using Unity.Extension;
using Unity.Injection;

#endregion

namespace DominatorHouseCore
{
    public class CoreUnityExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Container.RegisterSingleton<IGlobalDatabaseConnection, GlobalDatabaseConnection>();

            Container.RegisterSingleton<IApplicationResourceProvider, ApplicationResourceProvider>();
            Container.RegisterSingleton<ILogViewModel, LogViewModel>();

            Container.RegisterSingleton<IAccountScopeFactory, AccountScopeFactory>();

            Container.RegisterSingleton<IAccountsCacheService, AccountsCacheService>();
            Container.RegisterSingleton<ITemplatesCacheService, TemplatesCacheService>();
            Container.RegisterSingleton<ITemplatesFileManager, TemplatesFileManager>();
            Container.RegisterSingleton<IGenericFileManager, GenericFileManager>();
            Container.RegisterSingleton<IAccountsFileManager, AccountsFileManager>();
            Container.RegisterSingleton<IOtherConfigFileManager, OtherConfigFileManager>();
            Container.RegisterSingleton<IFBFileManager, FBFileManager>();

            Container.RegisterSingleton<IAccountGrowthPropertiesProvider, AccountGrowthPropertiesProvider>();

            Container.RegisterSingleton<ISoftwareSettings, SoftwareSettings>();

            Container.RegisterSingleton<IWebService, WebService>();
            Container.RegisterSingleton<IDateProvider, DateProvider>();
            Container.RegisterSingleton<IDispatcherUtility, DispatcherUtility>();
            Container.RegisterSingleton<IFileSystemProvider, FileSystemProvider>();
            Container.RegisterSingleton<IJobActivityConfigurationManager, JobActivityConfigurationManager>();
            Container.RegisterSingleton<ICampaignsFileManager, CampaignsFileManager>();
            Container.RegisterSingleton<IBinFileHelper, BinFileHelper>();
            Container.RegisterSingleton<ILockFileConfigProvider, LockFileConfigProvider>();
            Container.RegisterSingleton<IProtoBuffBase, ProtoBuffBase>();
            Container.RegisterSingleton<IRunningJobsHolder, RunningJobsHolder>();
            Container.RegisterSingleton<ICampaignInteractionDetails, CampaignInteractionDetails>();
            Container.RegisterSingleton<IGlobalInteractionDetails, GlobalInteractionDetails>();
            Container.RegisterSingleton<ISoftwareSettingsFileManager, SoftwareSettingsFileManager>();
            Container.RegisterSingleton<IPerfCounterService, PerfCounterService>();
            Container.RegisterSingleton<IDataBaseHandler, DataBaseHandler>();
            Container.RegisterSingleton<IProxyFileManager, ProxyFileManager>();

            Container.AddNewExtension<ViewModelUnityExtension>();
            Container.AddNewExtension<DbMigrationUnityExtension>();
            Container.AddNewExtension<ProxyManagmentUnityExtension>();

            Container.RegisterSingleton<IQueryScraperFactory, DominatorScraperFactory>(SocialNetworks.Social
                .ToString());
            Container.RegisterSingleton<IJobProcessFactory, DominatorJobProcessFactory>(
                SocialNetworks.Social.ToString());
            Container.RegisterSingleton<IDominatorScheduler, DominatorScheduler>();
            Container.RegisterSingleton<ISchedulerProxy, SchedulerProxy>();

            Container.RegisterType<IDbOperations, DbOperations>(new InjectionConstructor(typeof(string),
                typeof(SocialNetworks), typeof(string)));


            Container.AddNewExtension<JobProcessUnityExtension>();
            Container.AddNewExtension<EntityCounterUnityExtension>();

            Container.RegisterSingleton<IDelayService, DelayService>();
        }
    }
}