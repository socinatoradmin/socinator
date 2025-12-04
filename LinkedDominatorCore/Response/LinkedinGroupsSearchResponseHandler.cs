using System;
using System.Collections.Generic;
using System.Net;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using HtmlAgilityPack;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDUtility;
using LinkedDominatorCore.Utility;
using System.Text.RegularExpressions;
using System.Linq;

namespace LinkedDominatorCore.Response
{
    public class LinkedinGroupsSearchResponseHandler : LdResponseHandler
    {
        public LinkedinGroupsSearchResponseHandler(IResponseParameter response)
            : base(response)
        {
            Success = !string.IsNullOrEmpty(response?.Response) && (response.Response.Contains("\"primaryResultType\":\"GROUPS\"") || response.Response.Contains("<!DOCTYPE html>"));
            if (!Success)
                return;
            var jsonJArrayHandler = JsonJArrayHandler.GetInstance;

            if (RespJ == null && response.Response.Contains("<!DOCTYPE html>"))
            {
                BrowserResponsHandler();
                return;
            }

            try
            {
                var jsonObject = jsonJArrayHandler.ParseJsonToJObject(response.Response.ToString());
                var arr = jsonJArrayHandler.GetJArrayElement(jsonJArrayHandler.GetJArrayElement(jsonJArrayHandler.GetJTokenOfJToken(jsonJArrayHandler.ParseJsonToJObject(jsonJArrayHandler.GetJTokenOfJToken(jsonObject, "elements").FirstOrDefault(x => x.ToString().Contains("entityResult")).ToString()), "items").ToString()).ToString());
                if (arr.ToString().Contains("elements\":[]"))
                {
                    Success = false;
                    return;
                }
                foreach (var item in arr)
                {
                    // for web api response
                    var itemdetails=jsonJArrayHandler.GetJTokenOfJToken(item, "itemUnion", "entityResult");
                    var groupId = Regex.Match(jsonJArrayHandler.GetJTokenValue(itemdetails,"trackingUrn"), @"\d+").Value;

                    // for mobile api response
                    if(string.IsNullOrEmpty(groupId))
                        groupId = jsonJArrayHandler.GetJTokenValue(item, "hitInfo",
                        "com.linkedin.voyager.search.SearchGroup", "id");
                    
                    var objLinkedinGroups = new LinkedinGroup(groupId)
                    {
                        GroupName = Utils.AssignNa(jsonJArrayHandler.GetJTokenValue(itemdetails, "title",
                                    "text")),
                        TotalMembers = Utils.AssignNa(jsonJArrayHandler.GetJTokenValue(itemdetails, "primarySubtitle",
                                    "text").Replace("members", "").Trim()),
                        CommunityType= "STANDERD",
                        MembershipStatus=Utils.AssignNa(jsonJArrayHandler.GetJTokenValue(itemdetails,
                        "primaryActions",0, "actionDetails", "leaveGroupAction", "status"))

                    };
                    LinkedinGroupsList.Add(objLinkedinGroups);

                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public List<LinkedinGroup> LinkedinGroupsList { get; } = new List<LinkedinGroup>();

        private void BrowserResponsHandler()
        {
            try
            {
                var groupsNodeList =
                    HtmlAgilityHelper.GetListNodesFromClassName(Response.Response,
                        "reusable-search__result-container");

                foreach (var groupNode in groupsNodeList)
                {
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(groupNode.OuterHtml);
                    var linkedinGroup = new LinkedinGroup(Utils.GetBetween(groupNode.OuterHtml, "/groups/", "\"")
                        ?.Replace("/", "")?.Trim());
                    linkedinGroup.CommunityType = "STANDERD";
                    linkedinGroup.GroupName = WebUtility.HtmlDecode(
                        HtmlAgilityHelper.GetStringInnerTextFromClassName(groupNode.OuterHtml,
                            "entity-result__title-text  t-16", htmlDoc));
                    if (string.IsNullOrEmpty(linkedinGroup.GroupName))
                        linkedinGroup.GroupName = Utils.GetBetween(groupNode.OuterHtml, "height=\"48\" alt=\"", "\" id=\"");
                    linkedinGroup.TotalMembers = HtmlAgilityHelper.GetStringInnerTextFromClassName(groupNode.OuterHtml, "entity-result__primary-subtitle t-14 t-black", htmlDoc).Replace("members", "");/* Utils.GetBetween(groupNode.OuterHtml, "<!---->", "members");*/
                    LinkedinGroupsList.Add(linkedinGroup);
                }
            }

            catch (Exception exception)
            {
                exception.DebugLog();
            }
        }
    }
}