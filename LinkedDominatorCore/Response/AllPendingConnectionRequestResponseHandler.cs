using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using HtmlAgilityPack;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDUtility;
using LinkedDominatorCore.Utility;
using Newtonsoft.Json.Linq;

namespace LinkedDominatorCore.Response
{
    public class AllPendingConnectionRequestResponseHandler : LdResponseHandler
    {
        public AllPendingConnectionRequestResponseHandler(IResponseParameter response)
            : base(response)
        {
            Success = !string.IsNullOrEmpty(response?.Response) && (response.Response.Contains("\"heroInvitations\"") || response.Response.Contains("<!DOCTYPE html>"));
            if (!Success)
                return;

            try
            {
                if (Response.Response.Contains("<!DOCTYPE html>"))
                    GetUserListFromWebResponse(response);
                else
                    GetUserListFromNormalResponse(response);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                Success = false;
            }
        }

        public List<LinkedinUser> LstAllPendingConnectionRequest { get; } = new List<LinkedinUser>();

        private void GetUserListFromNormalResponse(IResponseParameter response)
        {
            var jsonJArrayHandler = JsonJArrayHandler.GetInstance;
            var arr = jsonJArrayHandler.GetJArrayElement(jsonJArrayHandler.GetJTokenValue(RespJ, "elements"));

            if (arr is null || !arr.HasValues)
            {
                Success = false;
                return;
            }


            foreach (var item in arr)
            {
                var emailAddress = string.Empty;

                try
                {
                    #region PublicIdentifier OR EmailAddress

                    var publicIdentifier = jsonJArrayHandler.GetJTokenValue(item, "toMember", "publicIdentifier");

                    if (string.IsNullOrEmpty(publicIdentifier))
                        publicIdentifier =
                            jsonJArrayHandler.GetJTokenValue(item, "heroInvitations", 0, "toMember",
                                "publicIdentifier");

                    if (string.IsNullOrEmpty(publicIdentifier))
                        emailAddress = jsonJArrayHandler.GetJTokenValue(item, "heroInvitations", 0, "invitee",
                            "com.linkedin.voyager.relationships.invitation.EmailInvitee", "email");

                    if (string.IsNullOrEmpty(emailAddress))
                        emailAddress = jsonJArrayHandler.GetJTokenValue(item, "invitee",
                            "com.linkedin.voyager.relationships.invitation.EmailInvitee", "email");

                    #endregion

                    var linkedinUser = !string.IsNullOrEmpty(publicIdentifier)
                        ? new LinkedinUser(publicIdentifier) {PublicIdentifier = publicIdentifier}
                        : new LinkedinUser(emailAddress) {EmailAddress = emailAddress};

                    // firstName
                    var firstName = jsonJArrayHandler.GetJTokenValue(item, "toMember", "firstName");
                    if (string.IsNullOrEmpty(firstName))
                        firstName = jsonJArrayHandler.GetJTokenValue(item, "heroInvitations", 0, "toMember",
                            "firstName");

                    // lastName
                    var lastName = jsonJArrayHandler.GetJTokenValue(item, "toMember", "lastName");
                    if (string.IsNullOrEmpty(lastName))
                        lastName = jsonJArrayHandler.GetJTokenValue(item, "heroInvitations", 0, "toMember", "lastName");


                    #region FullName

                    try
                    {
                        linkedinUser.FullName = firstName + " " + lastName;
                        linkedinUser.FullName = linkedinUser.FullName == " "
                            ? "N/A"
                            : Utils.InsertSpecialCharactersInCsv(linkedinUser.FullName);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    #endregion

                    //ProfileId
                    linkedinUser.ProfileId = jsonJArrayHandler.GetJTokenValue(item, "toMemberId");
                    if (string.IsNullOrEmpty(linkedinUser.ProfileId))
                        linkedinUser.ProfileId =
                            jsonJArrayHandler.GetJTokenValue(item, "heroInvitations", 0, "toMemberId");

                    linkedinUser.ProfileId = string.IsNullOrEmpty(linkedinUser.ProfileId)
                        ? "N/A"
                        : linkedinUser.ProfileId.Replace("urn:li:fs_miniProfile", "").Replace(":", "");


                    #region ProfileUrl,HasAnonymousProfilePicture,ProfilePicUrl

                    if (!string.IsNullOrEmpty(linkedinUser.PublicIdentifier))
                    {
                        linkedinUser.ProfileUrl = "https://www.linkedin.com/in/" + linkedinUser.PublicIdentifier;
                        var backgroundImage = jsonJArrayHandler.GetJTokenValue(item, "toMember", "backgroundImage",
                            "com.linkedin.common.VectorImage");

                        if (string.IsNullOrEmpty(backgroundImage))
                            backgroundImage = Utils.AssignNa(jsonJArrayHandler.GetJTokenValue(item, "heroInvitations",
                                0,
                                "toMember", "backgroundImage", "com.linkedin.common.VectorImage"));

                        var picture =
                            jsonJArrayHandler.GetJTokenValue(item, "toMember", "picture",
                                "com.linkedin.common.VectorImage");
                        if (string.IsNullOrEmpty(picture))
                            backgroundImage = Utils.AssignNa(jsonJArrayHandler.GetJTokenValue(item, "heroInvitations",
                                0,
                                "toMember", "picture", "com.linkedin.common.VectorImage"));


                        if (backgroundImage != "N/A" || picture != "N/A")
                        {
                            linkedinUser.HasAnonymousProfilePicture = true;
                            linkedinUser.ProfilePicUrl =
                                "https://www.linkedin.com/in/" + linkedinUser.PublicIdentifier + "/detail/photo/";
                        }
                    }

                    #endregion

                    //TrackingId
                    linkedinUser.TrackingId = jsonJArrayHandler.GetJTokenValue(item, "toMember", "trackingId");
                    linkedinUser.TrackingId = string.IsNullOrEmpty(linkedinUser?.TrackingId)
                        ? jsonJArrayHandler.GetJTokenValue(item, "heroInvitations", 0, "toMember", "trackingId") : linkedinUser.TrackingId;
                    if (string.IsNullOrEmpty(linkedinUser.TrackingId))
                        linkedinUser.TrackingId =
                            Utils.AssignNa(jsonJArrayHandler.GetJTokenValue(item, "heroInvitations", 0, "trackingId"));


                    // requestedTimeStamp
                    var sentTime = jsonJArrayHandler.GetJTokenValue(item, "sentTime");
                    if (string.IsNullOrEmpty(sentTime))
                        sentTime = jsonJArrayHandler.GetJTokenValue(item, "heroInvitations", 0, "sentTime");
                    long.TryParse(sentTime, out long requestedTimeStamp);
                    linkedinUser.RequestedTimeStamp = requestedTimeStamp;


                    //InvitationId
                    if (string.IsNullOrEmpty(linkedinUser.InvitationId =
                        jsonJArrayHandler.GetJTokenValue(item, "entityUrn")))
                        linkedinUser.InvitationId =
                            jsonJArrayHandler.GetJTokenValue(item, "heroInvitations", 0, "entityUrn");

                    if (linkedinUser.InvitationId.Contains("urn:li:fs_relInvitation"))
                        linkedinUser.InvitationId = string.IsNullOrEmpty(linkedinUser.InvitationId)
                            ? "N/A"
                            : linkedinUser.InvitationId.Replace("urn:li:fs_relInvitation:",
                                "");


                    #region Occupation

                    try
                    {
                        var occupation = jsonJArrayHandler.GetJTokenValue(item, "toMember", "occupation");
                        if (string.IsNullOrEmpty(occupation))
                            occupation = jsonJArrayHandler.GetJTokenValue(item, "heroInvitations", 0, "toMember",
                                "occupation");
                        linkedinUser.Occupation = Utils.InsertSpecialCharactersInCsv(occupation);
                    }
                    catch (Exception)
                    {
                        linkedinUser.Occupation = "N/A";
                    }

                    #endregion

                    #region CompanyName

                    try
                    {
                        if (linkedinUser.Occupation != "N/A" && linkedinUser.Occupation.Contains(" at "))
                            linkedinUser.CompanyName = Regex.Split(linkedinUser.Occupation, " at ")[1];
                    }
                    catch
                    {
                        // ignored
                    }

                    #endregion

                    if (!LstAllPendingConnectionRequest.Contains(linkedinUser))
                        LstAllPendingConnectionRequest.Add(linkedinUser);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                    GlobusLogHelper.log.Info(
                        "User does not contain profile informations or any unique public identifier or Email Address");
                }
            }
        }

        private void GetUserListFromWebResponse(IResponseParameter response)
        {
            var invitationNodesList = HtmlAgilityHelper.GetListNodesFromClassName(response.Response,
                "invitation-card artdeco-list__item ember-view");
            if(invitationNodesList.Count<=0)
                invitationNodesList = HtmlAgilityHelper.GetListNodesFromClassName(response.Response, "invitation-card artdeco-list__item");

            var ldDataHelper = LdDataHelper.GetInstance;

            foreach (var htmlNode in invitationNodesList)
            {
                var userResponse = htmlNode.OuterHtml;
                var FullName = Utils.GetBetween(HtmlAgilityHelper.GetListInnerHtmlFromClassName(userResponse, "invitation-card__details")?.FirstOrDefault(), "<!---->", "<!---->");
                var linkedInUser = new LinkedinUser(ldDataHelper.GetPublicIdentifierFromPageSource(userResponse))
                {
                    FullName = string.IsNullOrEmpty(FullName)? HtmlAgilityHelper.GetStringInnerTextFromClassName(userResponse,
                        "invitation-card__title t-16 t-black t-bold"):FullName,
                    InvitationId = ldDataHelper.GetInvitationIdFromPageSource(userResponse),
                    HeadlineTitle = HttpUtility.HtmlDecode(
                        HtmlAgilityHelper.GetStringInnerTextFromClassName(htmlNode,
                            "invitation-card__subtitle t-14 t-black--light t-normal"))
                };
                var timeValue = Utils.GetBetween(userResponse, "time-badge time-ago", "<")?.Trim();
                if(string.IsNullOrEmpty(timeValue))
                    timeValue= HtmlAgilityHelper.GetStringInnerTextFromClassName(htmlNode,
                            "time-badge t-12 t-black--light t-normal");
                linkedInUser.RequestedTimeStamp = ldDataHelper.GetTimeStamp(timeValue);
                linkedInUser.NodeResponse = userResponse;
                LstAllPendingConnectionRequest.Add(linkedInUser);
            }
        }
    }
}