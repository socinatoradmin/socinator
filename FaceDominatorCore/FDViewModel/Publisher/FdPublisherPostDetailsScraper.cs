using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.SocioPublisher;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDLibrary.FdFunctions.FdBrowserManager;
using FaceDominatorCore.FDLibrary.FdProcesses;
using FaceDominatorCore.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Unity;

namespace FaceDominatorCore.FDViewModel.Publisher
{
    public class FdPublisherPostDetailsScraper : PostScraper
    {
        private readonly IAccountScopeFactory _accountScopeFactory;

        private static object _postSaveLock = new object();

        private readonly SoftwareSettingsModel _softwareSettingsModel;

        private readonly IGenericFileManager _genericFileManager;

        private static List<KeyValuePair<string, string>> _postCampaignList { get; set; }

        private bool SharePostActive { get; set; }

        private bool ScrapePostActive { get; set; }

        public FdPublisherPostDetailsScraper(string CampaignId, CancellationTokenSource campaignCancellationToken, PublisherPostFetchModel postFetchModel) :
            base(CampaignId, campaignCancellationToken, postFetchModel)
        {
            _accountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();
            _softwareSettingsModel = InstanceProvider.GetInstance<ISoftwareSettingsFileManager>().GetSoftwareSettings();
            _postCampaignList = _postCampaignList == null ? new List<KeyValuePair<string, string>>() : _postCampaignList;
            _genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
        }

        public override void ScrapeFdPagePostUrl(string accountId, string campaignId, SharePostModel fdPagePostUrlScraperDetails, CancellationTokenSource tokenSource, int count = 10)
        {
            try
            {

                if (SharePostActive)
                    return;

                if (string.IsNullOrEmpty(accountId) || string.IsNullOrEmpty(campaignId) || fdPagePostUrlScraperDetails == null)
                    return;

                SharePostActive = true;

                int totalcount = 0;

                count = fdPagePostUrlScraperDetails.MaxPost == 0 ? 25 : fdPagePostUrlScraperDetails.MaxPost;

                int maxDaysOld = fdPagePostUrlScraperDetails.MinimumDays;

                var filterText = fdPagePostUrlScraperDetails.AddKeywords;

                var splitFilterText = filterText.Split(',').ToArray();

                var likersCount = fdPagePostUrlScraperDetails.PostBetween;

                var accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();

                var account = accountsFileManager.GetAccountById(accountId);

                var loginProcess = _accountScopeFactory[$"{account.AccountId}_{campaignId}"].Resolve<IFdLoginProcess>();

                if (account == null)
                    return;

                var islogin = false;

                IFdLoginProcess fdLoginProcess = _accountScopeFactory[$"{accountId}_{campaignId}"].Resolve<IFdLoginProcess>();

                var fdRequestLibrary = _accountScopeFactory[$"{account.AccountId}_{campaignId}"]
                    .Resolve<IFdRequestLibrary>();

                tokenSource.Token.ThrowIfCancellationRequested();

                if (!account.IsRunProcessThroughBrowser)
                    islogin = fdLoginProcess.CheckLoginPostScrapperAsync(account, account.Token, true).Result;
                else
                {
                    _accountScopeFactory[$"{accountId}_{campaignId}"].Resolve<IFdLoginProcess>().
                        LoginWithBrowserMethod(account, tokenSource.Token);

                    islogin = account.IsUserLoggedIn;
                }

                if (islogin)
                {
                    tokenSource.Token.ThrowIfCancellationRequested();

                    if (account.IsRunProcessThroughBrowser)
                        fdLoginProcess._browserManager.SearchPostsByPageUrl(account, FbEntityType.Fanpage, fdPagePostUrlScraperDetails.AddFdPageUrl);

                    IResponseHandler objScrapPostListFromFanpageResponseHandler = null;

                    while (totalcount <= count && (objScrapPostListFromFanpageResponseHandler == null ||
                                objScrapPostListFromFanpageResponseHandler.HasMoreResults))
                    {

                        try
                        {

                            objScrapPostListFromFanpageResponseHandler = !account.IsRunProcessThroughBrowser
                                ? fdRequestLibrary.GetPostListFromFanpagesNew(account, objScrapPostListFromFanpageResponseHandler, fdPagePostUrlScraperDetails.AddFdPageUrl)
                                : fdLoginProcess._browserManager.ScrollWindowAndGetDataForPost(account, FbEntityType.Fanpage, 7, 0);

                            foreach (var post in objScrapPostListFromFanpageResponseHandler.ObjFdScraperResponseParameters.ListPostDetails)
                            {
                                try
                                {
                                    if (CheckForDuplicatePost(post.Id))
                                        continue;

                                    tokenSource.Token.ThrowIfCancellationRequested();

                                    if (fdPagePostUrlScraperDetails.IsExtractMinimumPost)
                                        if (totalcount > count)
                                            break;

                                    if (fdPagePostUrlScraperDetails.IsMinimumDays)
                                        if ((DateTime.Now - post.PostedDateTime).Days > maxDaysOld)
                                        {
                                            objScrapPostListFromFanpageResponseHandler.HasMoreResults = false;
                                            continue;
                                        }

                                    if (fdPagePostUrlScraperDetails.IsPostBetween)
                                        if (Int32.Parse(post.LikersCount) < likersCount.StartValue || Int32.Parse(post.LikersCount) > likersCount.EndValue)
                                            continue;

                                    #region Complete post details scraping commented
                                    //if (!account.IsRunProcessThroughBrowser)
                                    //    fdRequestLibrary.GetPostDetails(account, post);
                                    //else
                                    //    UpdatePostDetails(account, post); 
                                    #endregion

                                    tokenSource.Token.ThrowIfCancellationRequested();

                                    try
                                    {
                                        if (!splitFilterText.Any(x => post.Caption.ToLower().Contains(x.ToLower()) || post.SubDescription.ToLower().Contains(x.ToLower())))
                                            continue;
                                    }
                                    catch (Exception)
                                    { }

                                    SaveSharePostDetails(campaignId, post, tokenSource);

                                    totalcount++;

                                    _publisherInitialize.UpdatePostCounts(campaignId);
                                }
                                catch (OperationCanceledException)
                                {
                                    fdLoginProcess._browserManager.CloseBrowser(account);
                                    throw new OperationCanceledException("Requested Cancelled !");
                                }
                                catch (ArgumentException e)
                                {
                                    e.DebugLog(e.Message);
                                }
                                catch (Exception ex)
                                {
                                    ex.DebugLog(ex.Message);
                                }
                            }
                        }
                        catch (OperationCanceledException)
                        {
                            fdLoginProcess._browserManager.CloseBrowser(account);
                            throw new OperationCanceledException("Requested Cancelled !");
                        }
                        catch (ArgumentException e)
                        {
                            e.DebugLog(e.Message);
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog(ex.Message);
                        }
                    }
                }

                loginProcess._browserManager.CloseBrowser(account);
            }
            catch (Exception ex)
            {
                ex.DebugLog(ex.Message);
            }
            _accountScopeFactory[$"{accountId}_{campaignId}"].Resolve<IFdLoginProcess>()._browserManager.CloseBrowser(new DominatorAccountModel());
            SharePostActive = false;
        }

