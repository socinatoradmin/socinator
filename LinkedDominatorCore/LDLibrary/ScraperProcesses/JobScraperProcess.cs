using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.LdTables.Account;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.DetailedInfo;
using LinkedDominatorCore.Factories;
using LinkedDominatorCore.LDLibrary.DAL;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDUtility;
using LinkedDominatorCore.Request;
using LinkedDominatorCore.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// ReSharper disable once CheckNamespace
namespace LinkedDominatorCore.LDLibrary
{
    public class
        JobScraperProcess : LDJobProcessInteracted<InteractedJobs>
    {
        private readonly ILdFunctions _ldFunctions;


        public JobScraperProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IDbGlobalService globalService,
            IExecutionLimitsManager executionLimitsManager, ILdQueryScraperFactory queryScraperFactory,
            ILdHttpHelper ldHttpHelper, ILdLogInProcess logInProcess,
            ILdFunctionFactory ldFunctionFactory, IDbInsertionHelper dbInsertionHelper)
            : base(processScopeModel, accountServiceScoped, globalService, executionLimitsManager, queryScraperFactory,
                ldHttpHelper, logInProcess, dbInsertionHelper)
        {
            JobScraperModel = processScopeModel.GetActivitySettingsAs<JobScraperModel>();
            _ldFunctions = ldFunctionFactory.LdFunctions;
        }

