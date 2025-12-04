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
    public class GraphSearchGroupProcessors : BaseFbGroupProcessor
    {
        private IResponseHandler GroupScraperResponseHandler;

        public GraphSearchGroupProcessors(IFdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
            IDbCampaignServiceScoped campaignService, IFdRequestLibrary objFdRequestLibrary,
            IFdBrowserManager browserManager, IProcessScopeModel processScopeModel)
            : base(jobProcess, dbAccountService, campaignService, objFdRequestLibrary, browserManager, processScopeModel)
        {
            GroupScraperResponseHandler = null;
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            string graphSearchUrl = queryInfo.QueryValue;

            GroupType objGroupType = GroupType.Any;

            GroupMemberShip objGroupMemberShip = GroupMemberShip.AnyGroup;

            if (AccountModel.IsRunProcessThroughBrowser)
                Browsermanager.SearchByGraphSearchUrl(AccountModel, FbEntityType.Groups, graphSearchUrl);

            ApplyGroupFilter(ref objGroupMemberShip, ref objGroupType, queryInfo);

            try
            {

                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    try
                    {
                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                        GroupScraperResponseHandler = AccountModel.IsRunProcessThroughBrowser
                            ? Browsermanager.ScrollWindowAndGetData(AccountModel, FbEntityType.Groups, 20, 0)
                            : ObjFdRequestLibrary.ScrapGroups(AccountModel, graphSearchUrl, objGroupMemberShip, objGroupType, GroupScraperResponseHandler, queryInfo.QueryType);

                        if (GroupScraperResponseHandler != null && GroupScraperResponseHandler.Status)
                        {
                            List<GroupDetails> listGroupDetails = GroupScraperResponseHandler
                                        .ObjFdScraperResponseParameters.ListGroup;

                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                            GlobusLogHelper.log.Info(Log.FoundXResults, AccountModel.AccountBaseModel.AccountNetwork,
                                AccountModel.AccountBaseModel.UserName, listGroupDetails.Count, queryInfo.QueryType,
                                        queryInfo.QueryValue, _ActivityType);

                            ProcessDataOfGroups(queryInfo, ref jobProcessResult, listGroupDetails);
                            jobProcessResult.maxId = GroupScraperResponseHandler.PageletData;

                            jobProcessResult.HasNoResult = GroupScraperResponseHandler.HasMoreResults;
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
