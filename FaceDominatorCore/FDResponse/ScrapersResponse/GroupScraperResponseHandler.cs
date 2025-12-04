using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDResponse.BaseResponse;
using FaceDominatorCore.Interface;
using FaceDominatorCore.Utility;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace FaceDominatorCore.FDResponse.ScrapersResponse
{
    public class GroupScraperResponseHandler : FdResponseHandler, IResponseHandler
    {
        public string PageletData { get; set; }

        public bool HasMoreResults { get; set; }

        public string EntityId { get; set; }

        public bool Status { get; set; }

        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; }
            = new FdScraperResponseParameters();

        public GroupScraperResponseHandler(IResponseParameter responseParameter, List<GroupDetails> listGroupDetails)
            : base(responseParameter)
        {
            if (responseParameter.HasError)
                return;

            try
            {
                ObjFdScraperResponseParameters = new FdScraperResponseParameters();

                ObjFdScraperResponseParameters.ListGroup = listGroupDetails ?? new List<GroupDetails>();

                string decodedResponse = FdFunctions.GetNewPrtialDecodedResponse(responseParameter.Response);

                HtmlDocument objHtmlDocument = new HtmlDocument();

                objHtmlDocument.LoadHtml(decodedResponse);

                HtmlNodeCollection objNodeCollection =
                    objHtmlDocument.DocumentNode.SelectNodes("//div[starts-with(@class, '_3u1 _gli')]");

                if (objNodeCollection != null)
                {
                    List<string> fanpageResponseList = new List<string>();

                    objNodeCollection.ForEach(objNode =>
                            fanpageResponseList.Add(objNode.InnerHtml)
                        );

                    GetGroupDetails(fanpageResponseList);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public GroupScraperResponseHandler(IResponseParameter responseParameter, List<string> listJsonData, bool hasMoreResults,
           bool ClassicUi = true) : base(responseParameter)
        {

            ObjFdScraperResponseParameters = ObjFdScraperResponseParameters ?? new FdScraperResponseParameters();

            ObjFdScraperResponseParameters.ListGroup = new List<GroupDetails>();
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
                                var splittedArray = Utilities.GetBetween(Regex.Split(postResponse, "</script>").SingleOrDefault(x => x.Contains("{\"groups_tab\":{\"tab_groups_list\"")) + "/end", "{\"require\"", "/end");
                                decodedResponse = "{\"require\"" + splittedArray;
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
                            var _groupcount = parser.GetJTokenValue(jObject, "data", "viewer", "all_joined_groups", "total_joined_groups");
                            if (string.IsNullOrEmpty(_groupcount))
                                _groupcount = JsonSearcher.FindStringValueByKey(jObject, "total_joined_groups");
                            if (!string.IsNullOrEmpty(_groupcount))
                                ObjFdScraperResponseParameters.TotalCount = _groupcount;
                            var nodes = parser.GetJTokenOfJToken(jObject, "data", "viewer", "all_joined_groups", "tab_groups_list", "edges");
                            if (nodes == null || nodes.Count() == 0)
                                nodes = parser.GetJTokenOfJToken(jObject, "data", "viewer", "people_you_may_know", "edges");
                            if (nodes == null || nodes.Count() == 0)
                                nodes = JsonSearcher.FindByKey(jObject, "edges");
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
                            nodes = parser.GetJTokenOfJToken(node, "data", "feedback", "reshares", "edges");
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
                        if (nodeObject != null && nodeObject.Count() > 0 && JsonSearcher.FindStringValueByKey(nodeObject, "__typename") == "Group")
                            storyObject = nodeObject;
                        var _username = parser.GetJTokenValue(storyObject, "relay_rendering_strategy", "view_model", "profile", "name");
                        if (string.IsNullOrEmpty(_username))
                            _username = JsonSearcher.FindStringValueByKey(storyObject, "name");
                        var _userUrl = parser.GetJTokenValue(storyObject, "relay_rendering_strategy", "view_model", "profile", "url");
                        if (string.IsNullOrEmpty(_userUrl))
                            _userUrl = JsonSearcher.FindStringValueByKey(storyObject, "url");
                        var isMember = JsonSearcher.FindStringValueByKey(storyObject, "viewer_join_state");
                        var _userId = parser.GetJTokenValue(storyObject, "relay_rendering_strategy", "view_model", "profile", "id");
                        if (string.IsNullOrEmpty(_userId))
                            _userId = JsonSearcher.FindStringValueByKey(storyObject, "id");
                        ObjFdScraperResponseParameters.ListGroup.Add(new GroupDetails()
                        {
                            GroupName = _username,
                            GroupId = _userId,
                            GroupUrl = _userUrl,
                            GroupJoinStatus = isMember
                        });
                    }

                }
            }
            catch (Exception ex)
            { ex.DebugLog(); }
            HasMoreResults = hasMoreResults;
            Status = ObjFdScraperResponseParameters.ListGroup.Count > 0;
        }

        private void GetGroupDetails(List<string> listGroupDetails)
        {
            HtmlDocument objHtmlDocument = new HtmlDocument();

            foreach (string response in listGroupDetails)
            {
                try
                {
                    GroupDetails objGroupDetails = new GroupDetails();

                    var decodedResponse = FdFunctions.GetDecodedResponse(response);

                    objHtmlDocument.LoadHtml(decodedResponse);


                    var groupDetails = HtmlParseUtility.GetListInnerHtmlFromTagName(decodedResponse, "div", "class", "x78zum5 xdt5ytf xz62fqu x16ldp7u");

                    objGroupDetails.GroupUrl = FdRegexUtility.FirstMatchExtractor(groupDetails[0], FdConstants.ScrapedUrlRegx);

                    if (string.IsNullOrEmpty(objGroupDetails.GroupUrl))
                        continue;

                    if (!objGroupDetails.GroupUrl.StartsWith("https://www.facebook.com/groups/")
                        || objGroupDetails.GroupUrl.StartsWith("https://www.facebook.com/groups/?"))
                        continue;

                    objGroupDetails.GroupId = Regex.Split(objGroupDetails.GroupUrl, "/").LastOrDefault(x => !string.IsNullOrEmpty(x));

                    objGroupDetails.GroupName = HtmlParseUtility.GetInnerTextFromTagName(groupDetails[0], "a", "role", "presentation");

                    using (FdHtmlParseUtility utility = new FdHtmlParseUtility())
                    {
                        objGroupDetails.GroupDescription = utility.GetInnerTextFromPartialTagNameContains(response, "span", "class", "x1lliihq x6ikm8r x10wlt62 x1n2onr6 x1j85h84");
                    }

                    try
                    {
                        var groupInfo = HtmlParseUtility.GetInnerTextFromTagName(groupDetails[0], "span", "class", "x1lliihq x6ikm8r x10wlt62 x1n2onr6");

                        groupDetails = Regex.Split(groupInfo, " · ").ToList();

                        var groupMemberCountDetails = groupDetails.FirstOrDefault(x => x.Contains("members"))?.Replace("members", string.Empty);

                        if (groupMemberCountDetails.ToLower().Contains("m"))
                        {
                            groupMemberCountDetails = FdFunctions.GetDouleOnlyString(groupMemberCountDetails);
                            groupMemberCountDetails = (double.Parse(groupMemberCountDetails) * 1000000).ToString(CultureInfo.InvariantCulture);
                        }
                        else if (groupMemberCountDetails.ToLower().Contains("k"))
                        {
                            groupMemberCountDetails = FdFunctions.GetDouleOnlyString(groupMemberCountDetails);
                            groupMemberCountDetails = (double.Parse(groupMemberCountDetails) * 1000).ToString(CultureInfo.InvariantCulture);
                        }

                        objGroupDetails.GroupMemberCount = FdFunctions.GetIntegerOnlyString(groupMemberCountDetails);
                        objGroupDetails.GroupType = groupDetails[0];
                    }
                    catch (ArgumentException) { }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }


                    if (decodedResponse.ToLower().Contains("aria-label=\"join group"))
                        objGroupDetails.GroupJoinStatus = "Not a member";
                    else
                        objGroupDetails.GroupJoinStatus = "Request Sent / Already a member";

                    if (ObjFdScraperResponseParameters.ListGroup.FirstOrDefault(x => x.GroupId == objGroupDetails.GroupId) == null)
                        ObjFdScraperResponseParameters.ListGroup.Add(objGroupDetails);

                }
                catch (ArgumentException)
                {

                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

            }

            Status = ObjFdScraperResponseParameters.ListGroup.Count > 0;

        }

    }
}
