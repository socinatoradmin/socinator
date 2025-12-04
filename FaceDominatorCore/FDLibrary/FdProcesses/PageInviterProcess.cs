using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDFactories;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDLibrary.FdFunctions.FdBrowserManager;
using FaceDominatorCore.FDModel.InviterModel;
using FaceDominatorCore.FDRequest;
using Newtonsoft.Json;
using System;
using Unity;
using AccountInteractedUsres = DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedUsers;
using CampaignInteractedUsres = DominatorHouseCore.DatabaseHandler.FdTables.Campaigns.InteractedUsers;

namespace FaceDominatorCore.FDLibrary.FdProcesses
{

    public class PageInviterProcess : FdJobProcessInteracted<AccountInteractedUsres>
    {
        public FanpageInviterModel FanpageInviterModel { get; set; }

        //        public Queue<string> QueMessage = new Queue<string>();

        public DominatorAccountModel Account { get; set; }

        private readonly IFdRequestLibrary _fdRequestLibrary;

        public PageInviterProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IFdQueryScraperFactory queryScraperFactory,
            IFdHttpHelper fdHttpHelper, IFdLoginProcess fdLogInProcess, IFdRequestLibrary fdRequestLibrary,
            IDbCampaignServiceScoped dbCampaignServiceScoped)
            : base(processScopeModel, accountServiceScoped, queryScraperFactory, fdHttpHelper, fdLogInProcess, dbCampaignServiceScoped)
        {
            _fdRequestLibrary = fdRequestLibrary;
            FanpageInviterModel = processScopeModel.GetActivitySettingsAs<FanpageInviterModel>();
            AccountModel = DominatorAccountModel;
            //TemplateId = template;
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            JobProcessResult jobProcessResult = new JobProcessResult();

            FacebookUser objFacebookUser = (FacebookUser)scrapeResult.ResultUser;

            var query = scrapeResult.QueryInfo;


            FanpageDetails objPageDetails = (FanpageDetails)scrapeResult.ResultPage;

            var _accountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();

            IFdBrowserManager browserManager = null;

            try
            {

                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                bool isSuccess;

                if (AccountModel.IsRunProcessThroughBrowser && (!FanpageInviterModel.InviterOptionsModel.IsSendInvitationWithNote ||
                        (FanpageInviterModel.InviterOptionsModel.IsSendInvitationWithNote && FanpageInviterModel.InviterOptionsModel.Note.
                        Trim() == ConstantVariable.PageInviterNote.Trim())))
                {
                    if (query.QueryType != "profile")
                    {
                        browserManager = _accountScopeFactory[$"{objFacebookUser.UserId}{AccountModel.AccountId}"]
                                .Resolve<IFdBrowserManager>();
                        browserManager.GetFullPageDetails(AccountModel, objPageDetails, true, false);
                    }
                    else
                        FdLogInProcess._browserManager.GetFullPageDetails(AccountModel, objPageDetails, false, false);

                }


                if (query.QueryType != "profile")
                {
                    isSuccess = AccountModel.IsRunProcessThroughBrowser && (!FanpageInviterModel.InviterOptionsModel.IsSendInvitationWithNote ||
                    (FanpageInviterModel.InviterOptionsModel.IsSendInvitationWithNote &&
                    FanpageInviterModel.InviterOptionsModel.Note.Trim() == ConstantVariable.PageInviterNote.Trim())) ?
                    browserManager.InvitePages(AccountModel, objFacebookUser, FanpageInviterModel.InviterOptionsModel)
                    : _fdRequestLibrary.SendPageInvittationTofriends(AccountModel, objPageDetails.FanPageID,
                    FanpageInviterModel.InviterOptionsModel.IsSendInvitationWithNote ? FanpageInviterModel.InviterOptionsModel.Note
                    : string.Empty, objFacebookUser, FanpageInviterModel.InviterOptionsModel.IsSendInvitationInMessanger);
                }
                else
                {
                    isSuccess = AccountModel.IsRunProcessThroughBrowser && (!FanpageInviterModel.InviterOptionsModel.IsSendInvitationWithNote ||
                    (FanpageInviterModel.InviterOptionsModel.IsSendInvitationWithNote &&
                    FanpageInviterModel.InviterOptionsModel.Note.Trim() == ConstantVariable.PageInviterNote.Trim()))
                    ? FdLogInProcess._browserManager.InvitePages(AccountModel, objFacebookUser, FanpageInviterModel.InviterOptionsModel)
                    : _fdRequestLibrary.SendPageInvittationTofriends(AccountModel, objPageDetails.FanPageID,
                    FanpageInviterModel.InviterOptionsModel.IsSendInvitationWithNote ? FanpageInviterModel.InviterOptionsModel.Note
                    : string.Empty, objFacebookUser, FanpageInviterModel.InviterOptionsModel.IsSendInvitationInMessanger);
                }


                if (isSuccess)
                {

                    GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objFacebookUser.UserId);

                    if (FanpageInviterModel.InviterOptionsModel.IsSendInvitationInMessanger || FanpageInviterModel.InviterOptionsModel.IsSendInvitationWithNote)
                        GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                            AccountModel.AccountBaseModel.UserName, "",
                            "Successfully sent invitation in messanger");

                    IncrementCounters();
                    AddSendRequestDataToDatabase(scrapeResult);
                    jobProcessResult.IsProcessSuceessfull = true;
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.ActivityFailed,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $"Either {objFacebookUser.UserId} is not your friend or Page is not suggesting to send request", "");
                    jobProcessResult.IsProcessSuceessfull = false;
                }

                if (DominatorAccountModel.IsRunProcessThroughBrowser && query.QueryType != "profile")
                    browserManager.CloseBrowser(DominatorAccountModel);

                DelayBeforeNextActivity();
            }

            catch (OperationCanceledException)
            {
                if (DominatorAccountModel.IsRunProcessThroughBrowser && query.QueryType != "profile")
                    browserManager.CloseBrowser(DominatorAccountModel);
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                if (DominatorAccountModel.IsRunProcessThroughBrowser && query.QueryType != "profile")
                    browserManager.CloseBrowser(DominatorAccountModel);
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

                FanpageDetails objGroupDetails = (FanpageDetails)scrapeResult.ResultPage;

                var jobActivityConfigurationManager = InstanceProvider.GetInstance<IJobActivityConfigurationManager>();

                var moduleConfiguration = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

                if (moduleConfiguration != null && moduleConfiguration.IsTemplateMadeByCampaignMode)
                {
                    DbCampaignService.Add(new CampaignInteractedUsres
                    {
                        ActivityType = ActivityType.ToString(),
                        QueryType = FanpageInviterModel.InviterOptionsModel.IsSendInvitationInMessanger.ToString(),
                        QueryValue = FanpageInviterModel.InviterOptionsModel.IsSendInvitationWithNote ? FanpageInviterModel.InviterOptionsModel.Note : string.Empty,
                        AccountEmail = DominatorAccountModel.AccountBaseModel.UserName,
                        UserId = user.UserId,
                        Username = user.Familyname,
                        UserProfileUrl = user.ProfileUrl,
                        ScrapedProfileUrl = user.ScrapedProfileUrl,
                        DetailedUserInfo = JsonConvert.SerializeObject(objGroupDetails),
                        Gender = user.Gender,
                        University = user.University,
                        Workplace = user.WorkPlace,
                        CurrentCity = user.Currentcity,
                        HomeTown = user.Hometown,
                        BirthDate = user.DateOfBirth,
                        ContactNo = user.ContactNo,
                        ProfilePic = user.ProfilePicUrl,
                        InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                        InteractionDateTime = DateTime.Now


                    });
                }

                DbAccountService.Add(new AccountInteractedUsres
                {
                    ActivityType = ActivityType.ToString(),
                    QueryType = FanpageInviterModel.InviterOptionsModel.IsSendInvitationInMessanger.ToString(),
                    QueryValue = FanpageInviterModel.InviterOptionsModel.IsSendInvitationWithNote ? FanpageInviterModel.InviterOptionsModel.Note : string.Empty,
                    UserId = user.UserId,
                    Username = user.Familyname,
                    UserProfileUrl = user.ProfileUrl,
                    ScrapedProfileUrl = user.ScrapedProfileUrl,
                    DetailedUserInfo = JsonConvert.SerializeObject(objGroupDetails),
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
