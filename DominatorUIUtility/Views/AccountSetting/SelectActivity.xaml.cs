using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup;

namespace DominatorUIUtility.Views.AccountSetting
{
    /// <summary>
    ///     Interaction logic for SelectActivity.xaml
    /// </summary>
    public partial class SelectActivity : UserControl
    {
        public SelectActivity(ISelectActivityViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}