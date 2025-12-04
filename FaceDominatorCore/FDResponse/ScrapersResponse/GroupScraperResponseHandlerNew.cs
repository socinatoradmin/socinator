using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDResponse.BaseResponse;
using FaceDominatorCore.Interface;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FaceDominatorCore.FDResponse.ScrapersResponse
{
    public class GroupScraperResponseHandlerNew : FdResponseHandler, IResponseHandler
    {

        public bool HasMoreResults { get; set; }

        public string EntityId { get; set; }

        // public string PageletData { get; set; }

        public bool Status { get; set; }
        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; }
        = new FdScraperResponseParameters();

        //public List<GroupDetails> ListGroupDetails { get; set; } = new List<GroupDetails>();

        public string PageletData { get; set; }

        /*
                public string FinalEncodedQuery { get; set; }
        */
        private string[] PaginationArray { get; } = new string[60];

        private int Count { get; set; }

        public GroupScraperResponseHandlerNew(IResponseParameter responseParameter, List<GroupDetails> listGroupDetails
                            , string pagination)
            : base(responseParameter)
        {
            if (responseParameter.HasError)
                return;
            try
            {
                ObjFdScraperResponseParameters.ListGroup = new List<GroupDetails>();
                bool isPagination = false;

                if (listGroupDetails == null)
                {
                    ObjFdScraperResponseParameters.ListGroup = new List<GroupDetails>();

                    PaginationArray = new string[60];
                }
                else
                {
                    ObjFdScraperResponseParameters.ListGroup = listGroupDetails;

                    PageletData = pagination;

                    isPagination = true;
                }

                string decodedResponse = FdFunctions.GetNewPrtialDecodedResponse(responseParameter.Response);

                HtmlDocument objHtmlDocument = new HtmlDocument();

                objHtmlDocument.LoadHtml(decodedResponse);

                HtmlNodeCollection objNodeCollection = objHtmlDocument.DocumentNode.SelectNodes("//div[starts-with(@id, 'GroupDiscoverCard_admin')]");

                if (objNodeCollection != null)
                {
                    List<string> fanpageResponseList = new List<string>();
                    //GroupDiscoverCard_629450117259244

                    objHtmlDocument.LoadHtml(objNodeCollection[0].InnerHtml);

                    objNodeCollection = objHtmlDocument.DocumentNode.SelectNodes("//li[starts-with(@id, 'GroupDiscoverCard_')]");

                    if (objNodeCollection != null)
                    {
                        objNodeCollection.ForEach(objNode =>
                                fanpageResponseList.Add(objNode.InnerHtml));

                        GetGroupDetails(fanpageResponseList, false, false);
                    }
                }

                objHtmlDocument.LoadHtml(decodedResponse);

                objNodeCollection = objHtmlDocument.DocumentNode.SelectNodes("//div[starts-with(@id, 'GroupDiscoverCard_membership')]");

                if (objNodeCollection != null)
                {
                    var document = objNodeCollection[0].InnerHtml;

                    objHtmlDocument.LoadHtml(document);

                    var objNodeCollectionLeft = objHtmlDocument.DocumentNode.SelectNodes("//ul[starts-with(@id, 'group-discover-card-left-columnmembership')]");

                    var listLeftColumn = ScrapGroups(objNodeCollectionLeft, decodedResponse);

                    GetGroupDetails(listLeftColumn, true, true);

                    var objNodeCollectionRight = objHtmlDocument.DocumentNode.SelectNodes("//ul[starts-with(@id, 'group-discover-card-right-columnmembership')]");

                    var listRightColumn = ScrapGroups(objNodeCollectionRight, decodedResponse);

                    GetGroupDetails(listRightColumn, true, false);
                }
                else if (isPagination)
                {
                    PageletData += ",";

                    var objNodeCollectionLeft = Regex.Split(decodedResponse, "group-discover-card-right-columnmembership");

                    if (objNodeCollectionLeft.Length > 0)
                    {
                        var listLeftColumn = ScrapGroupsPagelet(objNodeCollectionLeft[0], decodedResponse);

                        GetGroupDetails(listLeftColumn, true, true);
                    }

                    if (objNodeCollectionLeft.Length > 1)
                    {

                        var listRightColumn = ScrapGroupsPagelet(objNodeCollectionLeft[1], decodedResponse);

                        GetGroupDetails(listRightColumn, true, false);
                    }
                }

                PaginationArray.ForEach(x =>
                {
                    if (!string.IsNullOrEmpty(x))
                        PageletData += x + ",";
                });

                PageletData = string.IsNullOrEmpty(PageletData)
                    ? PageletData
                    : PageletData.Remove(PageletData.Length - 1);

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public GroupScraperResponseHandlerNew(IResponseParameter responseParameter, List<GroupDetails> listGroupDetails
                            , string pagination, bool isNewFormat = true, string groupType = "")
            : base(responseParameter)
        {
            if (responseParameter.HasError)
                return;
            try
            {
                ObjFdScraperResponseParameters.ListGroup = new List<GroupDetails>();

                if (listGroupDetails != null)
                    ObjFdScraperResponseParameters.ListGroup.AddRange(listGroupDetails);

                var jsonResponse = JObject.Parse(responseParameter.Response);

                var totalConversions = string.IsNullOrEmpty(groupType)
                    ? jsonResponse["data"]["viewer"]["groups_tab"]["badged_group_list"]["edges"]
                    : jsonResponse["data"]["viewer"]["account_user"]["groups"]["edges"];

                foreach (var Conversion in totalConversions)
                {
                    GroupDetails objGroupDetails = new GroupDetails();

                    objGroupDetails.GroupType = groupType;

                    var groupId = Conversion["node"]["id"].ToString();

                    var groupName = Conversion["node"]["name"].ToString();

                    objGroupDetails.GroupId = groupId;

                    objGroupDetails.GroupName = groupName;

                    var groupUrl = $"{FdConstants.FbHomeUrl}{groupId}";

                    objGroupDetails.GroupUrl = groupUrl;

                    objGroupDetails.GroupJoinStatus = "Member";


                    //if (ListGroupDetails.FirstOrDefault(x => x.GroupId == groupId) == null)
                    //    ListGroupDetails.Add(objGroupDetails);

                    if (ObjFdScraperResponseParameters.ListGroup.FirstOrDefault(x => x.GroupId == groupId) == null)
                        ObjFdScraperResponseParameters.ListGroup.Add(objGroupDetails);

                }

                if (groupType == "NonAdmin")
                {
                    var pageInfo = jsonResponse["data"]["viewer"]["account_user"]["groups"]["page_info"];

                    PageletData = pageInfo["end_cursor"].ToString();

                    HasMoreResults = bool.Parse(pageInfo["has_next_page"].ToString());
                }


            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public List<string> ScrapGroups(HtmlNodeCollection nodeCollection, string decodedResponse)
        {
            List<string> fanpageResponseList = new List<string>();

            HtmlDocument objHtmlDocument = new HtmlDocument();

            if (nodeCollection != null)
            {
                objHtmlDocument.LoadHtml(nodeCollection[0].InnerHtml);

                nodeCollection = objHtmlDocument.DocumentNode.SelectNodes("//li[starts-with(@id, 'GroupDiscoverCard_')]");

                if (nodeCollection != null)
                    nodeCollection.ForEach(objNode =>
                    fanpageResponseList.Add(objNode.InnerHtml));

            }
            return fanpageResponseList;
        }

        public List<string> ScrapGroupsPagelet(string response, string decodedResponse)
        {
            List<string> fanpageResponseList = new List<string>();

            HtmlDocument objHtmlDocument = new HtmlDocument();


            objHtmlDocument.LoadHtml(response);

            var nodeCollection = objHtmlDocument.DocumentNode.SelectNodes("//li[starts-with(@id, 'GroupDiscoverCard_')]");

            if (nodeCollection != null)
            {
                nodeCollection.ForEach(objNode =>
                    fanpageResponseList.Add(objNode.InnerHtml));
            }

            return fanpageResponseList;
        }

        private void GetGroupDetails(List<string> fanpageResponseList, bool isAddPagelet, bool isAddAtOdd)
        {
            string groupId;

            Count = !isAddAtOdd ? 1 : 0;


            HtmlDocument objHtmlDocument = new HtmlDocument();

            foreach (string response in fanpageResponseList)
            {
                try
                {
                    GroupDetails objGroupDetails = new GroupDetails();

                    objHtmlDocument.LoadHtml(response);

                    var nameDetails = objHtmlDocument.DocumentNode.SelectNodes("//a");

                    if (nameDetails == null)
                        continue;

                    objGroupDetails.GroupName = nameDetails[0].InnerText;

                    groupId = FdRegexUtility.FirstMatchExtractor(nameDetails[0].OuterHtml, "id=(.*?)&");

                    objGroupDetails.GroupId = groupId;

                    var groupUrl = $"{FdConstants.FbHomeUrl}{groupId}";

                    objGroupDetails.GroupUrl = groupUrl;

                    objGroupDetails.GroupJoinStatus = "Member";



                    try
                    {
                        if (objGroupDetails.GroupJoinStatus.Contains("Member"))
                        {
                            var date = DateTime.Now;
                            TimeSpan time = new TimeSpan(8760, 0, 0);
                            objGroupDetails.JoinDate = date.Subtract(time);
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    if (ObjFdScraperResponseParameters.ListGroup.FirstOrDefault(x => x.GroupId == groupId) == null)
                    {
                        ObjFdScraperResponseParameters.ListGroup.Add(objGroupDetails);

                        if (isAddPagelet)
                        {
                            PaginationArray[Count] = objGroupDetails.GroupId;

                            Count += 2;
                        }

                    }

                    else
                        continue;


                }
                catch (ArgumentException)
                {

                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                Status = ObjFdScraperResponseParameters.ListGroup.Count > 0;
            }
        }
    }
}
