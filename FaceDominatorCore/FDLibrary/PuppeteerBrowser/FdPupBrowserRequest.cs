using DominatorHouseCore;
using DominatorHouseCore.Models;
using DominatorHouseCore.PuppeteerBrowser;
using DominatorHouseCore.Request;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDResponse.BrowserResponseHandler.UserResponseHandler;
using FaceDominatorCore.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
namespace FaceDominatorCore.FDLibrary.PuppeteerBrowser
{
    public class FdPupBrowserRequest
    {
        public DominatorAccountModel AccountModel { get; set; }
        public CancellationTokenSource CancellationToken { get; set; }
        public PuppeteerBrowserActivity BrowserWindow { get; set; }
        public KeyValuePair<int, int> ScreenResolution { get; set; } = FdFunctions.FdFunctions.GetScreenResolution();
        public void InitializeBrowser(DominatorAccountModel dominatorAccount, CancellationTokenSource cancellationToken)
        {
            CancellationToken = cancellationToken;
            AccountModel = dominatorAccount;
            BrowserWindow = new PuppeteerBrowserActivity(AccountModel, isNeedResourceData: true, targetUrl: FdConstants.FbHomeUrl);
        }
        public FdPupBrowserRequest(DominatorAccountModel dominatorAccountModel, CancellationTokenSource cancellationToken)
        {
            InitializeBrowser(dominatorAccountModel, cancellationToken);
        }
        public async Task FacebookHomePageLogin(DominatorAccountModel account, CancellationToken accountCancellationToken)

        {
            try
            {
                await BrowserWindow.LaunchBrowserAsync();
                await Task.Delay(3000);
            }
            catch (AggregateException e)
            { e.DebugLog(); }
            catch (Exception e)
            { e.DebugLog(); }
        }
        public void SearchByFriendRequests(DominatorAccountModel account, FbEntityType entity)
        {
            bool isRunning = true;
            if (entity == FbEntityType.AddedFriends)
            {
                var userName = account.AccountBaseModel?.UserId;
                if (string.IsNullOrEmpty(account.AccountBaseModel?.UserId))
                {
                    var pageSource = BrowserWindow.GetPageSource();
                    userName = FdRegexUtility.FirstMatchExtractor(pageSource, $"\"username\":\"(.*?)\"");

                    userName = string.IsNullOrEmpty(userName) ? FdRegexUtility.FirstMatchExtractor(pageSource, $"\"USER_ID\":\"(.*?)\"") : userName;

                    if (string.IsNullOrEmpty(userName) || userName.Contains("profile.php"))
                        userName = account.AccountBaseModel?.UserId;
                }
                if (FdFunctions.FdFunctions.IsIntegerOnly(userName))
                    BrowserWindow.GoToUrl($"https://www.facebook.com/profile.php?id={userName}&sk=friends");
                else
                    BrowserWindow.GoToUrl($"{FdConstants.FbHomeUrl}{userName}/friends_recent");

            }
            else
                BrowserWindow.GoToUrl("https://www.facebook.com/friends");
            LoadSource(8).Wait(CancellationToken.Token);
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    BrowserWindow.ClearResources();
                    if (entity == FbEntityType.AddedFriends)
                        await BrowserWindow.ExecuteScriptAsync($"{string.Format(FdConstants.ByTagRoleAttributeButtonScript, "div>a", "tab", "textContent", "Recently added")}[0].click()", delayInSec: 7);
                    else if (entity == FbEntityType.SuggestedFriends)
                        await BrowserWindow.ExecuteScriptAsync($"{string.Format(FdConstants.ByTagRoleAttributeButtonScript, "div>a", "link", "textContent", "Suggestions")}[0].click()", delayInSec: 7);
                    else if (entity == FbEntityType.Friends)
                        await BrowserWindow.ExecuteScriptAsync($"{string.Format(FdConstants.ByTagRoleAttributeButtonScript, "div>a", "link", "textContent", "All friends")}[0].click()", delayInSec: 7);
                    else if (entity == FbEntityType.IncommingFriendRequests || entity == FbEntityType.SentFriendRequests)
                    {
                        await BrowserWindow.ExecuteScriptAsync($"{string.Format(FdConstants.ByTagRoleAttributeButtonScript, "div>a", "link", "textContent", "Friend requests")}[0].click()", delayInSec: 7);
                        if (entity == FbEntityType.SentFriendRequests)
                        {
                            BrowserWindow.ClearResources();
                            await BrowserWindow.ExecuteScriptAsync($"{string.Format(FdConstants.ByTagRoleAttributeButtonScript, "div>div", "button", "textContent", "View sent requests")}[0].click()", delayInSec: 7);
                        }
                    }
                    await LoadSource(8);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                isRunning = false;
            });

