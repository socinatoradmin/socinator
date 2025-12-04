using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for MarketPlaceScraper.xaml
    /// </summary>
    public partial class MarketPlaceScraper : UserControl
    {
        public MarketPlaceScraper(IMarketPlaceScraperViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}