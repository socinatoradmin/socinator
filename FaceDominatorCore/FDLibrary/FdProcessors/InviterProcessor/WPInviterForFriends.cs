using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.FdTables.Accounts;
using DominatorHouseCore.Enums.FdQuery;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDLibrary.FdFunctions.FdBrowserManager;
using FaceDominatorCore.FDLibrary.FdProcesses;
using FaceDominatorCore.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaceDominatorCore.FDLibrary.FdProcessors.InviterProcessor
{
    public class WpInviterForFriends : BaseFbInviterProcessor
    {
        private readonly IDbAccountService _objDbAccountService;

        public WpInviterForFriends(IFdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
            IDbCampaignServiceScoped campaignService, IFdRequestLibrary objFdRequestLibrary,
            IFdBrowserManager browserManager, IProcessScopeModel processScopeModel)
            : base(jobProcess, dbAccountService, campaignService, objFdRequestLibrary, browserManager, processScopeModel)
        {

            _objDbAccountService = dbAccountService;
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            try
            {

                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                var objListWatchPartyUrl = JobProcess.ModuleSetting.InviterDetailsModel.ListWatchPartyUrls;

                objListWatchPartyUrl.Shuffle();

                foreach (var watchPart in objListWatchPartyUrl)
                {
                    FacebookPostDetails objFacebookDetails = new FacebookPostDetails { PostUrl = watchPart };


                    ObjFdRequestLibrary.GetPostDetailNew(AccountModel, objFacebookDetails, true);

                    var lstUserId = JobProcess.ModuleSetting.SelectAccountDetailsModel.AccountFriendsPair
                        .Where(y => y.Key == AccountModel.AccountId).Select(x => x.Value).ToList();

                    var friendDetails = _objDbAccountService.Get<Friends>().ToList();

                    if (objFacebookDetails.EntityType == FbEntityTypes.Group)
                        StartInviteForGroupMembers(queryInfo, ref jobProcessResult, objFacebookDetails, friendDetails);
                    else
                    {
                        List<FacebookUser> objListFacebookUser = new List<FacebookUser>();

                        foreach (var friendUrl in lstUserId)
                        {
                            try
                            {
                                if (friendDetails.FirstOrDefault(x => friendUrl == x.ProfileUrl) != null)
                                {
                                    var friend = friendDetails.FirstOrDefault(x => friendUrl == x.ProfileUrl);
                                    if (friend != null)
                                    {
                                        FacebookUser objFacebookUser = new FacebookUser()
                                        {
                                            UserId = friend.FriendId,
                                            ProfileUrl = friend.ProfileUrl,
                                            Familyname = friend.FullName,
                                            Currentcity = friend.Location
                                        };

                                        objListFacebookUser.Add(objFacebookUser);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                ex.DebugLog();
                            }
                        }

                        ProcessDataForInviter(queryInfo, ref jobProcessResult, objListFacebookUser, objFacebookDetails);

                        jobProcessResult.HasNoResult = true;
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
            jobProcessResult.HasNoResult = true;
        }


        protected void StartInviteForGroupMembers(QueryInfo queryInfo, ref JobProcessResult jobProcessResult,
            IEntity objEntity, List<Friends> lstFriends)
        {
            string groupUrl = FdConstants.FbHomeUrl + ((FacebookPostDetails)objEntity).OwnerId;

            ObjFdRequestLibrary.GetLangugae(AccountModel);

            ObjFdRequestLibrary.ChangeLanguage(AccountModel, FdConstants.AccountLanguage[AccountModel.AccountBaseModel.UserId]);

            IResponseHandler objGroupMembersResponseHandler = null;

            if (AccountModel.IsRunProcessThroughBrowser)
                Browsermanager.SearchMembersData(AccountModel, FbEntityType.Groups, (GroupMemberCategory)JobProcess.ModuleSetting.GenderAndLocationFilter.SelectedGroupMemberCategory, groupUrl);

            try
            {
                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    try
                    {
                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                        objGroupMembersResponseHandler = AccountModel.IsRunProcessThroughBrowser
                            ? Browsermanager.ScrollWindowAndGetData(AccountModel, FbEntityType.GroupMembers, 4)
                            : ObjFdRequestLibrary.GetGroupMembers(AccountModel, groupUrl, objGroupMembersResponseHandler, GroupMemberCategory.Friends);

                        if (objGroupMembersResponseHandler.Status)
                        {
                            List<FacebookUser> lstFacebookUser = objGroupMembersResponseHandler.ObjFdScraperResponseParameters.ListUser;

                            lstFacebookUser = lstFacebookUser.Where(x => lstFriends.Any(y => y.FriendId == x.UserId)).ToList();

                            GlobusLogHelper.log.Info(Log.FoundXResults, AccountModel.AccountBaseModel.AccountNetwork,
                                AccountModel.AccountBaseModel.UserName, lstFacebookUser.Count, queryInfo.QueryType,
                                        queryInfo.QueryValue, _ActivityType);

                            ProcessDataForInviter(queryInfo, ref jobProcessResult, lstFacebookUser, objEntity);
                            jobProcessResult.maxId = objGroupMembersResponseHandler.PageletData;

                            if (objGroupMembersResponseHandler.HasMoreResults == false)
                                jobProcessResult.HasNoResult = true;

                        }
                        else
                        {
                            jobProcessResult.HasNoResult = true;
                            jobProcessResult.maxId = null;
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
        }
    }
}
