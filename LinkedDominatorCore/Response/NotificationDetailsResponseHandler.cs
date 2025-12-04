using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using HtmlAgilityPack;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDUtility;
using LinkedDominatorCore.Utility;
using Newtonsoft.Json.Linq;

namespace LinkedDominatorCore.Response
{
    public class NotificationDetailsResponseHandler : LdResponseHandler
    {
        public NotificationDetailsResponseHandler(IResponseParameter response) : base(response)
        {
            try
            {
                if (!Success || Response.ToString().Contains("element\":[]"))
                    return;

                if (response.Response.Contains("<!DOCTYPE html>"))
                {
                    BrowserProcessResponse();
                    return;
                }

                var jsonJArrayHandler = JsonJArrayHandler.GetInstance;
                NextStart = jsonJArrayHandler.GetJTokenValue(RespJ,
                    "metadata", "nextStart");

                var notificationJArray = JArray.Parse(RespJ["elements"].ToString());

                foreach (var notificationItems in notificationJArray)
                    try
                    {
                        var entityUrn = jsonJArrayHandler.GetJTokenValue(notificationItems, "entityUrn");
                        // entityUrn = Utils.GetBetween(entityUrn + "**", "urn:li:genericNotification:", "**");

                        if (!entityUrn.Contains("BIRTHDAY_PROP") && !entityUrn.Contains("WORK_ANNIVERSARY_PROP") &&
                            !entityUrn.Contains("JOB_CHANGE_PROP")) continue;
                        try
                        {
                            #region if notification items appears in a Bunch all at once

                            var notificationCard = jsonJArrayHandler.GetJTokenValue(notificationItems, "actions");
                            if (string.IsNullOrEmpty(notificationCard))
                                notificationCard = jsonJArrayHandler.GetJTokenValue(notificationItems,
                                    "headerImage", "attributes");
                            var arrayOfNotificationItems = JArray.Parse(notificationCard);

                            foreach (var item in arrayOfNotificationItems)
                                GetNotificationDetails(entityUrn, item.ToString(),notificationItems.ToString());

                            #endregion
                        }
                        catch (Exception)
                        {
                            // ignored

                            if (!string.IsNullOrEmpty(GetActionType(notificationItems, jsonJArrayHandler, false)))
                                GetNotificationDetails(entityUrn, notificationItems["value"].ToString(),notificationItems.ToString());
                        }
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

        public List<LinkedinUser> LstNotificationDetails { get; } = new List<LinkedinUser>();

        public string NextStart { get; }

        private void BrowserProcessResponse()
        {
            try
            {
                var ldDataHelper = LdDataHelper.GetInstance;
                var notificationNodeList =
                    HtmlAgilityHelper.GetListNodesFromClassName(WebUtility.HtmlDecode(Response.Response),
                        "nt-card--with-hover-states");
                foreach (var node in notificationNodeList)
                {
                    var publicIdentifier =
                        WebUtility.UrlDecode(ldDataHelper.GetPublicIdentifierFromPageSource(node.OuterHtml));
                    var linkedInUser = new LinkedinUser(publicIdentifier);
                    if (SetNotificationType(node, linkedInUser))
                        continue;


                    linkedInUser.FullName = Utils.GetBetween(node.OuterHtml, "alt=\"", "\"")?.Replace("View", "")
                        ?.Replace("’s profile", "")?.Trim();
                    if (node.OuterHtml.Contains($"You sent {linkedInUser.FullName.Split(' ')[0]} a message"))
                        continue;
                    var InvitationId = Utils.GetBetweenAndAddStart(node.OuterHtml, "/feed/update/urn", "\"");
                    InvitationId = string.IsNullOrEmpty(InvitationId) ? Utils.GetBetweenAndAddStart(node.OuterHtml, "/feed/?highlightedUpdateUrn", "\"") : InvitationId;
                    linkedInUser.InvitationId = "https://www.linkedin.com" + InvitationId;
                    linkedInUser.ProfilePicUrl = ldDataHelper.GetSource(node.OuterHtml);
                    if (string.IsNullOrEmpty(linkedInUser.ProfilePicUrl))
                        linkedInUser.ProfilePicUrl = Utils.GetBetween(node.OuterHtml, "<img src=\"", "\" loading=");
                    linkedInUser.TrackingId = $"{LstNotificationDetails.Count}";

                    var response = HtmlAgilityHelper.GetListNodesFromClassName(node.OuterHtml, "nt-card__left-rail mr2")
                        .First().InnerHtml;
                    linkedInUser.NodeId = Utils.GetBetween(response, "id=\"", "\"");
                    LstNotificationDetails.Add(linkedInUser);
                }
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }
        }

        private bool SetNotificationType(HtmlNode node, LinkedinUser linkedInUser)
        {
            if (node.OuterHtml.Contains("birthday"))
            {
                linkedInUser.NotificationType =
                    Application.Current.FindResource("LangKeyBirthdayGreeting")?.ToString();
            }
            else if (node.OuterHtml.Contains("for starting a new position") || node.OuterHtml.Contains("shared about starting a new position"))
            {
                linkedInUser.NotificationType =
                    Application.Current.FindResource("LangKeyNewJobGreeting")?.ToString();
            }
            else if (node.OuterHtml.Contains("years at") || node.OuterHtml.Contains("year at"))
            {
                if (string.IsNullOrEmpty(linkedInUser.Occupation = Utils.GetBetween(node.OuterHtml, "years at", "<")))
                    linkedInUser.CompanyName = Utils.GetBetween(node.OuterHtml, "year at", "<");
                linkedInUser.NotificationType =
                    Application.Current.FindResource("LangKeyWorkAnniversaryGreeting")?.ToString();
            }
            else
            {
                return true;
            }

            return false;
        }

        private string GetActionType(JToken jToken, JsonJArrayHandler jsonJArrayHandler,
            bool isBunchOfNotifications = true)
        {
            var action = isBunchOfNotifications
                ? jsonJArrayHandler.GetJTokenValue(jToken, "actions")
                : jsonJArrayHandler.GetJTokenValue(jToken, "value", "com.linkedin.voyager.identity.notifications.Card",
                    "actions");
            return Utils.GetBetween(action, "text\": \"", "\"");
        }

        public void GetNotificationDetails(string entityUrn, string notificationDetails, string notificationItem)
        {
            try
            {
                var linkedinUser = new LinkedinUser();

                #region Firstname

                var firstname = Utils.GetBetween(notificationDetails, "firstName\": \"", "\"");
                var lastName = Utils.GetBetween(notificationDetails, "lastName\": \"", "\"");

                #endregion


                #region FullName

                if (!string.IsNullOrEmpty(firstname))
                {
                    linkedinUser.FullName = firstname + " " + lastName;
                    linkedinUser.FullName = Utils.InsertSpecialCharactersInCsv(linkedinUser.FullName);
                }
                if (string.IsNullOrEmpty(linkedinUser.FullName))
                {
                    linkedinUser.FullName = Utils.GetBetween(notificationItem,"\"text\": \"Mute ", "\",");
                    linkedinUser.FullName = Utils.InsertSpecialCharactersInCsv(linkedinUser.FullName);
                }
                #endregion

                linkedinUser.ProfileId = Utils.GetBetween(notificationDetails,
                    "urn:li:fsd_profile:", "\"");

                if (string.IsNullOrEmpty(linkedinUser.ProfileId))
                {
                    linkedinUser.ProfileId = Utils.GetBetween(notificationItem, "urn:li:fsd_profile:", "\"");
                    linkedinUser.ProfileUrl =$"https://www.linkedin.com{Utils.GetBetween(notificationItem, "\"actionTarget\": \"", "\",")}";
                }
                linkedinUser.TrackingId = Utils.GetBetween(notificationDetails, "trackingId\": \"", "\"");

                if (string.IsNullOrEmpty(linkedinUser.TrackingId))
                {
                    linkedinUser.TrackingId = Utils.GetBetween(notificationItem, "\"trackingId\": \"", "\"");
                }
                #region title and Company

                linkedinUser.Occupation = Utils.GetBetween(notificationDetails, "occupation\": \"", "\"");
                linkedinUser.Occupation = Utils.InsertSpecialCharactersInCsv(linkedinUser.Occupation);

                if (!string.IsNullOrEmpty(linkedinUser.Occupation) && linkedinUser.Occupation.Contains(" at "))
                    try
                    {
                        linkedinUser.CompanyName = Regex.Split(linkedinUser.Occupation, " at ")[1];
                    }
                    catch (Exception)
                    {
                        // ignored
                        linkedinUser.CompanyName = "N/A";
                    }

                #endregion

                #region MemberId

                linkedinUser.MemberId =
                    Utils.GetBetween(notificationDetails, "objectUrn\":\"urn:li:member:", "\"");
                if (string.IsNullOrEmpty(linkedinUser.MemberId))
                {
                    linkedinUser.MemberId = Utils.GetBetween(notificationItem, "\"objectUrn\": \"urn:li:notificationV2:(urn:li:member:", ",");
                }
                #endregion
                var ThreadId = Utils.GetBetween(notificationDetails, "urn:li:activity:", "\"");
                ThreadId = string.IsNullOrEmpty(ThreadId) ? Utils.GetBetween(notificationDetails, "\"actionTarget\": \"", "\"")?.Split('/')?.LastOrDefault()?.Replace("%3A",":") : ThreadId;
                linkedinUser.MessageThreadId = ThreadId;

                #region NotificationType

                try
                {
                    linkedinUser.NotificationType = entityUrn;
                    if (linkedinUser.NotificationType.Contains("BIRTHDAY_PROP"))
                        linkedinUser.NotificationType =
                            Application.Current.FindResource("LangKeyBirthdayGreeting")?.ToString();

                    if (linkedinUser.NotificationType != null &&
                        linkedinUser.NotificationType.Contains("JOB_CHANGE_PROP"))
                        linkedinUser.NotificationType =
                            Application.Current.FindResource("LangKeyNewJobGreeting")?.ToString();

                    if (linkedinUser.NotificationType != null &&
                        linkedinUser.NotificationType.Contains("WORK_ANNIVERSARY_PROP"))
                        linkedinUser.NotificationType = Application.Current
                            .FindResource("LangKeyWorkAnniversaryGreeting")?.ToString();
                }
                catch (Exception)
                {
                    // ignored
                }

                #endregion

                #region UniqueNotificationSuffix

                linkedinUser.UniqueNotificationSuffix =
                    Utils.GetBetween(entityUrn + "**", "urn:li:genericNotification:", "**");

                #endregion

                if (!LstNotificationDetails.Contains(linkedinUser))
                    LstNotificationDetails.Add(linkedinUser);
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}