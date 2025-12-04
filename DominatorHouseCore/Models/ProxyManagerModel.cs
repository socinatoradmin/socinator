#region

using System.Collections.ObjectModel;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models
{
    [ProtoContract]
    public class ProxyManagerModel : BindableBase
    {
        private int _index;

        public int Index
        {
            get => _index;
            set => SetProperty(ref _index, value);
        }

        private Proxy _accountProxy = new Proxy();

        [ProtoMember(1)]
        public Proxy AccountProxy
        {
            get => _accountProxy;
            set => SetProperty(ref _accountProxy, value);
        }

        private bool _isProxySelected;

        public bool IsProxySelected
        {
            get => _isProxySelected;
            set => SetProperty(ref _isProxySelected, value);
        }

        private string _status = "Not Checked";

        [ProtoMember(12)]
        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        private string _responseTime = "0 milli seconds";

        [ProtoMember(13)]
        public string ResponseTime
        {
            get => _responseTime;
            set => SetProperty(ref _responseTime, value);
        }

        private int _failures;

        [ProtoMember(14)]
        public int Failures
        {
            get => _failures;
            set => SetProperty(ref _failures, value);
        }

        private ObservableCollection<AccountAssign> _accountsAssignedto = new ObservableCollection<AccountAssign>();

        [ProtoMember(15)]
        public ObservableCollection<AccountAssign> AccountsAssignedto
        {
            get => _accountsAssignedto;
            set => SetProperty(ref _accountsAssignedto, value);
        }

        private ObservableCollection<AccountAssign> _accountsToBeAssign = new ObservableCollection<AccountAssign>();

        [ProtoMember(16)]
        public ObservableCollection<AccountAssign> AccountsToBeAssign
        {
            get => _accountsToBeAssign;
            set => SetProperty(ref _accountsToBeAssign, value);
        }

        private string _group;

        [ProtoMember(17)]
        public string Group
        {
            get => _group;
            set => SetProperty(ref _group, value);
        }
    }

    [ProtoContract]
    public class AccountAssign : BindableBase
    {
        private string _userName;

        [ProtoMember(1)]
        public string UserName
        {
            get => _userName;
            set => SetProperty(ref _userName, value);
        }

        private SocialNetworks _accountNetwork = SocialNetworks.Facebook;

        [ProtoMember(2)]
        public SocialNetworks AccountNetwork
        {
            get => _accountNetwork;
            set => SetProperty(ref _accountNetwork, value);
        }
    }
}