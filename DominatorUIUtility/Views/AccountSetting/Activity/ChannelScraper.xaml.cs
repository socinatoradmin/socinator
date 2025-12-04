using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for ChannelScraper.xaml
    /// </summary>
    public partial class ChannelScraper : UserControl
    {
        public ChannelScraper(IChannelScraperViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}