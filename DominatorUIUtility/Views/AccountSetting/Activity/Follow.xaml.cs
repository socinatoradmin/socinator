using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for Follow.xaml
    /// </summary>
    public partial class Follow : UserControl
    {
        public Follow(IFollowViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}