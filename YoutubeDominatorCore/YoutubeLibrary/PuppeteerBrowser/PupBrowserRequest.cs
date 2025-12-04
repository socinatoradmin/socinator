using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.PuppeteerBrowser;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using YoutubeDominatorCore.YoutubeModels;

namespace YoutubeDominatorCore.YoutubeLibrary.PuppeteerBrowser
{
    public class PupBrowserRequest
    {
        private bool SkipYoutubeAd { get; set; }
        public bool FoundAd { get; set; }
        public string CurrentData { get; set; }

        public bool SetVideoQuality { get; set; }
        public DominatorAccountModel AccountModel { get; set; }
        public PuppeteerBrowserActivity browserActivity { get; set; }
        public CancellationTokenSource CancellationToken { get; set; }
        public void InitializeBrowser(DominatorAccountModel dominatorAccount, CancellationTokenSource cancellationToken)
        {
            CancellationToken = cancellationToken;
            AccountModel = dominatorAccount;
            browserActivity = new PuppeteerBrowserActivity(AccountModel, isNeedResourceData: true);
        }
        private Stopwatch VideoPlayTime;
        private const string HomeUrl = "https://www.youtube.com/";
        private readonly YoutubeModel _youtubeModel;
        public PupBrowserRequest() { }
        public PupBrowserRequest(DominatorAccountModel dominatorAccountModel, CancellationTokenSource cancellationToken)
        {
            try
            {
                InitializeBrowser(dominatorAccountModel, cancellationToken);
                var genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
                _youtubeModel = genericFileManager.GetModel<YoutubeModel>(ConstantVariable.GetOtherYoutubeSettingsFile());
            }
            catch (Exception)
            {
                _youtubeModel = new YoutubeModel();
            }
        }
        string response = string.Empty;
        List<string> ResponseList = new List<string>();
        public bool ViewIncreaserVideo(DominatorAccountModel acountModel, YoutubePost youtubePost,
            int delay,
            string channelPageIdSource, bool browserHidden, bool skipAd,
            double delayBeforeHit = 0)
        {
            bool isRunning = true;
            var viewed = false;
            if (string.IsNullOrEmpty(youtubePost?.PostUrl))
                return viewed;
            Task.Factory.StartNew(async () =>
            {
                try
                {
                    await browserActivity.LaunchBrowserAsync(false, youtubePost.PostUrl);
                    await Task.Delay(5000);
                    browserActivity.CloseUnWantedTabs(youtubePost.PostUrl, CancellationToken);
                    await Task.Delay(5000);
                    SkipYoutubeAd = skipAd;
                    browserActivity.CheckAd();
                    if (FoundAd) browserActivity.Sleep(3);
                    if (SkipYoutubeAd && FoundAd)
                        await browserActivity.BrowserActAsync(ActType.Click, AttributeType.ClassName,
                            "ytp-ad-skip-button-icon");
                    CancellationToken.Token.ThrowIfCancellationRequested();

                    GlobusLogHelper.log.Info(Log.StartedActivity, acountModel.AccountBaseModel.AccountNetwork,
                        acountModel.AccountBaseModel.UserName, ActivityType.ViewIncreaser,
                        $"{youtubePost.PostUrl} (Viewing times -> {youtubePost.TotalWatchingCount + 1})");
                    await Task.Factory.StartNew(() => viewed = ViewIncreaseWithBrowser(acountModel, youtubePost.PostUrl,
                     delay, browserHidden, youtubePost.Title));
                    viewed = true;
                    CancellationToken.Token.ThrowIfCancellationRequested();
                    isRunning = false;
                    return viewed;
                }
                catch (OperationCanceledException ex)
                {
                    ex.DebugLog();
                    return viewed;
                }
                catch (AggregateException ex)
                {
                    ex.DebugLog();
                    return viewed;
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                    return viewed;
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
            return viewed;
        }
        private bool ViewIncreaseWithBrowser(DominatorAccountModel accountModel, string youtubeVideoUrl, int delay,
             bool hiddenBrowser, string videoTitle)
        {
            try
            {
                var openPupBrowser = new Task(async () =>
                {
                    try
                    {
                        browserActivity.SetVideoQualityAs144P(accountModel);
                        CurrentData = browserActivity.GetCurrentPageurl();
                        var currentUrl = CurrentData;
                        var increaseCount = VideoPlayTime == null ? 0 : VideoPlayTime.ElapsedMilliseconds / 1000;

                        while (delay > ++increaseCount && currentUrl == CurrentData)
                        {
                            CancellationToken.Token.ThrowIfCancellationRequested();
                            while (FoundAd)
                            {
                                CancellationToken.Token.ThrowIfCancellationRequested();
                                browserActivity.Sleep();
                            }
                            browserActivity.Sleep(0.99);
                            CurrentData = browserActivity.GetCurrentPageurl();
                        }

                        browserActivity.PlayPauseVideo(accountModel);
                    }
                    catch (OperationCanceledException e)
                    {
                        e.DebugLog();
                    }
                    catch (Exception e)
                    {
                        e.DebugLog();
                    }

                    if (VideoPlayTime != null)
                    {
                        VideoPlayTime.Stop();
                        VideoPlayTime = null;
                    }
                });
                openPupBrowser.Start();
                openPupBrowser.Wait(CancellationToken.Token);

                return true;
            }
            catch (OperationCanceledException)
            {
                return false;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return false;
            }
        }
        public async Task<int> GetTryIndexByInnerText(string className, string contains)
        {
            int count = 0;
            do
            {
                var innerTexts = await browserActivity.GetValue(AttributeType.ClassName, className,
                                                 index: count, valueType: ValueTypes.InnerText);
                if (innerTexts.Equals(contains))
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
