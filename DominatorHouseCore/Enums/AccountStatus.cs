#region

using System.ComponentModel;

#endregion

namespace DominatorHouseCore.Enums
{
    public enum AccountStatus
    {
        //New Comment
        [Description("LangKeySuccess")] Success,
        [Description("LangKeyFailed")] Failed,

        [Description("LangKeyInvalidCredentials")]
        InvalidCredentials,

        [Description("LangKeyProxyNotWorking")]
        ProxyNotWorking,

        [Description("LangKeyEmailVerification")]
        EmailVerification,

        [Description("LangKeyPhoneVerification")]
        PhoneVerification,

        [Description("LangKeyNeedsVerification")]
        NeedsVerification,

        [Description("LangKeyPermanentlyBlocked")]
        PermanentlyBlocked,

        [Description("LangKeyTemporarilyBlocked")]
        TemporarilyBlocked,
        [Description("LangKeyNotChecked")] NotChecked,
        [Description("LangKeyTryingToLogin")] TryingToLogin,

        [Description("LangKeyAddPhoneNumberToYourAccount")]
        AddPhoneNumberToYourAccount,

        [Description("LangKeyTooManyAttemptsOnPhoneVerification")]
        TooManyAttemptsOnPhoneVerification,
        [Description("LangKeyRetypeEmail")] ReTypeEmail,

        [Description("LangKeyRetypePhoneNumber")]
        ReTypePhoneNumber,

        [Description("LangKeyProfileSuspended")]
        ProfileSuspended,

        [Description("LangKeyTwoFactorLoginAttempt")]
        TwoFactorLoginAttempt,

        [Description("LangKeyTooManyAttemptsOnSignIn")]
        TooManyAttemptsOnSignIn,
        [Description("LangKeySetNewPassword")] SetNewPassword,
        [Description("LangKeyActionBlock")] ActionBlocked,
        [Description("LangKeyDisableAccount")] DisableAccount,

        [Description("LangKeyInvalidSecurityCode")]
        InvalidSecurityCode,

        [Description("LangKeyTooManyAttemptWithSameNumber")]
        TooManyAttemptsWithTheSamePhoneNumber,

        [Description("LangKeyBrowserNotsecure")]
        BrowerNotSecure,

        [Description("LangKeyAccountDisabled")]
        AccountDisabled,

        [Description("LangKeyAccountNotRegisteredYet")]
        NotRegisteredYet,

        [Description("LangKeyUpdatingDetails")]
        UpdatingDetails,
        [Description("LangKeyCaptchFound")]
        FoundCaptcha,
        [Description("LangKeyAddPhoneAndVerify")]
        AddPhoneAndVerify,

        [Description("LangKeyAccountCompromised")]
        AccountCompromised,

        [Description("LangKeyCheckProxy")]
        CheckProxy,
    }

    public enum PinterestAccountType
    {
        [Description("LangKeyNormalMode")] Inactive,
        [Description("LangKeyNotAvailable")] NotAvailable,
        [Description("LangKeyBusinessMode")] Active
    }
}