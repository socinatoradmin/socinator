using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DominatorUIUtility.Behaviours
{
    public class HeaderHelper
    {
        public static Action UpdateToggleButtonInCampaignMode;
        public static Action UpdateToggleButtonInAccountActivityMode;
        public static Action UpdateToggleForQuery;
        public static Action UpdateToggleForNonQuery;

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject usercontrol) where T : DependencyObject
        {
            if (usercontrol != null)
                for (var i = 0; i < VisualTreeHelper.GetChildrenCount(usercontrol); i++)
                {
                    var userControlChild = VisualTreeHelper.GetChild(usercontrol, i);
                    if (userControlChild is T child) yield return child;

                    foreach (var childOfChild in FindVisualChildren<T>(userControlChild)) yield return childOfChild;
                }
        }

        public static void ExpandCollapseAllExpander(object sender, bool isExpanded)
        {
            var currentControl = ((FrameworkElement) ((FrameworkElement) sender).DataContext).DataContext;

            foreach (var expander in FindVisualChildren<Expander>(currentControl as UserControl))
                expander.IsExpanded = isExpanded;
        }

        public static void ExpandCollapseAllExpanderForActivity(object sender, bool isExpanded)
        {
            var currentControl = (FrameworkElement) sender;

            foreach (var expander in FindVisualChildren<Expander>(currentControl as UserControl))
                expander.IsExpanded = isExpanded;
        }

        public static bool IsAllExpanderCollapseOrNot(object sender)
        {
            var allExpander = FindVisualChildren<Expander>(sender as UserControl);
            return allExpander.Count() != 0 && allExpander.All(x => !x.IsExpanded);
        }
    }
}