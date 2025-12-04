using System;
using System.Collections.Generic;
using DominatorHouseCore;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Models;
using FaceDominatorCore.FDRequest;
using FaceDominatorCore.FDResponse.AccountsResponse;
using FaceDominatorCore.FDResponse.CommonResponse;
using System.Text.RegularExpressions;


namespace FaceDominatorCore.FDLibrary
{
    public class FdRequestLibrary
    {      
        public bool Login(DominatorAccountModel account)
        {
            try
            {
                account.IsUserLoggedIn = IsLoggedIn(account).LoginStatus;

                var objFdLoginResponseHandler = IsLoggedIn(account);

                var objFdFunctions = new FdFunctions(account);

                if (!objFdLoginResponseHandler.LoginStatus && objFdLoginResponseHandler.LoginParameters != null)
                {
                    var objFdRequestParameter = new FdRequestParameter();
                    
                    objFdRequestParameter.UrlParameters.Add("login_attempt","1");
                    objFdRequestParameter.UrlParameters.Add("lwv", "111");
                    var url = objFdRequestParameter.GenerateUrl(FdConstants.FbLoginPhpUrl);

                    var objFdJsonElement = objFdFunctions.GetJsonElementsForLogin(objFdLoginResponseHandler);
                    objFdRequestParameter.FdPostElements = objFdJsonElement;                 
                    var postdata = objFdRequestParameter.GetPostDataFromJson();

                    var request = account.HttpHelper.GetRequestParameter();

                    request.ContentType = "application/x-www-form-urlencoded";

                    account.HttpHelper.SetRequestParameter(request);

                    account.HttpHelper.PostRequest(url, postdata);

                    objFdLoginResponseHandler = IsLoggedIn(account);

                    if (objFdLoginResponseHandler.LoginStatus)
                    {
                        objFdFunctions.ChangeAccountStatus(account, objFdLoginResponseHandler);


                        UpdateAccountInfo(account);

                        //UpdateAccountInfo();

                        //UpdateFriends();

                        GetGroupMembers(account,"https://www.facebook.com/groups/cricket.god.sachin10/");


                        return true;
                    }
                }
            }
            catch (ArgumentNullException ex)
            {
                ex.ErrorLog($"{ex.GetType().Name} : A null parameter passed to login method.");
            }
            catch (Exception ex)
            {
                ex.ErrorLog(ex.StackTrace);
            }
            return false;
        }


        public FdLoginResponseHandler IsLoggedIn([NotNull] DominatorAccountModel accountModel)
        {
            if (accountModel == null)
                throw new ArgumentNullException(nameof(accountModel));

            var homepageResponse = accountModel.HttpHelper.GetRequest(FdConstants.FbHomeUrl);

            var loginHandler = new FdLoginResponseHandler(homepageResponse);

            accountModel.AccountBaseModel.Status = loginHandler.FbErrorDetails.Status;

            return loginHandler;
        }


        public void UpdateAccountInfo(DominatorAccountModel account)
        {
            var objFdRequestParameter = new FdRequestParameter();

            objFdRequestParameter.Url = $"profile.php?id={account.AccountBaseModel.UserId}&sk=about";

            var url = objFdRequestParameter.GenerateUrl();

            var aboutPageResponse = account.HttpHelper.GetRequest($"{FdConstants.FbHomeUrl}{url}");

            var userResponseHandler = new FdUserInfoResponseHandler(aboutPageResponse, new FacebookUser());

            objFdRequestParameter.Url =
                $"profile.php?id={account.AccountBaseModel.UserId}&sk=about&section=contact-info";
            url = objFdRequestParameter.GenerateUrl();

            var contactPageResponse = account.HttpHelper.GetRequest($"{FdConstants.FbHomeUrl}{url}");

            userResponseHandler.objFacebookUser.isFullDetailsRequired = true;

            userResponseHandler =
                new FdUserInfoResponseHandler(contactPageResponse, userResponseHandler.objFacebookUser);

            getGenderResponse(userResponseHandler, account);
        }

