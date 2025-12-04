using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.LdTables.Campaign;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.Factories;
using LinkedDominatorCore.LDLibrary.DAL;
using LinkedDominatorCore.LDUtility;
using LinkedDominatorCore.Response;

namespace LinkedDominatorCore.LDLibrary.Processor.Companies
{
    public class CompanySearchUrlProcessor : BaseLinkedinCompanyProcessor, IQueryProcessor
    {
        public CompanySearchUrlProcessor(ILdJobProcess jobProcess,
            IDbCampaignService campaignService, ILdFunctionFactory ldFunctionFactory, IDelayService delayService,
            IProcessScopeModel processScopeModel) :
            base(jobProcess, campaignService, ldFunctionFactory, delayService, processScopeModel)
        {
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            var start = IsBrowser ? 1 : 0;
            var maxPage = 25;
            try
            {
                var constructedActionUrl = string.Empty;
                var isNotFirst = false;
                List<InteractedCompanies>
                    listInteractedCompaniesFromCampaignDb;
                var queryValue = queryInfo.QueryValue;
                if (ActivityType == ActivityType.SalesNavigatorCompanyScraper)
                {
                    if (queryValue.Contains("sales/search/companies") ||
                        queryValue.Contains("sales/search/company"))
                    {
                        constructedActionUrl = IsBrowser
                            ? queryValue
                            : GetSalesNavCompanyApi(LdFunctions, queryValue);
                        if (string.IsNullOrEmpty(constructedActionUrl))
                            constructedActionUrl = GetConstructedApiSalesNavigatorSearch(queryValue);
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            "please provide correct search url to get results for companies");
                        jobProcessResult.HasNoResult = true;
                        return;
                    }
                }
                else if (ActivityType == ActivityType.CompanyScraper)
                {
                    if (queryValue.Contains("https://www.linkedin.com/search/results/companies/"))
                    {
                        constructedActionUrl = IsBrowser
                            ? queryValue
                            : GetConstructedApiCompanySearch(LdFunctions, queryValue);
                    }
                    else
                    {
                        jobProcessResult.HasNoResult = true;
                        return;
                    }
                }


                if (LdJobProcess.ModuleSetting.IsSavePagination)
                    int.TryParse(GetPaginationId(queryInfo), out start);

                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                {
                    // We are not saving pagination first time we hit the url we will 
                    // save next time when we paginate it because pagination id gives us next page users
                    // here we save last page id
                    if (LdJobProcess.ModuleSetting.IsSavePagination)
                        AddOrUpdatePaginationId(queryInfo, start.ToString(), ref isNotFirst);
                    var actionUrl = IsBrowser
                        ? BrowserActionUrl(constructedActionUrl, start)
                        : NormalActionUrl(constructedActionUrl, start);


                    #region companySearchResponseHandler

                    CompanySearchResponseHandler companySearchResponseHandler;

                    if (ActivityType == ActivityType.SalesNavigatorCompanyScraper)
                    {
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                        if ((companySearchResponseHandler = LdFunctions.CompanySearch(actionUrl, true)) == null)
                            LdFunctions.CompanySearch(actionUrl, true, true); //demoUrl ActionUrl
                    }
                    else
                    {
                        companySearchResponseHandler = LdFunctions.CompanySearch(actionUrl, false);
                    }

                    #region Display Total Number Of Results in the Search

                    if (IsBrowser ? start == 1 : start == 0 && !string.IsNullOrEmpty(companySearchResponseHandler.TotalResults))
                    {
                        GetMaxPage(out maxPage);
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            "total results in search = " + companySearchResponseHandler.TotalResults + "");
                    }

                    #endregion
                    LdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    if (companySearchResponseHandler.Success)
                    {
                        if (companySearchResponseHandler.CompanyList.Count > 0)
                        {
                            if (string.IsNullOrEmpty(LdJobProcess.CurrentCampaignId))
                            {
                                var listInteractedCompaniesFromAccountDb =
                                    DbAccountService.GetInteractedCompanies(ActivityTypeString).ToList();
                                companySearchResponseHandler.CompanyList.RemoveAll(x =>
                                    listInteractedCompaniesFromAccountDb.Any(y => y.CompanyUrl == x.CompanyUrl));
                            }
                            else
                            {
                                listInteractedCompaniesFromCampaignDb = DbCampaignService
                                    .GetInteractedCompanies(ActivityTypeString).ToList();
                                if (listInteractedCompaniesFromCampaignDb != null &&
                                    listInteractedCompaniesFromCampaignDb.Count > 0)
                                    companySearchResponseHandler.CompanyList.RemoveAll(x =>
                                        listInteractedCompaniesFromCampaignDb.Any(y => y.CompanyUrl == x.CompanyUrl));
                            }

                            if (companySearchResponseHandler.CompanyList.Count > 0)
                                ProcessLinkedinCompanyFromCompany(queryInfo, ref jobProcessResult,
                                    companySearchResponseHandler.CompanyList);
                        }

                        if (start >= 1000 || IsBrowser && start >= maxPage)
                        {
                            if(maxPage==1)
                                GlobusLogHelper.log.Info("No more pagination data found for this url from Account-->" +
                                                     DominatorAccountModel.AccountBaseModel.UserName);
                            else
                            GlobusLogHelper.log.Info("we have reached 100 pages from accounts --> " +
                                                     DominatorAccountModel.AccountBaseModel.UserName);
                            jobProcessResult.HasNoResult = true;
                            break;
                        }
                        start += IsBrowser ? 1 : 40;
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(Log.NoMoreDataToPerform,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType);
                        jobProcessResult.HasNoResult = true;
                    }

                    #endregion
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                jobProcessResult.HasNoResult = true;
            }
        }

        private void GetMaxPage(out int maxPage)
        {
            maxPage = 25;
            if (IsBrowser)
            {
                var pages = HtmlAgilityHelper.GetListInnerHtmlFromClassName(
                    LdFunctions.BrowserWindow.GetPageSource(),
                    "artdeco-pagination__indicator artdeco-pagination__indicator--number ember-view");

                string lastpagenumber = string.Empty;
                if (pages.Count <= 0)
                {
                    pages = HtmlAgilityHelper.GetListInnerHtmlFromClassName(
                    LdFunctions.BrowserWindow.GetPageSource(),
                    "search-results__pagination-list");
                    var pagenumber = Utilities.GetBetween(pages.Count > 0 ?pages.FirstOrDefault():string.Empty, "search-results__pagination-ellipsis", " </li>");
                    lastpagenumber = Utilities.GetBetween(pagenumber, "Navigate to page ", "\"");
                }
                if (!string.IsNullOrEmpty(lastpagenumber))
                    maxPage = int.Parse(lastpagenumber);


                if (IsNullOrZeroLength(pages))
                    maxPage = 1;
            }
        }

        private string BrowserActionUrl(string constructedActionUrl, int start)=>
            $"{constructedActionUrl}&page={start}";
        private bool IsNullOrZeroLength(List<string> dataString)=>
            dataString == null || dataString.Count == 0;

        private string NormalActionUrl(string constructedActionUrl, int start)=>
            constructedActionUrl.Contains("count=")
                ? $"{constructedActionUrl.Replace("&start=0", $"&start={start}")}"
                : $"{constructedActionUrl}&count=40&start={start}";
    }
}