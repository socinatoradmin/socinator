using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using LinkedDominatorCore.Factories;
using LinkedDominatorCore.LDLibrary.DAL;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDUtility;

namespace LinkedDominatorCore.LDLibrary.Processor.Companies
{
    public abstract class BaseLinkedinCompanyProcessor : BaseLinkedinProcessor
    {
        protected BaseLinkedinCompanyProcessor(ILdJobProcess ldJobProcess,
            IDbCampaignService campaignService, ILdFunctionFactory ldFunctionFactory, IDelayService delayService,
            IProcessScopeModel processScopeModel)
            : base(ldJobProcess, campaignService, ldFunctionFactory, delayService, processScopeModel)
        {
        }


        public void ProcessLinkedinCompanyFromCompany(QueryInfo queryInfo, ref JobProcessResult jobProcessResult,
            List<LinkedinCompany> linkedinCompany)
        {
            try
            {
                foreach (var company in linkedinCompany)
                {
                    LdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    SendToPerformActivity(ref jobProcessResult, company, queryInfo);
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void SendToPerformActivity(ref JobProcessResult jobProcessResult, LinkedinCompany linkedinCompany,
            QueryInfo queryInfo)
        {
            try
            {
                jobProcessResult = LdJobProcess.FinalProcess(new ScrapeResultNew
                {
                    ResultCompany = linkedinCompany,
                    QueryInfo = queryInfo
                });
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public string GetConstructedApiCompanySearch(ILdFunctions ldFunctions,string desktopUrl)
        {
            string api;
            
            try
            {
                api = GetApiFromPageResponse(ldFunctions, desktopUrl);
                if (string.IsNullOrWhiteSpace(api))
                {
                    var keyword = string.Empty;

                    desktopUrl = Uri.UnescapeDataString(desktopUrl);

                    if (desktopUrl.Contains("companies/?keywords"))
                        keyword = Utils.GetBetween(desktopUrl, "companies/?keywords=", "&");
                    else if (desktopUrl.Contains("index/?keywords="))
                        keyword = Utils.GetBetween(desktopUrl, "index/?keywords=", "&");
                    else if (desktopUrl.Contains("companies/v2/?keywords"))
                        keyword = Utils.GetBetween(desktopUrl, "companies/v2/?keywords=", "&");
                    else if (desktopUrl.Contains("&keywords="))
                        keyword = Utils.GetBetween(desktopUrl, "&keywords=", "&");
                    if (!string.IsNullOrEmpty(keyword))
                        keyword = Uri.EscapeDataString(keyword);
                    api = !string.IsNullOrEmpty(keyword)
                        ? $"{LdConstants.CompanySearchTypeApiConstant}&keywords={keyword}"
                        : LdConstants.CompanySearchTypeApiConstant;
                }
            }
            catch (Exception ex)
            {
                api = null;
                ex.DebugLog();
            }

            return api;
        }

        private static string GetApiFromPageResponse(ILdFunctions ldFunctions, string desktopUrl)
        {
            string api;
            var requestResponse = ldFunctions.GetInnerLdHttpHelper().GetRequest(desktopUrl);
            var pageResponse = requestResponse.Response;
            api = Utils.GetBetween(pageResponse, "/voyager/api/search/dash/clusters?", "\"");
            if (string.IsNullOrEmpty(api))
            {
                var decodedUrl = WebUtility.UrlDecode(desktopUrl);
                #region Request Parameters.
                var location = string.Empty;
                var queryParameter = string.Empty;
                var companySize = string.Empty;
                var CompanyCount = string.Empty;
                var keyword = string.Empty;
                var position = string.Empty;
                var searchId = string.Empty;
                if (decodedUrl.Contains("companyHqGeo="))
                    location = Utils.GetBetween(decodedUrl, "companyHqGeo=[\"", "\"]")?.Replace("\"", "");
                if (decodedUrl.Contains("companySize="))
                    companySize = Utils.GetBetween(decodedUrl, "companySize=[\"", "\"]")?.Replace("\"","");
                if (decodedUrl.Contains("industryCompanyVertical="))
                    CompanyCount = Utils.GetBetween(decodedUrl, "industryCompanyVertical=[\"", "\"]")?.Replace("\"","");
                if (decodedUrl.Contains("keywords="))
                    keyword = Utils.GetBetween(decodedUrl, "keywords=", "&origin=");
                if (decodedUrl.Contains("position="))
                    position = Utils.GetBetween(decodedUrl, "position=", "&searchId=")?.Replace("\"","");
                if (decodedUrl.Contains("searchId="))
                    searchId = Utils.GetBetween(decodedUrl, "searchId=", "&sid=");
                #endregion
                #region Generate Request Parameters.
                keyword = Uri.EscapeDataString(keyword);
                location = string.IsNullOrEmpty(location) ? "" : $"companyHqGeo:List({location}),";
                companySize = string.IsNullOrEmpty(companySize) ? "" : $"companySize:List({companySize}),";
                CompanyCount = string.IsNullOrEmpty(CompanyCount) ? "" : $"industryCompanyVertical:List({CompanyCount}),";
                position=string.IsNullOrEmpty(position)?"":$"position:List({position}),";
                searchId = string.IsNullOrEmpty(searchId) ? "" : $",searchId:List({searchId})";
                queryParameter = $"{location}{companySize}{CompanyCount}{position}resultType:List(COMPANIES){searchId}";
                #endregion
                api = $"https://www.linkedin.com/voyager/api/search/dash/clusters?decorationId=com.linkedin.voyager.dash.deco.search.SearchClusterCollection-177&origin=FACETED_SEARCH&q=all&query=(keywords:{keyword},flagshipSearchIntent:SEARCH_SRP,queryParameters:({queryParameter}),includeFiltersInResponse:false)";
                return api;
            }
            var decodeUrl = Regex.Unescape(Regex.Replace(api, "\\\\([^u])", "\\\\$1")).Replace("&count=10", "")
                .Replace("count=10", "")
                .Replace("&start=0", "").Replace("&start=10", "");
            api = $"https://www.linkedin.com/voyager/api/search/dash/clusters?{decodeUrl}";
            return api;
        }
    }
}