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
using System.Linq;

namespace FaceDominatorCore.FDLibrary.FdProcessors.FanpageProcessor
{
    public class PagesLikedByFriendProcessor : BaseFbFanpageProcessor
    {
        IResponseHandler _objPagesLikedByFriendsRsponseHandler;

        public PagesLikedByFriendProcessor(IFdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
            IDbCampaignServiceScoped campaignService, IFdRequestLibrary objFdRequestLibrary,
            IFdBrowserManager browserManager, IProcessScopeModel processScopeModel)
            : base(jobProcess, dbAccountService, campaignService, objFdRequestLibrary, browserManager, processScopeModel)
        {
            _objPagesLikedByFriendsRsponseHandler = null;
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            string keyword = queryInfo.QueryValue;

            if (FdFunctions.FdFunctions.IsIntegerOnly(keyword))
                keyword = $"{FdConstants.FbHomeUrl}{keyword}";

            else if (!keyword.Contains(FdConstants.FbHomeUrl))
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                           AccountModel.AccountBaseModel.UserName, _ActivityType, string.Format("LangKeyQueryValueIsInInvalidFormat".FromResourceDictionary(), $"{keyword}"));
                jobProcessResult.HasNoResult = true;
                return;
            }
            bool isVerifiedFilter = false;

            bool isLikedByFriends = false;

            FanpageCategory objFanpageCategory = FanpageCategory.AnyCategory;

            ApplyFanpageFilter(ref isVerifiedFilter, ref isLikedByFriends, ref objFanpageCategory, queryInfo);

            var entity = queryInfo.QueryType == "Pages Liked By Friends" ? FbEntityType.FanpageLikedByFriends : FbEntityType.Fanpage;

            if (AccountModel.IsRunProcessThroughBrowser)
                Browsermanager.SearchFanpageLikedByFriends(AccountModel, entity, keyword);


            try
            {
                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    try
                    {
                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                        _objPagesLikedByFriendsRsponseHandler = AccountModel.IsRunProcessThroughBrowser
                            ? Browsermanager.ScrollWindowAndGetData(AccountModel, entity, 5, 0, FdConstants.FriendsFanpageLikes2Element)
                            : ObjFdRequestLibrary.GetFanpageDetailsLikedByFriend(AccountModel, keyword, isVerifiedFilter, isLikedByFriends, objFanpageCategory, _objPagesLikedByFriendsRsponseHandler);

                        if (_objPagesLikedByFriendsRsponseHandler.Status)
                        {
                            List<FanpageDetails> listFanpageDetails = _objPagesLikedByFriendsRsponseHandler
                                        .ObjFdScraperResponseParameters.ListPage;

                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                            GlobusLogHelper.log.Info(Log.FoundXResults, AccountModel.AccountBaseModel.AccountNetwork,
                                AccountModel.AccountBaseModel.UserName, listFanpageDetails.Count, queryInfo.QueryType,
                                        queryInfo.QueryValue, _ActivityType);

                            if (isVerifiedFilter)
                            {
                                var backup = listFanpageDetails;
                                listFanpageDetails = backup;
                                listFanpageDetails = listFanpageDetails.Where(x => x.IsVerifiedPage == true.ToString()).ToList();
                                GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, _ActivityType, $"Found {listFanpageDetails.Count} results after applying verified page filter!");
                            }

                            ProcessDataOfPages(queryInfo, ref jobProcessResult, listFanpageDetails);
                            jobProcessResult.maxId = _objPagesLikedByFriendsRsponseHandler.PageletData;

                            jobProcessResult.HasNoResult = !_objPagesLikedByFriendsRsponseHandler.HasMoreResults;
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
                        throw new OperationCanceledException();
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}
