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
using System.Text.RegularExpressions;

namespace FaceDominatorCore.FDLibrary.FdProcessors.FanpageProcessor
{
    public class GraphSearchPlaceProcessor : BaseFbFanpageProcessor
    {
        IResponseHandler SearchPlaceDetailsResponseHandler;

        public GraphSearchPlaceProcessor(IFdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
            IDbCampaignServiceScoped campaignService, IFdRequestLibrary objFdRequestLibrary,
            IFdBrowserManager browserManager, IProcessScopeModel processScopeModel)
            : base(jobProcess, dbAccountService, campaignService, objFdRequestLibrary, browserManager, processScopeModel)
        {
            SearchPlaceDetailsResponseHandler = null;

        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            var filterModel = JobProcess.ModuleSetting.FdPlaceFilterModel;
            if (Regex.IsMatch(queryInfo.QueryValue, "filters=(.*?)") && (filterModel.IsDeliveryChecked || filterModel.IsOpenNowChecked || filterModel.IsPriceRangeChecked || filterModel.IsTakeAwayChecked || filterModel.IsVisitedByFriendsChecked))
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType, "Please Provide the Correct Graph Search URL As Filter is Selected By Software");
                jobProcessResult.IsProcessCompleted = true;
                return;
            }
            string keyword = queryInfo.QueryValue;
            if (AccountModel.IsRunProcessThroughBrowser)
            {
                Browsermanager.SearchByGraphSearchUrl(AccountModel, FbEntityType.Places, keyword);
                Browsermanager.ApplyPlaceFilters(AccountModel, filterModel);
            }

            try
            {
                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    try
                    {
                        var placeELEMENT = FdConstants.PlaceElement2;
                        SearchPlaceDetailsResponseHandler = AccountModel.IsRunProcessThroughBrowser
                            ? Browsermanager.ScrollWindowAndGetData(AccountModel, FbEntityType.Places, 4, 0, placeELEMENT)
                            : ObjFdRequestLibrary.GetPlaceDetailsFromKeyword(AccountModel, keyword, JobProcess.ModuleSetting.FdPlaceFilterModel, SearchPlaceDetailsResponseHandler);

                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                        if (SearchPlaceDetailsResponseHandler.Status)
                        {
                            List<FanpageDetails> listFanpageDetails = SearchPlaceDetailsResponseHandler
                                        .ObjFdScraperResponseParameters.ListPage;

                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                            GlobusLogHelper.log.Info(Log.FoundXResults, AccountModel.AccountBaseModel.AccountNetwork,
                                AccountModel.AccountBaseModel.UserName, listFanpageDetails.Count, queryInfo.QueryType,
                                        queryInfo.QueryValue, _ActivityType);

                            ProcessDataOfPages(queryInfo, ref jobProcessResult, listFanpageDetails);
                            jobProcessResult.maxId = SearchPlaceDetailsResponseHandler.PageletData;

                            if (SearchPlaceDetailsResponseHandler.HasMoreResults == false)
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
                        throw new OperationCanceledException("Requested Cancelled !");
                    }
                    catch (Exception ex)
                    {
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
