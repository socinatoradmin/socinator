using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using PinDominator.CustomControl;
using PinDominator.PDViews.Tools.EditPins;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace PinDominator.PDViews.Tools
{
    /// <summary>
    ///     Interaction logic for EditPinTab.xaml
    /// </summary>
    public partial class EditPinTab
    {
        public EditPinTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyConfiguration").ToString(),
                    Content = new Lazy<UserControl>(EditPinsConfiguration.GetSingletonObjectEditPinsConfiguration)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReports").ToString(),
                    Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.EditPin))
                }
            };
            EditPinsTabs.ItemsSource = tabItems;
        }
    }
}