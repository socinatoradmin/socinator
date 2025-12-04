using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using RedditDominatorCore.RDUtility;
using RedditDominatorCore.ReportModel;
using System;
using System.Collections.Generic;

namespace RedditDominatorCore.Response
{
    public class CommunitiesScraperResponseHandler : RdResponseHandler
    {
        public List<CommunitiesModel> LstCommunities = new List<CommunitiesModel>();

        public CommunitiesScraperResponseHandler(IResponseParameter response) : base(response)
        {
            try
            {
                //Response = response.Response;
                //if (!Success)
                //    return;

                //var jResponse = response.Response;
                //var jObject = JObject.Parse(jResponse);

                //IEnumerable<JToken> jData = jObject["subreddits"].Children();
                //NewPageResponse(jData);
                var jsonData = Utils.GetCommunityJson(response.Response);
                if (!string.IsNullOrEmpty(jsonData))
                {
                    var communities = jsonHandler.GetJArrayElement(jsonData);
                    if (communities != null && communities.HasValues)
                    {
                        communities.ForEach(community =>
                        {
                            LstCommunities.Add(new CommunitiesModel
                            {
                                CommunityId = jsonHandler.GetJTokenValue(community, "id"),
                                Url = jsonHandler.GetJTokenValue(community, "prefixedName"),
                                DisplayText = jsonHandler.GetJTokenValue(community, "prefixedName"),
                                CommunityIcon = jsonHandler.GetJTokenValue(community, "styles", "icon")
                            }); ;
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public string BookMark { get; set; }
        public bool HasMoreResults { get; set; }

        //private void NewPageResponse(IEnumerable<JToken> jSonData)
        //{
        //    try
        //    {
        //        foreach (var data in jSonData)
        //        {
        //            var community = new CommunitiesModel {CommunityId = data.First["id"].ToString()};
        //            community.WhitelistStatus = data.First["whitelistStatus"].ToString();
        //            var num1 = Convert.ToBoolean(data.First["isNSFW"].ToString()) ? 1 : 0;
        //            community.IsNsfw = num1 != 0;
        //            community.Subscribers = (int) data.First["subscribers"];
        //            community.PrimaryColor = data.First["primaryColor"].ToString();
        //            var num2 = Convert.ToBoolean(data.First["isQuarantined"].ToString()) ? 1 : 0;
        //            community.IsQuarantined = num2 != 0;
        //            community.Name = data.First["name"].ToString();
        //            community.Title = data.First["title"].ToString();
        //            community.Url = data.First["url"].ToString();
        //            community.DisplayText = data.First["displayText"].ToString();
        //            community.Type = data.First["type"].ToString();
        //            LstCommunities.Add(community);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.DebugLog();
        //    }
        //}
    }
}