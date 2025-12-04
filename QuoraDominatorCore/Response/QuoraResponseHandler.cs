using System;
using System.Net;
using System.Text;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Requests;
using DominatorHouseCore.Utility;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using QuoraDominatorCore.Enums;
using QuoraDominatorCore.Models;
using QuoraDominatorCore.QdUtility;

namespace QuoraDominatorCore.Response
{
    public class QuoraResponseHandler
    {
        protected readonly JObject RespJ;
        public readonly IResponseParameter Response;
        protected HtmlDocument HtmlDocument;
        public readonly JsonJArrayHandler jsonHandler = JsonJArrayHandler.GetInstance;
        public QuoraResponseHandler(IResponseParameter response)
        {
            Response = response;
            try
            {
                if (response.HasError)
                {
                    WebHelper.WebExceptionIssue errorMsgWebrequest;
                    try
                    {
                        errorMsgWebrequest = ((WebException)response.Exception).GetErrorMsgWebrequest();
                    }
                    catch (Exception ex)
                    {
                        errorMsgWebrequest = new WebHelper.WebExceptionIssue
                        {
                            MessageLong = response.Exception.Message
                        };
                        ex.DebugLog();
                    }

                    Success = false;
                    Issue = new QuoraIssue
                    {
                        Message = errorMsgWebrequest.MessageLong,
                        Error = QuoraError.FailedRequest
                    };
                }
                else
                {
                    if (string.IsNullOrEmpty(response.Response))
                    {
                        Success = false;
                        return;
                    }

                    if (!response.Response.IsValidJson())
                    {
                        Success = true;
                        HtmlDocument = new HtmlDocument();
                        HtmlDocument.LoadHtml(response.Response);
                    }else if(response.Response.IsValidJson())
                        Success = true;
                    else
                    {
                        RespJ = jsonHandler.ParseJsonToJObject(response.Response);
                        var value = jsonHandler.GetJTokenValue(RespJ, "value");
                        if (!string.IsNullOrEmpty(value) && !value.Contains("exception"))
                        {
                            Success = true;
                        }
                        else
                        {
                            Success = false;
                            if (RespJ["exception"].HasValues)
                            {
                                var stringBuilder = new StringBuilder();
                                foreach (var jtoken in RespJ["exception"]["type"])
                                {
                                    var str = (string)jtoken;
                                    stringBuilder.AppendLine($"- {(object)str}");
                                }

                                var quoraIssue = new QuoraIssue { Error = QuoraError.InputError };
                                var str1 = stringBuilder.ToString();
                                quoraIssue.Message = str1;
                                var num = 0;
                                quoraIssue.Health = (HealthState)num;
                                Issue = quoraIssue;
                            }
                            else
                            {
                                var responseMessage = RespJ["exception"].ToString();
                                var message = string.Empty;
                                HealthState? health = null;
                                QuoraError? error = null;
                                var status = string.Empty;
                                var changeStatus = false;
                                switch (responseMessage)
                                {
                                    case "Thread does not exist":
                                        error = QuoraError.NotFound;
                                        //message = Resources.Response_Response_Thread_not_found_;
                                        health = HealthState.Green;
                                        break;
                                    case "challenge_required":
                                        error = QuoraError.Challenge;
                                        //message = Resources
                                        //    .Response_Response_Instagram_detected_suspicious_activity__you_need_to_verify_your_identity_;
                                        health = HealthState.HotPink;
                                        //status = Resources.Response_Response_Suspicious_Activity;
                                        changeStatus = true;
                                        break;
                                    case "Not authorized to view user":
                                        error = QuoraError.NotAuthorized;
                                        //message = Resources
                                        //    .Response_Response_Not_authorized_to_perform_action__skipping___;
                                        health = HealthState.Green;
                                        break;
                                    case "login_required":
                                        error = QuoraError.LoginRequired;
                                        //message = Resources.Response_Response_You_are_not_logged_in__please_log_in_again_;
                                        health = HealthState.Green;
                                        changeStatus = true;
                                       // status = Resources.Response_Response_Login_session_expired;
                                        break;
                                    case "User not found":
                                        error = QuoraError.NotFound;
                                       // message = Resources.Response_Response_User_not_found_;
                                        health = HealthState.Green;
                                        break;
                                    case "Please wait a few minutes before you try again.":
                                        error = QuoraError.RateLimit;
                                       // message = Resources
                                        //    .Response_Response_Too_many_requests_made__waiting_a_few_minutes_before_retrying_;
                                        health = HealthState.Green;
                                        break;
                                    case "feedback_required":
                                        error = QuoraError.Feedback;
                                        //Message = GramDominatorCore.;
                                        //status = Resources.Response_Response_Action_Block;
                                        changeStatus = true;
                                        health = HealthState.Green;
                                        break;
                                    case "Sorry, you cannot like this media":
                                        error = QuoraError.CanNotLike;
                                       // message = Resources.Response_Response_Can_t_like_this_picture_;
                                        health = HealthState.Green;
                                        break;
                                    case
                                        "To secure your account, we've reset your password. Tap \"Get help signing in\" on the login screen and follow the instructions to access your account."
                                        :
                                        error = QuoraError.PasswordReset;
                                       // message = Resources
                                       //     .Response_Response_Your_password_has_been_reset__Surf_to_Instagram_for_instructions_on_getting_your_account_back_;
                                        health = HealthState.Purple;
                                        changeStatus = true;
                                        //status = Resources.Response_Response_Password_Reset;
                                        break;
                                    case "Media is unavailable":
                                        error = QuoraError.NotFound;
                                        //message = Resources.Response_Response_Media_not_found_;
                                        health = HealthState.Green;
                                        break;
                                    case "checkpoint_required":
                                        error = QuoraError.CheckPoint;
                                        //message = Resources
                                        //    .Response_Response_Instagram_checkpoint__account_needs_mobile_verification_;
                                        health = HealthState.Red;
                                        //status = Resources.Response_Response_Checkpoint;
                                        changeStatus = true;
                                        break;
                                    case
                                        "The username you entered doesn't appear to belong to an account. Please check your username and try again."
                                        :
                                        error = QuoraError.UsernameNotExist;
                                        //  Message = DominatorHouseCore.Resources.Response_Response_Can_t_login__the_username_doesn_t_exist_;
                                        health = HealthState.PaleTurquoise;
                                        changeStatus = true;
                                        //status = Resources.Response_Response_Username_Doesn_t_Exist;
                                        break;
                                    case "The password you entered is incorrect. Please try again.":
                                        error = QuoraError.IncorrectPassword;
                                        //  Message = DominatorHouseCore.Resources.Response_Response_Failed_to_login__please_correct_your_password_and_try_again_;
                                        health = HealthState.Green;
                                       // status = Resources.Response_Response_Incorrect_Password;
                                        changeStatus = true;
                                        break;
                                    case
                                        "Your account has been disabled for violating our terms. Learn how you may be able to restore your account."
                                        :
                                        error = QuoraError.AccountDisabled;
                                        //Message = DominatorHouseCore.Resources.Response_Response_Your_account_has_been_disabled_for_violating_Instagrams_terms__Contact_Instagram_to_learn_how_to_restore_your_account_;
                                        health = HealthState.Tomato;
                                        changeStatus = true;
                                       // status = Resources.Response_Response_Account_Disabled;
                                        break;
                                }

                                if (error != null)
                                    Issue = new QuoraIssue
                                    {
                                        Error = error,
                                        Message = message,
                                        Status = status,
                                        ChangeStatus = changeStatus,
                                        Health = health
                                    };
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public QuoraIssue Issue { get; }

        public bool Success { get; set; }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Response.Response))
                return Response.Response;
            return string.Empty;
        }
    }
}