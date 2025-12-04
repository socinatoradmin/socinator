using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using System;
using TwtDominatorCore.TDEnums;
using TwtDominatorCore.TDModels;

namespace TwtDominatorCore.Response
{
    public class LogInResponseHandler : TdBaseHtmlResponseHandler
    {
        public AccountStatus DominatorStatus;
        public string Response = string.Empty;
        public Enums.LoginStatus status;

        public LogInResponseHandler()

        {
        }

        public LogInResponseHandler(IResponseParameter response) : base(response)
        {
            Response = response?.Response;
            try
            {
                var jsonHandler = new JsonHandler(Response);
                var secondaryText = jsonHandler.GetElementValue("subtasks", 0, "enter_text", "header", "secondary_text", "text");
                secondaryText = string.IsNullOrEmpty(secondaryText) ? jsonHandler.GetElementValue("subtasks", 0, "cta", "secondary_text", "text") : secondaryText;
                if (!string.IsNullOrEmpty(secondaryText) && secondaryText.Contains("Verify your identity by entering the phone number associated with your Twitter account"))
                {
                    Success = false;
                    if(Issue == null)
                    {
                        Issue = new TwitterIssue
                        {
                            Message = secondaryText,
                            Error = TwitterError.Challenge_VerifyPhoneNumber
                        };
                    }
                    else
                        Issue.Error = TwitterError.Challenge_VerifyPhoneNumber;
                }
                else if(!string.IsNullOrEmpty(secondaryText) && (secondaryText.Contains("Verify your identity by entering the email address associated with your Twitter account")
                    || secondaryText.Contains("In order to protect your account from suspicious activity, we've sent a confirmation code to")))
                {
                    Success = false;
                    if (Issue == null)
                    {
                        Issue = new TwitterIssue
                        {
                            Message = secondaryText,
                            Error = TwitterError.Challenge_VerifyEmail
                        };
                    }
                    else
                        Issue.Error = TwitterError.Challenge_VerifyEmail;
                }else if(!string.IsNullOrEmpty(secondaryText) && secondaryText.Contains("We blocked an attempt to access your account because we weren't sure it was really you"))
                {
                    Success = false;
                    if (Issue == null)
                    {
                        Issue = new TwitterIssue
                        {
                            Message = secondaryText,
                            Error = TwitterError.TooManyRequests
                        };
                    }
                    else
                        Issue.Error = TwitterError.TooManyRequests;
                }
            }
            catch (Exception)
            {
                                
            }
            if (!Success)
                switch (Issue?.Error)
                {
                    case TwitterError.IncorrectPassword:
                        status = Enums.LoginStatus.InvalidCredentials;
                        DominatorStatus = AccountStatus.InvalidCredentials;
                        return;
                    case TwitterError.Challenge_AccountSuspended:
                        status = Enums.LoginStatus.AccountSuspended;
                        DominatorStatus = AccountStatus.PermanentlyBlocked;
                        return;
                    //case TwitterError.Challenge_PasswordReset:
                    //    status = Enums.LoginStatus.ResetPassword;
                    //    DominatorStatus = AccountStatus.ResetPassword;
                    //    return;
                    case TwitterError.Challenge_VerifyPhoneNumber:
                        status = Enums.LoginStatus.VerifyPhoneNumber;
                        DominatorStatus = AccountStatus.PhoneVerification;
                        return;
                    case TwitterError.Challenge_VerifyEmail:
                        status = Enums.LoginStatus.VerifyEmail;
                        DominatorStatus = AccountStatus.EmailVerification;
                        return;
                    case TwitterError.Challenge_RetypeEmail:
                        status = Enums.LoginStatus.RetypeEmail;
                        DominatorStatus = AccountStatus.ReTypeEmail;
                        return;
                    case TwitterError.Challenge_RetypePhoneNumber:
                        status = Enums.LoginStatus.RetypePhoneNumber;
                        DominatorStatus = AccountStatus.PhoneVerification;
                        return;
                    case TwitterError.Challenge_RetypeUserName:
                        status = Enums.LoginStatus.RetypeUserName;
                        DominatorStatus = AccountStatus.InvalidCredentials;
                        return;
                    case TwitterError.Challenge_Captcha:
                        status = Enums.LoginStatus.Captcha;
                        DominatorStatus = AccountStatus.Failed;
                        return;
                    case TwitterError.Add_phonenumber:
                        status = Enums.LoginStatus.Add_Phonenumber;
                        DominatorStatus = AccountStatus.AddPhoneNumberToYourAccount;
                        return;
                    case TwitterError.TooManyRequests:
                        status = Enums.LoginStatus.Failed;
                        DominatorStatus = AccountStatus.TemporarilyBlocked;
                        return;
                }

            if (!string.IsNullOrEmpty(response?.Response) &&
                (response.Response.Contains("signout") && response.Response.Contains("timeline-tweet-box") ||
                 response.Response.Contains("\"screen_name\":\"")
                 || response.Response.Contains("\"subtasks\":[]")))
            {
                status = Enums.LoginStatus.Success;
                DominatorStatus = AccountStatus.Success;
            }
            else
            {
                Issue = new TwitterIssue
                {
                    Message = "Password didn't match or Cookie missing"
                };
                status = Enums.LoginStatus.Failed;
                DominatorStatus = AccountStatus.Failed;
                Success = false;
            }
        }
    }
}