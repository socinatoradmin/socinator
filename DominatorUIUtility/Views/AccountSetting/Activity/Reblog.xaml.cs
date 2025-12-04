using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for Reblog.xaml
    /// </summary>
    public partial class Reblog : UserControl
    {
        public Reblog(IReblogViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}