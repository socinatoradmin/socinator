using DominatorHouseCore;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace FaceDominatorCore.FDLibrary.FdProcessors.UserProecessor
{

    public class UnfollowUserProcessor : BaseFbNonQueryUserProcessor
    {
        public UnfollowUserProcessor(IFdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
            IDbCampaignServiceScoped campaignService, IFdRequestLibrary objFdRequestLibrary,
            IFdBrowserManager browserManager, IProcessScopeModel processScopeModel)
            : base(jobProcess, dbAccountService, campaignService, objFdRequestLibrary, browserManager, processScopeModel)
        {
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            try
            {

                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    //All Selected Quey Value Will be added in List
                    List<string> queryList = new List<string>();

                    if (JobProcess.ModuleSetting.UnfriendOptionModel.IsAddedThroughSoftware
                       && JobProcess.ModuleSetting.UnfriendOptionModel.IsAddedOutsideSoftware)
                        queryList.Add("Both");
                    else if (JobProcess.ModuleSetting.UnfriendOptionModel.IsAddedThroughSoftware
                             || JobProcess.ModuleSetting.UnfriendOptionModel.IsAddedOutsideSoftware)
                    {
                        if (JobProcess.ModuleSetting.UnfriendOptionModel.IsAddedThroughSoftware)
                            queryList.Add("Through Software");
                        if (JobProcess.ModuleSetting.UnfriendOptionModel.IsAddedOutsideSoftware)
                            queryList.Add("Outside Software");
                    }
                    if (JobProcess.ModuleSetting.UnfriendOptionModel.IsCustomUserList)
                        queryList.Add("Custom");
                    if (JobProcess.ModuleSetting.UnfriendOptionModel.IsMutualFriends)
                        queryList.Add("UnfollowMutualFriend");

                    FilterAndStartFinalProcessForUnfollowFriendWithQueryList(ref jobProcessResult, queryList, "Unfollow");
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Requested Cancelled !");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            jobProcessResult.HasNoResult = true;
        }

        private void FilterAndStartFinalProcessForUnfollowFriendWithQueryList(ref JobProcessResult jobProcessResult, List<string> queryList, string queryValue)
        {
            //All User Ids will be added in single List
            List<FacebookUser> lstFilteredUserIds = new List<FacebookUser>();
            var query = string.Empty;

            try
            {
                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    if (queryList.Contains("Both") || queryList.Contains("Through Software") ||
                        queryList.Contains("Outside Software"))
                    {
                        query = queryList.Contains("Both") ? "Both" :
                            queryList.Contains("Through Software") ? "Through Software" : "Outside Software";

                        //It Will Updated All friends List And filter the values by above content Through or outside Software
                        lstFilteredUserIds.AddRange(FilterBySourceTypeWithQueryList(new QueryInfo { QueryType = query }).Result);
                    }

                    if (queryList.Contains("Custom"))
                    {
                        query = "Custom";
                        JobProcess.ModuleSetting.UnfriendOptionModel.LstCustomUsers.ForEach(x =>
                        {
                            var _userInfoResponseHandler = ObjFdRequestLibrary.GetDetailedInfoUserMobileScraper
                                (new FacebookUser() { ProfileUrl = x, QueryType = "Custom" }, AccountModel, false, true);

                            Thread.Sleep(2000);

                            if (_userInfoResponseHandler != null && _userInfoResponseHandler?.ObjFdScraperResponseParameters?.FacebookUser.UserId != "0")
                                lstFilteredUserIds.Add(_userInfoResponseHandler.ObjFdScraperResponseParameters.FacebookUser);
                            else
                            {
                                lstFilteredUserIds.Add(FdConstants.getFaceBookUserFromUrlOrIdOrUserName(x));
                            }
                        });
                    }

                    if (queryList.Contains("UnfollowMutualFriend"))
                    {
                        query = "UnfollowMutualFriend";
                        //var mutualFriendList = objFdFunctions.GetMutualFriendBetweenAccounts(AccountModel);
                        List<FacebookUser> mutualFriendList = null;
                        if (AccountModel.IsRunProcessThroughBrowser)
                        {
                            Browsermanager.LoadPageSource(AccountModel, FdConstants.MutualFriendUrl(AccountModel.AccountBaseModel.UserId));

                            lstFilteredUserIds.AddRange(Browsermanager.ScrollWindowAndGetFriends(AccountModel, FbEntityType.Unfriend, 5, 0, FdConstants.UnfriendElement).ObjFdScraperResponseParameters.ListUser);
                        }
                        else
                        {
                            mutualFriendList = ObjFdRequestLibrary.GetAllMutualFriends(AccountModel, AccountModel.AccountBaseModel.UserId);

                            lstFilteredUserIds.AddRange((from user in mutualFriendList
                                                         select new FacebookUser
                                                         {
                                                             UserId = user.UserId,
                                                             Familyname = user.FullName,
                                                             ProfileUrl = string.IsNullOrEmpty(user.ProfileUrl) ? $"{FdConstants.FbHomeUrl}{user.UserId}" : user.ProfileUrl,
                                                             QueryType = "UnfriendMutualFriend"
                                                         }).ToList());
                        }


                    }

                    jobProcessResult.IsProcessCompleted = lstFilteredUserIds.Count == 0;

                    GlobusLogHelper.log.Info(Log.FoundXResults, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, lstFilteredUserIds.Count, "Unfriend", "", _ActivityType);

                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    ProcessDataOfUsers(ref jobProcessResult, lstFilteredUserIds, query, queryValue);

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
    }
}