        public override void ScrapeFdKeywords(string accountId, string campaignId,
            SharePostModel fdKeywordScraperDetails, CancellationTokenSource cancellationTokenSource, int count = 10)
        {
            try
            {
                if (SharePostActive)
                    return;

                if (string.IsNullOrEmpty(accountId) || string.IsNullOrEmpty(campaignId) || fdKeywordScraperDetails == null)
                    return;

                SharePostActive = true;

                int totalcount = 0;

                count = fdKeywordScraperDetails.MaxPost == 0 ? 25 : fdKeywordScraperDetails.MaxPost;

                int maxDaysOld = fdKeywordScraperDetails.MinimumDays;

                var filterText = fdKeywordScraperDetails.AddKeywords;

                var splitFilterText = filterText.Split(',').ToArray();

                var likersCount = fdKeywordScraperDetails.PostBetween;

                var accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();

                var account = accountsFileManager.GetAccountById(accountId);

                var loginProcess = _accountScopeFactory[$"{account.AccountId}_{campaignId}"].Resolve<IFdLoginProcess>();

                if (account == null)
                    return;

                var islogin = false;

                IFdLoginProcess fdLoginProcess = _accountScopeFactory[$"{accountId}_{campaignId}"].Resolve<IFdLoginProcess>();

                var fdRequestLibrary = _accountScopeFactory[$"{account.AccountId}_{campaignId}"]
                    .Resolve<IFdRequestLibrary>();

                cancellationTokenSource.Token.ThrowIfCancellationRequested();

                if (!account.IsRunProcessThroughBrowser)
                    islogin = fdLoginProcess.CheckLoginPostScrapperAsync(account, account.Token, true).Result;
                else
                {
                    _accountScopeFactory[$"{accountId}_{campaignId}"].Resolve<IFdLoginProcess>().
                        LoginWithBrowserMethod(account, cancellationTokenSource.Token);

                    islogin = account.IsUserLoggedIn;
                }

                if (islogin)
                {
                    cancellationTokenSource.Token.ThrowIfCancellationRequested();

                    if (account.IsRunProcessThroughBrowser)
                        fdLoginProcess._browserManager.SearchByKeywordOrHashTag(account, SearchKeywordType.Posts, filterText);

                    IResponseHandler objScrapPostListFromKeywordResponseHandler = null;

                    while (totalcount <= count && (objScrapPostListFromKeywordResponseHandler == null ||
                                objScrapPostListFromKeywordResponseHandler.HasMoreResults))
                    {
                        try
                        {
                            if (account.IsRunProcessThroughBrowser)
                                objScrapPostListFromKeywordResponseHandler =
                                    fdLoginProcess._browserManager.ScrollWindowAndGetDataForPost(account, FbEntityType.Post, 5, 0);

                            foreach (var post in objScrapPostListFromKeywordResponseHandler.ObjFdScraperResponseParameters.ListPostDetails)
                            {
                                try
                                {
                                    if (CheckForDuplicatePost(post.Id))
                                        continue;

                                    cancellationTokenSource.Token.ThrowIfCancellationRequested();

                                    if (fdKeywordScraperDetails.IsExtractMinimumPost)
                                        if (totalcount > count)
                                            break;

                                    if (fdKeywordScraperDetails.IsMinimumDays)
                                        if ((DateTime.Now - post.PostedDateTime).Days > maxDaysOld)
                                        {
                                            objScrapPostListFromKeywordResponseHandler.HasMoreResults = false;
                                            continue;
                                        }

                                    if (fdKeywordScraperDetails.IsPostBetween)
                                        if (Int32.Parse(post.LikersCount) < likersCount.StartValue || Int32.Parse(post.LikersCount) > likersCount.EndValue)
                                            continue;

                                    cancellationTokenSource.Token.ThrowIfCancellationRequested();

                                    try
                                    {
                                        if (!splitFilterText.Any(x => post.Caption.ToLower().Contains(x.ToLower()) || post.SubDescription.ToLower().Contains(x.ToLower())))
                                            continue;
                                    }
                                    catch (Exception)
                                    { }

                                    SaveSharePostDetails(campaignId, post, cancellationTokenSource);

                                    totalcount++;

                                    _publisherInitialize.UpdatePostCounts(campaignId);

                                }
                                catch (OperationCanceledException)
                                {
                                    fdLoginProcess._browserManager.CloseBrowser(account);
                                    throw new OperationCanceledException("Requested Cancelled !");
                                }
                                catch (ArgumentException e)
                                {
                                    e.DebugLog(e.Message);
                                }
                                catch (Exception ex)
                                {
                                    ex.DebugLog(ex.Message);
                                }
                            }
                        }
                        catch (OperationCanceledException)
                        {
                            fdLoginProcess._browserManager.CloseBrowser(account);
                            throw new OperationCanceledException("Requested Cancelled !");
                        }
                        catch (ArgumentException e)
                        {
                            e.DebugLog(e.Message);
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog(ex.Message);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                ex.DebugLog(ex.Message);
            }
            _accountScopeFactory[$"{accountId}_{campaignId}"].Resolve<IFdLoginProcess>()._browserManager.CloseBrowser(new DominatorAccountModel());
            SharePostActive = false;
        }

        public override void ScrapePosts(string accountId, string campaignId, ScrapePostModel scrapePostDetails, CancellationTokenSource tokenSource, int count = 10)
        {
            try
            {
                if (ScrapePostActive)
                    return;

                ScrapePostActive = true;

                if (string.IsNullOrEmpty(accountId) || string.IsNullOrEmpty(campaignId) || scrapePostDetails == null)
                    return;

                var accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();

                var postSource = Regex.Split(scrapePostDetails.AddFdPostSource, ",");

                if (postSource.Count() <= 1 && scrapePostDetails.AddFdPostSource.Contains("\r\n"))
                    postSource = Regex.Split(scrapePostDetails.AddFdPostSource, "\r\n");

                var account = accountsFileManager.GetAccountById(accountId);

                if (account == null)
                    return;

                tokenSource.Token.ThrowIfCancellationRequested();

                var islogin = false;

                if (!account.IsRunProcessThroughBrowser)
                    islogin = _accountScopeFactory[$"{accountId}_{campaignId}"].Resolve<IFdLoginProcess>().CheckLoginPostScrapperAsync(account, account.Token, true).Result;
                else
                {
                    _accountScopeFactory[$"{accountId}_{campaignId}"].Resolve<IFdLoginProcess>().
                        LoginWithBrowserMethod(account, tokenSource.Token);

                    islogin = account.IsUserLoggedIn;
                }

                if (islogin)
                {
                    tokenSource.Token.ThrowIfCancellationRequested();

                    postSource.Shuffle();

                    var finalPostSource = new List<string>();

                    foreach (string source in postSource)
                    {

                        var urls = Regex.Split(source, "\\[").ToList();
                        urls.RemoveAll(x => string.IsNullOrEmpty(x));

                        if (urls.Count >= 1)
                            finalPostSource.AddRange(urls.Select(x => x.Replace(x, $"[{x}")));
                    }


                    foreach (var source in finalPostSource)
                    {
                        int totalcount;
                        if (source.Contains("[G]") || source.Contains("[g]"))
                        {
                            totalcount = 0;
                            ScrapPostFromGroups(source, ref totalcount, count, campaignId, account, tokenSource, scrapePostDetails);
                        }

                        else if (source.Contains("[P]") || source.Contains("[p]"))
                        {
                            totalcount = 0;
                            ScrapPostFromPages(source, ref totalcount, count, campaignId, account, tokenSource, scrapePostDetails);
                        }
                        else if (source.Contains("[N]") || source.Contains("[n]"))
                        {
                            totalcount = 0;
                            ScrapPostFromNewsFeed(ref totalcount, count, campaignId, account, tokenSource, scrapePostDetails);
                        }
                        else if (source.Contains("[F]") || source.Contains("[f]"))
                        {
                            totalcount = 0;
                            ScrapPostFromWall(source, ref totalcount, count, campaignId, account, tokenSource, scrapePostDetails);
                        }
                        else if (source.Contains("[O]") || source.Contains("[o]"))
                        {
                            totalcount = 0;
                            ScrapPostFromWall(source, ref totalcount, count, campaignId, account, tokenSource, scrapePostDetails);
                        }
                    }
                }

                _accountScopeFactory[$"{accountId}_{campaignId}"].Resolve<IFdLoginProcess>()._browserManager.CloseBrowser(account);
            }
            catch (OperationCanceledException)
            {
                _accountScopeFactory[$"{accountId}_{campaignId}"].Resolve<IFdLoginProcess>()._browserManager.CloseBrowser(new DominatorAccountModel());
                ScrapePostActive = false;
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            _accountScopeFactory[$"{accountId}_{campaignId}"].Resolve<IFdLoginProcess>()._browserManager.CloseBrowser(new DominatorAccountModel());
            ScrapePostActive = false;

        }

        private void ScrapPostFromWall(string source, ref int totalcount, int count, string campaignId, DominatorAccountModel account, CancellationTokenSource tokenSource, ScrapePostModel scrapePostDetails)
        {
            var loginProcess = _accountScopeFactory[$"{account.AccountId}_{campaignId}"].Resolve<IFdLoginProcess>();

            try
            {
                IResponseHandler objScrapPostListFromTimelineResponseHandler = null;

                tokenSource.Token.ThrowIfCancellationRequested();

                var fdRequestLibrary = _accountScopeFactory[$"{account.AccountId}_{campaignId}"].Resolve<IFdRequestLibrary>();

                var friendSource = Regex.Split(source, "]");

                if (account.IsRunProcessThroughBrowser)
                {
                    var entityUrl = string.IsNullOrEmpty(friendSource[1]) ? $"{FdConstants.FbHomeUrl}{account.AccountBaseModel.UserId}" : friendSource[1];
                    loginProcess._browserManager.LoadPageSource(account, !FdFunctions.IsIntegerOnly(entityUrl) ? entityUrl : FdConstants.FbHomeUrl + entityUrl, true);
                }

                while (totalcount <= count)
                {
                    tokenSource.Token.ThrowIfCancellationRequested();

                    try
                    {

                        if (account.IsRunProcessThroughBrowser)
                            objScrapPostListFromTimelineResponseHandler = loginProcess._browserManager.ScrollWindowAndGetDataForPost
                                           (account, FbEntityType.Friends, 10, 0);

                        else if (string.Compare("[F]", source, StringComparison.CurrentCultureIgnoreCase) == 0)
                        {
                            objScrapPostListFromTimelineResponseHandler = fdRequestLibrary.GetPostListFromFriendTimelineNew
                                (account, objScrapPostListFromTimelineResponseHandler, friendSource[1]);
                        }
                        else if (string.Compare("[O]", source, StringComparison.CurrentCultureIgnoreCase) == 0)
                        {
                            objScrapPostListFromTimelineResponseHandler = fdRequestLibrary.GetPostListFromFriendTimelineNew
                                (account, objScrapPostListFromTimelineResponseHandler, account.AccountBaseModel.UserId);
                        }


                        foreach (var post in objScrapPostListFromTimelineResponseHandler.ObjFdScraperResponseParameters
                            .ListPostDetails)
                        {
                            tokenSource.Token.ThrowIfCancellationRequested();

                            try
                            {
                                if (scrapePostDetails.IsScrapePostOlderThanXXDays)
                                    if ((DateTime.Now - post.PostedDateTime).Days > scrapePostDetails.DoNotScrapePostOlderThanNDays.StartValue)
                                        continue;

                                if (CheckForDuplicatePost(post.Id))
                                    continue;

                                if (!account.IsRunProcessThroughBrowser)
                                    fdRequestLibrary.GetPostDetails(account, post);
                                else
                                    UpdatePostDetails(account, post);

                                tokenSource.Token.ThrowIfCancellationRequested();

                                SaveFacebookPostModel(campaignId, post, tokenSource);

                                totalcount++;

                                _publisherInitialize.UpdatePostCounts(campaignId);

                            }
                            catch (OperationCanceledException)
                            {
                                throw new OperationCanceledException("Requested Cancelled !");
                            }
                            catch (Exception ex)
                            {
                                ex.DebugLog(ex.Message);
                            }
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        loginProcess._browserManager.CloseBrowser(account);
                        throw new OperationCanceledException("Requested Cancelled !");
                    }
                    catch (Exception e)
                    {
                        e.DebugLog();
                    }

                    if (objScrapPostListFromTimelineResponseHandler == null ||
                           !objScrapPostListFromTimelineResponseHandler.HasMoreResults)
                        break;
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

        private void ScrapPostFromNewsFeed(ref int totalcount, int count, string campaignId, DominatorAccountModel account, CancellationTokenSource tokenSource, ScrapePostModel scrapePostDetails)
        {
            var loginProcess = _accountScopeFactory[$"{account.AccountId}_{campaignId}"].Resolve<IFdLoginProcess>();

            try
            {
                IResponseHandler objScrapPostListFromNewsFeedResponseHandler = null;


                var fdRequestLibrary = _accountScopeFactory[$"{account.AccountId}_{campaignId}"].Resolve<IFdRequestLibrary>();


                while (totalcount <= count)
                {
                    tokenSource.Token.ThrowIfCancellationRequested();

                    try
                    {

                        objScrapPostListFromNewsFeedResponseHandler = !account.IsRunProcessThroughBrowser
                            ? fdRequestLibrary.GetPostListFromNewsFeed(account, objScrapPostListFromNewsFeedResponseHandler)
                            : loginProcess._browserManager.ScrollWindowAndGetDataForPost(account, FbEntityType.NewFeedPost, 7, 0);

                        if (!objScrapPostListFromNewsFeedResponseHandler.HasMoreResults && objScrapPostListFromNewsFeedResponseHandler.ObjFdScraperResponseParameters.ListPostDetails.Count == 0)
                            break;


                        foreach (var post in objScrapPostListFromNewsFeedResponseHandler.ObjFdScraperResponseParameters.ListPostDetails)
                        {
                            tokenSource.Token.ThrowIfCancellationRequested();

                            try
                            {

                                if (scrapePostDetails.IsScrapePostOlderThanXXDays)
                                    if ((DateTime.Now - post.PostedDateTime).Days > scrapePostDetails.DoNotScrapePostOlderThanNDays.StartValue)
                                        continue;

                                if (string.IsNullOrEmpty(post.Id) || CheckForDuplicatePost(post.Id))
                                    continue;

                                if (totalcount >= count)
                                    return;

                                if (!account.IsRunProcessThroughBrowser)
                                    fdRequestLibrary.GetPostDetails(account, post);
                                else
                                    UpdatePostDetails(account, post);

                                tokenSource.Token.ThrowIfCancellationRequested();

                                SaveFacebookPostModel(campaignId, post, tokenSource);

                                totalcount++;

                                _publisherInitialize.UpdatePostCounts(campaignId);

                                Task.Delay(TimeSpan.FromSeconds(5)).Wait(tokenSource.Token);
                            }
                            catch (OperationCanceledException)
                            {
                                throw new OperationCanceledException("Requested Cancelled !");
                            }
                            catch (Exception ex)
                            {
                                ex.DebugLog(ex.Message);
                            }
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        throw new OperationCanceledException("Requested Cancelled !");
                    }
                    catch (ArgumentException e)
                    {
                        e.DebugLog(e.Message);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog(ex.Message);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                loginProcess._browserManager.CloseBrowser(account);
                throw new OperationCanceledException("Requested Cancelled !");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void ScrapPostFromPages(string source, ref int totalcount, int count, string campaignId, DominatorAccountModel account, CancellationTokenSource tokenSource, ScrapePostModel scrapePostDetails)
        {
            var loginProcess = _accountScopeFactory[$"{account.AccountId}_{campaignId}"].Resolve<IFdLoginProcess>();

            try
            {
                IResponseHandler objScrapPostListFromFanpageResponseHandler = null;


                var fdRequestLibrary = _accountScopeFactory[$"{account.AccountId}_{campaignId}"]
                    .Resolve<IFdRequestLibrary>();

                var groupSource = Regex.Split(source, "]");

                tokenSource.Token.ThrowIfCancellationRequested();

                if (account.IsRunProcessThroughBrowser)
                    loginProcess._browserManager.SearchPostsByPageUrl(account, FbEntityType.Fanpage, groupSource[1]);

                while (totalcount < count)
                {
                    tokenSource.Token.ThrowIfCancellationRequested();

                    try
                    {
                        objScrapPostListFromFanpageResponseHandler = !account.IsRunProcessThroughBrowser
                            ? fdRequestLibrary.GetPostListFromFanpagesNew(account, objScrapPostListFromFanpageResponseHandler, groupSource[1])
                            : loginProcess._browserManager.ScrollWindowAndGetDataForPost(account, FbEntityType.Fanpage, 10, 0);

                        if (objScrapPostListFromFanpageResponseHandler.ObjFdScraperResponseParameters.ListPostDetails
                                .Count == 0)
                            break;

                        foreach (var post in objScrapPostListFromFanpageResponseHandler.ObjFdScraperResponseParameters
                            .ListPostDetails)
                        {

                            if (scrapePostDetails.IsScrapePostOlderThanXXDays)
                                if ((DateTime.Now - post.PostedDateTime).Days > scrapePostDetails.DoNotScrapePostOlderThanNDays.StartValue)
                                    continue;


                            if (string.IsNullOrEmpty(post.Id) || CheckForDuplicatePost(post.Id))
                                continue;

                            tokenSource.Token.ThrowIfCancellationRequested();

                            try
                            {

                                if (totalcount >= count)
                                    break;

                                if (!account.IsRunProcessThroughBrowser)
                                    fdRequestLibrary.GetPostDetails(account, post);
                                else
                                    UpdatePostDetails(account, post);

                                tokenSource.Token.ThrowIfCancellationRequested();

                                SaveFacebookPostModel(campaignId, post, tokenSource);

                                Thread.Sleep(2000);

                                totalcount++;

                                _publisherInitialize.UpdatePostCounts(campaignId);
                            }
                            catch (OperationCanceledException)
                            {
                                throw new OperationCanceledException("Requested Cancelled !");
                            }
                            catch (ArgumentException e)
                            {
                                e.DebugLog(e.Message);
                            }
                            catch (Exception ex)
                            {
                                ex.DebugLog(ex.Message);
                            }
                        }

                        if (objScrapPostListFromFanpageResponseHandler == null ||
                           !objScrapPostListFromFanpageResponseHandler.HasMoreResults)
                            break;
                    }
                    catch (OperationCanceledException)
                    {
                        throw new OperationCanceledException("Requested Cancelled !");
                    }
                    catch (ArgumentException e)
                    {
                        e.DebugLog(e.Message);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog(ex.Message);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                loginProcess._browserManager.CloseBrowser(account);
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void ScrapPostFromGroups(string source, ref int totalcount, int count, string campaignId, DominatorAccountModel account, CancellationTokenSource tokenSource, ScrapePostModel scrapePostDetails)
        {
            var loginProcess = _accountScopeFactory[$"{account.AccountId}_{campaignId}"].Resolve<IFdLoginProcess>();

            try
            {

                var fdRequestLibrary = _accountScopeFactory[$"{account.AccountId}_{campaignId}"].Resolve<IFdRequestLibrary>();

                IResponseHandler objScrapGroupPostListResponseHandler = null;

                var groupSource = Regex.Split(source, "]");

                if (account.IsRunProcessThroughBrowser)
                    loginProcess._browserManager.GetFullGroupDetails(account, groupSource[1], false);

                while (totalcount < count)
                {
                    tokenSource.Token.ThrowIfCancellationRequested();

                    try
                    {
                        objScrapGroupPostListResponseHandler = !account.IsRunProcessThroughBrowser
                            ? fdRequestLibrary.GetPostListFromGroupsNew(account, objScrapGroupPostListResponseHandler, groupSource[1])
                            : loginProcess._browserManager.ScrollWindowAndGetDataForPost(account, FbEntityType.Groups, 8, 0);

                        if (objScrapGroupPostListResponseHandler == null ||
                        objScrapGroupPostListResponseHandler.ObjFdScraperResponseParameters.ListPostDetails == null || objScrapGroupPostListResponseHandler.ObjFdScraperResponseParameters.ListPostDetails.Count == 0)
                            break;

                        foreach (var post in objScrapGroupPostListResponseHandler.ObjFdScraperResponseParameters.ListPostDetails)
                        {
                            if (scrapePostDetails.IsScrapePostOlderThanXXDays)
                                if ((DateTime.Now - post.PostedDateTime).Days > scrapePostDetails.DoNotScrapePostOlderThanNDays.StartValue)
                                    continue;

                            if (CheckForDuplicatePost(post.Id))
                                continue;

                            tokenSource.Token.ThrowIfCancellationRequested();

                            try
                            {
                                if (totalcount >= count)
                                    break;

                                if (!account.IsRunProcessThroughBrowser)
                                    fdRequestLibrary.GetPostDetails(account, post);
                                else
                                    UpdatePostDetails(account, post);

                                tokenSource.Token.ThrowIfCancellationRequested();

                                SaveFacebookPostModel(campaignId, post, tokenSource);

                                _publisherInitialize.UpdatePostCounts(campaignId);
                                totalcount++;
                            }
                            catch (OperationCanceledException)
                            {
                                throw new OperationCanceledException("Requested Cancelled !");
                            }
                            catch (ArgumentException e)
                            {
                                e.DebugLog(e.Message);
                            }
                            catch (Exception ex)
                            {
                                ex.DebugLog(ex.Message);
                            }
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        throw new OperationCanceledException("Requested Cancelled !");
                    }
                    catch (ArgumentException e)
                    {
                        e.DebugLog(e.Message);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog(ex.Message);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                loginProcess._browserManager.CloseBrowser(account);
                throw new OperationCanceledException("Requested Cancelled !");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private async Task SaveFacebookPostModel(string campaignId, FacebookPostDetails post,
            CancellationTokenSource fetchPostToken)
        {

            try
            {
                PublisherPostlistModel model = new PublisherPostlistModel();
                model.GenerateClonePostId();
                model.CampaignId = campaignId;
                model.ExpiredTime = DateTime.Now.AddYears(2);
                model.PostSource = PostSource.ScrapedPost;
                model.PostQueuedStatus = PostQueuedStatus.Pending;

                if (post.MediaType == MediaType.Video && (post.MediaUrl.Contains("https://") ||
                   post.MediaUrl.Contains("http://")))
                {
                    var filePath = FdConstants.DownloadFolderPath + @"\" + post.Id + ".mp4";
                    await FdFunctions.DownLoadMediaFromUrlAsync(post.MediaUrl, filePath, FdConstants.DownloadFolderPath);

                    model.MediaList.Add(filePath);
                }

                if ((string.IsNullOrEmpty(post.Caption) || post.Caption == "NA")
                    && (string.IsNullOrEmpty(post.MediaUrl) || post.MediaUrl == "NA")
                    && (string.IsNullOrEmpty(post.OtherMediaUrl) || post.OtherMediaUrl == "NA"))
                    return;

                model.PostDescription = !string.IsNullOrEmpty(post.Caption) ? post.Caption : string.Empty;

                if (post.MediaType != MediaType.Video)
                {
                    if (post.MediaUrl != "NA" && !string.IsNullOrEmpty(post.MediaUrl))
                    {
                        var filePath = FdConstants.DownloadFolderPath + @"\" + post.Id + ".jpg";
                        await FdFunctions.DownLoadMediaFromUrlAsync(post.MediaUrl, filePath, FdConstants.DownloadFolderPath);
                        model.MediaList.Add(filePath);
                    }
                    else if (post.OtherMediaUrl != "NA" && !string.IsNullOrEmpty(post.OtherMediaUrl))
                    {
                        var media = Regex.Split(post.OtherMediaUrl, ",");

                        var count = 0;

                        foreach (var eachMedia in media)
                        {
                            var newMedia = eachMedia.Replace("||", string.Empty);
                            if (newMedia.Contains("https://external") && newMedia.Contains("url="))
                            {
                                newMedia = Utilities.GetBetween(newMedia, "url=", "&");
                                newMedia = Uri.UnescapeDataString(newMedia);
                            }

                            var filePath = FdConstants.DownloadFolderPath + @"\" + $"{post.Id}_{count}" + ".jpg";
                            await FdFunctions.DownLoadMediaFromUrlAsync(post.MediaUrl, filePath, FdConstants.DownloadFolderPath);
                            model.MediaList.Add(filePath);
                            count++;
                        }
                    }
                }

                model.CampaignId = campaignId;

                if (model.PostDescription == "NA")
                {
                    model.PostDescription = string.Empty;
                }

                model.FetchedPostIdOrUrl = post.Id;

                var postList = PostlistFileManager.GetAll(campaignId);

                fetchPostToken.Token.ThrowIfCancellationRequested();

                lock (_postSaveLock)
                {
                    if (postList.FirstOrDefault(x => x.FetchedPostIdOrUrl == model.FetchedPostIdOrUrl) == null)
                        PostlistFileManager.Add(campaignId, model);
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Requested Cancelled !");
            }
            catch (ArgumentException e)
            {
                e.DebugLog(e.Message);
            }
            catch (Exception ex)
            {
                ex.DebugLog(ex.Message);
            }
        }

        private async Task SaveSharePostDetails(string campaignId, FacebookPostDetails post,
            CancellationTokenSource fetchPostToken)
        {

            try
            {
                PublisherPostlistModel model = new PublisherPostlistModel();
                model.GenerateClonePostId();
                model.CampaignId = campaignId;
                model.ExpiredTime = DateTime.Now.AddYears(2);
                model.PostSource = PostSource.SharePost;
                model.PostQueuedStatus = PostQueuedStatus.Pending;
                model.FetchedPostIdOrUrl = post.Id;
                model.PostDescription = post.Caption;

                if (post.MediaType == MediaType.Video && (post.MediaUrl.Contains("https://") ||
                   post.MediaUrl.Contains("http://")))
                {
                    var filePath = FdConstants.DownloadFolderPath + @"\" + post.Id + ".mp4";
                    await FdFunctions.DownLoadMediaFromUrlAsync(post.MediaUrl, filePath, FdConstants.DownloadFolderPath);

                    model.MediaList.Add(filePath);
                }

                if (post.MediaType != MediaType.Video)
                {
                    if (post.OtherMediaUrl == "NA" && post.MediaUrl != "NA" && !string.IsNullOrEmpty(post.MediaUrl))
                    {
                        var filePath = FdConstants.DownloadFolderPath + @"\" + post.Id + ".jpg";
                        await FdFunctions.DownLoadMediaFromUrlAsync(post.MediaUrl, filePath, FdConstants.DownloadFolderPath);
                        model.MediaList.Add(filePath);
                    }
                    else if (post.OtherMediaUrl != "NA" && !string.IsNullOrEmpty(post.OtherMediaUrl))
                    {
                        var media = Regex.Split(post.OtherMediaUrl, ",");

                        var count = 0;

                        foreach (var eachMedia in media)
                        {
                            var newMedia = eachMedia.Replace("||", string.Empty);
                            if (newMedia.Contains("https://external") && newMedia.Contains("url="))
                            {
                                newMedia = Utilities.GetBetween(newMedia, "url=", "&");
                                newMedia = Uri.UnescapeDataString(newMedia);
                            }

                            var filePath = FdConstants.DownloadFolderPath + @"\" + $"{post.Id}_{count}" + ".jpg";
                            await FdFunctions.DownLoadMediaFromUrlAsync(post.MediaUrl, filePath, FdConstants.DownloadFolderPath);
                            model.MediaList.Add(filePath);
                            count++;
                        }
                    }
                }

                if (model.PostDescription == "NA")
                    model.PostDescription = string.Empty;

                //model.ShareUrl = FdConstants.FbHomeUrl + post.Id;
                model.ShareUrl = $"{FdConstants.FbHomeUrl}{post.OwnerId}/posts/{post.Id}";

                fetchPostToken.Token.ThrowIfCancellationRequested();

                lock (_postSaveLock)
                {

                    var postList = PostlistFileManager.GetAll(campaignId);
                    if (postList.FirstOrDefault(x => x.ShareUrl == model.ShareUrl) == null)
                        PostlistFileManager.Add(campaignId, model);
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Requested Cancelled !");
            }
            catch (ArgumentException e)
            {
                e.DebugLog(e.Message);
            }
            catch (Exception ex)
            {
                ex.DebugLog(ex.Message);
            }

        }

        private void UpdatePostDetails(DominatorAccountModel account, FacebookPostDetails post)
        {
            var customBrowserWindow = _accountScopeFactory[$"{account.AccountId}_scraper_{post.Id}"].Resolve<IFdBrowserManager>();
            post = customBrowserWindow.GetFullPostDetails(account, post).ObjFdScraperResponseParameters.PostDetails;
            customBrowserWindow.CloseBrowser(account);
        }



        /*#region Old Save Share Post
        private bool SaveSharePostDetails(string campaignId, FacebookPostDetails post)
        {
            bool status = false;
            try
            {

                PublisherPostlistModel model = new PublisherPostlistModel();
                model.GenerateClonePostId();
                model.ExpiredTime = DateTime.Now.AddYears(2);
                model.PostSource = PostSource.ScrapedPost;
                model.PostQueuedStatus = PostQueuedStatus.Pending;

                model.PostDescription = post.Caption;

                if (post.OtherMediaUrl == "NA" && post.MediaUrl != "NA")
                {
                    model.MediaList.Add(post.MediaUrl);
                }
                else if (post.OtherMediaUrl != "NA")
                {
                    var media = Regex.Split(post.OtherMediaUrl, ",");


                    foreach (var eachMedia in media)
                    {
                        var newMedia = eachMedia.Replace("||", string.Empty);
                        model.MediaList.Add(eachMedia);
                    }
                }

                model.ShareUrl = FdConstants.FbHomeUrl + post.Id;

                status = PostlistFileManager.Add(campaignId, model);

            }
            catch (ArgumentException e)
            {
                e.DebugLog(e.Message);
            }
            catch (Exception ex)
            {
                ex.DebugLog(ex.Message);
            }
            return status;
        }

        #endregion*/

    }
}