using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for Unlike.xaml
    /// </summary>
    public partial class Unlike : UserControl
    {
        public Unlike(IUnlikeViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}