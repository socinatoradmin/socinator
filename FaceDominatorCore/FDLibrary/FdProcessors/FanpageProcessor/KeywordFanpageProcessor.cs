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
using FaceDominatorCore.FDResponse.ScrapersResponse;
using FaceDominatorCore.Interface;
using System;
using System.Collections.Generic;

namespace FaceDominatorCore.FDLibrary.FdProcessors.FanpageProcessor
{
    public class KeywordFanpageProcessor : BaseFbFanpageProcessor
    {
        SearchFanpageDetailsResponseHandler _objSearchFanpageDetailsResponseHandler;

        public KeywordFanpageProcessor(IFdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
            IDbCampaignServiceScoped campaignService, IFdRequestLibrary objFdRequestLibrary,
            IFdBrowserManager browserManager, IProcessScopeModel processScopeModel)
            : base(jobProcess, dbAccountService, campaignService, objFdRequestLibrary, browserManager, processScopeModel)
        {
            _objSearchFanpageDetailsResponseHandler = null;
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {

            string keyword = queryInfo.QueryValue;

            bool isVerifiedFilter = false;

            bool isLikedByFriends = false;

            FanpageCategory objFanpageCategory = FanpageCategory.AnyCategory;

            ApplyFanpageFilter(ref isVerifiedFilter, ref isLikedByFriends, ref objFanpageCategory, queryInfo);

            if (AccountModel.IsRunProcessThroughBrowser)
            {
                Browsermanager.SearchByKeywordOrHashTag(AccountModel, SearchKeywordType.Pages, keyword);

                Browsermanager.ApplyPageFilters(AccountModel, objFanpageCategory, isVerifiedFilter);
            }

            IResponseHandler responseHandler = null;

            try
            {
                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    try
                    {
                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                        responseHandler = AccountModel.IsRunProcessThroughBrowser
                            ? Browsermanager.ScrollWindowAndGetData(AccountModel, FbEntityType.Fanpage, 4, 0, className: FdConstants.KeywordClassElement2)
                            : ObjFdRequestLibrary.GetFanpageDetailsFromKeyword
                            (AccountModel, keyword, isVerifiedFilter, isLikedByFriends, objFanpageCategory, _objSearchFanpageDetailsResponseHandler);

                        if (responseHandler.Status)
                        {
                            List<FanpageDetails> listFanpageDetails = responseHandler
                                        .ObjFdScraperResponseParameters.ListPage;

                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                            GlobusLogHelper.log.Info(Log.FoundXResults, AccountModel.AccountBaseModel.AccountNetwork,
                                AccountModel.AccountBaseModel.UserName, listFanpageDetails.Count, queryInfo.QueryType,
                                        queryInfo.QueryValue, _ActivityType);

                            ProcessDataOfPages(queryInfo, ref jobProcessResult, listFanpageDetails);

                            if (AccountModel.IsRunProcessThroughBrowser)
                                _objSearchFanpageDetailsResponseHandler.HasMoreResults = true;

                            jobProcessResult.maxId = _objSearchFanpageDetailsResponseHandler.PageletData;

                            jobProcessResult.HasNoResult = !_objSearchFanpageDetailsResponseHandler.HasMoreResults;

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
