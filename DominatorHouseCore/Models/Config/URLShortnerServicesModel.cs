#region

using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models.Config
{
    [ProtoContract]
    public class UrlShortnerServicesModel : BindableBase
    {
        private bool _isBitly;

        [ProtoMember(1)]
        public bool IsBitly
        {
            get => _isBitly;
            set
            {
                if (_isBitly == value)
                    return;
                SetProperty(ref _isBitly, value);
            }
        }

        private string _login;

        [ProtoMember(2)]
        public string Login
        {
            get => _login;
            set
            {
                if (value == _login)
                    return;
                SetProperty(ref _login, value);
            }
        }

        private string _apiKey;

        [ProtoMember(3)]
        public string ApiKey
        {
            get => _apiKey;
            set
            {
                if (value == _apiKey)
                    return;
                SetProperty(ref _apiKey, value);
            }
        }
    }
}