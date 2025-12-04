/*
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using FaceDominatorCore.FDRequest;
using FaceDominatorCore.FDResponse.BaseResponse;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.Interface;

namespace FaceDominatorCore.FDResponse.NewResponseHandler
{
    public class FdFriendOfFriendResponseHandlerNew : FdResponseHandler, IResponseHandler
    {

        public bool Status { get; set; }

        public bool HasMoreResults { get; set; }

        public string EntityId { get; set; }

        public string PageletData { get; set; }

        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; }



        public FdFriendOfFriendResponseHandlerNew(IResponseParameter responseParameter, bool isPagination, string pageletData)
            : base(responseParameter)
        {
            if (responseParameter.HasError)
                return;

            if (FbErrorDetails == null)
            {
                try
                {
                    ObjFdScraperResponseParameters.ListUser = new List<FacebookUser>();

                    PageletData = pageletData;

                    string decodedResponse = FdFunctions.GetDecodedResponse(responseParameter.Response);

                    var userId = !isPagination ? Regex.Matches(decodedResponse, "{profile_id:(.*?),", RegexOptions.Singleline)[0].Groups[1].ToString() : Regex.Matches(decodedResponse, "pagelet_timeline_app_collection_(.*?):", RegexOptions.Singleline)[0].Groups[1].ToString();

                    UpdateFriendList(decodedResponse);

                    UpadetePaginationData(responseParameter, userId);

                    if (ObjFdScraperResponseParameters.ListUser.Count > 0)
                    {
                        Success = true;
                    }

                    Status = Success;

                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
        }


        private string[] GetStringArrayFromNodeCollection(HtmlNodeCollection objHtmlNodeCollection)
        {
            string[] friendItemArray = new string[objHtmlNodeCollection.Count];

            int count = 0;

            foreach (var htmlNodes in objHtmlNodeCollection)
            {
                try
                {
                    var friendItem = htmlNodes.InnerHtml;
                    friendItemArray[count] = friendItem;
                    count++;
                }
                catch (Exception ex)
                {
                    ex.DebugLog(ex.Message);
                }
            }

            return friendItemArray;
        }



        private void UpdateFriendList(string decodedResponse)
        {
            HtmlDocument objHtmlDocument = new HtmlDocument();

            try
            {
                objHtmlDocument.LoadHtml(decodedResponse);
                HtmlNodeCollection objHtmlNodeCollection = objHtmlDocument.DocumentNode.SelectNodes("(//ul[@class=\"uiList _262m _4kg\"])") ??
                                                           objHtmlDocument.DocumentNode.SelectNodes("(//ul[@class=\"uiList _262m expandedList _4kg\"])");

                objHtmlDocument.LoadHtml(objHtmlNodeCollection[0].InnerHtml);
                objHtmlNodeCollection = objHtmlDocument.DocumentNode.SelectNodes("//li");

                if (objHtmlNodeCollection == null)
                {
                    PageletData = string.Empty;
                    return;
                }

                string[] friendItemArray = GetStringArrayFromNodeCollection(objHtmlNodeCollection);

                foreach (string friendItem in friendItemArray)
                {
                    FacebookUser objFacebookUser = new FacebookUser();

                    try
                    {
                        objHtmlDocument.LoadHtml(friendItem);
                        objHtmlNodeCollection = objHtmlDocument.DocumentNode.SelectNodes("(//div[@class=\"fsl fwb fcb\"])");


                        var friendProfileIdResponse = objHtmlNodeCollection[0].InnerHtml;
                        var friendProfileId = Regex.Matches(friendProfileIdResponse, "eng_tid\":\"(.*?)\"", RegexOptions.Singleline)[0].Groups[1].ToString();

                        try
                        {
                            var friendProfileName = FdRegexUtility.FirstMatchExtractor(friendProfileIdResponse, FdConstants.FamilyNameRegex);

                            objFacebookUser.Familyname = friendProfileName;

                            var friendButtonDetails = objHtmlDocument.DocumentNode.SelectNodes("(//button[@class=\"_42ft _4jy0 FriendRequestAdd addButton _4jy3 _4jy1 selected _51sy\"])")[0].OuterHtml;

                        }
                        catch (Exception ex)
                        {
                            objFacebookUser.IsAlreadyFriend = true;
                            ex.DebugLog(ex.Message);
                        }

                        objFacebookUser.UserId = friendProfileId;

                        if (!ObjFdScraperResponseParameters.ListUser.Contains(objFacebookUser))
                        {
                            ObjFdScraperResponseParameters.ListUser.Add(objFacebookUser);
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }




        private void UpadetePaginationData(IResponseParameter responseParameter, string userId)
        {
            try
            {
                string fullDataValue;

                string cursorData = string.Empty;
                try
                {
                    cursorData = Regex.Matches(responseParameter.Response, "enableContentLoader(.*?)FriendButtonIcon", RegexOptions.Singleline)[0].Groups[1].ToString();
                }

                catch (Exception)
                {
                    try
                    {
                        cursorData = Regex.Matches(responseParameter.Response, "enableContentLoader(.*?)AddFriendButton", RegexOptions.Singleline)[0].Groups[1].ToString();

                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }



                string cursor = Regex.Matches(cursorData, "},\"(.*?)\"]]", RegexOptions.Singleline)[0].Groups[1].ToString();

                if (string.IsNullOrEmpty(cursor))
                {
                    PageletData = string.Empty;
                    HasMoreResults = false;
                    return;
                }

                if (!string.IsNullOrEmpty(PageletData))
                {
                    PageletData = Uri.UnescapeDataString(PageletData);
                    var objFdJsonElement = JsonConvert.DeserializeObject<FdJsonElement>(PageletData);

                    fullDataValue = "{\"collection_token\":\"" + objFdJsonElement.Collectiontoken + "\",\"cursor\":\"" + cursor + "\",\"disablepager\":false,\"overview\":false,\"profile_id\":\"" + userId + "\",\"pagelet_token\":\"" + objFdJsonElement.Pagelettoken + "\",\"tab_key\":\"friends_all\",\"lst\":\"" + objFdJsonElement.Lst + "\",\"ftid\":null,\"order\":null,\"sk\":\"friends\",\"importer_state\":null}";
                }
                else
                {
                    string pageLetToken = Regex.Matches(responseParameter.Response, "pagelet_token:\"(.*?)\"}", RegexOptions.Singleline)[0].Groups[1].ToString();
                    string collectionToken = Regex.Matches(responseParameter.Response, "id=\"pagelet_timeline_app_collection_(.*?)\"", RegexOptions.Singleline)[0].Groups[1].ToString();
                    string lst = Regex.Matches(responseParameter.Response, "lst:\"(.*?)\"", RegexOptions.Singleline)[0].Groups[1].ToString();

                    fullDataValue = "{\"collection_token\":\"" + collectionToken + "\",\"cursor\":\"" + cursor + "\",\"disablepager\":false,\"overview\":false,\"profile_id\":\"" + userId + "\",\"pagelet_token\":\"" + pageLetToken + "\",\"tab_key\":\"friends_all\",\"lst\":\"" + lst + "\",\"ftid\":null,\"order\":null,\"sk\":\"friends\",\"importer_state\":null}";

                }

                HasMoreResults = true;
                PageletData = fullDataValue;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}
*/
