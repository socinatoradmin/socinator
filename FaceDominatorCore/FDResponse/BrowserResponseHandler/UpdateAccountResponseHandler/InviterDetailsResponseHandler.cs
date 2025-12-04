using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDResponse.BaseResponse;
using FaceDominatorCore.Interface;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FaceDominatorCore.FDResponse.BrowserResponseHandler.UpdateAccountResponseHandler
{
    public class InviterDetailsResponseHandler : FdResponseHandler, IResponseHandler
    {

        public bool HasMoreResults { get; set; }

        public string EntityId { get; set; }


        public string PageletData { get; set; }


        public bool Status { get; set; }


        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; }


        public InviterDetailsResponseHandler(IResponseParameter responseParameter, bool isClassicUi = true, bool isMentionFriend = false)
            : base(responseParameter)
        {

            if (responseParameter.HasError)
                return;

            try
            {
                ObjFdScraperResponseParameters = new FdScraperResponseParameters();

                ObjFdScraperResponseParameters.ListUser = new List<FacebookUser>();

                var decodedResponse = responseParameter.Response;

                if (isMentionFriend)
                    GetFriendMentionList(decodedResponse);
                else
                    GetFriendsList(decodedResponse);
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
                JToken friendList;

                var decodedResponse2 = Regex.Replace(decodedResponse, "for \\(;;\\);", string.Empty);

                JObject jObject = JObject.Parse(decodedResponse2);

                if (decodedResponse.Contains("not_invited_friends"))
                    friendList = jObject["data"]["node"]["not_invited_friends"]["edges"];
                else if (decodedResponse.Contains("comet_group_typeahead_search"))
                    friendList = jObject["data"]["comet_group_typeahead_search"];
                else
                    friendList = jObject["data"]["user"]["friends"]["edges"];

                ObjFdScraperResponseParameters.ListUser = (from token in friendList
                                                           let userId = token["node"]["id"]
                                                           let name = token["node"]["name"]
                                                           let photo = token["node"]["photo"]["uri"]
                                                           let url = token["node"]["id"]
                                                           select new FacebookUser
                                                           {
                                                               UserId = userId.ToString(),
                                                               Familyname = name.ToString(),
                                                               ProfileUrl = $"https://www.facebook.com/{url.ToString()}",
                                                               ProfilePicUrl = Regex.Replace(Regex.Replace
                                                                    (photo.ToString(), "_nc_ohc=(.*?)&", string.Empty), "oh=(.*?)&", string.Empty)
                                                           }).ToList();

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void GetFriendMentionList(string decodedResponse)
        {
            try
            {
                JToken friendList;

                var decodedResponse2 = Regex.Replace(decodedResponse, "for \\(;;\\);", string.Empty);

                JObject jObject = JObject.Parse(decodedResponse2);

                friendList = jObject["data"]["comet_composer_typeahead_search"];

                ObjFdScraperResponseParameters.ListUser = (from token in friendList
                                                           let userId = token["node"]["id"]
                                                           let name = token["node"]["name"]
                                                           let photo = token["node"]["photo"]["uri"]
                                                           let url = token["node"]["id"]
                                                           select new FacebookUser
                                                           {
                                                               UserId = userId.ToString(),
                                                               Familyname = name.ToString(),
                                                               ProfileUrl = $"https://www.facebook.com/{url.ToString()}",
                                                               ProfilePicUrl = Regex.Replace(Regex.Replace
                                                                    (photo.ToString(), "_nc_ohc=(.*?)&", string.Empty), "oh=(.*?)&", string.Empty)
                                                           }).ToList();

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }


    }
}
