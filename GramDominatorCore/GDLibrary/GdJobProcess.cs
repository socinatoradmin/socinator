using DominatorHouseCore.Enums;
using System;
using DominatorHouseCore;
using DominatorHouseCore.BusinessLogic.ActivitiesWorkflow;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDModel;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using GramDominatorCore.GDLibrary.DAL;
using CommonServiceLocator;
using DominatorHouseCore.Process.JobLimits;
using GramDominatorCore.Request;
using Unity;
using DominatorHouseCore.BusinessLogic.Scraper;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.Interfaces;
using Newtonsoft.Json;
using GramDominatorCore.GDLibrary.InstagramBrowser;
using System.Threading;
using System.Windows;
using ThreadUtils;

namespace GramDominatorCore.GDLibrary
{

    public interface IGdJobProcess : IJobProcess
    {
        JobProcessResult FinalProcess(ScrapeResultNew scrapedResult);
        ModuleSetting ModuleSetting { get; set; }
        bool AddedToDb { get; set; }
        IGdLogInProcess loginProcess { get; set; }
        IGdHttpHelper HttpHelper { get; set; }
        AccountModel AccountModel { get; set; }
        string campaignId { get; set; }
        string maxId { get; set; }
        IDbAccountService DbAccountService { get; set; }
        IDbCampaignService DbCampaignService { get; set; }
        IDbOperations AccountDbOperation { get; set; }
        IDbOperations CampaignDbOperation { get; set; }
        AddInstagramDataIntoDatabase addDataIntoDatabase { get; set; }
        TemplateModel templateModel { get; set; }
        IInstaFunction instaFunct { get; set; }
        bool IsStop();

    }
    public abstract class GdJobProcessInteracted<T> : GdJobProcess where T : class, new()
    {
        private readonly IExecutionLimitsManager _executionLimitsManager;

        protected GdJobProcessInteracted(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IQueryScraperFactory queryScraperFactory, IGdHttpHelper httpHelper, IGdBrowserManager gdBrowser,IDelayService _delayService)
            : base(processScopeModel, accountServiceScoped, queryScraperFactory, httpHelper, gdBrowser, _delayService)
        {
            _executionLimitsManager = InstanceProvider.GetInstance<IExecutionLimitsManager>();
        }

        public override ReachedLimitInfo CheckLimit()
        {
            return _executionLimitsManager.CheckIfLimitreached<T>(Id, SocialNetworks.Instagram,
                ActivityType);
        }
    }

    public abstract class GdJobProcess : JobProcess, IGdJobProcess
    {
        public string _maxId = "Nothing";

        private static readonly object SyncJobProcess = new object();
        public ModuleSetting ModuleSetting { get; set; }

        public bool AddedToDb { get; set; }

        public IGdLogInProcess loginProcess { get; set; }

        public IGdHttpHelper HttpHelper { get; set; }

        public AccountModel AccountModel { get; set; }

        public string campaignId { get; set; }

        public string maxId { get; set; }

        public IDbAccountService DbAccountService { get; set; }

        public IDbCampaignService DbCampaignService { get; set; }

        public IDbOperations AccountDbOperation { get; set; }

        public IDbOperations CampaignDbOperation { get; set; }

        public AddInstagramDataIntoDatabase addDataIntoDatabase { get; set; }

        public TemplateModel templateModel { get; set; }
        public IGlobalInteractionDetails GlobalInteractionDetails { get; }

        public IInstaFunction instaFunct { get; set; }
        public IGdBrowserManager gdBrowserManager { get; set; }
        public IDelayService delayservice { get; set; }

