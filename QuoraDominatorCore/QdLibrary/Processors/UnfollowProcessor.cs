using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using QuoraDominatorCore.Enums;
using QuoraDominatorCore.QdLibrary.DAL;
using QuoraDominatorCore.QdLibrary.QdFunctions;
using QuoraDominatorCore.QdUtility;
using QuoraDominatorCore.Request;
using QuoraDominatorCore.Response;

namespace QuoraDominatorCore.QdLibrary.Processors
{
    internal class UnfollowProcessor : BaseQuoraProcessor
    {
        private readonly IQdHttpHelper _httpHelper;
        public UnfollowProcessor(IQuoraBrowserManager browser, IProcessScopeModel processScopeModel,
            IQdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService, IDbGlobalService globalService,
            IDbCampaignService campaignService, IQuoraFunctions quoraFunctions, IQdHttpHelper httpHelper) :
            base(browser, jobProcess, dbAccountService, globalService, campaignService, quoraFunctions,
                processScopeModel)
        {
            _httpHelper = httpHelper;
        }
        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                if (_unFollowModel.IsChkPeopleFollowedBySoftwareChecked &&
                    _unFollowModel.IsChkPeopleFollowedOutsideSoftwareChecked &&
                    _unFollowModel.IsChkCustomUsersListChecked)
                    queryInfo.QueryValue = "LangKeyPeopleFollowedBySoftwareOutsideSoftwareAndCustomUserList"
                        .FromResourceDictionary();
                else if (_unFollowModel.IsChkPeopleFollowedBySoftwareChecked &&
                         _unFollowModel.IsChkPeopleFollowedOutsideSoftwareChecked)
                    queryInfo.QueryValue = "LangKeyPeopleFollowedBySoftwareAndOutsideSoftware".FromResourceDictionary();
                else if (_unFollowModel.IsChkPeopleFollowedOutsideSoftwareChecked &&
                         _unFollowModel.IsChkCustomUsersListChecked)
                    queryInfo.QueryValue =
                        "LangKeyPeopleFollowedByOutsideSoftwareAndCustomUserList".FromResourceDictionary();
                else if (_unFollowModel.IsChkPeopleFollowedBySoftwareChecked &&
                         _unFollowModel.IsChkCustomUsersListChecked)
                    queryInfo.QueryValue = "LangKeyPeopleFollowedBySoftwareAndCustomUserList".FromResourceDictionary();
                else if (_unFollowModel.IsChkPeopleFollowedBySoftwareChecked)
                    queryInfo.QueryValue = "LangKeyPeopleFollowedBySoftware".FromResourceDictionary();
                else if (_unFollowModel.IsChkPeopleFollowedOutsideSoftwareChecked)
                    queryInfo.QueryValue = "LangKeyPeopleFollowedOutsideSoftware".FromResourceDictionary();
                else if (_unFollowModel.IsChkCustomUsersListChecked)
                    queryInfo.QueryValue = "LangKeyCustomUsersList".FromResourceDictionary();

                GlobusLogHelper.log.Info(Log.CustomMessage,
                    JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType,
                    $"Searching for {queryInfo.QueryType} {queryInfo.QueryValue}");

