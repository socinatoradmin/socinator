using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DominatorHouseCore.Utility
{
    public class PlaceholderService
    {
        public static readonly DependencyProperty PlaceholderProperty =
        DependencyProperty.RegisterAttached("Placeholder", typeof(string), typeof(PlaceholderService),
            new PropertyMetadata(string.Empty, OnPlaceholderChanged));

        public static string GetPlaceholder(DependencyObject obj)
        {
            return (string)obj.GetValue(PlaceholderProperty);
        }

        public static void SetPlaceholder(DependencyObject obj, string value)
        {
            obj.SetValue(PlaceholderProperty, value);
        }

        private static void OnPlaceholderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox)
            {
                if (!textBox.IsLoaded)
                    textBox.Loaded += (s, args) => ShowPlaceholder(textBox);

                textBox.TextChanged += (s, args) => ShowPlaceholder(textBox);
                textBox.GotFocus += (s, args) => ShowPlaceholder(textBox);
                textBox.LostFocus += (s, args) => ShowPlaceholder(textBox);
                //textBox.PreviewKeyDown += (s, args) => ShowPlaceholder(textBox);

                DataObject.AddPastingHandler(textBox, (s, args) =>
                {
                    // Delay call to allow paste to complete
                    textBox.Dispatcher.BeginInvoke(new Action(() => ShowPlaceholder(textBox)));
                });
            }
        }

        private static void ShowPlaceholder(TextBox textBox)
        {
            if (string.IsNullOrEmpty(textBox.Text))
            {
                if (VisualTreeHelper.GetChildrenCount(textBox) == 0)
                    return;

                var grid = VisualTreeHelper.GetChild(textBox, 0) as Grid;
                if (grid == null) return;
                TextBlock existingPlaceholder = null;
                foreach (var child in grid.Children)
                {
                    if (child is TextBlock tb && tb.Name == "PlaceholderTextBlock")
                    {
                        existingPlaceholder = tb;
                        break;
                    }
                }
                if (existingPlaceholder == null)
                {
                    var placeholder = new TextBlock
                    {
                        Name = "PlaceholderTextBlock",
                        Text = GetPlaceholder(textBox),
                        Margin = new Thickness(5, 0, 0, 0),
                        FontSize = 16,
                        FontWeight = FontWeights.DemiBold,
                        FontStyle = FontStyles.Italic,
                        Foreground = Brushes.Gray,
                        VerticalAlignment = VerticalAlignment.Center,
                        IsHitTestVisible = false
                    };
                    grid.Children.Add(placeholder);
                }
                else
                {
                    if (!string.IsNullOrEmpty(textBox.Text))
                        grid.Children.Remove(existingPlaceholder);
                }
            }
            else
            {
                RemovePlaceholder(textBox);
            }
        }

        private static void RemovePlaceholder(TextBox textBox)
        {
            if (VisualTreeHelper.GetChildrenCount(textBox) == 0)
                return;

            var grid = VisualTreeHelper.GetChild(textBox, 0) as Grid;
            if (grid == null) return;
            TextBlock existingPlaceholder = null;
            foreach (var child in grid.Children)
            {
                if (child is TextBlock tb && tb.Name == "PlaceholderTextBlock")
                {
                    existingPlaceholder = tb;
                    break;
                }
            }
            if (existingPlaceholder != null)
            {
                grid.Children.Remove(existingPlaceholder);
            }
        }
    }
}
