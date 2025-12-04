using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for HashtagsScraper.xaml
    /// </summary>
    public partial class HashtagsScraper : UserControl
    {
        public HashtagsScraper(IHashtagsScraperViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}