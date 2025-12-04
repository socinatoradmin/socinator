using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorUIUtility.CustomControl;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace RedditDominatorUI.TabManager
{
    /// <summary>
    ///     Interaction logic for CampaignTab.xaml
    /// </summary>
    // ReSharper disable once InheritdocConsiderUsage
    public partial class CampaignTab : UserControl
    {
        public CampaignTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyAllCampaigns") == null
                        ? "Campaigns"
                        : Application.Current.FindResource("LangKeyAllCampaigns")?.ToString(),
                    Content = new Lazy<UserControl>(() => new Campaigns(SocialNetworks.Reddit))
                }
            };
            CampaignTabs.ItemsSource = tabItems;
        }

        public static int SelectedIndex { get; private set; }

        public void SetIndex(int index)
        {
            SelectedIndex = index;
        }
    }
}