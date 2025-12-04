using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDFactories;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDModel.InviterModel;
using FaceDominatorCore.FDRequest;
using Newtonsoft.Json;
using System;
using AccountInteractedUsres = DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedUsers;
using CampaignInteractedUsres = DominatorHouseCore.DatabaseHandler.FdTables.Campaigns.InteractedUsers;

namespace FaceDominatorCore.FDLibrary.FdProcesses
{

    public class EventInviterProcess : FdJobProcessInteracted<AccountInteractedUsres>
    {
        public EventInviterModel EventInviterModel { get; set; }

        //        public Queue<string> QueMessage = new Queue<string>();

        public DominatorAccountModel Account { get; set; }

        public IFdRequestLibrary FdRequestLibrary { get; set; }

        public EventInviterProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped,
            IFdQueryScraperFactory queryScraperFactory,
            IFdHttpHelper fdHttpHelper, IFdLoginProcess fdLogInProcess, IFdRequestLibrary fdRequestLibrary,
            IDbCampaignServiceScoped dbCampaignServiceScoped)
            : base(processScopeModel, accountServiceScoped, queryScraperFactory, fdHttpHelper, fdLogInProcess, dbCampaignServiceScoped)
        {
            FdRequestLibrary = fdRequestLibrary;
            EventInviterModel = processScopeModel.GetActivitySettingsAs<EventInviterModel>();
            AccountModel = DominatorAccountModel;
            // TemplateId = template;
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            JobProcessResult jobProcessResult = new JobProcessResult();

            FacebookUser objFacebookUser = (FacebookUser)scrapeResult.ResultUser;

            FdEvents objEventDetails = (FdEvents)scrapeResult.ResultEvent;
            objEventDetails.Note = EventInviterModel.InviterOptionsModel.Note;
            objEventDetails.IsInvitedInMessanger = EventInviterModel.InviterOptionsModel.IsSendInvitationInMessanger;



            // ReSharper disable once UnusedVariable

            try
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                bool isSuccess = false;

                if (DominatorAccountModel.IsRunProcessThroughBrowser)
                {
                    isSuccess = FdLogInProcess._browserManager.SendEventInvittationTofriends(AccountModel,
                        scrapeResult.ResultEvent.EventId, objFacebookUser, objEventDetails.Note);

                    if (EventInviterModel.InviterOptionsModel.IsSendInvitationInMessanger)
                        isSuccess = FdLogInProcess._browserManager.InviteAsPersonalMessage(AccountModel, scrapeResult.ResultEvent.EventId,
                            objFacebookUser, objEventDetails.Note);
                }
                else
                {
                    if (EventInviterModel.InviterOptionsModel.IsSendInvitationInMessanger)
                    {
                        FdRequestLibrary.InviteAsPersonalMessage(AccountModel, scrapeResult.ResultEvent.EventId,
                            objFacebookUser, objEventDetails.Note);

                        isSuccess = FdRequestLibrary.SendEventInvittationTofriends(AccountModel,
                            scrapeResult.ResultEvent.EventId, objFacebookUser, objEventDetails.Note);
                    }
                    else
                        isSuccess = FdRequestLibrary.SendEventInvittationTofriends(AccountModel,
                            scrapeResult.ResultEvent.EventId, objFacebookUser, objEventDetails.Note);

                }


                if (isSuccess)
                {
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objFacebookUser.UserId);
                    IncrementCounters();
                    AddSendRequestDataToDatabase(scrapeResult);
                    jobProcessResult.IsProcessSuceessfull = true;
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objFacebookUser.UserId, "");
                    jobProcessResult.IsProcessSuceessfull = false;
                }
                DelayBeforeNextActivity();
            }

            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return jobProcessResult;
        }


        public override void StartOtherConfiguration(ScrapeResultNew scrapeResult)
        {

        }



        private void AddSendRequestDataToDatabase(ScrapeResultNew scrapeResult)
        {
            try
            {

                FacebookUser user = (FacebookUser)scrapeResult.ResultUser;
                FdEvents objEventDetails = (FdEvents)scrapeResult.ResultEvent;
                var jobActivityConfigurationManager = InstanceProvider.GetInstance<IJobActivityConfigurationManager>();

                var moduleConfiguration = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];
                if (moduleConfiguration != null && moduleConfiguration.IsTemplateMadeByCampaignMode)
                {
                    DbCampaignService.Add(new CampaignInteractedUsres
                    {
                        ActivityType = ActivityType.ToString(),
                        QueryType = scrapeResult.QueryInfo.QueryType,
                        QueryValue = scrapeResult.QueryInfo.QueryValue,
                        AccountEmail = DominatorAccountModel.AccountBaseModel.UserName,
                        UserId = user.UserId,
                        Username = user.Familyname,
                        UserProfileUrl = user.ProfileUrl,
                        Gender = user.Gender,
                        University = user.University,
                        Workplace = user.WorkPlace,
                        CurrentCity = user.Currentcity,
                        HomeTown = user.Hometown,
                        BirthDate = user.DateOfBirth,
                        ContactNo = user.ContactNo,
                        ProfilePic = user.ProfilePicUrl,
                        DetailedUserInfo = JsonConvert.SerializeObject(objEventDetails),
                        InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                        InteractionDateTime = DateTime.Now


                    });
                }

                DbAccountService.Add(new AccountInteractedUsres
                {
                    ActivityType = ActivityType.ToString(),
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    QueryValue = scrapeResult.QueryInfo.QueryValue,
                    UserId = user.UserId,
                    Username = user.Familyname,
                    UserProfileUrl = user.ProfileUrl,
                    DetailedUserInfo = JsonConvert.SerializeObject(objEventDetails),
                    InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
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
