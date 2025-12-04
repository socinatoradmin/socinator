using DominatorHouseCore.Models;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using YoutubeDominatorUI.CustomControl;
using YoutubeDominatorUI.YDViews.Tools.GrowMessagers;

namespace YoutubeDominatorUI.YDViews.Tools
{
    /// <summary>
    /// Interaction logic for MessageConfigurationTab.xaml
    /// </summary>
    public partial class MessageConfigurationTab : UserControl
    {
        public MessageConfigurationTab()
        {
            InitializeComponent();
            var TabItems = new List<TabItemTemplates>()
            {
                new TabItemTemplates
                {
                    Title=FindResource("LangKeyConfiguration").ToString(),

                    Content=new Lazy<UserControl>(()=> MessageConfiguration.GetSingeltonObjectMessageConfiguration())

                }
                ,
                new TabItemTemplates
                {
                    Title=FindResource("LangKeyReports").ToString(),
                    Content=new Lazy<UserControl>(()=>new AccountReport(DominatorHouseCore.Enums.ActivityType.BroadcastMessages))
                }
            };
            MessageTabs.ItemsSource = TabItems;
        }
    }
}
