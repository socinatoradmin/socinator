using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for Unsubscribe.xaml
    /// </summary>
    public partial class UnSubscribe : UserControl
    {
        public UnSubscribe(IUnsubscribeViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}