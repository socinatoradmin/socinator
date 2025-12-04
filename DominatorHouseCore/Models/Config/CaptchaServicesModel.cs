#region

using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models.Config
{
    [ProtoContract]
    public class CaptchaServicesModel : BindableBase
    {
        private bool _isTwoCaptcha;

        [ProtoMember(1)]
        public bool IsTwoCaptcha
        {
            get => _isTwoCaptcha;
            set
            {
                if (_isTwoCaptcha == value)
                    return;
                SetProperty(ref _isTwoCaptcha, value);
            }
        }

        private string _twoCaptchaApiKey;

        [ProtoMember(2)]
        public string TwoCaptchaApiKey
        {
            get => _twoCaptchaApiKey;
            set
            {
                if (value == _twoCaptchaApiKey)
                    return;
                SetProperty(ref _twoCaptchaApiKey, value);
            }
        }

        private bool _isAntiCaptcha;

        [ProtoMember(3)]
        public bool IsAntiCaptcha
        {
            get => _isAntiCaptcha;
            set
            {
                if (_isAntiCaptcha == value)
                    return;
                SetProperty(ref _isAntiCaptcha, value);
            }
        }

        private string _antiCaptchaApiKey;

        [ProtoMember(4)]
        public string AntiCaptchaApiKey
        {
            get => _antiCaptchaApiKey;
            set
            {
                if (value == _antiCaptchaApiKey)
                    return;
                SetProperty(ref _antiCaptchaApiKey, value);
            }
        }
    }
}