using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.FdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDFactories;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDModel.CommonSettings;
using FaceDominatorCore.FDRequest;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace FaceDominatorCore.FDLibrary.FdProcesses
{
    public interface IFdJobProcess : IJobProcess
    {
        string CampaignId { get; }
        ModuleSetting ModuleSetting { get; }
        JobProcessResult FinalProcess(ScrapeResultNew scrapedResult);
        bool CheckJobCompleted();
        DominatorAccountModel AccountModel { get; set; }
        BlackListWhitelistHandler BlackListWhitelistHandler { get; set; }
        DbCampaignService ObjDbCampaignService { get; set; }
        DbAccountService ObjDbAccountService { get; set; }
        int JobActionCount { get; set; }
    }

    public abstract class FdJobProcessInteracted<T> : FdJobProcess where T : class, new()
    {
        private readonly IExecutionLimitsManager _executionLimitsManager;
        protected FdJobProcessInteracted(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IFdQueryScraperFactory queryScraperFactory,
            IFdHttpHelper fdHttpHelper, IFdLoginProcess fdLogInProcess, IDbCampaignServiceScoped dbCampaignServiceScoped)
            : base(processScopeModel, accountServiceScoped, queryScraperFactory, fdHttpHelper, fdLogInProcess, dbCampaignServiceScoped)
        {

            _executionLimitsManager = InstanceProvider.GetInstance<IExecutionLimitsManager>();
            var jobLimitsHolder = InstanceProvider.GetInstance<IJobLimitsHolder>();
            jobLimitsHolder.Reset(Id, JobConfiguration);
        }

        public override ReachedLimitInfo CheckLimit()
        {
            try
            {
                return _executionLimitsManager.CheckIfLimitreached<T>(Id, SocialNetworks.Facebook,
                ActivityType);

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return _executionLimitsManager.CheckIfLimitreached<T>(Id, SocialNetworks.Facebook,
                ActivityType);
        }
    }



    public abstract class FdJobProcess : JobProcess, IFdJobProcess
    {

        public ModuleSetting ModuleSetting { get; set; }

        public DominatorAccountModel AccountModel { get; set; }

        public BlackListWhitelistHandler BlackListWhitelistHandler { get; set; }

        public DbAccountService ObjDbAccountService { get; set; }

        public DbCampaignService ObjDbCampaignService { get; set; }

        public IDbAccountService DbAccountService { get; set; }

        public IDbCampaignService DbCampaignService { get; set; }

        //public IDbOperations DbCampaignOperations { get; set; }

        public int JobActionCount { get; set; }


        protected SoftwareSettingsModel _SoftwareSettingsModel { get; set; }

        public bool AddedToDb { get; set; }

        protected IFdLoginProcess FdLogInProcess;

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


        protected FdJobProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped,
            IFdQueryScraperFactory queryScraperFactory, IFdHttpHelper fdHttpHelper,
            IFdLoginProcess fdLogInProcess, IDbCampaignServiceScoped dbCampaignServiceScoped)
            : base(processScopeModel, queryScraperFactory, fdHttpHelper)
        {
            FdLogInProcess = fdLogInProcess;

            ModuleSetting = processScopeModel.GetActivitySettingsAs<ModuleSetting>();

            AccountModel = DominatorAccountModel;

            ObjDbCampaignService = new DbCampaignService(CampaignId);

            ObjDbAccountService = new DbAccountService(AccountModel);

            _SoftwareSettingsModel = _softwareSettingsFileManager.GetSoftwareSettings();

            BlackListWhitelistHandler = new BlackListWhitelistHandler(ModuleSetting, DominatorAccountModel, ActivityType);

            DbAccountService = accountServiceScoped;

            DbCampaignService = dbCampaignServiceScoped;

            ObjDbAccountService = new DbAccountService(AccountModel);

            BlackListWhitelistHandler = new BlackListWhitelistHandler
                (ModuleSetting, DominatorAccountModel, ActivityType);
        }

        /// <summary>
        /// Overriding Login method to Login before Each activity
        /// </summary>
        /// <returns></returns>
        protected override bool Login() => LoginBase(FdLogInProcess);

        public override bool ProcessBeforeStartingJob()
        {
            try
            {
                switch (ActivityType)
                {
                    case ActivityType.SendFriendRequest:
                        {
                            var query = SavedQueries.FirstOrDefault();

                            if (query?.QueryType == "Friend Of Friend")
                            {

                                var user = FdConstants.getFaceBookUserFromUrlOrIdOrUserName(query?.QueryValue);

                                if (user == null)
                                {
                                    return false;
                                }
                                var friendMatch = DbAccountService.Get<Friends>().ToList().Any(x =>
                                {
                                    var facebookUser = JsonConvert.DeserializeObject<FacebookUser>(x.DetailedUserInfo);
                                    bool userIdMatch = facebookUser?.UserId != null && user.UserId != null && facebookUser.UserId == user.UserId;
                                    bool profileIdMatch = facebookUser?.ProfileId != null && user.ProfileId != null && facebookUser.ProfileId == user.ProfileId;

                                    return userIdMatch || profileIdMatch;
                                });


                                if (!friendMatch)
                                {
                                    GlobusLogHelper.log.Info(Log.CustomMessage,
                                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                        "Cannot send request as the user is not in the friend list");
                                    return false;
                                }
                            }
                            break;
                        }
                }
            }
            catch (Exception)
            {

                return true;
            }
            return true;
        }

        public bool CheckProcessLimitsReached()
        {
            return CheckJobProcessLimitsReached();
        }

        protected override bool CloseAutomationBrowser()
        {
            FdLogInProcess._browserManager.CloseBrowser(AccountModel);
            FdLogInProcess._browserManager.CloseBrowserCustom(AccountModel);
            return true;
        }

    }
}



