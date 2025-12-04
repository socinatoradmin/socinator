using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for TweetTo.xaml
    /// </summary>
    public partial class TweetTo : UserControl
    {
        public TweetTo(ITweetToViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}