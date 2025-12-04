using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums.SocioPublisher;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using PinDominatorCore.PDLibrary;
using PinDominatorCore.PDModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using DominatorHouseCore.Annotations;
using Unity;
using DominatorHouseCore.LogHelper;

namespace PinDominatorCore.PDViewModel.Publisher
{
    public class PdPublisherPostDetailsScraper : PostScraper
    {
        private IPinFunction PinFunct { get; set; }
        private IPdBrowserManager BrowserManager { get; set; }
        private string bookmark = string.Empty;
        public PdPublisherPostDetailsScraper(string campaignId
            , CancellationTokenSource campaignCancellationToken, PublisherPostFetchModel postFetchModel) :
            base(campaignId, campaignCancellationToken, postFetchModel)
        {

        }

        public override void ScrapePosts(string accountId, string campaignId, ScrapePostModel scrapePostModel,
            CancellationTokenSource cancellationTokenSource, int count = 10)
        {
            try
            {
                cancellationTokenSource.Token.ThrowIfCancellationRequested();
                var totalcount = 0;
                if (string.IsNullOrEmpty(accountId) || string.IsNullOrEmpty(campaignId) || scrapePostModel == null)
                    return;

                var accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
                var objDominatorAccountModel = accountsFileManager.GetAccountById(accountId);

                if (!LoginForScraper(objDominatorAccountModel, campaignId, cancellationTokenSource))
                    return;

                cancellationTokenSource.Token.ThrowIfCancellationRequested();
                var postSource = Regex.Split(scrapePostModel.AddPdPostSource, "\r\n");
                postSource.Shuffle();
                foreach (string source in postSource)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                            objDominatorAccountModel.AccountBaseModel.AccountNetwork,
                            objDominatorAccountModel.AccountBaseModel.UserName, "Scrap Post",
                            $"Started Scrapping Post(s) Of {source?.Split('/').Last(x => x.ToString() != string.Empty)}");
                    cancellationTokenSource.Token.ThrowIfCancellationRequested();
                    if (Utilities.GetBetween(source, "[", "]") == "K" || Utilities.GetBetween(source, "[", "]") == "k")
                    {
                        ScrapePinsByKeyword(accountId, campaignId, ref totalcount, source, scrapePostModel, cancellationTokenSource,
                            count, scrapePostModel.LstScrapedPostDetails.ToList());
                    }

                    if (Utilities.GetBetween(source, "[", "]") == "U" || Utilities.GetBetween(source, "[", "]") == "u")
                    {
                        ScrapePinsFromSpecificUser(accountId, campaignId, ref totalcount, source, scrapePostModel, cancellationTokenSource,
                            count, scrapePostModel.LstScrapedPostDetails.ToList());
                    }

                    if (Utilities.GetBetween(source, "[", "]") == "B" || Utilities.GetBetween(source, "[", "]") == "b")
                    {
                        ScrapePinsByBoardUrl(accountId, campaignId, ref totalcount, source, scrapePostModel, cancellationTokenSource,
                            count, scrapePostModel.LstScrapedPostDetails.ToList());
                    }
                }
                var publisherInitialize = PublisherInitialize.GetInstance;
                publisherInitialize.UpdatePostCounts(campaignId);
                publisherInitialize.UpdatePostStatus(campaignId);
                GlobusLogHelper.log.Info(Log.CustomMessage,
                            objDominatorAccountModel.AccountBaseModel.AccountNetwork,
                            objDominatorAccountModel.AccountBaseModel.UserName, "Scrap Post",
                            $"Successfully Scrapped {publisherInitialize.ListPublisherCampaignStatusModels.FirstOrDefault(x=>x.CampaignId==campaignId).PendingCount} Post(s).");
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            finally
            {
                if (BrowserManager != null && BrowserManager.BrowserWindows.Count > 0)
                    BrowserManager.CloseLast();
            }
        }

