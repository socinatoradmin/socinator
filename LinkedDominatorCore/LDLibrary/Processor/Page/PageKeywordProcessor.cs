using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.LdTables.Account;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.Factories;
using LinkedDominatorCore.LDLibrary.DAL;
using LinkedDominatorCore.LDUtility;

namespace LinkedDominatorCore.LDLibrary.Processor.Page
{
    public class PageKeywordProcessor : BaseLinkedinPageProcessor, IQueryProcessor
    {
        public PageKeywordProcessor(ILdJobProcess jobProcess, IDbAccountService dbAccountService,
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
            var start = IsBrowser ? 1 : 0;
            try
            {
                List<InteractedPage> listInteractedPageFromAccountDb = null;
                List<DominatorHouseCore.DatabaseHandler.LdTables.Campaign.InteractedPage>
                    listInteractedPageFromCampaignDb = null;
                if (keyword.Contains("”") || keyword.Contains("“"))
                    keyword = keyword.Replace("”", "\"").Replace("“", "\"");

                keyword = WebUtility.UrlEncode(keyword);

                #region GetInteractedGroups 

                listInteractedPageFromAccountDb = DbAccountService.GetInteractedPages(ActivityTypeString).ToList();
                if (!string.IsNullOrEmpty(LdJobProcess.CurrentCampaignId))
                    listInteractedPageFromCampaignDb =
                        DbCampaignService.GetInteractedPages(ActivityTypeString).ToList();

                #endregion

                var constructedActionUrl = IsBrowser
                    ? $"https://www.linkedin.com/search/results/companies/?keywords={keyword}&origin=SWITCH_SEARCH_VERTICAL"
                    : //$"https://www.linkedin.com/voyager/api/search/blended?count=20&filters=List(resultType-%3ECOMPANIES)&keywords={keyword}&origin=SWITCH_SEARCH_VERTICAL&q=all&queryContext=List(spellCorrectionEnabled-%3Etrue)";
                    LdConstants.GetPageSearchAPI(keyword,start);

                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                {
                    #region MyRegion

                    var actionUrl = IsBrowser
                        ? start == 1 ? constructedActionUrl : $"{constructedActionUrl}&page={start}"
                        : //$"{constructedActionUrl}&start={start}";
                        LdConstants.GetPageSearchAPI(keyword, start);
                    var refer =
                        $"https://www.linkedin.com/search/results/companies/?keywords={keyword}&origin=SWITCH_SEARCH_VERTICAL";
                    LdFunctions.SetWebRequestParametersforjobsearchurl(refer, "");
                    var linkedinPageSearchResponseHandler = LdFunctions.LinkedinPageResponseByKeyword(actionUrl);
                    LdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    if (linkedinPageSearchResponseHandler.Success)
                    {
                        #region Filter Already Interacted

                        linkedinPageSearchResponseHandler.PageList.RemoveAll(x =>
                            listInteractedPageFromAccountDb.Any(y => y.PageUrl == x.PageUrl));
                        if (listInteractedPageFromCampaignDb != null && listInteractedPageFromCampaignDb.Count > 0)
                            linkedinPageSearchResponseHandler.PageList.RemoveAll(x =>
                                listInteractedPageFromCampaignDb.Any(y => y.PageUrl == x.PageUrl));

                        #endregion

                        if (linkedinPageSearchResponseHandler.PageList.Count > 0)
                            ProcessLinkedinPageFromPage(queryInfo, ref jobProcessResult,
                                linkedinPageSearchResponseHandler.PageList);
                        if (IsBrowser && start >= 100 || IsBrowser && start >= 1000)
                        {
                            GlobusLogHelper.log.Info("we have reached 100 pages from accounts --> " +
                                                     DominatorAccountModel.AccountBaseModel.UserName);
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
    }
}