using ProtoBuf;
using DominatorHouseCore.Utility;
using System.Collections.ObjectModel;
using Prism.Commands;
using System;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Enums;

namespace DominatorHouseCore.Models.Config
{
    public interface IBrowserAutomationModel
    {
        ObservableCollection<DominatorAccountModel> ListSocialAccounts { get; set; }

        ObservableCollection<string> ListSocialNetworks { get; set; }

        string SelectedNetwork { get; set; }

        DelegateCommand NetWorkChangedCommand { get; set; }
    }


    [ProtoContract]
    public class BrowserAutomationModel : BindableBase, IBrowserAutomationModel
    {
        public DelegateCommand NetWorkChangedCommand { get; set; }

        private readonly IAccountsFileManager _accountsFileManager;

        public BrowserAutomationModel(IAccountsFileManager accountsFileManager)
        {
            _accountsFileManager = accountsFileManager;
            NetWorkChangedCommand = new DelegateCommand(UpdateAccountList);
        }

        private void UpdateAccountList()
        {
            if (SelectedNetwork == SocialNetworks.Social.ToString())
                ListSocialAccounts = new ObservableCollection<DominatorAccountModel>(_accountsFileManager.GetAll());
            else
                ListSocialAccounts = new ObservableCollection<DominatorAccountModel>(_accountsFileManager.GetAll((SocialNetworks)Enum.Parse(typeof(SocialNetworks), SelectedNetwork)));
        }

        private ObservableCollection<DominatorAccountModel> _listSocialAccounts;

        [ProtoMember(1)]
        public ObservableCollection<DominatorAccountModel> ListSocialAccounts
        {
            get
            {
                return _listSocialAccounts;
            }
            set
            {
                if (_listSocialAccounts == value)
                    return;
                SetProperty(ref _listSocialAccounts, value);
            }
        }

        private ObservableCollection<string> _listSocialNetworks;

        [ProtoMember(2)]
        public ObservableCollection<string> ListSocialNetworks
        {
            get
            {
                return _listSocialNetworks;
            }
            set
            {
                if (_listSocialNetworks == value)
                    return;
                SetProperty(ref _listSocialNetworks, value);
            }
        }

        private string _selectedNetwork;

        [ProtoMember(3)]
        public string SelectedNetwork
        {
            get
            {
                return _selectedNetwork;
            }
            set
            {
                if (_selectedNetwork == value)
                    return;
                SetProperty(ref _selectedNetwork, value);
            }
        }
    }
}