        protected GdJobProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IQueryScraperFactory queryScraperFactory, IGdHttpHelper httpHelper, IGdBrowserManager gdBrowser, IDelayService _delayService)
            : base(processScopeModel, queryScraperFactory, httpHelper)
        {
            var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
            templateModel = templatesFileManager.GetTemplateById(processScopeModel.TemplateId);
            //templateModel=model;
            ModuleSetting = JsonConvert.DeserializeObject<ModuleSetting>(templateModel.ActivitySettings);
            AccountModel = new AccountModel(DominatorAccountModel);
            GlobalInteractionDetails = InstanceProvider.GetInstance<IGlobalInteractionDetails>();
            DbAccountService = accountServiceScoped;
            AccountDbOperation = InstanceProvider.ResolveAccountDbOperations(DominatorAccountModel.AccountId, SocialNetworks.Instagram);
            CampaignDbOperation = InstanceProvider.ResolveCampaignDbOperations(CampaignId, SocialNetworks.Instagram);
            HttpHelper = httpHelper;
            campaignId = CampaignId;
            gdBrowserManager = gdBrowser;
            chekingBrowserModule();
            delayservice = _delayService;
        }
        protected override bool Login()
        {
            try
            {              
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var scope = InstanceProvider.GetInstance<IAccountScopeFactory>();
                loginProcess = scope[DominatorAccountModel.AccountId].Resolve<IGdLogInProcess>();
                instaFunct = scope[DominatorAccountModel.AccountId].Resolve<IInstaFunction>();
                HttpHelper = scope[DominatorAccountModel.AccountId].Resolve<IGdHttpHelper>();
                if (DominatorAccountModel.IsRunProcessThroughBrowser)
                {
                   
                    return LoginBase(loginProcess);
                }     
                else
                    return LoginForJobRunning(loginProcess);                
            }
            catch (OperationCanceledException ex)
            {
                ex.DebugLog();
            }
            catch (AggregateException e)
            {
                foreach (Exception ex in e.InnerExceptions)
                    Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return false;
        }

        private bool LoginForJobRunning(ILoginProcess logInProcess)
        {
            int showingLoginSuccessful = 0;
            try
            {
                if (string.IsNullOrEmpty(CampaignId) && string.IsNullOrEmpty(TemplateId))
                {
                    GlobusLogHelper.log.Info($"Campign Id not set for {ActivityType} - {TemplateId}");
                    return false;
                }

                GlobusLogHelper.log.Info(Log.StartingJob, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType);

                if (HttpHelper.GetRequestParameter().Cookies == null || HttpHelper.GetRequestParameter().Cookies.Count == 0 || HttpHelper.GetRequestParameter().Cookies.Count<=4|| !DominatorAccountModel.IsUserLoggedIn)
                {
                    if(HttpHelper.GetRequestParameter().Cookies!=null)
                    {
                        if (HttpHelper.GetRequestParameter().Cookies.Count <= 4 && DominatorAccountModel.Cookies.Count > 4)
                            HttpHelper.GetRequestParameter().Cookies = DominatorAccountModel.Cookies;
                    }                   
                    GlobusLogHelper.log.Info(Log.AccountLogin, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName);
                    logInProcess.LoginWithAlternativeMethod(DominatorAccountModel,DominatorAccountModel.Token);
                    showingLoginSuccessful++;
                }
                if (DominatorAccountModel.IsUserLoggedIn && showingLoginSuccessful == 0 )
                {
                    GlobusLogHelper.log.Info(Log.SuccessfulLogin, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName);
                    DominatorAccountModel.IsUserLoggedIn = true;
                   
                }
                loginProcess.AssignBrowserFunction(DominatorAccountModel);

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return DominatorAccountModel.IsUserLoggedIn;
        }

        //alternate method is available in JobProcess class in dominatorHouse IsStopped() method
        public bool IsStop()
        {
            lock (SyncJobProcess)
            {
                return JobCancellationTokenSource == null || JobCancellationTokenSource.IsCancellationRequested;
            }
        }

        protected override bool CloseAutomationBrowser()
        {
            instaFunct.GdBrowserManager.CloseBrowser();
            GDUtility.GdAccountsBrowserDetails.CloseAllBrowser(DominatorAccountModel, !DominatorAccountModel.IsRunProcessThroughBrowser);
            return true;
        }
        public void chekingBrowserModule()
        {
            try
            {
                if (!DominatorAccountModel.IsRunProcessThroughBrowser 
                    //&& (
                    //ActivityType == ActivityType.Reposter || ActivityType == ActivityType.Unfollow ||
                    //          ActivityType == ActivityType.BlockFollower || ActivityType == ActivityType.FollowBack
                    //          || ActivityType == ActivityType.DeletePost || ActivityType == ActivityType.SendMessageToFollower ||
                    //          ActivityType == ActivityType.BroadcastMessages || ActivityType == ActivityType.AutoReplyToNewMessage
                    //          || ActivityType == ActivityType.LikeComment || ActivityType == ActivityType.Unlike ||
                    //          ActivityType == ActivityType.UserScraper || ActivityType == ActivityType.DownloadScraper ||
                    //          ActivityType == ActivityType.HashtagsScraper || ActivityType == ActivityType.CommentScraper)
                              )
                {
                    Application.Current.Dispatcher.Invoke(() =>
                        {
                            Dialog.ShowDialog("Activity", $"For Account {DominatorAccountModel.UserName} activity {ActivityType} won't work as it is running through browser automation setting, please try with Http");      
                            Stop();
                        });
                  
                }
            }
            catch (Exception )
            {
            }
        }
    }
}
