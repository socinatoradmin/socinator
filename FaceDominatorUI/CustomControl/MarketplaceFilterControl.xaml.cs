using FaceDominatorCore.FDModel.FilterModel;
using System.Windows;

namespace FaceDominatorUI.CustomControl
{
    /// <summary>
    ///     Interaction logic for FanpageFilterControl.xaml
    /// </summary>
    public partial class MarketplaceFilterControl
    {
        // Using a DependencyProperty as the backing store for PostFilter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FanpageFilterProperty =
            DependencyProperty.Register("MarketplaceFilterModel", typeof(MarketplaceFilterModel),
                typeof(MarketplaceFilterControl), new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true
                });

        public MarketplaceFilterControl()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
        }


        public MarketplaceFilterModel MarketplaceFilterModel
        {
            get => (MarketplaceFilterModel)GetValue(FanpageFilterProperty);
            set => SetValue(FanpageFilterProperty, value);
        }
    }
}