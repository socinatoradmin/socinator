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

namespace FaceDominatorCore.FDLibrary.FdProcessors.GroupProcessor
{
    public class KeywordGroupProcessor : BaseFbGroupProcessor
    {
        private IResponseHandler _groupResponseHandler;
        public KeywordGroupProcessor(IFdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
            IDbCampaignServiceScoped campaignService, IFdRequestLibrary objFdRequestLibrary,
            IFdBrowserManager browserManager, IProcessScopeModel processScopeModel)
            : base(jobProcess, dbAccountService, campaignService, objFdRequestLibrary, browserManager, processScopeModel)
        {
            _groupResponseHandler = null;

        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {

            string keyword = queryInfo.QueryValue;

            GroupType objGroupType = GroupType.Any;

            GroupMemberShip objGroupMemberShip = GroupMemberShip.AnyGroup;

            ApplyGroupFilter(ref objGroupMemberShip, ref objGroupType, queryInfo);

            if (AccountModel.IsRunProcessThroughBrowser)
                Browsermanager.SearchByKeywordOrHashTag(AccountModel, SearchKeywordType.Groups, keyword);

            Browsermanager.ApplyGroupFilters(AccountModel, objGroupType, objGroupMemberShip);

            try
            {

                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    try
                    {
                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        string className = "x78zum5 x1n2onr6 xh8yej3";

                        _groupResponseHandler = AccountModel.IsRunProcessThroughBrowser
                            ? Browsermanager.ScrollWindowAndGetData(AccountModel, FbEntityType.Groups, 4, 0, className: className)
                            : ObjFdRequestLibrary.ScrapGroups(AccountModel, keyword, objGroupMemberShip, objGroupType, _groupResponseHandler, queryInfo.QueryType);

                        if (_groupResponseHandler.Status)
                        {
                            List<GroupDetails> listGroupDetails = _groupResponseHandler.ObjFdScraperResponseParameters
                                        .ListGroup;

                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                            GlobusLogHelper.log.Info(Log.FoundXResults, AccountModel.AccountBaseModel.AccountNetwork,
                                AccountModel.AccountBaseModel.UserName, listGroupDetails.Count, queryInfo.QueryType,
                                queryInfo.QueryValue, _ActivityType);

                            ProcessDataOfGroups(queryInfo, ref jobProcessResult, listGroupDetails);
                            jobProcessResult.maxId = _groupResponseHandler.PageletData;

                            if (_groupResponseHandler.HasMoreResults == false)
                                jobProcessResult.HasNoResult = true;

                        }
                        else
                        {
                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
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