            while (isRunning)
                Task.Delay(1000, CancellationToken.Token).Wait();
        }
        public IResponseHandler ScrollWindowAndGetFriends(DominatorAccountModel account,
            FbEntityType entity, int noOfPageToScroll, int lastPageNo = 0, string className = "",
            string paginationClass = "")
        {
            int failedCount = 0;
            bool isRunning = true;
            List<string> JsonResponseList = new List<string>();
            string pageSource = string.Empty;
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    int currentCount = 0;
                    int previousCount = 0;
                    var tempjsonresponselist = new List<string>();
                    while (lastPageNo < noOfPageToScroll && failedCount < 2)
                    {
                        if (entity == FbEntityType.SentFriendRequests || entity == FbEntityType.Friends || entity == FbEntityType.SuggestedFriends || entity == FbEntityType.IncommingFriendRequests)
                            await BrowserWindow.MouseScrollAsync(100, ScreenResolution.Value / 2, 0, -500, delayAfter: 5, scrollCount: 5);
                        else
                            await BrowserWindow.MouseScrollAsync(ScreenResolution.Key / 2 + 50, ScreenResolution.Value / 2, 0, -500, delayAfter: 5, scrollCount: 5);

                        if (entity == FbEntityType.SentFriendRequests)
                            tempjsonresponselist = await BrowserWindow.GetPaginationDataList("data\":{\"viewer\":{\"outgoing_friend_requests\"", true);
                        else if (entity == FbEntityType.IncommingFriendRequests)
                            tempjsonresponselist = await BrowserWindow.GetPaginationDataList("data\":{\"viewer\":{\"friend_requests\"", true);
                        else if (entity == FbEntityType.SuggestedFriends)
                            tempjsonresponselist = await BrowserWindow.GetPaginationDataList("\"people_you_may_know\":{\"", true, "\"__typename\":\"User\"");
                        else if (entity == FbEntityType.UserGreetings)
                            tempjsonresponselist = await BrowserWindow.GetPaginationDataList("data\":{\"", true, "\"all_friends_by_birthday_month\"");
                        else if (entity == FbEntityType.Friends)
                            tempjsonresponselist = await BrowserWindow.GetPaginationDataList("data\":{\"viewer\":{\"all_friends", true);
                        else
                            tempjsonresponselist = await BrowserWindow.GetPaginationDataList("\"__typename\":\"User\"", true);

                        tempjsonresponselist.RemoveAll(x => JsonResponseList.Any(y => y.Contains(x)));
                        currentCount += tempjsonresponselist.Count;
                        JsonResponseList.AddRange(tempjsonresponselist);
                        lastPageNo++;
                        BrowserWindow.ClearResources();
                        if (currentCount == previousCount && JsonResponseList.Count == currentCount) failedCount++;
                        else
                        {
                            failedCount = 0;
                            previousCount = currentCount;
                        }
                        CancellationToken.Token.ThrowIfCancellationRequested();
                    }
                    pageSource = await BrowserWindow.GetPageSourceAsync();
                    CancellationToken.Token.ThrowIfCancellationRequested();
                }
                catch (Exception) { }
                isRunning = false;
            });
            while (isRunning)
                Task.Delay(500, CancellationToken.Token).Wait();
            return new SearchPeopleResponseHandler(JsonResponseList, new ResponseParameter() { Response = pageSource }, new List<string>(), failedCount < 2, entity);
        }
        public async Task LoadSource(int timesec = 15)
        {
            DateTime currentTime = DateTime.Now;
            string pageSource;
            do
            {
                await Task.Delay(TimeSpan.FromSeconds(2), CancellationToken.Token);
                pageSource = await BrowserWindow.GetPageSourceAsync();
                CancellationToken.Token.ThrowIfCancellationRequested();
            } while ((!BrowserWindow.IsLoaded && (DateTime.Now - currentTime).TotalSeconds < timesec + 15) || (string.IsNullOrEmpty(pageSource) && (DateTime.Now - currentTime).TotalSeconds < timesec));

        }
        //public async Task< IResponseHandler> ScrollWindowAndGetFriends(DominatorAccountModel account,
        //   FbEntityType entity, int noOfPageToScroll, int lastPageNo = 0, string className = "",
        //   string paginationClass = "")
        //{
        //    int failedCount = 0;
        //    bool isRunning = true;

        //    bool hasMoreResults = true;

        //    int currentCount = 0;

        //    List<string> itemList = new List<string>();
        //    var screenResolution = FdFunctions.FdFunctions.GetScreenResolution();
        //    string pageSource = string.Empty;
        //    try
        //    {
        //        int index = 0;
        //        int previousCount = 0;
        //        while (lastPageNo < noOfPageToScroll && (failedCount <= 2 || noOfPageToScroll > 100)
        //                   && failedCount <= 10)
        //        {
        //            lastPageNo++;
        //            if (entity == FbEntityType.SentFriendRequests)
        //                currentCount = await browserActivity.GetCountInnerHtmlChildElement(ActType.GetLengthByQuery, parentAttributeType: AttributeType.AriaLabel, parentAttributeValue: "Sent requests"
        //                    , childAttributeName: AttributeType.ClassName, childAttributeValue: className);
        //            else if (entity == FbEntityType.IncommingFriendRequests)
        //                currentCount = await browserActivity.GetItemCountInnerHtml(ActType.GetValue, AttributeType.ClassName, className);
        //            else
        //                currentCount = await browserActivity.GetItemCountInnerHtml(ActType.GetValue, AttributeType.ClassName, className);

        //            if (entity == FbEntityType.UserGreetings)
        //                currentCount = currentCount + await browserActivity.GetItemCountInnerHtml(ActType.GetValue, AttributeType.ClassName, paginationClass);

        //            if (entity == FbEntityType.SentFriendRequests || entity == FbEntityType.IncommingFriendRequests)
        //                await browserActivity.BrowserActAsync(ActType.ScrollIntoView, AttributeType.ClassName, className, index: currentCount - 1);
        //            else if (entity == FbEntityType.UserGreetings || entity == FbEntityType.Unfriend || entity == FbEntityType.AddedFriends)
        //                await browserActivity.MouseScrollAsync(screenResolution.Key / 2, screenResolution.Value / 2
        //                            ,0, -500, delayAfter: 3, scrollCount: 15);
        //            if (currentCount > 2000)
        //                await Task.Delay(2000, CancellationToken.Token);
        //            if (currentCount == previousCount)
        //            {
        //                await Task.Delay(3000, CancellationToken.Token);
        //                failedCount++;
        //            }
        //            else
        //            {
        //                failedCount = 0;
        //                previousCount = currentCount;
        //            }

        //            CancellationToken.Token.ThrowIfCancellationRequested();
        //        }
        //        if (index == 0 || failedCount >= 2)
        //            hasMoreResults = false;

        //        await Task.Delay(10000, CancellationToken.Token);

        //        CancellationToken.Token.ThrowIfCancellationRequested();

        //        pageSource = await browserActivity.GetPageSourceAsync();


        //        itemList = await browserActivity.GetListInnerHtml(ActType.GetValue,
        //                   AttributeType.ClassName, className, ValueTypes.outerHTML);

        //        if (entity == FbEntityType.IncommingFriendRequests)
        //            itemList.RemoveAll(x => !x.Contains("Confirm"));
        //        else if (entity == FbEntityType.SentFriendRequests)
        //            itemList.RemoveAll(x => !x.Contains("aria-label=\"Cancel Request\""));
        //        else if (entity == FbEntityType.AddedFriends)
        //            itemList.RemoveAll(x => !x.ToLower().Contains("friends"));

        //        if (entity == FbEntityType.UserGreetings)
        //        {
        //            itemList.RemoveAll(x => x.StartsWith("<a"));
        //            itemList.RemoveAll(x => x.StartsWith("<label"));
        //            itemList.Reverse();

        //            List<string> paginationItemlist = await browserActivity.GetListInnerHtml
        //                (ActType.GetValue, AttributeType.ClassName, paginationClass);

        //            paginationItemlist.Reverse();

        //            itemList.AddRange(paginationItemlist);
        //        }
        //    }catch (AggregateException e)
        //    {
        //        e.DebugLog();
        //    }
        //    catch(Exception ex)
        //    {
        //        ex.DebugLog();
        //    }
        //    if (entity == FbEntityType.IncommingFriendRequests || entity == FbEntityType.Unfriend)
        //        return new IncommingFriendsResponseHandler(new ResponseParameter(), itemList, hasMoreResults);
        //    else if (entity == FbEntityType.SentFriendRequests)
        //        return new SentFriendRequestResponseHandler(new ResponseParameter(), itemList, hasMoreResults, false);
        //    else if (entity == FbEntityType.AddedFriends)
        //        return new SentFriendRequestResponseHandler(new ResponseParameter(), itemList, hasMoreResults, false, true);
        //    else if (entity == FbEntityType.UserGreetings)
        //        return new SendGreetingsReponseHandler(new ResponseParameter() { Response = pageSource }, itemList, hasMoreResults);
        //    else
        //        return null;

        //}
    }
}
