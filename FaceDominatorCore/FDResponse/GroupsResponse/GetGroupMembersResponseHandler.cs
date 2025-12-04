using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDResponse.BaseResponse;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace FaceDominatorCore.FDResponse.GroupsResponse
{
    public class GetGroupMembersResponseHandler : FdResponseHandler
    {

        public List<string> ListMember = new List<string>();

        public bool IsGroupAdmin = false;

        public GetGroupMembersResponseHandler(IResponseParameter responseParameter)
            : base(responseParameter)
        {

            if (responseParameter.HasError)
                return;

            try
            {

                JObject.Parse(responseParameter.Response)["data"]["group"]["group_member_profiles"]["edges"].ForEach(userValue =>
                    ListMember.Add(userValue["node"]["id"].ToString()));

            }
            catch (Exception)
            {

            }
        }
    }
}
