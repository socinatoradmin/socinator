using DominatorHouseCore.Interfaces;
using TwtDominatorCore.TDEnums;
using TwtDominatorCore.TDModels;

namespace TwtDominatorCore.Response
{
    public class TdBaseHtmlResponseHandler : TdResponseHandler
    {
        public TdBaseHtmlResponseHandler()
        {
        }

        public TdBaseHtmlResponseHandler(IResponseParameter response) : base(response)
        {
            if (!Success)
                return;
            if (response.Response.Contains("<!DOCTYPE html>"))
            {
                if (response.Response.Contains("The username and password you entered did not match our records") ||
                    response.Response.Contains("The email and password you entered did not match our records"))
                {
                    Issue = new TwitterIssue
                    {
                        Message = "Password didn't match",
                        Error = TwitterError.IncorrectPassword
                    };
                    Success = false;
                    return;
                }

                if (response.Response.Contains("name=\"discoverable_by_mobile_phone\"") &&
                    response.Response.Contains("input id=\"phone_number\""))
                {
                    Issue = new TwitterIssue
                    {
                        Message = "Add a phone number",
                        Error = TwitterError.Add_phonenumber
                    };
                    Success = false;
                    return;
                }

                if (response.Response.Contains("errorpage-body-content"))
                {
                    Issue = new TwitterIssue
                    {
                        Message = "Page doesn't exist",
                        Error = (TwitterError) 404
                    };
                    Success = false;
                    return;
                }

                if (response.Response.Contains("access_password_reset?current_user_only=true") ||
                    response.Response.Contains("Password change required"))
                {
                    Issue = new TwitterIssue
                    {
                        Message = "Password change required",
                        Error = TwitterError.Challenge_PasswordReset
                    };
                    Success = false;
                    return;
                }

                if (response.Response.Contains("Verify your identity by entering the phone number") ||
                    response.Response.Contains("challenge_type=RetypePhoneNumber"))
                {
                    Issue = new TwitterIssue
                    {
                        Message = "Verify your mobile number",
                        Error = TwitterError.Challenge_RetypePhoneNumber
                    };
                    Success = false;
                    return;
                }

                if (response.Response.Contains("challenge_type=RetypeScreenName"))
                {
                    Issue = new TwitterIssue
                    {
                        Message = "Retype User name",
                        Error = TwitterError.Challenge_RetypeUserName
                    };
                    Success = false;
                    return;
                }

                if (response.Response.Contains("challenge_type=RetypeEmail"))
                {
                    Issue = new TwitterIssue
                    {
                        Message = "Retype Email",
                        Error = TwitterError.Challenge_RetypeEmail
                    };
                    Success = false;
                    return;
                }


                if (response.Response.Contains("account-suspended") &&
                    response.Response.Contains("/account/suspended_help"))
                {
                    Issue = new TwitterIssue
                    {
                        Message = "Account suspended",
                        Error = TwitterError.Challenge_AccountSuspended
                    };
                    Success = false;
                    return;
                }

                if (response.Response.Contains("challenge_type=TemporaryPassword"))
                {
                    Issue = new TwitterIssue
                    {
                        Message = "Verify Your Email",
                        Error = TwitterError.Challenge_VerifyEmail
                    };
                    Success = false;
                }

                if (response.Response.Contains("reCAPTCHA challenge") ||
                    response.Response.Contains("recaptcha_element"))
                {
                    Issue = new TwitterIssue
                    {
                        Message = "reCAPTCHA challenge",
                        Error = TwitterError.Challenge_Captcha
                    };
                    Success = false;
                }
            }
        }
    }
}