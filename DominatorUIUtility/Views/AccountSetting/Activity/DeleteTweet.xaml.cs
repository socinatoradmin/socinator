using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for DeleteTweet.xaml
    /// </summary>
    public partial class DeleteTweet : UserControl
    {
        public DeleteTweet(IDeleteTweetViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}