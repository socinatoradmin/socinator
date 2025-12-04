using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.YdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using YoutubeDominatorCore.Response;
using YoutubeDominatorCore.YDUtility;
using YoutubeDominatorCore.YoutubeLibrary.DAL;
using YoutubeDominatorCore.YoutubeLibrary.Processes;
using YoutubeDominatorCore.YoutubeLibrary.YdFunctions;
using YoutubeDominatorCore.YoutubeModels;
using YoutubeDominatorCore.YoutubeModels.GrowSubscribersModel;

namespace YoutubeDominatorCore.YoutubeLibrary.Processors.Channel
{
    public class UnsubscribeProcessor : BaseYoutubeChannelProcessor
    {
        public UnsubscribeProcessor(IYdJobProcess jobProcess, IBlackWhiteListHandler blackWhiteListHandler,
            IDbCampaignService campaignService,
            IYoutubeFunctionality youtubeFunctionality) : base(jobProcess, blackWhiteListHandler, campaignService,
            youtubeFunctionality)
        {
        }

        protected override void Process(QueryInfo queryInfo)
        {
            var unsubscribeModel =
                JsonConvert.DeserializeObject<UnsubscribeModel>(TemplateModel.ActivitySettings);

            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            if (unsubscribeModel.IsChkChannelSubscribedBySoftwareChecked)
            {
                queryInfo = new QueryInfo { QueryType = "Subscribed by software" };
                UnSubscribeChannelSubscribedBySoftware(queryInfo);
            }

            if (unsubscribeModel.IsChkChannelSubscribedOutsideSoftwareChecked)
            {
                queryInfo = new QueryInfo { QueryType = "Subscribed outside software" };
                UnSubscribeChannelSubscribedOutsideSoftware(queryInfo);
            }

            if (unsubscribeModel.IsChkCustomChannelsListChecked)
            {
                queryInfo = new QueryInfo { QueryType = "Custom users" };
                UnSubscribeCustomChannelList(queryInfo, unsubscribeModel);
            }
        }

