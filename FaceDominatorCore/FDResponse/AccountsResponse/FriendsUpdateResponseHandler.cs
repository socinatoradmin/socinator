using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDResponse.BaseResponse;
using FaceDominatorCore.Interface;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaceDominatorCore.FDResponse.AccountsResponse
{

    public class FriendsUpdateResponseHandler : FdResponseHandler, IResponseHandler
    {

        public bool HasMoreResults { get; set; } = true;
        public string EntityId { get; set; }
        public string PageletData { get; set; }
        public bool Status { get; set; }
        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; }
            = new FdScraperResponseParameters();

        public FriendsUpdateResponseHandler(IResponseParameter responseParameter, bool friendsPage)
            : base(responseParameter)
        {
            if (responseParameter.HasError)
                return;
            ObjFdScraperResponseParameters.ListUser = new List<FacebookUser>();
            var decodedResponse = responseParameter.Response;

            try
            {
                if (!friendsPage)
                {
                    decodedResponse = decodedResponse.Replace("for (;;);", string.Empty);
                    if (!string.IsNullOrEmpty(decodedResponse))
                        GetFriendsList(decodedResponse);

                    if (ObjFdScraperResponseParameters.ListUser.Count <= 0)
                    {
                        GetFriendsListNewUi(decodedResponse);
                    }

                }
                else
                {
                    if (!string.IsNullOrEmpty(decodedResponse))
                        GetFriendsListFromPageLikers(decodedResponse);
                }

                if (ObjFdScraperResponseParameters.ListUser?.Count == 0)
                {
                    JObject jObject = JObject.Parse(decodedResponse);
                    var friendList = jObject["payload"]["entries"];
                    ObjFdScraperResponseParameters.ListUser = (from token in friendList
                                                               let userId = token["uid"]
                                                               let name = token["names"][0]
                                                               let invitestatus = token["inviteStatus"]
                                                               where userId != null && name != null
                                                               select new FacebookUser
                                                               {
                                                                   UserId = userId.ToString(),
                                                                   Familyname = name.ToString(),
                                                                   ProfileUrl = FdConstants.FbHomeUrl + userId,
                                                                   InviteStatus = invitestatus == null ? InviteStatus.Pending : (invitestatus.ToString().Contains("not_invited") ? InviteStatus.Notinvited : (invitestatus.ToString().Contains("pending") ? InviteStatus.Pending : InviteStatus.Liked)),
                                                                   InteractionDate = DateTime.Now.ToString()
                                                               }).ToList();
                }


            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

        }

        private void GetFriendsListNewUi(string decodedResponse)
        {
            try
            {

                var ddd = "{\"entries\":" + FDLibrary.FdFunctions.FdRegexUtility.FirstMatchExtractor(decodedResponse, "{\"entries\":(.*?)display_ttl\":0}") + "display_ttl\":0}";

                JObject jObject = JObject.Parse(ddd);

                var friendList = jObject["entries"];
                ObjFdScraperResponseParameters.ListUser = (from token in friendList
                                                           let userId = token["uid"]
                                                           let name = token["text"]
                                                           let invitestatus = token["inviteStatus"]
                                                           where userId != null && name != null
                                                           select new FacebookUser
                                                           {
                                                               UserId = userId.ToString(),
                                                               Familyname = name.ToString(),
                                                               ProfileUrl = FdConstants.FbHomeUrl + userId,
                                                               InviteStatus = invitestatus == null ? InviteStatus.Pending : (invitestatus.ToString().Contains("not_invited") ? InviteStatus.Notinvited : (invitestatus.ToString().Contains("pending") ? InviteStatus.Pending : InviteStatus.Liked)),
                                                               InteractionDate = DateTime.Now.ToString()
                                                           }).ToList();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void GetFriendsListFromPageLikers(string decodedResponse)
        {
            try
            {

                JObject jObject = JObject.Parse(decodedResponse);

                var friendList = jObject["data"]["page"]["friends_who_like"]["edges"];

                ObjFdScraperResponseParameters.ListUser = (from token in friendList
                                                           let userId = token["node"]["id"]
                                                           let name = token["node"]["name"]
                                                           select new FacebookUser
                                                           {
                                                               UserId = userId.ToString(),
                                                               Familyname = name.ToString(),
                                                               ProfileUrl = FdConstants.FbHomeUrl + userId
                                                           }).ToList();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void GetFriendsList(string decodedResponse)
        {
            try
            {
                JObject jObject = JObject.Parse(decodedResponse);

                var friendList = jObject["jsmods"]["require"][0][3][0]["props"]["pageInviteDialogV2ContainerServerCallableProps"]["friendList"];

                ObjFdScraperResponseParameters.ListUser = (from token in friendList
                                                           let userId = token["id"]
                                                           let name = token["name"]
                                                           let invitestatus = token["inviteStatus"]
                                                           where userId != null && name != null
                                                           select new FacebookUser
                                                           {
                                                               UserId = userId.ToString(),
                                                               Familyname = name.ToString(),
                                                               ProfileUrl = FdConstants.FbHomeUrl + userId,
                                                               InviteStatus = invitestatus == null ? InviteStatus.Pending : (invitestatus.ToString().Contains("not_invited") ? InviteStatus.Notinvited : (invitestatus.ToString().Contains("pending") ? InviteStatus.Pending : InviteStatus.Liked)),
                                                               InteractionDate = DateTime.Now.ToString()
                                                           }).ToList();

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            //var z = ListSentFriendId.FirstOrDefault(x => x.UserId == "100007464069871");

        }
    }
}
