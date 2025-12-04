using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDUtility;
using LinkedDominatorCore.Utility;
using Newtonsoft.Json.Linq;

namespace LinkedDominatorCore.Response
{
    public class ReceivedInvitationsResponseHandler : LdResponseHandler
    {
        public ReceivedInvitationsResponseHandler(IResponseParameter response) : base(response)
        {
            try
            {
                Success = !string.IsNullOrEmpty(response?.Response) && (response.Response.Contains("urn:li:fs_relInvitationView") || response.Response.Contains("<!DOCTYPE html>"));
                if (!Success)
                    return;
                var InvitationResponse = response.Response.ToString();
                if (response.Response.Contains("<!DOCTYPE html>"))
                {
                    GetUserListFromWebResponse();
                    Success = UsersList.Count != 0;
                }else if (InvitationResponse.Contains("{\"metadata\":{},\"elements\":[],\"paging\":{\"total\":0,\"") ||InvitationResponse.Contains("{\"metadata\":{},\"elements\":[]") || InvitationResponse.Contains("\"elements\":[]"))
                {
                    Success = false;
                    return;
                }
                else
                {
                    GetUserListFromNormalResponse();
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public List<LinkedinUser> UsersList { get; } = new List<LinkedinUser>();

        private void GetUserListFromNormalResponse()
        {
            var jsonJArrayHandler = JsonJArrayHandler.GetInstance;
            var arr = jsonJArrayHandler.GetJArrayElement(jsonJArrayHandler.GetJTokenValue(RespJ, "elements"));

            #region invitations

            foreach (var item in arr)
                try
                {
                    var publicIdentifier =
                        jsonJArrayHandler.GetJTokenValue(item, "invitation", "fromMember", "publicIdentifier");

                    var linkedinUser = new LinkedinUser(publicIdentifier)
                    {
                        PublicIdentifier = publicIdentifier
                    };

                    // firstName
                    var firstName = jsonJArrayHandler.GetJTokenValue(item, "invitation", "fromMember", "firstName");
                    firstName = string.IsNullOrEmpty(firstName) ? jsonJArrayHandler.GetJTokenValue(item, "genericInvitationView", "primaryImage", "attributes", 0, "miniProfile", "firstName") : firstName;
                    firstName = string.IsNullOrEmpty(firstName) ? jsonJArrayHandler.GetJTokenValue(item, "genericInvitationView", "primaryImage", "attributes", 0, "miniCompany", "name") : firstName;
                    if (string.IsNullOrEmpty(firstName) || firstName.Contains("Linkedin") ||
                        firstName.Contains("Linkedin Member"))
                        continue;

                    var lastName = jsonJArrayHandler.GetJTokenValue(item, "invitation", "fromMember", "lastName");
                    lastName = string.IsNullOrEmpty(lastName) ? jsonJArrayHandler.GetJTokenValue(item, "genericInvitationView", "primaryImage", "attributes", 0, "miniProfile", "lastName") : lastName;
                    #region FullName

                    try
                    {
                        linkedinUser.FullName = firstName + " " + lastName;
                        linkedinUser.FullName = Utils.InsertSpecialCharactersInCsv(linkedinUser.FullName);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                        linkedinUser.FullName = "N/A";
                    }

                    #endregion

                    // ProfileId
                    linkedinUser.ProfileId = GetProfileId(item, jsonJArrayHandler);

                    if(string.IsNullOrEmpty(linkedinUser.PublicIdentifier))
                        linkedinUser.PublicIdentifier = jsonJArrayHandler.GetJTokenValue(item, "genericInvitationView", "primaryImage", "attributes", 0, "miniProfile", "publicIdentifier");

                    // ProfileUrl
                    if (!string.IsNullOrEmpty(linkedinUser.PublicIdentifier))
                        linkedinUser.ProfileUrl = "https://www.linkedin.com/in/" + linkedinUser.PublicIdentifier;

                    //TrackingId
                    var trackingId = jsonJArrayHandler.GetJTokenValue(item, "invitation", "fromMember", "trackingId");
                    trackingId = string.IsNullOrEmpty(trackingId) ? jsonJArrayHandler.GetJTokenValue(item, "genericInvitationView", "primaryImage", "attributes", 0, "miniCompany", "trackingId") : trackingId;
                    linkedinUser.TrackingId =
                        Utils.AssignNa(string.IsNullOrEmpty(trackingId)?jsonJArrayHandler.GetJTokenValue(item, "genericInvitationView", "primaryImage", "attributes", 0, "miniProfile", "trackingId") :trackingId);
                    //sentDate
                    double.TryParse(jsonJArrayHandler.GetJTokenValue(item, "invitation", "sentTime"), out _);

                    // invitationId
                    linkedinUser.InvitationId = GetInvitationId(item, jsonJArrayHandler);

                    // SharedSecret
                    linkedinUser.InvitationSharedSecret =
                        jsonJArrayHandler.GetJTokenValue(item, "invitation", "sharedSecret");

                    UsersList.Add(linkedinUser);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                    GlobusLogHelper.log.Info(
                        "publicIdentifier doesnot exist on getting List of users from your search query");
                }

            #endregion
        }

        private void GetUserListFromWebResponse()
        {
            try
            {
                var ldDataHelper = LdDataHelper.GetInstance;
                var nodesListResponse = HtmlAgilityHelper.GetListNodesFromAttibute(Response.Response,"div",AttributeIdentifierType.DataViewName,null,
                    "pending-invitation");
                if(nodesListResponse != null && nodesListResponse.Count > 0)
                {
                    foreach (var node in nodesListResponse)
                    {
                        var userResponse = node.OuterHtml;

                        if (!userResponse.Contains("data-view-name=\"pending-invitation\"") || !userResponse.Contains("aria-label=\"Accept"))
                            continue;
                        var fullName = Utils.RemoveHtmlTags(HtmlAgilityHelper.GetListInnerTextFromTagName(userResponse, "a")?.FirstOrDefault(x => !string.IsNullOrEmpty(x.Trim()?.Replace("\n", ""))));
                        var publicIdentifier = Utils.GetBetween(userResponse, "href=\"https://www.linkedin.com/in/", "/\"");
                        var invitationID = Utils.GetBetween(userResponse, "componentkey=\"urn:li:invitation:", "\"");
                        var linkedInUser = new LinkedinUser(publicIdentifier)
                        {
                            FullName = fullName,
                            InvitationId = invitationID
                        };
                        linkedInUser.FullName = linkedInUser.FullName != null ? linkedInUser.FullName.Contains("invited you to subscribe to") ? Regex.Replace(linkedInUser.FullName, "invited you to subscribe to(.*)", "") : linkedInUser.FullName : linkedInUser.FullName;
                        linkedInUser.NodeResponse = userResponse;
                        UsersList.Add(linkedInUser);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private string GetInvitationId(JToken jToken, JsonJArrayHandler jsonJArrayHandler = null)
        {
            var invitationId = "";

            #region InvitationId

            try
            {
                if (jsonJArrayHandler == null)
                    jsonJArrayHandler = JsonJArrayHandler.GetInstance;

                var entityUrn =
                    jsonJArrayHandler.GetJTokenValue(jToken, "entityUrn");
                
                if (entityUrn.Contains("fs_relInvitationView:"))
                    invitationId = Regex.Split(entityUrn, "fs_relInvitationView:")[1];
                if (string.IsNullOrEmpty(invitationId))
                {
                    var mailboxItemId = jsonJArrayHandler.GetJTokenValue(jToken, "invitation", "mailboxItemId");
                    
                    if (mailboxItemId.Contains("invitation:"))
                        invitationId = Regex.Split(mailboxItemId, "invitation:")[1];
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                invitationId = "N/A";
            }

            #endregion

            return invitationId;
        }

        private string GetProfileId(JToken jToken, JsonJArrayHandler jsonJArrayHandler = null)
        {
            var profileId = "";

            #region ProfileId

            try
            {
                if (jsonJArrayHandler == null)
                    jsonJArrayHandler = JsonJArrayHandler.GetInstance;

                profileId = jsonJArrayHandler.GetJTokenValue(jToken, "invitation", "fromMember", "entityUrn");
               

                if (string.IsNullOrEmpty(profileId))
                    profileId =jsonJArrayHandler.GetJTokenValue(jToken, "genericInvitationView", "primaryImage", "attributes", 0, "miniProfile", "entityUrn");      
                if (profileId.Contains("urn:li:fs_miniProfile"))
                    try
                    {
                        profileId = Regex.Split(profileId, "urn:li:fs_miniProfile")[1].Replace(":", "");
                    }
                    catch (Exception exx)
                    {
                        exx.DebugLog();
                    }
            }
            catch (Exception exx)
            {
                exx.DebugLog();
                profileId = "N/A";
            }

            #endregion

            return profileId;
        }
    }
}