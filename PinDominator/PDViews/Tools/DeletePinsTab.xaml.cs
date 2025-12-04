using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using PinDominator.CustomControl;
using PinDominator.PDViews.Tools.DeletePins;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace PinDominator.PDViews.Tools
{
    /// <summary>
    ///     Interaction logic for DeletePinsTab.xaml
    /// </summary>
    public partial class DeletePinsTab 
    {
        public DeletePinsTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyConfiguration").ToString(),
                    Content = new Lazy<UserControl>(DeletePinsConfiguration.GetSingeltonObjectDeletePinsConfiguration)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReports").ToString(),
                    Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.DeletePin))
                }
            };
            DeletePinsTabs.ItemsSource = tabItems;
        }
    }
}