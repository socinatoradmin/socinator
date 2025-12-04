using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for DeletePin.xaml
    /// </summary>
    public partial class DeletePin : UserControl
    {
        public DeletePin(IDeletePinViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}