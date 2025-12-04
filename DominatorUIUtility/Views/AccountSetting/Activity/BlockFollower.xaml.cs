using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for BlockFollower.xaml
    /// </summary>
    public partial class BlockFollower : UserControl
    {
        public BlockFollower(IBlockFollowerViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}