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
    public class KeywordUserProcessor : BaseFbUserProcessor
    {
#pragma warning disable 414
        FdSearchPeopleResponseHandler _objFdSearchPeopleResponseHandler;
#pragma warning restore 414


        public KeywordUserProcessor(IFdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
            IDbCampaignServiceScoped campaignService, IFdRequestLibrary objFdRequestLibrary,
            IFdBrowserManager browserManager, IProcessScopeModel processScopeModel)
            : base(jobProcess, dbAccountService, campaignService, objFdRequestLibrary, browserManager, processScopeModel)
        {
            _objFdSearchPeopleResponseHandler = null;
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            string keyword = queryInfo.QueryValue;
            List<string> listSavedIds = new List<string>();

            if (AccountModel.IsRunProcessThroughBrowser)
                Browsermanager.SearchByKeywordOrHashTag(AccountModel, SearchKeywordType.People, keyword);

            IResponseHandler objFdSearchPeopleResponseHandlerrBrowser = null;

            try
            {

                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    try
                    {

                        objFdSearchPeopleResponseHandlerrBrowser = AccountModel.IsRunProcessThroughBrowser
                            ? Browsermanager.ScrollWindowAndGetData(AccountModel, FbEntityType.User, noOfPageToScroll: 2, className: FdConstants.KeywordClassElement3, listSavedIds: listSavedIds)
                            : ObjFdRequestLibrary.SearchPeopleFromKeyword(AccountModel, keyword, objFdSearchPeopleResponseHandlerrBrowser);

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
                            jobProcessResult.maxId = objFdSearchPeopleResponseHandlerrBrowser.PageletData;

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
