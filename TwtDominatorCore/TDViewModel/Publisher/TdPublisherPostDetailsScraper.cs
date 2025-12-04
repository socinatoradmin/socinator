using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.SocioPublisher;
using DominatorHouseCore.Enums.TdQuery;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using TwtDominatorCore.Response;
using TwtDominatorCore.TDLibrary;
using TwtDominatorCore.TDModels;
using TwtDominatorCore.TDUtility;
using Unity;

namespace TwtDominatorCore.TDViewModel.Publisher
{
    public class TdPublisherPostDetailsScraper : PostScraper
    {
        private readonly IAccountScopeFactory _accountScopeFactory;
        private readonly IAccountsFileManager _accountsFileManager;
        private readonly Dictionary<string, string> ListSearchKeyWordWithPagination = new Dictionary<string, string>();
        private readonly object LockScrapPosts = new object();
        private string campaignId = string.Empty;
        private DominatorAccountModel domAccModel;
        private ITwitterFunctions twitterFunction;
        public TdPublisherPostDetailsScraper(string CampaignId, CancellationTokenSource campaignCancellationToken,
            PublisherPostFetchModel postFetchModel) :
            base(CampaignId, campaignCancellationToken, postFetchModel)
        {
            _accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
            _accountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();
        }
        public override void ScrapeFdKeywords(string accountId, string campaignId, SharePostModel tdKeywordScraperDetails, CancellationTokenSource cancellationTokenSource, int count = 10)
        {
            if (string.IsNullOrEmpty(accountId) || string.IsNullOrEmpty(campaignId) || tdKeywordScraperDetails == null)
                return;
            try
            {
                ScrapePostLogin(accountId);
                this.campaignId=campaignId;
                var currentQueryPosts = 1;
                var query = tdKeywordScraperDetails.AddKeywords;
                if (query.Contains("[K]") && domAccModel.IsUserLoggedIn)
                {
                    var keyword = query.Replace("[K]", "").Trim();
                    var HasMoreResult = true;
                    var minPosition = "";
                    while (currentQueryPosts <= count && HasMoreResult)
                    {
                        var SearchTagHandler = twitterFunction.SearchForTag(domAccModel, keyword,
                            "Keywords", cancellationTokenSource.Token, minPosition);

                        if (SearchTagHandler == null || string.IsNullOrEmpty(SearchTagHandler.MinPosition))
                            break;

                        if (SearchTagHandler.HasMore)
                            minPosition = SearchTagHandler.MinPosition;
                        else minPosition = null;
                        HasMoreResult = !string.IsNullOrEmpty(minPosition);
                        if(SearchTagHandler!=null && SearchTagHandler.ListTagDetails.Count > 0)
                        {
                            RemoveAlreadyPresentPosts(ref SearchTagHandler.ListTagDetails);
                            SearchTagHandler.ListTagDetails.RemoveAll(x => CheckForDuplicatePost(x.Id));
                        }
                        foreach (var tag in SearchTagHandler.ListTagDetails)
                        {
                            if (!string.IsNullOrEmpty(tag.Id))
                            {
                                if (currentQueryPosts <= count)
                                {
                                    SaveScrapedPost(tag);
                                    ++currentQueryPosts;
                                    _publisherInitialize.UpdatePostCounts(campaignId);
                                }
                                else
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { ex.DebugLog(ex.Message); }
        }

        private void SaveScrapedPost(TagDetails tag)
        {
            try
            {
                var posListModel = new PublisherPostlistModel();
                posListModel.CreatedTime = DateTime.Now;
                posListModel.ExpiredTime = DateTime.Now.AddYears(2);
                posListModel.PostSource = PostSource.SharePost;
                posListModel.PostQueuedStatus = PostQueuedStatus.Pending;
                posListModel.CampaignId = campaignId == null ? this.campaignId : campaignId;
                posListModel.FetchedPostIdOrUrl = tag.Id;
                posListModel.ShareUrl = $"{TdConstants.MainUrl}{tag.Username}/status/{tag.Id}";
                posListModel.PostDescription = tag.Caption;
                PostlistFileManager.Add(posListModel.CampaignId, posListModel);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void ScrapePostLogin(string accountId)
        {
            domAccModel = _accountsFileManager.GetAccountById(accountId);
            twitterFunction = _accountScopeFactory[accountId].Resolve<ITwitterFunctions>();
            if (!domAccModel.IsUserLoggedIn)
            {
                var logInProcess = _accountScopeFactory[accountId].Resolve<ITwtLogInProcess>();
                logInProcess.LoginWithBrowserMethod(domAccModel, domAccModel.Token);
            }
        }

        public override void ScrapePosts(string accountId, string campaignId, ScrapePostModel scrapePostDetails,
            CancellationTokenSource cancellationTokenSource, int count)
        {
            var LstTagPostOlderThanXXDays = new List<TagDetails>();
            if (string.IsNullOrEmpty(accountId) || string.IsNullOrEmpty(campaignId) || scrapePostDetails == null)
                return;

            try
            {
                ScrapePostLogin(accountId);
                this.campaignId = campaignId;
                if (scrapePostDetails.IsScrapeTwitterPost &&
                    !string.IsNullOrEmpty(scrapePostDetails.AddTdPostSource.Trim()))
                {
                    var LstQueries = new List<string>(scrapePostDetails.AddTdPostSource.Split(','));
                    LstQueries.ForEach(query =>
                    {
                        if (!ListSearchKeyWordWithPagination.Keys.Contains(query))
                            ListSearchKeyWordWithPagination.Add(query, "");
                    });

                    #region getting posts 

                    lock (LockScrapPosts)
                    {
                        foreach (var query in LstQueries)
                        {
                            var campaignCancellationToken = cancellationTokenSource.Token;

                            var currentQueryPosts = 1;
                            #region search part
                            var queryType = query.Contains("[U]") ?
                                EnumUtility.GetQueryFromEnum(TdTweetInteractionQueryEnum.SpecificUserTweets) :
                                query.Contains("[P]") ? EnumUtility.GetQueryFromEnum(TdTweetInteractionQueryEnum.CustomTweetsList)
                                : EnumUtility.GetQueryFromEnum(TdTweetInteractionQueryEnum.Keywords);
                            var queryValue = query.Replace("[S]", "")?.Replace("[U]","")?.Replace("[P]","").Trim();
                            var minPosition = "";
                            var HasMoreResult = true;
                            while (currentQueryPosts <= count && HasMoreResult)
                            {
                                if (queryType == "Custom Tweet Lists")
                                {
                                    var Tweetdetails = twitterFunction.GetSingleTweetDetails(domAccModel, queryValue);
                                    if (Tweetdetails == null)
                                        break;
                                    FilterAndSaveScrappedPost(Tweetdetails != null ? Tweetdetails.singleTweetsDetails : LstTagPostOlderThanXXDays, scrapePostDetails, LstTagPostOlderThanXXDays, currentQueryPosts, count, query);
                                    HasMoreResult = false;
                                }
                                else
                                {
                                    var SearchTagHandler = twitterFunction.SearchForTag(domAccModel, queryValue,
                                        queryType, campaignCancellationToken, minPosition);
                                    if (SearchTagHandler == null)
                                        break;
                                    if (SearchTagHandler.HasMore)
                                        minPosition = SearchTagHandler.MinPosition;
                                    else minPosition = null;
                                    HasMoreResult = !string.IsNullOrEmpty(minPosition);
                                    FilterAndSaveScrappedPost(SearchTagHandler != null ? SearchTagHandler.ListTagDetails : LstTagPostOlderThanXXDays, scrapePostDetails, LstTagPostOlderThanXXDays, currentQueryPosts, count, query);
                                }
                            }
                            

                            #endregion
                        }
                    }

                    #endregion
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

        private void FilterAndSaveScrappedPost(List<TagDetails> ListTagDetails,
            ScrapePostModel scrapePostDetails, List<TagDetails> LstTagPostOlderThanXXDays,
            int currentQueryPosts,int count,string query)
        {
            try
            {
                RemoveAlreadyPresentPosts(ref ListTagDetails);
                if (scrapePostDetails.IsScrapePostOlderThanXXDays)
                    CheckpostTimewithXXDays(scrapePostDetails, ListTagDetails,
                        LstTagPostOlderThanXXDays);
                else
                    LstTagPostOlderThanXXDays = ListTagDetails;
                if (ListTagDetails.Count > 0)
                    ListTagDetails.RemoveAll(x => CheckForDuplicatePost(x.Id));
                foreach (var tag in LstTagPostOlderThanXXDays)
                {
                    var FilePath = new List<string>();
                    if (currentQueryPosts <= count)
                    {
                        if (!(scrapePostDetails.IgnoreTextOnlyPosts &&
                              string.IsNullOrEmpty(tag.Code)))
                        {
                            SavePublishPosts(tag, FilePath, query);
                            ++currentQueryPosts;
                            _publisherInitialize.UpdatePostCounts(campaignId);
                        }
                    }
                    else
                        break;
                }
            }
            catch(Exception ex) { ex.DebugLog(); }
        }

        private static void CheckpostTimewithXXDays(ScrapePostModel scrapePostDetails,
            List<TagDetails> ListTagDetails, List<TagDetails> LstTagPostOlderThanXXDays,
            SingleTweetDetailsResponseHandler SingleTweetDetails = null)
        {
            try
            {
                var currentDateTime = DateTime.Parse(DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss"));
                currentDateTime =
                    currentDateTime.Subtract(
                        TimeSpan.FromDays(scrapePostDetails.DoNotScrapePostOlderThanNDays.StartValue));

                var CurrentDateTimeSpan =
                    Convert.ToInt64(Math.Floor((currentDateTime - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime())
                        .TotalSeconds));
                var currentTimeStamp = TimeSpan.FromTicks(CurrentDateTimeSpan);
                if (ListTagDetails != null)
                    foreach (var items in ListTagDetails)
                    {
                        var postTime = TimeSpan.FromTicks(items.TweetedTimeStamp);
                        var dateCompare = TimeSpan.Compare(postTime, currentTimeStamp);
                        if (dateCompare >= 0)
                            LstTagPostOlderThanXXDays.Add(items);
                    }
                else
                    foreach (var items in SingleTweetDetails.singleTweetsDetails)
                    {
                        var postTime = TimeSpan.FromTicks(items.TweetedTimeStamp);
                        var dateCompare = TimeSpan.Compare(postTime, currentTimeStamp);
                        if (dateCompare >= 0)
                            LstTagPostOlderThanXXDays.Add(items);
                    }
            }
            catch (Exception ex)
            {
            }
        }

        private void SavePublishPosts(TagDetails tag, List<string> FilePath,
            string searchUrlTag = null, string campaignId = null)
        {
            try
            {
                var posListModel = new PublisherPostlistModel();
                posListModel.CreatedTime = DateTime.Now;
                posListModel.ExpiredTime = DateTime.Now.AddYears(2);
                posListModel.PostSource = PostSource.ScrapedPost;
                posListModel.PostQueuedStatus = PostQueuedStatus.Pending;
                posListModel.CampaignId = campaignId == null ? this.campaignId : campaignId;
                if (FilePath != null && FilePath.Count > 0)
                    posListModel.MediaList = new ObservableCollection<string>(FilePath);
                else if (!string.IsNullOrEmpty(tag.Code))
                    posListModel.MediaList = new ObservableCollection<string>(tag.Code.Split('\n'));
                posListModel.FetchedPostIdOrUrl = tag.Id;
                posListModel.PdSourceUrl = searchUrlTag;
                posListModel.PostDescription = tag.Caption;
                GlobusLogHelper.log.Info(Log.CustomMessage, domAccModel.AccountBaseModel.AccountNetwork,
                    domAccModel.UserName, ActivityType.PostScraper, $"Scraped Post {tag.Id}");

                PostlistFileManager.Add(posListModel.CampaignId, posListModel);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void RemoveAlreadyPresentPosts(ref List<TagDetails> ListTagDetails)
        {
            try
            {
                var publisherPostlistModel = PostlistFileManager.GetAll(campaignId);
                ListTagDetails.RemoveAll(x => publisherPostlistModel.Any(y => y.FetchedPostIdOrUrl == x.Id));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}