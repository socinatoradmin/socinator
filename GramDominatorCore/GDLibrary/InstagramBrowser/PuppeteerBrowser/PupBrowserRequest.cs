using DominatorHouseCore.Models;
using DominatorHouseCore.PuppeteerBrowser;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using EmbeddedBrowser;
using GramDominatorCore.GDModel;
using GramDominatorCore.Response;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
namespace GramDominatorCore.GDLibrary.InstagramBrowser.PuppeteerBrowser
{
    public class PupBrowserRequest
    {
        public PupBrowserRequest(DominatorAccountModel dominatorAccountModel, CancellationTokenSource cancellationToken)
        {
            InitializeBrowser(dominatorAccountModel, cancellationToken);
        }
        public void InitializeBrowser(DominatorAccountModel dominatorAccount, CancellationTokenSource cancellationToken)
        {
            this.CancellationToken = cancellationToken;
            AccountModel = dominatorAccount;
            browserActivity = new PuppeteerBrowserActivity(AccountModel,isNeedResourceData:true);
        }
        string response = string.Empty;
        List<string> ResponseList = new List<string>();

        public DominatorAccountModel AccountModel { get; set; }
        public CancellationTokenSource CancellationToken { get; set; }
        public PuppeteerBrowserActivity browserActivity { get; set; }

