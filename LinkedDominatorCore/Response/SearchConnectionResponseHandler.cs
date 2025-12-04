using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.LdTables.Account;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Utility;
using EmbeddedBrowser;
using HtmlAgilityPack;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDUtility;
using LinkedDominatorCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;

namespace LinkedDominatorCore.Response
{
    public class SearchConnectionResponseHandler : LdResponseHandler
    {
        public SearchConnectionResponseHandler(IResponseParameter response) : base(response)
        {
            try
            {
                Success = !string.IsNullOrEmpty(response?.Response) && (response.Response.Contains("\"type\":\"ConnectionsGetAll\"") || response.Response.Contains("<!DOCTYPE html>"));
                if (!Success)
                    return;
                if (response.Response.Contains("<!DOCTYPE html>"))
                {
                    BrowserConnectionUpdate();
                    return;
                }
                var jsonJArrayHandler = JsonJArrayHandler.GetInstance;
                var arr = jsonJArrayHandler.GetJArrayElement(jsonJArrayHandler.GetJTokenValue(RespJ, "elements"));
                if (response.Response.Contains(
                        "{\"metadata\":{\"id\":\"0\",\"type\":\"ConnectionsGetAll\"},\"elements\":[]") ||
                    response.Response.Contains("elements\":[]"))
                {
                    Success = false;
                    return;
                }

                #region MyRegion

                foreach (var item in arr)
                    try
                    {
                        var publicIdentifier =
                            jsonJArrayHandler.GetJTokenValue(item, "miniProfile", "publicIdentifier");
                        var linkedinUser = new LinkedinUser(publicIdentifier) { PublicIdentifier = publicIdentifier };

                        // firstName
                        var firstName =
                            jsonJArrayHandler.GetJTokenValue(item, "miniProfile",
                                "firstName");
                        // lastName
                        var lastName =
                            jsonJArrayHandler.GetJTokenValue(item, "miniProfile",
                                "lastName");


                        // FullName
                        linkedinUser.FullName = firstName + " " + lastName;
                        linkedinUser.FullName = Utils.InsertSpecialCharactersInCsv(linkedinUser.FullName);


                        //ProfileId
                        linkedinUser.ProfileId = jsonJArrayHandler.GetJTokenValue(item, "miniProfile", "entityUrn");
                        linkedinUser.ProfileId = string.IsNullOrEmpty(linkedinUser.ProfileId)
                            ? "N/A"
                            : linkedinUser.ProfileId.Replace("urn:li:fs_miniProfile", "").Replace(":", "");

                        #region ProfileUrl,HasAnonymousProfilePicture,ProfilePicUrl

                        if (!string.IsNullOrEmpty(linkedinUser.PublicIdentifier))
                        {
                            linkedinUser.ProfileUrl = "https://www.linkedin.com/in/" + linkedinUser.PublicIdentifier;
                            var backgroundImage = Utils.AssignNa(jsonJArrayHandler.GetJTokenValue(item, "miniProfile",
                                "backgroundImage", "com.linkedin.common.VectorImage"));
                            var Picture = jsonJArrayHandler.GetJTokenValue(item, "miniProfile",
                                "picture", "com.linkedin.common.VectorImage", "rootUrl")
                                +jsonJArrayHandler.GetJTokenValue(jsonJArrayHandler.GetJArrayElement(jsonJArrayHandler.GetJTokenValue(item, "miniProfile",
                                "picture", "com.linkedin.common.VectorImage", "artifacts"))?.LastOrDefault(x=>x.ToString().Contains("\"width\": 800") ||x.ToString().Contains("\"width\": 400") ||x.ToString().Contains("\"width\": 200") || x.ToString().Contains("\"width\": 100")), "fileIdentifyingUrlPathSegment");
                            var picture =string.IsNullOrEmpty(Picture)? Utils.AssignNa(jsonJArrayHandler.GetJTokenValue(item, "miniProfile",
                                "picture", "com.linkedin.common.VectorImage")):Picture;
                            

                            if (backgroundImage != "N/A" || picture != "N/A")
                            {
                                linkedinUser.HasAnonymousProfilePicture = true;
                                linkedinUser.ProfilePicUrl = picture;
                            }
                        }

                        #endregion

                        //TrackingId
                        linkedinUser.TrackingId = jsonJArrayHandler.GetJTokenValue(item, "miniProfile", "trackingId");


                        #region Connected date

                        try
                        {
                            var connectedTimeStamp = (long)item["createdAt"];
                            linkedinUser.ConnectedTimeStamp = connectedTimeStamp;
                            linkedinUser.ConnectionType = ConnectionType.FirstDegree;
                        }
                        catch (Exception)
                        {
                            // ex.DebugLog();
                            linkedinUser.ConnectedTimeStamp = 0;
                            linkedinUser.ConnectionType = 0;
                        }

                        #endregion

                        #region Occupation

                        try
                        {
                            var occupation = item["miniProfile"]["occupation"]?.ToString();
                            linkedinUser.Occupation = occupation;
                            linkedinUser.Occupation = Utils.InsertSpecialCharactersInCsv(linkedinUser.Occupation);
                            linkedinUser.HeadlineTitle = linkedinUser.Occupation;
                        }
                        catch (Exception)
                        {
                            // ex.DebugLog();
                            linkedinUser.Occupation = "N/A";
                            linkedinUser.HeadlineTitle = linkedinUser.Occupation;
                        }

                        #endregion

                        #region Location

                        try
                        {
                            var location = item["miniProfile"]["location"]?.ToString();
                            linkedinUser.Location = location;
                            linkedinUser.Location = Utils.InsertSpecialCharactersInCsv(linkedinUser.Location);
                        }
                        catch (Exception)
                        {
                            //Ignored
                            linkedinUser.Location = "N/A";
                        }

                        #endregion

                        #region CompanyName

                        try
                        {
                            if (linkedinUser.Occupation != "N/A" && linkedinUser.Occupation.Contains(" at "))
                                linkedinUser.CompanyName = Regex.Split(linkedinUser.Occupation, " at ")[1];
                        }
                        catch (Exception)
                        {
                            // ex.DebugLog();
                        }

                        #endregion

                        linkedinUser.Industry = string.IsNullOrEmpty(linkedinUser.Industry)
                            ? "N/A"
                            : Utils.InsertSpecialCharactersInCsv(linkedinUser.Industry);

                        if (ConnectionsList.All(x => x.ProfileUrl != linkedinUser.ProfileUrl))
                            ConnectionsList.Add(linkedinUser);
                    }
                    catch (Exception)
                    {
                        GlobusLogHelper.log.Info(
                            "publicIdentifier doesnot exist on getting List of users from your search query");
                    }

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public SearchConnectionResponseHandler(IResponseParameter response, bool isReplyToAllMessagesChecked,
            string specificWords, bool isReplyToAllUsersWhodidnotReply, string userId) : base(response)
        {
            try
            {
                Success = !string.IsNullOrEmpty(response?.Response) && (response.Response.Contains("urn:li:fsd_conversation") || response.Response.Contains("<!DOCTYPE html>"));
                if (!Success)
                    return;

                #region lst_specificword

                List<string> lstSpecificWord = null;
                if (specificWords != null)
                    lstSpecificWord = Regex.Split(specificWords, ",").ToList();

                #endregion

                var jsonArray = JsonJArrayHandler.GetInstance;
                var arr = jsonArray.GetJArrayElement(jsonArray.GetJTokenValue(RespJ, "elements"));

                if (arr is null ||!arr.HasValues)
                {
                    Success = false;
                    return;
                }

                HasMoreResults = !(arr.Count < 20);

                var loopCount = 0;

                #region MyRegion

                foreach (var item in arr)
                {
                    loopCount = ++loopCount;

                    #region MessageContent

                    var messageContent = string.Empty;
                    var blocked = jsonArray.GetJTokenValue(item, "withNonConnection");
                    try
                    {
                       messageContent = jsonArray.GetJTokenValue(item, "events",0, "eventContent", "com.linkedin.voyager.messaging.event.MessageEvent", "attributedBody","text");
                    }
                    catch (Exception)
                    {
                        // ex.DebugLog();
                        messageContent = "N/A";
                    }

                    #endregion

                    #region MyRegion

                    var firstName = string.Empty;
                    var lastName = string.Empty;
                    var publicIdentifier = string.Empty;
                    long connectedTimeStamp = 0;
                    var occupation = string.Empty;
                    var messageThreadId = jsonArray.GetJTokenValue(item, "dashEntityUrn")?.Replace("urn:li:fsd_conversation:","");
                    #endregion

                    if (lstSpecificWord != null && lstSpecificWord.Count != 0 && !isReplyToAllUsersWhodidnotReply)
                    {
                        #region Filter Message according to SpecificWords and then add the connection to the list

                        if ((!string.IsNullOrEmpty(messageContent) || messageContent != "N/A") &&
                            lstSpecificWord[0] != "" &&
                            lstSpecificWord.Any(x => messageContent.ToLower().Contains(x.ToLower())))
                            if (jsonArray.GetJTokenValue(item,"events") != null && blocked == "False")
                                try
                                {
                                    if (jsonArray.GetJTokenValue(item,"events",0)
                                        .Contains("com.linkedin.voyager.messaging.MessagingMember"))
                                    {
                                        var details = jsonArray.GetJTokenOfJToken(item, "events", 0, "from", "com.linkedin.voyager.messaging.MessagingMember", "miniProfile");
                                        publicIdentifier = jsonArray.GetJTokenValue(details,"publicIdentifier");

                                        #region FirstName

                                        try
                                        {
                                            firstName = jsonArray.GetJTokenValue(details, "firstName");
                                            var messageType = jsonArray.GetJTokenValue(item, "events",0, "subtype");
                                            if (firstName == "LinkedIn Member" || messageType == "SPONSORED_MESSAGE") continue;
                                        }
                                        catch (Exception)
                                        {
                                        }

                                        #endregion

                                        #region Creating New object for LinkedinUser

                                        var linkedinUser = new LinkedinUser(publicIdentifier);
                                        linkedinUser.PublicIdentifier = publicIdentifier;

                                        #endregion

                                        #region lastName

                                        try
                                        {
                                            lastName = jsonArray.GetJTokenValue(details, "lastName");
                                        }
                                        catch (Exception)
                                        {
                                        }

                                        #endregion

                                        #region FullName

                                        try
                                        {
                                            linkedinUser.FullName = firstName + " " + lastName;
                                            linkedinUser.FullName =
                                                Utils.InsertSpecialCharactersInCsv(linkedinUser.FullName);
                                        }
                                        catch (Exception)
                                        {
                                        }

                                        #endregion

                                        #region ProfileId

                                        try
                                        {
                                            linkedinUser.ProfileId = jsonArray.GetJTokenValue(details, "entityUrn");
                                            if (linkedinUser.ProfileId.Contains("urn:li:fs_miniProfile"))
                                                try
                                                {
                                                    linkedinUser.ProfileId =
                                                        Regex.Split(linkedinUser.ProfileId, "urn:li:fs_miniProfile")[1]
                                                            .Replace(":", "");
                                                }
                                                catch (Exception)
                                                {
                                                }
                                        }
                                        catch (Exception)
                                        {
                                            linkedinUser.ProfileId = "N/A";
                                        }

                                        #endregion

                                        #region ProfileUrl,HasAnonymousProfilePicture,ProfilePicUrl

                                        if (!string.IsNullOrEmpty(linkedinUser.PublicIdentifier))
                                        {
                                            linkedinUser.ProfileUrl =
                                                "https://www.linkedin.com/in/" + linkedinUser.PublicIdentifier;
                                            var backgroundImage = string.Empty;
                                            var picture = string.Empty;
                                            try
                                            {
                                                backgroundImage = jsonArray.GetJTokenValue(details, "backgroundImage", "com.linkedin.common.VectorImage","rootUrl")+jsonArray.GetJTokenValue(jsonArray.GetJArrayElement(jsonArray.GetJTokenValue(details, "backgroundImage", "com.linkedin.common.VectorImage", "artifacts")).LastOrDefault(x=> x.ToString().Contains(" \"width\": 1400") || x.ToString().Contains(" \"width\": 800")|| x.ToString().Contains(" \"width\": 400")|| x.ToString().Contains(" \"width\": 200")|| x.ToString().Contains(" \"width\": 100")), "fileIdentifyingUrlPathSegment");
                                            }
                                            catch (Exception)
                                            {
                                                backgroundImage = "N/A";
                                            }

                                            try
                                            {
                                                picture = jsonArray.GetJTokenValue(details, "picture", "com.linkedin.common.VectorImage", "rootUrl") + jsonArray.GetJTokenValue(jsonArray.GetJArrayElement(jsonArray.GetJTokenValue(details, "picture", "com.linkedin.common.VectorImage", "artifacts")).LastOrDefault(x => x.ToString().Contains(" \"width\": 800") || x.ToString().Contains(" \"width\": 400") || x.ToString().Contains(" \"width\": 200") || x.ToString().Contains(" \"width\": 100")), "fileIdentifyingUrlPathSegment");
                                            }
                                            catch (Exception)
                                            {
                                                picture = "N/A";
                                            }

                                            if (backgroundImage != "N/A" || picture != "N/A")
                                            {
                                                linkedinUser.HasAnonymousProfilePicture = true;
                                                linkedinUser.ProfilePicUrl = picture;
                                            }
                                        }

                                        #endregion

                                        #region TrackingId

                                        try
                                        {
                                            linkedinUser.TrackingId = jsonArray.GetJTokenValue(details, "trackingId");
                                        }
                                        catch (Exception)
                                        {
                                        }

                                        #endregion

                                        #region Connected date

                                        try
                                        {
                                            connectedTimeStamp = (long)item["events"][0]["createdAt"];
                                            linkedinUser.ConnectedTimeStamp = connectedTimeStamp;
                                        }
                                        catch (Exception)
                                        {
                                        }

                                        #endregion

                                        #region Occupation

                                        try
                                        {
                                            occupation = jsonArray.GetJTokenValue(details, "occupation");
                                            linkedinUser.Occupation = occupation;
                                            linkedinUser.Occupation =
                                                Utils.InsertSpecialCharactersInCsv(linkedinUser.Occupation);
                                        }
                                        catch (Exception)
                                        {
                                            linkedinUser.Occupation = "N/A";
                                        }

                                        #endregion

                                        #region CompanyName

                                        try
                                        {
                                            if (linkedinUser.Occupation != "N/A" &&
                                                linkedinUser.Occupation.Contains(" at "))
                                                linkedinUser.CompanyName =
                                                    Regex.Split(linkedinUser.Occupation, " at ")[1];
                                        }
                                        catch (Exception)
                                        {
                                        }

                                        #endregion

                                        #region MessageContent

                                        try
                                        {
                                            linkedinUser.MessageContent = jsonArray.GetJTokenValue(item, "events",0, "eventContent", "com.linkedin.voyager.messaging.event.MessageEvent", "attributedBody","text");
                                        }
                                        catch (Exception)
                                        {
                                            linkedinUser.MessageContent = "N/A";
                                        }

                                        #endregion

                                        #region Selected Message Filter

                                        try
                                        {
                                            var selectedMessageFilter = lstSpecificWord.FirstOrDefault(x =>
                                                messageContent.ToLower().Contains(x.ToLower()));
                                            linkedinUser.SelectedMessageFilter = selectedMessageFilter;
                                        }
                                        catch (Exception)
                                        {
                                        }

                                        #endregion

                                        // connected time stamp
                                        if (long.TryParse(jsonArray.GetJTokenValue(item, "events", 0, "createdAt"),
                                            out connectedTimeStamp))
                                            linkedinUser.ConnectedTimeStamp = connectedTimeStamp;
                                        linkedinUser.MessageThreadId = messageThreadId;
                                        if (!ConnectionsList.Contains(linkedinUser)) ConnectionsList.Add(linkedinUser);
                                    }
                                }
                                catch (Exception)
                                {
                                    GlobusLogHelper.log.Info(
                                        "publicIdentifier doesnot exist on getting List of users from your search query");
                                }

                        #endregion
                    }

                    if (messageContent != "N/A" && isReplyToAllMessagesChecked)
                    {
                        #region For All Message Add Connection To ConnectionsList

                        if (jsonArray.GetJTokenValue(item,"events") != null)
                            try
                            {
                                if (jsonArray.GetJTokenValue(item,"events",0)
                                    .Contains("com.linkedin.voyager.messaging.MessagingMember"))
                                {
                                    var details = jsonArray.GetJTokenOfJToken(item, "events",0,"from", "com.linkedin.voyager.messaging.MessagingMember", "miniProfile");
                                    publicIdentifier = jsonArray.GetJTokenValue(details, "publicIdentifier");

                                    #region Creating New object for LinkedinUser

                                    var linkedinUser = new LinkedinUser(publicIdentifier);

                                    #endregion


                                    #region FirstName

                                    try
                                    {
                                        firstName = jsonArray.GetJTokenValue(details, "firstName");
                                    }
                                    catch (Exception)
                                    {
                                    }

                                    #endregion

                                    #region lastName

                                    try
                                    {
                                        lastName = jsonArray.GetJTokenValue(details, "lastName");
                                    }
                                    catch (Exception)
                                    {
                                    }

                                    #endregion

                                    #region FullName

                                    try
                                    {
                                        linkedinUser.FullName = firstName + " " + lastName;
                                        linkedinUser.FullName =
                                            Utils.InsertSpecialCharactersInCsv(linkedinUser.FullName?.Trim());
                                    }
                                    catch (Exception)
                                    {
                                    }
                                    var messageType = jsonArray.GetJTokenValue(item, "events",0, "subtype");
                                    #endregion

                                    if (linkedinUser.FullName == "LinkedIn Member" ||
                                        string.IsNullOrEmpty(linkedinUser.FullName) || messageType== "SPONSORED_MESSAGE")
                                        continue;

                                    #region ProfileId

                                    try
                                    {
                                        linkedinUser.ProfileId = jsonArray.GetJTokenValue(details, "entityUrn");
                                        if (linkedinUser.ProfileId.Contains("urn:li:fs_miniProfile"))
                                            try
                                            {
                                                linkedinUser.ProfileId =
                                                    Regex.Split(linkedinUser.ProfileId, "urn:li:fs_miniProfile")[1]
                                                        .Replace(":", "");
                                            }
                                            catch (Exception)
                                            {
                                            }
                                    }
                                    catch (Exception)
                                    {
                                        linkedinUser.ProfileId = "N/A";
                                    }

                                    #endregion

                                    #region ProfileUrl,HasAnonymousProfilePicture,ProfilePicUrl

                                    if (!string.IsNullOrEmpty(linkedinUser.PublicIdentifier))
                                    {
                                        linkedinUser.ProfileUrl =
                                            "https://www.linkedin.com/in/" + linkedinUser.PublicIdentifier;
                                        var backgroundImage = string.Empty;
                                        var picture = string.Empty;
                                        try
                                        {
                                            backgroundImage = jsonArray.GetJTokenValue(details, "backgroundImage", "com.linkedin.common.VectorImage","rootUrl") + jsonArray.GetJTokenValue(jsonArray.GetJArrayElement(jsonArray.GetJTokenValue(details, "backgroundImage", "com.linkedin.common.VectorImage", "artifacts")).LastOrDefault(x => x.ToString().Contains(" \"width\": 1400") || x.ToString().Contains(" \"width\": 800") || x.ToString().Contains(" \"width\": 400") || x.ToString().Contains(" \"width\": 200") || x.ToString().Contains(" \"width\": 100")), "fileIdentifyingUrlPathSegment");
                                        }
                                        catch (Exception)
                                        {
                                            backgroundImage = "N/A";
                                        }

                                        try
                                        {
                                            picture = jsonArray.GetJTokenValue(details, "picture", "com.linkedin.common.VectorImage", "rootUrl") + jsonArray.GetJTokenValue(jsonArray.GetJArrayElement(jsonArray.GetJTokenValue(details, "picture", "com.linkedin.common.VectorImage", "artifacts")).LastOrDefault(x => x.ToString().Contains(" \"width\": 800") || x.ToString().Contains(" \"width\": 400") || x.ToString().Contains(" \"width\": 200") || x.ToString().Contains(" \"width\": 100")), "fileIdentifyingUrlPathSegment");
                                        }
                                        catch (Exception)
                                        {
                                            picture = "N/A";
                                        }

                                        if (backgroundImage != "N/A" || picture != "N/A")
                                        {
                                            linkedinUser.HasAnonymousProfilePicture = true;
                                            linkedinUser.ProfilePicUrl = picture;
                                        }
                                    }

                                    #endregion

                                    #region TrackingId

                                    try
                                    {
                                        linkedinUser.TrackingId = jsonArray.GetJTokenValue(details, "trackingId");
                                    }
                                    catch (Exception)
                                    {
                                    }

                                    #endregion

                                    #region Connected date

                                    try
                                    {
                                        connectedTimeStamp = (long)item["events"][0]["createdAt"];
                                        linkedinUser.ConnectedTimeStamp = connectedTimeStamp;
                                    }
                                    catch (Exception)
                                    {
                                    }

                                    #endregion

                                    #region Occupation

                                    try
                                    {
                                        occupation = jsonArray.GetJTokenValue(details, "occupation");
                                        linkedinUser.Occupation = occupation;
                                        linkedinUser.Occupation =
                                            Utils.InsertSpecialCharactersInCsv(linkedinUser.Occupation);
                                    }
                                    catch (Exception)
                                    {
                                        linkedinUser.Occupation = "N/A";
                                    }

                                    #endregion

                                    #region CompanyName

                                    try
                                    {
                                        if (linkedinUser.Occupation != "N/A" &&
                                            linkedinUser.Occupation.Contains(" at "))
                                            linkedinUser.CompanyName =
                                                Regex.Split(linkedinUser.Occupation, " at ")[1];
                                    }
                                    catch (Exception)
                                    {
                                    }

                                    #endregion

                                    #region MessageContent

                                    try
                                    {
                                        linkedinUser.MessageContent = jsonArray.GetJTokenValue(item, "events",0, "eventContent", "com.linkedin.voyager.messaging.event.MessageEvent", "attributedBody","text");
                                    }
                                    catch (Exception)
                                    {
                                        linkedinUser.MessageContent = "N/A";
                                    }

                                    #endregion

                                    #region Selected Message Filter

                                    try
                                    {
                                        linkedinUser.SelectedMessageFilter = Application.Current
                                            .FindResource("LangKeyReplyToAllMessages").ToString();
                                    }
                                    catch (Exception)
                                    {
                                    }

                                    #endregion

                                    if (long.TryParse(jsonArray.GetJTokenValue(item, "events", 0, "createdAt"),
                                        out connectedTimeStamp))
                                        linkedinUser.ConnectedTimeStamp = connectedTimeStamp;
                                    linkedinUser.MessageThreadId = messageThreadId;
                                    if (!ConnectionsList.Any(x => x.ProfileId == linkedinUser.ProfileId))
                                        ConnectionsList.Add(linkedinUser);

                                }
                            }
                            catch (Exception)
                            {
                                GlobusLogHelper.log.Info(
                                    "publicIdentifier doesnot exist on getting List of users from your search query");
                            }

                        #endregion
                    }

                    if (messageContent != "N/A" && isReplyToAllUsersWhodidnotReply)
                        if (jsonArray.GetJTokenValue(item,"events") != null)
                            try
                            {
                                if (jsonArray.GetJTokenValue(item,"events")
                                    .Contains("com.linkedin.voyager.messaging.MessagingMember"))
                                {
                                    var ProfileId = jsonArray.GetJTokenValue(item, "events", 0, "from",
                                        "com.linkedin.voyager.messaging.MessagingMember", "miniProfile", "entityUrn");
                                    ProfileId = Regex.Split(ProfileId, "urn:li:fs_miniProfile")[1].Replace(":", "");
                                    if (ProfileId != userId)
                                        continue;

                                    firstName = jsonArray.GetJTokenValue(item, "participants", 0,
                                    "com.linkedin.voyager.messaging.MessagingMember", "miniProfile", "firstName");
                                    lastName = jsonArray.GetJTokenValue(item, "participants", 0,
                                        "com.linkedin.voyager.messaging.MessagingMember", "miniProfile", "lastName");
                                    publicIdentifier = jsonArray.GetJTokenValue(item, "participants", 0,
                                        "com.linkedin.voyager.messaging.MessagingMember", "miniProfile",
                                        "publicIdentifier");
                                    var linkedinUser = new LinkedinUser(publicIdentifier);
                                    linkedinUser.FullName = firstName + " " + lastName;
                                    linkedinUser.FullName =
                                        Utils.InsertSpecialCharactersInCsv(linkedinUser.FullName?.Trim());
                                    linkedinUser.MessageContent = messageContent;
                                    if (linkedinUser.FullName == "LinkedIn Member" ||
                                        string.IsNullOrEmpty(linkedinUser.FullName))
                                        continue;
                                    linkedinUser.ProfileId =
                                        Regex.Split(
                                            jsonArray.GetJTokenValue(item, "participants", 0,
                                                "com.linkedin.voyager.messaging.MessagingMember", "miniProfile",
                                                "entityUrn"), "urn:li:fs_miniProfile")[1].Replace(":", "");
                                    var backgroundImage = string.Empty;
                                    var picture = string.Empty;
                                    picture = jsonArray.GetJTokenValue(item, "participants", 0,
                                        "com.linkedin.voyager.messaging.MessagingMember", "miniProfile", "picture",
                                        "com.linkedin.common.VectorImage", "artifacts", 0,
                                        "fileIdentifyingUrlPathSegment");
                                    var rootPictureUrl = jsonArray.GetJTokenValue(item, "participants", 0,
                                        "com.linkedin.voyager.messaging.MessagingMember", "miniProfile", "picture",
                                        "com.linkedin.common.VectorImage", "rootUrl");
                                    picture = $"{rootPictureUrl}{picture}";
                                    if (backgroundImage != "N/A" || picture != "N/A")
                                    {
                                        linkedinUser.HasAnonymousProfilePicture = true;
                                        linkedinUser.ProfilePicUrl = picture;
                                    }
                                    if (long.TryParse(jsonArray.GetJTokenValue(item, "events", 0, "createdAt"),
                                        out connectedTimeStamp))
                                        linkedinUser.ConnectedTimeStamp = connectedTimeStamp;

                                    linkedinUser.TrackingId = jsonArray.GetJTokenValue(item, "participants", 0,
                                        "com.linkedin.voyager.messaging.MessagingMember", "miniProfile", "trackingId");
                                    linkedinUser.Occupation = jsonArray.GetJTokenValue(item, "participants", 0,
                                        "com.linkedin.voyager.messaging.MessagingMember", "miniProfile", "occupation");
                                    linkedinUser.SelectedMessageFilter = Application.Current
                                        .FindResource("LangKeyReplyToAllUserMessageWhodidnotreply").ToString();
                                    linkedinUser.MessageThreadId = messageThreadId;
                                    if (!ConnectionsList.Any(x => x.ProfileId == linkedinUser.ProfileId))
                                    {
                                        //if users last message contains then send it again else skip user
                                        if (lstSpecificWord!=null)
                                        {
                                            if (lstSpecificWord[0] != "")
                                            {
                                                if (lstSpecificWord.Any(x => messageContent.ToLower().Contains(x.ToLower())))
                                                    ConnectionsList.Add(linkedinUser);
                                            }
                                            else
                                                ConnectionsList.Add(linkedinUser); 
                                        }
                                        else
                                            ConnectionsList.Add(linkedinUser);
                                    }
                                }
                            }
                            catch (Exception)
                            {
                            }

                    if (loopCount == arr.Count && item["events"][0].ToString()
                            .Contains("com.linkedin.voyager.messaging.MessagingMember"))
                        LastConnectedTimeStamp = (long)item["events"][0]["createdAt"];


                }

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public SearchConnectionResponseHandler(IResponseParameter response, bool isReplyToAllMessagesChecked,
            string specificWords, BrowserWindow browserWindow,
            BrowserAutomationExtension automation, bool isReplyToAllUsersWhodidnotReply,List<HtmlNode> ChatNodeList=null) : base(response)
        {
            try
            {
                var UsersNodeList = new List<HtmlNode>();
                var ChatNodeList1 = HtmlParseUtility.GetListNodeFromPartialTagNamecontains(browserWindow.GetPageSource(),
                        HTMLTags.List, HTMLTags.HtmlAttribute.Class, LDClassesConstant.Messenger.ChatUserListClass);
                var UserCount = 0;
                foreach (var user in ChatNodeList1)
                {
                    if (user == ChatNodeList.FirstOrDefault())
                        UsersNodeList.Add(user);
                    else if (user.InnerHtml.Contains(LDClassesConstant.Messenger.NewMessageNotificationClass))
                        UsersNodeList.Add(user);
                }
                foreach (var user in UsersNodeList)
                {
                    if (UserCount > 20)
                        break;
                    UserCount++;
                    var currentNodeId = UserCount > 1 ? GetCurrentNodeID(user, browserWindow.GetPageSource()):user.Id;
                    browserWindow.ExecuteScript($"document.getElementById('{currentNodeId}').scrollIntoView();", 4);
                    //Thread.Sleep(TimeSpan.FromSeconds(50));
                    var done = browserWindow.ExecuteScript($"document.querySelector('li[id=\"{currentNodeId}\"]>div>div div[role=\"button\"]').click();", 4);
                    if(done != null && !done.Success)
                    {
                        done = browserWindow.ExecuteScript($"document.querySelector('li[id=\"{currentNodeId}\"]>div>div>a').click();", 4);
                        if (done == null || !done.Success)
                        {
                            done = browserWindow.ExecuteScript($"document.querySelector('li[id=\"{currentNodeId}\"]').click();", 4);
                            if(done == null || !done.Success)
                                continue;
                        }
                            
                    }
                    var messageNodeList = HtmlAgilityHelper.GetListNodesFromClassName(browserWindow.GetPageSource(),
                    "msg-s-message-list__event clearfix\n");
                    var AcceptMessageRequestClass = LDClassesConstant.Messenger.AcceptMessageRequestClass;
                    if(browserWindow.GetPageSource().Contains(AcceptMessageRequestClass))
                        browserWindow.ExecuteScript($"document.getElementsByClassName('{AcceptMessageRequestClass}')[0].click();", 3);
                    if (!browserWindow.GetPageSource().Contains("Write a message"))
                        continue;
                    var dbOperations =
                        InstanceProvider.ResolveAccountDbOperations(browserWindow.DominatorAccountModel.AccountId,
                            SocialNetworks.LinkedIn);
                    var connectionList = dbOperations.Get<Connections>();
                    var firstName = "";
                    browserWindow.DominatorAccountModel.ExtraParameters.TryGetValue("FirstName", out firstName);
                    var wordsList = new List<string>();
                    if (!string.IsNullOrEmpty(specificWords))
                        wordsList = specificWords.Split(',').ToList().Where(x => !string.IsNullOrEmpty(x.Trim())).ToList();

                    // here we first checking users from side scroll bar
                    // that contains 'You' means we replied already therefore we skipping them
                    try
                    {
                        if(messageNodeList?.LastOrDefault() != null && messageNodeList?.LastOrDefault()?.OuterHtml != null)
                        {
                            var innerText = HtmlParseUtility.GetInnerTextFromSingleNode(messageNodeList?.LastOrDefault()?.OuterHtml, HTMLTags.Span, HTMLTags.HtmlAttribute.Class, "msg-s-message-group__profile-link msg-s-message-group__name t-14 t-black t-bold hoverable-link-text");
                            if (!string.IsNullOrEmpty(innerText) && innerText.Contains(firstName))
                                continue;
                        }
                    }
                    catch { }
                    foreach (var messageNode in messageNodeList)
                    {
                        var fullName = Utils.RemoveHtmlTags(HtmlAgilityHelper.GetStringFromClassName(messageNode.OuterHtml,
                            "msg-s-message-group__profile-link msg-s-message-group__name t-14 t-black t-bold hoverable-link-text"));
                        if (string.IsNullOrEmpty(fullName))
                            fullName = Utils.RemoveHtmlTags(HtmlAgilityHelper.GetStringFromClassName(messageNode.OuterHtml,
                                "msg-conversation-listitem__participant-names msg-conversation-card__participant-names truncate pr1 t-16 t-black--light t-normal"));

                        if (string.IsNullOrEmpty(fullName))
                            fullName = LdDataHelper.GetInstance.GetAltFromPageSource(messageNode.OuterHtml);

                        if (fullName.Contains(","))
                            fullName = Utils.GetBetween("$$" + fullName, "$$", ",");
                        if (isReplyToAllUsersWhodidnotReply)
                        {
                            var innerText = HtmlParseUtility.GetInnerTextFromSingleNode(messageNode?.OuterHtml, HTMLTags.Span, HTMLTags.HtmlAttribute.Class, "msg-s-message-group__profile-link msg-s-message-group__name t-14 t-black t-bold hoverable-link-text");
                            if (!string.IsNullOrEmpty(innerText) && !innerText.Contains(fullName))
                                continue;
                        }
                        // checking user from UserFullName is present on db if not then it must be an promotion message
                        var single = connectionList.Where(x => x.FullName == fullName)?.FirstOrDefault();
                        if (single == null)
                            continue;

                        // if we didn't get full then select from full name
                        var node = HtmlAgilityHelper.GetListNodesFromClassName(messageNode.OuterHtml,
                                "msg-facepile-grid msg-facepile-grid--no-facepile msg-facepile-grid--large ember-view")
                            .FirstOrDefault();
                        if (node == null)
                            automation.ExecuteScript(AttributeIdentifierType.Xpath, $"//h3[text()='{fullName}']", 5);
                        else
                            automation.ExecuteScript(AttributeIdentifierType.Id, node?.Id, 5);

                        automation.ExecuteScript(
                            "document.getElementsByClassName('msg-s-message-list full-width ember-view')[0].scrollTop -=5000",
                            5);

                        var messagePage = HtmlAgilityHelper.GetStringFromClassName(browserWindow.GetPageSource(),
                            "msg-s-message-list-content list-style-none full-width");
                        if (string.IsNullOrEmpty(messagePage))
                            messagePage = HtmlAgilityHelper.GetStringFromClassName(browserWindow.GetPageSource(),
                            "scaffold-layout__list-detail-inner");
                        var ss = browserWindow.GetPageSource();
                        var lastMessage = HtmlAgilityHelper.GetListInnerHtmlFromClassName(messagePage,
                            "msg-s-message-list full-width ember-view").LastOrDefault();
                        lastMessage = HtmlAgilityHelper
                            .GetListInnerHtmlFromClassName(messagePage,
                                "msg-s-event-listitem__body t-14 t-black--light t-normal").LastOrDefault();
                        var messages = Utils.RemoveHtmlTags(lastMessage);
                        var lastMessagetime = string.Empty;
                        if (isReplyToAllUsersWhodidnotReply)
                        {
                            try
                            {   //if lastmessage time is null then it will take connected time
                                lastMessagetime = HtmlAgilityHelper
                                                  .GetListHtmlFromClassName(messagePage,
                                                      "msg-s-event-with-indicator__sending-indicator align-self-flex-end").LastOrDefault();

                                var currentTime = Utils.GetBetween(lastMessagetime, "title=\"Sent at ", "\">").Replace(",", "");
                                DateTime.TryParse(currentTime, out DateTime Date);
                                lastMessagetime = Date.ConvertToEpoch().ToString();
                            }
                            catch (Exception)
                            {
                            }
                        }
                        if (!isReplyToAllMessagesChecked && !Utils.IsContains(messages, wordsList.ToArray()))
                            if (!isReplyToAllUsersWhodidnotReply)
                                continue;

                        var linkedinUser = ClassMapper.Instance.MappedConnectionToLinkedInUser(single);
                        var selectedMessageFilter = wordsList.FirstOrDefault(x =>
                            messages.ToLower().Contains(x.ToLower()));
                        linkedinUser.SelectedMessageFilter = selectedMessageFilter;

                        linkedinUser.MessageContent = messages;
                        if (isReplyToAllUsersWhodidnotReply)
                        {
                            if (string.IsNullOrEmpty(lastMessagetime))
                                linkedinUser.ConnectedTimeStamp = int.Parse(linkedinUser.ConnectedTimeStamp.ToString().Substring(0, linkedinUser.ConnectedTimeStamp.ToString().Length - 3));
                            else
                                linkedinUser.ConnectedTimeStamp = int.Parse(lastMessagetime);
                        }
                        ConnectionsList.Add(linkedinUser);
                    }
                }
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }
        }

        private string GetCurrentNodeID(HtmlNode node,string PageSource)
        {
            var nodeId =string.Empty;
            try
            {
                var ChatNodeList = HtmlParseUtility.GetListNodeFromPartialTagNamecontains(PageSource,
                        HTMLTags.List, HTMLTags.HtmlAttribute.Class, LDClassesConstant.Messenger.ChatUserListClass);
                var oldText = node?.InnerText?.Replace("\n", "")?.Replace("\t", "")?.Replace(" ","");
                var currentNode = ChatNodeList.FirstOrDefault(x=>x.InnerText?.Replace("\n", "")?.Replace("\t", "")?.Replace(" ", "")==oldText);
                nodeId = currentNode?.Id ?? node?.Id;
            }
            catch { }
            return string.IsNullOrEmpty(nodeId) ? node.Id :nodeId;
        }

        public bool HasMoreResults { get; set; }

        public List<LinkedinUser> ConnectionsList { get; } = new List<LinkedinUser>();
        public long LastConnectedTimeStamp { get; set; }

        private void BrowserConnectionUpdate()
        {
            var connectionsList = HtmlAgilityHelper.GetListNodesFromClassName(WebUtility.HtmlDecode(Response.Response),
                "mn-connection-card artdeco-list ember-view");
            if(connectionsList.Count<=0)
                connectionsList = HtmlAgilityHelper.GetListNodesFromClassName(WebUtility.HtmlDecode(Response.Response),
                "mn-connection-card artdeco-list");
            var ldDataHelper = LdDataHelper.GetInstance;
            foreach (var connectionNode in connectionsList)
            {
                var outerHtml = connectionNode.OuterHtml;

                var publicIdentifier = ldDataHelper.GetPublicIdentifierFromPageSource(outerHtml)?.Trim('/');
                publicIdentifier=string.IsNullOrEmpty(publicIdentifier)?Utils.GetBetween(outerHtml,"<a href=\"/in/","/\""):publicIdentifier.Trim();
                var linkedinUser = new LinkedinUser(publicIdentifier);
                // don't use send 'connectionNode' here in param it is giving same name for all nodes use 'outerHtml'
                linkedinUser.FullName =
                    HtmlAgilityHelper.GetStringInnerHtmlFromClassName(outerHtml,
                        "mn-connection-card__name t-16 t-black t-bold");
                linkedinUser.Occupation =
                    HtmlAgilityHelper.GetStringInnerHtmlFromClassName(outerHtml,
                        "mn-connection-card__occupation t-14 t-black--light t-normal", htmlNode: connectionNode);
                // here HtmlAgilityHelper not getting exact response as aspected
                var result = Utils.GetBetween(outerHtml, "time-badge t-12 t-black--light t-normal", "<")?.Trim().Replace("\">","");
                linkedinUser.ConnectedTimeStamp = ldDataHelper.GetTimeStamp(result);

                linkedinUser.ProfilePicUrl = Utils.GetBetween(outerHtml, "<img src=\"", "\" loading=\"");
                ConnectionsList.Add(linkedinUser);
            }
        }
    }
}