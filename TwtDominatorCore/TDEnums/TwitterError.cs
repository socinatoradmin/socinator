namespace TwtDominatorCore.TDEnums
{
    public enum TwitterError
    {
        FailedRequest,
        ToManyAction,
        RateLimitExceeded = 88,
        IncorrectPassword,
        NotAuthorized,
        PageNotFound = 404,
        LoginRequired,
        Challenge_PasswordReset = 235,
        AccountDisabled,
        Challenge_AccountSuspended = 64,
        passwordChanged = 32,
        TooManyRequests = 429,
        Challenge_VerifyPhoneNumber,
        Challenge_VerifyEmail,
        Challenge_RetypePhoneNumber,
        Challenge_RetypeEmail,
        Challenge_RetypeUserName,
        Challenge_Captcha,
        Add_phonenumber
    }
}