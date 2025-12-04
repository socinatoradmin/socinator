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
using FaceDominatorCore.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaceDominatorCore.FDLibrary.FdProcessors.UserProecessor
{
    public class GroupMembersProcessor : BaseFbUserProcessor
    {
        IResponseHandler _objGroupMembersResponseHandler;

        public GroupMembersProcessor(IFdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
            IDbCampaignServiceScoped campaignService, IFdRequestLibrary objFdRequestLibrary,
            IFdBrowserManager browserManager, IProcessScopeModel processScopeModel)
            : base(jobProcess, dbAccountService, campaignService, objFdRequestLibrary, browserManager, processScopeModel)
        {
            _objGroupMembersResponseHandler = null;

        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            string groupUrl = queryInfo.QueryValue;
            List<string> listUsers = new List<string>();

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

                        _objGroupMembersResponseHandler = AccountModel.IsRunProcessThroughBrowser
                            ? Browsermanager.ScrollWindowAndGetData(AccountModel, FbEntityType.GroupMembers, 4, listSavedIds: listUsers)
                            : ObjFdRequestLibrary.GetGroupMembers(AccountModel, groupUrl, _objGroupMembersResponseHandler,
                                        JobProcess.ModuleSetting.GenderAndLocationFilter.IsGroupCategoryEnabled
                                            ? (GroupMemberCategory)JobProcess.ModuleSetting.GenderAndLocationFilter.SelectedGroupMemberCategory
                                            : GroupMemberCategory.AllMembers);

                        #region CommentedGroupMemberBasicDetails
                        //if (AccountModel.IsRunProcessThroughBrowser)
                        //    _objGroupMembersResponseHandler = Browsermanager.ScrollWindowAndGetData(AccountModel, FbEntityType.GroupMembers, 50);
                        //else
                        //    _objGroupMembersResponseHandler = ObjFdRequestLibrary.GetGroupMembers
                        //                (AccountModel, groupUrl, _objGroupMembersResponseHandler,
                        //                JobProcess.ModuleSetting.GenderAndLocationFilter.IsGroupCategoryEnabled ?
                        //                (GroupMemberCategory)JobProcess.ModuleSetting.GenderAndLocationFilter.SelectedGroupMemberCategory
                        //                : GroupMemberCategory.AllMembers); 
                        #endregion

                        if (_objGroupMembersResponseHandler.Status)
                        {
                            List<FacebookUser> lstFacebookUser = _objGroupMembersResponseHandler.ObjFdScraperResponseParameters.ListUser;

                            if (AccountModel.IsRunProcessThroughBrowser)
                                listUsers.AddRange(lstFacebookUser.Select(x => x.UserId));

                            GlobusLogHelper.log.Info(Log.FoundXResults, AccountModel.AccountBaseModel.AccountNetwork,
                                AccountModel.AccountBaseModel.UserName, lstFacebookUser.Count, queryInfo.QueryType,
                                        queryInfo.QueryValue, _ActivityType);

                            //if (queryInfo.QueryTypeEnum == "GroupMemberBasicDetails" &&  _ActivityType != ActivityType.ProfileScraper)
                            //    foreach (var user in lstFacebookUser)
                            //        SendToPerformActivity(ref jobProcessResult, user, queryInfo);
                            //else
                            ProcessDataOfUsers(queryInfo, ref jobProcessResult, lstFacebookUser);

                            jobProcessResult.maxId = _objGroupMembersResponseHandler.PageletData;

                            jobProcessResult.HasNoResult = !_objGroupMembersResponseHandler.HasMoreResults;
                        }
                        else
                        {
                            jobProcessResult.HasNoResult = true;
                            jobProcessResult.maxId = null;
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        throw new OperationCanceledException("Requested Cancelled !");
                    }
                    catch (Exception ex)
                    {
                        jobProcessResult.HasNoResult = true;
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

