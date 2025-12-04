using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for FollowBack.xaml
    /// </summary>
    public partial class FollowBack : UserControl
    {
        public FollowBack(IFollowBackViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}