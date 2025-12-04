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

namespace FaceDominatorCore.FDLibrary.FdProcessors.UserProecessor
{
    public class GraphSearchUserPrcessor : BaseFbUserProcessor
    {
        private IResponseHandler _objFdSearchPeopleResponseHandlerr;

        public GraphSearchUserPrcessor(IFdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
            IDbCampaignServiceScoped campaignService, IFdRequestLibrary objFdRequestLibrary,
            IFdBrowserManager browserManager, IProcessScopeModel processScopeModel)
            : base(jobProcess, dbAccountService, campaignService, objFdRequestLibrary, browserManager, processScopeModel)
        {

        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();


            string keyword = queryInfo.QueryValue;

            if (AccountModel.IsRunProcessThroughBrowser)
                Browsermanager.SearchByGraphSearchUrl(AccountModel, FbEntityType.User, keyword);


            try
            {
                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                {
                    try
                    {
                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();


                        _objFdSearchPeopleResponseHandlerr = AccountModel.IsRunProcessThroughBrowser
                            ? Browsermanager.ScrollWindowAndGetData(AccountModel, FbEntityType.User, 3, 0)
                            : ObjFdRequestLibrary.SearchPeopleFromGraphSearch(AccountModel, keyword, _objFdSearchPeopleResponseHandlerr, JobProcess.JobCancellationTokenSource.Token);

                        if (_objFdSearchPeopleResponseHandlerr.Status)
                        {
                            List<FacebookUser> lstFacebookUser = _objFdSearchPeopleResponseHandlerr.ObjFdScraperResponseParameters.ListUser;

                            GlobusLogHelper.log.Info(Log.FoundXResults, AccountModel.AccountBaseModel.AccountNetwork,
                                AccountModel.AccountBaseModel.UserName, lstFacebookUser.Count, queryInfo.QueryType,
                                        queryInfo.QueryValue, _ActivityType);

                            ProcessDataOfUsers(queryInfo, ref jobProcessResult, lstFacebookUser);
                            jobProcessResult.maxId = _objFdSearchPeopleResponseHandlerr.PageletData;

                            jobProcessResult.HasNoResult = !_objFdSearchPeopleResponseHandlerr.HasMoreResults;

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
