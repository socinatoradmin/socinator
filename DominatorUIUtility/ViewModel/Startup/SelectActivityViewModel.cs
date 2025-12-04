using System;
using System.Collections.Generic;
using System.Linq;
using DominatorHouseCore.Converters;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces.StartUp;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.NetworkActivitySetting;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup
{
    public interface ISelectActivityViewModel
    {
        SelectActivityModel SelectActivityModel { get; set; }
        DominatorAccountModel SelectAccount { get; set; }
        string SelectedNetwork { get; set; }
        void SetActivityTypeByNetwork(string network);
    }

    public class SelectActivityViewModel : StartupBaseViewModel, ISelectActivityViewModel
    {
        private DominatorAccountModel _selectAccount = new DominatorAccountModel();

        private SelectActivityModel _selectActivityModel = new SelectActivityModel();
        private string _selectedNetwork;

        public SelectActivityViewModel(IRegionManager region) : base(region)
        {
            NextCommand = new DelegateCommand(OnNextClick);
            PreviousCommand = new DelegateCommand(NavigatePrevious);
        }

        public SelectActivityModel SelectActivityModel
        {
            get => _selectActivityModel;
            set => SetProperty(ref _selectActivityModel, value);
        }

        public string SelectedNetwork
        {
            get => _selectedNetwork;
            set
            {
                SetProperty(ref _selectedNetwork, value);
                if (SelectedNetwork != SocialNetworks.Social.ToString())
                    SetActivityTypeByNetwork(SelectedNetwork);
            }
        }

        public DominatorAccountModel SelectAccount
        {
            get => _selectAccount;
            set => SetProperty(ref _selectAccount, value);
        }

        public void SetActivityTypeByNetwork(string network)
        {
            SelectActivityModel.LstNetworkActivityType.Clear();

            foreach (var name in Enum.GetNames(typeof(ActivityType)))
                if (EnumDescriptionConverter.GetDescription((ActivityType)Enum.Parse(typeof(ActivityType), name))
                    .Contains(network) && name != "Try")
                    SelectActivityModel.LstNetworkActivityType.Add(new ActivityChecked
                    {
                        ActivityType = name
                    });
        }

        private void OnNextClick()
        {
            var allSelectedActivity = SelectActivityModel.LstNetworkActivityType.Where(x => x.IsActivity).ToList();
            if (allSelectedActivity.Count() == 0)
            {
                Dialog.ShowDialog("LangKeyError".FromResourceDictionary(),
                    "LangKeySelectAtleastOneActivity".FromResourceDictionary());
                return;
            }

            NavigationList = new List<string>();
            LstGlobalQuery = new Dictionary<Type, List<QueryInfo>>();
            NavigationList.Add("SelectActivity");
            allSelectedActivity.ForEach(name => NavigationList.Add(name.ActivityType));
            SocialNetworkActivity.RegisterNetwork();
            NavigateNext();
        }
    }
}