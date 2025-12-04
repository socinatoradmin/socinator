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
using System.Net;

namespace FaceDominatorCore.FDResponse.FriendsResponse
{

    public class MutualFriendsResponseHandler : FdResponseHandler, IResponseHandler
    {
        public bool HasMoreResults { get; set; }
        public string EntityId { get; set; }
        public string PageletData { get; set; }
        public bool Status { get; set; }
        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; }
            = new FdScraperResponseParameters();

        public MutualFriendsResponseHandler(IResponseParameter responseParameter)
            : base(responseParameter)
        {
            if (responseParameter.HasError || responseParameter.Response == null)
                return;

            HtmlDocument objHtmlDocument = new HtmlDocument();

            ObjFdScraperResponseParameters.ListUser = new List<FacebookUser>();

            List<string> friendSuggetionList = new List<string>();

            var decodedResponse = FdFunctions.GetDecodedResponse(responseParameter.Response);


            try
            {

                objHtmlDocument.LoadHtml(decodedResponse);


                HtmlNodeCollection objHtmlNodeCollection = objHtmlDocument.DocumentNode.SelectNodes("//div[starts-with(@class, 'fbProfileBrowserList')]");

                if (objHtmlNodeCollection != null)
                {
                    var mutualFriendDetails = objHtmlNodeCollection[0].InnerHtml;

                    objHtmlDocument.LoadHtml(mutualFriendDetails);

                    objHtmlNodeCollection = objHtmlDocument.DocumentNode.SelectNodes("//li[starts-with(@class, 'fbProfileBrowserListItem')]");

                    if (objHtmlNodeCollection != null)
                    {
                        foreach (var objChildNode in objHtmlNodeCollection)
                        {
                            friendSuggetionList.Add(objChildNode.InnerHtml);
                        }
                    }
                }

                GetFriends(friendSuggetionList);
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
                    FacebookUser objFacebookUser = new FacebookUser();

                    objHtmlDocument.LoadHtml(friendItem);

                    var friendDetails = objHtmlDocument.DocumentNode.SelectNodes("//div[starts-with(@class, 'fsl fwb fcb')]")[0].InnerHtml;

                    objHtmlDocument.LoadHtml(friendDetails);

                    objFacebookUser.UserId = FdRegexUtility.FirstMatchExtractor(friendDetails, "\"eng_tid\":\"(.*?)\"");

                    objFacebookUser.Familyname = WebUtility.HtmlDecode(objHtmlDocument.DocumentNode.InnerText);

                    if (ObjFdScraperResponseParameters.ListUser.FirstOrDefault(x => x.UserId == objFacebookUser.UserId) == null)
                        ObjFdScraperResponseParameters.ListUser.Add(objFacebookUser);

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
