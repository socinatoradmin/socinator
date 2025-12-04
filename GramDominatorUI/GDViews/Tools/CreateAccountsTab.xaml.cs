using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Models;
using GramDominatorUI.GDViews.Tools.CreateAccounts;

namespace GramDominatorUI.GDViews.Tools
{
    /// <summary>
    ///     Interaction logic for CreateAccountsTab.xaml
    /// </summary>
    public partial class CreateAccountsTab : UserControl
    {
        public CreateAccountsTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyConfiguration") != null
                        ? Application.Current.FindResource("LangKeyConfiguration").ToString()
                        : "Configuration",
                    Content = new Lazy<UserControl>(() => new AccountConfiguration())
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyReports") != null
                        ? Application.Current.FindResource("LangKeyReports").ToString()
                        : "Reports"
                    //Content=new Lazy<UserControl>(()=>new AccountReports())
                }
            };
            createAccountTabs.ItemsSource = tabItems;
        }
    }
}