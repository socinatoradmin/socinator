#region

using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

#endregion

namespace DominatorHouseCore.Utility
{
    public static class GridViewColumns
    {
        public static void SetColumnValues(DependencyObject obj, object value)
        {
            obj.SetValue(ColumnValuesProperty, value);
        }

        // Using a DependencyProperty as the backing store for ColumnValues.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnValuesProperty =
            DependencyProperty.RegisterAttached("ColumnValues", typeof(object), typeof(GridViewColumns),
                new UIPropertyMetadata(null, ColumnValuesChanged));


        [AttachedPropertyBrowsableForType(typeof(GridView))]
        public static string GetColumnHeader(DependencyObject obj)
        {
            return (string) obj.GetValue(ColumnHeaderProperty);
        }

        public static void SetColumnHeader(DependencyObject obj, string value)
        {
            obj.SetValue(ColumnHeaderProperty, value);
        }

        // Using a DependencyProperty as the backing store for ColumnHeader.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnHeaderProperty =
            DependencyProperty.RegisterAttached("ColumnHeader", typeof(string), typeof(GridViewColumns),
                new UIPropertyMetadata(null));


        [AttachedPropertyBrowsableForType(typeof(GridView))]
        public static string GetColumnBinding(DependencyObject obj)
        {
            return (string) obj.GetValue(ColumnBindingProperty);
        }

        public static void SetColumnBinding(DependencyObject obj, string value)
        {
            obj.SetValue(ColumnBindingProperty, value);
        }

        // Using a DependencyProperty as the backing store for ColumnBinding.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnBindingProperty =
            DependencyProperty.RegisterAttached("ColumnBinding", typeof(string), typeof(GridViewColumns),
                new UIPropertyMetadata(null));


        private static void ColumnValuesChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (!(obj is GridView gridView)) return;
            gridView.Columns.Clear();

            if (e.OldValue != null)
            {
                var view = CollectionViewSource.GetDefaultView(e.OldValue);
                if (view != null)
                    RemoveHandlers(gridView, view);
            }

            if (e.NewValue == null) return;
            {
                var view = CollectionViewSource.GetDefaultView(e.NewValue);
                if (view == null) return;
                AddHandlers(gridView, view);
                CreateColumns(gridView, view);
            }
        }

        private static readonly IDictionary<ICollectionView, List<GridView>> _gridViewsByColumnsSource =
            new Dictionary<ICollectionView, List<GridView>>();

        private static List<GridView> GetGridViewsForColumnSource(ICollectionView columnSource)
        {
            if (_gridViewsByColumnsSource.TryGetValue(columnSource, out var gridViews)) return gridViews;
            gridViews = new List<GridView>();
            _gridViewsByColumnsSource.Add(columnSource, gridViews);

            return gridViews;
        }

        private static void AddHandlers(GridView gridView, ICollectionView view)
        {
            GetGridViewsForColumnSource(view).Add(gridView);
            view.CollectionChanged += ColumnsSource_CollectionChanged;
        }

        private static void CreateColumns(GridView gridView, ICollectionView view)
        {
            foreach (var item in view)
            {
                var column = CreateColumn(gridView, item);
                gridView.Columns.Add(column);
            }
        }

        private static void RemoveHandlers(GridView gridView, ICollectionView view)
        {
            view.CollectionChanged -= ColumnsSource_CollectionChanged;
            GetGridViewsForColumnSource(view).Remove(gridView);
        }

        private static void ColumnsSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var view = sender as ICollectionView;
            var gridViews = GetGridViewsForColumnSource(view);
            if (gridViews == null || gridViews.Count == 0)
                return;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var gridView in gridViews)
                        for (var i = 0; i < e.NewItems.Count; i++)
                        {
                            var column = CreateColumn(gridView, e.NewItems[i]);
                            gridView.Columns.Insert(e.NewStartingIndex + i, column);
                        }

                    break;
                case NotifyCollectionChangedAction.Move:
                    foreach (var gridView in gridViews)
                    {
                        var columns = new List<GridViewColumn>();
                        for (var i = 0; i < e.OldItems.Count; i++)
                        {
                            var column = gridView.Columns[e.OldStartingIndex + i];
                            columns.Add(column);
                        }

                        for (var i = 0; i < e.NewItems.Count; i++)
                        {
                            var column = columns[i];
                            gridView.Columns.Insert(e.NewStartingIndex + i, column);
                        }
                    }

                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var gridView in gridViews)
                        for (var i = 0; i < e.OldItems.Count; i++)
                            gridView.Columns.RemoveAt(e.OldStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    foreach (var gridView in gridViews)
                        for (var i = 0; i < e.NewItems.Count; i++)
                        {
                            var column = CreateColumn(gridView, e.NewItems[i]);
                            gridView.Columns[e.NewStartingIndex + i] = column;
                        }

                    break;
                case NotifyCollectionChangedAction.Reset:
                    foreach (var gridView in gridViews)
                    {
                        gridView.Columns.Clear();
                        CreateColumns(gridView, sender as ICollectionView);
                    }

                    break;
                // ReSharper disable once RedundantEmptySwitchSection
                default:
                    break;
            }
        }

        private static GridViewColumn CreateColumn(GridView gridView, object columnSource)
        {
            var column = new GridViewColumn();
            var columnHeader = GetColumnHeader(gridView);
            var columnBinding = GetColumnBinding(gridView);
            if (!string.IsNullOrEmpty(columnHeader)) column.Header = GetPropertyValue(columnSource, columnHeader);
            if (string.IsNullOrEmpty(columnBinding)) return column;
            var propertyName = GetPropertyValue(columnSource, columnBinding) as string;
            column.DisplayMemberBinding = new Binding(propertyName);

            return column;
        }

        private static object GetPropertyValue(object obj, string propertyName)
        {
            if (obj == null) return null;
            var prop = obj.GetType().GetProperty(propertyName);
            if (prop != null)
                return prop.GetValue(obj, null);

            return null;
        }
    }

    public class GridViewColumnDescriptor
    {
        public string ColumnHeaderText { get; set; }

        public string ColumnBindingText { get; set; }
    }
}