using System;
using System.Collections.Generic;
using System.Linq;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDUtility;
using LinkedDominatorCore.Utility;
using Newtonsoft.Json.Linq;

namespace LinkedDominatorCore.Response
{
    public class JobSearchResponseHandler : LdResponseHandler
    {
        public JobSearchResponseHandler(IResponseParameter response) : base(response)
        {
            try
            {
                Success = !string.IsNullOrEmpty(response?.Response) && (response.Response.Contains("com.linkedin.voyager.dash.jobs.search.JobSearchMetadata") || response.Response.Contains("<!DOCTYPE html>"));
                if (!Success)
                    return;

                if (response.Response.Contains("<!DOCTYPE html>"))
                {
                    var jobNodesList = HtmlAgilityHelper.GetListNodesFromClassName(Response.Response,
                        "jobs-search-results__list-item occludable-update p0 relative ember-view");
                    if (jobNodesList.Count == 0)
                        jobNodesList = HtmlAgilityHelper.GetListNodesFromClassName(Response.Response,
                            "occludable-update artdeco-list__item--offset-2 artdeco-list__item p0 ember-view");
                    if (jobNodesList.Count == 0)
                        jobNodesList = HtmlAgilityHelper.GetListNodesFromClassName(Response.Response,
                            "flex-grow-1 artdeco-entity-lockup artdeco-entity-lockup--size-4 ember-view");
                    if (jobNodesList.Count == 0)
                        jobNodesList = HtmlAgilityHelper.GetListNodesFromClassName(Response.Response,
                            "job-card-list__entity-lockup artdeco-entity-lockup artdeco-entity-lockup--size-4 ember-view");
                    if (jobNodesList.Count == 0)
                        jobNodesList = HtmlAgilityHelper.GetListNodesFromClassName(Response.Response, "jobs-search-results__list-item occludable-update p0 relative scaffold-layout__list-item");
                    foreach (var jobNode in jobNodesList)
                    {
                        var jobPostId = Utils.GetBetween(jobNode.OuterHtml, "fs_normalized_jobPosting:", "\"");
                        if (string.IsNullOrEmpty(jobPostId))
                            jobPostId = Utils.GetBetween(jobNode.OuterHtml, "href=\"/jobs/view/", "/?");
                        var linkedinJob = new LinkedinJob(jobPostId);
                        if (string.IsNullOrEmpty(linkedinJob.JobTitle = Utils.RemoveHtmlTags(
                                HtmlAgilityHelper.GetStringInnerHtmlFromClassName(jobNode.OuterHtml,
                                    "job-card-search__title job-card-search__title--full artdeco-entity-lockup__title ember-view"))
                            ?.Replace("Promoted", "")?.Replace("&amp;", "")?.Trim()))
                            linkedinJob.JobTitle = Utils.RemoveHtmlTags(
                                    HtmlAgilityHelper.GetStringInnerHtmlFromClassName(jobNode.OuterHtml,
                                        "disabled ember-view job-card-container__link job-card-list__title"))
                                ?.Replace("Promoted", "")?.Replace("&amp;", "")?.Trim();
                        JobsList.Add(linkedinJob);
                    }
                }
                else
                {
                    var jsonHandler = JsonJArrayHandler.GetInstance;
                    var jsonObject = jsonHandler.ParseJsonToJObject(response.Response);
                    var Jobs = jsonHandler.GetJArrayElement(jsonHandler.GetJTokenValue(jsonObject,"included"));
                    if(Jobs!=null && Jobs.Count > 0)
                    {
                        foreach(var job in Jobs)
                        {
                            var title = jsonHandler.GetJTokenValue(job,"title");
                            var jobPostId = jsonHandler.GetJTokenValue(job, "entityUrn")?.Replace("urn:li:fsd_jobPosting:", "");
                            jobPostId = string.IsNullOrEmpty(jobPostId) ?jsonHandler.GetJTokenValue(job, "trackingUrn")?.Replace("urn:li:jobPosting:", ""): jobPostId;
                            var jobPosterId = jsonHandler.GetJTokenValue(job, "posterId");
                            if(!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(jobPostId))
                            {
                                JobsList.Add(new LinkedinJob(jobPostId) { JobTitle = title,JobPosterID=jobPosterId});
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }

        public List<LinkedinJob> JobsList { get; } = new List<LinkedinJob>();
    }
}