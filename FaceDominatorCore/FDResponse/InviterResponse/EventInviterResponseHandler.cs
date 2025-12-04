
using DominatorHouseCore.Interfaces;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDResponse.BaseResponse;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaceDominatorCore.FDResponse.InviterResponse
{

    public class EventInviterResponseHandler : FdResponseHandler
    {

        public EventInviterResponseHandler(IResponseParameter responseParameter)
            : base(responseParameter)
        {
            if (responseParameter.HasError)
                return;


        }

        public string GetInvitableCount(List<string> inviteeList)
        {
            var inviteeValues = inviteeList.FirstOrDefault(x => x.Contains("invitee_candidates_v2"));
            var jObject = JObject.Parse(inviteeValues);
            return jObject["data"]["event"]["invitee_candidates_v2"]["count"].ToString();
        }

        public int SelectionCount(List<string> inviteeList, FacebookUser objFacebookUser)
        {
            int count = 1;
            var inviteeValues = inviteeList.LastOrDefault();
            var jObject = JObject.Parse(inviteeValues.Replace("for (;;);", string.Empty));

            foreach (var objectValue in jObject["payload"]["entries"])
            {
                if (objectValue.ToString().Contains(objFacebookUser.UserId))
                    break;
                count++;
            }
            return count;
        }

        public List<FacebookUser> GetFacebookUser(string friendResponse)
        {
            List<FacebookUser> objFacebookUserList = new List<FacebookUser>();

            JObject jObject = JObject.Parse(friendResponse);
            var friendList = jObject["data"]["event"]["invitable_entries_search"]["nodes"];

            objFacebookUserList = (from token in friendList
                                   let userId = token["token"]
                                   let name = token["title"]
                                   let profilePic = token["photo_source"]["uri"]
                                   where userId != null && name != null
                                   select new FacebookUser
                                   {
                                       UserId = userId.ToString(),
                                       Familyname = name.ToString(),
                                       ProfileUrl = $"{FdConstants.FbHomeUrl}{userId}",
                                       InteractionDate = DateTime.Now.ToString(),
                                       ProfilePicUrl = profilePic.ToString()
                                   }).ToList();

            return objFacebookUserList;
        }
    }
}

