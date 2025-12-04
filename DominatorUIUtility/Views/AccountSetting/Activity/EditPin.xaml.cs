using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for EditPin.xaml
    /// </summary>
    public partial class EditPin : UserControl
    {
        public EditPin(IEditPinViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}