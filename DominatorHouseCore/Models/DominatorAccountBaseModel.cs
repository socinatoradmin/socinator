#region

using System.Collections.Generic;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;
using ProtoBuf;
using BindableBase = Prism.Mvvm.BindableBase;

#endregion

namespace DominatorHouseCore.Models
{
    [ProtoContract]
    public class DominatorAccountBaseModel : BindableBase
    {
        private SocialNetworks _accountNetwork = SocialNetworks.Facebook;

        /// <summary>
        ///     To Identify Account is belongs to which network
        /// </summary>
        [ProtoMember(1)]
        public SocialNetworks AccountNetwork
        {
            get => _accountNetwork;
            set => SetProperty(ref _accountNetwork, value);
        }

        [ProtoIgnore]
        public List<GrowthProperty> GrowthProperties
        {
            get => _growthProperties;
            set => SetProperty(ref _growthProperties, value);
        }

        private ContentSelectGroup _accountGroup = new ContentSelectGroup();

        /// <summary>
        ///     To define the account is belongs to which group
        /// </summary>
        [ProtoMember(2)]
        public ContentSelectGroup AccountGroup
        {
            get => _accountGroup;
            set
            {
                if (_accountGroup == value)
                    return;
                SetProperty(ref _accountGroup, value);
            }
        }


        private string _userName = string.Empty;

        /// <summary>
        ///     To define the social networks username of the accounts
        /// </summary>
        [ProtoMember(3)]
        public string UserName
        {
            get => _userName;
            set
            {
                if (_userName != null && _userName == value)
                    return;
                SetProperty(ref _userName, value);
            }
        }

        private string _password = string.Empty;

        /// <summary>
        ///     To store the account password
        /// </summary>
        [ProtoMember(4)]
        public string Password
        {
            get => _password;
            set
            {
                if (_password != null && _password == value)
                    return;
                SetProperty(ref _password, value);
            }
        }


        private string _userId = string.Empty;

        /// <summary>
        ///     To define the social networks id of the account
        /// </summary>
        [ProtoMember(5)]
        public string UserId
        {
            get => _userId;
            set
            {
                if (_userId != null && _userId == value)
                    return;
                SetProperty(ref _userId, value);
            }
        }


        private string _userFullName = string.Empty;

        /// <summary>
        ///     To define the username of the account
        /// </summary>
        [ProtoMember(6)]
        public string UserFullName
        {
            get => _userFullName;
            set
            {
                if (_userFullName != null && _userFullName == value)
                    return;
                SetProperty(ref _userFullName, value);
            }
        }

        private string _profilePictureUrl = string.Empty;

        /// <summary>
        ///     To define the profile picture url of the account
        /// </summary>
        [ProtoMember(7)]
        public string ProfilePictureUrl
        {
            get => _profilePictureUrl;
            set
            {
                if (_profilePictureUrl != null && _profilePictureUrl == value)
                    return;
                SetProperty(ref _profilePictureUrl, value);
            }
        }

        private Proxy _accountProxy = new Proxy();

        /// <summary>
        ///     To define the Account Proxy
        /// </summary>
        [ProtoMember(8)]
        public Proxy AccountProxy
        {
            get => _accountProxy;
            set
            {
                if (_accountProxy != null && _accountProxy == value)
                    return;
                SetProperty(ref _accountProxy, value);
            }
        }

        private string _accountId = Utilities.GetGuid();

        /// <summary>
        ///     To access the account with unique Id
        /// </summary>
        [ProtoMember(9)]
        public string AccountId
        {
            get => _accountId;
            set
            {
                if (_accountId != null && _accountId == value)
                    return;
                SetProperty(ref _accountId, value);
            }
        }

        private AccountStatus _status;

        /// <summary>
        ///     To define the status of the account
        /// </summary>
        [ProtoMember(10)]
        public AccountStatus Status
        {
            get => _status;
            set
            {
                if (_status == value)
                    return;
                SetProperty(ref _status, value);
            }
        }

        private string _profileId = string.Empty;
        private List<GrowthProperty> _growthProperties;

        /// <summary>
        ///     To define network profile username
        /// </summary>
        [ProtoMember(11)]
        public string ProfileId
        {
            get => _profileId;
            set
            {
                if (_profileId != null && _profileId == value)
                    return;
                SetProperty(ref _profileId, value);
            }
        }

        private bool _isChkTwoFactorLogin;

        [ProtoMember(12)]
        public bool IsChkTwoFactorLogin
        {
            get => _isChkTwoFactorLogin;
            set
            {
                if (_isChkTwoFactorLogin == value)
                    return;
                SetProperty(ref _isChkTwoFactorLogin, value);
            }
        }

        private string _alternateEmail = string.Empty;

        [ProtoMember(13)]
        public string AlternateEmail
        {
            get => _alternateEmail;
            set
            {
                if (_alternateEmail != null && _alternateEmail == value)
                    return;
                SetProperty(ref _alternateEmail, value);
            }
        }

        private string _phoneNumber = string.Empty;

        [ProtoMember(14)]
        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                if (_phoneNumber != null && _phoneNumber == value)
                    return;
                SetProperty(ref _phoneNumber, value);
            }
        }

        private string _banned = "No";

        [ProtoMember(15)]
        public string Banned
        {
            get => _banned;
            set => SetProperty(ref _banned, value);
        }

        private string _accountName = "";

        [ProtoMember(16)]
        public string AccountName
        {
            get => _accountName;
            set => SetProperty(ref _accountName, value);
        }

        private PinterestAccountType _pinterestAccountType;

        [ProtoMember(17)]
        public PinterestAccountType PinterestAccountType
        {
            get
            {
                if (AccountNetwork != SocialNetworks.Pinterest)
                    return PinterestAccountType.NotAvailable;
                return _pinterestAccountType;
            }
            set => SetProperty(ref _pinterestAccountType, value);
        }

        private bool _checkAccountStatus=true;

        [ProtoMember(18)]
        public bool NeedToCloseBrowser
        {
            get
            {
                return _checkAccountStatus;
            }
            set => SetProperty(ref _checkAccountStatus, value);
        }
        public override string ToString()
        {
            return string.Format("{0} on {1}", _userName, _accountNetwork);
        }
    }
}