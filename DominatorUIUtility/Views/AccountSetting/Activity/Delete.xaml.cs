using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for Delete.xaml
    /// </summary>
    public partial class Delete : UserControl
    {
        public Delete(IDeleteViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}