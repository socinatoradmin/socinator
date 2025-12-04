using System.Windows;
using System.Windows.Controls;

namespace DominatorUIUtility.Behaviours
{
    public class FixedWidthGridViewColumn : GridViewColumn
    {
        public static readonly DependencyProperty FixedWidthProperty = DependencyProperty.Register("FixedWidth",
            typeof(double), typeof(FixedWidthGridViewColumn),
            new FrameworkPropertyMetadata(double.NaN, OnFixedWidthChanged));

        static FixedWidthGridViewColumn()
        {
            WidthProperty.OverrideMetadata(typeof(FixedWidthGridViewColumn),
                new FrameworkPropertyMetadata(null, OnCoerceWidth));
        }

        public double FixedWidth
        {
            get => (double) GetValue(FixedWidthProperty);

            set => SetValue(FixedWidthProperty, value);
        }

        private static object OnCoerceWidth(DependencyObject o, object baseValue)
        {
            if (o is FixedWidthGridViewColumn fixedWidthGridViewColumn)
                return fixedWidthGridViewColumn.FixedWidth;

            return baseValue;
        }

        private static void OnFixedWidthChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (o is FixedWidthGridViewColumn fixedWidthGridViewColumn)
                fixedWidthGridViewColumn.CoerceValue(WidthProperty);
        }
    }
}