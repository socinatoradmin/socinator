using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using CommonServiceLocator;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.LdTables.Account;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.Factories;
using LinkedDominatorCore.LDLibrary.DAL;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDModel.GrowConnection;
using LinkedDominatorCore.LDUtility;
using LinkedDominatorCore.Request;
using LinkedDominatorCore.Utility;

namespace LinkedDominatorCore.LDLibrary.GrowConnectionProcesses
{
    public class FollowPageProcess : LDJobProcessInteracted<InteractedPage>
    {
        private readonly IDelayService _delayService;
        private readonly ILdFunctions _ldFunctions;
        private JobProcessResult _jobProcessResult;

        public FollowPageProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IDbGlobalService globalService,
            IExecutionLimitsManager executionLimitsManager, ILdQueryScraperFactory queryScraperFactory,
            ILdHttpHelper ldHttpHelper, ILdLogInProcess logInProcess, ILdFunctionFactory ldFunctionFactory,
            IDbInsertionHelper dbInsertionHelper, IDelayService delayService)
            : base(processScopeModel, accountServiceScoped, globalService, executionLimitsManager, queryScraperFactory,
                ldHttpHelper, logInProcess, dbInsertionHelper)
        {
            FollowPagesModel = processScopeModel.GetActivitySettingsAs<FollowPagesModel>();
            _ldFunctions = ldFunctionFactory.LdFunctions;
            _delayService = delayService;
        }

        public FollowPagesModel FollowPagesModel { get; set; }
        public string CurrentActivityType { get; set; }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            _jobProcessResult = new JobProcessResult();
            #region Follow Page Process.
            var ldCampaignInteractionDetails = InstanceProvider.GetInstance<ICampaignInteractionDetails>();
            var IsFollow = false;
            string PostData = null, PostUrl = null;

            try
            {
                var objLinkedinPage = (LinkedinPage)scrapeResult.ResultPage;
                if (_jobProcessResult.IsProcessSuceessfull) return _jobProcessResult;

                if (new LdUniqueHandler().IsCampaignWiseUnique(new UniquePreRequisticProperties
                {
                    AccountModel = DominatorAccountModel,
                    ActivityType = ActivityType,
                    CampaignId = CampaignId,
                    IsUniqueOperationChecked = FollowPagesModel.IsUniqueOperationChecked,
                    ProfileUrl = objLinkedinPage.PageUrl
                }))
                    return _jobProcessResult;

                var jobActivityConfigurationManager =
                    InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
                var moduleSetting = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

                // filtering user for globally unique
                if (new LdUniqueHandler().IsGlobalUnique(new UniquePreRequisticProperties
                {
                    AccountModel = DominatorAccountModel,
                    ActivityType = ActivityType,
                    CampaignId = CampaignId,
                    IsUniqueOperationChecked = FollowPagesModel.IsUniqueOperationChecked,
                    ProfileUrl = objLinkedinPage.PageUrl
                }))

                    return _jobProcessResult;
                GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objLinkedinPage.PageName);
                if (IsBrowser)
                {
                    ForBrowser(ref IsFollow, ref PostUrl, objLinkedinPage);
                }
                else
                {
                    string posturl, postdate;
                    bool isFollow;
                    GeneratePostData(objLinkedinPage, out isFollow, out posturl, out postdate);
                    PostData = postdate;
                    PostUrl = posturl;
                    IsFollow = isFollow;
                }


