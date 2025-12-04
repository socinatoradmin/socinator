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
using FaceDominatorCore.FDResponse.CommonResponse;
using FaceDominatorCore.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaceDominatorCore.FDLibrary.FdProcessors.UserProecessor
{
    public class LocationUserProcessor : BaseFbUserProcessor
    {
        FdSearchPeopleResponseHandler _objFdSearchPeopleResponseHandlerr;

        public LocationUserProcessor(IFdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
            IDbCampaignServiceScoped campaignService, IFdRequestLibrary objFdRequestLibrary,
            IFdBrowserManager browserManager, IProcessScopeModel processScopeModel)
            : base(jobProcess, dbAccountService, campaignService, objFdRequestLibrary, browserManager, processScopeModel)
        {
            _objFdSearchPeopleResponseHandlerr = null;

        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            string keyword = $"people in {queryInfo.QueryValue}";
            List<string> listSavedIds = new List<string>();

            if (AccountModel.IsRunProcessThroughBrowser)
                Browsermanager.SearchByKeywordOrHashTag(AccountModel, SearchKeywordType.People, keyword);

            IResponseHandler objFdSearchPeopleResponseHandlerrBrowser = null;

            try
            {
                string className = FdConstants.KeywordClassElement3;

                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    try
                    {
                        objFdSearchPeopleResponseHandlerrBrowser = AccountModel.IsRunProcessThroughBrowser
                            ? Browsermanager.ScrollWindowAndGetData(AccountModel, FbEntityType.User, 2, 0, listSavedIds: listSavedIds, className: className)
                            : ObjFdRequestLibrary.SearchPeopleByLocation(AccountModel, keyword, _objFdSearchPeopleResponseHandlerr);

                        if (objFdSearchPeopleResponseHandlerrBrowser.Status)
                        {
                            List<FacebookUser> lstFacebookUser = objFdSearchPeopleResponseHandlerrBrowser.ObjFdScraperResponseParameters.ListUser;

                            if (AccountModel.IsRunProcessThroughBrowser)
                                listSavedIds.AddRange(lstFacebookUser.Select(x => x.UserId));

                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                            GlobusLogHelper.log.Info(Log.FoundXResults, AccountModel.AccountBaseModel.AccountNetwork,
                                AccountModel.AccountBaseModel.UserName, lstFacebookUser.Count, queryInfo.QueryType,
                                        queryInfo.QueryValue, _ActivityType);

                            ProcessDataOfUsers(queryInfo, ref jobProcessResult, lstFacebookUser);
                            jobProcessResult.maxId = objFdSearchPeopleResponseHandlerrBrowser.PageletData ?? string.Empty;

                            jobProcessResult.HasNoResult = !objFdSearchPeopleResponseHandlerrBrowser.HasMoreResults;

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
