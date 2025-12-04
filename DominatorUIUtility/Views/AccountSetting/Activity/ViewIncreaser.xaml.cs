using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for ViewIncreaser.xaml
    /// </summary>
    public partial class ViewIncreaser : UserControl
    {
        public ViewIncreaser(IViewIncreaserViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}