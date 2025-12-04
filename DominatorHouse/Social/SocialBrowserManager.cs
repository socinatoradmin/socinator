using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.PuppeteerBrowser;
using DominatorHouseCore.Utility;
using EmbeddedBrowser;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using ThreadUtils;
using CoreUtilities = DominatorHouseCore.Utility.Utilities;

namespace DominatorHouse.Social
{

    public class SocialBrowserManager : ISocialBrowserManager
    {
        private CancellationToken _cancellationToken;
        private IDelayService _delayService;

        public BrowserWindow BrowserWindow { get; set; }

        public SocialBrowserManager()
        {
            _delayService = InstanceProvider.GetInstance<IDelayService>();
        }


        private void AssignCancellationToken(CancellationToken cancellationToken)
        {
            this._cancellationToken = cancellationToken;
        }

        public bool BrowserLogin(DominatorAccountModel account, CancellationToken cancellationToken, LoginType loginType = LoginType.AutomationLogin, VerificationType verificationType = 0)
        {
            bool isRunning = true;

            Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                try
                {
                    account.IsUserLoggedIn = false;

                    if (loginType == LoginType.AutomationLogin || loginType == LoginType.BrowserLogin
                          || loginType == LoginType.InitialiseBrowser || (loginType == LoginType.CheckLogin && BrowserWindow.IsDisposed))
                        BrowserWindow = new BrowserWindow(account) //, userAgent:"Mozilla/5.0 (Windows NT 6.3; Trident/7.0; rv:11.0) like Gecko"
                        { Visibility = Visibility.Hidden, IsLoggedIn = false };

                    if (loginType == LoginType.BrowserLogin)
                        BrowserWindow.Visibility = Visibility.Visible;

#if DEBUG
                    BrowserWindow.Visibility = Visibility.Visible;
#endif

                    BrowserWindow.Show();
                    await _delayService.DelayAsync(3000, cancellationToken);

                    var pageSource = await BrowserWindow.GetPageSourceAsync();

                    var currentTime = DateTime.Now;

                    while ((string.IsNullOrEmpty(pageSource) || pageSource.Length < 1000)
                                && (DateTime.Now - currentTime).TotalSeconds < 60)
                    {
                        BrowserWindow.Refresh();
                        await _delayService.DelayAsync(5000, cancellationToken);
                    }

                    AssignCancellationToken(cancellationToken);

                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                isRunning = false;
            });

            while (isRunning)
                _delayService.DelayAsync(1000, cancellationToken).Wait(cancellationToken);

            return account.IsUserLoggedIn;
        }

        public void CloseBrowser(DominatorAccountModel account)
        {
            bool isRunning = true;
            try
            {
                if (BrowserWindow == null) return;
                Application.Current.Dispatcher.Invoke(async () =>
                {
                    try
                    {
                        BrowserWindow.Close();
                        BrowserWindow.Dispose();
                        await _delayService.DelayAsync(1000, _cancellationToken);
                        isRunning = false;
                    }
                    catch (Exception exc)
                    {
                        exc.DebugLog();
                    }
                });
            }
            catch (Exception exc)
            {
                exc.DebugLog();
            }

            while (isRunning)
                _delayService.DelayAsync(500, _cancellationToken).Wait(_cancellationToken);
        }