                var usernames = new List<string>();
                var ProfileUrl=string.Empty;
                FollowingsResponseHandler scrapedFollowing = null;
                var IsBrowser = JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser;
                if (_unFollowModel.IsChkCustomUsersListChecked)
                    usernames.AddRange(Regex.Split(_unFollowModel.CustomUsersList, "\r\n").ToList());
                else if (_unFollowModel.IsChkPeopleFollowedOutsideSoftwareChecked || _unFollowModel.IsChkPeopleFollowedBySoftwareChecked)
                {
                    var userName = JobProcess.DominatorAccountModel.AccountBaseModel.UserFullName;
                    userName = string.IsNullOrEmpty(userName) ? QdUtilities.GetDecodedResponse(quoraFunct.GetProfileUrl(JobProcess.DominatorAccountModel)) : userName;
                    var SearchByCustomUrlFailedCount = 0;
                    ProfileUrl = userName.StartsWith("https://") ? userName : $"{QdConstants.HomePageUrl}/profile/" + userName;
                TryAgain:
                    if (IsBrowser)
                        _browser.SearchByCustomUrl(JobProcess.DominatorAccountModel, ProfileUrl);
                    var response = !IsBrowser ? quoraFunct.GetUserActivityDetailsByType(quoraFunct.GetUserId(ProfileUrl), UserActivityType.ProfileFollowings, JobProcess.DominatorAccountModel, -1, "MostRecent").Result : _browser.GetFollwersAndFollowingsFromProfile(JobProcess.DominatorAccountModel, UserFollowTypes.OwnFollowings);
                    while (SearchByCustomUrlFailedCount++ < 3 && string.IsNullOrEmpty(response.Response))
                        goto TryAgain;
                    scrapedFollowing = new FollowingsResponseHandler(response, IsBrowser);
                    scrapedFollowing.FollowingList = CheckSourceFilterWhoFollowBackAndDoNotFollowBack(scrapedFollowing.FollowingList, _browser, JobProcess.DominatorAccountModel, ProfileUrl);
                    if (scrapedFollowing != null && scrapedFollowing.FollowingList.Count > 0)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Quora, JobProcess.DominatorAccountModel.UserName, ActivityType,
                        $"Found {scrapedFollowing.FollowingList.Count} users");
                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        if (_unFollowModel.IsChkPeopleFollowedBySoftwareChecked && !_unFollowModel.IsChkPeopleFollowedOutsideSoftwareChecked)
                            usernames.AddRange(DbAccountService.GetInteractedUser());
                        else
                            usernames = scrapedFollowing.FollowingList;
                    }
                }
                if (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult && usernames.Count > 0)
                    jobProcessResult = FilterAndStartFinalProcess(null, jobProcessResult, usernames);
                if (!_unFollowModel.IsChkCustomUsersListChecked && (!_unFollowModel.IsChkPeopleFollowedOutsideSoftwareChecked || jobProcessResult.IsProcessCompleted
                    || StartPagination(ref scrapedFollowing, ref jobProcessResult,ProfileUrl,IsBrowser)))
                    return;
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            finally {
                if (_browser.BrowserWindow != null) _browser.CloseBrowser();
                jobProcessResult.IsProcessCompleted = true;
            }
        }

        private List<string> CheckSourceFilterWhoFollowBackAndDoNotFollowBack(List<string> followingList, IQuoraBrowserManager browser,DominatorAccountModel dominatorAccountModel,string url)
        {
            var SearchByCustomUrlFailedCount = 0;
            TryAgain:
            var ScrappedFollower = _browser.SearchByCustomUrl(JobProcess.DominatorAccountModel, url);
            ScrappedFollower = _browser.GetFollwersAndFollowingsFromProfile(dominatorAccountModel, UserFollowTypes.OwnFollowers);
            while (SearchByCustomUrlFailedCount++ < 3 && string.IsNullOrEmpty(ScrappedFollower.Response))
                goto TryAgain;
            var Followers = new FollowerResponseHandler(ScrappedFollower);
            if (_unFollowModel.IsWhoDoNotFollowBackChecked)
                followingList.RemoveAll(x =>Followers.FollowerList.Any(y=>y==x||y.Contains(x)));
            else if(_unFollowModel.IsWhoFollowBackChecked)
                followingList.RemoveAll(x => !Followers.FollowerList.Any(y => y == x));
            return followingList;
        }

        private bool StartPagination(ref FollowingsResponseHandler scrapedFollowing,
            ref JobProcessResult jobProcessResult,string ProfileUrl,bool IsBrowser)
        {
            #region OLD Pagination Logic.
            //var responseFollowerFirstPage = scrapedFollowing.Response.Response;
            //var post = new BasePostData(responseFollowerFirstPage);
            //do
            //{
            //    try
            //    {
            //        var urlServerCallPost = "https://www.quora.com/webnode2/server_call_POST?_v=" +
            //                                Uri.EscapeDataString(post.VconJson) +
            //                                "&_m=increase_count";

            //        var postDataCallPost = "json=%7B%22args%22%3A%5B%5D%2C%22kwargs%22%3A%7B%22cid%22%3A%22" +
            //                               post.PagedList +
            //                               "%22%2C%22num%22%3A18%2C%22current%22%3A72%7D%7D&revision=" +
            //                               post.Revision + "&formkey=" + post.FormKey + "&postkey=" + post.PostKey +
            //                               "&window_id=" + post.WindowId +
            //                               "&referring_controller=user&referring_action=followers&__vcon_json=%5B%22" +
            //                               "" +
            //                               Uri.EscapeDataString(post.VconJson) +
            //                               "%22%5D&__vcon_method=increase_count&__e2e_action_id=exz34w8dfi&js_init=%7B%22object_id%22%3A44794736%2C%22initial_count%22%3A18%2C%22buffer_count%22%3A18%2C%22crawler%22%3Afalse%2C%22has_more%22%3Atrue%2C%22retarget_links%22%3Atrue%2C%22fixed_size_paged_list%22%3Afalse%2C%22auto_paged%22%3Atrue%7D&__metadata=%7B%7D";

            //        _httpHelper.PostRequest(urlServerCallPost, postDataCallPost);
            //        var urlPagination = "https://tch678384.tch.quora.com/up/" + post.Chan + "/updates?&callback=" +
            //                            post.JsonP + "&" +
            //                            "min_seq=" + post.MinSeq + "&channel=" + post.Channel + "&hash=" + post.Hash +
            //                            "&_=" + GenerateTimeStamp();

            //        scrapedFollowing = quoraFunct.Following(JobProcess.DominatorAccountModel, urlPagination);
            //        if (jobProcessResult.IsProcessCompleted)
            //            return true;
            //        if (scrapedFollowing.FollowingList.Count == 0)
            //        {
            //            numberOfAttempt++;
            //            break;
            //        }

            //        numberOfAttempt = 0;
            //        if (scrapedFollowing.Success)
            //            if (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
            //                jobProcessResult =
            //                    FilterAndStartFinalProcess(null, jobProcessResult, scrapedFollowing.FollowingList);
            //        var followerpaginationData = scrapedFollowing.Response.Response;
            //        if (followerpaginationData.Contains("jsonp"))
            //            post.JsonP = "jsonp" + Utilities.GetBetween(followerpaginationData, "jsonp", "({");
            //        if (followerpaginationData.Contains("min_seq\":"))
            //            post.MinSeq = Utilities.GetBetween(followerpaginationData, "min_seq\":", "})");
            //    }
            //    catch (Exception ex)
            //    {
            //        ex.DebugLog();
            //        numberOfAttempt++;
            //        break;
            //    }
            //} while (!string.IsNullOrEmpty(post.MinSeq));
            #endregion
            var IsSuccess = false;
            var usernames = new List<string>();
            try
            {
                var SearchByCustomUrlFailedCount = 0;
            TryAgain:
                if (IsBrowser)
                    _browser.SearchByCustomUrl(JobProcess.DominatorAccountModel, ProfileUrl);
                var response = !IsBrowser ? quoraFunct.GetUserActivityDetailsByType(quoraFunct.GetUserId(ProfileUrl), UserActivityType.ProfileFollowings, JobProcess.DominatorAccountModel,scrapedFollowing.PaginationCount, "Paginated").Result : _browser.GetFollwersAndFollowingsFromProfile(JobProcess.DominatorAccountModel, UserFollowTypes.OwnFollowings);
                while (SearchByCustomUrlFailedCount++ < 3 && string.IsNullOrEmpty(response.Response))
                    goto TryAgain;
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                scrapedFollowing = new FollowingsResponseHandler(response, IsBrowser);
                scrapedFollowing.FollowingList = CheckSourceFilterWhoFollowBackAndDoNotFollowBack(scrapedFollowing.FollowingList, _browser, JobProcess.DominatorAccountModel, ProfileUrl);
                if (scrapedFollowing != null && scrapedFollowing.FollowingList.Count > 0)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Quora, JobProcess.DominatorAccountModel.UserName, ActivityType,
                    $"Found {scrapedFollowing.FollowingList.Count} users");
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    if (_unFollowModel.IsChkPeopleFollowedBySoftwareChecked && !_unFollowModel.IsChkPeopleFollowedOutsideSoftwareChecked)
                        usernames.AddRange(DbAccountService.GetInteractedUser());
                    else
                        usernames = scrapedFollowing.FollowingList;
                }
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                if (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult && usernames.Count > 0)
                    jobProcessResult = FilterAndStartFinalProcess(null, jobProcessResult, usernames);
            }
            catch (Exception)
            {

            }finally { 
                IsSuccess = true;
                jobProcessResult.IsProcessCompleted = true;
            }
            return IsSuccess;
        }
    }
}