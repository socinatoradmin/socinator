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
using FaceDominatorCore.Interface;
using Newtonsoft.Json;
using System;
using AccountInteractedUser = DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedUsers;
using CampaignInteractedUser = DominatorHouseCore.DatabaseHandler.FdTables.Campaigns.InteractedUsers;

namespace FaceDominatorCore.FDLibrary.FdProcesses
{


    public class GroupInviterProcess : FdJobProcessInteracted<AccountInteractedUser>
    {
        public GroupInviterModel GroupInviterModel { get; set; }

        //public Queue<string> QueMessage = new Queue<string>();

        public DominatorAccountModel Account { get; set; }

        private readonly IFdRequestLibrary _fdRequestLibrary;

        public GroupInviterProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped,
            IFdQueryScraperFactory queryScraperFactory,
            IFdHttpHelper fdHttpHelper, IFdLoginProcess fdLogInProcess, IFdRequestLibrary fdRequestLibrary, IDbCampaignServiceScoped dbCampaignServiceScoped)
            : base(processScopeModel, accountServiceScoped, queryScraperFactory, fdHttpHelper, fdLogInProcess, dbCampaignServiceScoped)
        {
            this._fdRequestLibrary = fdRequestLibrary;
            GroupInviterModel = processScopeModel.GetActivitySettingsAs<GroupInviterModel>();
            AccountModel = DominatorAccountModel;
            //TemplateId = template;
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            JobProcessResult jobProcessResult = new JobProcessResult();

            FacebookUser objFacebookUser = (FacebookUser)scrapeResult.ResultUser;

            GroupDetails objGroupDetails = (GroupDetails)scrapeResult.ResultGroup;




            try
            {

                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                if (AccountModel.IsRunProcessThroughBrowser)
                    FdLogInProcess._browserManager.GetFullGroupDetails(AccountModel, objGroupDetails.GroupUrl, false);

                bool Status = false;
                IResponseHandler responseHandler = null;
                if (AccountModel.IsRunProcessThroughBrowser)
                {
                    responseHandler = FdLogInProcess._browserManager.InviteGroups(AccountModel, objFacebookUser,
                         GroupInviterModel.InviterOptionsModel.Note);
                    Status = responseHandler.Status;
                }
                else
                {
                    Status = _fdRequestLibrary.SendGroupInvittationTofriends(AccountModel, objGroupDetails.GroupId,
                             objFacebookUser, GroupInviterModel.InviterOptionsModel.IsSendInvitationWithNote
                            ? GroupInviterModel.InviterOptionsModel.Note
                            : string.Empty).Status;
                }

                if (Status)
                {
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objFacebookUser.UserId);
                    IncrementCounters();
                    AddSendRequestDataToDatabase(scrapeResult);
                    jobProcessResult.IsProcessSuceessfull = true;
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        $"Failed to invite user {objFacebookUser.ProfileUrl} with error: {responseHandler?.ObjFdScraperResponseParameters?.FailedReason}");
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
            //base.ModuleSetting.
        }



        private void AddSendRequestDataToDatabase(ScrapeResultNew scrapeResult)
        {
            try
            {
                FacebookUser user = (FacebookUser)scrapeResult.ResultUser;

                GroupDetails objGroupDetails = (GroupDetails)scrapeResult.ResultGroup;

                var jobActivityConfigurationManager = InstanceProvider.GetInstance<IJobActivityConfigurationManager>();

                var moduleConfiguration = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

                if (moduleConfiguration != null && moduleConfiguration.IsTemplateMadeByCampaignMode)
                {
                    DbCampaignService.Add(new CampaignInteractedUser
                    {
                        ActivityType = ActivityType.ToString(),
                        QueryType = GroupInviterModel.InviterOptionsModel.IsSendInvitationInMessanger.ToString(),
                        QueryValue = GroupInviterModel.InviterOptionsModel.Note,
                        AccountEmail = DominatorAccountModel.AccountBaseModel.UserName,
                        UserId = user.UserId,
                        Username = user.Familyname,
                        UserProfileUrl = FdConstants.FbHomeUrl + user.UserId,
                        ScrapedProfileUrl = user.ScrapedProfileUrl,
                        Gender = user.Gender,
                        University = user.University,
                        Workplace = user.WorkPlace,
                        CurrentCity = user.Currentcity,
                        HomeTown = user.Hometown,
                        BirthDate = user.DateOfBirth,
                        ContactNo = user.ContactNo,
                        ProfilePic = user.ProfilePicUrl,
                        InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                        InteractionDateTime = DateTime.Now,
                        DetailedUserInfo = JsonConvert.SerializeObject(objGroupDetails)

                    });
                }

                DbAccountService.Add(new AccountInteractedUser
                {
                    ActivityType = ActivityType.ToString(),
                    QueryType = GroupInviterModel.InviterOptionsModel.IsSendInvitationInMessanger.ToString(),
                    QueryValue = GroupInviterModel.InviterOptionsModel.Note,
                    UserId = user.UserId,
                    Username = user.Familyname,
                    UserProfileUrl = FdConstants.FbHomeUrl + user.UserId,
                    ScrapedProfileUrl = user.ScrapedProfileUrl,
                    InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                    InteractionDateTime = DateTime.Now,
                    DetailedUserInfo = JsonConvert.SerializeObject(objGroupDetails)

                });
            }
            catch (Exception ex)
            {
                ex.ErrorLog(ex.Message);
            }
        }




    }
}
