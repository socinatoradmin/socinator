using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for Tweet.xaml
    /// </summary>
    public partial class Tweet : UserControl
    {
        public Tweet(ITweetViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}