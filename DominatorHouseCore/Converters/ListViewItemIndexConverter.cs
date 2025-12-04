#region

using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

#endregion

namespace DominatorHouseCore.Converters
{
    public class ListViewItemIndexConverter : IValueConverter
    {
        public object Convert(object value, Type TargetType, object parameter, CultureInfo culture)
        {
            try
            {
                var item = (ListViewItem) value;
                var listView = ItemsControl.ItemsControlFromItemContainer(item) as ListView;
                var index = listView.ItemContainerGenerator.IndexFromContainer(item) + 1;
                return index.ToString();
            }
            catch (Exception)
            {
                return "1";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}