using System;
using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums.DHEnum;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using PinDominatorCore.PDFactories;
using PinDominatorCore.PDLibrary.DAL;
using PinDominatorCore.PDModel;
using PinDominatorCore.Request;
using Unity;

namespace PinDominatorCore.PDLibrary
{
    public interface IPdJobProcess : IJobProcess
    {
        string CampaignId { get; }
        CampaignDetails CampaignDetails { get; }
        bool AddedToDb { get; set; }
        ModuleSetting ModuleSetting { get; }
        IDbAccountService DbAccountService { get; set; }
        IPdBrowserManager BrowserManager { get; set; }
        JobProcessResult FinalProcess(ScrapeResultNew scrapedResult);
        bool CheckJobCompleted();
    }

    public abstract class PdJobProcess : JobProcess, IPdJobProcess
    {
        public ModuleSetting ModuleSetting { get; set; }
        public IDbAccountService DbAccountService { get; set; }
        public readonly ICampaignInteractionDetails CampaignInteractionDetails;
        public IDbOperations DbBlackListOperations;
        public readonly IDbGlobalService DbGlobalService;
        public readonly IDbOperations DbWhiteListOperations;
        public readonly IGlobalInteractionDetails GlobalInteractionDetails;
        public IGlobalDatabaseConnection DataBaseConnectionGlb;
        public  IPdLogInProcess PdLogInProcess;
        public IPinFunction PinFunct { get; set; }
        public TemplateModel TemplateModel { get; set; }
        public IPdBrowserManager BrowserManager { get; set; }
        public SoftwareSettingsModel SoftwareSettingsModel { get; set; }
        public bool AddedToDb { get; set; }

        /// <summary>
        /// Here we are initializing all the necessory properties such as pinFunct, pdLoginProcess, templateModel etc.
        /// which will be used to perfom job operations
        /// </summary>
        /// <param name="processScopeModel"></param>
        /// <param name="accountServiceScoped"></param>
        /// <param name="globalService"></param>
        /// <param name="queryScraperFactory"></param>
        /// <param name="httpHelper"></param>
        /// <param name="pdLogInProcess"></param>
        protected PdJobProcess(IProcessScopeModel processScopeModel, IDbAccountServiceScoped accountServiceScoped,
            IDbGlobalService globalService, IPdQueryScraperFactory queryScraperFactory, IPdHttpHelper httpHelper, IPdLogInProcess pdLogInProcess) : 
            base(processScopeModel, queryScraperFactory, httpHelper)
        {
            ModuleSetting = processScopeModel.GetActivitySettingsAs<ModuleSetting>();
            DbAccountService = accountServiceScoped;
            DbGlobalService = globalService;

            var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
            var model = templatesFileManager.GetTemplateById(processScopeModel.TemplateId);
            TemplateModel = model;
            
            DataBaseConnectionGlb = SocinatorInitialize.GetGlobalDatabase();
            DbWhiteListOperations = new DbOperations(DataBaseConnectionGlb.GetSqlConnection(SocialNetworks, DominatorHouseCore.Enums.DHEnum.UserType.WhiteListedUser));
                
            PdLogInProcess = pdLogInProcess;
            PinFunct = PdLogInProcess.PinFunct;
            BrowserManager = PdLogInProcess.BrowserManager;
            SoftwareSettingsModel = PdLogInProcess.SoftwareSettingsModel;
            CampaignInteractionDetails = InstanceProvider.GetInstance<ICampaignInteractionDetails>();
            GlobalInteractionDetails = InstanceProvider.GetInstance<IGlobalInteractionDetails>();
        }
        
        /// <summary>
        /// Here we will check if any activity has reached job, hour, daily or weekly limit then it will return true
        /// </summary>
        /// <returns></returns>
        public bool CheckJobCompleted()
        {
            if (CheckJobProcessLimitsReached())
            {
                GlobusLogHelper.log.Info(Log.JobCompleted, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.UserName, ActivityType);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Here we are doing login before send to perform activity
        /// </summary>
        /// <returns></returns>
        protected override bool Login()
        {
            try
            {
                if (ActivityType == DominatorHouseCore.Enums.ActivityType.CreateAccount)
                    return true;
                return LoginBase(PdLogInProcess);
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return false;
            }
        }
        
        /// <summary>
        /// Here we are closing last browser in which the activity was performed
        /// </summary>
        /// <returns></returns>
        protected override bool CloseAutomationBrowser()
        {
            try
            {
                if (BrowserManager != null && BrowserManager.BrowserWindows != null && BrowserManager.BrowserWindows.Count > 0)
                    BrowserManager.CloseLast();
            }
            catch (Exception e)
            {
                e.DebugLog();
            }
            return true;
        }
    }
}