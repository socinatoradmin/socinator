using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for Repin.xaml
    /// </summary>
    public partial class Repin : UserControl
    {
        public Repin(IRepinViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}