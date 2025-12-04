using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for Try.xaml
    /// </summary>
    public partial class Try : UserControl
    {
        public Try(ITryViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}