using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for BlockUser.xaml
    /// </summary>
    public partial class BlockUser : UserControl
    {
        public BlockUser(IBlockUserViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}