                if (IsFollow)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        "already Follow " + objLinkedinPage.PageName + " from outside/inside of the software");
                    _delayService.ThreadSleep(3000);
                    _jobProcessResult.IsProcessSuceessfull = false;
                    return _jobProcessResult;
                }
                _delayService.ThreadSleep(RandomUtilties.GetRandomNumber(5000,2000));
                var response = _ldFunctions.FollowComapanyPages(PostData, PostUrl);
                if (response?.Response == string.Empty)
                {
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objLinkedinPage.PageName);
                    IncrementCounters();
                    objLinkedinPage.IsFollowed = "True";
                    DbInsertionHelper.Followpages(scrapeResult, objLinkedinPage);
                    _jobProcessResult.IsProcessSuceessfull = true;
                }
                else
                {
                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    var message = response?.Exception?.Message;
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, message);
                    if (FollowPagesModel.IsUniqueOperationChecked && moduleSetting.IsTemplateMadeByCampaignMode)
                    {
                        AddInteractedData(ldCampaignInteractionDetails, objLinkedinPage.PageUrl);
                        GlobusLogHelper.log.Info(Log.ActivityFailed,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objLinkedinPage.PageName,
                            "");
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(Log.ActivityFailed,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objLinkedinPage.PageName,
                            "");
                    }
                }
                StartOtherConfiguration(scrapeResult);
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
            #endregion
            return _jobProcessResult;
        }

        private void ForBrowser(ref bool IsFollow, ref string PostUrl, LinkedinPage objLinkedinPage)
        {
            var ldPage = _ldFunctions.GetHtmlFromUrlNormalMobileRequest(objLinkedinPage.PageUrl);

            var forEmployees = HtmlAgilityHelper.GetStringInnerHtmlFromClassName(ldPage,
                "ph5");
            var noOfemployees = HtmlAgilityHelper.GetStringTextFromClassName(forEmployees, "org-top-card-secondary-content__see-all t-normal t-black--light link-without-visited-state link-without-hover-state");
            if (string.IsNullOrEmpty(noOfemployees))
                noOfemployees = Utilities.GetBetween(forEmployees, "class=\"link-without-visited-state t-bold t-black--light\">", "</span>");

            noOfemployees = Regex.Replace(noOfemployees, "[^0-9]", "");
            var FollowerDetails =
                HtmlAgilityHelper.GetStringInnerHtmlFromClassName(ldPage,
                    "inline-block");
            var followArray = Regex.Split(FollowerDetails, "org-top-card-summary-info-list__info-item");
            objLinkedinPage.FollowerCount = followArray.Length > 0 ?Utils.AssignNa(Regex.Replace(Regex.Replace(Utils.RemoveHtmlTags(followArray.FirstOrDefault(x => x.Contains("followers") || x.Contains("follower"))), "[0-9](follower|followers)(.*?)", ""), "[^0-9]", "")) : "N/A";
            objLinkedinPage.StaffCount = followArray.Length > 0 ?string.IsNullOrEmpty(noOfemployees)? Utils.AssignNa(Regex.Replace(Utils.RemoveHtmlTags(followArray.FirstOrDefault(x=>x.Contains("employees") || x.Contains("employee"))),"[^0-9]","")):Utils.AssignNa(noOfemployees) : "N/A";
            var PageId = Uri.UnescapeDataString(Utilities.GetBetween(forEmployees, "href=\"", "\""));
            PageId = Regex.Replace(PageId, "[^0-9]", "");
            objLinkedinPage.PageId = PageId;
            IsFollow = !ldPage.Contains("follow   org-company-follow-button org-top-card-primary-actions__action artdeco-button");
            PostUrl = objLinkedinPage.PageUrl;
        }

        private void GeneratePostData(LinkedinPage objLinkedinPage, out bool isFollow, out string posturl,
            out string postdate)
        {
            var PageUrl = objLinkedinPage.PageUrl;
            posturl = postdate = string.Empty;isFollow = false;
            var urlForPageDetails =
                $"https://www.linkedin.com/voyager/api/organization/companies?decorationId=com.linkedin.voyager.deco.organization.web.WebFullCompanyMain-20&q=universalName&universalName={WebUtility.UrlEncode(objLinkedinPage.PageUrl.Replace("https://www.linkedin.com/company/", "")?.Replace("/", ""))}";
            var getCompanydetails = _ldFunctions.GetInnerHttpHelper().GetRequest(urlForPageDetails);
            if (string.IsNullOrEmpty(getCompanydetails.Response))
                return;
            var json = new JsonHandler(getCompanydetails.Response);
            var jsonHandler = JsonJArrayHandler.GetInstance;
            var followerJsonData =jsonHandler.ParseJsonToJObject(json.GetJToken("included").Where(x=>jsonHandler.GetJTokenValue(x, "entityUrn").Contains("urn:li:company")).FirstOrDefault().ToString());
            var followercount = jsonHandler.GetJTokenValue(followerJsonData,"followerCount");
            isFollow = jsonHandler.GetJTokenValue(followerJsonData, "following") == "True";
            var i = 0;
            var lastdetails = json.GetJToken("included");
            var jsonData = lastdetails.Where(x => x.ToString().Contains("companyEmployeesSearchPageUrl"))
                .FirstOrDefault().ToString();
            json = new JsonHandler(jsonData);
            objLinkedinPage.FollowerCount = followercount;
            var StaffCount = json.GetElementValue("staffCount");
            objLinkedinPage.StaffCount = StaffCount;
            var versionTag = json.GetElementValue("versionTag");
            objLinkedinPage.IsFollowed = isFollow.ToString();
            objLinkedinPage.UniversalPageName = json.GetElementValue("universalName");
            var dataVersion = json.GetElementValue("dataVersion");
            _ldFunctions.SetWebRequestParametersforjobsearchurl(PageUrl, "");
            _ldFunctions.GetInnerLdHttpHelper().GetRequestParameter().ContentType = "application/json; charset=UTF-8";
            posturl = $"https://www.linkedin.com/voyager/api/organization/companies/{objLinkedinPage.PageId}";
            int.TryParse(followercount, out i);
            postdate =
                $"%7B\"patch\":%7B\"followingInfo\":%7B\"$set\":%7B\"followerCount\":{++i},\"following\":true%7D%7D,\"$set\":%7B\"dataVersion\":{dataVersion},\"versionTag\":\"{versionTag}\"%7D%7D%7D";
            postdate = Uri.UnescapeDataString(postdate);
        }

        private void AddInteractedData(ICampaignInteractionDetails ldCampaignInteractionDetails, string url)
        {
            try
            {
                ldCampaignInteractionDetails.AddInteractedData(SocialNetworks.LinkedIn,
                    CampaignId, url);
            }
            catch (Exception)
            {
            }
        }
    }
}