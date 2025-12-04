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

namespace FaceDominatorCore.FDLibrary.FdProcessors.FanpageProcessor
{
    public class GraphSearchFnapageProcessor : BaseFbFanpageProcessor
    {
        IResponseHandler SearchFanpageDetailsResponseHandler;

        public GraphSearchFnapageProcessor(IFdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
            IDbCampaignServiceScoped campaignService, IFdRequestLibrary objFdRequestLibrary,
            IFdBrowserManager browserManager, IProcessScopeModel processScopeModel)
            : base(jobProcess, dbAccountService, campaignService, objFdRequestLibrary, browserManager, processScopeModel)
        {
            SearchFanpageDetailsResponseHandler = null;

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
                Browsermanager.SearchByGraphSearchUrl(AccountModel, FbEntityType.Fanpage, keyword);

                Browsermanager.ApplyPageFilters(AccountModel, objFanpageCategory, isVerifiedFilter);
            }

            try
            {
                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    try
                    {
                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                        SearchFanpageDetailsResponseHandler = AccountModel.IsRunProcessThroughBrowser
                            ? Browsermanager.ScrollWindowAndGetData(AccountModel, FbEntityType.Fanpage, 5, 0)
                            : ObjFdRequestLibrary.GetFanpageDetailsFromGraphSearch
                                 (AccountModel, keyword, isVerifiedFilter, isLikedByFriends, objFanpageCategory, SearchFanpageDetailsResponseHandler);

                        if (SearchFanpageDetailsResponseHandler.Status)
                        {
                            List<FanpageDetails> listFanpageDetails = SearchFanpageDetailsResponseHandler
                                        .ObjFdScraperResponseParameters.ListPage;

                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                            GlobusLogHelper.log.Info(Log.FoundXResults, AccountModel.AccountBaseModel.AccountNetwork,
                                AccountModel.AccountBaseModel.UserName, listFanpageDetails.Count, queryInfo.QueryType,
                                        queryInfo.QueryValue, _ActivityType);

                            ProcessDataOfPages(queryInfo, ref jobProcessResult, listFanpageDetails);
                            jobProcessResult.maxId = SearchFanpageDetailsResponseHandler.PageletData;

                            if (SearchFanpageDetailsResponseHandler.HasMoreResults == false)
                                jobProcessResult.HasNoResult = true;

                        }
                        else
                        {
                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            jobProcessResult.HasNoResult = true;
                            jobProcessResult.maxId = null;

                            if (AccountModel.IsRunProcessThroughBrowser && queryInfo.QueryType == "Graph Search Url")
                                Browsermanager.CloseBrowser(AccountModel);

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
                throw new OperationCanceledException("Requested Cancelled !");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

    }
}
