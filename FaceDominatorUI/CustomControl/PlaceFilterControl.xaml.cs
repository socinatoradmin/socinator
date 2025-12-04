using FaceDominatorCore.FDModel.FilterModel;
using System;
using System.Windows;
using System.Windows.Controls;

namespace FaceDominatorUI.CustomControl
{
    /// <summary>
    ///     Interaction logic for FanpageFilterControl.xaml
    /// </summary>
    public partial class PlaceFilterControl
    {
        // Using a DependencyProperty as the backing store for PostFilter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FdPlaceFilterProperty =
            DependencyProperty.Register("PlaceFilter", typeof(FdPlaceFilterModel), typeof(PlaceFilterControl),
                new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });

        public static readonly DependencyProperty IsSaveCloseButtonVisisbleProperty =
            DependencyProperty.Register("IsSaveCloseButtonVisisble", typeof(bool), typeof(PlaceFilterControl),
                new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });

        public PlaceFilterControl()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
        }


        public FdPlaceFilterModel PlaceFilter
        {
            get => (FdPlaceFilterModel)GetValue(FdPlaceFilterProperty);
            set => SetValue(FdPlaceFilterProperty, value);
        }

        public bool IsSaveCloseButtonVisisble
        {
            get => (bool)GetValue(IsSaveCloseButtonVisisbleProperty);
            set => SetValue(IsSaveCloseButtonVisisbleProperty, value);
        }


        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
        }

        //        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        //        {
        //            SaveUserFilterEventArgsHandler();
        //        }
        //
        //        void SaveUserFilterEventArgsHandler()
        //        {
        //            var rountedargs = new RoutedEventArgs(SaveFanpageFilterEvent);
        //            RaiseEvent(rountedargs);
        //
        //        }

        //        private static readonly RoutedEvent SaveFanpageFilterEvent =
        //            EventManager.RegisterRoutedEvent("SaveGroupFilterEventHandler", RoutingStrategy.Bubble,
        //                typeof(RoutedEventHandler), typeof(FanpageFilterControl));
        //
        //        public event RoutedEventHandler SaveUserFilterEventHandler
        //        {
        //            add { AddHandler(SaveFanpageFilterEvent, value); }
        //            remove { RemoveHandler(SaveFanpageFilterEvent, value); }
        //        }


        private void Combo_Category_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                var selectedvalue = (ComboBox)(FrameworkElement)sender;

                if (selectedvalue != null)
                    PlaceFilter.SelectedPriceRange = selectedvalue.SelectedItem.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}