using System;
using System.Collections.Generic;
using System.Linq;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDUtility;
using LinkedDominatorCore.Utility;
using Newtonsoft.Json.Linq;

namespace LinkedDominatorCore.Response
{
    public class MyGroupsResponseHandler : LdResponseHandler
    {
        private readonly JsonJArrayHandler _jsonJArrayHandler = JsonJArrayHandler.GetInstance;

        // ReSharper disable once UnusedParameter.Local
        public MyGroupsResponseHandler(IResponseParameter response, bool onlyJoinedGroups) : base(response)
        {
            try
            {
                try
                {
                    Success = !string.IsNullOrEmpty(response?.Response) && (response.Response.Contains("\"viewerGroupMembership\"") || response.Response.Contains("<!DOCTYPE html>"));
                    if (!Success)
                        return;
                    if (response.Response.Contains("<!DOCTYPE html>"))
                    {
                        BrowserResponseHandler();
                        return;
                    }

                    var arr = ParsedJArray(response.Response);
                    if (response.Response.Contains("data\":[]"))
                    {
                        Success = false;
                        return;
                    }

                    #region MyRegion

                    foreach (var item in arr)
                        try
                        {
                            // GroupId
                            var groupId = GetGroupId(item);

                            // skip non groups
                            if (string.IsNullOrEmpty(groupId))
                                continue;
                            var objLinkedinGroup = new LinkedinGroup(groupId);


                            // GetGroupName
                            objLinkedinGroup.GroupName = Utils.AssignNa(GetGroupName(item));

                            //TotalMembers
                            objLinkedinGroup.TotalMembers = Utils.AssignNa(GetGroupMembersCount(item));

                            // CommunityType
                            objLinkedinGroup.CommunityType = Utils.AssignNa(GetCommunityType(item));

                            // MembershipStatus
                            objLinkedinGroup.MembershipStatus = GetMembershipStatus(item);

                            if (!GroupsList.Contains(objLinkedinGroup) &&
                                objLinkedinGroup.MembershipStatus != "PENDING")
                                GroupsList.Add(objLinkedinGroup);
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }

                    #endregion
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public List<LinkedinGroup> GroupsList { get; set; } = new List<LinkedinGroup>();

        private void BrowserResponseHandler()
        {
            var groupNodeList =
                HtmlAgilityHelper.GetListNodesFromClassName(Response.Response, "artdeco-list__item");
            groupNodeList = groupNodeList is null || groupNodeList.Count == 0 ? HtmlAgilityHelper.GetListNodesFromClassName(Response.Response,
                    "artdeco-entity-lockup__content ember-view") : groupNodeList;
            var ldDataHelper = LdDataHelper.GetInstance;
            foreach (var groupNode in groupNodeList)
            {
                var groupId = ldDataHelper.GetGroupIdFromPageSource(groupNode.OuterHtml);
                var objLinkedinGroup = new LinkedinGroup(groupId);
                var node = HtmlParseUtility.GetListInnerTextFromPartialTagNamecontains(groupNode.InnerHtml, "a", "href", "/groups/");
                var groupName = node != null ? node.FirstOrDefault()?.Replace("\n","")?.Trim()?.Replace(",", " ")?.Replace("&amp;", "&"):string.Empty;
                groupName = !string.IsNullOrEmpty(groupName) && groupName.Contains("members") ? Utils.GetBetween(groupNode.InnerHtml,"alt=\"","\">") : groupName;
                objLinkedinGroup.GroupName = groupName;
                var CommunityType= Utils.GetBetween(groupNode.OuterHtml, "artdeco-entity-lockup__metadata ember-view\">", "•")
                    ?.Replace(" members", "")?.Trim();
                CommunityType = string.IsNullOrEmpty(CommunityType) ? "STANDERD" : CommunityType;
                objLinkedinGroup.CommunityType = CommunityType;
                objLinkedinGroup.TotalMembers = Utils.GetBetween(groupNode.OuterHtml, "artdeco-entity-lockup__metadata ember-view\">", "<")
                    ?.Replace(" members", "")?.Trim();
                if (groupNode.OuterHtml.Contains("artdeco-entity-lockup__badge ember-view"))
                    objLinkedinGroup.MembershipStatus = Utils
                        .GetBetween(groupNode.OuterHtml, "artdeco-entity-lockup__label\">", "<").ToUpper();
                else
                    objLinkedinGroup.MembershipStatus = "MEMBER";
                if (!GroupsList.Contains(objLinkedinGroup) && objLinkedinGroup.MembershipStatus != "PENDING")
                    GroupsList.Add(objLinkedinGroup);
            }
        }

        public JArray ParsedJArray(string jsonResponse)
        {
            JArray arr = null;
            try
            {
                try
                {
                    arr = JArray.Parse(_jsonJArrayHandler.GetJTokenValue(RespJ, "data"));
                }
                catch (Exception)
                {
                    // ignored
                }

                if (arr == null)
                    try
                    {
                        arr = JArray.Parse(RespJ["elements"].ToString());
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                if (arr == null)
                {
                    var currentCount = 0;
                    arr = JArray.Parse(RespJ["elements"][currentCount]["recommendedEntities"].ToString());
                    while (!arr.ToString().Contains("type\": \"GROUP\""))
                        arr = JArray.Parse(RespJ["elements"][++currentCount]["recommendedEntities"].ToString());
                }
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }

            return arr;
        }

        public string GetGroupId(JToken item)
        {
            var groupId = string.Empty;
            try
            {
                try
                {
                    groupId = item["groupUrn"].ToString();
                }
                catch (Exception)
                {
                    // ignored
                }

                if (string.IsNullOrEmpty(groupId))
                    groupId =
                        item["com.linkedin.voyager.feed.packageRecommendations.RecommendedGenericEntity"]["entityUrn"]
                            .ToString();

                groupId = Utils.GetBetween(groupId + "&&", "group:", "&&");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return groupId;
        }


        public string GetGroupName(JToken item)
        {
            var groupName = string.Empty;

            try
            {
                if (string.IsNullOrEmpty(groupName = _jsonJArrayHandler.GetJTokenValue(item, "group", "mini", "name")))
                    groupName = _jsonJArrayHandler.GetJTokenValue(item, "name", "text");

                groupName = Utils.InsertSpecialCharactersInCsv(groupName);
                if (string.IsNullOrEmpty(groupName))
                    groupName =
                        item["com.linkedin.voyager.feed.packageRecommendations.RecommendedGenericEntity"]["group"]
                            ["name"]["text"].ToString();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return groupName;
        }

        public string GetGroupMembersCount(JToken item)
        {
            var totalMembers = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(totalMembers =
                    _jsonJArrayHandler.GetJTokenValue(item, "group", "mini", "totalMembers")))
                    totalMembers = _jsonJArrayHandler.GetJTokenValue(item, "group", "totalMembers", "totalMembers");

                if (string.IsNullOrEmpty(totalMembers))
                    totalMembers = _jsonJArrayHandler.GetJTokenValue(item, "memberCount");

                if (string.IsNullOrEmpty(totalMembers))
                    totalMembers =
                        item["com.linkedin.voyager.feed.packageRecommendations.RecommendedGenericEntity"]["group"][
                            "memberCount"].ToString();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return totalMembers;
        }

        public string GetCommunityType(JToken item)
        {
            var communityType = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(communityType = _jsonJArrayHandler.GetJTokenValue(item, "type")))
                    communityType =
                        item["com.linkedin.voyager.feed.packageRecommendations.RecommendedGenericEntity"]["group"][
                            "type"].ToString();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return communityType;
        }

        public string GetMembershipStatus(JToken item)
        {
            var membershipStatus = string.Empty;
            try
            {
                membershipStatus =
                        _jsonJArrayHandler.GetJTokenValue(item, "miniMembership", "status");
                if (string.IsNullOrEmpty(membershipStatus))
                    membershipStatus = _jsonJArrayHandler.GetJTokenValue(item, "viewerGroupMembership", "status");
                if (string.IsNullOrEmpty(membershipStatus))
                    membershipStatus = _jsonJArrayHandler.GetJTokenValue(item, "com.linkedin.voyager.feed.packageRecommendations.RecommendedGenericEntity",
                        "group", "viewerGroupMembership", "status");
            }
            catch (Exception ex)
            {
                membershipStatus = "N/A";
                ex.DebugLog();
            }

            return membershipStatus;
        }
    }
}