        public UploadMediaResponse UploadVideo(InstagramPost instagramPost, List<string> mediaList)
        {
            bool isRunning = true;
            string pageSource = string.Empty;
            Task.Factory.StartNew(async () =>
            {
                try
                {
                    var dataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Google", "Chrome", "UserData", "Profile1");
                    var IsHeadLess = true;
#if DEBUG
                    IsHeadLess = false;
#endif
                    await browserActivity.LaunchBrowserAsync(HeadLess: IsHeadLess,ProfileDir:dataPath);
                    var content = await browserActivity.GetPageSourceAsync();
                    if (content != null && content.Contains("Turn on Notifications"))
                        await browserActivity.ClickEvent(
                                                AttributeType.ClassName,
                                                "_a9-- _ap36 _a9_1",
                                                index: 0);
                    int count = await GetTryIndexByInnerText("x1wj20lx x1lq5wgf xgqcy7u x30kzoy x9jhf4c", "Create");
                    await browserActivity.ClickEvent(
                                                AttributeType.ClassName,
                                                "x1wj20lx x1lq5wgf xgqcy7u x30kzoy x9jhf4c",
                                                index: 6);
                    await Task.Delay(5000);
                    content = await browserActivity.GetPageSourceAsync();
                    if (content != null && !content.Contains("Drag photos and videos here"))
                        await browserActivity.ClickEvent(
                                                AttributeType.ClassName,
                                                "x1ja2u2z x1t137rt x1q0g3np x87ps6o x1lku1pv x1a2a7pz x1dm5mii x16mil14 xiojian x1yutycm x1lliihq x193iq5w xh8yej3",
                                                index: 0);
                    await Task.Delay(5000);
                    count = await GetTryIndexByInnerText(" _acan _acap _acas _aj1- _ap30", "Select from computer");
                    await browserActivity.ChooseFile(mediaList?.FirstOrDefault(),$"document.getElementsByClassName(' _acan _acap _acas _aj1- _ap30')[{count}].click()");
            //        await browserActivity.ChooseFileFromDialog(mediaList[0], " _acan _acap _acas _aj1- _ap30", count);
                    await Task.Delay(10000);
                    var data = await browserActivity.GetPageSourceAsync();
                    if (instagramPost != null && instagramPost.IsCheckedCropMedia && !string.IsNullOrEmpty(instagramPost.CropRatio))
                    {
                        {
                            await browserActivity.ExecuteScriptAsync("document.querySelector('svg[aria-label=\"Select Crop\" i]').parentElement.parentElement.click();", delayInSec: 4);
                            var SelectRatioClass = "x1t137rt x1o1ewxj x3x9cwd x1e5q0jg x13rtm0m x3nfvp2 x1q0g3np x87ps6o x1lku1pv x1a2a7pz";
                            var Nodes = HtmlParseUtility.GetListNodesFromClassName(browserActivity.GetPageSource(), SelectRatioClass);
                            var ClickIndex = Nodes != null && Nodes.Count > 0 ? Nodes.IndexOf(Nodes.FirstOrDefault(x => x.InnerText == instagramPost.CropRatio || !string.IsNullOrEmpty(x.InnerText) && x.InnerText.Contains(instagramPost.CropRatio)) ?? Nodes.FirstOrDefault()) : 0;
                            await browserActivity.ExecuteScriptAsync($"document.getElementsByClassName('{SelectRatioClass}')[{ClickIndex}].click();", delayInSec: 4);
                        }
                    }
                    if (data.Contains("Video posts are now shared as reels"))
                    {
                        //_acan _acap _acaq _acas _acav _aj1-
                        await browserActivity.ClickEvent(
                                                AttributeType.ClassName,
                                                "_acan _acap _acaq _acas _acav _aj1-",
                                                index: 0);
                    }
                    count = await GetTryIndexByInnerText("x9f619 xjbqb8w x78zum5 x168nmei x13lgxp2 x5pf9jr xo71vjh x1iorvi4 x150jy0e",
                                                 "Create");
                    await browserActivity.ClickEvent(AttributeType.ClassName,
                                           "x9f619 xjbqb8w x78zum5 x168nmei x13lgxp2 x5pf9jr xo71vjh x1iorvi4 x150jy0e", count);
                    await Task.Delay(5000);
                    count = await GetTryIndexByInnerText("xexx8yu x4uap5 x18d9i69 xkhd6sd x1n2onr6 x1n5bzlp x173jzuc x1yc6y37",
                                                 "Next");
                    await browserActivity.ClickEvent(AttributeType.ClassName,
                                                        "xexx8yu x4uap5 x18d9i69 xkhd6sd x1n2onr6 x1n5bzlp x173jzuc x1yc6y37",
                                                        count);
                    await Task.Delay(5000);
                    count = await GetTryIndexByInnerText("xexx8yu x4uap5 x18d9i69 xkhd6sd x1n2onr6 x1n5bzlp x173jzuc x1yc6y37",
                                                 "Next");
                    await browserActivity.ClickEvent(AttributeType.ClassName,
                                                        "xexx8yu x4uap5 x18d9i69 xkhd6sd x1n2onr6 x1n5bzlp x173jzuc x1yc6y37",
                                                        count);
                    if (!string.IsNullOrEmpty(instagramPost.Caption))
                    {
                        count = await GetTryIndexByInnerText("xen30ot x1swvt13 x1pi30zi xh8yej3 xb88cxz x1a2a7pz x47corl x10l6tqk",
                                                                 "Write a caption...");
                        var element = await browserActivity.GetXAndYByClassNameAsync(AttributeType.ClassName, "xen30ot x1swvt13 x1pi30zi xh8yej3 xb88cxz x1a2a7pz x47corl x10l6tqk", count);
                        await browserActivity.MouseClickAsync(element.Key, element.Value);

                        await browserActivity.EnterCharsAsync(instagramPost.Caption); 
                    }
                    await Task.Delay(5000);
                    if(instagramPost.UserTags.Count > 0)
                    {
                        foreach (var user in instagramPost.UserTags)
                        {
                            count = await GetTryIndexByInnerText("_acan _acao _acas _aj1-", "Tag people");
                            var xyElement = await browserActivity.GetXAndYByClassNameAsync(AttributeType.ClassName, "_acan _acao _acas _aj1-", count); 
                            await browserActivity.MouseClickAsync(xyElement.Key, xyElement.Value);
                            pageSource = await browserActivity.GetPageSourceAsync();
                            var SearchTagUserClass = "xd10rxx x1sy0etr x17r0tee xnz67gz xzd0ubt x1qx5ct2";
                            if (pageSource.Contains("Add tag"))
                            {
                                count = await GetTryIndexByInnerText("_acan _acao _acas _aj1-", "Add tag");
                                xyElement = await browserActivity.GetXAndYByClassNameAsync(AttributeType.ClassName, "_acan _acao _acas _aj1-", count);
                                await browserActivity.MouseClickAsync(xyElement.Key, xyElement.Value);
                                count = await GetTryIndexByInnerText(SearchTagUserClass, "Search", ValueTypes.Placeholder);
                                xyElement = await browserActivity.GetXAndYByClassNameAsync(AttributeType.ClassName, SearchTagUserClass, count);
                                await browserActivity.MouseClickAsync(xyElement.Key+10, xyElement.Value+3);
                                await browserActivity.EnterCharsAsync(user.Username);
                                await Task.Delay(5000, CancellationToken.Token);
                                await browserActivity.ClickEvent(AttributeType.ClassName, " _acmy", 0);
                            }
                            else
                            {
                                count = await GetTryIndexByInnerText(SearchTagUserClass, "Search",ValueTypes.Placeholder);
                                xyElement = await browserActivity.GetXAndYByClassNameAsync(AttributeType.ClassName, SearchTagUserClass, count);
                                await browserActivity.MouseClickAsync(xyElement.Key+10, xyElement.Value+3);
                                await browserActivity.EnterCharsAsync(user.Username);
                                await Task.Delay(5000, CancellationToken.Token);
                                await browserActivity.ClickEvent(AttributeType.ClassName, " _acmy",0);
                            }
                        }
                        
                    }
                    count = await GetTryIndexByInnerText("xexx8yu x4uap5 x18d9i69 xkhd6sd x1n2onr6 x1n5bzlp x173jzuc x1yc6y37",
                                                 "Share");
                    var xy = await browserActivity.GetXAndYByClassNameAsync(AttributeType.ClassName, "xexx8yu x4uap5 x18d9i69 xkhd6sd x1n2onr6 x1n5bzlp x173jzuc x1yc6y37", count);
                    await browserActivity.MouseClickAsync(xy.Key, xy.Value);
                    while (browserActivity.ResponseList.Count <= 0)
                        await Task.Delay(3000, CancellationToken.Token);
                    count = 0;
                    var finalResponse = string.Empty;
                    while (count < 30
                    && !data.Contains("Your reel has been shared")
                    && string.IsNullOrEmpty(finalResponse))
                    {
                        await Task.Delay(5000, CancellationToken.Token);
                        data = await browserActivity.GetPageSourceAsync();
                        finalResponse = await browserActivity.GetPaginationData("{\"media\":{\"taken_at\":", true);
                        finalResponse = string.IsNullOrEmpty(finalResponse) ? await browserActivity.GetPaginationData("{\"media\":{\"pk\"", true) : finalResponse;
                        finalResponse = string.IsNullOrEmpty(finalResponse) ? await browserActivity.GetPaginationData("\"media\":{\"pk\"", true) : finalResponse;
                        count++;
                    }
                    if (string.IsNullOrEmpty(finalResponse))
                    {
                        finalResponse = await browserActivity.GetPaginationData("{\"media\":{\"taken_at\":", true);
                        finalResponse = string.IsNullOrEmpty(finalResponse) ? await browserActivity.GetPaginationData("\"media\":{\"taken_at\":", true) : finalResponse;
                        finalResponse = string.IsNullOrEmpty(finalResponse) ? await browserActivity.GetPaginationData("{\"media\":{\"pk\"", true) : finalResponse;
                        finalResponse = string.IsNullOrEmpty(finalResponse) ? await browserActivity.GetPaginationData("\"media\":{\"pk\"", true) : finalResponse;
                    }
                    response = finalResponse;
                    isRunning = false;
                }
                catch (Exception)
                {
                }
                finally
                {
                    browserActivity.ClosedBrowser();
                    isRunning = false;
                }
            });
            while (isRunning)
            {
                Task.Delay(2000, CancellationToken.Token).Wait(CancellationToken.Token);
            }
            return new UploadMediaResponse(new ResponseParameter() { Response = response });
        }
        public async Task<int> GetTryIndexByInnerText(string className, string contains,ValueTypes valueTypes = ValueTypes.InnerText)
        {
            int count = 0;
            do
            {
                var innerTexts = await browserActivity.GetValue(AttributeType.ClassName, className,
                                                 index: count, valueType: valueTypes);
                if (innerTexts!=null && innerTexts.Equals(contains))
                    break;
            } while (++count > 0 && count < 50);
            if (count > 50)
            {
                return 0;
            }
            else
                return count;
        }
    }
}
