/*
using FaceDominatorCore.FDResponse.BaseResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using DominatorHouseCore.Interfaces;
using HtmlAgilityPack;
using FaceDominatorCore.FDLibrary.FdFunctions;
using System.Text.RegularExpressions;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using DominatorHouseCore;

namespace FaceDominatorCore.FDResponse.FriendsResponse
{
    public class SentRequestFriendListResponseHandler : FdResponseHandler
    {
        public List<FacebookUser> ListSentFriendId { get; set; }

        public string Page { get; set; }

        public string PageSize { get; set; }

        public string PagerId { get; set; }

        public bool HasMoreResults { get; set; }

        public SentRequestFriendListResponseHandler(IResponseParameter responseParameter)
            : base(responseParameter)
        {
            HtmlDocument objHtmlDocument = new HtmlDocument();

            List<string> friendSuggetionList = new List<string>();

            var decodedResponse = FdFunctions.GetDecodedResponse(responseParameter.Response);


            try
            {

                ListSentFriendId = new List<FacebookUser>();


                objHtmlDocument.LoadHtml(decodedResponse);

                HtmlNodeCollection objHtmlNodeCollection = objHtmlDocument.DocumentNode.SelectNodes("(//div[@class=\"clearfix ruUserBox _3-z\"])");

                if (objHtmlNodeCollection != null)
                {
                    foreach (var objNode in objHtmlNodeCollection)
                    {
                        friendSuggetionList.Add(objNode.OuterHtml);
                    }

                    GetFriends(friendSuggetionList);

                    //objHtmlNodeCollection = objHtmlDocument.DocumentNode.SelectNodes("(//div[@class=\"clearfix mtm uiMorePager stat_elem _646 _52jv\"])");

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

                var pageletSection = objHtmlDocument.DocumentNode.SelectNodes("(//div[@class=\"clearfix mtm uiMorePager stat_elem _646 _52jv\"])")[0].InnerHtml;

                if (pageletSection != null)
                {
                    objHtmlDocument.LoadHtml(pageletSection);

                    pageletSection = objHtmlDocument.DocumentNode.SelectNodes("//a")[0].OuterHtml;



                    pageletSection = Regex.Matches(pageletSection, "ajaxify=\"(.*?)\"", RegexOptions.Singleline)[0].Groups[1].ToString();

                    pageletSection = pageletSection.Split('?')[1];

                    Page = Regex.Matches(pageletSection, "page=(.*?)&", RegexOptions.Singleline)[0].Groups[1].ToString();

                    PageSize = Regex.Matches(pageletSection, "page_size=(.*?)&", RegexOptions.Singleline)[0].Groups[1].ToString();

                    PagerId = Regex.Matches($"{pageletSection}\"", "pager_id=(.*?)\"", RegexOptions.Singleline)[0].Groups[1].ToString();


                }

                if(!string.IsNullOrEmpty(PagerId))
                {
                    HasMoreResults = true;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

        }

        private void GetFriends(List<string> friendSuggetionList)
        {
            string friendId;

            HtmlDocument objHtmlDocument = new HtmlDocument();

            try
            {
                foreach (string friendItem in friendSuggetionList)
                {
                    try
                    {
                        objHtmlDocument.LoadHtml(friendItem);

                        //friendId = objHtmlDocument.DocumentNode.SelectNodes("(//button[@class=\"clearfix mtm uiMorePager stat_elem _646 _52jv\"])")[0].OuterHtml;

                        friendId = FdRegexUtility.FirstMatchExtractor(friendItem, FdConstants.DataProfileIdRegx);

                        FacebookUser objFacebookUser = new FacebookUser {UserId = friendId};


                        if (ListSentFriendId.FirstOrDefault(x=> x.UserId==friendId)== null)
                            ListSentFriendId.Add(objFacebookUser);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }

                if (ListSentFriendId.Count > 0)
                    Success = true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }




    }
}
*/
