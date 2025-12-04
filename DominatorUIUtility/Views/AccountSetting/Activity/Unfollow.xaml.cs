using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for Unfollow.xaml
    /// </summary>
    public partial class Unfollow : UserControl
    {
        public Unfollow(IUnFollowerViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}