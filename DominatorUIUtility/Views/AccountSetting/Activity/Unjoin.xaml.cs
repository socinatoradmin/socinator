using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for Unjoin.xaml
    /// </summary>
    public partial class Unjoin : UserControl
    {
        public Unjoin(IUnjoinViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}