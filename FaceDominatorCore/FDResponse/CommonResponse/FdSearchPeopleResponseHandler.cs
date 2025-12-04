using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDResponse.BaseResponse;
using FaceDominatorCore.Interface;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FaceDominatorCore.FDResponse.CommonResponse
{
    public class FdSearchPeopleResponseHandler : FdResponseHandler, IResponseHandler
    {
        public bool HasMoreResults { get; set; }

        public string EntityId { get; set; }

        public string PageletData { get; set; }

        public bool Status { get; set; }

        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; }

        public FdSearchPeopleResponseHandler(IResponseParameter responseParameter)
            : base(responseParameter)
        {

            ObjFdScraperResponseParameters = new FdScraperResponseParameters();

            ObjFdScraperResponseParameters.ListUser = new List<FacebookUser>();

            string decodedResponse = FdFunctions.GetDecodedResponse(responseParameter.Response);

            HtmlDocument objHtmlDocument = new HtmlDocument();

            objHtmlDocument.LoadHtml(decodedResponse);

            // HtmlNodeCollection objNodeCollection = objHtmlDocument.DocumentNode.SelectNodes("(//div[@class=\"_4p2o\"])");
            HtmlNodeCollection objNodeCollection = objHtmlDocument.DocumentNode.SelectNodes("//div[starts-with(@class, '_4p2o')]");
            if (objNodeCollection != null)
            {
                List<string> fanpageResponseList = new List<string>();

                objNodeCollection.ForEach(objNode =>
                        fanpageResponseList.Add(objNode.InnerHtml));

                GetProfileIds(fanpageResponseList);

                Status = ObjFdScraperResponseParameters.ListUser.Count > 0;

            }
        }

        private void GetProfileIds(List<string> fanpageResponseList)
        {
            string scrapedUrl = string.Empty;

            HtmlDocument objHtmlDocument = new HtmlDocument();

            foreach (string response in fanpageResponseList)
            {
                FacebookUser objFacebookUser = new FacebookUser();

                var username = string.Empty;

                try
                {
                    objHtmlDocument.LoadHtml(response);

                    var userData = objHtmlDocument.DocumentNode.SelectNodes("//div[starts-with(@class, '_3u1 _gli')]");
                    var userId = userData != null && userData.Count > 0 ? userData[0].OuterHtml : string.Empty;
                    var spliteValue = Regex.Matches(userId, "\"id\":\"(.*?)\",");
                    userId = spliteValue != null && spliteValue.Count > 0 ? spliteValue[0].Groups[1].ToString() : string.Empty;
                    userId = FdFunctions.GetIntegerOnlyString(userId);

                    try
                    {
                        var userNameData = objHtmlDocument.DocumentNode.SelectNodes("(//div[@class=\"_52eh _5bcu\"])");
                        username = userNameData != null && userNameData.Count > 0 ? userNameData[0].OuterHtml : string.Empty;

                        scrapedUrl = FdRegexUtility.FirstMatchExtractor(username, FdConstants.ScrapedUrlRegx);

                        username = FdRegexUtility.FirstMatchExtractor(username, FdConstants.UserNameModRegx);

                        var friendsButtonDetailsData = objHtmlDocument.DocumentNode.SelectNodes("(//button[@class=\"_42ft _4jy0 FriendRequestAdd addButton _4jy3 _517h _51sy\"])");
                        // ReSharper disable once UnusedVariable
                        var friendButtonDetails = friendsButtonDetailsData != null && friendsButtonDetailsData.Count > 0 ? friendsButtonDetailsData[0].OuterHtml : string.Empty;
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                        objFacebookUser.IsAlreadyFriend = "true";
                    }

                    objFacebookUser.UserId = userId;

                    objFacebookUser.Familyname = username;

                    objFacebookUser.ScrapedProfileUrl = scrapedUrl;

                    if (ObjFdScraperResponseParameters.ListUser.FirstOrDefault(x => x.UserId == objFacebookUser.UserId) == null)
                        ObjFdScraperResponseParameters.ListUser.Add(objFacebookUser);

                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
        }
    }
}
