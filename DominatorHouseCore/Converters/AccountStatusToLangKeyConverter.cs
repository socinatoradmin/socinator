using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using System;
using System.Globalization;
using System.Windows.Data;

namespace DominatorHouseCore.Converters
{
    public class AccountStatusToLangKeyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is AccountStatus status)
            {
                var text = string.Empty;
                switch (status)
                {
                    case AccountStatus.InvalidCredentials:
                        text = "LangKeyInvalidCredentials";
                        break;
                    case AccountStatus.TryingToLogin:
                        text = "LangKeyTryingToLogin";
                        break;
                    case AccountStatus.Success:
                        text = "LangKeySuccess";
                        break;
                    case AccountStatus.Failed:
                        text = "LangKeyFailed";
                        break;
                    case AccountStatus.NeedsVerification:
                        text = "LangKeyNeedsVerification";
                        break;
                    case AccountStatus.EmailVerification:
                        text = "LangKeyEmailVerification";
                        break;
                    case AccountStatus.PhoneVerification:
                        text = "LangKeyPhoneVerification";
                        break;
                    case AccountStatus.NotChecked:
                        text = "LangKeyNotChecked";
                        break;
                    case AccountStatus.ProxyNotWorking:
                        text = "LangKeyProxyNotWorking";
                        break;
                    case AccountStatus.PermanentlyBlocked:
                        text = "LangKeyPermanentlyBlocked";
                        break;
                    case AccountStatus.TemporarilyBlocked:
                        text = "LangKeyTemporarilyBlocked";
                        break;
                    case AccountStatus.AddPhoneNumberToYourAccount:
                        text = "LangKeyAddPhoneNumberToYourAccount";
                        break;
                    case AccountStatus.TooManyAttemptsOnPhoneVerification:
                        text = "LangKeyTooManyAttemptsOnPhoneVerification";
                        break;
                    case AccountStatus.ReTypeEmail:
                        text = "LangKeyRetypeEmail";
                        break;
                    case AccountStatus.ReTypePhoneNumber:
                        text = "LangKeyRetypePhoneNumber";
                        break;
                    case AccountStatus.ProfileSuspended:
                        text = "LangKeyProfileSuspended";
                        break;
                    case AccountStatus.TwoFactorLoginAttempt:
                        text = "LangKeyTwoFactorLoginAttempt";
                        break;
                    case AccountStatus.TooManyAttemptsOnSignIn:
                        text = "LangKeyTooManyAttemptsOnSignIn";
                        break;
                    case AccountStatus.SetNewPassword:
                        text = "LangKeySetNewPassword";
                        break;
                    case AccountStatus.ActionBlocked:
                        text = "LangKeyActionBlock";
                        break;
                    case AccountStatus.DisableAccount:
                        text = "LangKeyDisableAccount";
                        break;
                    case AccountStatus.BrowerNotSecure:
                        text = "LangKeyBrowserNotsecure";
                        break;
                    case AccountStatus.AccountDisabled:
                        text = "LangKeyAccountDisabled";
                        break;
                    case AccountStatus.InvalidSecurityCode:
                        text = "LangKeyInvalidSecurityCode";
                        break;
                    case AccountStatus.TooManyAttemptsWithTheSamePhoneNumber:
                        text = "LangKeyTooManyAttemptWithSameNumber";
                        break;
                    case AccountStatus.NotRegisteredYet:
                        text = "LangKeyAccountNotRegisteredYet";
                        break;
                    case AccountStatus.UpdatingDetails:
                        text = "LangKeyUpdatingDetails";
                        break;
                    case AccountStatus.FoundCaptcha:
                        text = "LangKeyCaptchFound";
                        break;
                    case AccountStatus.AddPhoneAndVerify:
                        text = "LangKeyAddPhoneAndVerify";
                        break;
                    case AccountStatus.AccountCompromised:
                        text = "LangKeyAccountCompromised";
                        break;
                    case AccountStatus.CheckProxy:
                        text = "LangKeyCheckProxy";
                        break;
                    default:
                        text = status.ToString();
                        break;
                }
                return !string.IsNullOrEmpty(text)
                    ? text.FromResourceDictionary() : value?.ToString()  ?? string.Empty;
            }
            return value?.ToString() ?? "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
