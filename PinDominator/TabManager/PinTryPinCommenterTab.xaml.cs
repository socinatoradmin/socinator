using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Models;
using PinDominator.PDViews.PinTryComment;

namespace PinDominator.TabManager
{
    /// <summary>
    ///     Interaction logic for PinTryPinCommenter.xaml
    /// </summary>
    public partial class PinTryPinCommenterTab
    {
        public PinTryPinCommenterTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyComments") == null
                        ? "Comments"
                        : Application.Current.FindResource("LangKeyComments")?.ToString(),
                    Content = new Lazy<UserControl>(Comment.GetSingeltonObjectComment)
                }
                //,new TabItemTemplates
                //{
                //    Title = Application.Current.FindResource("LangKeyTry") == null
                //        ? "Try"
                //        : Application.Current.FindResource("LangKeyTry")?.ToString(),
                //    Content = new Lazy<UserControl>(Try.GetSingeltonObjectTry)
                //}
            };

            PinTryPinCommenterTabs.ItemsSource = tabItems;
        }

        private static PinTryPinCommenterTab CurrentPinTryPinCommenterTab { get; set; }

        /// <summary>
        ///     GetSingeltonObjectPinTryPinCommenterTab is used to get the object of the current user control,
        ///     if object is already created then its won't create a new object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static PinTryPinCommenterTab GetSingeltonObjectPinTryPinCommenterTab()
        {
            return CurrentPinTryPinCommenterTab ?? (CurrentPinTryPinCommenterTab = new PinTryPinCommenterTab());
        }

        public void SetIndex(int index)
        {
            PinTryPinCommenterTabs.SelectedIndex = index;
        }
    }
}