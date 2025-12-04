using System;
using System.Collections.Generic;
using System.Linq;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDUtility;
using LinkedDominatorCore.Utility;
using Newtonsoft.Json.Linq;

namespace LinkedDominatorCore.Response
{
    public class LinkedinGroupMemberResponseHandler : LdResponseHandler
    {
        public LinkedinGroupMemberResponseHandler(IResponseParameter response)
            : base(response)
        {
            Success = !string.IsNullOrEmpty(response?.Response) && (response.Response.Contains("urn:li:fs_groupMember") || response.Response.Contains("<!DOCTYPE html>"));
            if (!Success)
                return;

            try
            {
                if (response.Response.Contains("<!DOCTYPE html>"))
                {
                    BrowserProcessResponse();
                    return;
                }

                var jsonJArrayHandler = JsonJArrayHandler.GetInstance;
                var elements = jsonJArrayHandler.GetJTokenValue(RespJ, "data");
                if (string.IsNullOrEmpty(elements))
                    elements = jsonJArrayHandler.GetJTokenValue(RespJ, "elements");
                var membersDetailsArray = jsonJArrayHandler.GetJArrayElement(elements);

                foreach (var memberDetail in membersDetailsArray)
                {
                    #region Initializations Required

                    var tempMemberDetail = jsonJArrayHandler.GetTokenElement(memberDetail, "miniProfile");

                    #endregion

                    #region Forming UsersList

                    try
                    {
                        var profileUrl = jsonJArrayHandler.GetJTokenValue(tempMemberDetail, "publicIdentifier");
                        var firstName = jsonJArrayHandler.GetJTokenValue(tempMemberDetail, "firstName");
                        var lastName = jsonJArrayHandler.GetJTokenValue(tempMemberDetail, "lastName");


                        var objLinkedinUser = new LinkedinUser(profileUrl)
                        {
                            ProfileUrl = "https://www.linkedin.com/in/" + profileUrl,
                            PublicIdentifier = profileUrl,
                            ProfileId = Utils.AssignNa(Utils.GetBetween(
                                $"{jsonJArrayHandler.GetJTokenValue(tempMemberDetail, "entityUrn")}&&&", "miniProfile:",
                                "&&&")),
                            MemberId = jsonJArrayHandler.GetJTokenValue(tempMemberDetail, "objectUrn")
                                ?.Replace("urn:li:member:", "")
                        };

                        #region FullName

                        try
                        {
                            objLinkedinUser.FullName = firstName + " " + lastName;
                            objLinkedinUser.FullName = Utils.InsertSpecialCharactersInCsv(objLinkedinUser.FullName);
                        }
                        catch (Exception)
                        {
                            // ignored
                            objLinkedinUser.FullName = "N/A";
                        }

                        #endregion


                        if (UsersList.All(x => x.ProfileUrl != profileUrl))
                            UsersList.Add(objLinkedinUser);
                    }
                    catch (Exception)
                    {
                        // ignored
                    }

                    #endregion
                }
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }
        }

        public List<LinkedinUser> UsersList { get; } = new List<LinkedinUser>();

        private void BrowserProcessResponse()
        {
            try
            {
                var ldDataHelper = LdDataHelper.GetInstance;
                var groupUserNodeList = HtmlAgilityHelper.GetListNodesFromClassName(Response.Response,
                    "artdeco-list__item groups-members-list__typeahead-result relative p0");


                foreach (var groupUserNode in groupUserNodeList)
                {
                    var publicIdentifier = ldDataHelper.GetPublicIdentifierFromPageSource(groupUserNode.OuterHtml);
                    publicIdentifier = string.IsNullOrEmpty(publicIdentifier) ?Utils.GetBetween(groupUserNode.OuterHtml,"<a href=\"/in/","/\""): publicIdentifier;
                    var linkedinUser = new LinkedinUser(publicIdentifier);
                    linkedinUser.FullName = ldDataHelper.GetAltFromPageSource(groupUserNode.OuterHtml);
                    if (string.IsNullOrEmpty(linkedinUser.FullName))
                        linkedinUser.FullName = HtmlAgilityHelper.GetStringInnerTextFromClassName(groupUserNode.OuterHtml, "artdeco-entity-lockup__title ember-view");
                    if (linkedinUser.FullName.Contains("LinkedIn"))
                        continue;
                    linkedinUser.ProfilePicUrl = ldDataHelper.GetSource(groupUserNode.OuterHtml);
                    if (string.IsNullOrEmpty(linkedinUser.ProfilePicUrl))
                        linkedinUser.ProfilePicUrl = Utils.GetBetween(groupUserNode.OuterHtml, "<img src=\"", "\" loading=\"");
                    UsersList.Add(linkedinUser);
                }
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }
        }
    }
}