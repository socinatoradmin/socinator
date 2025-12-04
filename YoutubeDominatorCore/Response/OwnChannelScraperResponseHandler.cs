using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using YoutubeDominatorCore.YoutubeModels;

namespace YoutubeDominatorCore.Response
{
    public class OwnChannelScraperResponseHandler : YdResponseHandler
    {
        public OwnChannelScraperResponseHandler(IResponseParameter response)
        {
            try
            {


                var ChannelNodes = HtmlParseUtility.GetListNodeFromPartialTagNamecontains(response.Response, "tp-yt-paper-icon-item", "role", "option");
                foreach (var channelNode in ChannelNodes)
                {
                    try
                    {
                        var details = channelNode.InnerText.Split('\n').ToList();
                        details.RemoveAll(x => string.IsNullOrEmpty(x?.Trim()));
                        var youtubeChannel = new YoutubeChannel();
                        switch (details.Count)
                        {
                            case 1:
                                youtubeChannel.ChannelName = details[0]?.Trim();
                                break;
                            case 2:
                                youtubeChannel.ChannelName = details[0]?.Trim();
                                youtubeChannel.ChannelUsername = details[1]?.Trim();
                                break;
                            case 3:
                                youtubeChannel.ChannelName = details[0]?.Trim();
                                youtubeChannel.ChannelUsername = details[1]?.Trim();
                                string subsCount = Regex.Match(details[2]?.Trim(), "[\\d]")?.Value;
                                youtubeChannel.SubscriberCount = string.IsNullOrEmpty(subsCount) ? "0" : subsCount;
                                break;
                        }
                        youtubeChannel.ChannelUrl = "https://www.youtube.com/" + youtubeChannel.ChannelUsername;
                        ListChannels.Add(youtubeChannel);
                    }
                    catch (Exception) { }

                }

            }
            catch (Exception e) { e.DebugLog(); }
        }

        public List<YoutubeChannel> ListChannels { get; set; } = new List<YoutubeChannel>();
    }
}