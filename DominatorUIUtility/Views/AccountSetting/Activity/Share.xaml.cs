using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for Share.xaml
    /// </summary>
    public partial class Share : UserControl
    {
        public Share(IShareViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}