using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDResponse.BaseResponse;
using FaceDominatorCore.Interface;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FaceDominatorCore.FDResponse.FriendsResponse
{
    public class IncomingFriendListResponseHandler : FdResponseHandler, IResponseHandler
    {
        public bool HasMoreResults { get; set; }

        public string EntityId { get; set; }

        public string PageletData { get; set; }

        public bool Status { get; set; }

        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; }
            = new FdScraperResponseParameters();

        public IncomingFriendListResponseHandler(IResponseParameter responseParameter)
                : base(responseParameter)
        {

            if (responseParameter.HasError)
                return;

            HtmlDocument objHtmlDocument = new HtmlDocument();

            List<string> friendSuggetionList = new List<string>();

            var decodedResponse = FdFunctions.GetDecodedResponse(responseParameter.Response);


            try
            {

                ObjFdScraperResponseParameters.ListUser = new List<FacebookUser>();

                objHtmlDocument.LoadHtml(decodedResponse);
                //clearfix ruUserBox _3-z friendRequestItem
                HtmlNodeCollection objHtmlNodeCollection = objHtmlDocument.DocumentNode.SelectNodes("(//div[@class=\"clearfix ruUserBox _3-z friendRequestItem\"])");

                if (objHtmlNodeCollection != null)
                {
                    //objHtmlNodeCollection.ForEach(objNode=>
                    //    friendSuggetionList.Add(objNode.OuterHtml)
                    //    );

                    foreach (var objNode in objHtmlNodeCollection)
                    {
                        friendSuggetionList.Add(objNode.OuterHtml);
                    }

                    GetFriends(friendSuggetionList);


                    GetPaginationData(decodedResponse);

                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

        }

        private void GetPaginationData(string decodedResponse)
        {
            try
            {
                HtmlDocument objHtmlDocument = new HtmlDocument();

                objHtmlDocument.LoadHtml(decodedResponse);

                if (objHtmlDocument.DocumentNode.SelectNodes("(//div[@id=\"FriendRequestMorePager\"])") != null)
                {
                    var pageletSection = objHtmlDocument.DocumentNode.SelectNodes("(//div[@id=\"FriendRequestMorePager\"])")[0].InnerHtml;

                    if (pageletSection != null)
                    {
                        objHtmlDocument.LoadHtml(pageletSection);

                        var paginationData = objHtmlDocument.DocumentNode.SelectNodes("//a");

                        if (paginationData != null && paginationData.Count > 0)
                        {
                            pageletSection = objHtmlDocument.DocumentNode.SelectNodes("//a")[0].OuterHtml;

                            pageletSection = Regex.Matches(pageletSection, "ajaxify=\"(.*?)\"", RegexOptions.Singleline)[0].Groups[1].ToString();

                            pageletSection = pageletSection.Split('?')[1];

                            ObjFdScraperResponseParameters.SeenTimeStamp = Regex.Matches(pageletSection, "seenTimestamp=(.*?)&", RegexOptions.Singleline)[0].Groups[1].ToString();

                            ObjFdScraperResponseParameters.EndingId = Regex.Matches(pageletSection, "endingID=(.*?)&", RegexOptions.Singleline)[0].Groups[1].ToString();
                        }

                    }
                }

                HasMoreResults = !string.IsNullOrEmpty(ObjFdScraperResponseParameters.EndingId) &&
                !string.IsNullOrEmpty(ObjFdScraperResponseParameters.SeenTimeStamp);

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

        }

        private void GetFriends(List<string> friendSuggetionList)
        {
            HtmlDocument objHtmlDocument = new HtmlDocument();


            try
            {
                foreach (string friendItem in friendSuggetionList)
                {
                    try
                    {
                        FacebookUser objFacebookUser = new FacebookUser();

                        var friendId = Regex.Matches(friendItem, "data-id=\"(.*?)\"")[0].Groups[1].ToString();

                        objFacebookUser.UserId = friendId;

                        objHtmlDocument.LoadHtml(friendItem);

                        var collection = objHtmlDocument.DocumentNode.SelectNodes("//a");

                        if (collection != null)
                        {
                            if (collection.Count >= 1)
                                objFacebookUser.Familyname = collection[1].InnerHtml;
                        }

                        if (ObjFdScraperResponseParameters.ListUser.FirstOrDefault(x => x.UserId == objFacebookUser.UserId) == null)
                            ObjFdScraperResponseParameters.ListUser.Add(objFacebookUser);

                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    Status = ObjFdScraperResponseParameters.ListUser.Count > 0;

                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}
