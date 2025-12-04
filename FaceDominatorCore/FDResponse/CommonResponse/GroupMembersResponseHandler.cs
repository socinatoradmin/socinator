using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using FaceDominatorCore.FDEnums;
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
    public class GroupMembersResponseHandler : FdResponseHandler, IResponseHandler
    {
        public bool HasMoreResults { get; set; }

        public string EntityId { get; set; }

        public string PageletData { get; set; }

        public bool Status { get; set; }

        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; }

        public GroupMembersResponseHandler(IResponseParameter responseParameter, bool isPagination
            , string pageletData, GroupMemberCategory groupMemberCategory)
            : base(responseParameter)
        {
            if (responseParameter.HasError)
                return;

            ObjFdScraperResponseParameters = new FdScraperResponseParameters();

            ObjFdScraperResponseParameters.ListUser = new List<FacebookUser>();

            pageletData = !string.IsNullOrEmpty(pageletData) ? pageletData : string.Empty;

            string decodedResponse = FdFunctions.GetNewPrtialDecodedResponse(responseParameter.Response);

            HtmlDocument objHtmlDocument = new HtmlDocument();

            try
            {
                if (!isPagination)
                {
                    //var membersData = Regex.Split(decodedResponse, "groupsMemberBrowserContent")[1];
                    string membersData = string.Empty;
                    var data = Regex.Split(decodedResponse, "groupsMemberBrowserContent");
                    if (data.Length > 0)
                        membersData = data[1];

                    GetInitialMembers(membersData);
                }

                objHtmlDocument.LoadHtml(decodedResponse);

                HtmlNodeCollection objHtmlNodeCollection = objHtmlDocument.DocumentNode.SelectNodes("(//div[@data-name=\"GroupProfileGridItem\"])");

                if (objHtmlNodeCollection == null)
                {
                    objHtmlNodeCollection = objHtmlDocument.DocumentNode.SelectNodes("(//div[@class=\"clearfix _60rh _gse\"])");
                }

                GetMembersNodeCollection(objHtmlNodeCollection);


                UpadetePaginationData(decodedResponse, groupMemberCategory);

                Status = ObjFdScraperResponseParameters.ListUser.Count > 0;
            }
            catch (Exception ex)
            {
                ex.DebugLog(ex.Message);
            }
        }


        private void UpadetePaginationData(string responseParameter, GroupMemberCategory groupMemberCategory)
        {
            try
            {
                string cursor;

                if (groupMemberCategory == GroupMemberCategory.AllMembers)
                    cursor = FdRegexUtility.FirstMatchExtractor(responseParameter, "recently_joined&cursor=(.*?)&");

                else if (groupMemberCategory == GroupMemberCategory.AdminsAndModerators ||
                    groupMemberCategory == GroupMemberCategory.Friends)
                    cursor = FdRegexUtility.FirstMatchExtractor(responseParameter, "default&cursor=(.*?)&");
                else
                    cursor = FdRegexUtility.FirstMatchExtractor(responseParameter, "aftercursor=(.*?)&");

                cursor = Uri.UnescapeDataString(cursor);

                PageletData = string.IsNullOrEmpty(cursor) ? string.Empty : cursor;

                HasMoreResults = !string.IsNullOrEmpty(PageletData);

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
            try
            {
                string memberId = string.Empty;

                FdFunctions objFdFunctions = new FdFunctions();

                HtmlDocument objHtmlDocument = new HtmlDocument();

                var nodecollection = objFdFunctions.GetInnerHtmlListFromNodeCollection(objHtmlNodeCollection);


                foreach (var node in nodecollection)
                {
                    FacebookUser objFacebookUser = new FacebookUser();

                    try
                    {
                        var memberNode = node;
                        try
                        {
                            //memberId = FdRegexUtility.FirstMatchExtractor(memberNode, "member_id=(.*?)&");

                            var scrapedProfileUrl = FdRegexUtility.FirstMatchExtractor(memberNode, FdConstants.ScrapedUrlRegx);

                            objFacebookUser.ScrapedProfileUrl = scrapedProfileUrl;

                            //if (string.IsNullOrEmpty(memberId))
                            //{
                            memberId = FdRegexUtility.FirstMatchExtractor(memberNode, "user.php\\?id=(.*?)&");
                            //}
                            memberId = FdFunctions.GetIntegerOnlyString(memberId);
                        }
                        catch (ArgumentException)
                        {

                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }

                        if (string.IsNullOrEmpty(memberId))
                            continue;

                        try
                        {
                            objHtmlDocument.LoadHtml(memberNode);
                            var objNodeCollection = objHtmlDocument.DocumentNode.SelectNodes("(//div[@class=\"_60ri fsl fwb fcb\"])"); //data-name="GroupProfileGridItem" 
                            string memberName;
                            if (objNodeCollection != null && objNodeCollection.Count > 0)
                            {
                                memberName = objNodeCollection[0].InnerHtml;
                                memberName = FdRegexUtility.FirstMatchExtractor(memberName, FdConstants.FamilyNameRegex);

                            }
                            else
                            {
                                memberName = FdRegexUtility.FirstMatchExtractor(memberNode, "aria-label=\"(.*?)\"");
                            }

                            objFacebookUser.Familyname = memberName;


                        }
                        catch (Exception ex)
                        {

                            ex.DebugLog(ex.Message);
                        }

                        objFacebookUser.IsAlreadyFriend = "false";

                        objFacebookUser.UserId = memberId;
                        if (ObjFdScraperResponseParameters.ListUser.FirstOrDefault(x => x.UserId == objFacebookUser.UserId) == null)
                            ObjFdScraperResponseParameters.ListUser.Add(objFacebookUser);

                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog(ex.Message);
                    }
                }

                Status = ObjFdScraperResponseParameters.ListUser.Count > 0;

            }
            catch (Exception ex)
            {
                ex.DebugLog(ex.Message);
            }
        }
    }
}
