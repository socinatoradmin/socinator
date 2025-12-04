using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for FanpageScraper.xaml
    /// </summary>
    public partial class FanpageScraper : UserControl
    {
        public FanpageScraper(IFanpageScraperViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}