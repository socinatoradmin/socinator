using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for Retweet.xaml
    /// </summary>
    public partial class Retweet : UserControl
    {
        public Retweet(IRetweetViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}