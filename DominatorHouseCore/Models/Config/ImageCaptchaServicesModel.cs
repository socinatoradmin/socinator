#region

using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models.Config
{
    [ProtoContract]
    public class ImageCaptchaServicesModel : BindableBase
    {
        private string _userName;

        [ProtoMember(1)]
        public string UserName
        {
            get => _userName;
            set => SetProperty(ref _userName, value);
        }

        private string _password;

        [ProtoMember(2)]
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        private string _token;

        [ProtoMember(3)]
        public string Token
        {
            get => _token;
            set => SetProperty(ref _token, value);
        }
    }
}