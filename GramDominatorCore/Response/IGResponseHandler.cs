using CefSharp.DevTools.Database;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Requests;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDEnums;
using GramDominatorCore.GDModel;
using GramDominatorCore.GDUtility;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Text;

namespace GramDominatorCore.GDLibrary.Response
{
    public abstract class IGResponseHandler
    {
        protected readonly JObject RespJ;
        protected readonly IResponseParameter response;
        public readonly JsonJArrayHandler handler = JsonJArrayHandler.GetInstance;
        protected IGResponseHandler()
        {
            Success = false;
        }
        protected IGResponseHandler(IResponseParameter response)
        {
            try
            {
                this.response = response;
                if (string.IsNullOrEmpty(response.Response))
                {
                    Success = false;
                    return;
                }

                if (response.HasError)
                {
                    WebHelper.WebExceptionIssue errorMsgWebrequest;
                    try
                    {
                        errorMsgWebrequest = ((WebException)response.Exception).GetErrorMsgWebrequest();
                    }
                    catch (Exception)
                    {
                        errorMsgWebrequest = new WebHelper.WebExceptionIssue()
                        {
                            MessageLong = response.Exception.Message
                        };
                    }

                    Success = false;
                    Issue = new InstagramIssue()
                    {
                        Message = errorMsgWebrequest.MessageLong,
                        Error = InstagramError.FailedRequest
                    };
                }
                else
                {
                    if (response.Response != null && (response.Response.Contains("DOCTYPE html") || response.Response.IsValidJson()))
                    {
                        Success = true;
                        if (response.Response.IsValidJson())
                        {
                            try
                            {
                                RespJ = handler.ParseJsonToJObject(response.Response);
                                if(RespJ != null)
                                {
                                    Issue = new InstagramIssue()
                                    {
                                        Message = handler.GetJTokenValue(RespJ,"message")
                                    };
                                    Success = Issue != null && string.IsNullOrEmpty(Issue?.Message);
                                }
                            }
                            catch (Exception)
                            {

                            }
                        }
                        return;
                    }
                    try
                    {
                        RespJ = handler.ParseJsonToJObject(response.Response);
                        if(RespJ is null)
                        {
                            var FormattedJson = Utilities.ValidateJsonString(response.Response);
                            RespJ = handler.ParseJsonToJObject(FormattedJson);
                            Success = RespJ != null;
                            response.Response = FormattedJson;
                            if (Success)
                                return;
                        }
                        else
                        {
                            Success = true;
                            return;
                        }
                    }
                    catch (Exception)
                    {

                    }
                    if (RespJ.Property("errors") == null && RespJ["status"] != null && RespJ["status"].ToString() != "fail")
                    {
                        Success = true;
                    }
                    else
                    {
                        Success = false;
                        if (RespJ["message"] != null && RespJ["message"].HasValues)
                        {
                            StringBuilder stringBuilder = new StringBuilder();
                            foreach (JToken jtoken in RespJ["message"]["errors"])
                            {
                                string str = (string)jtoken;
                                stringBuilder.AppendLine($"- {(object)str}");
                            }

                            InstagramIssue instagramIssue = new InstagramIssue { Error = InstagramError.InputError };
                            string str1 = stringBuilder.ToString();
                            instagramIssue.Message = str1;
                            int num = 0;
                            instagramIssue.Health = (HealthState)num;
                            Issue = instagramIssue;
                        }
                        else
                        {
                            string ResponseMessage = string.Empty;
                            if (RespJ["message"] != null)
                                ResponseMessage = RespJ["message"].ToString();

                            string Message = String.Empty;
                            HealthState? Health = null;
                            InstagramError? Error = null;
                            string Status = string.Empty;
                            bool ChangeStatus = false;
                            string ApiPath = string.Empty;
                            string ChallangeApiPath = string.Empty;
                            if (ResponseMessage.Contains("challenge_required"))
                            {
                                ChallangeApiPath = RespJ["challenge"]["api_path"].ToString();
                            }
                            switch (ResponseMessage)
                            {
                                case "Thread does not exist":
                                    Error = InstagramError.NotFound;
                                    Message = DominatorHouseCore.Resources.Response_Response_Thread_not_found_;
                                    Health = HealthState.Green;
                                    break;
                                case "challenge_required":
                                    Error = InstagramError.Challenge;
                                    Message = DominatorHouseCore.Resources
                                        .Response_Response_Instagram_detected_suspicious_activity__you_need_to_verify_your_identity_;
                                    Health = HealthState.HotPink;
                                    Status = DominatorHouseCore.Resources.Response_Response_Suspicious_Activity;
                                    ChangeStatus = true;
                                    ApiPath = ChallangeApiPath;
                                    break;
                                case "Not authorized to view user":
                                    Error = InstagramError.NotAuthorized;
                                    Message = DominatorHouseCore.Resources
                                        .Response_Response_Not_authorized_to_perform_action__skipping___;
                                    Health = HealthState.Green;
                                    break;
                                case "login_required":
                                    Error = InstagramError.LoginRequired;
                                    Message = DominatorHouseCore.Resources
                                        .Response_Response_You_are_not_logged_in__please_log_in_again_;
                                    Health = HealthState.Green;
                                    ChangeStatus = true;
                                    Status = DominatorHouseCore.Resources.Response_Response_Login_session_expired;
                                    break;
                                case "User not found":
                                    Error = InstagramError.NotFound;
                                    Message = DominatorHouseCore.Resources.Response_Response_User_not_found_;
                                    Health = HealthState.Green;
                                    break;
                                case "Please wait a few minutes before you try again.":
                                    Error = InstagramError.RateLimit;
                                    Message = DominatorHouseCore.Resources
                                        .Response_Response_Too_many_requests_made__waiting_a_few_minutes_before_retrying_;
                                    Health = HealthState.Green;
                                    break;
                                case "feedback_required":
                                    Error = InstagramError.Feedback;
                                    //Message = GramDominatorCore.;
                                    Status = DominatorHouseCore.Resources.Response_Response_Action_Block;
                                    ChangeStatus = true;
                                    Health = HealthState.Green;
                                    break;
                                case "Sorry, you cannot like this media":
                                    Error = InstagramError.CanNotLike;
                                    Message = DominatorHouseCore.Resources.Response_Response_Can_t_like_this_picture_;
                                    Health = HealthState.Green;
                                    break;
                                case
                                    "To secure your account, we've reset your password. Tap \"Get help signing in\" on the login screen and follow the instructions to access your account."
                                    :
                                    Error = InstagramError.PasswordReset;
                                    Message = DominatorHouseCore.Resources
                                        .Response_Response_Your_password_has_been_reset__Surf_to_Instagram_for_instructions_on_getting_your_account_back_;
                                    Health = HealthState.Purple;
                                    ChangeStatus = true;
                                    Status = DominatorHouseCore.Resources.Response_Response_Password_Reset;
                                    break;
                                case "Media not found or unavailable":
                                    Error = InstagramError.NotFound;
                                    Message = DominatorHouseCore.Resources.Response_Response_Media_not_found_;
                                    Health = HealthState.Green;
                                    break;
                                case "Media is unavailable":
                                    Error = InstagramError.NotFound;
                                    Message = DominatorHouseCore.Resources.Response_Response_Media_not_found_;
                                    Health = HealthState.Green;
                                    break;
                                case "checkpoint_required":
                                    Error = InstagramError.CheckPoint;
                                    Message = DominatorHouseCore.Resources
                                        .Response_Response_Instagram_checkpoint__account_needs_mobile_verification_;
                                    Health = HealthState.Red;
                                    Status = DominatorHouseCore.Resources.Response_Response_Checkpoint;
                                    ChangeStatus = true;
                                    break;
                                case
                                    "The username you entered doesn't appear to belong to an account. Please check your username and try again."
                                    :
                                    Error = InstagramError.UsernameNotExist;
                                    //  Message = DominatorHouseCore.Resources.Response_Response_Can_t_login__the_username_doesn_t_exist_;
                                    Health = HealthState.PaleTurquoise;
                                    ChangeStatus = true;
                                    Status = DominatorHouseCore.Resources.Response_Response_Username_Doesn_t_Exist;
                                    break;
                                //case "The password you entered is incorrect. Please try again.":
                                case "The password that you've entered is incorrect. Please try again.":
                                    Error = InstagramError.IncorrectPassword;
                                    //  Message = DominatorHouseCore.Resources.Response_Response_Failed_to_login__please_correct_your_password_and_try_again_;
                                    Health = HealthState.Green;
                                    Status = DominatorHouseCore.Resources.Response_Response_Incorrect_Password;
                                    ChangeStatus = true;
                                    break;
                                case
                                    "Your account has been disabled for violating our terms. Learn how you may be able to restore your account."
                                    :
                                    Error = InstagramError.AccountDisabled;
                                    Message = DominatorHouseCore.Resources.Response_Response_Account_Disabled;
                                    Health = HealthState.Tomato;
                                    ChangeStatus = true;
                                    Status = DominatorHouseCore.Resources.Response_Response_Account_Disabled;
                                    break;
                                case
                                    "Sorry, you're following the max limit of accounts. You'll need to unfollow some accounts to start following more."
                                    :
                                    Error = InstagramError.maxLimit;
                                    // Message = DominatorHouseCore.Resources.you_Are_Following_The_Max_Limit_Of_Accounts;
                                    Health = HealthState.Tomato;
                                    break;
                                case "Uploaded image isn't in an allowed aspect ratio":
                                    Error = InstagramError.AspectRatio;
                                    Message = "Uploaded image isn't in an allowed aspect ratio";
                                    Health = HealthState.blue;
                                    break;
                                case
                                "Please check the code we sent you and try again.":
                                    Error = InstagramError.wrongSecurityCode;
                                    Message = "Please check the code we sent you and try again.";
                                    Health = HealthState.Yellow;
                                    ChangeStatus = true;
                                    //Status = DominatorHouseCore.Resources.Response_Response_Invalid_Security_Code;
                                    break;
                                case
                                    "The user has not liked this media":
                                    Error = InstagramError.NotLikedMedia;
                                    Message = "The user has not liked this media";
                                    Health = HealthState.Yellow;
                                    ChangeStatus = true;
                                    break;
                                case
                                    "Caption too long":
                                    Error = InstagramError.CaptionTooLong;
                                    Message = "Caption too long";
                                    Health = HealthState.Yellow;
                                    ChangeStatus = true;
                                    break;
                                case
                                    "there was a problem with your request":
                                    Error = InstagramError.SentryBlocked;
                                    Message = "Sentry Block";
                                    Health = HealthState.Yellow;
                                    ChangeStatus = true;
                                    break;
                                //There was an error with your request. Please try again          
                                case
                                    "Comments on this post have been limited":
                                    Error = InstagramError.CommentLimited;
                                    Message = "CommentLimited";
                                    Health = HealthState.Yellow;
                                    break;
                                case
                                    "user_has_logged_out":
                                    Error = InstagramError.UserLogOut;
                                    Message = "UserLogOut";
                                    Health = HealthState.Red;
                                    break;
                                case "could not delete":
                                    Error = InstagramError.CouldNotDeletePost;
                                    Message = "CouldNotDeletePost";
                                    Health = HealthState.Red;
                                    break;
                                case
                                    "There was an error with your request. Please try again.":
                                    Error = InstagramError.SentryBlocked;
                                    Message = "Sentry Block";
                                    Health = HealthState.Yellow;
                                    ChangeStatus = true;
                                    break;
                                default:
                                    if (RespJ["two_factor_required"] != null &&
                                        Convert.ToBoolean(RespJ["two_factor_required"]))
                                    {
                                        Error = InstagramError.wrongSecurityCode;
                                        Message = DominatorHouseCore.Resources
                                            .Response_Response_Two_Factor_login__Right_click_your_account_and_enter_auth_code_;
                                        Status = DominatorHouseCore.Resources.Response_Response_Two_Factor_login;
                                        ChangeStatus = true;
                                        Health = HealthState.SteelBlue;
                                    }
                                    else if (RespJ["error_type"] != null && RespJ["error_type"].ToString() == "sentry_block")
                                    {
                                        Error = InstagramError.SentryBlocked;
                                        Message = DominatorHouseCore.Resources
                                            .Response_Response_Your_account_has_been_sentry_blocked__Your_IP_account_has_been_flagged__It_is_advised_to_stop_botting_;
                                        Health = HealthState.Salmon;
                                        ChangeStatus = true;
                                        Status = DominatorHouseCore.Resources.Response_Response_Sentry_Blocked;
                                    }
                                    else if (RespJ["challengeType"] != null && RespJ["challengeType"].ToString() == "SubmitPhoneNumberForm")
                                    {
                                        Error = InstagramError.SubmitPhoneNumber;
                                        Message = "Submit Phone Number After Resolving Captcha";
                                        Health = HealthState.Yellow;
                                        ChangeStatus = true;
                                        Status = "ok";
                                        ApiPath = RespJ["challenge_context"].ToString();
                                    }
                                    else if (RespJ["challengeType"] != null && RespJ["challengeType"].ToString() == "VerifySMSCodeFormForSMSCaptcha")
                                    {
                                        Error = InstagramError.SubmitPhoneNumber;
                                        Message = "Submit Phone Number After Resolving Captcha";
                                        Health = HealthState.Yellow;
                                        ChangeStatus = true;
                                        Status = "ok";
                                        ApiPath = RespJ["challenge_context"].ToString();
                                    }
                                    else if (RespJ["challengeType"] != null && RespJ["challengeType"].ToString() == "UFACBlockingForm")
                                    {
                                        Error = InstagramError.AccountCompromised;
                                        Message = "we'll review your info and if we can confirm it,you'll be able to access your account within approximately 24 hours";
                                        Health = HealthState.Red;
                                        ChangeStatus = true;
                                        //Status = DominatorHouseCore.Resources.Response_Response_Compromised_Account; ;
                                    }
                                    break;
                            }
                            if (Error != null)
                            {
                                Issue = new InstagramIssue()
                                {
                                    Error = Error,
                                    Message = Message,
                                    Status = Status,
                                    ChangeStatus = ChangeStatus,
                                    Health = Health,
                                    ApiPath = ApiPath
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public InstagramIssue Issue { get; set; }

        public bool Success { get; set; }
        public override string ToString()
        {
            if (!string.IsNullOrEmpty(response.Response))
                return response.Response;
            return string.Empty;
        }
    }

}