        public JobScraperModel JobScraperModel { get; set; }


        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            JobProcessResult jobProcessResult = null;
            var success = false;
            var detailedJobInfoJasonString = string.Empty;
            try
            {
                jobProcessResult = new JobProcessResult();

                var objLinkedinJob = (LinkedinJob)scrapeResult.ResultJob;

                try
                {
                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    try
                    {
                        JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        GlobusLogHelper.log.Info(Log.StartedActivity,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            objLinkedinJob.JobPostUrl);
                        var resultScrapeJobDetails = ScrapeJobDetails(objLinkedinJob.JobPostUrl);
                        detailedJobInfoJasonString = resultScrapeJobDetails.Item2;
                        success = resultScrapeJobDetails.Item1;
                    }
                    catch (OperationCanceledException)
                    {
                        throw new OperationCanceledException("Operation Cancelled!");
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    if (success)
                    {
                        GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            objLinkedinJob.JobPostUrl);
                        IncrementCounters();
                        if (string.IsNullOrEmpty(objLinkedinJob.JobTitle))
                            objLinkedinJob.JobTitle =
                                Utils.GetBetween(detailedJobInfoJasonString, "\"JobTitle\":\"", "\"");
                        DbInsertionHelper.JobScraper(scrapeResult, objLinkedinJob, detailedJobInfoJasonString);

                        jobProcessResult.IsProcessSuceessfull = true;
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(Log.ActivityFailed,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            objLinkedinJob.JobPostUrl, "");
                        jobProcessResult.IsProcessSuceessfull = false;
                    }

                    DelayBeforeNextActivity();
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
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return jobProcessResult;
        }


        private Tuple<bool, string> ScrapeJobDetails(string jobPostUrl)
        {
            var detailedJobInfoJasonString = string.Empty;
            var jsonJArrayHandler = JsonJArrayHandler.GetInstance;
            var objJObject = new JObject();
            Tuple<bool, string> resultScrapeJobDetails;

            try
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                #region Initializations Type One

                var jobApiResponse = string.Empty;

                #endregion

                #region Initializations Required For Detail Info

                var accountEmail = string.Empty;
                var accountUserFullName = string.Empty;
                var accountUserProfileUrl = string.Empty;
                var companyWebsite = string.Empty;
                var companyName = string.Empty;
                var jobTitle = string.Empty;
                var jobLocation = string.Empty;
                var postedOn = string.Empty;
                var numberOfApplicants = string.Empty;
                var industry = string.Empty;
                var employmentType = string.Empty;
                var seniorityLevel = string.Empty;
                var jobFunction = string.Empty;
                var jobPosterProfileUrl = string.Empty;
                var jobPosterFirstName = string.Empty;
                var jobPosterLastName = string.Empty;
                var degreeOfConnection = string.Empty;
                var elements = string.Empty;

                #endregion

                #region Getting Response From API

                try
                {
                    var jobId = Utils.GetBetween(jobPostUrl + "**", "view/", "**");
                    if (jobId.Contains("/"))
                        jobId = Regex.Match(jobId, @"\d+").Value;
                    var actionUrl = IsBrowser
                        ? jobPostUrl
                        : "https://www.linkedin.com/voyager/api/jobs/jobPostings/" + jobId +
                          "?_capColoOverride=false&decoration=%28title%2CformattedIndustries%2CformattedJobFunctions%2CformattedEmploymentStatus%2CformattedExperienceLevel%2CformattedLocation%2CtrackingPixelUrl%2CsourceDomain%2ClistingType%2CjobState%2ClistedAt%2CexpireAt%2CclosedAt%2CcompanyDetails%28com.linkedin.voyager.jobs.JobPostingCompany%28company~%28backgroundCoverImage%2CcompanyType%2CcompanyPageUrl%2CcoverPhoto%2CentityUrn%2Cdescription%2CfollowingInfo%2Cindustries%2Clogo%2Cname%2CpaidCompany%2Curl%2C~targetedContent%28additionalMediaSections*%29%29%29%2Ccom.linkedin.voyager.jobs.JobPostingCompanyName%29%2CcompanyDescription%2Cdescription%2CskillsDescription%2CsavingInfo%2CapplyingInfo%2Cnew%2CapplyMethod%28com.linkedin.voyager.jobs.OffsiteApply%2Ccom.linkedin.voyager.jobs.SimpleOnsiteApply%2Ccom.linkedin.voyager.jobs.ComplexOnsiteApply%29%2CjobPostingUrl%2Capplies%2Cviews%2Cposter~%28entityUrn%2CprofilePicture%2Cheadline%2CfirstName%2ClastName%2Cdistance%2Clocation%29%2CsalaryInsights%2CstandardizedTitle~%28entityUrn%2ClocalizedName%29%2CjobRegion~%28entityUrn%2CregionName%29%2Ccountry%2CeligibleForReferrals%2CencryptedPricingParams%2CentityUrn~%28~relevanceReason%28entityUrn%2CjobPosting%2Cdetails%28com.linkedin.voyager.jobs.shared.InNetworkRelevanceReasonDetails%28totalNumberOfConnections%2CtopConnections*~%28profilePicture%2CfirstName%2ClastName%2CentityUrn%29%29%2Ccom.linkedin.voyager.jobs.shared.CompanyRecruitRelevanceReasonDetails%28totalNumberOfPastCoworkers%2CcurrentCompany~%28entityUrn%2Cname%2Clogo%2CbackgroundCoverImage%29%29%2Ccom.linkedin.voyager.jobs.shared.SchoolRecruitRelevanceReasonDetails%28totalNumberOfAlumni%2CmostRecentSchool~%28entityUrn%2Cname%2Clogo%29%29%2Ccom.linkedin.voyager.jobs.shared.HiddenGemRelevanceReasonDetails%29%29%2C~jobSeekerQuality%28entityUrn%2CqualityType%29%2CentityUrn%29%29";
                    _ldFunctions.SetRequestParametersAndProxy_MobileLogin();
                    jobApiResponse = IsBrowser
                        ? _ldFunctions.GetInnerHttpHelper().GetRequest(LdConstants.GetJobDetailsAPI(JobId:jobId)).Response
                        : _ldFunctions.GetRequestUpdatedUserAgent(LdConstants.GetJobDetailsAPI(JobId: jobId));
                    if (string.IsNullOrEmpty(jobApiResponse))
                    {
                        actionUrl = $"https://www.linkedin.com/voyager/api/jobs/jobPostings/{jobId}?decorationId=com.linkedin.voyager.deco.jobs.web.shared.WebFullJobPosting-60&_capColoOverride=false&topN=1&topNRequestedFlavors=List(TOP_APPLICANT,IN_NETWORK,COMPANY_RECRUIT,SCHOOL_RECRUIT,HIDDEN_GEM,ACTIVELY_HIRING_COMPANY)";
                        jobApiResponse = IsBrowser
                                                  ? _ldFunctions.GetInnerHttpHelper().GetRequest(LdConstants.GetJobDetailsAPI(JobId: jobId)).Response
                                                  : _ldFunctions.GetRequestUpdatedUserAgent(LdConstants.GetJobDetailsAPI(JobId: jobId));
                    }

                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                #endregion

                #region Account Info

                try
                {
                    accountEmail = DominatorAccountModel.AccountBaseModel.UserName;
                    accountUserFullName = DominatorAccountModel.AccountBaseModel.UserFullName;
                    accountUserProfileUrl = DominatorAccountModel.AccountBaseModel.ProfilePictureUrl;
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                #endregion

                #region Job Poster Details
                JToken DetailsNode = null;
                try
                {
                    objJObject = JObject.Parse(jobApiResponse);
                    #region JobPosterFullName

                    try
                    {
                        #region Job Poster Full Name.
                        var FailedCount = 0;
                        var JobPostID = jsonJArrayHandler.GetJTokenValue(objJObject, "data", "dashEntityUrn");
                        JobPostID = string.IsNullOrEmpty(JobPostID) ?jsonJArrayHandler.GetJTokenValue(objJObject, "dashEntityUrn") : JobPostID;
                        if (!string.IsNullOrEmpty(JobPostID))
                        {
                            TryAgain:
                            var JobPosterDetails = _ldFunctions.GetInnerLdHttpHelper().GetRequest(LdConstants.GetJobPosterDetailsAPI(JobPostID)).Response;
                            while(FailedCount++<2&&string.IsNullOrEmpty(JobPosterDetails))
                                goto TryAgain;
                            var jObject = jsonJArrayHandler.ParseJsonToJObject(JobPosterDetails);
                            DetailsNode = jsonJArrayHandler.GetJTokenOfJToken(jObject, "data", "elements",0, "jobPostingDetailSectionUnions",0, "hiringTeamCard");
                            DetailsNode = !DetailsNode.HasValues ? jsonJArrayHandler.GetJTokenOfJToken(jObject,"elements", 0, "jobPostingDetailSectionUnions", 0, "hiringTeamCard") : DetailsNode;
                            DetailsNode = !DetailsNode.HasValues ? jsonJArrayHandler.GetJTokenOfJToken(jObject,"data"):DetailsNode;
                            var FullName = jsonJArrayHandler.GetJTokenValue(DetailsNode, "title","text")?.Split(' ');
                            jobPosterLastName=Utils.RemoveSpecialCharacters(FullName.Last());
                            jobPosterFirstName=Utils.RemoveSpecialCharacters(FullName.First()?.Length > 0? string.Join(" ",FullName)?.Replace(jobPosterLastName, ""):string.Empty);
                        }
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    #endregion

                    #region DegreeOfConnection

                    try
                    {
                        degreeOfConnection =Regex.Replace(jsonJArrayHandler.GetJTokenValue(DetailsNode, "titleInsight","text"),"[^0-9]","");
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    #endregion

                    #region JobPosterProfileUrl

                    try
                    {
                        jobPosterProfileUrl = jsonJArrayHandler.GetJTokenValue(DetailsNode, "navigationUrl");
                        jobPosterProfileUrl = string.IsNullOrEmpty(jobPosterProfileUrl) ? jsonJArrayHandler.GetJTokenValue(DetailsNode, "jobPostingUrl") : jobPosterProfileUrl;
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    #endregion
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                #endregion

                #region Job and Company Details

                try
                {
                    #region CompanyWebsite

                    try
                    {
                        var failedCount = 0;
                        var CompanyUniversalName = Utils.GetBetween(jsonJArrayHandler.GetJTokenValue(objJObject, "included"), "\r\n    \"universalName\": \"", "\"");
                        CompanyUniversalName = string.IsNullOrEmpty(CompanyUniversalName) ?jsonJArrayHandler.GetJTokenValue(objJObject, "companyDetails", "com.linkedin.voyager.deco.jobs.web.shared.WebJobPostingCompany", "companyResolutionResult", "universalName") : CompanyUniversalName;
                        var CompanyDetails = _ldFunctions.GetInnerLdHttpHelper().GetRequest(LdConstants.GetCompanyDetailsAPI(CompanyUniversalName)).Response;
                        while(failedCount++<2&&string.IsNullOrEmpty(CompanyDetails))
                            CompanyDetails = _ldFunctions.GetInnerLdHttpHelper().GetRequest(LdConstants.GetCompanyDetailsAPI(CompanyUniversalName)).Response;
                        try{DetailsNode = jsonJArrayHandler.GetTokenElement(jsonJArrayHandler.GetJArrayElement(jsonJArrayHandler.GetJTokenValue(jsonJArrayHandler.ParseJsonToJObject(CompanyDetails), "included")).FirstOrDefault(x => x.ToString().Contains("specialities")));}
                        catch (Exception){DetailsNode = jsonJArrayHandler.GetTokenElement(jsonJArrayHandler.ParseJsonToJObject(CompanyDetails), "data", "organizationDashCompaniesByUniversalName", "elements",0);}
                        companyWebsite = jsonJArrayHandler.GetJTokenValue(DetailsNode, "websiteUrl");
                        jobPosterProfileUrl = string.IsNullOrEmpty(jobPosterProfileUrl) ?jsonJArrayHandler.GetJTokenValue(DetailsNode,"url"): jobPosterProfileUrl;
                        jobPosterFirstName = string.IsNullOrEmpty(jobPosterFirstName) ? jsonJArrayHandler.GetJTokenValue(DetailsNode, "name") : jobPosterFirstName;
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    #endregion

                    #region CompanyName

                    try
                    {
                        if (IsBrowser)
                        {
                            companyName = jsonJArrayHandler.GetTokenElement(objJObject, "companyDetails",
                                    "com.linkedin.voyager.deco.jobs.web.shared.WebJobPostingCompany", "companyResolutionResult", "name")
                                .ToString();
                        }
                        else
                        {
                            companyName = Utils.GetBetween(elements, "\"name\": \"", "\"");
                            if (string.IsNullOrWhiteSpace(companyName))
                                companyName = jsonJArrayHandler.GetJTokenValue(DetailsNode, "name");
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    #endregion

                    #region JobTitle

                    try
                    {
                        if (IsBrowser)
                            jobTitle = jsonJArrayHandler.GetTokenElement(objJObject, "title").ToString();
                        else
                            jobTitle = jsonJArrayHandler.GetTokenElement(objJObject, "data", "title").ToString();
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    #endregion

                    #region JobLocation

                    try
                    {
                        if (IsBrowser)
                            jobLocation = Utils.GetBetween(jobApiResponse, "formattedLocation\":\"", "\",\"");
                        else
                            jobLocation = jsonJArrayHandler.GetTokenElement(objJObject, "data", "formattedLocation")
                                .ToString();
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    #endregion

                    #region PostedOn

                    try
                    {
                        if (IsBrowser)
                            postedOn = Utils.GetBetween(jobApiResponse, "listedAt\":", ",");
                        else
                            postedOn = jsonJArrayHandler.GetTokenElement(objJObject, "data", "listedAt").ToString();

                        postedOn = long.Parse(postedOn).EpochToDateTimeUtc().ToLocalTime()
                            .ToString(CultureInfo.InvariantCulture);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    #endregion

                    #region NumberOfApplicants

                    if (jobApiResponse.Contains("applies\":"))
                        numberOfApplicants = Utils.GetBetween(jobApiResponse, "applies\":", ",");

                    #endregion

                    #region SeniorityLevel   

                    try
                    {
                        seniorityLevel = Utils.GetBetween(jobApiResponse, "formattedExperienceLevel\":\"", "\",\"");
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    #endregion

                    #region Industry

                    try
                    {
                        var allIndustries = Utils.GetBetween(jobApiResponse, "formattedIndustries\":[\"", "\"]");

                        industry = allIndustries.Replace("\",\"", ". ");
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    #endregion

                    #region EmploymentType 

                    try
                    {
                        employmentType = Utils.GetBetween(jobApiResponse, "formattedEmploymentStatus\":\"", "\",\"");
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    #endregion

                    #region JobFunction

                    try
                    {
                        jobFunction = Utils.GetBetween(jobApiResponse, "formattedJobFunctions\":[\"", "\"]");
                        jobFunction = jobFunction.Replace("\",\"", ". ");
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    #endregion
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                #endregion

                #region Replacing IF empty with N/A

                try
                {
                    if (string.IsNullOrWhiteSpace(companyWebsite)) companyWebsite = "N/A";
                    if (string.IsNullOrWhiteSpace(companyName)) companyName = "N/A";
                    if (string.IsNullOrWhiteSpace(jobTitle)) jobTitle = "N/A";
                    if (string.IsNullOrWhiteSpace(jobLocation)) jobLocation = "N/A";
                    if (string.IsNullOrWhiteSpace(postedOn)) postedOn = "N/A";
                    if (string.IsNullOrWhiteSpace(numberOfApplicants)) numberOfApplicants = "N/A";
                    if (string.IsNullOrWhiteSpace(industry)) industry = "N/A";
                    if (string.IsNullOrWhiteSpace(seniorityLevel)) seniorityLevel = "N/A";
                    if (string.IsNullOrWhiteSpace(jobFunction)) jobFunction = "N/A";
                    if (string.IsNullOrWhiteSpace(jobPosterProfileUrl)) jobPosterProfileUrl = "N/A";
                    if (string.IsNullOrWhiteSpace(jobPosterFirstName)) jobPosterFirstName = "N/A";
                    if (string.IsNullOrWhiteSpace(jobPosterLastName)) jobPosterLastName = "N/A";
                    if (string.IsNullOrWhiteSpace(degreeOfConnection)) degreeOfConnection = "N/A";
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                #endregion

                #region Replacing Specail Characters

                accountUserFullName = Utils.RemoveSpecialCharacters(accountUserFullName);
                companyWebsite = Utils.RemoveSpecialCharacters(companyWebsite);
                companyName = Utils.RemoveSpecialCharacters(companyName);
                jobTitle = Utils.RemoveSpecialCharacters(jobTitle);
                jobLocation = Utils.RemoveSpecialCharacters(jobLocation);
                postedOn = Utils.RemoveSpecialCharacters(postedOn);
                numberOfApplicants = Utils.RemoveSpecialCharacters(numberOfApplicants);
                industry = Utils.RemoveSpecialCharacters(industry);
                employmentType = Utils.RemoveSpecialCharacters(employmentType);
                seniorityLevel = Utils.RemoveSpecialCharacters(seniorityLevel);
                jobFunction = Utils.RemoveSpecialCharacters(jobFunction);
                jobPosterProfileUrl = Utils.RemoveSpecialCharacters(jobPosterProfileUrl);
                jobPosterFirstName = Utils.RemoveSpecialCharacters(jobPosterFirstName);
                jobPosterLastName = Utils.RemoveSpecialCharacters(jobPosterLastName);
                degreeOfConnection = Utils.RemoveSpecialCharacters(degreeOfConnection);

                #endregion

                #region Getting DetailedJobInfoJasonString

                try
                {
                    var objUserScraperDetailedInfo = new JobScraperDetailedInfo
                    {
                        AccountEmail = accountEmail,
                        AccountUserFullName = accountUserFullName,
                        AccountUserProfileUrl = accountUserProfileUrl,
                        CompanyWebsite = companyWebsite,
                        CompanyName = companyName,
                        JobTitle = jobTitle,
                        JobLocation = jobLocation,
                        PostedOn = postedOn,
                        NumberOfApplicants = numberOfApplicants,
                        Industry = industry,
                        EmploymentType = employmentType,
                        SeniorityLevel = seniorityLevel,
                        JobFunction = jobFunction,
                        JobPosterProfileUrl = jobPosterProfileUrl,
                        JobPosterFirstName = jobPosterFirstName,
                        JobPosterLastName = jobPosterLastName,
                        DegreeOfConnection = degreeOfConnection
                    };

                    try
                    {
                        detailedJobInfoJasonString = JsonConvert.SerializeObject(objUserScraperDetailedInfo);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                resultScrapeJobDetails = new Tuple<bool, string>(true, detailedJobInfoJasonString);

                #endregion
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }

            return resultScrapeJobDetails;
        }
    }
}