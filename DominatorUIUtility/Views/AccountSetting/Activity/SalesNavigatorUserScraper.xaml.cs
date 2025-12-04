using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for SalesNavigatorUserScraper.xaml
    /// </summary>
    public partial class SalesNavigatorUserScraper : UserControl
    {
        public SalesNavigatorUserScraper(ISalesNavigatorUserScraperViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}