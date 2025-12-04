using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using FaceDominatorCore.FDResponse.BaseResponse;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaceDominatorCore.FDResponse.GroupsResponse
{
    public class GetFriendsJoiningStatusResponseHandler : FdResponseHandler
    {
        public List<string> memberList = new List<string>();
        public GetFriendsJoiningStatusResponseHandler(IResponseParameter responseParameter)
            : base(responseParameter)
        {
            if (responseParameter.HasError)
                return;

            try
            {
                if (responseParameter.Response.StartsWith("for (;;);"))
                {
                    JObject jObject = JObject.Parse(responseParameter.Response.Replace("for (;;);", string.Empty));
                    var totalFriends = jObject["payload"]["entries"];

                    for (int n = 0; n < totalFriends.Count(); n++)
                    {
                        try
                        {
                            string renderType = jObject["payload"]["entries"][n]["render_type"] == null
                                ? string.Empty
                                : jObject["payload"]["entries"][n]["render_type"].ToString();

                            string uId = jObject["payload"]["entries"][n]["uid"] == null
                                ? string.Empty
                                : jObject["payload"]["entries"][n]["uid"].ToString();

                            if (renderType.ToLower() == "member")
                                memberList.Add(uId);
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}
