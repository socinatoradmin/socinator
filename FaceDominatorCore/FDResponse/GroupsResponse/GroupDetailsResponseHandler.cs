using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDResponse.BaseResponse;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace FaceDominatorCore.FDResponse.GroupsResponse
{
    public class GroupDetailsResponseHandler : FdResponseHandler
    {
        GroupDetails GroupDetails { get; }

        public GroupDetailsResponseHandler(IResponseParameter responseParameter)
            : base(responseParameter)
        {
            string jsonValue = "{bootloadable:{\"B" + Utilities.GetBetween(responseParameter.Response, "{bootloadable:{\"B", ");}),");
            try
            {
                var jResp = JObject.Parse(jsonValue);
                JToken jTokens = jResp["jsmods"]["require"][0][3][1];
                foreach (JToken jtoken in jTokens)
                {
                    GroupDetails = new GroupDetails
                    {
                        GroupId = jtoken["id"]?.ToString() == null ? string.Empty : jtoken["id"]?.ToString(),
                        GroupName = jtoken["name"]?.ToString() == null ? string.Empty : jtoken["name"]?.ToString()
                    };
                    GroupDetails.GroupUrl = FdConstants.FbHomeUrl + GroupDetails.GroupId;

                    ListGroupDetails.Add(GroupDetails);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

        }
        public List<GroupDetails> ListGroupDetails = new List<GroupDetails>();
    }
}
