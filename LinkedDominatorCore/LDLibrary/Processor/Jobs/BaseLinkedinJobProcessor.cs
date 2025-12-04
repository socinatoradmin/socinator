using System;
using System.Collections.Generic;
using System.Net;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using LinkedDominatorCore.Factories;
using LinkedDominatorCore.LDLibrary.DAL;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDUtility;

namespace LinkedDominatorCore.LDLibrary.Processor.Jobs
{
    public abstract class BaseLinkedinJobProcessor : BaseLinkedinProcessor
    {
        protected BaseLinkedinJobProcessor(ILdJobProcess ldJobProcess,
            IDbCampaignService campaignService, ILdFunctionFactory ldFunctionFactory, IDelayService delayService,
            IProcessScopeModel processScopeModel)
            : base(ldJobProcess, campaignService, ldFunctionFactory, delayService, processScopeModel)
        {
        }

        public void ProcessLinkedinJobFromJob(QueryInfo queryInfo, ref JobProcessResult jobProcessResult,
            List<LinkedinJob> linkedinJob)
        {
            try
            {
                foreach (var job in linkedinJob)
                {
                    LdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    SendToPerformActivity(ref jobProcessResult, job, queryInfo);
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

        public void SendToPerformActivity(ref JobProcessResult jobProcessResult, LinkedinJob linkedinJob,
            QueryInfo queryInfo)
        {
            try
            {
                jobProcessResult = LdJobProcess.FinalProcess(new ScrapeResultNew
                {
                    ResultJob = linkedinJob,
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

        public string GetConstructedApiJobSearch(string desktopUrl,int PaginationCount=0)
        {
            string api;
            try
            {
                #region Varibale Initialization

                var keyword = string.Empty;

                #endregion

                desktopUrl = Uri.UnescapeDataString(desktopUrl);

                #region Keyword

                if (desktopUrl.Contains("search/?keywords"))
                {
                    keyword = Utils.GetBetween(desktopUrl, "search/?keywords=", "&");
                    if (string.IsNullOrEmpty(keyword) && !desktopUrl.Contains("&"))
                        keyword = Utils.GetBetween(desktopUrl + "**", "search/?keywords=", "**");
                }
                else if (desktopUrl.Contains("&keywords="))
                {
                    keyword = Utils.GetBetween(desktopUrl, "&keywords=", "&");
                    if (string.IsNullOrEmpty(keyword))
                        keyword = Utils.GetBetween(desktopUrl + "**", "&keywords=", "**");
                }
                else if (desktopUrl.Contains("search-results/?keywords="))
                {
                    keyword = Utils.GetBetween(desktopUrl, "search-results/?keywords=", "&");
                }

                if (!string.IsNullOrEmpty(keyword))
                {
                    if (keyword.Contains("”") || keyword.Contains("“"))
                        keyword = keyword.Replace("”", "\"").Replace("“", "\"");
                    keyword = Uri.EscapeDataString(keyword);
                }

                #endregion
                var origin = Utils.GetBetween(desktopUrl, "&origin=", "&");
                origin = string.IsNullOrEmpty(origin) ? Utils.GetBetween(desktopUrl+"#","&origin=","#") : origin;
                var currentJobId = Utils.GetBetween(desktopUrl, "currentJobId=", "&");
                var filter = GetJobSearchFilter(desktopUrl);
                var CurrentJobID = string.IsNullOrEmpty(currentJobId) ? string.Empty : $"currentJobId:{currentJobId},";
                //api = $"https://www.linkedin.com/voyager/api/voyagerJobsDashJobCards?decorationId=com.linkedin.voyager.dash.deco.jobs.search.JobSearchCardsCollectionLite-186&count=25&q=jobSearch&query=(origin:JOB_SEARCH_PAGE_JOB_FILTER,keywords:{keyword},selectedFilters:(sortBy:List(R),applyWithLinkedin:List({applyWithLinkedIn}),company:List({company}),experience:List({experienceLevel}),function:List({jobFunction}),industry:List({Industry}),jobType:List({jobType}),jobCollections:List({jobCollections}),populatedPlace:List({location}),title:List({jobTitle}),timePostedRange:List({datePosted}),workplaceType:List({workPlaceType})),spellCorrectionEnabled:true)&servedEventEnabled=false&start={PaginationCount.ToString()}";
                api = $"https://www.linkedin.com/voyager/api/voyagerJobsDashJobCards?decorationId=com.linkedin.voyager.dash.deco.jobs.search.JobSearchCardsCollection-215&count=25&q=jobSearch&query=({CurrentJobID}origin:{origin},keywords:{keyword}{filter},spellCorrectionEnabled:true)&start={PaginationCount}";
                
            }
            catch (Exception ex)
            {
                api = null;
                ex.DebugLog();
            }

            return api;
        }

        private string GetJobSearchFilter(string desktopUrl)
        {
            try
            {
                var datePosted = string.Empty; //f_TP
                var linkedInFeatures = string.Empty; //f_LF
                var company = string.Empty; //f_C
                var experienceLevel = string.Empty; //f_E
                var location = string.Empty; //f_GC
                var locationId = string.Empty;
                var jobTitle = string.Empty;
                var jobType = string.Empty;
                var Industry = string.Empty;
                var workPlaceType = string.Empty;
                var applyWithLinkedIn = string.Empty;
                var jobFunction = string.Empty;
                var jobCollections = string.Empty;
                #region DatePosted

                if (desktopUrl.Contains("search/?f_TP"))
                    datePosted = Utils.GetBetween(desktopUrl, "search/?f_TP=", "&");
                else if (desktopUrl.Contains("&f_TP=") || desktopUrl.Contains("&f_TPR"))
                    datePosted = Utils.GetBetween(desktopUrl, "&f_TPR=", "&");

                #endregion

                #region LinkedinFeatures

                if (desktopUrl.Contains("search/?f_LF"))
                    linkedInFeatures = Utils.GetBetween(desktopUrl, "search/?f_LF=", "&");
                else if (desktopUrl.Contains("&f_LF=") &&
                         string.IsNullOrEmpty(linkedInFeatures = Utils.GetBetween(desktopUrl, "&f_LF=", "&")))
                    linkedInFeatures = Utils.GetBetween(desktopUrl + "**", "&f_LF=", "**");

                #endregion

                #region Company

                if (desktopUrl.Contains("search/?f_C"))
                {
                    company = Utils.GetBetween(desktopUrl, "search/?f_C=", "&");
                }
                else if (desktopUrl.Contains("&f_C="))
                {
                    company = Utils.GetBetween(desktopUrl, "&f_C=", "&");
                    if (string.IsNullOrEmpty(company))
                        company = Utils.GetBetween(desktopUrl + "**", "&f_C=", "**");
                }

                #endregion

                #region Experience_Level

                if (desktopUrl.Contains("search/?f_E"))
                {
                    experienceLevel = Utils.GetBetween(desktopUrl, "search/?f_E=", "&");
                }
                else if (desktopUrl.Contains("&f_E="))
                {
                    experienceLevel = Utils.GetBetween(desktopUrl, "&f_E=", "&");
                    if (string.IsNullOrEmpty(experienceLevel))
                        experienceLevel = Utils.GetBetween(desktopUrl + "**", "&f_E=", "**");
                }

                #endregion

                #region Location

                if (desktopUrl.Contains("search/?f_GC"))
                {
                    location = Utils.GetBetween(desktopUrl, "search/?f_GC=", "&");
                }
                else if (desktopUrl.Contains("&f_GC=") || desktopUrl.Contains("geoId="))
                {
                    location = Utils.GetBetween(desktopUrl, "&f_GC=", "&");
                    location = string.IsNullOrEmpty(location) ? Utils.GetBetween(desktopUrl, "geoId=", "&") : location;
                    if (string.IsNullOrEmpty(location))
                        location = Utils.GetBetween(desktopUrl + "**", "&f_GC=", "**");
                }

                #endregion

                #region Location and LocationId

                if (desktopUrl.Contains("&location="))
                {
                    location = Utils.GetBetween(desktopUrl, "&location=", "&");
                    if (string.IsNullOrEmpty(location))
                        location = Utils.GetBetween(desktopUrl + "**", "&location=", "**");
                }

                if (desktopUrl.Contains("&locationId="))
                {
                    locationId = Utils.GetBetween(desktopUrl, "&locationId=", "&");
                    if (string.IsNullOrEmpty(locationId))
                        locationId = Utils.GetBetween(desktopUrl + "**", "&locationId=", "**");
                }
                if (desktopUrl.Contains("&f_PP"))
                    locationId = Utils.GetBetween(desktopUrl, "&f_PP=", "&");
                if (desktopUrl.Contains("f_JT="))
                    jobType = Utils.GetBetween(desktopUrl, "f_JT=", "&");
                if (desktopUrl.Contains("f_T"))
                    jobTitle = Utils.GetBetween(desktopUrl, "f_T=", "&");
                if (desktopUrl.Contains("f_WT="))
                    workPlaceType = Utils.GetBetween(desktopUrl, "f_WT=", "&");
                if (desktopUrl.Contains("f_AL="))
                    applyWithLinkedIn = Utils.GetBetween(desktopUrl, "f_AL=", "&");
                if (desktopUrl.Contains("f_F"))
                    jobFunction = Utils.GetBetween(desktopUrl, "f_F=", "&");
                if (desktopUrl.Contains("f_I"))
                    Industry = Utils.GetBetween(desktopUrl, "f_I=", "&");
                if (desktopUrl.Contains("&f_JC"))
                    jobCollections = WebUtility.UrlEncode(Utils.GetBetween(desktopUrl, "&f_JC=", "&"))?.Replace("(", "%28")?.Replace(")", "%29");
                var earlyApplicant = Utils.GetBetween(desktopUrl, "&f_EA=", "&");
                var sortBy = Utils.GetBetween(desktopUrl + "#", "&sortBy=", "#");
                #endregion
                #region Validation of Filter.
                applyWithLinkedIn = string.IsNullOrEmpty(applyWithLinkedIn) ? string.Empty : $",applyWithLinkedin:List({applyWithLinkedIn})";
                location = string.IsNullOrEmpty(location) ? string.Empty : $",locationUnion:(geoId:{location})";
                company = !string.IsNullOrEmpty(company) ? $",company:List({company})" : company;
                experienceLevel = !string.IsNullOrEmpty(experienceLevel) ? $",experience:List({experienceLevel})" :string.Empty;
                earlyApplicant = !string.IsNullOrEmpty(earlyApplicant) ?$",earlyApplicant:List({earlyApplicant})" :string.Empty;
                jobFunction = !string.IsNullOrEmpty(jobFunction) ? $",function:List({jobFunction})" :string.Empty;
                Industry = !string.IsNullOrEmpty(Industry) ? $",industry:List({Industry})" :Industry;
                locationId = !string.IsNullOrEmpty(locationId) ?$",populatedPlace:List({locationId})" :string.Empty;
                jobTitle = !string.IsNullOrEmpty(jobTitle) ? $",title:List({jobTitle})" : string.Empty;
                datePosted = !string.IsNullOrEmpty(datePosted)?$",timePostedRange:List({datePosted})" :string.Empty;
                workPlaceType = !string.IsNullOrEmpty(workPlaceType) ? $",workplaceType:List({workPlaceType})" : string.Empty;
                sortBy = !string.IsNullOrEmpty(sortBy)?$"sortBy:List({sortBy})" :string.Empty;
                var selectedFilter = $"{sortBy}{applyWithLinkedIn}{company}{experienceLevel}{earlyApplicant}{jobFunction}{jobTitle}{Industry}{locationId}{datePosted}{workPlaceType}"?.TrimStart(',');
                selectedFilter = !string.IsNullOrEmpty(selectedFilter) ? $",selectedFilters:({selectedFilter})" :string.Empty;
                #endregion
                
                return $"{location}{selectedFilter}";
            }
            catch { return string.Empty; }
        }
    }
}