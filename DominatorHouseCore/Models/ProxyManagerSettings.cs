#region

using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models
{
    [ProtoContract]
    public class ProxyManagerSettings : BindableBase
    {
        private bool _isNumOfAccountPerProxy;

        [ProtoMember(1)]
        public bool IsNumOfAccountPerProxy
        {
            get => _isNumOfAccountPerProxy;
            set => SetProperty(ref _isNumOfAccountPerProxy, value);
        }

        private int _numOfAccountPerProxy = 1;

        [ProtoMember(2)]
        public int NumOfAccountPerProxy
        {
            get => _numOfAccountPerProxy;
            set => SetProperty(ref _numOfAccountPerProxy, value);
        }

        private bool _dontLogin;

        [ProtoMember(3)]
        public bool DontLogin
        {
            get => _dontLogin;
            set => SetProperty(ref _dontLogin, value);
        }
        private bool _isShowByGroup;
        [ProtoMember(4)]
        public bool IsShowByGroup
        {
            get => _isShowByGroup;
            set
            {
                SetProperty(ref _isShowByGroup, value);
            }
        }
        private bool _HideAssignedSocialMediaProfiles;
        [ProtoMember(5)]
        public bool HideAssignedSocialMediaProfiles
        {
            get => _HideAssignedSocialMediaProfiles;
            set
            {
                SetProperty(ref _HideAssignedSocialMediaProfiles, value);
            }
        }
        private bool _HideUserNameAndPassword;
        [ProtoMember(6)]
        public bool HideUserNameAndPassword
        {
            get => _HideUserNameAndPassword;
            set
            {
                SetProperty(ref _HideUserNameAndPassword, value);
            }
        }
        private bool _FilterByGroup;
        [ProtoMember(7)]
        public bool FilterByGroup
        {
            get => _FilterByGroup;
            set
            {
                SetProperty(ref _FilterByGroup, value);
            }
        }
        private bool _ShowOnlyProxyWithError;
        [ProtoMember(8)]
        public bool ShowOnlyProxyWithError
        {
            get => _ShowOnlyProxyWithError;
            set
            {
                SetProperty(ref _ShowOnlyProxyWithError, value);
            }
        }
        private bool _ShowOnlyUnAssignedProxy;
        [ProtoMember(9)]
        public bool ShowOnlyUnAssignedProxy
        {
            get => _ShowOnlyUnAssignedProxy;
            set
            {
                SetProperty(ref _ShowOnlyUnAssignedProxy, value);
            }
        }
        private string _verificationUrl= "https://www.google.com";
        [ProtoMember(10)]
        public string VerificationUrl
        {
            get => _verificationUrl;
            set
            {
                SetProperty(ref _verificationUrl, value);
            }
        }
    }
}