#region UnUsedCodes


//protected override bool CheckJobProcessLimitsReached()
//{
//    var limitType = ReachedLimitType.NoLimit;
//    try
//    {
//        #region No of Actions performed per week

//        if (NoOfActionPerformedCurrentWeek >= MaxNoOfActionPerWeek && limitType == ReachedLimitType.NoLimit)
//        {
//            GlobusLogHelper.log.Info(Log.WeeklyLimitReached,
//                DominatorAccountModel.AccountBaseModel.AccountNetwork,
//                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, MaxNoOfActionPerWeek);
//            limitType = ReachedLimitType.Weekly;
//        }

//        #endregion

//        #region Number Actions performed per day


//        if (NoOfActionPerformedCurrentDay >= MaxNoOfActionPerDay && limitType == ReachedLimitType.NoLimit)
//        {
//            GlobusLogHelper.log.Info(Log.DailyLimitReached,
//                DominatorAccountModel.AccountBaseModel.AccountNetwork,
//                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, MaxNoOfActionPerDay);
//            limitType = ReachedLimitType.Daily;
//        }

//        #endregion

//        #region Number of actions performed per job

//        if (NoOfActionPerformedCurrentJob >= MaxNoOfActionPerJob && limitType == ReachedLimitType.NoLimit)
//        {
//            GlobusLogHelper.log.Info(Log.JobLimitReached, DominatorAccountModel.AccountBaseModel.AccountNetwork,
//                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, MaxNoOfActionPerJob);
//            limitType = ReachedLimitType.Job;
//        }

//        #endregion

//        #region Number of Actions performed per hour

//        if (NoOfActionPerformedCurrentHour >= MaxNoOfActionPerHour && limitType == ReachedLimitType.NoLimit)
//        {
//            GlobusLogHelper.log.Info(Log.HourlyLimitReached,
//                DominatorAccountModel.AccountBaseModel.AccountNetwork,
//                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, MaxNoOfActionPerHour);
//            limitType = ReachedLimitType.Hourly;
//        }

//        #endregion

//        if (limitType != ReachedLimitType.NoLimit)
//        {
//            Stop();
//            var jobActivityConfigurationManager = InstanceProvider.GetInstance<IJobActivityConfigurationManager>();

