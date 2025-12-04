using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDResponse.BaseResponse;
using FaceDominatorCore.Interface;
using FaceDominatorCore.Utility;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace FaceDominatorCore.FDResponse.BrowserResponseHandler.UserResponseHandler
{

    public class SearchPeopleResponseHandler : FdResponseHandler, IResponseHandler
    {

        public bool HasMoreResults { get; set; }

        public string EntityId { get; set; }


        public string PageletData { get; set; }


        public bool Status { get; set; }


        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; }

        public SearchPeopleResponseHandler(IResponseParameter responseParameter, bool status) : base(responseParameter)
        {
            Status = status;
        }

        public SearchPeopleResponseHandler(List<string> listJsonData, IResponseParameter responseParameter,
            List<string> listFacebookUserIds, bool hasmoreResult = false,
             FbEntityType entityType = FbEntityType.User) : base(responseParameter)
        {
            ObjFdScraperResponseParameters = new FdScraperResponseParameters();

            ObjFdScraperResponseParameters.ListUser = new List<FacebookUser>();
            try
            {
                foreach (var postResponse in listJsonData)
                {
                    JObject jObject = null;
                    JArray fdjArray = new JArray();
                    var elements = new JArray();
                    try
                    {
                        if (!postResponse.IsValidJson())
                        {
                            var decodedResponse = "";
                            if (postResponse.StartsWith("<!DOCTYPE html>"))
                            {
                                var splittedArray = "";
                                try
                                {
                                    if (entityType == FbEntityType.GroupMembers)
                                        splittedArray = Utilities.GetBetween(Regex.Split(postResponse, "</script>").SingleOrDefault(x => x.Contains("\"__typename\":\"User\"") && (x.Contains("all_active_members\":{\"count") || x.Contains("group_search_people_placeholder\":\"Find a member"))) + "/end", "{\"require\"", "/end");
                                    else
                                        splittedArray = Utilities.GetBetween(Regex.Split(postResponse, "</script>").SingleOrDefault(x => x.Contains("{\"role\":\"ENTITY_USER\"")) + "/end", "{\"require\"", "/end");
                                    decodedResponse = "{\"require\"" + splittedArray;
                                }
                                catch (Exception)
                                {
                                    if (entityType == FbEntityType.GroupMembers)
                                        splittedArray = Utilities.GetBetween(Regex.Split(postResponse, "</script>").FirstOrDefault(x => x.Contains("\"__typename\":\"User\"") && (x.Contains("all_active_members\":{\"count") || x.Contains("group_search_people_placeholder\":\"Find a member"))) + "/end", "{\"require\"", "/end");
                                    else
                                        splittedArray = Utilities.GetBetween(Regex.Split(postResponse, "</script>").FirstOrDefault(x => x.Contains("{\"role\":\"ENTITY_USER\"")) + "/end", "{\"require\"", "/end");
                                    decodedResponse = "{\"require\"" + splittedArray;
                                }
                                if (decodedResponse.IsValidJson())
                                    jObject = parser.ParseJsonToJObject(decodedResponse);
                            }
                            else
                                decodedResponse = "[" + postResponse.Replace("}}}}\r\n{\"label\":", "}}}},\r\n{\"label\":") + "]";
                            fdjArray = parser.GetJArrayElement(decodedResponse);
                        }
                        else
                        {
                            fdjArray = parser.GetJArrayElement(postResponse);
                            if (fdjArray.Count() == 0)
                                jObject = parser.ParseJsonToJObject(postResponse);
                        }
                        if (jObject != null && jObject.Count > 0)
                        {
                            var totalcount = parser.GetJTokenValue(jObject, "data", "viewer", "all_friends_data", "count");
                            if (string.IsNullOrEmpty(totalcount))
                                totalcount = JsonSearcher.FindStringValueByKey(JsonSearcher.FindByKey(jObject, "all_friends_data"), "count");
                            if (entityType == FbEntityType.GroupMembers)
                                totalcount = JsonSearcher.FindStringValueByKey(JsonSearcher.FindByKey(jObject, "all_active_members"), "count");
                            if (!string.IsNullOrEmpty(totalcount))
                                ObjFdScraperResponseParameters.TotalCount = totalcount;
                            var nodes = parser.GetJTokenOfJToken(jObject, "data", "serpResponse", "results", "edges");
                            if (nodes == null || nodes.Count() == 0)
                                nodes = parser.GetJTokenOfJToken(JsonSearcher.FindByKey(jObject, "data"), "serpResponse", "results", "edges");
                            if (nodes == null || nodes.Count() == 0)
                                nodes = parser.GetJTokenOfJToken(jObject, "data", "viewer", "people_you_may_know", "edges");
                            if (nodes == null || nodes.Count() == 0)
                                nodes = parser.GetJTokenOfJToken(jObject, "data", "viewer", "friending_possibilities", "edges");
                            if (nodes == null || nodes.Count() == 0)
                                nodes = parser.GetJTokenOfJToken(jObject, "data", "viewer", "outgoing_friend_requests_connection", "edges");
                            if (nodes == null || nodes.Count() == 0)
                                nodes = parser.GetJTokenOfJToken(jObject, "data", "user", "friends", "edges");
                            if (nodes == null || nodes.Count() == 0)
                                nodes = JsonSearcher.FindByKey(JsonSearcher.FindByKey(JsonSearcher.FindByKey(JsonSearcher.FindByKey(jObject, "data"), "all_collections"), "pageItems"), "edges");
                            if (nodes == null || nodes.Count() == 0)
                                nodes = JsonSearcher.FindByKey(JsonSearcher.FindByKey(JsonSearcher.FindByKey(jObject, "data"), "pageItems"), "edges");

                            if (entityType == FbEntityType.GroupMembers && nodes.Count() == 0 || entityType == FbEntityType.UserGreetings && nodes.Count() == 0)
                            {
                                JArray nodesArray = new JArray();
                                if (entityType == FbEntityType.GroupMembers)
                                {
                                    nodesArray.Add(JsonSearcher.FindByKey(JsonSearcher.FindByKey(jObject, "group_admin_profiles"), "edges"));
                                    nodesArray.Add(JsonSearcher.FindByKey(JsonSearcher.FindByKey(jObject, "paginated_member_sections"), "edges"));
                                    nodesArray.Add(JsonSearcher.FindByKey(JsonSearcher.FindByKey(jObject, "new_members"), "edges"));
                                    nodesArray.Add(JsonSearcher.FindByKey(JsonSearcher.FindByKey(jObject, "group_friend_members"), "edges"));
                                    nodesArray.Add(JsonSearcher.FindByKey(JsonSearcher.FindByKey(jObject, "group_member_discovery"), "edges"));
                                }
                                if (entityType == FbEntityType.UserGreetings)
                                {
                                    nodesArray.Add(JsonSearcher.FindByKey(JsonSearcher.FindByKey(JsonSearcher.FindByKey(jObject, "viewer"), "all_friends"), "edges"));
                                    var monthArrays = JsonSearcher.FindByKey(JsonSearcher.FindByKey(JsonSearcher.FindByKey(jObject, "viewer"), "all_friends_by_birthday_month"), "edges");
                                    foreach (var month in monthArrays)
                                    {
                                        nodesArray.Add(JsonSearcher.FindByKey(JsonSearcher.FindByKey(month, "friends"), "edges"));
                                    }

                                }
                                foreach (var x in nodesArray)
                                {
                                    if (x.Count() != 0)
                                    {
                                        if (nodes.Last != null)
                                            x.ForEach(y => nodes.Last.AddAfterSelf(y));
                                        else
                                            nodes = x;
                                    }
                                }
                            }
                            if (nodes == null || nodes.Count() == 0)
                                nodes = JsonSearcher.FindByKey(jObject, "edges");
                            if (nodes == null || nodes.Count() == 0)
                                nodes = JsonSearcher.FindByKey(jObject, "node");
                            if (nodes != null || nodes.Count() != 0)
                            {
                                if (nodes.Type == JTokenType.Array)
                                    nodes.ForEach(z => elements.Add(z));
                                else
                                    elements.Add(nodes);
                            }
                        }
                        foreach (var node in fdjArray)
                        {
                            var nodes = parser.GetJTokenOfJToken(node, "data", "serpResponse", "results", "edges");
                            nodes = parser.GetJTokenOfJToken(node, "data", "feedback", "reshares", "edges");
                            if (nodes == null || nodes.Count() == 0)
                                nodes = parser.GetJTokenOfJToken(node, "data", "node");
                            if (entityType == FbEntityType.GroupMembers && nodes.Count() == 0 || entityType == FbEntityType.UserGreetings && nodes.Count() == 0)
                            {
                                JArray nodesArray = new JArray();
                                if (entityType == FbEntityType.GroupMembers)
                                {
                                    nodesArray.Add(JsonSearcher.FindByKey(JsonSearcher.FindByKey(node, "group_admin_profiles"), "edges"));
                                    nodesArray.Add(JsonSearcher.FindByKey(JsonSearcher.FindByKey(node, "paginated_member_sections"), "edges"));
                                    nodesArray.Add(JsonSearcher.FindByKey(JsonSearcher.FindByKey(node, "new_members"), "edges"));
                                    nodesArray.Add(JsonSearcher.FindByKey(JsonSearcher.FindByKey(node, "group_friend_members"), "edges"));
                                    nodesArray.Add(JsonSearcher.FindByKey(JsonSearcher.FindByKey(node, "group_member_discovery"), "edges"));
                                }
                                if (entityType == FbEntityType.UserGreetings)
                                {
                                    nodesArray.Add(JsonSearcher.FindByKey(JsonSearcher.FindByKey(JsonSearcher.FindByKey(node, "viewer"), "all_friends"), "edges"));
                                    var monthArrays = JsonSearcher.FindByKey(JsonSearcher.FindByKey(JsonSearcher.FindByKey(node, "viewer"), "all_friends_by_birthday_month"), "edges");
                                    foreach (var month in monthArrays)
                                    {
                                        nodesArray.Add(JsonSearcher.FindByKey(JsonSearcher.FindByKey(month, "friends"), "edges"));
                                    }

                                }
                                foreach (var x in nodesArray)
                                {
                                    if (x.Count() != 0)
                                    {
                                        if (nodes.Last != null)
                                            x.ForEach(y => nodes.Last.AddAfterSelf(y));
                                        else
                                            nodes = x;
                                    }
                                }
                            }
                            if (nodes != null || nodes.Count() != 0)
                            {
                                if (nodes.Type == JTokenType.Array)
                                    nodes.ForEach(z => elements.Add(z));
                                else
                                    elements.Add(nodes);
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                    foreach (var userObject in elements)
                    {
                        var storyObject = userObject;
                        var nodeObject = JsonSearcher.FindByKey(userObject, "node");
                        if (nodeObject != null && nodeObject.Count() > 0 && (JsonSearcher.FindStringValueByKey(nodeObject, "__typename") == "User" || JsonSearcher.FindStringValueByKey(nodeObject, "__typename") == "RestrictedUser"))
                            storyObject = nodeObject;
                        var _username = parser.GetJTokenValue(storyObject, "relay_rendering_strategy", "view_model", "profile", "name");
                        if (string.IsNullOrEmpty(_username))
                            _username = JsonSearcher.FindStringValueByKey(storyObject, "name");
                        if (string.IsNullOrEmpty(_username))
                            _username = JsonSearcher.FindStringValueByKey(JsonSearcher.FindByKey(storyObject, "title"), "text");
                        var _userProfilePicUrl = parser.GetJTokenValue(storyObject, "relay_rendering_strategy", "view_model", "profile", "profile_picture", "uri");
                        if (string.IsNullOrEmpty(_userProfilePicUrl))
                            _userProfilePicUrl = JsonSearcher.FindStringValueByKey(JsonSearcher.FindByKey(storyObject, "profile_picture"), "uri");
                        if (string.IsNullOrEmpty(_userProfilePicUrl))
                            _userProfilePicUrl = JsonSearcher.FindStringValueByKey(storyObject, "uri");
                        if (string.IsNullOrEmpty(_userProfilePicUrl))
                            _username = JsonSearcher.FindStringValueByKey(JsonSearcher.FindByKey(storyObject, "image"), "uri");
                        var _gender = parser.GetJTokenValue(storyObject, "relay_rendering_strategy", "view_model", "profile", "gender");
                        if (string.IsNullOrEmpty(_gender))
                            _gender = JsonSearcher.FindStringValueByKey(storyObject, "gender");
                        var _userId = parser.GetJTokenValue(storyObject, "relay_rendering_strategy", "view_model", "profile", "id");
                        if (string.IsNullOrEmpty(_userId))
                            _userId = JsonSearcher.FindStringValueByKey(storyObject, "id");
                        if (!string.IsNullOrEmpty(_userId) && StringHelper.IsBase64String(_userId))
                            _userId = FdFunctions.GetIntegerOnlyString(StringHelper.Base64Decode(_userId).Split(':').LastOrDefault());
                        var _userUrl = parser.GetJTokenValue(storyObject, "relay_rendering_strategy", "view_model", "profile", "url");
                        if (string.IsNullOrEmpty(_userUrl))
                            _userUrl = JsonSearcher.FindStringValueByKey(storyObject, "url");
                        if (string.IsNullOrEmpty(_userUrl) && !string.IsNullOrEmpty(_userId))
                            _userUrl = FdConstants.FbHomeUrl + _userId;
                        var _isFriend = parser.GetJTokenValue(storyObject, "relay_rendering_strategy", "view_model", "profile", "friendship_closeness");
                        if (string.IsNullOrEmpty(_isFriend)) _isFriend = JsonSearcher.FindStringValueByKey(storyObject, "friendship_closeness");
                        var _canRequest = JsonSearcher.FindStringValueByKey(storyObject, "friendship_status");
                        var socialContext = JsonSearcher.FindStringValueByKey(storyObject, "social_context_top_mutual_friends");
                        if (string.IsNullOrEmpty(socialContext))
                            socialContext = JsonSearcher.FindStringValueByKey(storyObject, "social_context");
                        var mutualFriendCount = FdFunctions.GetIntegerOnlyString(socialContext);
                        var birthDateToken = JsonSearcher.FindByKey(storyObject, "birthdate");
                        var birthDate = JsonSearcher.FindStringValueByKey(birthDateToken, "text");
                        if (string.IsNullOrEmpty(birthDate))
                        {
                            int.TryParse(JsonSearcher.FindStringValueByKey(birthDateToken, "month"), out int monthNum);
                            if (monthNum > 0 && monthNum < 13)
                                birthDate = JsonSearcher.FindStringValueByKey(birthDateToken, "day") + " " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(monthNum) + " " + JsonSearcher.FindStringValueByKey(birthDateToken, "year");
                        }
                        if (!string.IsNullOrEmpty(_userId) && !ObjFdScraperResponseParameters.ListUser.Any(x => x.UserId == _userId))
                            ObjFdScraperResponseParameters.ListUser.Add(new FacebookUser()
                            {
                                Username = _username,
                                Familyname = _username,
                                FullName = _username,
                                Gender = _gender,
                                UserId = _userId,
                                ProfileUrl = _userUrl,
                                ProfileId = !_userUrl.Contains(_userId) ? Utilities.GetBetween(_userUrl + "/", FdConstants.FbHomeUrl, "/") : _userId,
                                ProfilePicUrl = _userProfilePicUrl,
                                ScrapedProfileUrl = _userUrl,
                                IsAlreadyFriend = string.IsNullOrEmpty(_isFriend) || string.IsNullOrEmpty(_canRequest) ? "" : _isFriend.Equals("NOT_FRIENDED") || !_canRequest.Contains("ARE_FRIENDS") ? "false" : "true",
                                CanSendFriendRequest = _canRequest.Equals("CAN_REQUEST") ? "true" : "",
                                HasMutualFriends = socialContext.Contains("mutual friends") && mutualFriendCount != "0" ? true : false,
                                IsverifiedUser = JsonSearcher.FindStringValueByKey(storyObject, "is_verified").ToLower().Contains("true") ? true : false,
                                DateOfBirth = birthDate
                            });
                    }

                }

            }
            catch (Exception)
            { }

            Status = ObjFdScraperResponseParameters.ListUser.Count > 0;
            HasMoreResults = hasmoreResult;
        }
    }
}
