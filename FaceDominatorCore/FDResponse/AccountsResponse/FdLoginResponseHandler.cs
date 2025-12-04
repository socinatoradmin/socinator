using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDResponse.BaseResponse;
using HtmlAgilityPack;
using System;
using System.Text.RegularExpressions;

namespace FaceDominatorCore.FDResponse.AccountsResponse
{
    public class FdLoginResponseHandler : FdResponseHandler
    {
        public bool LoginStatus { get; set; }

        public string FbDtsg { get; set; } = string.Empty;

        /// <summary>
        /// Postdata for login
        /// </summary>
        public FdLoginParameters LoginParameters { get; set; }

        public string UserId { get; set; }

        public DominatorAccountModel DominatorAccountModel;

        public FdLoginResponseHandler(IResponseParameter responseParameter, bool isFristResponse
            , DominatorAccountModel accountModel)
            : base(responseParameter)
        {
            if (responseParameter.HasError)
                return;

            if (FbErrorDetails == null)
            {
                try
                {
                    DominatorAccountModel = accountModel;
                    HtmlDocument objHtmlDocument = new HtmlDocument();

                    if (string.IsNullOrEmpty(responseParameter.Response) || responseParameter.Response.Contains("ERROR: Proxy Access Denied") ||
                       responseParameter.Response.Contains("ERR_ACCESS_DENIED"))
                    {
                        FbErrorDetails = new FdErrorDetails
                        {
                            FacebookErrors = FacebookErrors.ProxyNotWorking,
                            IsStatusChangedRequired = false,
                            Status = "LangKeyLoginFailed".FromResourceDictionary(),
                            Description = "LangKeyProxyNotWorking".FromResourceDictionary()
                        };

                        return;
                    }

                    objHtmlDocument.LoadHtml(responseParameter.Response);
                    HtmlNodeCollection objHtmlNodeCollection = objHtmlDocument.DocumentNode.SelectNodes("(//form[@id=\"login_form\"])");

                    if (objHtmlNodeCollection == null)
                        objHtmlNodeCollection = objHtmlDocument.DocumentNode.SelectNodes("(//form[@data-testid=\"royal_login_form\"])");

                    // If the user is not Logged assign login parameters from responseParameters
                    if (objHtmlNodeCollection != null)
                    {                                                                                                       //_42ft _4jy0 _52e0 _4jy6 _4jy1 selected _51sy
                        if (objHtmlNodeCollection[0].InnerHtml.Contains($"{FdConstants.FbHomeUrl}recover/initiate") && !isFristResponse)
                        {
                            FbErrorDetails = new FdErrorDetails
                            {
                                FacebookErrors = FacebookErrors.InvalidLogin,
                                IsStatusChangedRequired = false,
                                Status = "LangKeyLoginFailed".FromResourceDictionary(),
                                Description = "LangKeyInvalidCredentials".FromResourceDictionary()
                            };
                        }
                        else
                        {
                            try
                            {
                                LoginParameters = new FdLoginParameters();

                                try
                                {
                                    LoginParameters.Lsd = Utilities.GetBetween(responseParameter.Response, "name=\"lsd\" value=\"", "\"");

                                    LoginParameters.Lgnrnd = Utilities.GetBetween(responseParameter.Response, "name=\"lgnrnd\" value=\"", "\"");

                                    LoginParameters.Locale = Utilities.GetBetween(responseParameter.Response, "name=\"locale\" value=\"", "\"");

                                    LoginParameters.Revision = Utilities.GetBetween(responseParameter.Response, "revision\":", ",");

                                    LoginParameters.DeskToken = Utilities.GetBetween(responseParameter.Response, "index.php\",\"", "\"");

                                    LoginParameters.ImpressionId = Utilities.GetBetween(responseParameter.Response, "imp_id\":\"", "\"");

                                    LoginParameters.RegCookieValue = Utilities.GetBetween(responseParameter.Response, "reg_instance\" value=\"", "\"");

                                }
                                catch (Exception ex)
                                {
                                    ex.DebugLog();
                                }

                                LoginStatus = false;
                            }
                            catch (Exception ex)
                            {
                                ex.DebugLog();
                            }
                        }


                    }
                    else if (objHtmlDocument.DocumentNode.SelectNodes("(//button[@id=\"checkpointSubmitButton\"])") != null
                        || responseParameter.Response.ToLower().Contains("your account has been locked") ||
                        responseParameter.Response.ToLower().Contains("we locked your account") || responseParameter.Response.ToLower().Contains("aria-label=\"Account controls and settings") ||
                        responseParameter.Response.ToLower().Contains("suspended your account") || responseParameter.Response.ToLower().Contains("enter the code that we sent to"))
                    {
                        var node = string.Empty;
                        if (responseParameter.Response.Contains("checkpointSubmitButton"))
                            node = objHtmlDocument.DocumentNode.SelectNodes("(//button[@id=\"checkpointSubmitButton\"])")[0].OuterHtml;

                        if (responseParameter.Response.Contains("suspended your account") || FdRegexUtility.FirstMatchExtractor(node, "name=\"(.*?)\"") == "submit[Download Your Information]")
                        {
                            FbErrorDetails = new FdErrorDetails
                            {
                                FacebookErrors = FacebookErrors.AccountDisbled,
                                IsStatusChangedRequired = false,
                                Status = "LangKeyLoginFailed".FromResourceDictionary(),
                                Description = "LangKeyAccountDisabled".FromResourceDictionary()
                            };
                            LoginStatus = false;
                            accountModel.AccountBaseModel.Status = DominatorHouseCore.Enums.AccountStatus.AccountDisabled;
                        }
                        else
                        {
                            FbErrorDetails = new FdErrorDetails
                            {
                                FacebookErrors = FacebookErrors.CheckPoint,
                                IsStatusChangedRequired = false,
                                Status = "LangKeyLoginFailed".FromResourceDictionary(),
                                Description = "LangKeyVerficationRequired".FromResourceDictionary()
                            };
                            LoginStatus = false;
                        }

                    }

                    else
                    {
                        objHtmlDocument = new HtmlDocument();
                        objHtmlDocument.LoadHtml(responseParameter.Response);
                        objHtmlNodeCollection = objHtmlDocument.DocumentNode.SelectNodes("(//img[@class=\"_2qgu _7ql _1m6h img\"])");
                        if (objHtmlNodeCollection?.Count > 0)
                            UserId = Regex.Replace(objHtmlNodeCollection[0].Id, "profile_pic_header_", string.Empty);
                        FbDtsg = Uri.EscapeDataString(FdRegexUtility.FirstMatchExtractor(responseParameter.Response, "fb_dtsg\" value=\"(.*?)\""));

                        if (string.IsNullOrEmpty(FbDtsg))
                            FbDtsg = Uri.EscapeDataString(FdRegexUtility.FirstMatchExtractor(responseParameter.Response, "fb_dtsg\",\"value\":\"(.*?)\""));


                        accountModel.SessionId = FbDtsg;
                        if (string.IsNullOrEmpty(UserId))
                            UserId = FdRegexUtility.FirstMatchExtractor(responseParameter.Response, "\"ACCOUNT_ID\":\"(.*?)\"");

                        LoginStatus = true;
                    }

                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
        }


        public IFdRequestLibrary FdRequestLibrary;
    }
}