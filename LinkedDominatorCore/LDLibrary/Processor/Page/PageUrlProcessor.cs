using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.LdTables.Account;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.Factories;
using LinkedDominatorCore.LDLibrary.DAL;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDUtility;

namespace LinkedDominatorCore.LDLibrary.Processor.Page
{
    public class PageUrlProcessor : BaseLinkedinPageProcessor, IQueryProcessor
    {
        public PageUrlProcessor(ILdJobProcess jobProcess, IDbAccountService dbAccountService,
            IDbGlobalService globalService,
            IDbCampaignService campaignService, ILdFunctionFactory ldFunctionFactory, IDelayService delayService,
            IProcessScopeModel processScopeModel) :
            base(jobProcess, dbAccountService, globalService, campaignService, ldFunctionFactory, delayService,
                processScopeModel)
        {
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            var keyword = queryInfo.QueryValue;
            var CustomPagelist = new List<LinkedinPage>();
            try
            {
                List<InteractedPage> listInteractedPageFromAccountDb = null;
                List<DominatorHouseCore.DatabaseHandler.LdTables.Campaign.InteractedPage>
                    listInteractedPageFromCampaignDb = null;
                if (keyword.Contains("”") || keyword.Contains("“"))
                    keyword = keyword.Replace("”", "\"").Replace("“", "\"");


                #region GetInteractedpage

                listInteractedPageFromAccountDb = DbAccountService.GetInteractedPages(ActivityTypeString).ToList();
                if (!string.IsNullOrEmpty(LdJobProcess.CurrentCampaignId))
                    listInteractedPageFromCampaignDb =
                        DbCampaignService.GetInteractedPages(ActivityTypeString).ToList();

                #endregion
                var universalName = Utils.GetCompanyUniversalName(keyword);
                keyword = $"https://www.linkedin.com/company/{universalName}/";
                var constructedActionUrl = IsBrowser
                    ? keyword
                    : $"https://www.linkedin.com/voyager/api/organization/companies?decorationId=com.linkedin.voyager.deco.organization.web.WebFullCompanyMain-20&q=universalName&universalName={universalName}";
                if (IsBrowser)
                {
                    ForBrowser(keyword, CustomPagelist, constructedActionUrl);
                }
                else
                {
                    var refer = queryInfo.QueryValue;
                    LdFunctions.SetWebRequestParametersforjobsearchurl(refer, "");
                    var linkedinPageUrlResponseHandler =
                        LdFunctions.GetInnerHttpHelper().GetRequest(constructedActionUrl);
                    var json = new JsonHandler(linkedinPageUrlResponseHandler.Response);
                    var lastdetails = json.GetJToken("included").Last();
                    var ObjLinkedinPage = new LinkedinPage
                    {
                        PageId = json.GetElementValue("data", "*elements", 0)
                            .Replace("urn:li:fs_normalized_company:", ""),
                        PageName = json.GetJTokenValue(lastdetails, "name"),
                        PageUrl = keyword
                    };
                    CustomPagelist.Add(ObjLinkedinPage);
                }

                LdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                #region Filter Already Interacted

                CustomPagelist.RemoveAll(x => listInteractedPageFromAccountDb.Any(y => y.PageUrl == x.PageUrl));
                if (listInteractedPageFromCampaignDb != null && listInteractedPageFromCampaignDb.Count > 0)
                    CustomPagelist.RemoveAll(x => listInteractedPageFromCampaignDb.Any(y => y.PageUrl == x.PageUrl));

                #endregion

                if (CustomPagelist.Count > 0)
                {
                    ProcessLinkedinPageFromPage(queryInfo, ref jobProcessResult, CustomPagelist);
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.NoMoreDataToPerform,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType);
                    jobProcessResult.HasNoResult = true;
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

        private void ForBrowser(string keyword, List<LinkedinPage> CustomPagelist, string constructedActionUrl)
        {
            var ldPage = LdFunctions.GetHtmlFromUrlNormalMobileRequest(constructedActionUrl);
            var PageName = HtmlAgilityHelper.GetStringInnerTextFromClassName(ldPage,"org-top-card-summary__title t-24 t-black t-bold truncate");
            PageName = string.IsNullOrEmpty(PageName) ? HtmlAgilityHelper.GetStringInnerTextFromClassName(ldPage,
                    "t-24 t-black t-bold") : PageName;
            PageName = string.IsNullOrEmpty(PageName) ?HtmlAgilityHelper.GetStringInnerTextFromClassName(ldPage, "ember-view text-display-medium-bold org-top-card-summary__title") : PageName;
            var forEmployees = HtmlAgilityHelper.GetStringInnerHtmlFromClassName(ldPage,"ph5");
            var noOfemployees = HtmlAgilityHelper.GetStringTextFromClassName(forEmployees, "org-top-card-secondary-content__see-all t-normal t-black--light link-without-visited-state link-without-hover-state");
            noOfemployees = string.IsNullOrEmpty(noOfemployees) ?HtmlAgilityHelper.GetStringTextFromClassName(forEmployees, "t-normal t-black--light link-without-visited-state link-without-hover-state") : noOfemployees;
            noOfemployees = string.IsNullOrEmpty(noOfemployees) ? Utils.GetBetween(forEmployees, "class=\"link-without-visited-state t-bold t-black--light\">", "</span>") : noOfemployees;
            noOfemployees = Regex.Replace(noOfemployees, "[^0-9]", "");

            var FollowerDetails =
                HtmlAgilityHelper.GetStringInnerHtmlFromClassName(ldPage,
                    "org-top-card-summary-info-list t-14 t-black--light");
            if (string.IsNullOrEmpty(FollowerDetails))
            {
                var pageInfo = HtmlAgilityHelper.GetListInnerHtmlFromClassName(forEmployees, "org-top-card-summary-info-list__info-item");
                if(pageInfo.Count>0)
                    FollowerDetails = Regex.Replace(pageInfo.Last(x=>x.Contains("followers")), "[^0-9]","");
            }
            var PageId = Uri.UnescapeDataString(Utilities.GetBetween(forEmployees, "href=\"", "\""));
            PageId = Regex.Replace(PageId, "[^0-9]", "");
            var isFollowed = !ldPage.Contains("org-company-follow-button org-top-card-primary-actions__action artdeco-button artdeco-button--primary");
            var ObjLinkedinPagedetails = new LinkedinPage
            {
                PageId = PageId,
                PageUrl = keyword,
                PageName = PageName,
                StaffCount=noOfemployees,
                FollowerCount=FollowerDetails,
                IsFollowed=isFollowed.ToString(),
                UniversalPageName=constructedActionUrl.Split('/').LastOrDefault(x=>x!=string.Empty)
            };
            CustomPagelist.Add(ObjLinkedinPagedetails);
        }

        public void IsCsrfChanged()
        {
            try
            {
                if (!LdFunctions.GetInnerHttpHelper().GetRequestParameter().ToString().Contains("Csrf-Token"))
                {
                    LdFunctions.GetInnerHttpHelper().GetRequestParameter().Headers.Add("Csrf-Token",
                        LdFunctions.GetInnerHttpHelper().GetRequestParameter().Cookies["JSESSIONID"]?.Value
                            .Replace("\"", ""));
                    return;
                }

                var header = LdFunctions.GetInnerHttpHelper().GetRequestParameter().Headers["Csrf-Token"];
                if (header.Equals(LdFunctions.GetInnerHttpHelper().GetRequestParameter().Cookies["JSESSIONID"]?.Value
                    .Replace("\"", ""))) return;

                LdFunctions.GetInnerHttpHelper().GetRequestParameter().Headers.Remove("Csrf-Token");
                LdFunctions.GetInnerHttpHelper().GetRequestParameter().Headers.Add("Csrf-Token",
                    LdFunctions.GetInnerHttpHelper().GetRequestParameter().Cookies["JSESSIONID"]?.Value
                        .Replace("\"", ""));
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }
        }
    }
}