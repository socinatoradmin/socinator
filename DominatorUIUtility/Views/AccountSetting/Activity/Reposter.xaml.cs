using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for Reposter.xaml
    /// </summary>
    public partial class Reposter : UserControl
    {
        public Reposter(IReposterViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}