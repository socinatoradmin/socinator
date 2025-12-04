using System;
using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using TwtDominatorUI.CustomControl;
using TwtDominatorUI.TDViews.Tools.Message;

namespace TwtDominatorUI.TDViews.Tools
{
    /// <summary>
    ///     Interaction logic for MessageToNewFollowerTab.xaml
    /// </summary>
    public partial class MessageToNewFollowerTab : UserControl
    {
        private static MessageToNewFollowerTab objMessageToNewFollowerTab;

        public MessageToNewFollowerTab()
        {
            InitializeComponent();
            var TabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyConfiguration").ToString(),
                    Content = new Lazy<UserControl>(() => new MessageToNewFollowersConfig())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReports").ToString(),
                    Content = new Lazy<UserControl>(() => new AccountWiseReport(ActivityType.SendMessageToFollower))
                }
            };
            MessageToNewFollowersTabControl.ItemsSource = TabItems;
        }

        public static MessageToNewFollowerTab GetSingletonobjMessageToNewFollowerTab()
        {
            return objMessageToNewFollowerTab ?? (objMessageToNewFollowerTab = new MessageToNewFollowerTab());
        }
    }
}