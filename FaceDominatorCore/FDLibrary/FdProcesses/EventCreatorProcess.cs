using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.FacebookModels;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDFactories;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDRequest;
using System;
using AccountInteractedEvents = DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedEvents;
using CampaignInteractedEvents = DominatorHouseCore.DatabaseHandler.FdTables.Campaigns.InteractedEvents;

namespace FaceDominatorCore.FDLibrary.FdProcesses
{
    public class EventCreatorProcess : FdJobProcessInteracted<AccountInteractedEvents>
    {
        private readonly IFdRequestLibrary _fdRequestLibrary;

        public EventCreatorProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IFdQueryScraperFactory queryScraperFactory,
            IFdHttpHelper fdHttpHelper, IFdLoginProcess fdLogInProcess, IFdRequestLibrary fdRequestLibrary,
            IDbCampaignServiceScoped dbCampaignServiceScoped)
            : base(processScopeModel, accountServiceScoped, queryScraperFactory, fdHttpHelper, fdLogInProcess, dbCampaignServiceScoped)
        {
            _fdRequestLibrary = fdRequestLibrary;
            AccountModel = DominatorAccountModel;
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            var jobProcessResult = new JobProcessResult();

            try
            {
                EventCreaterManagerModel fdEvents = (EventCreaterManagerModel)scrapeResult.ResultEvent;

                GlobusLogHelper.log.Info(Log.StartedActivity, AccountModel.AccountBaseModel.AccountNetwork,
                       AccountModel.AccountBaseModel.UserName, fdEvents.EventType, fdEvents.EventName);

                var responseHandler = DominatorAccountModel.IsRunProcessThroughBrowser ?
                    FdLogInProcess._browserManager.EventCreater(AccountModel, fdEvents) :
                    _fdRequestLibrary.EventCreater(AccountModel, fdEvents);

                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                if (responseHandler.Status)
                {
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, fdEvents.EventType, fdEvents.EventName);
                    IncrementCounters();
                    jobProcessResult.IsProcessSuceessfull = true;
                    AddEventDetailsDataToDatabase(responseHandler.FdEvents);
                }
                else
                    GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $"{fdEvents.EventName} {responseHandler.ErrorMsg}");

                DelayBeforeNextActivity();

            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return jobProcessResult;
        }

        private void AddEventDetailsDataToDatabase(FdEvents fdEvents)
        {
            try
            {

                var jobActivityConfigurationManager = InstanceProvider.GetInstance<IJobActivityConfigurationManager>();

                var modulesetting = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

                if (modulesetting == null)
                    return;

                if (modulesetting.IsTemplateMadeByCampaignMode)
                {
                    DbCampaignService.Add(new CampaignInteractedEvents
                    {
                        EventGUid = fdEvents.Id,
                        ActivityType = ActivityType.ToString(),
                        EventName = fdEvents.EventName,
                        EventDescrption = fdEvents.EventDescription,
                        EventLocation = fdEvents.EventLocation,
                        EventId = fdEvents.EventId,
                        EventStartDate = fdEvents.EventStartDate,
                        EventEndDate = fdEvents.EventEndDate,
                        EventType = fdEvents.EventType,
                        IsGuestCanInviteFriends = fdEvents.IsShowGuestList,
                        IsShowGuestList = fdEvents.IsShowGuestList,
                        IsAnyOneCanPostForAllPost = fdEvents.IsAnyOneCanPostForAllPost,
                        IsQuesOnMessanger = fdEvents.IsQuesOnMessanger,
                        IsPostMustApproved = fdEvents.IsPostMustApproved,
                        EventUrl = $"{FdConstants.FbHomeUrl}{fdEvents.EventId}",
                        InteractionDateTime = DateTime.Now,
                        AccountEmail = DominatorAccountModel.AccountBaseModel.UserName,
                    });
                }
                DbAccountService.Add(new AccountInteractedEvents
                {
                    EventGUid = fdEvents.Id,
                    ActivityType = ActivityType.ToString(),
                    EventName = fdEvents.EventName,
                    EventDescrption = fdEvents.EventDescription,
                    EventLocation = fdEvents.EventLocation,
                    EventId = fdEvents.EventId,
                    EventStartDate = fdEvents.EventStartDate,
                    EventEndDate = fdEvents.EventEndDate,
                    EventType = fdEvents.EventType,
                    IsGuestCanInviteFriends = fdEvents.IsShowGuestList,
                    IsShowGuestList = fdEvents.IsShowGuestList,
                    IsAnyOneCanPostForAllPost = fdEvents.IsAnyOneCanPostForAllPost,
                    IsQuesOnMessanger = fdEvents.IsQuesOnMessanger,
                    IsPostMustApproved = fdEvents.IsPostMustApproved,
                    EventUrl = $"{FdConstants.FbHomeUrl}{fdEvents.EventId}",
                    InteractionDateTime = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                ex.ErrorLog(ex.Message);
            }
        }
    }
}
