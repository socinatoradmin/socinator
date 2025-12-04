using System;
using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore;
using DominatorHouseCore.Models;
using DominatorUIUtility.CustomControl;

namespace LinkedDominatorUI.TabManager
{
    /// <summary>
    ///     Interaction logic for SettingsTab.xaml
    /// </summary>
    public partial class SettingsTab : UserControl
    {
        private static SettingsTab objSettingsTab;

        public SettingsTab()
        {
            try
            {
                InitializeComponent();
                var tab_items = new List<TabItemTemplates>
                {
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyBlacklistedUsers").ToString(),
                        Content = new Lazy<UserControl>(() => new BlacklistUserControl())
                    },
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyWhitelistedUsers").ToString(),
                        Content = new Lazy<UserControl>(() => new WhitelistuserControl())
                    }
                    //new TabItemTemplates
                    //{
                    // Title=FindResource("LangKeyGeneralSettings").ToString(),
                    // Content=new Lazy<UserControl>(()=> new GeneralSettings())
                    //} 
                    //new TabItemTemplates
                    //{
                    //    Title=FindResource("LangKeyLicensingDetails").ToString(),
                    //    Content=new Lazy<UserControl>(()=> new LicensingDetails())
                    //},
                    //    new TabItemTemplates
                    //{
                    //    Title=FindResource("LangKeyColorSettings").ToString(),
                    //    Content=new Lazy<UserControl>(()=> new ColorSettings())
                    //},
                };
                SettingsTabs.ItemsSource = tab_items;
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }


        public static SettingsTab GetSingeltonObjectSettingsTab()
        {
            return objSettingsTab ?? (objSettingsTab = new SettingsTab());
        }

        public void SetIndex(int index)
        {
            //GrowConnectionsTab is the name of this Tab
            SettingsTabs.SelectedIndex = index;
        }
    }
}