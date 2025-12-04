using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for SalesNavigatorCompanyScraper.xaml
    /// </summary>
    public partial class SalesNavigatorCompanyScraper : UserControl
    {
        public SalesNavigatorCompanyScraper(ISalesNavigatorCompanyScraperViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}