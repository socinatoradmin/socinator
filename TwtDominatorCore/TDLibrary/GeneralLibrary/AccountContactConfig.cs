using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using TwtDominatorCore.Requests;
using TwtDominatorCore.TDUtility;
using Unity;
using static TwtDominatorCore.TDEnums.Enums;
using GenderGuesser = DominatorHouseCore.Utility.GenderGuesser;

namespace TwtDominatorCore.TDLibrary.GeneralLibrary
{
    public interface IAccountContactConfig
    {
        Task<UserProfileDetails> GetUserContactDetails(DominatorAccountModel dominatorAccountModel,
            bool GetFullDetails = false);

        void SaveUserContactDetails(UserProfileDetails userContactDetails);
    }

    public class AccountContactConfig : IAccountContactConfig
    {
        private readonly IAccountScopeFactory _accountScopeFactory;
        private static string Domain=>TdConstants.Domain;
        public AccountContactConfig(IAccountScopeFactory accountScopeFactory)
        {
            _accountScopeFactory = accountScopeFactory;
        }

        /// <summary>
        ///     Getting UserContactDetails after sucessfull login
        /// </summary>
        /// <param name="dominatorAccountModel"></param>
        /// <returns></returns>
        public async Task<UserProfileDetails> GetUserContactDetails(DominatorAccountModel dominatorAccountModel,
            bool GetFullDetails = false)
        {
            var httpHelper = _accountScopeFactory[dominatorAccountModel.AccountId]
                .Resolve<ITdHttpHelper>();
            // here for resolving ITwitterFunctions we don't need network
            var twtFunction = _accountScopeFactory[dominatorAccountModel.AccountId]
                .Resolve<ITwitterFunctions>();
            var userProfileDetails = new UserProfileDetails();
            try
            {
                var userName = GetUserName(dominatorAccountModel);
                var userDetail = twtFunction
                    .GetUserDetails(dominatorAccountModel, userName, QueryInfo.NoQuery.QueryType).UserDetail;

                userProfileDetails.FullName = userDetail.FullName ?? dominatorAccountModel.AccountBaseModel.UserFullName;
                userProfileDetails.Bio = userDetail.UserBio;
                userProfileDetails.UserName = userName ?? dominatorAccountModel.AccountBaseModel.UserName;
                userProfileDetails.ProfilePicUrl = userDetail.ProfilePicUrl ?? dominatorAccountModel.AccountBaseModel.ProfilePictureUrl;

                if (GetFullDetails)
                {
                    var refererUrl = $"https://{Domain}/settings/your_twitter_data/verify_password";
                    SetReqParams(ref dominatorAccountModel);

                    var passwordPostData =
                        $"return_to=%2Fsettings%2Fyour_twitter_data&auth_password={dominatorAccountModel.AccountBaseModel.Password}";
                    var TwitterDataPageResponse = httpHelper
                        .PostRequest(refererUrl, passwordPostData).Response;
                    userProfileDetails.PhoneNumber = HtmlAgilityHelper
                        .getStringInnerTextFromClassName(TwitterDataPageResponse, "messaging-device")
                        .Replace("Edit", "").Trim();

                    userProfileDetails.Email = GetEmailFromTwitterData(TwitterDataPageResponse);
                    var gender = GenderGuesser.GetGender(userProfileDetails?.FullName);
                    userProfileDetails.Gender = gender.ToString();
                    userProfileDetails.authenticityToken = GetAuthenticityToken(TwitterDataPageResponse);
                    userProfileDetails.csrfToken = dominatorAccountModel.Cookies.OfType<Cookie>()
                        .FirstOrDefault(x => x.Name == "ct0" && x.Domain == ".x.com").Value ?? dominatorAccountModel.Cookies.OfType<Cookie>()
                        .FirstOrDefault(x => x.Name == "ct0").Value;

                    #region last Data

                    //IRequestParameters reqParam = dominatorAccountModel.HttpHelper.GetRequestParameter();
                    //reqParam.Referer = twtData;
                    //reqParam.Headers.Add("Upgrade-Insecure-Requests","1");
                    //dominatorAccountModel.HttpHelper.SetRequestParameter(reqParam);
                    //string pageResponse = dominatorAccountModel.HttpHelper.GetRequest(twtData).Response;
                    //string SettingResponse = (await dominatorAccountModel.HttpHelper.GetRequestAsync(TdConstants.MainUrl + "settings/account", new CancellationToken())).Response;
                    //userProfileDetails.UserName = Utilities.GetBetween(SettingResponse, "orig_uname\" value=\"", "\"");
                    //userProfileDetails.Email = Utilities.GetBetween(SettingResponse, "orig_email\" value=\"", "\"");
                    //string MobilePageResponse = dominatorAccountModel.HttpHelper.GetRequest($"https://{Domain}/settings/devices").Response;
                    //userProfileDetails.PhoneNumber = HtmlAgilityHelper.getStringInnerHtmlFromClassName(MobilePageResponse, "device_number_with_country_code");
                    //userProfileDetails.PhoneNumber = WebUtility.HtmlDecode(userProfileDetails.PhoneNumber); 

                    #endregion
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return userProfileDetails;
        }

        /// <summary>
        ///     Saving UserContactDetails to csv file
        /// </summary>
        /// <returns></returns>
        public void SaveUserContactDetails(UserProfileDetails userContactDetails)
        {
            try
            {
                var ListAccountDetails = new List<string>();
                var folderPath = ConstantVariable.GetIndexAccountDir();
                var fileName = "AccountDetails.csv";
                var fullPath = $"{folderPath}\\{fileName}";
                var currentData =
                    $"{userContactDetails.UserName},{userContactDetails.Email},'{userContactDetails.PhoneNumber}'";

                if (File.Exists(fullPath))
                {
                    ListAccountDetails = File.ReadAllLines(fullPath).ToList();
                    if (!ListAccountDetails.Contains(currentData))
                    {
                        ListAccountDetails.Add(currentData);
                        File.WriteAllLines(fullPath, ListAccountDetails.ToArray());
                    }
                }
                else
                {
                    ListAccountDetails.Add(currentData);
                    File.WriteAllLines(fullPath, ListAccountDetails.ToArray());
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public static void ErrorMessage(DominatorAccountModel dominatorAccountModel, LoginStatus status)
        {
            try
            {
                switch (status)
                {
                    case LoginStatus.RetypePhoneNumber:
                    {
                        dominatorAccountModel.AccountBaseModel.Status = AccountStatus.ReTypePhoneNumber;

                        GlobusLogHelper.log.Info(Log.LoginFailed, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                            dominatorAccountModel.UserName, "LangKeyRetypePhoneNumber".FromResourceDictionary());
                    }
                        break;
                    case LoginStatus.RetypeEmail:
                    {
                        dominatorAccountModel.AccountBaseModel.Status = AccountStatus.ReTypeEmail;
                        GlobusLogHelper.log.Info(Log.LoginFailed, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                            dominatorAccountModel.UserName, "LangKeyRetypeEmail".FromResourceDictionary());
                    }
                        break;
                    case LoginStatus.Captcha:
                    {
                        dominatorAccountModel.AccountBaseModel.Status = AccountStatus.Failed;
                        GlobusLogHelper.log.Info(Log.LoginFailed, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                            dominatorAccountModel.UserName, "LangKeyRecatchaChallenge".FromResourceDictionary());
                    }
                        break;
                    case LoginStatus.VerifyEmail:
                    {
                        dominatorAccountModel.AccountBaseModel.Status = AccountStatus.EmailVerification;
                        GlobusLogHelper.log.Info(Log.LoginFailed, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                            dominatorAccountModel.UserName, "LangKeyVerifyEmail".FromResourceDictionary());
                    }
                        break;
                    case LoginStatus.Add_Phonenumber:
                    {
                        dominatorAccountModel.AccountBaseModel.Status = AccountStatus.AddPhoneNumberToYourAccount;
                        GlobusLogHelper.log.Info(Log.LoginFailed, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                            dominatorAccountModel.UserName,
                            "LangKeyAddPhoneNumberToYourAccount".FromResourceDictionary());
                    }
                        break;
                    //case LoginStatus.VerifyEmail:
                    //{
                    //    dominatorAccountModel.AccountBaseModel.Status = AccountStatus.EmailVerification;
                    //    GlobusLogHelper.log.Info(Log.LoginFailed, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                    //        dominatorAccountModel.UserName, "Email Verification Required");
                    //}
                    //    break;
                    default:
                    {
                        AccountStatus failStatus;
                        try
                        {
                            failStatus = (AccountStatus) Enum.Parse(typeof(AccountStatus), status.ToString(), true);
                        }
                        catch (Exception)
                        {
                            failStatus = AccountStatus.Failed;
                        }

                        dominatorAccountModel.AccountBaseModel.Status = failStatus;
                        GlobusLogHelper.log.Info(Log.LoginFailed, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                            dominatorAccountModel.UserName, dominatorAccountModel.AccountBaseModel.Status.ToString());
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void SetReqParams(ref DominatorAccountModel dominatorAccountModel)
        {
            try
            {
                var httpHelper = _accountScopeFactory[dominatorAccountModel.AccountId]
                    .Resolve<ITdHttpHelper>();

                #region setting reqparams

                var refererUrl = $"https://{Domain}/settings/your_twitter_data/verify_password";
                var RequestParameter = httpHelper.GetRequestParameter();
                var CsrfToken = dominatorAccountModel.Cookies.OfType<Cookie>().SingleOrDefault(x => x.Name == "ct0")
                    ?.Value;
                RequestParameter.Headers.Add("x-csrf-token", CsrfToken);
                RequestParameter.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                RequestParameter.Headers.Add("Upgrade-Insecure-Requests", "1");
                RequestParameter.Referer = refererUrl;
                httpHelper.SetRequestParameter(RequestParameter);

                #endregion
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }

        public static string GetEmailFromTwitterData(string TwitterDataPageResponse)
        {
            var email = "";
            try
            {
                var MatchedClassesHtml =
                    HtmlAgilityHelper.getListInnerHtmlFromClassName(TwitterDataPageResponse, "control-group");

                foreach (var data in MatchedClassesHtml)
                    if (data.Contains("Email"))
                    {
                        email = HtmlAgilityHelper.getStringTextFromClassName(data, "value");
                        if (string.IsNullOrEmpty(email?.Trim()))
                        {
                            var EmailPattern = @"(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@" +
                                               @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\." +
                                               @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|" +
                                               @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})";

                            var match = Regex.Match(TwitterDataPageResponse, EmailPattern);
                            if (match.Success) email = $"{match.Groups[0]}(pending confirmation)";
                        }
                    }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return email;
        }

        public static string GetAuthenticityToken(string TwitterDataPageResponse)
        {
            var authenticityToken = "";
            try
            {
                var tagData = HtmlAgilityHelper.MethodGetStringFromId(TwitterDataPageResponse, "authenticity_token");
                authenticityToken = Utilities.GetBetween(tagData, "value=\"", "\"");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return authenticityToken;
        }

        private string GetUserName(DominatorAccountModel dominatorAccountModel)
        {
            var userName = "";
            try
            {
                var httpHelper = _accountScopeFactory[dominatorAccountModel.AccountId]
                    .Resolve<ITdHttpHelper>();
                if (string.IsNullOrEmpty(dominatorAccountModel.AccountBaseModel?.ProfileId))
                    if (dominatorAccountModel.AccountBaseModel.UserName.Contains("@"))
                    {
                        userName = dominatorAccountModel.AccountBaseModel.UserName?.Replace("@", "");
                    }
                    else
                    {
                        userName = dominatorAccountModel.AccountBaseModel.UserName;
                    }
                else
                    userName = dominatorAccountModel.AccountBaseModel?.ProfileId;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return userName;
        }
    }
}