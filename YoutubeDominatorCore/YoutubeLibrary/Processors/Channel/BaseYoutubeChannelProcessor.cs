using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using YoutubeDominatorCore.YDUtility;
using YoutubeDominatorCore.YoutubeLibrary.DAL;
using YoutubeDominatorCore.YoutubeLibrary.Processes;
using YoutubeDominatorCore.YoutubeLibrary.YdFunctions;
using YoutubeDominatorCore.YoutubeModels;

namespace YoutubeDominatorCore.YoutubeLibrary.Processors.Channel
{
    public abstract class BaseYoutubeChannelProcessor : BaseYoutubeProcessor
    {
        protected BaseYoutubeChannelProcessor(IYdJobProcess jobProcess, IBlackWhiteListHandler blackWhiteListHandler,
            IDbCampaignService campaignService,
            IYoutubeFunctionality youtubeFunctionality) : base(jobProcess, blackWhiteListHandler,
            campaignService, youtubeFunctionality)
        {
        }

        public void StartProcessForListOfChannels(QueryInfo queryInfo, ref JobProcessResult jobProcessResult,
            List<YoutubeChannel> newYoutubeChannelsList)
        {
            var blacklistedFiltred = SkipBlackListOrWhiteList(newYoutubeChannelsList);
            foreach (var channel in blacklistedFiltred)
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                try
                {
                    if (ReturnIfFilterFailedOrLimitReached(queryInfo, ref jobProcessResult))
                        return;

                    if (AlreadyDoneInDB(queryInfo, channel.ChannelId, channel.ChannelUsername))
                        continue;

                    var url = YdStatic.ChannelUrl(channel.ChannelId, channel.ChannelUsername);
                    if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                        BrowserManager.LoadPageSource(JobProcess.DominatorAccountModel, url);

                    var channelInfoResponseHandler = JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser
                        ? BrowserManager.GetChannelDetails(url, 2)
                        : YoutubeFunction.GetChannelDetails(JobProcess.DominatorAccountModel, channel.ChannelUrl,
                            delayBeforeHit: 2);

                    if (channelInfoResponseHandler.Success)
                        StartCustomChannelProcess(queryInfo, ref jobProcessResult,
                            channelInfoResponseHandler.YoutubeChannel);
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
                    CloseSubBrowser();
                }
            }
        }

        public int StartCustomChannelProcess(QueryInfo queryInfo, ref JobProcessResult jobProcessResult,
            YoutubeChannel newYoutubeChannel)
        {
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            if (AlreadyInteracted(newYoutubeChannel) || FilterChannelApply(newYoutubeChannel, 1))
                return 7;

            StartFinalProcess(ref jobProcessResult, queryInfo, newYoutubeChannel);

            return 0;
        }

        /// <summary>
        ///     get to know channel is already interacted by web respond channel info
        /// </summary>
        /// <param name="channelInfo"> channel information inside</param>
        /// <returns></returns>
        protected bool AlreadyInteracted(YoutubeChannel channelInfo)
        {
            if (ActType == ActivityType.Subscribe && channelInfo.IsSubscribed
                || ActType == ActivityType.UnSubscribe && !channelInfo.IsSubscribed)
            {
                var channelIdentity = !string.IsNullOrEmpty(channelInfo.ChannelId)
                    ? channelInfo.ChannelId
                    : channelInfo.ChannelUsername;
                CustomLog(
                    $"Already {ActType.ToString().ToLower()}d the channel ({channelInfo.ChannelName} [ {channelIdentity} ])");
                return true;
            }

            return false;
        }

        protected bool AlreadyDoneInDB(QueryInfo queryInfo, string channelId, string channelUsername)
        {
            var alreadyUsed = DbAccountService.GetSpecificInteractedChannels(ActType,
                JobProcess.DominatorAccountModel.AccountBaseModel.UserId, channelId, channelUsername).ToList();

            return alreadyUsed.Count > 0;
        }
    }
}