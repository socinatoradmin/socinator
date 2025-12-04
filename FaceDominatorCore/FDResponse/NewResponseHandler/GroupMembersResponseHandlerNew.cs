/*
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using FaceDominatorCore.FDResponse.BaseResponse;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.Interface;

namespace FaceDominatorCore.FDResponse.NewResponseHandler
{
    public class GroupMembersResponseHandlerNew : FdResponseHandler,IResponseHandler
    {
        public bool HasMoreResults { get; set; }

        public List<FacebookUser> LstFacebookUser { get; set; }

        public string EntityId { get; set; }

        public string PageletData { get; set; }

        public bool Status { get; set; }

        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; }

        public GroupMembersResponseHandlerNew(IResponseParameter responseParameter, bool isPagination, string pageletData)
            : base(responseParameter)
        {
            if (responseParameter.HasError)
                return;

            if (FbErrorDetails == null)
            {
                PageletData = !string.IsNullOrEmpty(pageletData) ? pageletData : string.Empty;

                LstFacebookUser = new List<FacebookUser>();

                string decodedResponse = FdFunctions.GetDecodedResponse(responseParameter.Response);

                HtmlDocument objHtmlDocument = new HtmlDocument();

                try
                {
                    if (!isPagination)
                    {

                        decodedResponse = Regex.Split(decodedResponse, "groupsMemberBrowserContent")[1];

                        GetInitialMembers(decodedResponse);

                    }

                    objHtmlDocument.LoadHtml(decodedResponse);
                    HtmlNodeCollection objHtmlNodeCollection = objHtmlDocument.DocumentNode.SelectNodes("(//div[@data-name=\"GroupProfileGridItem\"])");

                    GetMembersNodeCollection(objHtmlNodeCollection);


                    UpadetePaginationData(responseParameter);

                    if (LstFacebookUser.Count > 0)
                        Success = true;

                    Status = Success;
                }
                catch (Exception ex)
                {
                    ex.DebugLog(ex.Message);
                }
            }
        }


        private void UpadetePaginationData(IResponseParameter responseParameter)
        {
            try
            {
                string cursor = Regex.Matches(responseParameter.Response, "recently_joined&cursor=(.*?)&", RegexOptions.Singleline)[0].Groups[1].ToString();

                PageletData = string.IsNullOrEmpty(cursor) ? string.Empty : cursor;

                if (!string.IsNullOrEmpty(PageletData))
                    HasMoreResults = true;

            }
            catch (Exception ex)
            {
                ex.DebugLog(ex.Message);
            }

        }


        private void GetInitialMembers(string responseParameter)
        {
            try
            {
                HtmlDocument objHtmlDocument = new HtmlDocument();

                #region Group Admin and Moderator

                objHtmlDocument.LoadHtml(responseParameter);
                var adminSectionCollection = objHtmlDocument.DocumentNode.SelectNodes("(//div[@id=\"groupsMemberSection_admins_moderators\"])");

                HtmlNodeCollection objHtmlNodeCollection;

                if (adminSectionCollection != null)
                {
                    string groupAdminsSection = adminSectionCollection[0].InnerHtml;

                    objHtmlDocument.LoadHtml(groupAdminsSection);

                    objHtmlNodeCollection = objHtmlDocument.DocumentNode.SelectNodes("(//div[@data-name=\"GroupProfileGridItem\"])"); //data-name="GroupProfileGridItem" 

                    GetMembersNodeCollection(objHtmlNodeCollection);
                }


                #endregion

                #region GroupMembers Recently Joined

                objHtmlDocument.LoadHtml(responseParameter);
                var recentlyJoinedCollection = objHtmlDocument.DocumentNode.SelectNodes("(//div[@id=\"groupsMemberSection_recently_joined\"])");

                if (recentlyJoinedCollection != null)
                {
                    string recentlyJoined = recentlyJoinedCollection[0].InnerHtml;
                    objHtmlDocument.LoadHtml(recentlyJoined);
                    objHtmlNodeCollection = objHtmlDocument.DocumentNode.SelectNodes("(//div[@data-name=\"GroupProfileGridItem\"])");

                    GetMembersNodeCollection(objHtmlNodeCollection); //clearfix _60rh _gse

                }

                #endregion

                #region GroupMembers Friends

                objHtmlDocument.LoadHtml(responseParameter);
                var sectionfriendsCollection = objHtmlDocument.DocumentNode.SelectNodes("(//div[@id=\"groupsMemberSection_friends\"])");
                if (sectionfriendsCollection != null)
                {
                    objHtmlDocument.LoadHtml(sectionfriendsCollection[0].InnerHtml);
                    objHtmlNodeCollection = objHtmlDocument.DocumentNode.SelectNodes("(//div[@data-name=\"GroupProfileGridItem\"])");

                    GetMembersNodeCollection(objHtmlNodeCollection);
                }

                #endregion

                #region GroupMembers ThingsInCommon

                objHtmlDocument.LoadHtml(responseParameter);
                var sectionThingsInCommonCollection = objHtmlDocument.DocumentNode.SelectNodes("(//div[@id=\"groupsMemberSection_things_in_common\"])");

                if (sectionThingsInCommonCollection != null)
                {
                    objHtmlDocument.LoadHtml(sectionThingsInCommonCollection[0].InnerHtml);
                    objHtmlNodeCollection = objHtmlDocument.DocumentNode.SelectNodes("(//div[@class=\"clearfix _60rh _gse\"])");

                    GetMembersNodeCollection(objHtmlNodeCollection);
                }


                #endregion

                #region GroupMembers LocalMembers

                objHtmlDocument.LoadHtml(responseParameter);
                var localMembersCollection = objHtmlDocument.DocumentNode.SelectNodes("(//div[@id=\"groupsMemberSection_local_members\"])");

                if (localMembersCollection != null)
                {
                    objHtmlDocument.LoadHtml(localMembersCollection[0].InnerHtml);
                    objHtmlNodeCollection = objHtmlDocument.DocumentNode.SelectNodes("(//div[@class=\"clearfix _60rh _gse\"])");

                    GetMembersNodeCollection(objHtmlNodeCollection);
                }

                //clearfix _60rh _gse

                #endregion


            }
            catch (Exception ex)
            {
                ex.DebugLog(ex.Message);
            }
        }

        private void GetMembersNodeCollection(HtmlNodeCollection objHtmlNodeCollection)
        {
            FdFunctions objFdFunctions = new FdFunctions();

            HtmlDocument objHtmlDocument = new HtmlDocument();

            var nodecollection = objFdFunctions.GetInnerHtmlListFromNodeCollection(objHtmlNodeCollection);

            try
            {
                foreach (var node in nodecollection)
                {
                    FacebookUser objFacebookUser = new FacebookUser();

                    try
                    {
                        var memberNode = node;
                        string memberId;
                        try
                        {
                            memberId = Regex.Matches(memberNode, "member_id=(.*?)&", RegexOptions.Singleline)[0].Groups[1].ToString();
                            memberId = FdFunctions.GetIntegerOnlyString(memberId);
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                            memberId = Regex.Matches(memberNode, "id=(.*?)&", RegexOptions.Singleline)[0].Groups[1].ToString();
                            memberId = FdFunctions.GetIntegerOnlyString(memberId);
                        }



                        try
                        {
                            objHtmlDocument.LoadHtml(memberNode);
                            var objNodeCollection = objHtmlDocument.DocumentNode.SelectNodes("(//div[@class=\"_60ri fsl fwb fcb\"])"); //data-name="GroupProfileGridItem" 
                            var memberName = objNodeCollection[0].InnerHtml;
                            memberName = FdRegexUtility.FirstMatchExtractor(memberName, FdConstants.FamilyNameRegex);

                            objFacebookUser.Familyname = memberName;


                        }
                        catch (Exception ex)
                        {

                            ex.ErrorLog(ex.Message);
                        }
                        try
                        {
                            var friendButtonDetails = objHtmlDocument.DocumentNode.SelectNodes("(//button[@class=\"_42ft _4jy0 FriendRequestAdd addButton _4jy3 _517h _51sy\"])")[0].OuterHtml;

                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                            objFacebookUser.IsAlreadyFriend = true;
                        }

                        objFacebookUser.UserId = memberId;
                        if (LstFacebookUser.FirstOrDefault(x => x.UserId == objFacebookUser.UserId) == null)
                        {
                            LstFacebookUser.Add(objFacebookUser);
                        }

                        if (LstFacebookUser.Count > 0)
                            Success = true;
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog(ex.Message);
            }
        }
    }
}
*/
