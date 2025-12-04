using System;
using System.Collections.Generic;
using System.Linq;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;

namespace QuoraDominatorCore.Response
{
    public class TopicFollowersResponseHandler : QuoraResponseHandler
    {
        public List<string> TopicFollowers = new List<string>();

        public TopicFollowersResponseHandler(IResponseParameter response) : base(response)
        {
            if (RespJ == null)
            {
                try
                {
                    var nodes = HtmlDocument.DocumentNode.SelectNodes(
                        "//span[starts-with(@class, 'photo_tooltip')]//a");

                    foreach (var item in nodes)
                    {
                        var username = item.Attributes["href"].Value.Replace("/profile/", "");
                        TopicFollowers.Add(username);
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
            else
            {
                var followers = HtmlDocument.DocumentNode.SelectNodes("//a[@class='user']").ToArray();
                foreach (var follower in followers)
                {
                    var username = follower.Attributes["href"].Value.Replace("'/profile/", "");
                    TopicFollowers.Add(username);
                }
            }
        }
    }
}