        private void UnSubscribeChannelSubscribedBySoftware(QueryInfo queryInfo)
        {
            var subscribedChannelDict = GetSubscribedChannelFromDb();
            var jobProcessResult = new JobProcessResult();

            if (subscribedChannelDict.Count > 0)
                try
                {
                    var lstUnsubscribed = DbAccountService
                        .GetUnsubscribedChannels(JobProcess.DominatorAccountModel.AccountBaseModel.UserId).ToList();
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    lstUnsubscribed.ForEach(x =>
                    {
                        var removeIt = subscribedChannelDict.FirstOrDefault(y =>
                            y.InteractedChannelId == x.InteractedChannelId ||
                            y.InteractedChannelUsername == x.InteractedChannelUsername);
                        if (removeIt != null)
                            subscribedChannelDict.Remove(removeIt);
                    });

                    subscribedChannelDict = SkipBlackListOrWhiteList(subscribedChannelDict);

                    FoundXResultLog(queryInfo, subscribedChannelDict.Count);

                    var waitTimeForNext = 3;
                    foreach (var user in subscribedChannelDict)
                    {
                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        try
                        {
                            var url = YdStatic.ChannelUrl(user.InteractedChannelId, user.InteractedChannelUsername);
                            var youtubeChannel = ChannelInfo(url, waitTimeForNext);

                            if (AlreadyInteracted(youtubeChannel))
                            {
                                AddToDBIfAlready(youtubeChannel, queryInfo);
                                waitTimeForNext = 7;
                            }

                            else
                            {
                                waitTimeForNext =
                                    StartCustomChannelProcess(queryInfo, ref jobProcessResult, youtubeChannel);
                            }

                            if (jobProcessResult.IsProcessCompleted)
                                break;
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
                catch (OperationCanceledException)
                {
                    throw new OperationCanceledException();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
        }

        private void AddToDBIfAlready(YoutubeChannel youtubeChannel, QueryInfo queryInfo)
        {
            try
            {
                DbAccountService.Add(new InteractedChannels
                {
                    AccountUsername = JobProcess.DominatorAccountModel.AccountBaseModel.UserName ?? "",
                    ActivityType = ActType.ToString(),
                    ChannelDescription = youtubeChannel.ChannelDescription ?? "",
                    ExternalLinks = youtubeChannel.ExternalLinks ?? "",
                    InteractedChannelId = youtubeChannel.ChannelId ?? "",
                    InteractedChannelName = youtubeChannel.ChannelName ?? "",
                    InteractionTimeStamp = DateTime.UtcNow.GetCurrentEpochTime(),

                    QueryType = queryInfo.QueryType ?? "",
                    QueryValue = "Subscribed outside software",

                    SubscriberCount = youtubeChannel.SubscriberCount ?? "",
                    IsSubscribed = false,
                    ChannelJoinedDate = youtubeChannel.ChannelJoinedDate ?? "",
                    ChannelLocation = youtubeChannel.ChannelLocation ?? "",
                    ChannelProfilePic = youtubeChannel.ProfilePicUrl ?? "",
                    ChannelUrl = youtubeChannel.ChannelUrl ?? "",
                    VideosCount = youtubeChannel.VideosCount ?? "",
                    ViewsCount = youtubeChannel.ViewsCount ?? "",
                    MyChannelId = JobProcess.DominatorAccountModel.AccountBaseModel.UserId,
                    MyChannelPageId = JobProcess.DominatorAccountModel.AccountBaseModel.ProfileId
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private YoutubeChannel ChannelInfo(string url, int waitTimeForNext)
        {
            if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
            {
                BrowserManager.LoadPageSource(JobProcess.DominatorAccountModel, url, clearandNeedResource: true);
                return BrowserManager.GetChannelDetails(url, waitTimeForNext).YoutubeChannel;
            }

            return YoutubeFunction
                .GetChannelDetails(JobProcess.DominatorAccountModel, url, delayBeforeHit: waitTimeForNext)
                .YoutubeChannel;
        }

        private void UnSubscribeChannelSubscribedOutsideSoftware(QueryInfo queryInfo)
        {
            if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                UsingBrowser(queryInfo);
            else
                UsingHttp(queryInfo);
        }

        private void UsingHttp(QueryInfo queryInfo)
        {
            try
            {
                var jobProcessResult = new JobProcessResult();
                var subscribedChannels = new SubscribedChannelScraperResponseHandler();

                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    subscribedChannels = YoutubeFunction.GetSubscribedChannels(JobProcess.DominatorAccountModel,
                        subscribedChannels.PostDataElements, subscribedChannels.HeadersElements);
                    if (subscribedChannels.Success)
                    {
                        jobProcessResult.maxId = subscribedChannels.PostDataElements?.ContinuationToken;

                        var lstYoutubeChannel = subscribedChannels.YoutubeSubscribedChannelsList;
                        ;

                        lstYoutubeChannel = SkipBlackListOrWhiteList(lstYoutubeChannel);

                        FoundXResultLog(queryInfo, lstYoutubeChannel.Count);
                        var waitTimeForNext = 3;
                        foreach (var channel in lstYoutubeChannel)
                        {
                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            try
                            {
                                var url = YdStatic.ChannelUrl(channel.ChannelId, channel.ChannelUsername);
                                var youtubeChannel = YoutubeFunction
                                    .GetChannelDetails(JobProcess.DominatorAccountModel, url,
                                        delayBeforeHit: waitTimeForNext).YoutubeChannel;
                                waitTimeForNext =
                                    StartCustomChannelProcess(queryInfo, ref jobProcessResult, youtubeChannel);

                                if (jobProcessResult.IsProcessCompleted)
                                    break;
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

                        if (string.IsNullOrEmpty(jobProcessResult.maxId))
                            jobProcessResult.HasNoResult = true;
                    }
                    else
                    {
                        jobProcessResult.maxId = null;
                        jobProcessResult.HasNoResult = true;
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

        private void UsingBrowser(QueryInfo queryInfo)
        {
            try
            {
                var jobProcessResult = new JobProcessResult();
                List<YoutubeChannel> channels = null;
                var index = 0;
                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    channels = BrowserManager.GetSubscribedChannels(ref index, channels == null, 2);

                    channels = SkipBlackListOrWhiteList(channels);

                    FoundXResultLog(queryInfo, channels.Count);

                    if (channels.Count == 0)
                    {
                        NoData(ref jobProcessResult);
                    }
                    else
                    {
                        var waitTimeForNext = 3;
                        foreach (var channel in channels)
                        {
                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            try
                            {
                                var url = YdStatic.ChannelUrl(channel.ChannelId, channel.ChannelUsername);
                                var youtubeChannel = BrowserManager.GetChannelDetails(url, waitTimeForNext)
                                    .YoutubeChannel;

                                waitTimeForNext =
                                    StartCustomChannelProcess(queryInfo, ref jobProcessResult, youtubeChannel);

                                if (jobProcessResult.IsProcessCompleted)
                                    break;
                            }
                            catch (OperationCanceledException)
                            {
                                throw new OperationCanceledException();
                            }
                            catch (Exception ex)
                            {
                                ex.DebugLog();
                            }
                            //finally
                            //{
                            //    CloseSubBrowser();
                            //}
                        }
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

        private void UnSubscribeCustomChannelList(QueryInfo queryInfo, UnsubscribeModel unsubscribeModel)
        {
            try
            {
                var channels = new List<YoutubeChannel>();
                unsubscribeModel.ListCustomChannels.ForEach(x =>
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    var idUsername = YdStatic.ChannelIdUsernameApart(x);
                    channels.Add(new YoutubeChannel { ChannelId = idUsername.Key, ChannelUsername = idUsername.Value });
                });

                var jobProcessResult = new JobProcessResult();

                channels = SkipBlackListOrWhiteList(channels);
                FoundXResultLog(queryInfo, channels.Count);
                var waitTimeForNext = 3;
                foreach (var channel in channels)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    try
                    {
                        var url = YdStatic.ChannelUrl(channel.ChannelId, channel.ChannelUsername);

                        var youtubeChannel = ChannelInfo(url, waitTimeForNext);

                        waitTimeForNext = StartCustomChannelProcess(queryInfo, ref jobProcessResult, youtubeChannel);

                        if (jobProcessResult.IsProcessCompleted)
                            break;
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
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private List<InteractedChannels> GetSubscribedChannelFromDb()
        {
            try
            {
                return
                    DbAccountService.GetInteractedChannels(ActivityType.Subscribe,
                        JobProcess.DominatorAccountModel.AccountBaseModel.UserId).ToList();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return new List<InteractedChannels>();
            }
        }
    }
}