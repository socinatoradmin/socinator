using DominatorHouseCore.Utility;
using GramDominatorUI.TabManager;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using GramDominatorCore.GDModel;
using GramDominatorUI.GDViews.SocialProfiles;

namespace GramDominatorUI.AccountGrowthMode
{
    /// <summary>
    /// Interaction logic for AccountGrowth.xaml
    /// </summary>
    public partial class AccountGrowth : UserControl
    {
        private static AccountGrowth objAccountGrowth = null;

        public static AccountGrowth GetSingletonAccountGrowth()
        {
            return objAccountGrowth ?? (objAccountGrowth = new AccountGrowth());
        }


        AccountGrowthHelper objAccountGrowthHelper = AccountGrowthHelper.GetSingeltonObjectAccountGrowthHelper();
        private AccountGrowth()
        {
            InitializeComponent();

            var TabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title=FindResource("langDashBoard").ToString(),
                 //   Content=new Lazy<UserControl>(()=>new CommentTab())
                },
                new TabItemTemplates
                {
                    Title=FindResource("langSocialProfiles").ToString(),
                  Content=new Lazy<UserControl>(()=>new SocialProfile())
                },
                new TabItemTemplates
                {
                    Title=FindResource("langTools").ToString(),
                    Content=new Lazy<UserControl>(()=>new ToolTabs())
                },
                
            };
            AccountGrowthModeTab.ItemsSource = TabItems;
            MainGrid.DataContext = objAccountGrowthHelper;
        }
    }
    public class AccountGrowthHelper : BindableBase
    {
        private static AccountGrowthHelper objAccountGrowthHelper = null;
        public static AccountGrowthHelper GetSingeltonObjectAccountGrowthHelper()
        {
            if (objAccountGrowthHelper == null)
                objAccountGrowthHelper = new AccountGrowthHelper();
            return objAccountGrowthHelper;
        }
        public AccountGrowthHelper()
        {
            var folder = ConstantVariable.GetDominatorPath(SocialNetworks.Instagram) + "//Index//AC";
            try
            {
                //foreach (var account in ProtoBuffBase.DeserializeObjects<FollowerModel>(folder + "//AccountDetails.bin"))
                {
                    // lstAccounts.Add(account.SelectedAccount);

                }
            }
            catch (Exception ex)
            {


            }
        }
        public ObservableCollectionBase<string> lstAccounts { get; set; } = new ObservableCollectionBase<string>();
        private string _selectedAccount = string.Empty;

        public string SelectedAccount
        {
            get
            {
                return _selectedAccount;
            }
            set
            {
                if (value == _selectedAccount)
                    return;
                SetProperty(ref _selectedAccount, value);
            }
        }

    }
}
