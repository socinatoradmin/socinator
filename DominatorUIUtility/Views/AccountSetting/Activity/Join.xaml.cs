using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for Join.xaml
    /// </summary>
    public partial class Join : UserControl
    {
        public Join(IJoinViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}