using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for TweetScraper.xaml
    /// </summary>
    public partial class TweetScraper : UserControl
    {
        public TweetScraper(ITweetScraperViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}