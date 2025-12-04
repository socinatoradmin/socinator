using System;
using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using LinkedDominatorUI.CustomControl;
using LinkedDominatorUI.LDViews.Tools.Profilling;

namespace LinkedDominatorUI.LDViews.Tools.Tabs
{
    /// <summary>
    ///     Interaction logic for ProfileEndorsementTab.xaml
    /// </summary>
    public partial class ProfileEndorsementTab : UserControl
    {
        private static ProfileEndorsementTab objProfileEndorsementTab;

        public ProfileEndorsementTab()
        {
            try
            {
                InitializeComponent();
                var tab_items = new List<TabItemTemplates>
                {
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyConfiguration").ToString(),
                        Content = new Lazy<UserControl>(() =>
                            ProfileEndorsementConfiguration.GetSingeltonObjectProfileEndorsementConfiguration())
                    },
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyReports").ToString(),
                        Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.ProfileEndorsement))
                    }
                };
                ProfileEndorsementTabs.ItemsSource = tab_items;
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }


        public static ProfileEndorsementTab GetSingeltonObjectProfileEndorsementTab()
        {
            return objProfileEndorsementTab ?? (objProfileEndorsementTab = new ProfileEndorsementTab());
        }
    }
}