#region

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

#endregion

namespace DominatorHouseCore.UIExtensions
{
    public static class GridViewExtension
    {
        public static ObservableCollection<GridViewColumn> GetColumns(DependencyObject obj)
        {
            return (ObservableCollection<GridViewColumn>) obj.GetValue(ColumnsProperty);
        }

        public static void SetColumns(DependencyObject obj, ObservableCollection<GridViewColumn> value)
        {
            obj.SetValue(ColumnsProperty, value);
        }

        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.RegisterAttached("Columns",
                typeof(ObservableCollection<GridViewColumn>),
                typeof(GridViewExtension),
                new UIPropertyMetadata(new ObservableCollection<GridViewColumn>(),
                    OnGridViewColumnsPropertyChanged));


        private static void OnGridViewColumnsPropertyChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            if (!(d is GridView myGrid)) return;
            var columns =
                (ObservableCollection<GridViewColumn>) e.NewValue;

            if (columns != null)
            {
                var lastAddedColumns = new List<GridViewColumn>();
                foreach (var column
                    in columns)
                {
                    myGrid.Columns.Add(column);
                    lastAddedColumns.Add(column);
                }

                columns.CollectionChanged += delegate(object sender,
                    NotifyCollectionChangedEventArgs args)
                {
                    if (args.NewItems != null)
                        foreach (var column
                            in args.NewItems.Cast<GridViewColumn>())
                        {
                            myGrid.Columns.Add(column);
                            lastAddedColumns.Add(column);
                        }

                    if (args.Action == NotifyCollectionChangedAction.Reset)
                        foreach (var lastAddedColumn in lastAddedColumns)
                            myGrid.Columns.Remove(lastAddedColumn);

                    if (args.OldItems != null)
                        foreach (var column
                            in args.OldItems.Cast<GridViewColumn>())
                            myGrid.Columns.Remove(column);
                };
            }
        }
    }
}