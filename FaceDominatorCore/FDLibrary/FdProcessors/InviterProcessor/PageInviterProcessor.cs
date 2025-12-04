using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.DatabaseHandler.FdTables.Accounts;
using DominatorHouseCore.Enums.FdQuery;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDLibrary.FdFunctions.FdBrowserManager;
using FaceDominatorCore.FDLibrary.FdProcesses;
using FaceDominatorCore.FDModel.InviterModel;
using FaceDominatorCore.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaceDominatorCore.FDLibrary.FdProcessors.InviterProcessor
{
    public class PageInviterProcessor : BaseFbProcessor
    {
        public PageInviterProcessor(IFdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
            IDbCampaignServiceScoped campaignService, IFdRequestLibrary objFdRequestLibrary,
            IFdBrowserManager browserManager, IProcessScopeModel processScopeModel)
            : base(jobProcess, dbAccountService, campaignService, objFdRequestLibrary, browserManager, processScopeModel)
        {
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {

                try
                {

                    while (!jobProcessResult.IsProcessCompleted)
                    {
                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        if (JobProcess.ModuleSetting.InviterDetailsModel.IsPostUrl)
                        {
                            var listInviterDetails = JobProcess.ModuleSetting.SelectAccountDetailsModel.AccountPagesBoardsPair;

                            var listPages = listInviterDetails.Where(x => x.Key == AccountModel.AccountId).Select(y => y.Value).ToList();

                            var pageDetails = _dbAccountService.Get<OwnPages>().Where(z => listPages.Any(y => y == z.PageUrl)).ToList();

                            var likedPageDetails = _dbAccountService.Get<LikedPages>().Where(z => listPages.Any(y => y == z.PageUrl)).ToList();

                            likedPageDetails.ForEach(page =>
                                pageDetails.Add(new OwnPages()
                                { PageId = page.PageId, PageName = page.PageName, PageUrl = page.PageUrl, PageType = page.PageType, ProfilePicUrl = page.ProfilePicUrl }));

                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            foreach (var inviterDetail in pageDetails)
                            {
                                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                                jobProcessResult = new JobProcessResult();

                                FilterAndStartFinalProcessForInvitePostLikers(ref jobProcessResult, "Post Url",
                                    inviterDetail, JobProcess.ModuleSetting.InviterDetailsModel.IsRandomPosts);
                            }

                            jobProcessResult.IsProcessCompleted = true;
                        }

                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                        if (JobProcess.ModuleSetting.InviterDetailsModel.IsProfileUrl)
                        {
                            SelectAccountDetailsModel selectAccountDetailsModel = new SelectAccountDetailsModel();

                            var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();

                            var pageInviterModel = JsonConvert.DeserializeObject<FanpageInviterModel>(templatesFileManager.Get().FirstOrDefault(x => x.Id == JobProcess.TemplateId)?.ActivitySettings);

                            var listInviterDetails = selectAccountDetailsModel.GetPageInviterDetails(pageInviterModel.SelectAccountDetailsModel).PageInviterDetails;


                            listInviterDetails = listInviterDetails.Where(x => x.Item1 == AccountModel.AccountId)
                                .ToList();

                            var pageDetails = _dbAccountService.GetInteractedUsers(_ActivityType);

                            listInviterDetails.Shuffle();

                            foreach (var inviterDetail in listInviterDetails)
                            {
                                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                                var pageUrl = inviterDetail.Item2;
                                var friendUrl = inviterDetail.Item3;

                                var currentPage = pageDetails.FirstOrDefault(x =>
                                    x.DetailedUserInfo.Contains(pageUrl) && (friendUrl.Contains(x.UserId) ||
                                                                             (!string.IsNullOrEmpty(
                                                                                  x.ScrapedProfileUrl) &&
                                                                              x.ScrapedProfileUrl
                                                                                  .Contains(friendUrl))));

                                if (currentPage != null)
                                {
                                    if (!JobProcess.ModuleSetting.InviterOptionsModel.IsReinvite ||
                                        (currentPage.InteractionDateTime - DateTime.Now).Days <
                                        JobProcess.ModuleSetting.InviterOptionsModel.SendMessageAfterDays)
                                    {
                                        GlobusLogHelper.log.Info(Log.CustomMessage,
                                            AccountModel.AccountBaseModel.AccountNetwork,

                                            AccountModel.AccountBaseModel.UserName, _ActivityType,
                                            string.Format("LangKeyInvitateUser".FromResourceDictionary(), $"{friendUrl}", $"{pageUrl}"));
                                        continue;
                                    }
                                    else if ((currentPage.InteractionDateTime - DateTime.Now).Days >=
                                             JobProcess.ModuleSetting.InviterOptionsModel.SendMessageAfterDays)
                                    {
                                        FilterAndStartFinalProcessForPageInviter("profile", pageUrl,
                                            friendUrl);
                                    }

                                }
                                else
                                    FilterAndStartFinalProcessForPageInviter("profile", pageUrl,
                                        friendUrl);

                            }

                            jobProcessResult.IsProcessCompleted = true;
                        }

                        ObjFdRequestLibrary.ChangeLanguage(AccountModel,
                            FdConstants.AccountLanguage[AccountModel.AccountBaseModel.UserId]);
                    }
                }
                catch (OperationCanceledException)
                {
                    throw new OperationCanceledException("Requested Cancelled !");
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
        }

        private void FilterAndStartFinalProcessForInvitePostLikers(ref JobProcessResult jobProcessResult, string queryType, OwnPages pageDetails, bool isRandomPosts)
        {
            try
            {
                if (!isRandomPosts)
                    FilterAndStartFinalProcessForInviteForEachPost(queryType, pageDetails, JobProcess.ModuleSetting.InviterDetailsModel.ListPostUrl);
                else
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    IResponseHandler objScrapPostListFromFanpageResponseHandler = null;

                    if (AccountModel.IsRunProcessThroughBrowser)
                        Browsermanager.SearchPostsByPageUrl(AccountModel, FbEntityType.Fanpage, pageDetails.PageUrl);

                    while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                    {
                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                        objScrapPostListFromFanpageResponseHandler = AccountModel.IsRunProcessThroughBrowser
                            ? Browsermanager.ScrollWindowAndGetDataForPost(AccountModel, FbEntityType.Fanpage, 7, 0)
                            : ObjFdRequestLibrary.GetPostListFromFanpages(AccountModel, pageDetails.PageUrl, objScrapPostListFromFanpageResponseHandler);


                        if (objScrapPostListFromFanpageResponseHandler.Status)
                        {
                            var postList = objScrapPostListFromFanpageResponseHandler.ObjFdScraperResponseParameters.ListPostDetails.Select(y => y.PostUrl).ToList();
                            FilterAndStartFinalProcessForInviteForEachPost(queryType, pageDetails, postList);
                        }
                        else
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                                AccountModel.AccountBaseModel.UserName, _ActivityType, string.Format("LangKeyNoMorePostsInPage".FromResourceDictionary(), $"{pageDetails.PageUrl}"));
                            jobProcessResult.HasNoResult = true;
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void FilterAndStartFinalProcessForInviteForEachPost(string queryType, OwnPages pageDetails, List<string> listPostUrl)
        {
            try
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                IResponseHandler objPostLikersResponseHandler = null;

                foreach (var postUrl in listPostUrl)
                {
                    var jobProcessResult = new JobProcessResult();

                    if (AccountModel.IsRunProcessThroughBrowser)
                        Browsermanager.SearchPostReactions(AccountModel, BrowserReactionType.Like, $"{postUrl}");

                    while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                    {
                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        int failedCount = 0;

                        var getpostDetail = ObjFdRequestLibrary.GetPostDetailNew(AccountModel,
                                new FacebookPostDetails() { PostUrl = postUrl });
                        if (string.IsNullOrEmpty(pageDetails.PageId))
                            pageDetails.PageId = getpostDetail.ObjFdScraperResponseParameters.PostDetails.OwnerId;
                        if (getpostDetail.ObjFdScraperResponseParameters.PostDetails.OwnerId !=
                            pageDetails.PageId)
                        {

                            GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                                AccountModel.AccountBaseModel.UserName, _ActivityType, $"Post with {postUrl} is not post of page {pageDetails.PageUrl}");
                            break;
                        }
                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                        objPostLikersResponseHandler = AccountModel.IsRunProcessThroughBrowser
                            ? Browsermanager.ScrollWindowAndGetData(AccountModel, FbEntityType.PostLikers, 5, 0, FdConstants.ReactionUserElement)
                            : ObjFdRequestLibrary.GetPostLikers(AccountModel, postUrl, objPostLikersResponseHandler);

                        foreach (var likers in objPostLikersResponseHandler.ObjFdScraperResponseParameters.ListUser)
                        {
                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            if (!FilterAndStartFinalProcessForInviteForEachPage(ref jobProcessResult, queryType,
                                pageDetails, likers, objPostLikersResponseHandler.EntityId))
                            {
                                failedCount++;
                            }
                        }

                        if (objPostLikersResponseHandler.ObjFdScraperResponseParameters.ListUser?.Count != 0 && failedCount == objPostLikersResponseHandler.ObjFdScraperResponseParameters.ListUser?.Count)
                            GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, _ActivityType, $"Already sent page invitation to these {failedCount} users!");

                        if (!objPostLikersResponseHandler.HasMoreResults || objPostLikersResponseHandler.ObjFdScraperResponseParameters.ListUser?.Count == 0)
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, _ActivityType, $"No more likers available for post Url {postUrl}");

                            jobProcessResult.HasNoResult = true;
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        // ReSharper disable once RedundantAssignment
        private bool FilterAndStartFinalProcessForInviteForEachPage([NotNull] ref JobProcessResult jobProcessResult, string queryType, OwnPages pageDetails, FacebookUser likers, string postId)
        {
            try
            {
                var activityType = _ActivityType.ToString();
                var inviterDetails = _dbAccountService.Get<InteractedUsers>(z => z.ActivityType == activityType);

                if (inviterDetails.FirstOrDefault(x => x.DetailedUserInfo.Contains(pageDetails.PageUrl) && (likers.UserId == x.UserId)) != null)
                {
                    jobProcessResult = new JobProcessResult();
                    return false;
                }

                QueryInfo objQueryInfo = new QueryInfo
                {
                    QueryType = queryType,
                    QueryValue = $"{FdConstants.FbHomeUrl}{postId}"
                };

                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                FanpageDetails objFanpageDetails = new FanpageDetails()
                {
                    FanPageUrl = pageDetails.PageUrl,
                    FanPageName = pageDetails.PageName,
                    FanPageID = pageDetails.PageId,
                    FanPageCategory = pageDetails.PageType
                };

                jobProcessResult = JobProcess.FinalProcess(new ScrapeResultNew()
                {
                    ResultUser = likers,
                    QueryInfo = objQueryInfo,
                    ResultPage = objFanpageDetails
                });

                return true;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                jobProcessResult = new JobProcessResult();
            }

            return false;
        }

        private void FilterAndStartFinalProcessForPageInviter(string query, string pageUrl, string friendUrl)
        {
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            var pageDetails = _dbAccountService.Get<OwnPages>().Where(x => x.PageUrl == pageUrl).ToList();

            FanpageDetails objDetails = new FanpageDetails();

            if (pageDetails.Count == 0)
            {
                var likedPageDetails = _dbAccountService.Get<LikedPages>().Where(x => x.PageUrl == pageUrl).ToList();

                if (likedPageDetails.Count > 0)
                {
                    objDetails.FanPageUrl = likedPageDetails[0].PageUrl;
                    objDetails.FanPageID = likedPageDetails[0].PageId;
                    objDetails.FanPageCategory = likedPageDetails[0].PageType;
                    objDetails.FanPageName = likedPageDetails[0].PageName;
                }
            }
            else
            {
                objDetails.FanPageUrl = pageDetails[0].PageUrl;
                objDetails.FanPageID = pageDetails[0].PageId;
                objDetails.FanPageCategory = pageDetails[0].PageType;
                objDetails.FanPageName = pageDetails[0].PageName;
            }

            FilterAndStartInviterProcessForEachPage(query, objDetails, friendUrl);

        }

        // ReSharper disable once RedundantAssignment
        private void FilterAndStartInviterProcessForEachPage(string query, FanpageDetails pageDetails, string friendUrl)
        {
            try
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                FacebookUser objUser = new FacebookUser();

                var friendDetails = _dbAccountService.Get<Friends>().Where(x => friendUrl.Contains(x.FriendId)).ToList()/*[0]*/.FirstOrDefault();

                if (friendDetails == null)
                {
                    string friendId = AccountModel.IsRunProcessThroughBrowser ?
                        Browsermanager.GetFullUserDetails(AccountModel, new FacebookUser() { ScrapedProfileUrl = friendUrl }).ObjFdScraperResponseParameters.FacebookUser.UserId :
                        ObjFdRequestLibrary.GetFriendUserId(AccountModel, friendUrl).UserId;

                    if (_dbAccountService.Count<Friends>(x => x.FriendId == friendId) == 0)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                            AccountModel.AccountBaseModel.UserName, _ActivityType,
                             string.Format("LangKeyNotAFriend".FromResourceDictionary(), $"{friendUrl}"));
                        return;
                    }

                    friendDetails = _dbAccountService.Get<Friends>(x => x.ProfileUrl.Contains(friendId)).ToList()
                        .FirstOrDefault();
                }
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                if (friendDetails == null)
                    GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                        AccountModel.AccountBaseModel.UserName, _ActivityType, string.Format("LangKeyNotAFriend".FromResourceDictionary(), $"{friendUrl}"));

                if (friendDetails != null)
                {
                    objUser.UserId = friendDetails.FriendId;
                    objUser.ProfileUrl = friendDetails.ProfileUrl;
                    objUser.Familyname = friendDetails.FullName;
                    objUser.ScrapedProfileUrl = friendUrl;
                    objUser.ProfilePicUrl = FdRegexUtility.FirstMatchExtractor(friendDetails.DetailedUserInfo, "ProfilePicUrl\":\"(.*?)\"");

                    FilterAndStartFinalProcessForPageInviteProfile(query, objUser, pageDetails, out _);
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void FilterAndStartFinalProcessForPageInviteProfile(string query, FacebookUser friendDetail, FanpageDetails pageDetails, out JobProcessResult jobProcessResult)
        {
            try
            {
                QueryInfo objQueryInfo = new QueryInfo { QueryType = query, QueryValue = friendDetail.ProfileUrl };
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                jobProcessResult = JobProcess.FinalProcess(new ScrapeResultNew()
                {
                    ResultUser = friendDetail,
                    QueryInfo = objQueryInfo,
                    ResultPage = pageDetails
                });
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                jobProcessResult = new JobProcessResult();
            }
        }


    }
}
