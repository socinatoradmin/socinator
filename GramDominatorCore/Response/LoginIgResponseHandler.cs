using GramDominatorCore.GDEnums;
using System;
using System.ComponentModel;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;

namespace GramDominatorCore.GDLibrary.Response
{
    [Localizable(false)]
    public class LoginIgResponseHandler : IGResponseHandler
    {
        public LoginIgResponseHandler(IResponseParameter response)
          : base(response)
        {
            try
            {
                if (!Success)
                {
                    // string close = RespJ["action"].ToString();
                    if (Issue == null || Issue.Error != InstagramError.TwoFactor)
                        return;
                    TwoFactor = new TwoFactorLogin(

                         RespJ["two_factor_info"]["obfuscated_phone_number"].ToString(),
                         RespJ["two_factor_info"]["two_factor_identifier"].ToString());
                }
                else if (response.Response.ToString().Contains("layout") && response.Response.ToString().Contains("profile_pic_url") && response.Response.ToString().Contains("ig.action.navigation.Login"))
                {

                    ActionBlockResponse = RespJ["layout"]["bloks_payload"]["tree"]["㐟"]["#"].ToString();
                }
                else
                {

                    var jsonHandler = new DominatorHouseCore.Utility.JsonHandler(response.Response);
                    var respJ = jsonHandler.GetElementValue("layout", "bloks_payload", "tree", "㐟", "#");
                    var responseJson = DominatorHouseCore.Utility.Utilities.GetBetween(respJ, "(bk.action.i32.Const, 35), \"", "\", (bk.action.i32.Const, 36)");
                    if (string.IsNullOrEmpty(responseJson) && respJ.Contains("(bk.action.caa.PresentCheckpointsFlow,"))
                    {
                        responseJson = DominatorHouseCore.Utility.Utilities.GetBetween(respJ, "(bk.action.caa.PresentCheckpointsFlow, \"", "\", (bk.action.core.GetArg, 0))");
                        responseJson = responseJson.Replace("\\\"", "\"");
                        responseJson = responseJson.Replace("\\\"", "\"");
                        IsChallenge = true;
                        ChallengeResponse = responseJson;
                        var jsonHand = new DominatorHouseCore.Utility.JsonHandler(responseJson);
                        var jToken = jsonHand.GetJToken("error");
                        Issue = new GramDominatorCore.GDModel.InstagramIssue()
                        {
                            Error = InstagramError.Challenge,
                            ApiPath = jToken["error_data"]["api_path"].ToString().Replace("\\","")
                        };
                        Success = false;
                        return;
                    }
                    else
                    {
                        responseJson = responseJson.Replace("\\\"", "\"");
                        responseJson = responseJson.Replace("\\\"", "\"");
                    }

                    try
                    {
                        var JsonHand = new DominatorHouseCore.Utility.JsonHandler(responseJson);
                        var loginResponseStr = JsonHand.GetElementValue("login_response");
                        var loginJsonHand = new DominatorHouseCore.Utility.JsonHandler(loginResponseStr);
                        var loginResponse = loginJsonHand.GetJToken("logged_in_user");
                        Username = loginJsonHand.GetJTokenValue(loginResponse, "username");
                        HasAnonymousProfilePicture = Convert.ToBoolean(loginJsonHand.GetJTokenValue(loginResponse, "has_anonymous_profile_picture"));
                        ProfilePicUrl = loginJsonHand.GetJTokenValue(loginResponse, "profile_pic_url");
                        FullName = loginJsonHand.GetJTokenValue(loginResponse, "full_name");
                        Pk = loginJsonHand.GetJTokenValue(loginResponse, "pk");
                        IsVerified = Convert.ToBoolean(loginJsonHand.GetJTokenValue(loginResponse, "is_verified"));
                        IsPrivate = Convert.ToBoolean(loginJsonHand.GetJTokenValue(loginResponse, "is_private"));
                        var headerJsonHand = JsonHand.GetJToken("headers").ToString();
                        AuthorizationHeader = DominatorHouseCore.Utility.Utilities.GetBetween(headerJsonHand, "{\"IG-Set-Authorization\": \"", "\"");
                    }
                    catch (Exception)
                    {

                    }


                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                throw new Exception("Error from LoginIgResponseHandler class constructor => " + response.Response);
            }
        }

        public string FullName { get; }

        public bool HasAnonymousProfilePicture { get; }

        public bool IsPrivate { get; }

        public bool IsVerified { get; }
        public bool IsChallenge { get; }
        public string ChallengeResponse { get; }
        public string Pk { get; }
        public string AuthorizationHeader { get; }

        public string ProfilePicUrl { get; }

        public TwoFactorLogin TwoFactor { get; }

        public string Username { get; }
        public string ActionBlockResponse { get; }

        public class TwoFactorLogin
        {
            public TwoFactorLogin(string obfuscatedPhoneNumber, string twoFactorIdentifier)
            {
                ObfuscatedPhoneNumber = obfuscatedPhoneNumber;
                TwoFactorIdentifier = twoFactorIdentifier;
            }

            public string ObfuscatedPhoneNumber { get; }

            public string TwoFactorIdentifier { get; }
        }


    }

    public class WebLoginIgResponseHandler : IGResponseHandler
    {
        public bool IsLogged { get; set; } = false;
        public bool OneTapPrompt { get; set; } = false;
        public WebLoginIgResponseHandler(IResponseParameter response):base(response)
        {
            try
            {
                var obj = handler.ParseJsonToJObject(response?.Response);
                bool.TryParse(handler.GetJTokenValue(obj, "authenticated"), out bool logged);
                IsLogged = logged;
            }
            catch { }
        }
    }
}