        public void ScrapePinsByKeyword(string accountId, string campaignId, ref int totalcount, string scrapePostDetails,
            ScrapePostModel scrapePostModel, CancellationTokenSource cancellationTokenSource, int count = 10, List<string> lstScrapePostDetails = null)
        {
            try
            {
                cancellationTokenSource.Token.ThrowIfCancellationRequested();
                var accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
                var objDominatorAccountModel = accountsFileManager.GetAccountById(accountId);

                var scrapPostLst = new List<string>(lstScrapePostDetails);

                var lstPinOriginal = new List<PinterestPin>();
                var Pins = new List<PinterestPin>();
                while (totalcount < count)
                {
                    if (objDominatorAccountModel.IsRunProcessThroughBrowser)
                    {
                        if (lstPinOriginal.Count == 0)
                            Pins = BrowserManager.SearchPinsByKeyword(objDominatorAccountModel, scrapePostDetails.Substring(3)?.TrimStart(),
                            objDominatorAccountModel.CancellationSource, scroll: 0);

                        else
                            Pins = BrowserManager.SearchPinsByKeyword(objDominatorAccountModel, scrapePostDetails.Substring(3)?.TrimStart(),
                            objDominatorAccountModel.CancellationSource, isScroll: true, scroll: 1);
                        lstPinOriginal.AddRange(Pins);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(bookmark))
                        {
                            var searchAllPinResponseHandler = PinFunct.GetAllPinsByKeyword(scrapePostDetails.Substring(3)?.TrimStart(), objDominatorAccountModel,string.Empty,string.Empty,false);
                            lstPinOriginal.AddRange(searchAllPinResponseHandler.LstPin);
                            bookmark = searchAllPinResponseHandler.BookMark;
                        }
                        else
                        {
                            Thread.Sleep(TimeSpan.FromSeconds(1));
                            var searchAllPinResponseHandler = PinFunct.GetAllPinsByKeyword(scrapePostDetails.Substring(3)?.TrimStart(),
                                objDominatorAccountModel, bookmark,string.Empty,false);
                            lstPinOriginal.AddRange(searchAllPinResponseHandler.LstPin);
                            bookmark = searchAllPinResponseHandler.BookMark;
                        }
                    }

                    if (!objDominatorAccountModel.IsRunProcessThroughBrowser && 
                        (string.IsNullOrEmpty(bookmark) || bookmark.Length < 10 || lstPinOriginal.Count == 0 || lstPinOriginal.Count >= 100))
                        break;
                    if (objDominatorAccountModel.IsRunProcessThroughBrowser && (lstPinOriginal.Count >= 100 || Pins.Count == 0))
                        break;
                    cancellationTokenSource.Token.ThrowIfCancellationRequested();
                    var LstPin = new List<PinterestPin>();
                    if (scrapePostModel.IsScrapePostOlderThanXXDays)
                        LstPin = PinListOlderThanXXDays(scrapePostModel, lstPinOriginal);
                    else
                        LstPin = lstPinOriginal;

                    foreach (PinterestPin pin in LstPin)
                    {
                        cancellationTokenSource.Token.ThrowIfCancellationRequested();

                        if (CheckForDuplicatePost(pin.PinId))
                            continue;

                        if (totalcount >= count)
                            break;
                        if (!scrapePostModel.IsOriginalPostDetails)
                        {
                            if (scrapPostLst.Count == 0 && lstScrapePostDetails.Count > 0)
                                scrapPostLst = new List<string>(lstScrapePostDetails);
                            var postDetails = scrapPostLst.GetRandomItem();
                            var postDetailsLst = postDetails.Trim().Split('\t').ToList();
                            var detailsCount = postDetailsLst.Count;
                            pin.PinName = postDetailsLst.First();
                            pin.Description = detailsCount > 1 ? postDetailsLst.First(x => x != pin.PinName) : "";
                            pin.PinWebUrl = detailsCount > 2 ? postDetailsLst.First(x => x != pin.PinName && x != pin.Description) : "";
                            scrapPostLst.Remove(postDetails);
                        }
                        cancellationTokenSource.Token.ThrowIfCancellationRequested();
                        SavePinterestPinModel(campaignId, pin);
                        totalcount++;
                        _publisherInitialize.UpdatePostCounts(campaignId);
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

        public void ScrapePinsFromSpecificUser(string accountId, string campaignId, ref int totalcount, string scrapePostDetails,
            ScrapePostModel scrapePostModel, CancellationTokenSource cancellationTokenSource, int count = 10, List<string> lstScrapPostDetails = null)
        {
            try
            {
                cancellationTokenSource.Token.ThrowIfCancellationRequested();
                var accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
                var objDominatorAccountModel = accountsFileManager.GetAccountById(accountId);

                var scrapPostLst = new List<string>(lstScrapPostDetails);

                var LstPinOriginal = new List<PinterestPin>();
                var Pins = new List<PinterestPin>();
                var bookmark = string.Empty;
                while (totalcount < count)
                {
                    if (objDominatorAccountModel.IsRunProcessThroughBrowser)
                    {
                        if (LstPinOriginal.Count == 0)
                            Pins = BrowserManager.SearchPinsOfUser(objDominatorAccountModel, scrapePostDetails.Substring(3)?.TrimStart(),
                            objDominatorAccountModel.CancellationSource, scroll: 0);

                        else
                            Pins = BrowserManager.SearchPinsOfUser(objDominatorAccountModel, scrapePostDetails.Substring(3)?.TrimStart(),
                            objDominatorAccountModel.CancellationSource, isScroll: true, scroll: 1);
                        LstPinOriginal.AddRange(Pins);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(bookmark))
                        {
                            var pinsFromSpecificUserResponseHandler = PinFunct.GetPinsFromSpecificUser(scrapePostDetails.Substring(3)?.TrimStart(), objDominatorAccountModel,string.Empty,string.Empty,false);
                            LstPinOriginal.AddRange(pinsFromSpecificUserResponseHandler.LstUserPin);
                            bookmark = pinsFromSpecificUserResponseHandler.BookMark;
                        }
                        else
                        {
                            Thread.Sleep(TimeSpan.FromSeconds(30));
                            var pinsFromSpecificUserResponseHandler = PinFunct.GetPinsFromSpecificUser(scrapePostDetails.Substring(3)?.TrimStart(),
                                objDominatorAccountModel, bookmark,string.Empty,false);
                            LstPinOriginal.AddRange(pinsFromSpecificUserResponseHandler.LstUserPin);
                            bookmark = pinsFromSpecificUserResponseHandler.BookMark;
                        }
                    }

                    if (!objDominatorAccountModel.IsRunProcessThroughBrowser &&
                        (string.IsNullOrEmpty(bookmark) || bookmark.Length < 10 || LstPinOriginal.Count == 0 || LstPinOriginal.Count >= 100))
                        break;
                    if (objDominatorAccountModel.IsRunProcessThroughBrowser && (Pins.Count == 0 || LstPinOriginal.Count >= 100))
                        break;
                    cancellationTokenSource.Token.ThrowIfCancellationRequested();
                    var LstPin = new List<PinterestPin>();
                    if (scrapePostModel.IsScrapePostOlderThanXXDays)
                        LstPin = PinListOlderThanXXDays(scrapePostModel, LstPinOriginal);
                    else
                        LstPin = LstPinOriginal;

                    foreach (PinterestPin pin in LstPin)
                    {
                        cancellationTokenSource.Token.ThrowIfCancellationRequested();

                        if (CheckForDuplicatePost(pin.PinId))
                            continue;

                        if (totalcount >= count)
                            break;
                        if (!scrapePostModel.IsOriginalPostDetails)
                        {
                            if (scrapPostLst.Count == 0 && lstScrapPostDetails.Count > 0)
                                scrapPostLst = new List<string>(lstScrapPostDetails);
                            var postDetails = scrapPostLst.GetRandomItem();
                            var postDetailsLst = postDetails.Trim().Split('\t').ToList();
                            var detailsCount = postDetailsLst.Count;
                            pin.PinName = postDetailsLst.First();
                            pin.Description = detailsCount > 1 ?postDetailsLst.First(x=>x!=pin.PinName): "";
                            pin.PinWebUrl = detailsCount > 2 ?postDetailsLst.First(x=>x!=pin.PinName&&x!=pin.Description): "";
                            scrapPostLst.Remove(postDetails);
                        }
                        cancellationTokenSource.Token.ThrowIfCancellationRequested();
                        SavePinterestPinModel(campaignId, pin);
                        totalcount++;
                        _publisherInitialize.UpdatePostCounts(campaignId);
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

        public void ScrapePinsByBoardUrl(string accountId, string campaignId, ref int totalcount, string scrapePostDetails,
            ScrapePostModel scrapePostModel, CancellationTokenSource cancellationTokenSource, int count = 10, List<string> lstScrapPostDetails = null)
        {
            try
            {
                cancellationTokenSource.Token.ThrowIfCancellationRequested();
                var accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
                var objDominatorAccountModel = accountsFileManager.GetAccountById(accountId);

                var scrapPostLst = new List<string>(lstScrapPostDetails);

                var lstPinOriginal = new List<PinterestPin>();
                var Pins = new List<PinterestPin>();
                var bookmark = string.Empty;
                while (totalcount < count)
                {
                    if (objDominatorAccountModel.IsRunProcessThroughBrowser)
                    {
                        if (lstPinOriginal.Count == 0)
                            Pins = BrowserManager.SearchPinsOfBoard(objDominatorAccountModel, scrapePostDetails.Substring(3)?.TrimStart(),
                             objDominatorAccountModel.CancellationSource, scroll: 0);

                        else
                            Pins = BrowserManager.SearchPinsOfBoard(objDominatorAccountModel, scrapePostDetails.Substring(3)?.TrimStart(),
                            objDominatorAccountModel.CancellationSource, isScroll: true, scroll: 1);
                        lstPinOriginal.AddRange(Pins);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(bookmark))
                        {
                            var pinsByBoardUrlResponseHandler = PinFunct.GetPinsByBoardUrl(scrapePostDetails.Substring(3)?.TrimStart(),
                            objDominatorAccountModel,string.Empty,string.Empty,false);
                            lstPinOriginal.AddRange(pinsByBoardUrlResponseHandler.LstBoardPin);
                            bookmark = pinsByBoardUrlResponseHandler.BookMark;
                        }
                        else
                        {
                            Thread.Sleep(TimeSpan.FromSeconds(30));
                            var pinsByBoardUrlResponseHandler = PinFunct.GetPinsByBoardUrl(scrapePostDetails.Substring(3)?.TrimStart(),
                                objDominatorAccountModel, bookmark,string.Empty,false);
                            lstPinOriginal.AddRange(pinsByBoardUrlResponseHandler.LstBoardPin);
                            bookmark = pinsByBoardUrlResponseHandler.BookMark;
                        }
                    }
                    var LstPin = new List<PinterestPin>();
                    if (scrapePostModel.IsScrapePostOlderThanXXDays)
                        LstPin = PinListOlderThanXXDays(scrapePostModel, lstPinOriginal);
                    else
                        LstPin = lstPinOriginal;
                    foreach (PinterestPin pin in LstPin)
                    {
                        cancellationTokenSource.Token.ThrowIfCancellationRequested();

                        if (CheckForDuplicatePost(pin.PinId))
                            continue;
                        cancellationTokenSource.Token.ThrowIfCancellationRequested();
                        if (totalcount >= count)
                            break;
                        if (!scrapePostModel.IsOriginalPostDetails)
                        {
                            if (scrapPostLst.Count == 0 && lstScrapPostDetails.Count > 0)
                                scrapPostLst = new List<string>(lstScrapPostDetails);
                            var postDetails = scrapPostLst.GetRandomItem();
                            var postDetailsLst = postDetails.Trim().Split('\t').ToList();
                            var detailsCount = postDetailsLst.Count;
                            pin.PinName = postDetailsLst.First();
                            pin.Description = detailsCount > 1 ?postDetailsLst.First(x=>x!=pin.PinName): "";
                            pin.PinWebUrl = detailsCount > 2 ?postDetailsLst.First(x=>x!=pin.PinName&&x!=pin.Description): "";
                            scrapPostLst.Remove(postDetails);
                        }
                        cancellationTokenSource.Token.ThrowIfCancellationRequested();
                        SavePinterestPinModel(campaignId, pin);
                        totalcount++;
                        _publisherInitialize.UpdatePostCounts(campaignId);
                    }
                    if (objDominatorAccountModel.IsRunProcessThroughBrowser && (Pins.Count == 0)||LstPin.Count >= 100)
                        break;
                    if (!objDominatorAccountModel.IsRunProcessThroughBrowser &&
                       (string.IsNullOrEmpty(bookmark) || bookmark.Contains("-end-") || bookmark.Length < 10 || LstPin.Count >= 100))
                        break;
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

        private List<PinterestPin> PinListOlderThanXXDays(ScrapePostModel scrapePostModel, [NotNull] List<PinterestPin> lstPin)
        {
            if (lstPin == null) throw new ArgumentNullException(nameof(lstPin));
            var lstPinOlderThanXxDays = new List<PinterestPin>();
            try
            {
                DateTime currentDateTime = DateTime.Parse(DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss"));
                currentDateTime = currentDateTime.Subtract(TimeSpan.FromDays(scrapePostModel.DoNotScrapePostOlderThanNDays.StartValue));

                foreach (var items in lstPin)
                {
                    DateTime pinDateTime = DateTime.Parse(items.PublishDate);
                    var dateCompare = DateTime.Compare(pinDateTime, currentDateTime);

                    if (dateCompare >= 0)
                        lstPinOlderThanXxDays.Add(items);
                }
            }
            catch
            {
                // ignored
            }

            return lstPinOlderThanXxDays;
        }

        private void SavePinterestPinModel(string campaignId, PinterestPin pin)
        {
            try
            {
                var model = new PublisherPostlistModel();
                model.GenerateClonePostId();
                model.CampaignId = campaignId;
                model.ExpiredTime = DateTime.Now.AddYears(2);
                model.PostSource = PostSource.ScrapedPost;
                model.PostQueuedStatus = PostQueuedStatus.Pending;

                model.PublisherInstagramTitle = pin.PinName;
                model.PostDescription = pin.Description;
                model.PdSourceUrl = pin.PinWebUrl;

                model.FetchedPostIdOrUrl = pin.PinId;
                model.MediaList.Add(pin.MediaString);

                PostlistFileManager.Add(campaignId, model);
            }
            catch (ArgumentException e)
            {
                e.DebugLog();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public bool LoginForScraper(DominatorAccountModel account, string campaignId, CancellationTokenSource cancellationTokenSource)
        {
            try
            {
                cancellationTokenSource.Token.ThrowIfCancellationRequested();
                var accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
                var objDominatorAccountModel = accountsFileManager.GetAccountById(account.AccountId);
                var accountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();
                var pdLogInProcess = accountScopeFactory[$"{account.AccountId}_{campaignId}_Publisher"]
                    .Resolve<IPdLogInProcess>();
                PinFunct = accountScopeFactory[$"{account.AccountId}_{campaignId}_Publisher"].Resolve<IPinFunction>();

                cancellationTokenSource.Token.ThrowIfCancellationRequested();
                if (objDominatorAccountModel.IsRunProcessThroughBrowser)
                    pdLogInProcess.BrowserManager =
                        accountScopeFactory[$"{account.AccountId}_{campaignId}_Publisher_ScrapPost"].Resolve<IPdBrowserManager>();
                bool isLoggedIn = pdLogInProcess.CheckLoginAsync(objDominatorAccountModel, objDominatorAccountModel.Token, true).Result;

                BrowserManager = pdLogInProcess.BrowserManager;

                return isLoggedIn;
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}