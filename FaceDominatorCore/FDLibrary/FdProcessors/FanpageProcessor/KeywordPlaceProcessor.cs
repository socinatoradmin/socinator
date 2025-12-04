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
    public class KeywordPlaceProcessor : BaseFbFanpageProcessor
    {
        IResponseHandler SearchPlaceDetailsResponseHandler;

        public KeywordPlaceProcessor(IFdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
            IDbCampaignServiceScoped campaignService, IFdRequestLibrary objFdRequestLibrary,
            IFdBrowserManager browserManager, IProcessScopeModel processScopeModel)
            : base(jobProcess, dbAccountService, campaignService, objFdRequestLibrary, browserManager, processScopeModel)
        {
            SearchPlaceDetailsResponseHandler = null;
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {

            string keyword = queryInfo.QueryValue;
            if (AccountModel.IsRunProcessThroughBrowser)
            {
                Browsermanager.SearchByKeywordOrHashTag(AccountModel, SearchKeywordType.Places, keyword);

                Browsermanager.ApplyPlaceFilters(AccountModel, JobProcess.ModuleSetting.FdPlaceFilterModel);
            }

            try
            {
                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    try
                    {
                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        var placeELEMENT = FdConstants.PlaceElement2;
                        SearchPlaceDetailsResponseHandler = AccountModel.IsRunProcessThroughBrowser
                            ? Browsermanager.ScrollWindowAndGetData(AccountModel, FbEntityType.Places, 4, 0, placeELEMENT)
                            : ObjFdRequestLibrary.GetPlaceDetailsFromKeyword(AccountModel, keyword, JobProcess.ModuleSetting.FdPlaceFilterModel, SearchPlaceDetailsResponseHandler);

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