//            var moduleConfiguration = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];
//            var nextStartTime = limitType == ReachedLimitType.Job ? DateTimeUtilities.GetNextStartTime(moduleConfiguration, limitType, JobConfiguration.DelayBetweenJobs.GetRandom()) : DateTimeUtilities.GetNextStartTime(moduleConfiguration, limitType);
//            if (moduleConfiguration != null)
//            {
//                moduleConfiguration.NextRun = nextStartTime;
//                var accountsCacheService = InstanceProvider.GetInstance<IAccountsCacheService>();
//                jobActivityConfigurationManager.AddOrUpdate(DominatorAccountModel.AccountBaseModel.AccountId, ActivityType, moduleConfiguration);
//                accountsCacheService.UpsertAccounts(DominatorAccountModel);
//            }
//            DominatorScheduler.ScheduleNextActivity(DominatorAccountModel, ActivityType);
//            //  JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
//        }

//    }
//    catch (Exception ex)
//    {
//        GlobusLogHelper.log.Error("Error in scheduling" + ex.StackTrace);
//    }
//    if (limitType != ReachedLimitType.NoLimit)
//    {
//        return true;
//    }
//    return false;
//}


/*
        public void InitializeFromDatabaseForInteractedUsers()
        {
            InitializeFieldsFromDatabaseForInteractedUsers();
        }
*/

//protected void InitializeFieldsFromDatabaseForInteractedPages()
//{
//    try
//    {
//        var getStartDateofWeek = DateTime.Now.GetStartOfWeek();
//        var getTodayDate = DateTime.Today;
//        var hour = DateTime.Now.Date.AddHours(DateTime.Now.Hour);

//        var avtivityType = ActivityType.ToString();


//        IncrementCounters();
//    }
//    catch (Exception ex)
//    {
//        GlobusLogHelper.log.Error("Error in scheduling" + ex.StackTrace);
//    }
//}

//public void InitializeFieldsFromDatabaseForInteractedUsers()
//{
//    try
//    {
//        var getStartDateofWeek = DateTime.Now.GetStartOfWeek();
//        var getTodayDate = DateTime.Today;
//        var hour = DateTime.Now.Date.AddHours(DateTime.Now.Hour);

//        var avtivityType = ActivityType.ToString();

//        IncrementCounters();
//    }
//    catch (Exception ex)
//    {
//        GlobusLogHelper.log.Error("Error in scheduling" + ex.StackTrace);
//    }
//}

//protected void InitializeFieldsFromDatabaseForInteractedPosts()
//{
//    try
//    {
//        var getStartDateofWeek = DateTime.Now.GetStartOfWeek();
//        var getTodayDate = DateTime.Today;
//        var hour = DateTime.Now.Date.AddHours(DateTime.Now.Hour);

//        var avtivityType = ActivityType.ToString();

//        IncrementCounters();
//    }
//    catch (Exception ex)
//    {
//        GlobusLogHelper.log.Error("Error in scheduling" + ex.StackTrace);
//    }
//}

//protected void InitializeFieldsFromDatabaseForInteractedGroups()
//{
//    try
//    {
//        var getStartDateofWeek = DateTime.Now.GetStartOfWeek();
//        var getTodayDate = DateTime.Today;
//        var hour = DateTime.Now.Date.AddHours(DateTime.Now.Hour);

//        var avtivityType = ActivityType.ToString();

//        IncrementCounters();
//    }
//    catch (Exception ex)
//    {
//        GlobusLogHelper.log.Error("Error in scheduling" + ex.StackTrace);
//    }
//}

//protected void InitializeFieldsFromDatabaseForInteractedComments()
//{
//    try
//    {
//        var getStartDateofWeek = DateTime.Now.GetStartOfWeek();
//        var getTodayDate = DateTime.Today;
//        var hour = DateTime.Now.Date.AddHours(DateTime.Now.Hour);

//        var avtivityType = ActivityType.ToString();

//        IncrementCounters();
//    }
//    catch (Exception ex)
//    {
//        GlobusLogHelper.log.Error("Error in scheduling" + ex.StackTrace);
//    }
//}

/*
        public int GetCurrentJobCount()
        {
            return NoOfActionPerformedCurrentJob;
        }
*/

#endregion