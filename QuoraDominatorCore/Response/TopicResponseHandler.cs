using DominatorHouseCore.Interfaces;
using System.Collections.Generic;
using DominatorHouseCore.Utility;
using System;
using QuoraDominatorCore.QdUtility;
using DominatorHouseCore;
using QuoraDominatorCore.Models;

namespace QuoraDominatorCore.Response
{
    public class TopicResponseHandler : QuoraResponseHandler
    {
        public List<TopicDetails> TopicsCollection { get; set; } = new List<TopicDetails>();
        public bool HasMoreResult { get; set; }
        public int PaginationCount { get; set; }
        public TopicResponseHandler(IResponseParameter response,bool IsBrowser=true) : base(response)
        {
            try
            {
                if(IsBrowser)
                {
                    response.Response = QdConstants.GetJsonForAllTypePosts(response.Response.Replace("\\\"", "\"").Replace("\\\"", "\""), "searchConnection");
                    //var topicLink = HtmlUtility.GetListNodesFromClassName(response.Response, "q-box Link___StyledBox-t2xg9c-0 hEoqSM puppeteer_test_link qu-cursor--pointer qu-hover--textDecoration--none", "a");
                    //foreach (HtmlNode link in topicLink)
                    //{
                    //    string url = link.Attributes[1].Value;
                    //    if (!string.IsNullOrEmpty(url)) TopicsCollection.Add(new TopicDetails { TopicUrl = url });
                    //}
                }
                var jsonObject = jsonHandler.ParseJsonToJObject(response.Response);
                var Data = jsonHandler.GetJTokenOfJToken(jsonObject, "data", "searchConnection");
                bool.TryParse(jsonHandler.GetJTokenValue(Data, "pageInfo", "hasNextPage"), out bool hasMoreResult);
                HasMoreResult = hasMoreResult;
                int.TryParse(jsonHandler.GetJTokenValue(Data, "pageInfo", "endCursor"), out int paginationCount);
                PaginationCount = paginationCount;
                var Topics = jsonHandler.GetJArrayElement(jsonHandler.GetJTokenValue(Data, "edges"));
                if (Topics != null && Topics.HasValues)
                    Topics.ForEach(topic =>
                    {
                        var url = QdConstants.HomePageUrl + jsonHandler.GetJTokenValue(topic, "node", "topic", "url");
                        int.TryParse(jsonHandler.GetJTokenValue(topic, "node", "topic", "numFollowers"), out int followerCount);
                        bool.TryParse(jsonHandler.GetJTokenValue(topic, "node", "topic", "isFollowing"), out bool isFollowing);
                        bool.TryParse(jsonHandler.GetJTokenValue(topic, "node", "topic", "isSensitive"), out bool isSensitive);
                        if (!string.IsNullOrEmpty(url))
                            TopicsCollection.Add(new TopicDetails
                            {
                                TopicUrl = url,
                                TopicId = jsonHandler.GetJTokenValue(topic, "node", "topic", "tid"),
                                TopicName = jsonHandler.GetJTokenValue(topic, "node", "topic", "name"),
                                TopicProfilePicUrl = jsonHandler.GetJTokenValue(topic, "node", "topic", "photoUrl"),
                                IsFollowing = isFollowing,
                                IsSensitive = isSensitive,
                                FollowerCount = followerCount
                            });
                    });

            }
            catch(Exception ex){ ex.DebugLog(); };
        }
    }
}