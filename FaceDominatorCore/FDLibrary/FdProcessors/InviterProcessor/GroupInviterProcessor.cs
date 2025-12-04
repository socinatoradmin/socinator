using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.FdTables.Accounts;
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

namespace FaceDominatorCore.FDLibrary.FdProcessors.InviterProcessor
{
    public class GroupInviterProcessor : BaseFbProcessor
    {
        public GroupInviterProcessor(IFdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
            IDbCampaignServiceScoped campaignService, IFdRequestLibrary objFdRequestLibrary,
            IFdBrowserManager browserManager, IProcessScopeModel processScopeModel)
            : base(jobProcess, dbAccountService, campaignService, objFdRequestLibrary, browserManager, processScopeModel)
        {
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {

                List<Tuple<string, string>> alreadyJoinedMembers = new List<Tuple<string, string>>();

                SelectAccountDetailsModel selectAccountDetailsModel = new SelectAccountDetailsModel();

                var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
                var groupInviterModel = JsonConvert.DeserializeObject<GroupInviterModel>(templatesFileManager.Get().FirstOrDefault(x => x.Id == JobProcess.TemplateId)?.ActivitySettings);

                var listInviterDetails = selectAccountDetailsModel.GetGroupInviterDetails(groupInviterModel.SelectAccountDetailsModel).GroupInviterDetails;

                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                //Filter Data Get Already Joined Frineds
                listInviterDetails = listInviterDetails.Where(x => x.Item1 == AccountModel.AccountId).ToList();

                listInviterDetails.Shuffle();

                foreach (var inviterDetail in listInviterDetails)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    var groupUrl = inviterDetail.Item2;
                    var friendUrl = inviterDetail.Item3;

                    var pageDetails = _dbAccountService.GetInteractedUsers(_ActivityType);

                    if (pageDetails.FirstOrDefault(x => x.DetailedUserInfo.Contains(groupUrl) && (friendUrl.Contains(x.UserId) ||
                                                                                      (!string.IsNullOrEmpty(x.ScrapedProfileUrl) && x.ScrapedProfileUrl.Contains(friendUrl)))) != null)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                            AccountModel.AccountBaseModel.AccountNetwork,
                            AccountModel.AccountBaseModel.UserName, _ActivityType,
                            string.Format("LangKeyInvitateUser".FromResourceDictionary(), $"{inviterDetail.Item3}", $"{inviterDetail.Item2}!"));
                        continue;
                    }

                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    var groupDetails = _dbAccountService.Get<OwnGroups>(x => x.GroupUrl == groupUrl).FirstOrDefault();

                    if (groupDetails == null)
                    {
                        var groupId = AccountModel.IsRunProcessThroughBrowser ?
                            Browsermanager.GetFullGroupDetails(AccountModel, groupUrl) :
                            ObjFdRequestLibrary.GetGroupIdFromUrl(AccountModel, groupUrl);

                        groupDetails = _dbAccountService.Get<OwnGroups>(x => x.GroupId == groupId).FirstOrDefault();
                    }
                    try
                    {
                        FacebookUser objUser = new FacebookUser();

                        var friendDetails = _dbAccountService.Get<Friends>(x => friendUrl.Contains(x.FriendId)).ToList().FirstOrDefault();

                        if (friendDetails == null)
                        {
                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            string friendId = AccountModel.IsRunProcessThroughBrowser ?
                                Browsermanager.GetFullUserDetails(AccountModel, new FacebookUser() { ScrapedProfileUrl = friendUrl }).ObjFdScraperResponseParameters.FacebookUser.UserId :
                                ObjFdRequestLibrary.GetFriendUserId(AccountModel, friendUrl).UserId;

                            if (_dbAccountService.Get<InteractedUsers>(x =>
                                    x.DetailedUserInfo.Contains(groupDetails.GroupUrl) &&
                                    x.UserProfileUrl.Contains(friendId)).FirstOrDefault() != null)
                            {
                                GlobusLogHelper.log.Info(Log.CustomMessage,
                                    AccountModel.AccountBaseModel.AccountNetwork,

                                    AccountModel.AccountBaseModel.UserName, _ActivityType,
                                    string.Format("LangKeyInvitateUser".FromResourceDictionary(), $"{friendDetails.ProfileUrl}", $"{groupDetails.GroupId}!"));

                                continue;
                            }

                            friendDetails = _dbAccountService.Get<Friends>(x => x.ProfileUrl.Contains(friendId)).ToList()
                                .FirstOrDefault();
                        }

                        if (friendDetails != null)
                        {
                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                            if (groupInviterModel.IsFilterMemberChkd)
                            {

                                //extracting all already Members Data 
                                List<string> alreadyGroupJoinedFriends = ObjFdRequestLibrary.GetAlreadyGroupJoinedFriendsList(AccountModel, groupDetails.GroupId);

                                alreadyGroupJoinedFriends.ForEach(alreadyGroupJoinedFriend =>
                                    alreadyJoinedMembers.Add(new Tuple<string, string>(groupDetails.GroupId,
                                        alreadyGroupJoinedFriend)));

                                if (alreadyJoinedMembers.Any(x =>
                                    x.Item1.Equals(groupDetails.GroupId) && x.Item2.Equals(friendDetails.FriendId)))
                                {
                                    GlobusLogHelper.log.Info(Log.CustomMessage,
                                        AccountModel.AccountBaseModel.AccountNetwork,
                                        AccountModel.AccountBaseModel.UserName, _ActivityType,
                                        string.Format("LangKeyAlreadyJoinedUser".FromResourceDictionary(), $"{friendDetails.ProfileUrl}", $"{groupDetails.GroupId}!"));
                                    continue;
                                }
                            }

                            objUser.UserId = friendDetails.FriendId;
                            objUser.ProfileUrl = friendDetails.ProfileUrl;
                            objUser.Familyname = friendDetails.FullName;
                            objUser.ScrapedProfileUrl = inviterDetail.Item3;

                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                            FilterAndStartFinalProcessForEachGroupInvite("Profile", objUser, groupDetails,
                                out jobProcessResult);
                        }

                    }
                    catch (OperationCanceledException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                }

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
                throw new OperationCanceledException("Requested Cancelled !");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                jobProcessResult = new JobProcessResult();
            }
        }

    }
}
