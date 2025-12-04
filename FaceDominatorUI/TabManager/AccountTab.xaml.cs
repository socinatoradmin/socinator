using DominatorHouseCore.Models;
using FaceDominatorUI.FDViews.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DominatorHouseCore.Enums;
using DominatorUIUtility.CustomControl;

namespace FaceDominatorUI.TabManager
{
    /// <summary>
    /// Interaction logic for AccountTab.xaml
    /// </summary>
    public partial class AccountTab : UserControl
    {
        public AccountTab()
        {
            InitializeComponent();

            var items = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title=FindResource("langAccountsManager").ToString(),
                    Content = new Lazy<UserControl>(()=>AccountCustomControl.GetAccountCustomControl(SocialNetworks.Facebook))
                },
                new TabItemTemplates
                {
                    Title =FindResource("langDashboardBeta").ToString(),
                    Content = new Lazy<UserControl>(() => new DashBoard())
                },
                new TabItemTemplates
                {
                     Title =FindResource("langAccountStatsBeta").ToString(),
                    Content = new Lazy<UserControl>(()=> new AccountStats())
                }
            };

            AccountTabs.ItemsSource = items;
        }
    
    }
}
