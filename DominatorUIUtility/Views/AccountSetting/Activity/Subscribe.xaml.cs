using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for Subscribe.xaml
    /// </summary>
    public partial class Subscribe : UserControl
    {
        public Subscribe(ISubscribeViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}