        public void ExpandGoogleImagesFromLink(string url, ref string title)
        {
            bool isRunning = true;
            var currentTitle = string.Empty;
            try
            {
                if (BrowserWindow == null)
                    return;
                Application.Current.Dispatcher.Invoke(async () =>
                {
                    try
                    {
                        await BrowserWindow.GoToCustomUrl(url, delayAfter: 5);

                        var imageTagCount = await BrowserWindow.GetElementValueAsync(ActType.GetLength, AttributeType.TagName,
                            "img");

                        var currentTime = DateTime.Now;

                        while ((string.IsNullOrEmpty(imageTagCount) || int.Parse(imageTagCount) < 25) && (DateTime.Now - currentTime).TotalSeconds < 60)
                        {
                            await BrowserWindow.GoToCustomUrl(url, delayAfter: 5);
                            await _delayService.DelayAsync(5000, _cancellationToken);
                            imageTagCount = await BrowserWindow.GetElementValueAsync(ActType.GetLength, AttributeType.TagName,
                            "img");
                        }

                        var pageSource = await BrowserWindow.GetPageSourceAsync();

                        currentTitle = CoreUtilities.GetBetween(pageSource, "<title>", "- Google Search</title>");

                        pageSource = Regex.Replace(pageSource, "<scrolling-carousel(.*?)scrolling-carousel>", string.Empty);

                        var imageJsName = Regex.Matches(Regex.Split(pageSource, "<img src")[2] ?? string.Empty,
                            "jsname=\"(.*?)\"")[0].Groups[1].ToString();


                        var ImageSource = await BrowserWindow.ExecuteScriptAsync($"document.querySelectorAll('[jsname=\"{imageJsName}\"]')[0].outerHTML");
                        //await BrowserWindow.BrowserActAsync(ActType.ActByQuery,
                        //    AttributeType.Jsname, imageJsName, delayAfter: 2);
                        //var imgurl = Regex.Matches(Regex.Split(ImageSource.Result.ToString(), "<img")[1] ?? string.Empty, "src=\"(.*?)\"")[0].Groups[1].ToString();
                        //await BrowserWindow.GoToCustomUrl(imgurl, delayAfter: 5);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    isRunning = false;
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            while (isRunning)
                _delayService.DelayAsync(500, _cancellationToken).Wait(_cancellationToken);

            title = currentTitle;
        }

        public string GetGoogleImages()
        {
            bool isRunning = true;
            var imageUrl = string.Empty;
            string pageSource;

            try
            {
                Application.Current.Dispatcher.Invoke(async () =>
                {
                    try
                    {
                        pageSource = await BrowserWindow.GetPageSourceAsync();
                        //         pageSource = Regex.Split(pageSource, "polygon>").Length > 1 ? Regex.Split(pageSource, "polygon>")[1]
                        //          : string.Empty;
                        imageUrl = Regex.Matches(pageSource, "<img src=\"(.*?)\"")[1].Groups[1].ToString();

                        var currentTime = DateTime.Now;

                        while (!imageUrl.StartsWith("http") && (DateTime.Now - currentTime).TotalSeconds < 10)
                        {
                            await _delayService.DelayAsync(200, _cancellationToken);
                            pageSource = await BrowserWindow.GetPageSourceAsync();
                            pageSource = Regex.Split(pageSource, "polygon>").Length > 1 ? Regex.Split(pageSource, "polygon>")[1]
                              : string.Empty;
                            imageUrl = Regex.Matches(pageSource, "src=\"(.*?)\"")[1].Groups[1].ToString();

                        }

                        await BrowserWindow.PressAnyKeyUpdated(39, delayAtLast: 1);

                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    isRunning = false;
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            while (isRunning)
                _delayService.DelayAsync(500, _cancellationToken).Wait(_cancellationToken);

            return imageUrl ?? string.Empty;
        }

        public bool HasMoreResults()
        {

            bool isRunning = true;
            bool hasMoreResults = true;

            try
            {
                Application.Current.Dispatcher.Invoke(async () =>
                {
                    try
                    {

                        var currentLength = (await BrowserWindow.GetXAndYAsync
                            (customScriptX: "document.querySelectorAll('[data-errormessage=\"Unable to load more. Click to retry.\"]')[0].getBoundingClientRect().left",
                            customScriptY: "document.querySelectorAll('[data-errormessage=\"Unable to load more. Click to retry.\"]')[0].getBoundingClientRect().top")).Value;

                        if (currentLength < 1000)
                            hasMoreResults = false;

                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    isRunning = false;
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            while (isRunning)
                _delayService.DelayAsync(500, _cancellationToken).Wait(_cancellationToken);

            return hasMoreResults;
        }
    }
}
