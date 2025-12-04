using System;
using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore.Models;

namespace DominatorHouse.Social.Settings.View
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home
    {
        public Home()
        {
            InitializeComponent();
            var TabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title=FindResource("LangKeyAppearance").ToString(),
                    Content=new Lazy<UserControl>(()=>new Appearance())
                }
            };
            SettingTabControls.ItemsSource = TabItems;
        }
    }
}
