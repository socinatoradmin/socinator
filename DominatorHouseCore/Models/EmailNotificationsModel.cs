#region

using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models
{
    [ProtoContract]
    public class EmailNotificationsModel : BindableBase
    {
        private bool _isEnableEmailNotificationsChecked;

        [ProtoMember(1)]
        public bool IsEnableEmailNotificationsChecked
        {
            get => _isEnableEmailNotificationsChecked;
            set
            {
                if (value == _isEnableEmailNotificationsChecked)
                    return;
                SetProperty(ref _isEnableEmailNotificationsChecked, value);
            }
        }

        private string _email;

        [ProtoMember(2)]
        public string Email
        {
            get => _email;
            set
            {
                if (value == _email)
                    return;
                SetProperty(ref _email, value);
            }
        }

        private int _emailRecieveTime;

        [ProtoMember(3)]
        public int EmailRecieveTime
        {
            get => _emailRecieveTime;
            set
            {
                if (value == _emailRecieveTime)
                    return;
                SetProperty(ref _emailRecieveTime, value);
            }
        }

        private bool _isNotifyAboutInvalidProxiesChecked;

        [ProtoMember(4)]
        public bool IsNotifyAboutInvalidProxiesChecked
        {
            get => _isNotifyAboutInvalidProxiesChecked;
            set
            {
                if (value == _isNotifyAboutInvalidProxiesChecked)
                    return;
                SetProperty(ref _isNotifyAboutInvalidProxiesChecked, value);
            }
        }

        private bool _isNotifyAboutInvalidSocialProfilesChecked;

        [ProtoMember(5)]
        public bool IsNotifyAboutInvalidSocialProfilesChecked
        {
            get => _isNotifyAboutInvalidSocialProfilesChecked;
            set
            {
                if (value == _isNotifyAboutInvalidSocialProfilesChecked)
                    return;
                SetProperty(ref _isNotifyAboutInvalidSocialProfilesChecked, value);
            }
        }

        private bool _isEmailConfirmationChecked;

        [ProtoMember(6)]
        public bool IsEmailConfirmationChecked
        {
            get => _isEmailConfirmationChecked;
            set
            {
                if (value == _isEmailConfirmationChecked)
                    return;
                SetProperty(ref _isEmailConfirmationChecked, value);
            }
        }

        private bool _isBlockedChecked;

        [ProtoMember(7)]
        public bool IsBlockedChecked
        {
            get => _isBlockedChecked;
            set
            {
                if (value == _isBlockedChecked)
                    return;
                SetProperty(ref _isBlockedChecked, value);
            }
        }

        private bool _isActionRequiredChecked;

        [ProtoMember(8)]
        public bool IsActionRequiredChecked
        {
            get => _isActionRequiredChecked;
            set
            {
                if (value == _isActionRequiredChecked)
                    return;
                SetProperty(ref _isActionRequiredChecked, value);
            }
        }

        private bool _isAccountDisabledChecked;

        [ProtoMember(9)]
        public bool IsAccountDisabledChecked
        {
            get => _isAccountDisabledChecked;
            set
            {
                if (value == _isAccountDisabledChecked)
                    return;
                SetProperty(ref _isAccountDisabledChecked, value);
            }
        }

        private bool _isSuspendedChecked;

        [ProtoMember(10)]
        public bool IsSuspendedChecked
        {
            get => _isSuspendedChecked;
            set
            {
                if (value == _isSuspendedChecked)
                    return;
                SetProperty(ref _isSuspendedChecked, value);
            }
        }

        private bool _isNoInternetConnectionChecked;

        [ProtoMember(11)]
        public bool IsNoInternetConnectionChecked
        {
            get => _isNoInternetConnectionChecked;
            set
            {
                if (value == _isNoInternetConnectionChecked)
                    return;
                SetProperty(ref _isNoInternetConnectionChecked, value);
            }
        }

        private bool _isPasswordResetChecked;

        [ProtoMember(12)]
        public bool IsPasswordResetChecked
        {
            get => _isPasswordResetChecked;
            set
            {
                if (value == _isPasswordResetChecked)
                    return;
                SetProperty(ref _isPasswordResetChecked, value);
            }
        }

        private bool _isCaptchaChecked;

        [ProtoMember(13)]
        public bool IsCaptchaChecked
        {
            get => _isCaptchaChecked;
            set
            {
                if (value == _isCaptchaChecked)
                    return;
                SetProperty(ref _isCaptchaChecked, value);
            }
        }

        private bool _isInvalidCredentialsChecked;

        [ProtoMember(14)]
        public bool IsInvalidCredentialsChecked
        {
            get => _isInvalidCredentialsChecked;
            set
            {
                if (value == _isInvalidCredentialsChecked)
                    return;
                SetProperty(ref _isInvalidCredentialsChecked, value);
            }
        }

        private bool _isPhoneValidationChecked;

        [ProtoMember(15)]
        public bool IsPhoneValidationChecked
        {
            get => _isPhoneValidationChecked;
            set
            {
                if (value == _isPhoneValidationChecked)
                    return;
                SetProperty(ref _isPhoneValidationChecked, value);
            }
        }

        private bool _isTwoFactorAuthenticationChecked;

        [ProtoMember(16)]
        public bool IsTwoFactorAuthenticationChecked
        {
            get => _isTwoFactorAuthenticationChecked;
            set
            {
                if (value == _isTwoFactorAuthenticationChecked)
                    return;
                SetProperty(ref _isTwoFactorAuthenticationChecked, value);
            }
        }

        private bool _isNotifyAboutInvalidCampaignChecked;

        [ProtoMember(17)]
        public bool IsNotifyAboutInvalidCampaignChecked
        {
            get => _isNotifyAboutInvalidCampaignChecked;
            set
            {
                if (value == _isNotifyAboutInvalidCampaignChecked)
                    return;
                SetProperty(ref _isNotifyAboutInvalidCampaignChecked, value);
            }
        }

        private bool _isNotifyAboutInvalidFailedPostsChecked;

        [ProtoMember(18)]
        public bool IsNotifyAboutInvalidFailedPostsChecked
        {
            get => _isNotifyAboutInvalidFailedPostsChecked;
            set
            {
                if (value == _isNotifyAboutInvalidFailedPostsChecked)
                    return;
                SetProperty(ref _isNotifyAboutInvalidFailedPostsChecked, value);
            }
        }

        private bool _isNotifyNoMoreResultFoundChecked;

        [ProtoMember(19)]
        public bool IsNotifyNoMoreResultFoundChecked
        {
            get => _isNotifyNoMoreResultFoundChecked;
            set
            {
                if (value == _isNotifyNoMoreResultFoundChecked)
                    return;
                SetProperty(ref _isNotifyNoMoreResultFoundChecked, value);
            }
        }

        private bool _isAlwaysSendEmailChecked;

        [ProtoMember(20)]
        public bool IsAlwaysSendEmailChecked
        {
            get => _isAlwaysSendEmailChecked;
            set
            {
                if (value == _isAlwaysSendEmailChecked)
                    return;
                SetProperty(ref _isAlwaysSendEmailChecked, value);
            }
        }
    }
}