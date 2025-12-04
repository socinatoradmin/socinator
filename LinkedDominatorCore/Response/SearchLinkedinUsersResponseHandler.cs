using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.LdTables.Account;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using HtmlAgilityPack;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDUtility;
using LinkedDominatorCore.Utility;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace LinkedDominatorCore.Response
{
    public class SearchLinkedinUsersResponseHandler : LdResponseHandler
    {
        private readonly JsonJArrayHandler _jsonJArrayHandler = JsonJArrayHandler.GetInstance;

        // keyword
        public SearchLinkedinUsersResponseHandler(IResponseParameter response)
            : base(response)
        {
            try
            {
                Success = !string.IsNullOrEmpty(response?.Response) && (response.Response.Contains("\"searchDashClustersByAll\":")||response.Response.Contains("<!DOCTYPE html>"));
                if (!Success)
                    return;
                if (RespJ == null && Response.Response.Contains("<!DOCTYPE html>"))
                    GetUserListFromBrowserResponse();

                // if we are not getting users from response, it may be normal non browser 
                if (UsersList.Count == 0)
                    GetUserListFromNormalResponse();

                if (Response.Response.Contains("<!DOCTYPE html>"))
                {
                    TotalResultsInSearch = Regex.Replace(HtmlAgilityHelper.GetStringInnerTextFromClassName(response.Response, "pb2 t-black--light t-14")?.Replace("results", "").Trim(), "[^0-9.KM]", "");
                    TotalResultsInSearch = string.IsNullOrEmpty(TotalResultsInSearch) ? Regex.Replace(HtmlAgilityHelper.GetStringInnerTextFromClassName(response.Response, "t-14 flex align-items-center mlA pl3")?.Replace("results", "").Trim(), "[^0-9.KM]", "") : TotalResultsInSearch;
                }
                else
                {
                    TotalResultsInSearch = Regex.Replace(_jsonJArrayHandler.GetJTokenValue(RespJ, "metadata", "totalResultDisplayText", "text"), "[^0-9]", "");
                    TotalResultsInSearch = string.IsNullOrEmpty(TotalResultsInSearch) ? _jsonJArrayHandler.GetJTokenValue(RespJ, "metadata", "totalResultCount") : TotalResultsInSearch;
                    TotalResultsInSearch = string.IsNullOrEmpty(TotalResultsInSearch) ? _jsonJArrayHandler.GetJTokenValue(RespJ, "data", "metadata", "totalResultCount") : TotalResultsInSearch;
                    TotalResultsInSearch = string.IsNullOrEmpty(TotalResultsInSearch) ? Utils.GetBetween(response.Response, "\"totalResultCount\":", ",") : TotalResultsInSearch;
                    TotalResultsInSearch = string.IsNullOrEmpty(TotalResultsInSearch) ? Utils.GetBetween(response.Response, "\"decoratedSpotlights\":{\"all\":", ",") : TotalResultsInSearch;
                }
                
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public SearchLinkedinUsersResponseHandler(IResponseParameter response, bool isSalesNavigator)
            : base(response)
        {
            if (!Success)
                return;
           
            try
            {
                if (RespJ == null && response.Response.Contains("<!DOCTYPE html>"))
                {
                    TotalResultsInSearch = isSalesNavigator ? GetSalesViewProfileUserList() : GetViewProfileUserList();
                    return;
                }
                var TupleItems = Utils.GetArrayToken(response.Response,false);
                var arrayElement = TupleItems.Item1;
                if (arrayElement==null?true:arrayElement.Contains("searchResults\":[]"))
                {
                    Success = false;
                    return;
                }
                TotalResultsInSearch = _jsonJArrayHandler.GetJTokenValue(RespJ, "pagination", "pagination");
                TotalResultsInSearch = string.IsNullOrEmpty(TotalResultsInSearch) ? _jsonJArrayHandler.GetJTokenValue(RespJ, "pagination", "total") : TotalResultsInSearch;
                TotalResultsInSearch = string.IsNullOrEmpty(TotalResultsInSearch) ? _jsonJArrayHandler.GetJTokenValue(RespJ, "metadata", "decoratedSpotlights", "all") : TotalResultsInSearch;
                TotalResultsInSearch = string.IsNullOrEmpty(TotalResultsInSearch) ? _jsonJArrayHandler.GetJTokenValue(RespJ, "paging", "total") : TotalResultsInSearch;
                foreach (var item in arrayElement)
                    try
                    {
                        var linkedinUser = new LinkedinUser();
                        var usersList = UsersList;
                        var memberId = _jsonJArrayHandler.GetJTokenValue(item, "memberId");
                        if(string.IsNullOrEmpty(memberId))
                            memberId= _jsonJArrayHandler.GetJTokenValue(item, "objectUrn").Replace("urn:li:member:","");
                        #region Creating New object for LinkedinUser
                        linkedinUser.MemberId = memberId;
                        #endregion

                        var firstName = _jsonJArrayHandler.GetJTokenValue(item, "firstName");

                        if (string.IsNullOrEmpty(firstName) || firstName.Contains("Linkedin") ||
                            firstName.Contains("Linkedin Member")) continue;

                        var lastName = _jsonJArrayHandler.GetJTokenValue(item, "lastName");

                        #region FullName

                        try
                        {
                            linkedinUser.FullName = firstName + " " + lastName;
                            linkedinUser.FullName = Utils.InsertSpecialCharactersInCsv(linkedinUser.FullName);
                        }
                        catch (Exception)
                        {
                            //ignored
                        }

                        #endregion


                        linkedinUser.ProfileId = _jsonJArrayHandler.GetJTokenValue(item, "profileId");
                        if(string.IsNullOrEmpty(linkedinUser.ProfileId))
                            linkedinUser.ProfileId = Utils.GetBetween(_jsonJArrayHandler.GetJTokenValue(item, "entityUrn"), "urn:li:fs_salesProfile:(", ",");
                        #region ConnectionType

                        try
                        {
                            var connectionDegree = _jsonJArrayHandler.GetJTokenValue(item, "degree");
                            switch (connectionDegree)
                            {
                                case "1":
                                    linkedinUser.ConnectionType = ConnectionType.FirstDegree;
                                    break;
                                case "2":
                                    linkedinUser.ConnectionType = ConnectionType.SeondDegree;
                                    break;
                                default:
                                    linkedinUser.ConnectionType = ConnectionType.ThirdPlusDegree;
                                    break;
                            }
                        }
                        catch (Exception)
                        {
                            //ignored
                            linkedinUser.ConnectionType = 0;
                        }

                        #endregion

                        linkedinUser.NumberOfSharedConnections = _jsonJArrayHandler.GetJTokenValue(item, "sharedConnectionsHighlight", "count");
                        if (string.IsNullOrEmpty(linkedinUser.NumberOfSharedConnections))
                             linkedinUser.NumberOfSharedConnections = "N/A";

                        #region ProfileUrl,HasAnonymousProfilePicture,ProfilePicUrl

                        if (!string.IsNullOrEmpty(linkedinUser.MemberId))
                        {
                            linkedinUser.ProfileUrl = "https://www.linkedin.com/sales/people/" + Utils.GetBetween(_jsonJArrayHandler.GetJTokenValue(item, "entityUrn"), "urn:li:fs_salesProfile:(", ")");

                            #region SalesNavigatorProfileUrl

                            try
                            {
                                linkedinUser.SalesNavigatorProfileUrl = linkedinUser.ProfileUrl;
                                linkedinUser.SalesNavigatorProfileUrl =
                                    Utils.InsertSpecialCharactersInCsv(linkedinUser.SalesNavigatorProfileUrl);
                            }
                            catch (Exception)
                            {
                                //ignored
                            }

                            #endregion

                            var rootUrl = _jsonJArrayHandler.GetJTokenValue(item, "profilePictureDisplayImage", "rootUrl");
                            if (!string.IsNullOrEmpty(rootUrl))
                            {
                               var PicUrl = _jsonJArrayHandler.GetJTokenValue(item, "profilePictureDisplayImage", "artifacts", 0, "fileIdentifyingUrlPathSegment");
                                linkedinUser.ProfilePicUrl = $"{rootUrl}{PicUrl}";
                            }
                            else
                            linkedinUser.ProfilePicUrl = "N/A";


                            if (!string.IsNullOrEmpty(linkedinUser.ProfilePicUrl) && linkedinUser.ProfilePicUrl != "N/A"
                            ) linkedinUser.HasAnonymousProfilePicture = true;
                        }

                        #endregion

                        #region AuthToken

                        try
                        {
                            string entityUrn = _jsonJArrayHandler.GetJTokenValue(item, "entityUrn");
                            if (!string.IsNullOrEmpty(entityUrn = _jsonJArrayHandler.GetJTokenValue(item, "entityUrn")))
                                linkedinUser.AuthToken = entityUrn.Split(',').Last().Trim(')');
                        }
                        catch (Exception)
                        {
                            //ignored
                        }

                        #endregion
                        var title = _jsonJArrayHandler.GetJTokenValue(item, "currentPositions", 0, "title");
                        linkedinUser.CurrentCompany = _jsonJArrayHandler.GetJTokenValue(item, "currentPositions", 0, "companyName");
                        linkedinUser.CurrentCompany = string.IsNullOrEmpty(linkedinUser.CurrentCompany)
                            ? "N/A"
                            : Utils.InsertSpecialCharactersInCsv(linkedinUser.CurrentCompany);
                        linkedinUser.HeadlineTitle = $"{title} at {linkedinUser.CurrentCompany}";
                        linkedinUser.HeadlineTitle = string.IsNullOrEmpty(linkedinUser.HeadlineTitle)
                            ? "N/A"
                            : Utils.InsertSpecialCharactersInCsv(linkedinUser.HeadlineTitle);



                        linkedinUser.Location = _jsonJArrayHandler.GetJTokenValue(item, "geoRegion");
                        linkedinUser.Location = string.IsNullOrEmpty(linkedinUser.Location)
                            ? "N/A"
                            : Utils.InsertSpecialCharactersInCsv(linkedinUser.Location);

                        usersList.Add(linkedinUser);
                    }
                    catch (Exception)
                    {
                        //ignored
                    }
                HasMoreResults = TupleItems.Item3;Success = TupleItems.Item2;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        //  SearchLinkedinUsersResponseHandler  for sales navigator, search url
        public SearchLinkedinUsersResponseHandler(IResponseParameter response, bool IsSalesNavigator, string sessionId)
            : base(response)
        {
            Success = !string.IsNullOrEmpty(response?.Response) && (response.Response.Contains("com.linkedin.sales.deco.desktop.searchv2.LeadSearchResult") || response.Response.Contains("<!DOCTYPE html>"));
            if (!Success)
                return;
            JArray arrayElement;
            try
            {
                if (response.Response.Contains("<!DOCTYPE html>"))
                {
                    TotalResultsInSearch = IsSalesNavigator ? GetSalesViewProfileUserList() : GetViewProfileUserList();

                    return;
                }

                arrayElement = _jsonJArrayHandler.GetJArrayElement(_jsonJArrayHandler.GetJTokenValue(RespJ,"elements"));

                #region If Response contains elements

                if (arrayElement.Contains("elements\":[]"))
                {
                    Success = false;
                    return;
                }
                TotalResultsInSearch = _jsonJArrayHandler.GetJTokenValue(RespJ, "paging", "total");
                foreach (var item in arrayElement)
                    try
                    {
                        var entityUrn = _jsonJArrayHandler.GetJTokenValue(item, "entityUrn");

                        var usersList = UsersList;
                        var authToken = entityUrn?.Split(',')?.LastOrDefault()?.Trim(')');
                        var ProfileID = Utils.GetBetween(entityUrn, "(", ",");
                        var objectUrn = _jsonJArrayHandler.GetJTokenValue(item, "objectUrn");
                        var memberId = objectUrn.Split(':').Last();
                        long.TryParse(memberId, out var memberUserId);
                        var publicIdentifier = memberUserId > 0 ? ProfileID : memberId;
                        var linkedinUser = new LinkedinUser(publicIdentifier)
                        {
                            MemberId = memberId,
                            ProfileId = ProfileID,
                            AuthToken = authToken,
                            SessionId = sessionId
                        };
                        #region FullName

                        try
                        {
                            if (!string.IsNullOrEmpty(linkedinUser.FullName =
                                _jsonJArrayHandler.GetJTokenValue(item, "fullName")))
                                linkedinUser.FullName =
                                    Utils.InsertSpecialCharactersInCsv(Utils.RemoveHtmlTags(linkedinUser.FullName));
                        }
                        catch (Exception)
                        {
                            //ignored
                        }

                        #endregion
                        if(string.IsNullOrEmpty(linkedinUser.FullName)||linkedinUser.FullName=="Linked Member")
                        {
                            InValidLinkedInUserCount++;
                            continue;
                        }
                        #region ProfileUrl,HasAnonymousProfilePicture,ProfilePicUrl

                        if (!string.IsNullOrEmpty(linkedinUser.MemberId))
                        {
                            linkedinUser.ProfileUrl = "https://www.linkedin.com/sales/people/" + linkedinUser.ProfileId + "," +
                                                      linkedinUser.AuthToken + ",NAME_SEARCH";

                            #region SalesNavigatorProfileUrl

                            try
                            {
                                linkedinUser.SalesNavigatorProfileUrl = linkedinUser?.ProfileUrl;
                                linkedinUser.SalesNavigatorProfileUrl =
                                    Utils.InsertSpecialCharactersInCsv(linkedinUser.SalesNavigatorProfileUrl);
                            }
                            catch (Exception)
                            {
                            }

                            #endregion

                            linkedinUser.ProfilePicUrl = Utils.AssignNa(GetProfileUrl(item));
                            linkedinUser.HasAnonymousProfilePicture =
                                !string.IsNullOrEmpty(linkedinUser.ProfilePicUrl) ||
                                linkedinUser.ProfilePicUrl != "N/A";

                            #endregion

                            // ConnectionType
                            linkedinUser.ConnectionType = GetConnectionType(item);
                            // NumberOfSharedConnections
                            linkedinUser.NumberOfSharedConnections = Utils.AssignNa(
                                _jsonJArrayHandler.GetJTokenValue(item, "sharedConnectionsHighlight", "count"));

                            //HeadlineTitle
                            linkedinUser.HeadlineTitle = Utils.AssignNa(GetHeadlineTitle(item));

                            #region CurrentCompany

                            try
                            {
                                linkedinUser.CurrentCompany =
                                    _jsonJArrayHandler.GetJTokenValue(item, "currentPositions", 0, "companyName");
                                linkedinUser.CurrentCompany =
                                    Utils.InsertSpecialCharactersInCsv(linkedinUser.CurrentCompany);
                                linkedinUser.CurrentCompany = Utils.RemoveHtmlTags(linkedinUser.CurrentCompany);
                            }
                            catch (Exception)
                            {
                                //ignored
                                linkedinUser.CurrentCompany = "N/A";
                            }

                            #endregion

                            #region Location

                            try
                            {
                                linkedinUser.Location = item["geoRegion"].ToString();
                                linkedinUser.Location = Utils.InsertSpecialCharactersInCsv(linkedinUser.Location);
                            }
                            catch (Exception)
                            {
                                //ignored
                                linkedinUser.Location = "N/A";
                            }

                            #endregion

                            if (!usersList.Contains(linkedinUser))
                                usersList.Add(linkedinUser);
                        }
                    }
                    catch (Exception)
                    {
                        GlobusLogHelper.log.Info(
                            "memberId doesnot exist on getting List of salesnavigator users from your search query");
                    }

                if (UsersList.Count() <= 0)
                    HasMoreResults = true;

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();

                #region If Repose Conatains searchResults

                try
                {
                    arrayElement = JArray.Parse(RespJ["searchResults"].ToString());
                    if (arrayElement.Contains("searchResults\":[]"))
                    {
                        Success = false;
                        return;
                    }

                    TotalResultsInSearch = _jsonJArrayHandler.GetJTokenValue(RespJ, "pagination", "total");


                    foreach (var item in arrayElement)
                        try
                        {
                            var usersList = UsersList;

                            var memberId = item["member"]["memberId"].ToString();

                            #region Creating New object for LinkedinUser

                            var linkedinUser = new LinkedinUser(memberId)
                            {
                                AuthToken = item["member"]["authToken"].ToString(),
                                FullName = _jsonJArrayHandler.GetJTokenValue(RespJ, "member", "formattedName")
                            };

                            #endregion

                            //FullName
                            if (!string.IsNullOrEmpty(linkedinUser.FullName))
                                linkedinUser.FullName = Utils.InsertSpecialCharactersInCsv(linkedinUser.FullName);


                            #region ProfileUrl,HasAnonymousProfilePicture,ProfilePicUrl

                            if (!string.IsNullOrEmpty(linkedinUser.MemberId))
                            {
                                linkedinUser.ProfileUrl =
                                    "https://www.linkedin.com/sales/profile/" + memberId + "," +
                                    linkedinUser.AuthToken + ",NAME_SEARCH?";

                                #region SalesNavigatorProfileUrl

                                try
                                {
                                    linkedinUser.SalesNavigatorProfileUrl = linkedinUser.ProfileUrl;
                                    linkedinUser.SalesNavigatorProfileUrl =
                                        Utils.InsertSpecialCharactersInCsv(linkedinUser.SalesNavigatorProfileUrl);
                                }
                                catch (Exception)
                                {
                                    //ignored
                                }

                                #endregion

                                try
                                {
                                    var rootUrl = item["member"]["vectorImage"]["rootUrl"].ToString();
                                    var artifactsArray =
                                        JArray.Parse(item["member"]["vectorImage"]["artifacts"].ToString());
                                    var fileIdentifyingUrlPathSegment =
                                        artifactsArray.Last()["fileIdentifyingUrlPathSegment"].ToString();
                                    linkedinUser.ProfilePicUrl = rootUrl + fileIdentifyingUrlPathSegment;
                                }
                                catch (Exception)
                                {
                                    //ignored
                                    linkedinUser.ProfilePicUrl = "N/A";
                                }

                                if (!string.IsNullOrEmpty(linkedinUser.ProfilePicUrl) &&
                                    linkedinUser.ProfilePicUrl != "N/A") linkedinUser.HasAnonymousProfilePicture = true;
                            }

                            #endregion

                            #region HeadlineTitle

                            try
                            {
                                linkedinUser.HeadlineTitle = item["member"]["title"].ToString();
                                linkedinUser.HeadlineTitle =
                                    Utils.InsertSpecialCharactersInCsv(linkedinUser.HeadlineTitle);
                            }
                            catch (Exception)
                            {
                                //ignored
                                linkedinUser.HeadlineTitle = "N/A";
                            }

                            #endregion

                            #region CurrentCompany

                            try
                            {
                                linkedinUser.CurrentCompany = item["company"]["companyName"].ToString();
                                linkedinUser.CurrentCompany =
                                    Utils.InsertSpecialCharactersInCsv(linkedinUser.CurrentCompany);
                            }
                            catch (Exception)
                            {
                                //ignored
                                linkedinUser.CurrentCompany = "N/A";
                            }

                            #endregion

                            #region Location

                            try
                            {
                                linkedinUser.Location = item["member"]["location"].ToString();
                                linkedinUser.Location = Utils.InsertSpecialCharactersInCsv(linkedinUser.Location);
                            }
                            catch (Exception)
                            {
                                //ignored
                                linkedinUser.Location = "N/A";
                            }

                            #endregion

                            usersList.Add(linkedinUser);
                        }
                        catch (Exception)
                        {
                            //ignored
                            GlobusLogHelper.log.Info(
                                "memberId doesnot exist on getting List of salesnavigator users from your search query");
                        }

                    if (UsersList.Count() <= 0)
                        HasMoreResults = true;
                }
                catch (Exception exx)
                {
                    exx.DebugLog();
                }

                #endregion
            }
        }

        public List<LinkedinUser> UsersList { get; } = new List<LinkedinUser>();
        public bool HasMoreResults { get; set; }
        public string TotalResultsInSearch { get; }

        private void GetUserListFromBrowserResponse()
        {
            try
            {
                UsersList.Clear();
                var userNodeList = GetUserNodeListFromBrowserResponse(Response.Response);
                foreach (var userNode in userNodeList)
                {
                    var user = userNode.OuterHtml;
                    var userDoc = new HtmlDocument();
                    userDoc.LoadHtml(user);
                    var publicIdentifier = LdDataHelper.GetInstance.GetPublicIdentifierFromPageSource(user);
                    if (string.IsNullOrEmpty(publicIdentifier))
                        publicIdentifier = Utils.GetBetween(user, "<a href=\"/sales/lead/", ",");
                    var linkedInUser = new LinkedinUser(publicIdentifier) {PublicIdentifier = publicIdentifier};
                    var FullName= HtmlAgilityHelper.GetStringTextFromClassName(user, "name actor-name");
                    FullName = string.IsNullOrEmpty(FullName) ? Utils.GetBetween(HtmlAgilityHelper.GetStringTextFromClassName(user, "actor-name-with-distance search-result__title single-line-truncate ember-view"), "<span aria-hidden=\"true\"><!---->", "<!---->"): FullName;
                    FullName = string.IsNullOrEmpty(FullName) ? Utils.GetBetween($">{HtmlAgilityHelper.GetStringInnerTextFromClassName(user, "entity-result__title-line flex-shrink-1 entity-result__title-text--black ")}", ">", "View") : FullName;
                    FullName = string.IsNullOrEmpty(FullName) ? HtmlAgilityHelper.GetStringInnerTextFromClassName(user, "result-lockup__name") : FullName;
                    FullName = string.IsNullOrEmpty(FullName) ? Utils.GetBetween(user, "<span aria-hidden=\"true\"><!---->", "<!----></span>") : FullName;
                    FullName = string.IsNullOrEmpty(FullName) ?HtmlAgilityHelper.GetStringInnerTextFromClassName(user, "artdeco-entity-lockup__title ember-view") : FullName;
                    linkedInUser.FullName = FullName?.Replace(",", "")?.Replace("\r\n", " ")?.Trim();
                    if (user.Contains("artdeco-button artdeco-button--2 artdeco-button--secondary ember-view"))
                    {
                        var node = HtmlAgilityHelper.GetListNodesFromClassName(user, "artdeco-button artdeco-button--2 artdeco-button--secondary ember-view").FirstOrDefault().Id;
                        linkedInUser.NodeId = node;
                    }
                    if (user.Contains("artdeco-button artdeco-button--muted artdeco-button--2 artdeco-button--full artdeco-button--secondary ember-view"))
                    {
                        var node = HtmlAgilityHelper.GetListNodesFromClassName(user, "artdeco-button artdeco-button--muted artdeco-button--2 artdeco-button--full artdeco-button--secondary ember-view").FirstOrDefault().Id;
                        linkedInUser.NodeId = node;
                    }
                    if (linkedInUser.FullName == "Linkedin Member" || string.IsNullOrEmpty(linkedInUser.FullName))
                    {
                        InValidLinkedInUserCount++;
                        continue;
                    }
                    var HeadLine= HtmlAgilityHelper.GetStringInnerTextFromClassName(user,
                        "subline-level-1 t-14 t-black t-normal search-result__truncate", userDoc);
                    HeadLine = string.IsNullOrEmpty(HeadLine) ? HtmlAgilityHelper.GetStringInnerTextFromClassName(user,
                        "entity-result__primary-subtitle t-14 t-black", userDoc) : HeadLine;
                    HeadLine = string.IsNullOrEmpty(HeadLine) ?HtmlAgilityHelper.GetStringInnerTextFromClassName(user, "artdeco-entity-lockup__subtitle ember-view t-14") : HeadLine;
                    HeadLine = string.IsNullOrEmpty(HeadLine) ?Utils.RemoveHtmlTags(HtmlAgilityHelper.GetStringInnerTextFromClassName(user, "t-14 t-black t-normal")) : HeadLine;
                    linkedInUser.HeadlineTitle = HeadLine?.Replace(",", "")?.Replace("\r\n","")?.Replace("\n","")?.Replace("\t","")?.Trim();
                    var Location= HtmlAgilityHelper.GetStringInnerTextFromClassName(user,
                        "subline-level-2 t-12 t-black--light t-normal search-result__truncate", userDoc);
                    Location = string.IsNullOrEmpty(Location) ? HtmlAgilityHelper.GetStringInnerTextFromClassName(user,
                        "entity-result__secondary-subtitle t-14", userDoc) : Location;
                    Location = string.IsNullOrEmpty(Location) ?HtmlAgilityHelper.GetStringInnerTextFromClassName(user, "artdeco-entity-lockup__caption ember-view") : Location;
                    Location = string.IsNullOrEmpty(Location) ? HtmlAgilityHelper.GetListNodesFromClassName(user, "t-14 t-normal", null)?.LastOrDefault()?.InnerText: Location;
                    linkedInUser.Location = Location.Replace(",", "")?.Replace("\r\n", " ")?.Trim();
                    linkedInUser.ProfilePicUrl = HttpUtility.HtmlDecode(Utils.GetBetween(user,"<img src=\"", "\" loading=\""));
                    UsersList.Add(linkedInUser);
                }

                // if we getting this message in page and userlist count '0' it means no more user available
                //and stop navigating to next page
                HasMoreResults = Response.Response.Contains("upgrade to Premium to continue searching.");

                Success = !HasMoreResults || UsersList.Count != 0;
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }
        }
        private List<HtmlNode> GetUserNodeListFromBrowserResponse(string response)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(WebUtility.HtmlDecode(response));
            var userNodeList = new List<HtmlNode>();
            var nodes= HtmlAgilityHelper.GetListNodesFromCustomTag(response, "data-view-name",
                        "search-entity-result-universal-template", htmlDoc);
            userNodeList = nodes.Count == 0 ? (nodes = HtmlAgilityHelper.GetListNodesFromClassName(response,
                        "reusable-search__result-container", htmlDoc)).Count==0?(nodes= HtmlAgilityHelper.GetListNodesFromClassName(response,
                                            "pv5 ph2 search-results__result-item", htmlDoc)).Count==0?
                                            (nodes = HtmlAgilityHelper.GetListNodesFromClassName(response,
                                            "artdeco-list__item pl3 pv3 ", htmlDoc)).Count == 0 ?
                                            (nodes = HtmlAgilityHelper.GetListNodesFromClassName(response,
                                            "NcGernYmxmskGJnBucgNmgRTilRzznfzmlQ", htmlDoc)).Count == 0 ? userNodeList :
                                            nodes :nodes :nodes : nodes:nodes;
            return userNodeList;
        }
        private void GetUserListFromNormalResponse()
        {
            var jsonJArrayHandler = JsonJArrayHandler.GetInstance;
            JArray arrayElement = null;
            var Response =RespJ!=null? RespJ.ToString():"";
            var IsSearchedByKeyword = false;
            JToken userDetails = null;
            var tupleItem = Utils.GetArrayToken(Response);
            Success = tupleItem.Item2;HasMoreResults = tupleItem.Item3;
            IsSearchedByKeyword = (arrayElement = tupleItem.Item1).HasValues;
            if (Response.Contains("\"itemUnion\"") && !IsSearchedByKeyword)
                arrayElement = jsonJArrayHandler.GetJArrayElement(jsonJArrayHandler.GetJArrayElement(jsonJArrayHandler.GetJTokenOfJToken(jsonJArrayHandler.ParseJsonToJObject(jsonJArrayHandler.GetJTokenOfJToken(RespJ, "elements")?.FirstOrDefault(x => x.ToString().Contains("entityResult")).ToString()), "items").ToString()).ToString());
            foreach (var item in arrayElement)
                try
                {
                    var usersList = UsersList;
                    userDetails = jsonJArrayHandler.GetJTokenOfJToken(item, "itemUnion", "entityResult");
                    userDetails=!userDetails.HasValues? jsonJArrayHandler.GetJTokenOfJToken(item, "item", "entityResult"):userDetails;
                    userDetails = !userDetails.HasValues ? item : userDetails;
                    var UserData = jsonJArrayHandler.GetJTokenOfJToken(userDetails, "primaryActions", 0, "actionDetails", "primaryProfileAction", "searchProfileActions", "primaryAction", "connection", "memberRelationship", "memberRelationshipUnion", "noConnection", "invitationUnion", "noInvitation", "targetInviteeResolutionResult");
                    UserData =!UserData.HasValues? jsonJArrayHandler.GetJTokenOfJToken(userDetails, "primaryActions", 0, "actionDetails", "primaryProfileAction", "searchProfileActions", "primaryAction", "connection", "memberRelationship", "memberRelationshipUnion", "noConnection", "invitationUnion", "invitation", "inviteeMemberResolutionResult"):UserData;
                    UserData=!UserData.HasValues? jsonJArrayHandler.GetJTokenOfJToken(userDetails, "primaryActions",0, "actionDetails", "primaryProfileActionV2","primaryAction", "connection", "memberRelationship", "memberRelationshipUnion", "noConnection", "invitationUnion", "noInvitation", "targetInviteeResolutionResult"):UserData;
                    UserData = !UserData.HasValues ?jsonJArrayHandler.GetJTokenOfJToken(userDetails, "primaryActions",0, "actionDetails", "primaryProfileActionV2", "primaryActionResolutionResult", "connection", "memberRelationship", "memberRelationship", "noConnection", "invitation", "noInvitation", "targetInviteeResolutionResult") : UserData;
                    var publicIdentifier = jsonJArrayHandler.GetJTokenValue(userDetails,"publicIdentifier");
                    publicIdentifier = string.IsNullOrEmpty(publicIdentifier) ? jsonJArrayHandler.GetJTokenValue(UserData, "publicIdentifier") : publicIdentifier;
                    publicIdentifier = string.IsNullOrEmpty(publicIdentifier) ? jsonJArrayHandler.GetJTokenValue(userDetails, "navigationUrl")?.ToString() : publicIdentifier;
                    publicIdentifier = !string.IsNullOrEmpty(publicIdentifier) ?publicIdentifier.Contains("miniProfileUrn=") ? GetPublicIdentifier(publicIdentifier): publicIdentifier.Split('/').Last(x => x.ToString() != string.Empty) : publicIdentifier;
                    var fullName = jsonJArrayHandler.GetJTokenValue(userDetails, "title", "text");
                    if (string.IsNullOrEmpty(fullName) || fullName == "LinkedIn Member")
                        continue;
                    fullName = string.IsNullOrEmpty(fullName) ? jsonJArrayHandler.GetJTokenValue(userDetails, "firstName") + " " +jsonJArrayHandler.GetJTokenValue(userDetails, "lastName") : fullName;
                    #region Creating New object for LinkedinUser

                    var linkedinUser = new LinkedinUser(publicIdentifier)
                    {
                        PublicIdentifier = publicIdentifier,
                        FullName = fullName
                    };

                    #endregion


                    //FullName
                    if (linkedinUser.FullName.Contains("LinkedIn Member") || linkedinUser.FullName.Contains("Linkedin Member"))
                    {
                        InValidLinkedInUserCount++;
                        continue;
                    }
                    linkedinUser.FullName = Utils.InsertSpecialCharactersInCsv(linkedinUser.FullName);

                    //ProfileId
                    var profileId = jsonJArrayHandler.GetJTokenValue(UserData, "entityUrn")?.Replace("urn:li:fsd_profile:", "");
                    profileId = string.IsNullOrEmpty(profileId) ? jsonJArrayHandler.GetJTokenValue(userDetails, "targetUrn")?.Replace("urn:li:fs_miniProfile:", "") : profileId;
                    profileId = string.IsNullOrEmpty(profileId) ? Utils.GetBetween(jsonJArrayHandler.GetJTokenValue(userDetails, "entityUrn"), "urn:li:fsd_profile:", ",") : profileId;
                    linkedinUser.ProfileId = profileId;
                    #region ProfileUrl,HasAnonymousProfilePicture,ProfilePicUrl

                    if (!string.IsNullOrEmpty(linkedinUser.PublicIdentifier))
                    {
                        linkedinUser.ProfileUrl ="https://www.linkedin.com/in/" + linkedinUser.PublicIdentifier;
                        var backgroundImage =Utils.AssignNa(jsonJArrayHandler.GetJTokenValue(userDetails, "image", "attributes",0, "miniProfile", "backgroundImage", "com.linkedin.common.VectorImage"));
                        var picture = jsonJArrayHandler.GetJTokenValue(userDetails, "image", "attributes", 0,"miniProfile", "picture","com.linkedin.common.VectorImage", "rootUrl");
                        picture = string.IsNullOrEmpty(picture) ? jsonJArrayHandler.GetJTokenValue(userDetails, "image", "attributes", 0, "detailData", "profilePicture", "profilePicture", "displayImageReference", "vectorImage", "rootUrl") : picture;
                        picture = string.IsNullOrEmpty(picture) ? jsonJArrayHandler.GetJTokenValue(userDetails, "image", "attributes", 0, "detailDataUnion", "nonEntityProfilePicture", "vectorImage", "artifacts", 0, "fileIdentifyingUrlPathSegment") : picture;
                        picture = Utils.AssignNa(string.IsNullOrEmpty(picture) ? jsonJArrayHandler.GetJTokenValue(userDetails, "image", "attributes", 0, "detailData", "nonEntityProfilePicture", "vectorImage", "artifacts", 0, "fileIdentifyingUrlPathSegment") : picture);
                        linkedinUser.ProfilePicUrl = picture;
                        linkedinUser.HasAnonymousProfilePicture = !(string.IsNullOrEmpty(picture) || picture == "N/A");
                    }

                    #endregion

                    #region ConnectionType

                    try
                    {
                        var connectionDegree = jsonJArrayHandler.GetJTokenValue(userDetails, "memberDistance", "value");
                        connectionDegree = string.IsNullOrEmpty(connectionDegree) ? jsonJArrayHandler.GetJTokenValue(userDetails, "primaryActions", 0, "actionDetails", "primaryProfileAction", "searchProfileActions", "primaryAction", "connection", "memberRelationship", "memberRelationshipUnion", "noConnection", "memberDistance") : connectionDegree;
                        connectionDegree = string.IsNullOrEmpty(connectionDegree) ? jsonJArrayHandler.GetJTokenValue(userDetails, "primaryActions", 0, "actionDetails", "primaryProfileActionV2", "primaryAction", "connection", "memberRelationship", "memberRelationshipUnion", "noConnection", "memberDistance") : connectionDegree;
                        connectionDegree = string.IsNullOrEmpty(connectionDegree) ?jsonJArrayHandler.GetJTokenValue(userDetails, "entityCustomTrackingInfo", "memberDistance") : connectionDegree;
                        switch (connectionDegree)
                        {
                            case "DISTANCE_1":
                            case "1":
                                linkedinUser.ConnectionType = ConnectionType.FirstDegree;
                                break;
                            case "DISTANCE_2":
                            case "2":
                                linkedinUser.ConnectionType = ConnectionType.SeondDegree;
                                break;
                            default:
                                linkedinUser.ConnectionType = ConnectionType.ThirdPlusDegree;
                                break;
                        }
                    }
                    catch (Exception)
                    {
                        //ignored
                        linkedinUser.ConnectionType = 0;
                    }

                    #endregion

                    //TrackingId
                    linkedinUser.TrackingId = Utils.AssignNa(jsonJArrayHandler.GetJTokenValue(userDetails, "trackingId"));

                    //HeadlineTitle
                    var HeadLine = jsonJArrayHandler.GetJTokenValue(userDetails, "headline", "text");
                    HeadLine = string.IsNullOrEmpty(HeadLine) ? jsonJArrayHandler.GetJTokenValue(UserData, "headline") : HeadLine;
                    HeadLine = string.IsNullOrEmpty(HeadLine) ?jsonJArrayHandler.GetJTokenValue(userDetails, "primarySubtitle","text") : HeadLine;
                    linkedinUser.HeadlineTitle = Utils.AssignNa(Utils.InsertSpecialCharactersInCsv(HeadLine));
                    //Industry
                    var industry = jsonJArrayHandler.GetJTokenValue(userDetails, "hitInfo","com.linkedin.voyager.search.SearchProfile", "industry");
                    industry = string.IsNullOrEmpty(industry) ? jsonJArrayHandler.GetJTokenValue(userDetails, "occupation") : industry;
                    linkedinUser.Industry = Utils.AssignNa(Utils.InsertSpecialCharactersInCsv(industry));

                    //Location
                    var location = jsonJArrayHandler.GetJTokenValue(userDetails, "subline", "text");
                    location = string.IsNullOrEmpty(location) ? jsonJArrayHandler.GetJTokenValue(userDetails, "secondarySubtitle", "text") : location;
                    linkedinUser.Location = Utils.AssignNa(Utils.InsertSpecialCharactersInCsv(location));
                    if (usersList.All(x => x.ProfileUrl != linkedinUser.ProfileUrl))
                        usersList.Add(linkedinUser);
                }
                catch (Exception)
                {
                    //ignored
                }
        }

        private string GetPublicIdentifier(string publicIdentifier)
        {
            if (string.IsNullOrEmpty(publicIdentifier))
                return publicIdentifier;
            var url = WebUtility.UrlDecode(publicIdentifier);
            if (url.Contains("urn:li:fs_miniProfile"))
            {
                return Utils.GetBetween(url+"##", "urn:li:fs_miniProfile:", "##");
            }
            return Utils.GetBetween(url, "https://www.linkedin.com/in/", "?miniProfileUrn=");
        }

        public string GetProfileUrl(JToken item)
        {
            var profilePicUrl = string.Empty;
            try
            {
                var rootUrl =
                    _jsonJArrayHandler.GetJTokenValue(item, "profilePictureDisplayImage",
                        "rootUrl");
                var artifactsArray =
                    JArray.Parse(_jsonJArrayHandler.GetJTokenValue(item, "profilePictureDisplayImage", "artifacts"));
                var fileIdentifyingUrlPathSegment = artifactsArray.Last()["fileIdentifyingUrlPathSegment"].ToString();
                profilePicUrl = rootUrl + fileIdentifyingUrlPathSegment;
            }
            catch (Exception)
            {
                //ignored
            }

            return profilePicUrl;
        }

        public ConnectionType GetConnectionType(JToken item)
        {
            ConnectionType ConnectionType = 0;
            try
            {
                var connectionDegree = item["degree"].ToString();
                if (connectionDegree == "1" || connectionDegree == "-1")
                    ConnectionType = ConnectionType.FirstDegree;
                else if (connectionDegree == "2" || connectionDegree == "-2")
                    ConnectionType = ConnectionType.SeondDegree;
                else
                    ConnectionType = ConnectionType.ThirdPlusDegree;
            }
            catch (Exception)
            {
                //ignored
            }

            return ConnectionType;
        }

        public string GetHeadlineTitle(JToken item)
        {
            var headlineTitle = string.Empty;
            try
            {
                headlineTitle = item["currentPositions"][0]["title"].ToString();
                headlineTitle = Utils.InsertSpecialCharactersInCsv(headlineTitle);
            }
            catch (Exception)
            {
                //ignored
            }

            return headlineTitle;
        }
        private string GetViewProfileUserList()
        {
            var totalResult = Utils.GetBetween(Response.Response, "\"totalResultCount\":", ",")?.Replace(",", "")
                ?.Replace("\"", "")?.Trim();


            try
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(Response.Response);
                UsersList.Clear();
                var userNodeList =
                    HtmlAgilityHelper.GetListNodesFromClassName(Response.Response,
                        "search-result search-result__occluded-item ember-view", htmlDoc);
                if(userNodeList.Count<=0)
                    userNodeList =
                    HtmlAgilityHelper.GetListNodesFromClassName(Response.Response,
                        "reusable-search__result-container", htmlDoc);
                foreach (var user in userNodeList)
                {
                    var userDoc = new HtmlDocument();
                    userDoc.LoadHtml(user.OuterHtml);
                    var publicIdentifier = LdDataHelper.GetInstance.GetPublicIdentifierFromPageSource(user.OuterHtml);
                    var linkedInUser = new LinkedinUser(publicIdentifier);
                    linkedInUser.FullName =
                        HtmlAgilityHelper.GetStringInnerTextFromClassName("", "name actor-name",
                            userDoc);
                    if (string.IsNullOrEmpty(linkedInUser.FullName))
                        linkedInUser.FullName = Utils.GetBetween(user.OuterHtml, "<span aria-hidden=\"true\"><!---->", "<!---->");
                    if (string.IsNullOrEmpty(linkedInUser.FullName) || linkedInUser.FullName == "Linkedin Member")
                        continue;
                    linkedInUser.HeadlineTitle = WebUtility
                        .HtmlDecode(HtmlAgilityHelper.GetStringInnerTextFromClassName("",
                            "subline-level-1 t-14 t-black t-normal search-result__truncate", userDoc)?.Trim())
                        ?.Split('\n')[0];
                    if (string.IsNullOrEmpty(linkedInUser.HeadlineTitle))
                        linkedInUser.HeadlineTitle = HtmlAgilityHelper.GetStringInnerTextFromClassName(user.OuterHtml,
                        "entity-result__primary-subtitle t-14 t-black");
                    linkedInUser.Location = HtmlAgilityHelper.GetStringInnerTextFromClassName("",
                        "subline-level-2 t-12 t-black--light t-normal search-result__truncate", userDoc);
                    if (string.IsNullOrEmpty(linkedInUser.Location))
                        linkedInUser.Location = HtmlAgilityHelper.GetStringInnerTextFromClassName(user.OuterHtml,
                        "entity-result__secondary-subtitle t-14");
                    linkedInUser.ProfileId = Utils.GetBetween(publicIdentifier, "people/", ",");
                    if (string.IsNullOrEmpty(linkedInUser.ProfileId))
                        linkedInUser.ProfileId =Utils.GetBetween(HtmlAgilityHelper.GetStringInnerHtmlFromClassName(user.OuterHtml,
                            "entity-result__simple-insight-text"), "https://www.linkedin.com/in/", "\"><strong>");

                      linkedInUser.ProfilePicUrl = LdDataHelper.GetInstance.GetSource(user.OuterHtml);
                    if (linkedInUser.ProfilePicUrl.Contains("data:image/gif"))
                        linkedInUser.ProfilePicUrl = "";
                    UsersList.Add(linkedInUser);
                }

                if (UsersList.Count() <= 0)
                    HasMoreResults = true;
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }

            return totalResult;
        }


        private string GetSalesViewProfileUserList()
        {
            var totalResult = "";

            switch (Utils.GetBetween(Response.Response, "selectedType\":\"", "\"")?.Trim())
            {
                case "RECENT_POSITION_CHANGE":
                    totalResult = Utils.GetBetween(Response.Response, "recentPositionChange\":", ",")?.Replace(",", "")
                        ?.Trim();
                    break;
                case "RECENTLY_POSTED_ON_LINKEDIN":
                    totalResult = Utils.GetBetween(Response.Response, "recentPostedOnLinkedIn\":", ",")
                        ?.Replace(",", "")?.Trim();
                    break;
                default:
                    totalResult = Utils.GetBetween(Response.Response, "decoratedSpotlights\":{\"all\":", ",")
                        ?.Replace(",", "")?.Trim();
                    if (string.IsNullOrEmpty(totalResult))
                        totalResult = Utils.GetBetween(Response.Response,
                            "selected\"><span class=\"artdeco-tab-primary-text\">", "</span>");
                    if (string.IsNullOrEmpty(totalResult))
                        totalResult = Utils.GetBetween(Response.Response, "totalDisplayCount\":\"", "\",\"");
                    if (string.IsNullOrEmpty(totalResult))
                    {
                        var InnerText =
                            HtmlAgilityHelper.MethodGetInnerStringFromId(Response.Response, "search-spotlight-tab-ALL");
                        totalResult = Utils.GetBetween(InnerText, ">", "<");
                    }
                    if(string.IsNullOrEmpty(totalResult))
                        totalResult=
                            HtmlAgilityHelper.GetStringInnerTextFromClassName(Response.Response, "ml3 pl3 _display-count-spacing");
                    if (string.IsNullOrEmpty(totalResult))
                        totalResult = HtmlAgilityHelper.GetStringInnerTextFromClassName(Response.Response, "t-14 flex align-items-center mlA pl3");
                    totalResult = Regex.Match(totalResult, "(.*)results")?.Value?.Replace("results","")?.Trim();
                    if (!string.IsNullOrEmpty(totalResult) &&!Regex.IsMatch(totalResult, "^[0-9]*$"))
                        totalResult = SetTotalResults(totalResult);
                    break;
            }

            try
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(Response.Response);
                UsersList.Clear();
                var userNodeList =
                    HtmlAgilityHelper.GetListNodesFromClassName(Response.Response,
                        "pv5 ph2 search-results__result-item", htmlDoc);
                if(userNodeList.Count<=0)
                    userNodeList =HtmlAgilityHelper.GetListNodesFromClassName(Response.Response,
                        "artdeco-list__item pl3 pv3", htmlDoc);
                foreach (var user in userNodeList)
                {
                    var userDoc = new HtmlDocument();
                    userDoc.LoadHtml(user.OuterHtml);
                    var salesUrl =
                        $"https://www.linkedin.com/sales/{LdDataHelper.GetInstance.GetSalesUrlFromPageSource(user.OuterHtml)}";
                    var linkedInUser = new LinkedinUser(salesUrl) {PublicIdentifier = ""};
                    linkedInUser.SalesNavigatorProfileUrl = salesUrl;
                    linkedInUser.ProfileUrl = salesUrl;
                    if (string.IsNullOrEmpty(linkedInUser.FullName))
                        linkedInUser.FullName = Utils.GetBetween(user.OuterHtml, "person-name\">", "</a>")?.Trim().Replace("</span>", "")?.Replace("\n","")?.Trim();

                    if (string.IsNullOrEmpty(linkedInUser.FullName) || linkedInUser.FullName == "Linkedin Member")
                    {
                        InValidLinkedInUserCount++;
                        continue;
                    }
                    if (string.IsNullOrEmpty(linkedInUser.HeadlineTitle))
                        linkedInUser.HeadlineTitle = WebUtility
                        .HtmlDecode(HtmlAgilityHelper
                            .GetStringInnerTextFromClassName("", "artdeco-entity-lockup__subtitle ember-view t-14", userDoc)?.Trim())
                        ?.Split('\n')[0];
                    var profileId = string.Empty;
                    if (string.IsNullOrEmpty(linkedInUser.Location))
                        linkedInUser.Location =Utils.GetBetween(HtmlAgilityHelper.GetStringInnerHtmlFromClassName("", "artdeco-entity-lockup__caption ember-view", userDoc), "\">", "</span>")?.Replace(",","");
                    linkedInUser.ProfileId =string.IsNullOrEmpty(profileId = Utils.GetBetween(salesUrl, "people/", ","))?Utils.GetBetween(salesUrl,"lead/", ",NAME_SEARCH") :profileId;
                    linkedInUser.ProfilePicUrl = LdDataHelper.GetInstance.GetSource(user.OuterHtml);
                    //linkedInUser.NumberOfSharedConnections = Regex.Replace(HtmlAgilityHelper.GetStringInnerTextFromClassName(user.InnerHtml, "artdeco-button artdeco-button--tertiary artdeco-button--0 artdeco-button--muted ml1"),"[^0-9]","");
                    var Nodes = HtmlAgilityHelper.GetListInnerHtmlOrInnerTextOrOuterHtmlFromIdOrClassName(user.InnerHtml,string.Empty,true, "a11y-text",false);
                    int.TryParse(Regex.Replace(Nodes.Count>0?Nodes.FirstOrDefault(x=>x.Contains("degree connection"))??string.Empty:string.Empty,"[^0-9]",""),out int connectionType);
                    linkedInUser.ConnectionType = connectionType == 1 ? ConnectionType.FirstDegree : connectionType == 2 ? ConnectionType.SeondDegree : ConnectionType.ThirdPlusDegree;
                    if (linkedInUser.ProfilePicUrl.Contains("data:image/gif"))
                        linkedInUser.ProfilePicUrl = "";
                    linkedInUser.PublicIdentifier = linkedInUser.ProfileId;//for sales user profileId and public identifier is remain same.
                    UsersList.Add(linkedInUser);
                }

                if (UsersList.Count <= 0)
                    HasMoreResults = true;
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }

            return totalResult;
        }
        /// <summary>
        /// manually adding number if its in 'K+' and 'M+'
        /// </summary>
        /// <param name="totalResult"></param>
        /// <returns></returns>
        private string SetTotalResults(string totalResult)
        {
            try
            {
                if (totalResult.Contains("K"))
                    return totalResult = Regex.Replace(totalResult, @"[^\d]", "") + "000";
                if (totalResult.Contains("M"))
                    return totalResult = Regex.Replace(totalResult, @"[^\d]", "") + "000000";
            }
            catch (Exception)
            {
            }
            return totalResult;
        }
    }
}