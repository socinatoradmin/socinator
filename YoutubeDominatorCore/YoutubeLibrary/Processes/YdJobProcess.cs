using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.YdQuery;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Threading;
using YoutubeDominatorCore.Request;
using YoutubeDominatorCore.YDFactories;
using YoutubeDominatorCore.YDUtility;
using YoutubeDominatorCore.YoutubeLibrary.DAL;
using YoutubeDominatorCore.YoutubeLibrary.YdFunctions;
using YoutubeDominatorCore.YoutubeModels;

namespace YoutubeDominatorCore.YoutubeLibrary.Processes
{
    public interface IYdJobProcess : IJobProcess
    {
        YdModuleSetting ModuleSetting { get; }
        IYdBrowserManager BrowserManager { get; set; }
        bool IsCustom { get; set; }
        string CampaignId { get; }
        bool AddedToDb { get; set; }
        JobProcessResult FinalProcess(ScrapeResultNew scrapedResult);
    }

    public abstract class YdJobProcessInteracted<T> : YdJobProcess where T : class, new()
    {
        private readonly IExecutionLimitsManager _executionLimitsManager;

        public YdJobProcessInteracted(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IDbGlobalService globalService,
            IExecutionLimitsManager executionLimitsManager, IYdQueryScraperFactory queryScraperFactory,
            IYdHttpHelper ydHttpHelper, IYoutubeLogInProcess ydLogInProcess) : base(processScopeModel,
            accountServiceScoped, globalService, queryScraperFactory, ydHttpHelper, ydLogInProcess)
        {
            _executionLimitsManager = executionLimitsManager;
        }

        public override ReachedLimitInfo CheckLimit()
        {
            return _executionLimitsManager.CheckIfLimitreached<T>(Id, SocialNetworks.YouTube,
                ActivityType);
        }
    }

    public abstract class YdJobProcess : JobProcess, IYdJobProcess
    {
        public readonly IYoutubeLogInProcess _youtubeLogInProcess;
        public readonly IDbGlobalService GlobalService;
        public readonly IYdHttpHelper HttpHelper;
        public IDbAccountService DbAccountService;

        public YdJobProcess(IProcessScopeModel processScopeModel, IDbAccountServiceScoped accountServiceScoped,
            IDbGlobalService globalService, IYdQueryScraperFactory queryScraperFactory, IYdHttpHelper httpHelper,
            IYoutubeLogInProcess ydLogInProcess) : base(processScopeModel,
            queryScraperFactory, httpHelper)
        {
            ModuleSetting = processScopeModel.GetActivitySettingsAs<YdModuleSetting>();
            DbAccountService = accountServiceScoped;
            GlobalService = globalService;
            _youtubeLogInProcess = ydLogInProcess;
            SoftwareSettingsModel = _youtubeLogInProcess.SoftwareSettingsModel;
            BrowserManager = _youtubeLogInProcess.BrowserManager;
            HttpHelper = httpHelper;
            if (DbCampaignService == null && !string.IsNullOrEmpty(CampaignId))
                DbCampaignService = new DbCampaignService(CampaignId);
        }

        public IDbCampaignService DbCampaignService { get; set; }
        public SoftwareSettingsModel SoftwareSettingsModel { get; set; }
        public YdModuleSetting ModuleSetting { get; set; }
        public IYdBrowserManager BrowserManager { get; set; }

        public bool IsCustom { get; set; }
        public bool AddedToDb { get; set; }

        public override List<string> ContinueIfLimitNotReached()
        {
            switch (ActivityType)
            {
                case ActivityType.Subscribe:
                case ActivityType.ChannelScraper:
                    return new List<string>
                        {YdScraperParameters.Keywords.ToString(), YdScraperParameters.YTVideoCommenters.ToString()};

                case ActivityType.Like:
                case ActivityType.Comment:
                case ActivityType.Dislike:
                case ActivityType.ReportVideo:
                case ActivityType.PostScraper:
                case ActivityType.ViewIncreaser:
                    return new List<string>
                        {YdScraperParameters.Keywords.ToString(), YdScraperParameters.CustomChannel.ToString()};

                case ActivityType.LikeComment:
                    return new List<string>
                    {
                        YdScraperParameters.Keywords.ToString(), YdScraperParameters.CustomChannel.ToString(),
                        YdScraperParameters.CustomUrls.ToString()
                    };
                default:
                    return null;
            }
        }

        protected override bool Login()
        {
            return LoginBase(_youtubeLogInProcess);
        }

        protected override bool CloseAutomationBrowser()
        {
            try
            {
                if (DominatorAccountModel.IsRunProcessThroughBrowser)
                {
                    BrowserManager.CloseBrowser();
                    lock (YdStatic.BrowserLock)
                    {
                        --YdStatic.BrowserOpening;
                        Monitor.Pulse(YdStatic.BrowserLock);
                    }
                }
            }
            catch (Exception e)
            {
                e.DebugLog();
            }

            return true;
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            throw new NotImplementedException();
        }


        protected void SuccessLog(string successMessage)
        {
            GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, successMessage);
        }

        protected void FailedLog(string failedMessage)
        {
            GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, failedMessage);
        }
        protected void CustomLog(string customMessage)
        {
            GlobusLogHelper.log.Info(Log.CustomMessage,
                DominatorAccountModel.AccountBaseModel.AccountName, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, customMessage);
        }

    }
}