using System.Collections.Generic;
using System.Linq;
using System.Net;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.Settings;
using ProtoBuf;

namespace LinkedDominatorCore.LDModel
{
    [ProtoContract]
    public class AccountModel : BindableBase, IAccountModel
    {
        private AccountGroup _accountGroup = new AccountGroup();


        private Proxy _accountProxy = new Proxy();


        private int _ConnectionsCount;


        [ProtoMember(14)] private HashSet<CookieHelper> _cookieHelperList = new HashSet<CookieHelper>();


        private int _GroupsCount;

        private bool _isAccountManagerAccountSelected;

        private bool _isAccountSelected;


        private string _password;


        private int _PostsCount;


        private string _profilePictureUrl;


        private int _rowNo = 1;

        private bool _selectedGroup;


        private string _status = "Not Checked";


        private string _userName;

        public ActivityType ActivityType;

        public AccountModel(DominatorAccountModel dominatorAccountModel)
        {
            _dominatorAccountModel = dominatorAccountModel;
        }

        private DominatorAccountModel _dominatorAccountModel { get; }

        [ProtoMember(21)]
        public bool IsAccountManagerAccountSelected
        {
            get => _isAccountManagerAccountSelected;
            set
            {
                if (value == _isAccountManagerAccountSelected)
                    return;
                SetProperty(ref _isAccountManagerAccountSelected, value);
            }
        }

        [ProtoMember(19)] public string AccountId { get; set; } = Utilities.GetGuid();

        public string UserAgentWeb { get; set; }

        public string UserAgentMobile { get; set; }


        public bool IsloggedinWithPhone { get; set; }


        public HttpHelper HttpHelperMobile { get; set; }

        public string Deviceid { get; set; }

        public string Guid { get; set; }


        [ProtoMember(15)] public string AdvertisingId { get; internal set; }


        [ProtoMember(16)] public string RankToken { get; internal set; }


        [ProtoMember(17)] public Dictionary<string, Dictionary<string, string>> Experiments { get; internal set; }


        [ProtoMember(18)] public int LastExperiments { get; internal set; }

        public string Pk { get; set; }

        public string CsrfToken { get; set; }


        public string Uuid { get; set; }


        public CookieCollection Cookies
        {
            get
            {
                var cookieCollection = new CookieCollection();
                foreach (var cookieHelper in _cookieHelperList)
                    cookieCollection.Add(new Cookie
                    {
                        Domain = cookieHelper.Domain,
                        Name = cookieHelper.Name,
                        Value = cookieHelper.Value
                    });
                return cookieCollection;
            }
            set
            {
                _cookieHelperList = value.Cast<Cookie>().Select(
                    cookie => new CookieHelper
                    {
                        Domain = cookie.Domain,
                        Name = cookie.Name,
                        Value = cookie.Value
                    }).ToHashSet();
            }
        }


        [ProtoMember(20)] public bool IsCretedFromNormalMode { get; set; }

        public List<LinkedinUser> LstConnections { get; set; }

        public SocialNetworks AccountNetwork { get; set; } = SocialNetworks.LinkedIn;

        [ProtoMember(12)]
        public bool IsAccountSelected
        {
            get => _isAccountSelected;
            set
            {
                if (_isAccountSelected && value == _isAccountSelected)
                    return;
                SetProperty(ref _isAccountSelected, value);
            }
        }

        [ProtoMember(1)]
        public int RowNo
        {
            get => _rowNo;
            set
            {
                if (_rowNo != 0 && value == _rowNo)
                    return;
                SetProperty(ref _rowNo, value);
            }
        }

        [ProtoMember(6)]
        public int ConnectionsCount
        {
            get => _ConnectionsCount;
            set
            {
                if (_ConnectionsCount != 0 && value == _ConnectionsCount)
                    return;
                SetProperty(ref _ConnectionsCount, value);
            }
        }

        [ProtoMember(7)]
        public int GroupsCount
        {
            get => _GroupsCount;
            set
            {
                if (_GroupsCount != 0 && value == _GroupsCount)
                    return;
                SetProperty(ref _GroupsCount, value);
            }
        }

        [ProtoMember(8)]
        public int PostsCount
        {
            get => _PostsCount;
            set
            {
                if (_PostsCount != 0 && value == _PostsCount)
                    return;
                SetProperty(ref _PostsCount, value);
            }
        }

        [ProtoMember(4)]
        public AccountGroup AccountGroup
        {
            get => _accountGroup;
            set
            {
                if (_accountGroup != null && value == _accountGroup)
                    return;
                SetProperty(ref _accountGroup, value);
            }
        }

        public bool SelectedGroup
        {
            get => _selectedGroup;
            set
            {
                if (_selectedGroup && value == _selectedGroup)
                    return;
                SetProperty(ref _selectedGroup, value);
            }
        }

        public string ProfilePictureUrl
        {
            get => _profilePictureUrl;
            set
            {
                if (_profilePictureUrl != null && value == _profilePictureUrl)
                    return;
                SetProperty(ref _profilePictureUrl, value);
            }
        }

        [ProtoMember(2)]
        public string UserName
        {
            get => _userName;
            set
            {
                if (_userName != null && value == _userName)
                    return;
                SetProperty(ref _userName, value);
            }
        }

        [ProtoMember(5)]
        public string Status
        {
            get => _status;
            set
            {
                if (_status != null && value == _status)
                    return;
                SetProperty(ref _status, value);
            }
        }

        [ProtoMember(9)]
        public Proxy AccountProxy
        {
            get => _accountProxy;
            set
            {
                if (_accountProxy != null && value == _accountProxy)
                    return;
                SetProperty(ref _accountProxy, value);
            }
        }


        public HttpHelper HttpHelper { get; set; }


        public string UserFullName { get; set; }


        public string AccountHasAnonymousProfile { get; set; }

        public bool IsUserLoggedIn { get; set; }

        public bool IsPrivateUser { get; set; }

        public bool IsVerifiedUser { get; set; }

        public int LastAnalyticsUpdate { get; set; }

        public int LastLogin { get; set; }

        [ProtoMember(3)]
        public string Password
        {
            get => _password;
            set
            {
                if (_accountProxy != null && value == _password)
                    return;
                SetProperty(ref _password, value);
            }
        }

        public string SessionId { get; set; }

        public string UserId { get; set; }

        public DeviceGenerator DeviceDetails { get; set; } = new DeviceGenerator();


        [ProtoMember(13)] public ActivityManager ActivityManager { get; set; } = new ActivityManager();

        
    }
}