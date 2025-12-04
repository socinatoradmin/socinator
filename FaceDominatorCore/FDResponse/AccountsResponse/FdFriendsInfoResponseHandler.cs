/*
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using FaceDominatorCore.FDResponse.BaseResponse;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
// ReSharper disable All

namespace FaceDominatorCore.FDResponse.AccountsResponse
{
    public class FdFriendsInfoResponseHandler : FdResponseHandler
    {

        public string PaginationData { get; set; }

        public List<FacebookUser> ListFacebookUser { get; set; }
        public string FinalEncodedQuery { get; internal set; }

        public FdFriendsInfoResponseHandler(IResponseParameter responseParameter, string pageletData)
            : base(responseParameter)
        {
            if (responseParameter.HasError)
                return;

            if (FbErrorDetails == null)
            {
                try
                {

                    ListFacebookUser = new List<FacebookUser>();



                    if (!string.IsNullOrEmpty(pageletData))
                        PaginationData = pageletData;

                    string decodedResponse = FdFunctions.GetDecodedResponse(responseParameter.Response);

                    //var userId = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.ProfileIdRegex);
                                               
                    //if(string.IsNullOrEmpty(userId))
                    //{
                    //    userId = FdRegexUtility.FirstMatchExtractor(decodedResponse, "pagelet_timeline_app_collection_(.*?):");
                    //}

                    UpdateFriendList(decodedResponse);                   

                }
                catch (ArgumentException ex)
                {
                    ex.DebugLog();
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
                catch (ArgumentException)
                {

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
                HtmlNodeCollection objHtmlNodeCollection = objHtmlDocument.DocumentNode.SelectNodes("//div[@class=\"_4p2o\"]");

                if (objHtmlNodeCollection == null)
                {
                    PaginationData = string.Empty;
                    return;
                }
             
                string[] friendItemArray = GetStringArrayFromNodeCollection(objHtmlNodeCollection);

                foreach (string friendItem in friendItemArray)
                {
                    FacebookUser objFacebookUser = new FacebookUser();

                    try
                    {
                        objHtmlDocument.LoadHtml(friendItem);
                        objHtmlNodeCollection = objHtmlDocument.DocumentNode.SelectNodes("//a[@class=\"_32mo\"]");

                        objFacebookUser.Familyname = objHtmlNodeCollection[0].InnerText;

                        objFacebookUser.UserId = FdRegexUtility.FirstMatchExtractor(friendItem, "id\":(.*?),");

                        var friendUrl = FdConstants.FbHomeUrl + objFacebookUser.UserId;

                        objFacebookUser.ProfileUrl = friendUrl;

                        if (!ListFacebookUser.Contains(objFacebookUser))
                        {
                            ListFacebookUser.Add(objFacebookUser);
                        }


                    }
                    catch(ArgumentException ex)
                    {
                        ex.DebugLog();
                    }

                    catch(Exception ex)
                    {
                        ex.DebugLog();
                    }
                }
                
            }
            catch (ArgumentException)
            {

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }




/*
        private void UpadetePaginationData(IResponseParameter responseParameter, string userId)
        {
            try
            {
                string cursor = string.Empty;

                string fullDataValue = string.Empty;

                if (responseParameter.Response.Contains("enableContentLoader") && responseParameter.Response.Contains("FriendButtonIcon"))
                {
                    var cursorData = Regex.Matches(responseParameter.Response, "enableContentLoader(.*?)FriendButtonIcon", RegexOptions.Singleline)[0].Groups[1].ToString();
                    cursor = Regex.Matches(cursorData, "},\"(.*?)\"]]", RegexOptions.Singleline)[0].Groups[1].ToString();
                }
                if (string.IsNullOrEmpty(cursor))
                {
                    PaginationData = string.Empty;
                }

                if (!string.IsNullOrEmpty(PaginationData))
                {
                    PaginationData = Uri.UnescapeDataString(PaginationData);
                    var objFdJsonElement = JsonConvert.DeserializeObject<FdJsonElement>(PaginationData);

                    fullDataValue = "{\"collection_token\":\"" + objFdJsonElement.Collectiontoken + "\",\"cursor\":\"" + cursor + "\",\"disablepager\":false,\"overview\":false,\"profile_id\":\"" + userId + "\",\"pagelet_token\":\"" + objFdJsonElement.Pagelettoken + "\",\"tab_key\":\"friends_all\",\"lst\":\"" + objFdJsonElement.Lst + "\",\"ftid\":null,\"order\":null,\"sk\":\"friends\",\"importer_state\":null}";
                }
                else if(!string.IsNullOrEmpty(cursor))
                {
                    string pageLetToken = Regex.Matches(responseParameter.Response, "pagelet_token:\"(.*?)\"}", RegexOptions.Singleline)[0].Groups[1].ToString();
                    string collectionToken = Regex.Matches(responseParameter.Response, "id=\"pagelet_timeline_app_collection_(.*?)\"", RegexOptions.Singleline)[0].Groups[1].ToString();
                    var lstMatch = Regex.Matches(responseParameter.Response, "lst:\"(.*?)\"", RegexOptions.Singleline);
                    string lst = string.Empty;
                    if(lstMatch.Count>0)
                    {
                        lst = lstMatch[0].Groups[1].ToString();
                    }

                    fullDataValue = "{\"collection_token\":\"" + collectionToken + "\",\"cursor\":\"" + cursor + "\",\"disablepager\":false,\"overview\":false,\"profile_id\":\"" + userId + "\",\"pagelet_token\":\"" + pageLetToken + "\",\"tab_key\":\"friends_all\",\"lst\":\"" + lst + "\",\"ftid\":null,\"order\":null,\"sk\":\"friends\",\"importer_state\":null}";

                }

                PaginationData = fullDataValue;
            }
            catch (ArgumentException)
            {

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
#1#
    }
}
*/
