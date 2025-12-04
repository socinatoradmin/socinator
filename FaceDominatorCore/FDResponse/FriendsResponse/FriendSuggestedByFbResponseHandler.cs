/*
using FaceDominatorCore.FDResponse.BaseResponse;
using System;
using System.Collections.Generic;
using DominatorHouseCore.Interfaces;
using HtmlAgilityPack;
using FaceDominatorCore.FDLibrary.FdFunctions;
using System.Text.RegularExpressions;
using DominatorHouseCore;

namespace FaceDominatorCore.FDResponse.FriendsResponse
{
    class FriendSuggestedByFbResponseHandler : FdResponseHandler
    {
        public List<string> ListSuggestedFriendId { get; set; }

        public string SeenTimeStamp { get; set; }

        public string EndingId { get; set; }

        public FriendSuggestedByFbResponseHandler(IResponseParameter responseParameter, List<string> listSuggestedFriendId) 
            : base(responseParameter)
        {
            HtmlDocument objHtmlDocument = new HtmlDocument();

            List<string> friendSuggetionList = new List<string>();

            var decodedResponse = FdFunctions.GetDecodedResponse(responseParameter.Response);


            try
            {
                ListSuggestedFriendId = listSuggestedFriendId ?? new List<string>();

                objHtmlDocument.LoadHtml(decodedResponse);

                HtmlNodeCollection objHtmlNodeCollection = objHtmlDocument.DocumentNode.SelectNodes("(//div[@class=\"clearfix ruUserBox _3-z\"])");

                if (objHtmlNodeCollection != null)
                {
                    foreach (var objNode in objHtmlNodeCollection)
                    {
                        friendSuggetionList.Add(objNode.InnerHtml);
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

                var pageletSection = objHtmlDocument.DocumentNode.SelectNodes("(//div[@id=\"FriendSuggestionMorePager\"])")[0].InnerHtml;

                if (pageletSection != null)
                {
                    objHtmlDocument.LoadHtml(pageletSection);

                    pageletSection = objHtmlDocument.DocumentNode.SelectNodes("//a")[0].OuterHtml;

                    pageletSection = Regex.Matches(pageletSection, "ajaxify=\"(.*?)\"", RegexOptions.Singleline)[0].Groups[1].ToString();

                    pageletSection = pageletSection.Split('?')[1];

                    SeenTimeStamp = Regex.Matches(pageletSection, "seenTimestamp=(.*?)&", RegexOptions.Singleline)[0].Groups[1].ToString();

                    EndingId = Regex.Matches(pageletSection, "endingID=(.*?)&", RegexOptions.Singleline)[0].Groups[1].ToString();
                }
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
                   
                    objHtmlDocument.LoadHtml(friendItem);
                    var friendId = objHtmlDocument.DocumentNode.SelectNodes("(//button[@class=\"_42ft _4jy0 _4jy3 _4jy1 selected _51sy\"])")[0].OuterHtml;
                    friendId = Regex.Matches(friendId, "'uid' : \"(.*?)\"")[0].Groups[1].ToString();

                    ListSuggestedFriendId.Add(friendId);

                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}
*/
