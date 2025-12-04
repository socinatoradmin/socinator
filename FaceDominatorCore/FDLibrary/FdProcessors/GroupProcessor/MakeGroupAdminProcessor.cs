using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.FdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDLibrary.FdFunctions.FdBrowserManager;
using FaceDominatorCore.FDLibrary.FdProcesses;
using FaceDominatorCore.FDModel.InviterModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaceDominatorCore.FDLibrary.FdProcessors.GroupProcessor
{
    public class MakeGroupAdminProcessor : BaseFbGroupProcessor
    {
        public MakeGroupAdminProcessor(IFdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
            IDbCampaignServiceScoped campaignService, IFdRequestLibrary objFdRequestLibrary,
            IFdBrowserManager browserManager, IProcessScopeModel processScopeModel)
            : base(jobProcess, dbAccountService, campaignService, objFdRequestLibrary, browserManager, processScopeModel)
        {

        }
        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {

            if (string.IsNullOrEmpty(AccountModel.SessionId))
                ObjFdRequestLibrary.Login(AccountModel);

            FacebookUser facebookUser = new FacebookUser();

            SelectAccountDetailsModel selectAccountDetailsModel = new SelectAccountDetailsModel();
            var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
            var groupsModel = JsonConvert.DeserializeObject<GroupInviterModel>(templatesFileManager.Get().FirstOrDefault(x => x.Id == JobProcess.TemplateId)?.ActivitySettings);
            var currentUserActivities = selectAccountDetailsModel.GetGroupInviterDetails(groupsModel.SelectAccountDetailsModel).GroupInviterDetails;

            //var currentUserActivities =
            //    JobProcess.ModuleSetting.SelectAccountDetailsModel.GroupInviterDetails.FindAll(x =>
            //        x.Item1 == AccountModel.AccountBaseModel.AccountId);

            //sorted perticular account Details
            var accontDetails =
                JobProcess.ModuleSetting.SelectAccountDetailsModel.AccountGroupPair.Where(x =>
                    x.Key == AccountModel.AccountBaseModel.AccountId);

            try
            {
                foreach (var accontDetail in accontDetails)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    if (!ObjFdRequestLibrary.IsGroupAdmin(AccountModel, accontDetail.Value))
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                            AccountModel.AccountBaseModel.UserName, ActivityType.MakeAdmin,
                            string.Format("LangKeyIsNotAdmin".FromResourceDictionary(), $"{accontDetail.Value}"));
                        continue;
                    }

                    List<string> groupMembers = new List<string>();

                    //checks user is admin for Group or not
                    //if (AccountModel.IsRunProcessThroughBrowser)
                    //    groupMembers = Browsermanager
                    //        .GetAllGroupMembers(AccountModel, accontDetail.Value.Replace(FdConstants.FbHomeUrl, string.Empty)).ListMember;
                    //else
                    groupMembers = ObjFdRequestLibrary
                        .GetAllGroupMembers(AccountModel, accontDetail.Value.Replace(FdConstants.FbHomeUrl, string.Empty)).ListMember;

                    var groupWithFriends = currentUserActivities.FindAll(x => x.Item2 == accontDetail.Value);

                    foreach (var groupWithFriend in groupWithFriends)
                    {
                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        //try to get Group details and Friend details from Database
                        string groupId = groupWithFriend.Item2.Replace(FdConstants.FbHomeUrl, string.Empty);
                        var groupDetails = _dbAccountService.Get<OwnGroups>(x => x.GroupId == groupId).FirstOrDefault();

                        var friendDetails = GetFriendDetailsFromDataBase(groupWithFriend);
                        if (friendDetails != null)
                        {
                            facebookUser.UserId = friendDetails.FriendId;
                            facebookUser.ProfileUrl = friendDetails.ProfileUrl;
                            facebookUser.Familyname = friendDetails.FullName;
                            facebookUser.ScrapedProfileUrl = groupWithFriend.Item3;
                        }

                        if (friendDetails == null)
                            facebookUser = ObjFdRequestLibrary.GetDetailedInfoUserMobileScraper(
                                new FacebookUser() { UserId = groupWithFriend.Item3 }, AccountModel, false, false).ObjFdScraperResponseParameters.FacebookUser;

                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                        if (!string.IsNullOrEmpty(facebookUser.UserId))
                        {
                            if (!groupMembers.Contains(facebookUser.UserId))
                                continue;

                            if (CheckAlreadyInterectedUserWithGroup(groupDetails, facebookUser))
                            {
                                GlobusLogHelper.log.Info(Log.CustomMessage,
                                    AccountModel.AccountBaseModel.AccountNetwork,
                                    AccountModel.AccountBaseModel.UserName, _ActivityType,
                                    string.Format("LangKeyInvitateUser".FromResourceDictionary(), $"{facebookUser.ProfileUrl}", $"{groupDetails.GroupUrl}!"));
                                continue;
                            }

                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                            FilterAndStartFinalProcessForEachGroupInvite("Profile", facebookUser, groupDetails,
                                out jobProcessResult);
                        }
                    }
                }
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            jobProcessResult.IsProcessCompleted = true;
        }

        private Friends GetFriendDetailsFromDataBase(Tuple<string, string, string> groupWithFriend)
        {
            var friendDetails = _dbAccountService.Get<Friends>(x => groupWithFriend.Item3.Contains(x.FriendId))
                .ToList().FirstOrDefault();

            if (friendDetails == null)
            {
                string friendId = ObjFdRequestLibrary.GetFriendUserId(AccountModel, groupWithFriend.Item3).UserId;

                friendDetails = _dbAccountService.Get<Friends>(x => x.ProfileUrl.Contains(friendId)).ToList()
                    .FirstOrDefault();
            }

            return friendDetails;
        }

        private bool CheckAlreadyInterectedUserWithGroup(OwnGroups groupDetails, FacebookUser facebookUser)
        {
            return _dbAccountService.GetInteractedGroups(_ActivityType)
                .Where(x => x.GroupUrl == groupDetails.GroupUrl && x.UserId == facebookUser.UserId).Count() != 0 ? true : false;
        }

        private void FilterAndStartFinalProcessForEachGroupInvite(string query, FacebookUser friendDetail, OwnGroups group, out JobProcessResult jobProcessResult)
        {
            try
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                QueryInfo objQueryInfo = new QueryInfo { QueryType = query, QueryValue = friendDetail.ProfileUrl };

                GroupDetails objGroupDetails = new GroupDetails
                {
                    GroupId = group.GroupId,
                    GroupUrl = group.GroupUrl,
                    GroupName = group.GroupName
                };

                jobProcessResult = JobProcess.FinalProcess(new ScrapeResultNew()
                {
                    ResultUser = friendDetail,
                    QueryInfo = objQueryInfo,
                    ResultGroup = objGroupDetails
                });
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                jobProcessResult = new JobProcessResult();
            }
        }



    }
}