        private void getGenderResponse(FdUserInfoResponseHandler userResponseHandler,DominatorAccountModel account)
        {
            var objFdFunctions = new FdFunctions(account);
            var objFdRequestParameter = new FdRequestParameter();
            var objFdJsonElement = objFdFunctions.GetJsonElementsForGender(userResponseHandler.objFacebookUser.fb_dtsg);

            objFdRequestParameter.Url = "profile/edit/infotab/forms/?dpr=1";

            objFdRequestParameter.FdPostElements = objFdJsonElement;

            var url = objFdRequestParameter.GenerateUrl();
            var postdata = objFdRequestParameter.GetPostDataFromJson();

            url = FdConstants.FbHomeUrl + url;

            var request = account.HttpHelper.GetRequestParameter();

            request.ContentType = "application/x-www-form-urlencoded";

            account.HttpHelper.SetRequestParameter(request);

            var genderResponse = account.HttpHelper.PostRequest(url, postdata);

            var genderResponseHandler =
                new GenderInfoResponseHandler(genderResponse, userResponseHandler.objFacebookUser);
        }

        public void UpdateFriends(DominatorAccountModel account)
        {

            string paginationData = string.Empty;

            List<string> lstAlreadyScrapedFriends = new List<string>();

            FdFunctions objFdFunctions = new FdFunctions(account);
            FdRequestParameter objFdRequestParameter = new FdRequestParameter();

            objFdRequestParameter.Url = $"{account.AccountBaseModel.UserId}?sk=friends";

            string url = objFdRequestParameter.GenerateUrl();

            url = FdConstants.FbHomeUrl + url;

            var friendsPageResponse = account.HttpHelper.GetRequest(url);

            var friendsResponseHandler = new FdFriendsInfoResponseHandler(friendsPageResponse,false,lstAlreadyScrapedFriends,string.Empty);

            do
            {
                paginationData = friendsResponseHandler.paginationData;

                string jsonData = objFdFunctions.GetPostDataInJson(paginationData);

                jsonData = objFdFunctions.GenerateDataFromJson(jsonData);
               
                objFdRequestParameter.Url = "ajax/pagelet/generic.php/AllFriendsAppCollectionPagelet?dpr=1";

                url = objFdRequestParameter.GenerateUrl();

                url =  $"{FdConstants.FbHomeUrl}{url}&{jsonData}";
                

                var paginationResponse= account.HttpHelper.GetRequest(url);

                friendsResponseHandler = new FdFriendsInfoResponseHandler(paginationResponse, true, lstAlreadyScrapedFriends,paginationData);

            } while (!string.IsNullOrEmpty(paginationData));

        }

        public void UpdateGroups(DominatorAccountModel account)
        {
        }

        public void UpdateOwnPages(DominatorAccountModel account)
        {
        }

        public void UpdateLikeedPages(DominatorAccountModel account)
        {
        }


        public List<string> GetGroupMembers(DominatorAccountModel account,string groupUrl)
        {
            string paginationData = string.Empty;

            List<string> lstScrapedMembers = new List<string>();

            FdFunctions objFdFunctions = new FdFunctions(account);
            FdRequestParameter objFdRequestParameter = new FdRequestParameter();

            objFdRequestParameter.Url = groupUrl;

            string url = objFdRequestParameter.GenerateUrl();

            if(!url.Contains(FdConstants.FbHomeUrl))
            {
                url = FdConstants.FbHomeUrl + url;
            }

            var groupPageResponse = account.HttpHelper.GetRequest(url);

            string groupId = Regex.Matches(groupPageResponse.Response, "groupID:(.*?),", RegexOptions.Singleline)[0].Groups[1].ToString();

            groupId = Regex.Replace(groupId, "[^0-9]+", string.Empty);

            objFdRequestParameter.Url = $"groups/{groupId}/members/";

            url = objFdRequestParameter.GenerateUrl();

            url = FdConstants.FbHomeUrl + url;

            var membersPageResponse = account.HttpHelper.GetRequest(url);

            var groupMemberResponseHandler = new GroupMembersResponseHandler(membersPageResponse, false, lstScrapedMembers, string.Empty);

            do
            {
                paginationData = groupMemberResponseHandler.paginationData;

                string jsonData = objFdFunctions.GetPostdataForGroupMembers(paginationData,groupId);

                jsonData = objFdFunctions.GenerateDataFromJson(jsonData);

                objFdRequestParameter.Url = "ajax/browser/list/group_confirmed_members/";

                url = objFdRequestParameter.GenerateUrl();

                url = $"{FdConstants.FbHomeUrl}{url}?{jsonData}";


                var paginationResponse = account.HttpHelper.GetRequest(url);

                groupMemberResponseHandler = new GroupMembersResponseHandler(paginationResponse, true, lstScrapedMembers, paginationData);


            } while (!string.IsNullOrEmpty(paginationData));


            return new List<string>();
        }


    }
}