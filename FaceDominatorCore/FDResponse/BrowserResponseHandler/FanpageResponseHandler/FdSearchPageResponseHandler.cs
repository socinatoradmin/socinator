using DominatorHouseCore;
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

namespace FaceDominatorCore.FDResponse.BrowserResponseHandler.FanpageResponseHandler
{
    public class FdSearchPageResponseHandler : FdResponseHandler, IResponseHandler
    {
        public string EntityId { get; set; }

        public bool HasMoreResults { get; set; }

        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; }

        public string PageletData { get; set; }

        public bool Status { get; set; }

        public FdSearchPageResponseHandler(IResponseParameter responseParameter, List<string> listJsonData, bool hasMoreResults,
          FbEntityType entityType = FbEntityType.Fanpage) : base(responseParameter)
        {
            ObjFdScraperResponseParameters = new FdScraperResponseParameters();

            ObjFdScraperResponseParameters.ListPage = new List<FanpageDetails>();

            try
            {
                foreach (var postResponse in listJsonData)
                {
                    JObject jObject = new JObject();
                    JArray fdjArray = new JArray();
                    var elements = new JArray();
                    try
                    {
                        if (!postResponse.IsValidJson())
                        {
                            var decodedResponse = "";
                            if (postResponse.StartsWith("<!DOCTYPE html>"))
                            {
                                var splittedArrayString = "";
                                try
                                {
                                    if (entityType == FbEntityType.Places)
                                        splittedArrayString = Utilities.GetBetween(Regex.Split(postResponse, "</script>").SingleOrDefault(x => x.Contains("\"node\":{\"role\":\"ENTITY_PLACES\"")) + "/end", "{\"require\"", "/end");
                                    else
                                        splittedArrayString = Utilities.GetBetween(Regex.Split(postResponse, "</script>").SingleOrDefault(x => x.Contains("data\":{\"viewer\":{\"has_biz_web_access\"")) + "/end", "{\"require\"", "/end");
                                    if (string.IsNullOrEmpty(splittedArrayString))
                                        splittedArrayString = Utilities.GetBetween(Regex.Split(postResponse, "</script>").SingleOrDefault(x => x.Contains("\"role\":\"ENTITY_PAGES\"")) + "/end", "{\"require\"", "/end");
                                }
                                catch (Exception)
                                {

                                    if (entityType == FbEntityType.Places)
                                        splittedArrayString = Utilities.GetBetween(Regex.Split(postResponse, "</script>").FirstOrDefault(x => x.Contains("\"node\":{\"role\":\"ENTITY_PLACES\"")) + "/end", "{\"require\"", "/end");
                                    else
                                        splittedArrayString = Utilities.GetBetween(Regex.Split(postResponse, "</script>").SingleOrDefault(x => x.Contains("\"role\":\"ENTITY_PAGES\"")) + "/end", "{\"require\"", "/end");
                                }
                                decodedResponse = "{\"require\"" + splittedArrayString;
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
                            var nodes = parser.GetJTokenOfJToken(jObject, "data", "user", "sorted_liked_and_followed_pages", "edges");
                            if (nodes == null || nodes.Count() == 0)
                                nodes = parser.GetJTokenOfJToken(jObject, "data", "serpResponse", "results", "edges");
                            if (nodes == null || nodes.Count() == 0)
                                nodes = parser.GetJTokenOfJToken(jObject, "data", "viewer", "actor", "additional_profiles_with_biz_tools", "edges");
                            if (nodes == null || nodes.Count() == 0)
                                nodes = JsonSearcher.FindByKey(JsonSearcher.FindByKey(jObject, "data"), "edges");
                            if (nodes == null || nodes.Count() == 0)
                                nodes = JsonSearcher.FindByKey(jObject, "edges");
                            if (nodes == null || nodes.Count() == 0)
                                nodes = JsonSearcher.FindByKey(jObject, "node");
                            if (nodes == null || nodes.Count() == 0)
                                nodes = JsonSearcher.FindByKey(jObject, "nodes");
                            if (nodes != null || nodes.Count() != 0)
                            {
                                if (nodes.Type == JTokenType.Array)
                                {
                                    foreach (var nodeuser in nodes)
                                        elements.Add(nodeuser);
                                }
                                else
                                    elements.Add(nodes);
                            }
                        }
                        foreach (var node in fdjArray)
                        {
                            var nodes = parser.GetJTokenOfJToken(node, "data", "serpResponse", "results", "edges");
                            if (nodes == null || nodes.Count() == 0)
                                nodes = parser.GetJTokenOfJToken(node, "data", "node");
                            if (nodes != null || nodes.Count() != 0)
                            {
                                foreach (var nodeuser in nodes)
                                    elements.Add(nodeuser);
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
                        if (nodeObject != null && nodeObject.Count() > 0 && (JsonSearcher.FindStringValueByKey(nodeObject, "__typename") == "User" || JsonSearcher.FindStringValueByKey(nodeObject, "__typename") == "Page"))
                            storyObject = nodeObject;
                        var _username = parser.GetJTokenValue(storyObject, "relay_rendering_strategy", "view_model", "profile", "name");
                        if (string.IsNullOrEmpty(_username))
                            _username = JsonSearcher.FindStringValueByKey(JsonSearcher.FindByKey(storyObject, "profile"), "name");
                        if (string.IsNullOrEmpty(_username))
                            _username = JsonSearcher.FindStringValueByKey(storyObject, "name");
                        var _userId = parser.GetJTokenValue(storyObject, "relay_rendering_strategy", "view_model", "profile", "id");
                        if (string.IsNullOrEmpty(_userId))
                            _userId = JsonSearcher.FindStringValueByKey(JsonSearcher.FindByKey(storyObject, "profile"), "id");
                        if (string.IsNullOrEmpty(_userId))
                            _userId = JsonSearcher.FindStringValueByKey(storyObject, "id");
                        var _userUrl = parser.GetJTokenValue(storyObject, "relay_rendering_strategy", "view_model", "profile", "url");
                        if (string.IsNullOrEmpty(_userUrl))
                            _userUrl = JsonSearcher.FindStringValueByKey(JsonSearcher.FindByKey(storyObject, "profile"), "url");
                        if (string.IsNullOrEmpty(_userUrl))
                            _userUrl = JsonSearcher.FindStringValueByKey(storyObject, "url");
                        if (string.IsNullOrEmpty(_userUrl) && !string.IsNullOrEmpty(_userId))
                            _userUrl = FdConstants.FbHomeUrl + _userId;
                        var _userProfilePicUrl = parser.GetJTokenValue(storyObject, "relay_rendering_strategy", "view_model", "profile", "profile_picture", "uri");
                        if (string.IsNullOrEmpty(_userProfilePicUrl))
                            _userProfilePicUrl = JsonSearcher.FindStringValueByKey(JsonSearcher.FindByKey(storyObject, "profile_picture"), "uri");
                        var isVerified = JsonSearcher.FindStringValueByKey(storyObject, "is_verified");
                        var category = JsonSearcher.FindStringValueByKey(storyObject, "category_name");
                        var _basicDetailsText = JsonSearcher.FindStringValueByKey(JsonSearcher.FindByKey(storyObject, "primary_snippet_text_with_entities"), "text");
                        var _descDetailsText = JsonSearcher.FindStringValueByKey(JsonSearcher.FindByKey(storyObject, "description_snippets_text_with_entities"), "text");
                        var _followers = "";
                        var _likes = "";
                        var _ratings = "";
                        var _reviewCount = "";
                        if (!string.IsNullOrEmpty(_basicDetailsText))
                        {
                            var arrays = _basicDetailsText.Split('·');
                            arrays.ForEach(x =>
                            {
                                if (string.IsNullOrEmpty(category)) category = arrays.FirstOrDefault()?.Trim();
                                if (x.Contains("followers"))
                                {
                                    _followers = x.Replace(" ", "").Replace("followers", "");
                                    if (_followers.ToLower().Contains("m"))
                                        _followers = (double.Parse(FdFunctions.GetDouleOnlyString(_followers)) * 1000000).ToString(CultureInfo.InvariantCulture);
                                    else if (_followers.ToLower().Contains("k"))
                                        _followers = (double.Parse(FdFunctions.GetDouleOnlyString(_followers)) * 1000).ToString(CultureInfo.InvariantCulture);
                                }
                                if (x.Contains("likes"))
                                {
                                    _likes = x.Replace(" ", "").Replace("likes", "");
                                    if (_likes.ToLower().Contains("m"))
                                        _likes = (double.Parse(FdFunctions.GetDouleOnlyString(_likes)) * 1000000).ToString(CultureInfo.InvariantCulture);
                                    else if (_likes.ToLower().Contains("k"))
                                        _likes = (double.Parse(FdFunctions.GetDouleOnlyString(_likes)) * 1000).ToString(CultureInfo.InvariantCulture);
                                }
                                if (x.Contains("reviews"))
                                {
                                    _ratings = Utilities.GetBetween(x.Trim().Insert(0, "/"), "/", " out of");
                                    _reviewCount = Utilities.GetBetween(x.Trim(), "(", " reviews)");
                                }
                            });
                        }
                        ObjFdScraperResponseParameters.ListPage.Add(new FanpageDetails()
                        {
                            FanPageName = _username,
                            FanPageID = _userId,
                            FanPageUrl = _userUrl,
                            FanPageProfilePicurl = _userProfilePicUrl,
                            IsVerifiedPage = isVerified,
                            FanPageCategory = category,
                            FanpageFollowerCount = _followers,
                            FanPageLikerCount = _likes,
                            RatingCount = _reviewCount,
                            RatingValue = _ratings,
                            FanPageDescription = _descDetailsText
                        });
                    }

                }

            }
            catch (Exception ex)
            { ex.DebugLog(); }
            Status = !string.IsNullOrEmpty(responseParameter?.Response);
            HasMoreResults = hasMoreResults;
        }

    }
}
