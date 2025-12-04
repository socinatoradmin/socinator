using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using CommonServiceLocator;
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

namespace LinkedDominatorCore.LDLibrary.Processor.Group
{
    public class GroupKeywordsProcessor : BaseLinkedinGroupProcessor, IQueryProcessor
    {
        public GroupKeywordsProcessor(ILdJobProcess jobProcess, IDbAccountService dbAccountService,
            IDbCampaignService campaignService, ILdFunctionFactory ldFunctionFactory, IDelayService delayService,
            IProcessScopeModel processScopeModel) :
            base(jobProcess, dbAccountService, campaignService, ldFunctionFactory, delayService, processScopeModel)
        {
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            var keyword = queryInfo.QueryValue;
            var start = IsBrowser ? 1 : 0;
            try
            {
                var Instance = InstanceProvider.GetInstance<ILdAccountUpdateFactory>();
                Instance.UpdateGroups(DominatorAccountModel, LdFunctions, DbAccountService);
                List<InteractedGroups> listInteractedGroupsFromAccountDb = null;
                List<DominatorHouseCore.DatabaseHandler.LdTables.Campaign.InteractedGroups>
                    listInteractedGroupsFromCampaignDb = null;
                if (keyword.Contains("”") || keyword.Contains("“"))
                    keyword = keyword.Replace("”", "\"").Replace("“", "\"");

                keyword = WebUtility.UrlEncode(keyword);

                #region GetInteractedGroups 

                listInteractedGroupsFromAccountDb = DbAccountService.GetInteractedGroups(ActivityTypeString).ToList();
                if (!string.IsNullOrEmpty(LdJobProcess.CurrentCampaignId))
                    listInteractedGroupsFromCampaignDb =
                        DbCampaignService.GetInteractedGroups(ActivityTypeString).ToList();

                #endregion
                var constructedActionUrl = IsBrowser
                    ? $"https://www.linkedin.com/search/results/groups/?keywords={keyword}&origin=GLOBAL_SEARCH_HEADER"
                    : $"https://www.linkedin.com/voyager/api/search/dash/clusters?decorationId=com.linkedin.voyager.dash.deco.search.SearchClusterCollection-103&origin=SUGGESTION&q=all&query=(keywords:{Uri.EscapeDataString(keyword)},flagshipSearchIntent:SEARCH_SRP,queryParameters:(resultType:List(GROUPS)),includeFiltersInResponse:false)";


                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                {
                    #region MyRegion

                    var actionUrl = IsBrowser
                        ? start == 1 ? constructedActionUrl : $"{constructedActionUrl}&page={start}"
                        : $"{constructedActionUrl}&count=40&start={start}";

                    var linkedinGroupsSearchResponseHandler = LdFunctions.SearchForLinkedinGroups(actionUrl);
                    LdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    if (linkedinGroupsSearchResponseHandler.Success)
                    {
                        #region Filter Already Interacted

                        linkedinGroupsSearchResponseHandler.LinkedinGroupsList.RemoveAll(x =>
                            listInteractedGroupsFromAccountDb.Any(y => y.GroupUrl == x.GroupUrl));
                        if (listInteractedGroupsFromCampaignDb != null && listInteractedGroupsFromCampaignDb.Count > 0)
                            linkedinGroupsSearchResponseHandler.LinkedinGroupsList.RemoveAll(x =>
                                listInteractedGroupsFromCampaignDb.Any(y => y.GroupUrl == x.GroupUrl));

                        #endregion

                        if (linkedinGroupsSearchResponseHandler.LinkedinGroupsList.Count > 0)
                            ProcessLinkedinGroupFromGroup(queryInfo, ref jobProcessResult,
                                linkedinGroupsSearchResponseHandler.LinkedinGroupsList);
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